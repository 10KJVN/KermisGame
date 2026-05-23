using System;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private GameObject hitPrefab;

    private void Start()
    {
    }

    private void Update()
    {
    }
    
    // Should do something like on Collision Enter/Trigger for a Hit() func
    // SFX
    // Affect other Physics Objects
    // Destroy GameObject
    
    // PlaySoundAtPosition(AudioClip clip)
    // Spatial blend functionality etc

    void ApplyForce()
    {
        var someRadius = 1f; // Placeholder var
        Collider[] colliders = Physics.OverlapSphere(transform.position, someRadius);
        
        foreach (Collider nearbyObjects in colliders)
        {
            Rigidbody rb = nearbyObjects.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // rb.AddForce() 
            }
        }
    }
}