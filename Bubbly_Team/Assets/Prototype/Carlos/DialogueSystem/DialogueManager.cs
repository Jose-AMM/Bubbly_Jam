using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Text;

public class DialogueManager : MonoBehaviour
{
    // Inspector fields
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Animator dialogueBoxAnimator;
    public AudioSource characterSound;
    public AudioClip letterSoundClip;

    // Private state
    private Queue<string> sentences;
    private float letterDelay;
    private Coroutine typeSentenceCoroutine;
    private readonly List<Coroutine> activeAnimations = new List<Coroutine>();
    private TMP_TextInfo cachedTextInfo;
    private List<Vector3[]> originalVertices = new List<Vector3[]>();
    private List<Color32[]> originalColors = new List<Color32[]>();

    // Regex optimizations
    private static readonly Regex TagRegex = new Regex(@"\[(/?)(wave|shake|bounce|rainbow|rotate|fade|bold|italic|underline|red|blue|green|br)\]", RegexOptions.Compiled);
    private static readonly Regex VisibleTextRegex = new Regex("<.*?>", RegexOptions.Compiled);

    // Animation parameters
    private const float RAINBOW_SPEED = 2.5f;
    private const float ROTATE_SPEED = 25f;
    private const float FADE_SPEED = 3f;

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
        dialogueBoxAnimator.SetBool("IsOpen", true);
        nameText.text = dialogue.name;
        letterDelay = dialogue.talkTime;
        sentences.Clear();
        foreach (string sentence in dialogue.sentences) sentences.Enqueue(sentence);
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        StopDialogueCoroutines();
        ProcessNextSentence(sentences.Dequeue());
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

    IEnumerator TypeSentence((string formatted, List<TextAnimation> animations) parsedData)
    {
        dialogueText.text = "";
        int visibleCharCount = 0;

        for (int i = 0; i < parsedData.formatted.Length; i++)
        {
            char c = parsedData.formatted[i];

            // Handle rich text tags
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

            if (IsVisibleCharacter(c))
            {
                visibleCharCount++;
                characterSound?.PlayOneShot(letterSoundClip);
            }

            foreach (var anim in parsedData.animations)
            {
                if (visibleCharCount == anim.StartIndex + 1)
                {
                    activeAnimations.Add(StartCoroutine(AnimateText(anim)));
                }
            }

            yield return new WaitForSeconds(letterDelay);
        }
    }

    private (string formatted, List<TextAnimation> animations) ParseSentence(string sentence)
    {
        List<TextAnimation> animations = new List<TextAnimation>();
        StringBuilder sb = new StringBuilder(sentence.Length);
        Stack<TextAnimation> openTags = new Stack<TextAnimation>();
        int tagAdjustment = 0;
        int visibleCharCount = 0;

        // Handle manual line breaks
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
        return (sb.ToString(), animations);
    }

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

                // Original position and color
                Vector3[] verts = originalVertices[materialIndex];
                Color32[] colors = originalColors[materialIndex];

                // Apply animations
                switch (animation.Type)
                {
                    case "rainbow":
                        Color32 rainbowColor = Color.HSVToRGB((hue + visibleIndex * 0.1f) % 1f, 0.8f, 1f);
                        for (int j = 0; j < 4; j++)
                            textInfo.meshInfo[materialIndex].colors32[vertexIndex + j] = rainbowColor;
                        break;

                    case "rotate":
                        Vector3 center = (verts[vertexIndex] + verts[vertexIndex + 2]) / 2f;
                        float angle = Mathf.Sin(Time.time * ROTATE_SPEED + visibleIndex) * 10f;
                        for (int j = 0; j < 4; j++)
                        {
                            Vector3 vert = verts[vertexIndex + j];
                            Vector3 rotated = RotateAround(vert, center, angle);
                            textInfo.meshInfo[materialIndex].vertices[vertexIndex + j] = rotated;
                        }
                        break;

                    case "fade":
                        float alpha = Mathf.Abs(Mathf.Sin(Time.time * FADE_SPEED + visibleIndex));
                        for (int j = 0; j < 4; j++)
                        {
                            Color32 c = colors[vertexIndex + j];
                            c.a = (byte)(alpha * 255);
                            textInfo.meshInfo[materialIndex].colors32[vertexIndex + j] = c;
                        }
                        break;

                    default:
                        Vector3 offset = GetAnimationOffset(animation.Type, visibleIndex - animation.StartIndex, timeOffset);
                        for (int j = 0; j < 4; j++)
                            textInfo.meshInfo[materialIndex].vertices[vertexIndex + j] = verts[vertexIndex + j] + offset;
                        break;
                }
            }

            dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            hue += Time.deltaTime * RAINBOW_SPEED;
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