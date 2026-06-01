using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MinigameTimer : MonoBehaviour
{
    public UnityEvent onTimerFinished;
    
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float duration = 30f;

    private float _remainingTime;
    private bool _running;

    private void Update()
    {
        if (!_running) return;

        _remainingTime -= Time.deltaTime;

        if (_remainingTime <= 0)
        {
            _remainingTime = 0;
            _running = false;

            onTimerFinished?.Invoke();
        }

        UpdateUI();
    }

    public void StartTimer()
    {
        _remainingTime = duration;
        _running = true;
        UpdateUI();
    }

    public void StopTimer()
    {
        _running = false;
    }

    private void UpdateUI()
    {
        if (timerText)
            timerText.text = $"TIME: {Mathf.CeilToInt(_remainingTime)}";
    }
}