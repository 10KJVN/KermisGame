using System;
using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    [SerializeField] private GameObject throwPrefab;
    
    [Header("Grenade Force")]
    [SerializeField] private KeyCode throwKey = KeyCode.Mouse0;
    [SerializeField] private Transform throwPosition; // reference to the throw position transform
    [SerializeField] private Vector3 throwDirection = new(0, 1, 0); // direction of the throw
    
    [Header("Grenade Force")]
    [SerializeField] private float throwForce = 10f; // force applied to throw the grenade
    [SerializeField] private float maxForce = 20f; // maximum force applied to throw the grenade 
    
    [Header("Trajectory Settings")]
    [SerializeField] private LineRenderer trajectoryLine; // reference to LineRenderer component

    private bool _isCharging = false; // flag to check if player is charging the throw
    private float chargeTime = 0f; // time player has been charging the throw
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKey(throwKey))
        {
            StartThrowing();
        }

        if (_isCharging)
        {
            ChargeThrow();
        }

        if (Input.GetKeyUp(throwKey))
        {
            ReleaseThrow();
        }
    }

    private void StartThrowing()
    {
        _isCharging = true;
        chargeTime = 0f;
        
        trajectoryLine.enabled = true;
    }

    private void ChargeThrow()
    {
        chargeTime += Time.deltaTime;
        
        Vector3 grenadeVelocity = (_mainCamera.transform.forward + throwDirection).normalized * Mathf.Min(chargeTime * throwForce, maxForce);
        ShowTrajectory(throwPosition.position + throwPosition.forward, grenadeVelocity);
    }

    private void ReleaseThrow()
    {
        ThrowGrenade(Mathf.Min(chargeTime * throwForce, maxForce));
        _isCharging = false;
        // hide line
    }

    private void ThrowGrenade(float force)
    {
        Vector3 spawnPosition = throwPosition.position - _mainCamera.transform.forward;

        GameObject grenade = Instantiate(throwPrefab, spawnPosition, _mainCamera.transform.rotation);
        
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        
        Vector3 finalThrowDirection = (_mainCamera.transform.forward + throwDirection).normalized;
        rb.AddForce(finalThrowDirection * force, ForceMode.VelocityChange);
        
        // Throwing sound
    }
    
    private void ShowTrajectory(Vector3 origin, Vector3 speed)
    {
        Vector3[] points = new Vector3[100];
        trajectoryLine.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            float time = i * 0.1f;
            // points[i] = origin + speed * time + 0.5f * Physics.gravity * time * time;
            points[i] = origin + speed * time + Physics.gravity * (0.5f * time * time);
        }
        trajectoryLine.SetPositions(points);
        // Displacement = initial velocity * time + 0.5f * acceleration * time ^ 2
    }
    
    
}
