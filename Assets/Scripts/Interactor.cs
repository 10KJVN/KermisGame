using InteractionSystem.Interfaces;
using UnityEngine;

/// <summary>
/// Simple ray‑cast based interactor.
/// Press E to interact with the first IInteractable hit within range.
/// </summary>

public class Interactor : MonoBehaviour
{
    public Transform interactorSource;
    public float interactRange;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray r = new Ray(interactorSource.position, interactorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }
    }
}