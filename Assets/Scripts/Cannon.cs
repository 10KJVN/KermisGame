using System;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private Projection _projection;
    [SerializeField] private Ball _ballPrefab;
    [SerializeField] private Transform _ballSpawn;
    [SerializeField] private float _force = 20f;

    private void Update()
    {
        HandleControls();
        
        _projection.SimulateTrajectory(_ballPrefab, _ballSpawn.position, _ballSpawn.forward * _force);
    }

    private void HandleControls()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var spawned = Instantiate(_ballPrefab, _ballSpawn.position, _ballSpawn.rotation);
            
            spawned.Init(_ballSpawn.forward * _force, false);
            // Launch Particles
            // SFX
        }
    }
}