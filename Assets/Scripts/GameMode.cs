using UnityEngine;
using Cinemachine;

/// <summary>
/// Hardcoded temp solution to switch the exploration camera to the shooting camera.
/// Enabling the virtual camera during runtime switches to it and disabling vice versa.
/// </summary>

public class GameMode : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CinemachineVirtualCamera shootingCamera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            playerController.enabled = !playerController.enabled;
            shootingCamera.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            playerController.enabled = !playerController.enabled;
            shootingCamera.gameObject.SetActive(false);
        }
    }
}
