using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;

    public AudioClip letterSoundClip;

    public float talkTime = 0.01f;

    [TextArea(3, 10)]
    public string[] sentences;
}
