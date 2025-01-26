using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Text;

public class DialogueManager : MonoBehaviour
{
    // Inspector fields
    //[Header("Dialogue Elements")]
    private ChoicesTracker choiceTracker;

    [Header("Dialogue Elements")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI optionAText;
    public TextMeshProUGUI optionBText;
    public GameObject continueButton;
    public GameObject optionsButtons;

    public Animator dialogueBoxAnimator;
    public AudioSource characterSound;

    [Header("CreepySmile")]
    public GameObject creepySmile;

    // Dialogue in parameters
    private Queue<string> sentences;
    private Dialogue currentDialogue;

    // Private state
    private Coroutine typeSentenceCoroutine;
    private readonly List<Coroutine> activeAnimations = new List<Coroutine>();
    private bool isLastSentence = false;
    private TMP_TextInfo cachedTextInfo;
    private List<Vector3[]> originalVertices = new List<Vector3[]>();
    private List<Color32[]> originalColors = new List<Color32[]>();

    // Regex optimizations
    private static readonly Regex TagRegex = new Regex(@"\[(/?)(wave|shake|bounce|rainbow|rotate|fade|bold|italic|underline|red|blue|green|br|smile)\]", RegexOptions.Compiled);
    private static readonly Regex VisibleTextRegex = new Regex("<.*?>", RegexOptions.Compiled);

    // Animation parameters
    private const float RAINBOW_SPEED = 2.5f;
    private const float ROTATE_SPEED = 25f;
    private const float FADE_SPEED = 3f;

    // Smile interval struct
    private struct SmileInterval
    {
        public int Start;
        public int End;

        public SmileInterval(int start, int end)
        {
            Start = start;
            End = end;
        }
    }

    // Singleton pattern
    public static DialogueManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() => sentences = new Queue<string>();

    public void StartDialogue(Dialogue dialogue)
    {
        choiceTracker = GameManager.Instance.GetComponent<ChoicesTracker>();
        currentDialogue = dialogue;

        dialogueBoxAnimator.SetBool("IsOpen", true);
        nameText.text = currentDialogue.name;
        if (currentDialogue.isChoiceDialogue)
        {
            optionAText.text = currentDialogue.sentenceOptionA;
            optionBText.text = currentDialogue.sentenceOptionB;
        }
        continueButton.SetActive(true);
        optionsButtons.SetActive(false);

        isLastSentence = false;

        characterSound.clip = currentDialogue.letterSoundClip;
        characterSound.volume = 0.7f;
        sentences.Clear();

        foreach (string sentence in dialogue.sentences) sentences.Enqueue(sentence);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        continueButton.SetActive(false);
        optionsButtons.SetActive(false);

        if (sentences.Count == 0)
        {
            if (currentDialogue.answerTrigger)
            {
                currentDialogue.answerTrigger.TriggerDialogue();
            }
            else
            {
                EndDialogue();
            }
            return;
        }
        else if (sentences.Count == 1)
        {
            isLastSentence = true;
        }

        StopDialogueCoroutines();
        ProcessNextSentence(sentences.Dequeue());
    }

    public void ProcessChoiceOptionA()
    {
        Debug.Log("DialogueID: " + currentDialogue.dialogueID + "; Chose Option A: " + optionAText.text);
        choiceTracker.SetChoiceOutput(currentDialogue.dialogueID, true);
        choiceTracker.SetChoiceOutput(currentDialogue.dialogueIndex, true);

        if (currentDialogue.answerTriggerA)
        {
            currentDialogue.answerTriggerA.TriggerDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    public void ProcessChoiceOptionB()
    {
        Debug.Log("DialogueID: " + currentDialogue.dialogueID + "; Chose Option B: " + optionBText.text);
        choiceTracker.SetChoiceOutput(currentDialogue.dialogueID, false);
        choiceTracker.SetChoiceOutput(currentDialogue.dialogueIndex, false);

        if (currentDialogue.answerTriggerB)
        {
            currentDialogue.answerTriggerB.TriggerDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void StopDialogueCoroutines()
    {
        if (typeSentenceCoroutine != null)
        {
            StopCoroutine(typeSentenceCoroutine);
            typeSentenceCoroutine = null;
        }

        foreach (var anim in activeAnimations)
        {
            if (anim != null) StopCoroutine(anim);
        }
        activeAnimations.Clear();
    }

    private void ProcessNextSentence(string sentence)
    {
        var parsedData = ParseSentence(sentence);
        typeSentenceCoroutine = StartCoroutine(TypeSentence(parsedData));
    }

    IEnumerator TypeSentence((string formatted, List<TextAnimation> animations, List<SmileInterval> smileIntervals) parsedData)
    {
        dialogueText.text = "";
        int visibleCharCount = 0;

        for (int i = 0; i < parsedData.formatted.Length; i++)
        {
            char c = parsedData.formatted[i];

            if (c == '<')
            {
                int tagEnd = parsedData.formatted.IndexOf('>', i);
                if (tagEnd == -1) break;
                i = tagEnd;
                continue;
            }

            dialogueText.text = parsedData.formatted.Substring(0, i + 1);
            dialogueText.ForceMeshUpdate();
            CacheOriginalVertices();

            bool isVisible = IsVisibleCharacter(c);
            if (isVisible)
            {
                float randPitch = Random.Range(currentDialogue.minRangePitch, currentDialogue.maxRangePitch);
                characterSound.pitch = randPitch;
                characterSound.Play();
                visibleCharCount++;
            }

            foreach (var anim in parsedData.animations)
            {
                if (visibleCharCount == anim.StartIndex + 1)
                {
                    activeAnimations.Add(StartCoroutine(AnimateText(anim)));
                }
            }

            // Update creepy smile state
            bool shouldActivateSmile = false;
            foreach (var interval in parsedData.smileIntervals)
            {
                if (visibleCharCount >= interval.Start && visibleCharCount <= interval.End)
                {
                    shouldActivateSmile = true;
                    break;
                }
            }
            creepySmile.SetActive(shouldActivateSmile);

            yield return new WaitForSeconds(currentDialogue.talkTime);
        }

        characterSound.loop = false;
        creepySmile.SetActive(false);

        if (isLastSentence && currentDialogue.isChoiceDialogue)
        {
            continueButton.SetActive(false);
            optionsButtons.SetActive(true);
        }
        else
        {
            continueButton.SetActive(true);
            optionsButtons.SetActive(false);
        }
    }

    private (string formatted, List<TextAnimation> animations, List<SmileInterval> smileIntervals) ParseSentence(string sentence)
    {
        List<TextAnimation> animations = new List<TextAnimation>();
        List<SmileInterval> smileIntervals = new List<SmileInterval>();
        StringBuilder sb = new StringBuilder(sentence.Length);
        Stack<TextAnimation> openTags = new Stack<TextAnimation>();
        Stack<int> smileStartIndices = new Stack<int>();
        int tagAdjustment = 0;
        int visibleCharCount = 0;

        sentence = sentence.Replace("[br]", "\n");

        foreach (Match m in TagRegex.Matches(sentence))
        {
            sb.Append(sentence, tagAdjustment, m.Index - tagAdjustment);
            string tagType = m.Groups[2].Value;
            bool isClosing = m.Groups[1].Length > 0;

            for (int i = tagAdjustment; i < m.Index; i++)
            {
                if (IsVisibleCharacter(sentence[i])) visibleCharCount++;
            }

            if (tagType == "smile")
            {
                if (!isClosing)
                {
                    smileStartIndices.Push(visibleCharCount);
                }
                else
                {
                    if (smileStartIndices.Count > 0)
                    {
                        int start = smileStartIndices.Pop();
                        smileIntervals.Add(new SmileInterval(start, visibleCharCount));
                    }
                }
            }
            else
            {
                if (!isClosing)
                {
                    openTags.Push(new TextAnimation(
                        tagType,
                        visibleCharCount,
                        m.Index + m.Length
                    ));
                }
                else
                {
                    if (openTags.Count > 0 && openTags.Peek().Type == tagType)
                    {
                        TextAnimation anim = openTags.Pop();
                        anim.Length = visibleCharCount - anim.StartIndex;
                        animations.Add(anim);
                    }
                }
            }

            tagAdjustment = m.Index + m.Length;

            if (tagType is "bold" or "italic" or "underline" or "red" or "blue" or "green")
            {
                sb.Append(m.Value
                    .Replace("[bold]", "<b>").Replace("[/bold]", "</b>")
                    .Replace("[italic]", "<i>").Replace("[/italic]", "</i>")
                    .Replace("[underline]", "<u>").Replace("[/underline]", "</u>")
                    .Replace("[red]", "<color=#FF0000>").Replace("[/red]", "</color>")
                    .Replace("[blue]", "<color=#0000FF>").Replace("[/blue]", "</color>")
                    .Replace("[green]", "<color=#00FF00>").Replace("[/green]", "</color>"));
            }
        }

        sb.Append(sentence, tagAdjustment, sentence.Length - tagAdjustment);
        return (sb.ToString(), animations, smileIntervals);
    }

    // Rest of the class remains unchanged (AnimateText and helper methods)

    IEnumerator AnimateText(TextAnimation animation)
    {
        float timeOffset = Random.Range(0f, 2f);
        float hue = 0f;

        while (true)
        {
            if (dialogueText == null || dialogueText.textInfo == null || originalVertices.Count == 0)
                yield break;

            TMP_TextInfo textInfo = dialogueText.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int visibleIndex = GetVisibleCharacterIndex(i);
                if (visibleIndex < animation.StartIndex || visibleIndex >= animation.StartIndex + animation.Length)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] verts = originalVertices[materialIndex];
                Color32[] colors = originalColors[materialIndex];

                switch (animation.Type)
                {
                    case "rainbow":
                        Color32 rainbowColor = Color.HSVToRGB((hue + visibleIndex * 0.1f) % 1f, 0.8f, 1f);
                        for (int j = 0; j < 4; j++)
                            textInfo.meshInfo[materialIndex].colors32[vertexIndex + j] = rainbowColor;
                        break;

                    case "rotate":
                        Vector3 center = (verts[vertexIndex] + verts[vertexIndex + 2]) / 2f;
                        float angle = Mathf.Sin(Time.time * ROTATE_SPEED + visibleIndex) * 10f * currentDialogue.effectsStrength;
                        for (int j = 0; j < 4; j++)
                        {
                            Vector3 vert = verts[vertexIndex + j];
                            Vector3 rotated = RotateAround(vert, center, angle);
                            textInfo.meshInfo[materialIndex].vertices[vertexIndex + j] = rotated;
                        }
                        break;

                    case "fade":
                        float alpha = Mathf.Abs(Mathf.Sin(Time.time * FADE_SPEED + visibleIndex)) * currentDialogue.effectsStrength;
                        for (int j = 0; j < 4; j++)
                        {
                            Color32 c = colors[vertexIndex + j];
                            c.a = (byte)(alpha * 255);
                            textInfo.meshInfo[materialIndex].colors32[vertexIndex + j] = c;
                        }
                        break;

                    default:
                        Vector3 offset = GetAnimationOffset(animation.Type, visibleIndex - animation.StartIndex, timeOffset) * currentDialogue.effectsStrength;
                        for (int j = 0; j < 4; j++)
                            textInfo.meshInfo[materialIndex].vertices[vertexIndex + j] = verts[vertexIndex + j] + offset;
                        break;
                }
            }

            dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            hue += Time.deltaTime * RAINBOW_SPEED * currentDialogue.effectsStrength;
            yield return null;
        }
    }

    private Vector3 RotateAround(Vector3 point, Vector3 center, float angle)
    {
        Vector3 dir = point - center;
        dir = Quaternion.Euler(0, 0, angle) * dir;
        return center + dir;
    }

    private void CacheOriginalVertices()
    {
        originalVertices.Clear();
        originalColors.Clear();

        if (dialogueText == null || dialogueText.textInfo == null) return;

        foreach (TMP_MeshInfo meshInfo in dialogueText.textInfo.meshInfo)
        {
            originalVertices.Add((Vector3[])meshInfo.vertices.Clone());
            originalColors.Add((Color32[])meshInfo.colors32.Clone());
        }
    }

    private int GetVisibleCharacterIndex(int characterIndex)
    {
        int count = -1;
        if (dialogueText == null || dialogueText.textInfo == null) return -1;

        for (int i = 0; i <= characterIndex; i++)
        {
            if (i >= dialogueText.textInfo.characterInfo.Length) break;
            if (dialogueText.textInfo.characterInfo[i].isVisible) count++;
        }
        return count;
    }

    private Vector3 GetAnimationOffset(string animType, int charOffset, float timeOffset)
    {
        float time = Time.time * 3f + timeOffset;
        return animType switch
        {
            "shake" => new Vector3(
                Mathf.PerlinNoise(time, charOffset) - 0.5f,
                Mathf.PerlinNoise(time + 1f, charOffset) - 0.5f,
                0) * 0.8f,
            "bounce" => new Vector3(0, Mathf.Abs(Mathf.Sin(time + charOffset)) * 3f, 0),
            "wave" => new Vector3(0, Mathf.Sin(time + charOffset * 0.3f) * 3f, 0),
            _ => Vector3.zero
        };
    }

    private bool IsVisibleCharacter(char c) => !char.IsWhiteSpace(c) && c != '<' && c != '>';

    private void EndDialogue()
    {
        dialogueBoxAnimator.SetBool("IsOpen", false);
        StopDialogueCoroutines();
        StartCoroutine(End());
    }

    private IEnumerator End()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.NextMap();
    }

    private class TextAnimation
    {
        public readonly string Type;
        public readonly int StartIndex;
        public int Length;
        public readonly int TagEndIndex;

        public TextAnimation(string type, int startIndex, int tagEndIndex)
        {
            Type = type;
            StartIndex = startIndex;
            TagEndIndex = tagEndIndex;
        }
    }
}