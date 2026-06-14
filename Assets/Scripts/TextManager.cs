using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textBox;

    //Speed between char being added to text
    public float letterDelay;

    //Speed between char being removed from text
    public float removeDelay;
    private char[] _letters;
    
    private readonly List<DialogueRequest> _stack = new();
    private int _onLetter;
    
    private struct DialogueRequest : IEquatable<DialogueRequest>
    {
        public float StartDelay;
        public float ReadTime;
        public string Dialogue;

        public bool Equals(DialogueRequest other)
        {
            return StartDelay.Equals(other.StartDelay) && ReadTime.Equals(other.ReadTime) && Dialogue == other.Dialogue;
        }

        public override bool Equals(object obj)
        {
            return obj is DialogueRequest other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartDelay, ReadTime, Dialogue);
        }
    }

    private void Start()
    {
        textBox = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        textBox.text = "";
    }

    public void TextRequest(float startDelay, string dialogue, float readTime)
    {
        var newRequest = new DialogueRequest
        {
            StartDelay = startDelay,
            Dialogue = dialogue,
            ReadTime = readTime
        };
        _stack.Add(newRequest);

        // If any longer than its already running
        if (_stack.Count == 1)
        {
            _letters = _stack[0].Dialogue.ToCharArray();
            StartCoroutine(nameof(AddChar), startDelay);
        }
    }

    //Adding text
    public void NextCharacter()
    {
        textBox.text += _letters[_onLetter];
        _onLetter++;
        if (_onLetter > _letters.Length - 1)
        {
            StartCoroutine(nameof(RemoveChar), _stack[0].ReadTime);
            _stack.Remove(_stack[0]);
            return;
        }

        StartCoroutine(nameof(AddChar), letterDelay);
    }

    private IEnumerator AddChar(float t)
    {
        yield return new WaitForSeconds(t);
        NextCharacter();
    }

    //Removing Text
    public void NextCharacterRemove()
    {
        textBox.text = textBox.text.Remove(_onLetter - 1);
        _onLetter--;
        if (_onLetter == 0)
        {
            if (_stack.Count > 0)
            {
                _letters = _stack[0].Dialogue.ToCharArray();
                StartCoroutine(nameof(AddChar), _stack[0].StartDelay);
            }

            return;
        }

        StartCoroutine(nameof(RemoveChar), removeDelay);
    }

    private IEnumerator RemoveChar(float t)
    {
        yield return new WaitForSeconds(t);
        NextCharacterRemove();
    }
}