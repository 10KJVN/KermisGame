using UnityEngine;
using TMPro;

/// <summary>
/// World‑space UI prompt that follows the focused interactable.
/// Displays a key hint and the object's DisplayName.
/// </summary>

public class InteractPrompt : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Vector3 worldOffset = new(0f, 1f, 0f);
    [SerializeField] private string keyHint = "[E]";

    private Camera _cam;
    private Transform _target;
    private Canvas _canvas;
    private RectTransform _canvasRect;
    private RectTransform _labelRect;

    private void Awake()
    {
        _cam = Camera.main;
        _labelRect = label.rectTransform;
        _canvas = label.GetComponentInParent<Canvas>();
        _canvasRect = _canvas.GetComponent<RectTransform>();
        Hide();
    }

    private void LateUpdate()
    {
        if (_target == null) return;
        if (!label.gameObject.activeSelf)
        {
            label.gameObject.SetActive(true);
        }
        
        Vector3 worldPos = _target.position  + worldOffset;
        Vector3 screenPos = _cam.WorldToScreenPoint(worldPos);
        Camera uiCam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _cam;
        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPos, uiCam, 
                out Vector2 localPoint))
        {
            _labelRect.anchoredPosition = localPoint;
        }
    }

    public void Show(IInteractable interactable)
    {
        if (interactable == null)
        {
            Hide();
            return;
        }
        
        _target = interactable.transform;
        label.text = $"{keyHint}{interactable.DisplayName}";
        label.gameObject.SetActive(true);
    }

    public void Hide()
    {
        label.gameObject.SetActive(false);
        _target = null;
    }
}
