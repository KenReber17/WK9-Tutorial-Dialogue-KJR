using System;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;               // Speaker’s name (e.g., "Apollo")
    [TextArea(3, 10)]
    public string[] sentences;        // Array of sentences in the conversation
}