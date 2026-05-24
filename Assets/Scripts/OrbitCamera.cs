using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform focus;
    [SerializeField, Range(1f, 120f)] private float distance = 5f;
    [SerializeField, Min(0f)] private float focusRadius = 1f;
    [SerializeField, Range(0f, 1f)] private float focusCentering = 0.5f;
    [SerializeField, Range(1f, 360f)] private float rotationSpeed = 90f;
    [SerializeField, Range(-89f, 89f)] private float minVerticalAngle = -30f;
    [SerializeField, Range(-89f, 89f)] private float maxVerticalAngle = 60f;
    [SerializeField, Min(0f)] private float alignDelay = 5f;
    [SerializeField, Range(0f, 90f)] private float alignSmoothRange = 45f;
    
    private Vector3 _focusPoint;
    private Vector3 _previousFocusPoint;
    private Vector2 _orbitAngles = new(45, 0f);
    private float _lastManualRotationTime;

    private void Awake()
    {
        _focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(_orbitAngles);
    }

    private void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
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
        
        var lookDirection = transform.forward;
        var lookPosition = _focusPoint - lookDirection * distance;
        
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

            if (distance > 0.01f && focusRadius > 0f)
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
            targetPoint = focus.position;
        }
    }

    private bool ManualRotation()
    {
        // TODO: Define Vertical, Horizontal Camera input axes bound to the third and fourth axis.
        var input = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
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
        if (Time.unscaledTime - _lastManualRotationTime > alignDelay)
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
            rotationChange *= (180f - deltaAbs) /  alignSmoothRange;
        }
        
        _orbitAngles.y = Mathf.MoveTowards(
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

    static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }
    
}
