using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private float mouseSensitivity = 0.4f;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private float gravity = -9.81f;
    private float _verticalVelocity;
    
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    
    private PlayerInputActions inputActions;
    private CharacterController characterController;
    private InputAction movementAction;
    private InputAction lookAction; 
    private Vector2 movementInput;
    private float xRotation = 0f;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        RaycastHit hit;
        Vector3 startPos = transform.position;

        if (Physics.Raycast(startPos + Vector3.up * 50, Vector3.down, out hit, 100f))
        {
            transform.position = hit.point;
        }
        else
        {
            Debug.LogWarning("No ground found below the player!");
        }
    }

    private void OnEnable()
    {
        EnableInputActions();
    }

    private void OnDisable()
    {
        DisableInputActions();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void EnableInputActions()
    {
        movementAction = inputActions.Player.Move;
        movementAction.Enable();
        movementAction.performed += OnMovementPerformed;
        movementAction.canceled += OnMovementCanceled;
        
        lookAction = inputActions.Player.Look;
        lookAction.Enable();
    }

    private void DisableInputActions()
    {
        movementAction.performed -= OnMovementPerformed;
        movementAction.canceled -= OnMovementCanceled;
        movementAction.Disable();
        
        lookAction.Disable();
    }

    private void HandleMovement()
    {

        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;

        // Apply gravity
        if (characterController.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f; // Small downward force to stay grounded
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
        move.y = _verticalVelocity;

        characterController.Move(move * movementSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
    }
}