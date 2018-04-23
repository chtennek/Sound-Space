﻿using UnityEngine;

[System.Serializable]
public struct Line
{
    public Actor actor;
    [TextArea(3, 10)]
    public string text;

    public float playTime;
    public AudioClip audio;
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public Line[] lines;
}
