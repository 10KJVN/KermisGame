using UnityEngine;

public class HitReaction : MonoBehaviour, IHittable, IResettable
{
    [Header("Scoring")]
    [SerializeField] private int points = 100;

    [Header("VFX & SFX")]
    [SerializeField] private GameObject hitVFXPrefab;   // optional
    [SerializeField] private AudioClip hitSFX;          // optional
    [SerializeField] private float sfxVolume = 1f;

    [Header("Behaviour")]
    [SerializeField] private bool deactivateAfterHit = true;

    public void OnHit(Collision collision, Ball ball)
    {
        // Award points
        ScoreManager.Instance?.AddScore(points);

        // Spawn VFX at impact point
        if (hitVFXPrefab != null)
        {
            Instantiate(hitVFXPrefab, collision.contacts[0].point, Quaternion.identity);
        }

        // Play SFX
        if (hitSFX != null)
        {
            AudioSource.PlayClipAtPoint(hitSFX, transform.position, sfxVolume);
        }

        // Deactivate (or destroy) the target
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