using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Defines dialogue sequences for a scene.
/// lightweight scene-specific narrative solution.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public TextManager textManager;

    private void Start()
    {
        Dialogue1();
    }
    
    // TODO: Hardcode an if-statement on which dialogue to play according to scene name.

    private void Dialogue1()
    {
        textManager.TextRequest(4, "It was the last day of the kermis.", 2);
        textManager.TextRequest(1, "I have fond memories of coming here in my youth.", 3);
        textManager.TextRequest(1, "It's totally unrecognizable compared to how I remember it.", 3);
        textManager.TextRequest(1, "Still...", 2);
        textManager.TextRequest(1, "I wanted to experience it one more time.", 3);
    }
}
