using UnityEngine;

public class HitReaction : MonoBehaviour, IHittable, IResettable
{
    [Header("Scoring")]
    [SerializeField] private int points = 100;

    [Header("Visual & Sound Effects")]
    [SerializeField] private GameObject hitVFXPrefab; // optional
    [SerializeField] private AudioClip hitSFX; // optional
    [SerializeField] private float sfxVolume = 1f;

    [Header("Behaviour")]
    [SerializeField] private bool deactivateAfterHit = true;

    public void OnHit(Collision collision, Ball ball)
    {
        ScoreManager.Instance?.AddScore(points);
        
        if (hitVFXPrefab)
        {
            Instantiate(hitVFXPrefab, collision.contacts[0].point, Quaternion.identity);
        }
        
        if (hitSFX)
        {
            AudioSource.PlayClipAtPoint(hitSFX, transform.position, sfxVolume);
        }
        
        if (deactivateAfterHit)
        {
            gameObject.SetActive(false);
        }
    }

    // Used by ShootingSession to reset
    public void ResetTarget()
    {
        gameObject.SetActive(true);
    }
}