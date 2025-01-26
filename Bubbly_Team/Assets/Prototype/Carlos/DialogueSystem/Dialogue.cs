using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string dialogueID;
    public int dialogueIndex;

    public string name;

    public AudioClip letterSoundClip;

    public float talkTime = 0.07f;

    public float effectsStrength = 1.0f;

    public float minRangePitch = 0.95f;
    public float maxRangePitch = 1.00f;

    [TextArea(3, 10)]
    public string[] sentences;

    public bool isChoiceDialogue = true;

    [TextArea(3, 10)]
    public string sentenceOptionA;

    [TextArea(3, 10)]
    public string sentenceOptionB;

    public DialogueTrigger answerTrigger;

    public DialogueTrigger answerTriggerA;
    public DialogueTrigger answerTriggerB;
}
