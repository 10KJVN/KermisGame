using Cinemachine;
using UnityEngine;

public enum GameState { Exploration, Shooting }

public class GameModeManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CinemachineFreeLook explorationCamera;
    [SerializeField] private CinemachineVirtualCamera shooterCamera;

    private GameState _currentState;

    private void Start()
    {
        _currentState = GameState.Exploration;
        //explorationCamera.Priority = 11;
    }

    private void Update()
    {
        if (_currentState == GameState.Shooting &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            EnterExploration();
        }
        
        else if (_currentState == GameState.Exploration)
        {
            
        }
    }

    public void EnterExploration()
    {
        _currentState = GameState.Exploration;
        playerController.enabled = true;

        explorationCamera.Priority = 10;
        shooterCamera.Priority = 0;
    }

    public void EnterShooting()
    {
        _currentState = GameState.Shooting;
        playerController.enabled = false;
        
        explorationCamera.Priority = 0;
        shooterCamera.Priority = 10;
    }
}
