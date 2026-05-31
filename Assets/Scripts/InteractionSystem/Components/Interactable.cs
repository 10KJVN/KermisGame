using InteractionSystem.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace InteractionSystem.Components
{
    /// <summary>
    /// This is the component you place on ALL interactable objects.
    /// </summary>
    
    public class Interactable : MonoBehaviour, IInteractable
    {
        public string DisplayName => displayName;
        public bool CanInteract() => isEnabled;
        
        [SerializeField] private string displayName = "Interactable";
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private UnityEvent OnInteract;
        
        private Outline _outline;

        private void Awake()
        {
            _outline = gameObject.AddComponent<Outline>();
            _outline.OutlineMode = Outline.Mode.OutlineVisible;
            _outline.OutlineColor = Color.yellow;
            _outline.OutlineWidth = 1f;
            _outline.enabled = false;
        }

        public void Interact()
        {
            OnInteract?.Invoke();
        }

        public void OnFocusGained()
        {
            _outline.enabled = true;
        }

        public void OnFocusLost()
        {
            _outline.enabled = false;
        }
    }
}