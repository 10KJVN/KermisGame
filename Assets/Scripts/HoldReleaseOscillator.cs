using UnityEngine;
using UnityEngine.Events;

public class HoldReleaseOscillator : MonoBehaviour
{
    [Header("Power Settings")]
    public float minPower = 0.2f;
    public float maxPower = 1.0f;
    public float oscillationSpeed = 2f;

    [Header("Launch Settings")]
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    public float baseForce = 20f;
    public enum AimMode { FixedForward, CameraForward, MouseWorld }
    public AimMode aimMode = AimMode.CameraForward;
    public Vector3 fixedDirection = Vector3.forward;
    
    [Header("Trajectory Settings")]
    public LineRenderer trajectoryLine;
    public int trajectoryResolution = 30;
    public float timeStep = 0.05f;

    [Header("Events (for UI, sound, etc.)")]
    public UnityEvent<float> onPowerChanged;
    public UnityEvent<float> onRelease;

    private bool _isCharging = false;
    private float _chargeStartTime;
    private float _currentPower = 0.5f;

    private void Update()
    {
        // Mouse Down, start holding
        if (Input.GetMouseButtonDown(0))
        {
            _isCharging = true;
            _chargeStartTime = Time.time;
        }

        // Mouse still down, oscillate power
        if (_isCharging)
        {
            float elapsed = Time.time - _chargeStartTime;
            float t = Mathf.PingPong(elapsed * oscillationSpeed, 1f);
            _currentPower = Mathf.Lerp(minPower, maxPower, t);
            
            UpdateTrajectory(_currentPower);
            
            onPowerChanged?.Invoke(_currentPower);
        }
        else if (!trajectoryLine && trajectoryLine.enabled)
        {
            trajectoryLine.enabled = false;
        }
    
        // Mouse Up, Launch!
        if (Input.GetMouseButtonUp(0) && _isCharging)
        {
            _isCharging = false;
            onRelease?.Invoke(_currentPower);
            LaunchProjectile(_currentPower);
        }
    }

    private void LaunchProjectile(float power)
    {
        if (!projectilePrefab || !spawnPoint) return;

        GameObject proj = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (!rb) return;
        
        Vector3 direction = GetAimDirection();
        float forceMagnitude = baseForce * power;
        rb.linearVelocity = direction * forceMagnitude;
    }

    private Vector3 GetAimDirection()
    {
        switch (aimMode)
        {
            case AimMode.FixedForward:
                return fixedDirection.normalized;
            
            case AimMode.CameraForward:
                return Camera.main!.transform.forward;
            
            case AimMode.MouseWorld:
                Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    return (hit.point - spawnPoint.position).normalized;
                }
                return Vector3.forward;
            
            default:
                return Vector3.forward;
        }
    }
    
    private void UpdateTrajectory(float power)
    {
        if (!trajectoryLine) return;
        trajectoryLine.enabled = true;

        Vector3 startPos = spawnPoint.position;
        Vector3 velocity = GetAimDirection() * (baseForce * power);
        Vector3 gravity = Physics.gravity;

        Vector3[] points = new Vector3[trajectoryResolution];
        for (int i = 0; i < trajectoryResolution; i++)
        {
            float t = i * timeStep;
            points[i] = startPos + velocity * t + 0.5f * gravity * t * t;
        }
        trajectoryLine.positionCount = trajectoryResolution;
        trajectoryLine.SetPositions(points);
    }
    
}