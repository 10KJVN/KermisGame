using UnityEngine;

public class FerrisWheelInteraction : MonoBehaviour
{
    public void Interact()
    {
        if (!GameProgress.HasPlayedShootingGame)
        {
            Debug.Log("Come back later.");
            // Play oneShot error SFX
            return;
        }

        // Play success SFX
        SceneLoader.LoadScene("Cutscene01");
    }
}