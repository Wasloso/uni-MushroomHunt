using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public LayerMask interactableLayer;

    [Header("Interaction Settings")]
    public float interactionDistance = 2f;
    public float holdTime = 1f;
    
    private CollectiblesManager collectiblesManager;
    private IInteractable currentInteractable;
    private float interactTimer = 0f;
    private InputAction interactAction;

    private void Start()
    {
        collectiblesManager = FindFirstObjectByType<CollectiblesManager>();
    }

    void Update()
    {
        InteractionRay();

    }

    private void InteractionRay()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green);
        if (Physics.SphereCast(ray, 0.1f, out RaycastHit hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                if (currentInteractable != null && currentInteractable != interactable)
                    currentInteractable.setActive(false);
                currentInteractable = interactable;
                currentInteractable.setActive(true);
                return;
            }
        }
        currentInteractable?.setActive(false);
        currentInteractable = null;
        

    }

    private void Interact(InputAction.CallbackContext context)
    {
        switch (currentInteractable)
        {
            case null:
                return;
            case Mushroom:
                collectiblesManager?.AddMushroom();
                break;
        }

        currentInteractable.Interact();
    }
    
    public void SetInteractAction(InputAction action)
    {
        interactAction = action;
        interactAction.performed += Interact;
        interactAction.Enable();
    }
}