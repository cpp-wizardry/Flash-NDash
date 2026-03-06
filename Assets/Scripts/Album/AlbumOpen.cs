using UnityEngine;
using UnityEngine.InputSystem;

public class AlbumInput : MonoBehaviour
{
    public InputActionReference openAlbumAction;
    public PhotoAlbum photoAlbum;
    public PlayerController playerController;
    public PhotoMode photoMode;

    private bool _albumOpen = false;

    void OnEnable()
    {
        openAlbumAction.action.Enable();
        openAlbumAction.action.performed += OnToggleAlbum;
    }

    void OnDisable()
    {
        openAlbumAction.action.performed -= OnToggleAlbum;
        openAlbumAction.action.Disable();
    }

    private void OnToggleAlbum(InputAction.CallbackContext ctx)
    {
        _albumOpen = !_albumOpen;

        photoAlbum.ToggleAlbum();

        playerController.enabled = !_albumOpen;

        if (photoMode != null)
            photoMode.enabled = !_albumOpen;

        Cursor.lockState = _albumOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible   = _albumOpen;
    }
}