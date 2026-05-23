using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform focus;
    [SerializeField, Range(1f, 120f)] private float distance = 5f;
    [SerializeField, Min(0f)] private float focusRadius = 1f;
    
    private Vector3 _focusPoint;

    private void Awake()
    {
        _focusPoint = focus.position;
    }

    private void LateUpdate()
    {
        UpdateFocusPoint();
        var lookDirection = transform.forward;
        transform.localPosition = _focusPoint - lookDirection * distance;
    }

    private void UpdateFocusPoint()
    {
        var targetPoint = focus.position;

        if (focusRadius > 0f)
        {
            var distance = Vector3.Distance(targetPoint, _focusPoint);
            
            if (distance > focusRadius)
            {
                _focusPoint = Vector3.Lerp(targetPoint, _focusPoint, focusRadius / distance);
            }
        }
        
        else
        {
            targetPoint = focus.position;
        }
    }
    
}
