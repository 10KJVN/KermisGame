using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private int points = 100;

    public void AwardPoints()
    {
        ScoreManager.Instance?.AddScore(points);
    }
}
