using UnityEngine;

public interface IInteractable
{
  
    string GetInteractionText();
    void Interact();

    void ShowPopUp();
    void HidePopUp();
    bool isValid();
    
    void setActive(bool isActive);
}