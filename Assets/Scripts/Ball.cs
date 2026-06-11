using UnityEngine;

public class Ball : MonoBehaviour, IThrowable
{
    public const float MaxForce = 50f;
    
    private Transform forceTransform;
    private SpriteMask forceSpriteMask;

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private GameObject _poofPrefab;
    private bool _isGhost;

    private void Awake()
    {
        forceSpriteMask = GetComponentInChildren<SpriteMask>();
    }

    public void Init(Vector3 velocity, bool isGhost)
    {
        _isGhost = isGhost;
        _rb.AddForce(velocity, ForceMode.Impulse);
    }
    
    public void OnCollisionEnter(Collision col)
    {
        if (_isGhost) return;

        // Notify the hit object
        IHittable hittable = col.gameObject.GetComponent<IHittable>();
        hittable?.OnHit(col, this);

        // Ball death effects (poof, sound, destroy)
        Instantiate(_poofPrefab, col.contacts[0].point, Quaternion.Euler(col.contacts[0].normal));
        _source.clip = _clips[Random.Range(0, _clips.Length)];
        _source.Play();
        GetComponent<ObjectDestroyer>()?.DestroySelf();
    }

    // Just launches in the direction of your POV.
    // So probably the forward vector of the found camera.
    public void Launch(float force)
    {
        Vector3 dir = (MouseUtils.GetMouseWorldPositionWithZ() - transform.position).normalized * -1f;
        transform.GetComponent<Rigidbody>().linearVelocity = dir * force;
        HideForce();
    }

    public void ShowForce(float force)
    {
        forceSpriteMask.alphaCutoff = 1 - force / MaxForce;
    }
    
    private void HideForce() 
    {
        forceSpriteMask.alphaCutoff = 1;
    }
}