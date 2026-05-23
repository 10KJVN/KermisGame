using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform focus;
    [SerializeField, Range(1f, 120f)] private float distance = 5f;


    private void LateUpdate()
    {
        var focusPoint = focus.position;
        var lookDirection = transform.forward;
        transform.localPosition = focusPoint - lookDirection * distance;
    }
}
