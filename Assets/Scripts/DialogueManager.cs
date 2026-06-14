using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public TextManager textManager;

    private void Start()
    {
        Dialogue1();
    }

    private void Dialogue1()
    {
        textManager.TextRequest(4, "This is a test of dialogue", 2);
        textManager.TextRequest(1, "This follows the first text", 3);
        textManager.TextRequest(1, "And this is the final one", 3);
    }
}
