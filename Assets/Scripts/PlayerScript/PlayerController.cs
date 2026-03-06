using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference jumpAction;

    public float moveSpeed = 5f;

    public float mouseSensitivity = 0.15f;

    private CharacterController _cc;
    private Transform _cameraTransform;
    private float _verticalRotation = 0f;
    private float _verticalVelocity = 0f;
    private float _gravity = -9.81f;
    private float jumpForce = 5f;
    private Rigidbody rb;

    void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        jumpAction.action.Enable();
        jumpAction.action.performed += OnJump;
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        jumpAction.action.Disable();
        jumpAction.action.performed -= OnJump;
    }

    void Start()
    {
        _cc = GetComponent<CharacterController>();
        _cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();

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
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (_cc.isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

    }
}