using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// The single entry point to our game.
/// </summary>

public class GameInitiator : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Light _mainDirectionalLight;
    [SerializeField] private EventSystem _mainEventSystem;

    private async void Start()
    {
        try 
        {
            BindObjects();
            Debug.Log("Loading...");
            
            await InitializeObjects();
            await CreateObjects();
            await PrepareGame();
            
            Debug.Log("Finished loading.");
            await BeginGame();
        }
        
        catch (Exception e) { Debug.Log($"Failed loading: {e}"); }
        
        finally { Debug.Log("Game Launched successfully."); }
    }

    // Connecting our instances
    private void BindObjects()
    {
        _mainCamera = Instantiate(_mainCamera);
        _mainDirectionalLight = Instantiate(_mainDirectionalLight);
        _mainEventSystem = Instantiate(_mainEventSystem);
    }

    // Turning on our services e.g. persistent systems.
    private async Awaitable InitializeObjects()
    { }
    
    // Loading in our Entities / Gameplay Objects
    private async Awaitable CreateObjects()
    { }
    
    // Setting up our objects
    private async Awaitable PrepareGame()
    {
        SceneManager.LoadScene("Level", LoadSceneMode.Additive);
    }
    
    // Here you decide the game's flow
    private async Awaitable BeginGame()
    { }
}