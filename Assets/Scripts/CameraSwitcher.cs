using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public static class CameraSwitcher
{
    public static CinemachineVirtualCamera ActiveCamera = null;
    
    private static List<CinemachineVirtualCamera> _cameras = new();

    public static bool IsActiveCamera(CinemachineVirtualCamera camera)
    {
        return camera == ActiveCamera;
    }

    public static void SwitchCamera(CinemachineVirtualCamera camera)
    {
        camera.Priority = 10;
        ActiveCamera = camera;

        foreach (CinemachineVirtualCamera c in _cameras)
        {
            if (c != camera && c.Priority != 0)
            {
                c.Priority = 0;
            }
        }
    }

    public static void Register(CinemachineVirtualCamera camera)
    {
        _cameras.Add(camera);
        Debug.Log("Registered Camera: " + camera?.gameObject.name);
    }

    public static void Unregister(CinemachineVirtualCamera camera)
    {   
        _cameras.Remove(camera);
        Debug.Log("Unregistered Camera: " + camera?.gameObject.name);
    }
}
