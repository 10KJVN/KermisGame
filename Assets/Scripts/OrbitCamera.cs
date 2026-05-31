using UnityEngine;

/// <summary>
/// Orbital camera that rotates around a focus target.
/// Features:
/// - Manual rotation via input axes (Vertical/Horizontal Camera)
/// - Automatic rotation that aligns with the target's movement direction
/// - Smooth focus point following with radius and centering
/// - Obstruction avoidance using box casting
/// - Mouse wheel zoom with configurable range and speed
/// </summary>

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform focus;
    [SerializeField, Range(1f, 120f)] private float distance = 7f;
    [SerializeField] private Vector2 initialAngles = new(45f, 0f);

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 20f;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 15f;

    [Header("Focus Point")]
    [SerializeField, Min(0f)] private float focusRadius = 5f;
    [SerializeField, Range(0f, 1f)] private float focusCentering = 0.5f;

    [Header("Rotation")]
    [SerializeField, Range(1f, 360f)] private float rotationSpeed = 90f;
    [SerializeField, Range(-89f, 89f)] private float minVerticalAngle = -30f;
    [SerializeField, Range(-89f, 89f)] private float maxVerticalAngle = 60f;

    [Header("Automatic Rotation")]
    [SerializeField, Min(0f)] private float alignDelay = 5f;
    [SerializeField, Range(0f, 90f)] private float alignSmoothRange = 45f;

    [Header("Obstruction")]
    [SerializeField] private LayerMask obstructionMask = -1;
    
    private Camera _regularCamera;
    private Vector3 _focusPoint;
    private Vector3 _previousFocusPoint;
    private Vector2 _orbitAngles = new(45, 0f);
    private float _lastManualRotationTime;

    private Vector3 CameraHalfExtends
    {
        get
        {
            Vector3 halfExtends;
            halfExtends.y = 
                _regularCamera.nearClipPlane *
                Mathf.Tan(0.5f * Mathf.Deg2Rad * _regularCamera.fieldOfView);
            halfExtends.x = halfExtends.y * _regularCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }
    
    private void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    private void Awake()
    {
        _regularCamera = GetComponent<Camera>();
        _focusPoint = focus.position;
        _orbitAngles = initialAngles;
        transform.localRotation = Quaternion.Euler(_orbitAngles);
    }

    private void LateUpdate()
    {
        UpdateFocusPoint();
        Quaternion lookRotation;
        
        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(_orbitAngles);
        }
        else
        {
            lookRotation = transform.localRotation;
        }
        
        var lookDirection = lookRotation * Vector3.forward;
        var lookPosition = _focusPoint - lookDirection * distance;
        
        var rectOffset = lookDirection * _regularCamera.nearClipPlane;
        var rectPosition = lookPosition + rectOffset;
        var castFrom = focus.position;
        var castLine = rectPosition - castFrom;
        var castDistance = castLine.magnitude;
        var castDirection = castLine / castDistance;

        if (Physics.BoxCast(castFrom, CameraHalfExtends, castDirection, out RaycastHit hit,
                lookRotation, castDistance, obstructionMask))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        HandleZoom();
        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    private void UpdateFocusPoint()
    {
        _previousFocusPoint = _focusPoint;
        var targetPoint = focus.position;

        if (focusRadius > 0f)
        {
            var distance = Vector3.Distance(targetPoint, _focusPoint);
            var t = 1f;

            if (distance > 0.01f && focusCentering > 0f)
            {
                t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
            }
            
            if (distance > focusRadius)
            {
                t = Mathf.Min(t, focusRadius / distance);
            }
            
            _focusPoint = Vector3.Lerp(targetPoint, _focusPoint, t);
        }
        else
        {
            _focusPoint = targetPoint;
        }
    }
    
    private bool ManualRotation()
    {
        var input = new Vector2(Input.GetAxis("Vertical Camera"), Input.GetAxis("Horizontal Camera"));
        // Debug.Log($"Input: {input.x}, {input.y}");  // Debug Statement #1
        
        const float e = 0.001f;

        if (input.x < -e || input.x > e || input.y < -e || input.y > e)
        {
            _orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            _lastManualRotationTime = Time.unscaledTime;
            return true;
        }

        return false;
    }

    private bool AutomaticRotation()
    {
        if (Time.unscaledTime - _lastManualRotationTime < alignDelay)
        {
            return false;
        }
        
        var movement = new Vector2(
            _focusPoint.x - _previousFocusPoint.x,
            _focusPoint.z - _previousFocusPoint.z
            );
        
        var movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < 0.0001f)
        {
            return false;
        }
        
        var headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        var deltaAbs = Mathf.Abs(Mathf.DeltaAngle(_orbitAngles.y, headingAngle));
        var rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);

        if (deltaAbs < alignSmoothRange)
        {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        else if (180f - deltaAbs < alignSmoothRange)
        {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        }
        
        _orbitAngles.y = Mathf.MoveTowardsAngle(
            _orbitAngles.y, headingAngle, rotationChange);
        
        return true;
    }

    private void ConstrainAngles()
    {
        _orbitAngles.x = Mathf.Clamp(_orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        if (_orbitAngles.y < 0f)
        {
            _orbitAngles.y += 360f;
        }
        else if (_orbitAngles.y >= 360f)
        {
            _orbitAngles.y -= 360f;
        }
    }

    private static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }

    #region Custom Extension Methods
    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    #endregion
    
}
