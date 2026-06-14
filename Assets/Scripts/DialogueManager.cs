using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Defines dialogue sequences for a scene.
/// lightweight scene-specific narrative solution.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextManager textManager;

    private void Start()
    {
        var sceneName = SceneManager.GetActiveScene().name;

        switch (sceneName)
        {
            case "NarrativeScene01": // Start Scene
                Dialogue1();
                break;

            case "NarrativeScene02": // End Scene
                Dialogue2();
                break;
        }
    }

    private void Dialogue1()
    {
        textManager.TextRequest(4, "It was the last day of the kermis.", 2);
        textManager.TextRequest(1, "I have fond memories of coming here in my youth.", 3);
        textManager.TextRequest(1, "It's totally unrecognizable compared to how I remember it.", 3);
        textManager.TextRequest(1, "Still...", 2);
        textManager.TextRequest(1, "I wanted to experience it one more time.", 3);
    }

    private void Dialogue2()
    {
        textManager.TextRequest(3, "You will be missed.", 2);
        textManager.TextRequest(1, "Yet I'm glad you happened.", 3);
        
        textManager.TextRequest(2, "The lights.", 2);
        textManager.TextRequest(1, "The laughter.", 2);
        textManager.TextRequest(1, "The games we played.", 2);
        
        textManager.TextRequest(2, "It will all be gone by tomorrow.", 3);
        textManager.TextRequest(2, "But the memories remain", 3);
        textManager.TextRequest(1, "Maybe that's what matters.", 2);
        textManager.TextRequest(3, "Maybe that's enough.", 5);
    }
}
