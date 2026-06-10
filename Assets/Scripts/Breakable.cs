using UnityEngine;

public class Breakable : MonoBehaviour, IHittable, IResettable
{
    [SerializeField] private GameObject _replacement;
    [SerializeField] private AudioClip _breakClip;
    [SerializeField] private float _breakForce = 2;
    [SerializeField] private float _collisionMultiplier = 100;
    [SerializeField] private bool _broken;

    // Optional: use a HitReaction for scoring & effects
    [SerializeField] private HitReaction _hitReaction;

    public void OnHit(Collision collision, Ball ball)
    {
        if (_broken) return;
        if (collision.relativeVelocity.magnitude >= _breakForce)
        {
            // If a HitReaction is present, let it handle scoring + VFX/SFX
            if (_hitReaction != null)
            {
                _hitReaction.OnHit(collision, ball);
            }
            else
            {
                ScoreManager.Instance?.AddScore(100); // fallback
            }

            _broken = true;
            var replacement = Instantiate(_replacement, transform.position, transform.rotation);

            var rbs = replacement.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs)
            {
                rb.AddExplosionForce(collision.relativeVelocity.magnitude * _collisionMultiplier,
                    collision.contacts[0].point, 2);
            }

            // If you have an AudioSource on this object, play break sound
            AudioSource.PlayClipAtPoint(_breakClip, transform.position);

            replacement.GetComponent<ObjectDestroyer>()?.DestroySelf();
            gameObject.SetActive(false);
        }
    }

    public void ResetTarget()
    {
        _broken = false;
        gameObject.SetActive(true);
    }
}