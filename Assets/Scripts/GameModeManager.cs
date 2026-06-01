using Cinemachine;
using UnityEngine;

public enum GameState
{
    Exploration, Shooting, Fishing //, 3rd miniGame
}

public class GameModeManager : MonoBehaviour
{
    [Header("Class References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private MinigameTimer minigameTimer;
    [SerializeField] private ShootingSession shootingSession;
    
    [Header("Camera References")]
    [SerializeField] private CinemachineFreeLook explorationCamera;
    [SerializeField] private CinemachineVirtualCamera shooterCamera;

    private GameState _currentState;

    private void Start()
    {
        _currentState = GameState.Exploration;
    }

    private void Update()
    {
        if (_currentState == GameState.Shooting &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log($"You left early");
            EnterExploration();
        }
        
        // else if (_currentState == GameState.Exploration)
        // {
        //     
        // }
    }

    public void EnterExploration()
    {
        _currentState = GameState.Exploration;
        playerController.enabled = true;
        
        Debug.Log($"Final Score: {scoreManager.GetScore()}");
        minigameTimer.StopTimer();
        uiManager.ShowUI(GameState.Exploration);

        explorationCamera.Priority = 10;
        shooterCamera.Priority = 0;
    }

    public void EnterShooting()
    {
        _currentState = GameState.Shooting;
        playerController.enabled = false;

        shootingSession.ResetTargets();
        scoreManager.ResetScore();
        minigameTimer.StartTimer();
        
        uiManager.ShowUI(GameState.Shooting);
        
        explorationCamera.Priority = 0;
        shooterCamera.Priority = 10;
    }
}
