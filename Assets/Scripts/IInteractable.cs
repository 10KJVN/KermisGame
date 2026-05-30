using UnityEngine;

/// <summary>
/// Interface for any object that can be interacted with.
/// Requires display name, interaction method, and focus callbacks.
/// </summary>

public interface IInteractable
{
    Transform transform { get; }
    string DisplayName { get; }
    
    bool CanInteract();
    void Interact();
    void OnFocusGained();
    void OnFocusLost();
}