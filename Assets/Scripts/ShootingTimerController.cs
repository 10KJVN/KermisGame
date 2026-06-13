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

    [Header("Timer Settings")]
    [SerializeField] private float roundDuration = 30f;

    [Header("Audio")]
    [SerializeField] private AudioClip alarmClip;
    [SerializeField] private float alarmVolume = 1f;

    [Header("External References")]
    [SerializeField] private GameModeManager gameModeManager;
    [SerializeField] private ScoreManager scoreManager;

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

    // Called by GameModeManager when entering shooting mode
    public void StartNewRound()
    {
        if (_timer != null)
        {
            _timer.OnTimerStop -= OnTimerFinished;
            _timer.Dispose();
        }
        
        _timer = new CountdownTimer(roundDuration);
        _timer.OnTimerStop += OnTimerFinished;
        _timer.Start();
        
        if (endScreenPanel) endScreenPanel.SetActive(false);
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

        // TODO: disable shooting input (handled by GameModeManager)
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