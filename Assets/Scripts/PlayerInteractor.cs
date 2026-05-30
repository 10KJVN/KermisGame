using System;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private LayerMask interactableLayers;

    private Collider[] _buffer = new Collider[32];
    private IInteractable _focused;

    private void Update()
    {
        IInteractable nearest = FindNearestInteractable();
        UpdateFocus(nearest);

        if (_focused != null && Input.GetKeyDown(KeyCode.E))
        {
            if (_focused.CanInteract()) _focused.Interact();
        }
    }


    private IInteractable FindNearestInteractable()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, _buffer, interactableLayers,
            QueryTriggerInteraction.Collide);
        IInteractable nearest = null;
        float bestDistSq = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider col = _buffer[i];
            if (col == null) continue;
            IInteractable interactable = col.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;
            if (!interactable.CanInteract()) continue;
            float distSq = (col.transform.position - transform.position).sqrMagnitude;
            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                nearest = interactable;
            }
        }
        return nearest;
    }
    
    private void UpdateFocus(IInteractable nearest)
    {
        if (ReferenceEquals(_focused, nearest)) return;
        _focused?.OnFocusLost();
        _focused = nearest;
        _focused?.OnFocusGained();
    }
}
