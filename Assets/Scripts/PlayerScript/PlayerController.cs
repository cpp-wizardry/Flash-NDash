using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference lookAction;

    public float moveSpeed = 5f;
    public float mouseSensitivity = 0.15f;

    private CharacterController _cc;
    private Transform _cameraTransform;
    private Animator _animator;

    private float _verticalRotation = 0f;
    private float _verticalVelocity = 0f;
    private float _gravity = -9.81f;
    private string _currentClip = "";

    void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
    }

    void Start()
    {
        _cc = GetComponent<CharacterController>();
        _cameraTransform = Camera.main.transform;
        _animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        Vector2 look = lookAction.action.ReadValue<Vector2>();
        transform.Rotate(Vector3.up * look.x * mouseSensitivity);
        _verticalRotation -= look.y * mouseSensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -80f, 80f);
        _cameraTransform.localEulerAngles = new Vector3(_verticalRotation, 0f, 0f);
    }

    void Move()
    {
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        Vector3 direction = transform.right * move.x + transform.forward * move.y;
        _cc.Move(direction * moveSpeed * Time.deltaTime);

        if (_cc.isGrounded)
            _verticalVelocity = -1f;
        else
            _verticalVelocity += _gravity * Time.deltaTime;

        _cc.Move(Vector3.up * _verticalVelocity * Time.deltaTime);

        PlayClip(move.magnitude > 0.1f ? "Walk" : "Idle");
    }

    private void PlayClip(string clipName)
    {
        if (_animator == null || clipName == _currentClip) return;
        _currentClip = clipName;
        _animator.SetTrigger(clipName);
    }
}