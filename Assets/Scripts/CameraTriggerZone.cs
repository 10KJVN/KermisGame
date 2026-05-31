using System;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CameraTriggerZone : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private Vector3 boxSize;

    private BoxCollider _boxCol;
    private Rigidbody _rb;

    private void Awake()
    {
        _boxCol = GetComponent<BoxCollider>();
        _rb = GetComponent<Rigidbody>();
        _boxCol.isTrigger = true;
        _boxCol.size = boxSize;
        
        _rb.isKinematic = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CameraSwitcher.ActiveCamera != cam)
            {
                CameraSwitcher.SwitchCamera(cam);
            }
        }
    }
}
