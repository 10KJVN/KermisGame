using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject explorationUI;
    [SerializeField] private GameObject shootingUI;
    [SerializeField] private GameObject fishingUI;

    public void ShowUI(GameState state)
    {
        explorationUI.SetActive(false);
        shootingUI.SetActive(false);
        fishingUI.SetActive(false);

        switch(state)
        {
            case GameState.Exploration:
                explorationUI?.SetActive(true);
                break;

            case GameState.Shooting:
                shootingUI?.SetActive(true);
                break;

            case GameState.Fishing:
                fishingUI?.SetActive(true);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}