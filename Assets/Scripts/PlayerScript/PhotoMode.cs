using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PhotoMode : MonoBehaviour
{
    public InputActionReference togglePhotoModeAction;
    public InputActionReference takePhotoAction;

    public GameObject photoModeUI;
    public Camera playerCamera;
    public float raycastDistance = 50f;

    public UnityEvent<PhotoScoreResult> onPhotoScored;

    public bool IsInPhotoMode { get; private set; } = false;

    private NPCReaction _currentAimedNPC;

    void OnEnable()
    {
        togglePhotoModeAction.action.Enable();
        takePhotoAction.action.Enable();
        togglePhotoModeAction.action.performed += OnTogglePhotoMode;
        takePhotoAction.action.performed += OnTakePhoto;
    }

    void OnDisable()
    {
        togglePhotoModeAction.action.performed -= OnTogglePhotoMode;
        takePhotoAction.action.performed -= OnTakePhoto;
        togglePhotoModeAction.action.Disable();
        takePhotoAction.action.Disable();
    }

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (photoModeUI != null)
            photoModeUI.SetActive(false);
    }

    void Update()
    {
        if (!IsInPhotoMode)
        {
            ClearAimedNPC();
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            NPCReaction reaction = hit.collider.GetComponentInParent<NPCReaction>();
            if (reaction != _currentAimedNPC)
            {
                ClearAimedNPC();
                _currentAimedNPC = reaction;
                _currentAimedNPC?.OnAimedAt();
            }
        }
        else
        {
            ClearAimedNPC();
        }
    }

    private void OnTogglePhotoMode(InputAction.CallbackContext ctx)
    {
        IsInPhotoMode = !IsInPhotoMode;

        if (photoModeUI != null)
            photoModeUI.SetActive(IsInPhotoMode);

        if (!IsInPhotoMode)
            ClearAimedNPC();
    }

    private void OnTakePhoto(InputAction.CallbackContext ctx)
    {
        if (!IsInPhotoMode) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            return;
        }

        hit.collider.GetComponentInParent<NPCReaction>()?.OnPhotographed();

        var (questValidated, questPoints) = QuestManager.Instance?.OnPhotoTaken(hit) ?? (false, 0);

        PhotoScoreResult result = PhotoScorer.Instance?.Evaluate(hit, null, questValidated, questPoints);

        if (result != null)
        {
            QuestManager.Instance?.AddPoints(result.total);
            PhotoSaver.Instance?.SavePhoto(result);
            onPhotoScored?.Invoke(result);

        }
    }

    private void ClearAimedNPC()
    {
        _currentAimedNPC?.OnAimCleared();
        _currentAimedNPC = null;
    }
}