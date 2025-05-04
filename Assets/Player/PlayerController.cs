using SoundManager;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 0.4f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float footstepInterval = 0.4f;
    private float footstepTimer;
    private float _verticalVelocity;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerInteraction playerInteraction;
    
    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    
    private bool isWalking = false;
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
        if (playerInteraction != null)
        {
            playerInteraction.SetInteractAction(inputActions.Player.Interact);
        }
    
    }

    private void OnEnable()
    {
        movementAction = inputActions.Player.Move;
        lookAction = inputActions.Player.Look;

        movementAction.Enable();
        lookAction.Enable();

        movementAction.performed += OnMovementPerformed;
        movementAction.canceled += OnMovementCanceled;
    }

    private void OnDisable()
    {
        movementAction.Disable();
        lookAction.Disable();

        movementAction.performed -= OnMovementPerformed;
        movementAction.canceled -= OnMovementCanceled;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;

        
        
        if (characterController.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }

        move.y = _verticalVelocity;
        characterController.Move(move * movementSpeed * Time.deltaTime);
        
        Vector3 horizontalMove = new Vector3(move.x, 0f, move.z);
        isWalking = horizontalMove.magnitude > 0.1f;


        if (characterController.isGrounded && isWalking)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                SoundManager.SoundManager.PlaySound(SoundType.Footstep, audioSource);
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f; 
        }
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
