using UnityEngine;
using ImprovedTimers;
using TMPro;

public class CountdownMediator : MonoBehaviour
{
    [SerializeField] private float duration = 30f;
    [SerializeField] private TMP_Text uiText;
    [SerializeField] private AudioClip alarmClip;
    [SerializeField] private GameObject endScreen;

    private CountdownTimer _timer;
    private Camera _mainCamera;

    private void Start()
    {
        _timer = new CountdownTimer(duration);
        _timer.OnTimerStop += OnTimerFinished;
        _timer.Start();
        
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_timer == null) return;
        
        // Update UI every frame
        if (uiText)
            uiText.text = $"TIME: {Mathf.CeilToInt(_timer.CurrentTime)}";
    }

    private void OnTimerFinished()
    {
        // Play alarm sound
        if (alarmClip != null)
            AudioSource.PlayClipAtPoint(alarmClip, _mainCamera.transform.position);
        
        // Show your end screen (you said you'll manage this)
        if (endScreen != null)
            endScreen.SetActive(true);
        
        // Optional: stop the ball / freeze game
        // GameModeManager.EnterExploration() etc.
    }

    private void OnDestroy()
    {
        _timer?.Dispose();
    }
}