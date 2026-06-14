using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ImprovedTimers;

public class ShootingTimerController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject[] uiElementsToHide;

    [Header("Timer Settings")]
    [SerializeField] private float roundDuration = 30f;

    [Header("Audio")]
    [SerializeField] private AudioClip alarmClip;
    [SerializeField] private float alarmVolume = 1f;

    [Header("External References")]
    [SerializeField] private GameModeManager gameModeManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private HoldReleaseOscillator holdReleaseOscillator;

    private CountdownTimer _timer;
    private Camera _mainCamera;

    private void Awake()
    {
        if (endScreenPanel) endScreenPanel.SetActive(false);
        if (retryButton) retryButton.onClick.AddListener(OnRetryClicked);
        if (continueButton) continueButton.onClick.AddListener(OnContinueClicked);
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    public void StartNewRound()
    {
        if (_timer != null)
        {
            _timer.OnTimerStop -= OnTimerFinished;
            _timer.Dispose();
        }
        
        if (holdReleaseOscillator != null)
        {
            holdReleaseOscillator.ResetCharge();
            holdReleaseOscillator.enabled = true;   // ensure it's active
        }
        
        _timer = new CountdownTimer(roundDuration);
        _timer.OnTimerStop += OnTimerFinished;
        _timer.Start();
        
        if (endScreenPanel) endScreenPanel.SetActive(false);
        foreach (GameObject uiElem in uiElementsToHide)
            if (uiElem) uiElem.SetActive(true);
        if (timerText) timerText.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_timer is not { IsRunning: true }) return;
        
        if (timerText)
        {
            float remaining = _timer.CurrentTime;
            timerText.text = $"TIME: {Mathf.CeilToInt(remaining)}";
        }
    }

    private void OnTimerFinished()
    {
        if (alarmClip && _mainCamera)
            AudioSource.PlayClipAtPoint(alarmClip, _mainCamera.transform.position, alarmVolume);
        
        if (endScreenPanel)
        {
            int finalScore = scoreManager ? scoreManager.GetScore() : 0;
            if (finalScoreText) finalScoreText.text = $"FINAL SCORE: {finalScore}";
            endScreenPanel.SetActive(true);
        }
        
        // Force hide trajectory line
        if (holdReleaseOscillator != null)
        {
            holdReleaseOscillator.ResetCharge();
            holdReleaseOscillator.enabled = false;
        }
        
        foreach (GameObject uiElem in uiElementsToHide)
        {
            if (uiElem) uiElem.SetActive(false);
        }
        if (timerText) timerText.gameObject.SetActive(false);

        // Disable shooting input
        if (holdReleaseOscillator) holdReleaseOscillator.enabled = false;
    }

    private void OnRetryClicked()
    {
        if (gameModeManager)
        {
            gameModeManager.EnterShooting();
        }
        else
        {
            endScreenPanel?.SetActive(false);
            StartNewRound();
        }
    }

    private void OnContinueClicked()
    {
        if (gameModeManager)
            gameModeManager.EnterExploration();
        else
            endScreenPanel?.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_timer != null)
        {
            _timer.OnTimerStop -= OnTimerFinished;
            _timer.Dispose();
        }
    }
}