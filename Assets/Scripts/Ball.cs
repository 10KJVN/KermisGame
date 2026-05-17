using System;
using UnityEngine;

public class Ball : MonoBehaviour, IThrowable
{
    public const float MaxForce = 50f;
    
    [SerializeField] private Transform forceTransform;
    private SpriteMask forceSpriteMask;

    private void Awake()
    {
        forceSpriteMask = GetComponentInChildren<SpriteMask>();
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