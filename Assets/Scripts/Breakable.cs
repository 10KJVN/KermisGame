using UnityEngine;

public class Breakable : MonoBehaviour, IResettable
{
    [SerializeField] private GameObject _replacement;
    [SerializeField] private AudioClip _breakClip;
    [SerializeField] private int points = 100;
    [SerializeField] private float _breakForce = 2;
    [SerializeField] private float _collisionMultiplier = 100;
    [SerializeField] private bool _broken;

    public void OnCollisionEnter(Collision collision)
    {
        if (_broken) return;
        if (collision.relativeVelocity.magnitude >= _breakForce)
        {
            ScoreManager.Instance?.AddScore(points);

            _broken = true;
            var replacement = Instantiate(_replacement, transform.position, transform.rotation);

            var rbs = replacement.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs)
            {
                rb.AddExplosionForce(collision.relativeVelocity.magnitude * _collisionMultiplier,
                    collision.contacts[0].point, 2);
            }
            
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