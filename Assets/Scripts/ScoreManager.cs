using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private TMP_Text scoreText;

    private int _score;

    private void Awake()
    {
        Instance = this;
    }

    public void ResetScore()
    {
        _score = 0;
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        _score += amount;
        UpdateUI();
    }

    public int GetScore()
    {
        return _score;
    }

    private void UpdateUI()
    {
        if (scoreText)
            scoreText.text = $"SCORE: {_score}";
    }
}