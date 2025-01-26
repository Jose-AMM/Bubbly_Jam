using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Transactions;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera Camera;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] public GameObject Player;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public float GlitchedMusicRatio = 0.0f;
    private ManualCamera manualCamera;
    [SerializeField] private int CurrentMap = 1;
    [SerializeField] private GameObject[] checkpoints;
    [SerializeField] private int maxCheckpointsSize;
    [SerializeField] private int currentCheckpoint = 0;

    [SerializeField] private float[] oxygenByLevel;

    [SerializeField] private BlackPanelLogic BlackPanel;
    [SerializeField] private List<GameObject> ShopPrefabs;
    private GameObject InstantiatedShop;
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        blackScreen.GetComponent<BlackPanelLogic>().StartFadeOut();
        manualCamera = virtualCamera.GetComponent<ManualCamera>();
        maxCheckpointsSize = checkpoints.Length;
        DisablePlayer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2 GetPlayerPos()
    {
        return new Vector2(Player.transform.position.x, Player.transform.position.y);
    }

    public void DisablePlayer()
    {
        Player.GetComponent<PlayerMovement>().Stop();
        Player.GetComponent<JellyfishFloatSimple>().enabled = true;
        Player.GetComponent<OxygenBar>().StopDrowning();
    }

    public void KillJellyfish()
    {
        Player.GetComponent<JellyfishFloatSimple>().enabled = false;
    }

    public void GetOxygen()
    {
        Player.transform.Find("Canvas/OxigenBar").GetComponent<OxygenBar>().AddOxygen(100.0f);
        SoundManager.Instance.PlaySound("Gasp", 0.2f);
    }

    public void EnterShop(String PrefabName)
    {
        SoundManager.Instance.EnterShop();
        StartCoroutine(LoadShop(PrefabName));
    }

    public void ExitShop()
    {
        Cursor.visible = false;
        SoundManager.Instance.ExitShop();
        StartCoroutine(UnloadShop());
    }

    IEnumerator LoadShop(String PrefabName)
    {
        Cursor.visible = true;
        BlackPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0);
        BlackPanel.StartFadeIn();
        DisablePlayer();
        yield return new WaitForSeconds(BlackPanel.fadeDuration);
        Debug.Log("Load Prefab: " + PrefabName);
        foreach (GameObject ShopPrefab in ShopPrefabs)
        {
            if (ShopPrefab.name == PrefabName)
            {
                InstantiatedShop = Instantiate(ShopPrefab, Vector3.zero, Quaternion.identity);
            }
        }

        yield return new WaitForSeconds(0.2f);
        BlackPanel.StartFadeOut();
        yield return new WaitForSeconds(BlackPanel.fadeDuration);
        Debug.Log("Start dialogue");

        // STARTING DIALOGUE
        DialogueTrigger[] allDialogues;
        ChoicesTracker choicesTracker = GetComponent<ChoicesTracker>();

        if (InstantiatedShop.name.Contains("SHOP1"))
        {
            allDialogues = GameObject.FindObjectsOfType<DialogueTrigger>();
            foreach (DialogueTrigger t in allDialogues)
            {
                if (t.dialogue.dialogueIndex == 1)
                {
                    t.TriggerDialogue();
                }
            }
        } else if (InstantiatedShop.name.Contains("SHOP2"))
        {
            allDialogues = GameObject.FindObjectsOfType<DialogueTrigger>();
            foreach (DialogueTrigger t in allDialogues)
            {
                if (choicesTracker.FindChoiceOutput(1))
                {
                    // trigger el 5
                    if (t.dialogue.dialogueIndex == 5)
                    {
                        t.TriggerDialogue();
                    }
                }
                else
                {
                    // trigger el 6
                    if (t.dialogue.dialogueIndex == 6)
                    {
                        t.TriggerDialogue();
                    }
                }
            }

        } else if (InstantiatedShop.name.Contains("SHOP3"))
        {
            allDialogues = GameObject.FindObjectsOfType<DialogueTrigger>();
            foreach (DialogueTrigger t in allDialogues)
            {
                if (choicesTracker.FindChoiceOutput(5))
                {
                    // trigger el 9
                    if (t.dialogue.dialogueIndex == 9)
                    {
                        t.TriggerDialogue();
                    }
                }
                else
                {
                    // trigger el 10
                    if (t.dialogue.dialogueIndex == 10)
                    {
                        t.TriggerDialogue();
                    }
                }
            }
        }
        else if (InstantiatedShop.name.Contains("SHOP4"))
        {
            allDialogues = GameObject.FindObjectsOfType<DialogueTrigger>();
            foreach (DialogueTrigger t in allDialogues)
            {
                if (t.dialogue.dialogueIndex == 18)
                {
                    t.TriggerDialogue();
                }
            }
        }

        /*
        switch (InstantiatedShop.name)
        {
            case "SHOP1":
                allDialogues = GameObject.FindObjectsOfType<DialogueTrigger>();
                foreach (DialogueTrigger t in allDialogues)
                {
                    if (t.dialogue.dialogueIndex == 1)
                    {
                        t.TriggerDialogue();
                    }
                }

                break;
            case "SHOP2":
                allDialogues = GameObject.FindObjectsOfType<DialogueTrigger>();
                foreach (DialogueTrigger t in allDialogues)
                {
                    if (choicesTracker.FindChoiceOutput(1))
                    {
                        // trigger el 5
                        if (t.dialogue.dialogueIndex == 5)
                        {
                            t.TriggerDialogue();
                        }
                    }
                    else
                    {
                        // trigger el 6
                        if (t.dialogue.dialogueIndex == 6)
                        {
                            t.TriggerDialogue();
                        }
                    }
                }

                break;
            case "SHOP3":
                allDialogues = GameObject.FindObjectsOfType<DialogueTrigger>();
                foreach (DialogueTrigger t in allDialogues)
                {
                    if (choicesTracker.FindChoiceOutput(5))
                    {
                        // trigger el 9
                        if (t.dialogue.dialogueIndex == 9)
                        {
                            t.TriggerDialogue();
                        }
                    }
                    else
                    {
                        // trigger el 10
                        if (t.dialogue.dialogueIndex == 10)
                        {
                            t.TriggerDialogue();
                        }
                    }
                }

                break;
            case "SHOP4":
                allDialogues = GameObject.FindObjectsOfType<DialogueTrigger>();
                foreach (DialogueTrigger t in allDialogues)
                {
                    if (t.dialogue.dialogueIndex == 18)
                    {
                        t.TriggerDialogue();
                    }
                }

                break;
            default:
                break;
        }*/
    }

    public IEnumerator UnloadShop()
    {
        BlackPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0);
        BlackPanel.StartFadeIn();
        yield return new WaitForSeconds(BlackPanel.fadeDuration);
        Destroy(InstantiatedShop);
        yield return new WaitForSeconds(0.2f);
        BlackPanel.StartFadeOut();
        yield return new WaitForSeconds(BlackPanel.fadeDuration);
        EnablePlayer();
    }

    public void EnablePlayer()
    {
        Player.GetComponent<PlayerMovement>().AsNew();
        Player.GetComponent<JellyfishFloatSimple>().enabled = false;
        Player.GetComponent<OxygenBar>().StartDrowning();
        //Player.SetActive(true);
    }

    public void BlackScreenFadeOut()
    {
        blackScreen.GetComponent<BlackPanelLogic>().StartFadeOut();
    }

    public void BlackScreenFadeIn()
    {
        blackScreen.SetActive(true);
        blackScreen.GetComponent<BlackPanelLogic>().StartFadeIn();
    }

    public void MakeCameraFollowPlayer()
    {
        virtualCamera.Follow = Player.transform;
        //StartCoroutine("EaseCameraToPlayer");
    }

    private IEnumerator EaseCameraToPlayer()
    {
        Vector3 startPosition = virtualCamera.transform.position;
        Vector3 targetPosition = Player.transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < 2.0f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 2.0f;

            // Use an easing function for smooth movement (ease-in-out).
            t = Mathf.SmoothStep(0f, 1f, t);

            //targetPosition = Player.transform.position;

            // Interpolate position and rotation.
            virtualCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        // Ensure the final position and rotation match exactly.
        virtualCamera.transform.position = targetPosition;

        // Set the camera to follow the player (if applicable in your setup).
        SetFollowToPlayer();
    }

    public void SetFollowToPlayer()
    {
        virtualCamera.Follow = Player.transform;

        Debug.Log("Camera follow set to the player.");
    }

    public void CleanVirtualCameraFollow()
    {
        virtualCamera.Follow = null;
    }

    public void StartAutoScroll()
    {
        virtualCamera.Follow = null;
        manualCamera.StartAutoScroll();
    }

    public void StopAutoScroll()
    {
        virtualCamera.transform.position = Player.transform.position;
        virtualCamera.Follow = Player.transform;
        manualCamera.StopAutoScroll();
    }

    public void TPPlayerToPosition(Vector3 pos)
    {
        KillJellyfish();
        Vector3 playerPos = Player.transform.position;
        Vector3 delta = pos - playerPos;
        Player.transform.position = pos;
        virtualCamera.OnTargetObjectWarped(Player.transform, delta);
    }

    public void NextCheckpoint()
    {
        if (currentCheckpoint + 1 >= maxCheckpointsSize)
        {
            currentCheckpoint = 0;
        }
        else
        {
            currentCheckpoint++;
        }
    }

    public void UpdateCheckpoint(int index)
    {
        if (index >= maxCheckpointsSize)
        {
            Debug.Log("Indice de checkpoint no valido");
        }
        else
        {
            currentCheckpoint = index;
        }
    }

    public void RespawnPlayer()
    {
        if (currentCheckpoint >=3)
        {
            SoundManager.Instance.PlaySound("SIRENA", 0.7f);
        }
        TPPlayerToPosition(checkpoints[currentCheckpoint].transform.position);
        Player.GetComponent<OxygenBar>().AddOxygen(Player.GetComponent<OxygenBar>().GetMaxOxygen());
    }

    public void UpdateOxygenByLevel()
    {
        Player.GetComponent<OxygenBar>().SetStartLevelOxygen(oxygenByLevel[currentCheckpoint]);
    }

    public void NextMap()
    {
        //CurrentMap++;
        GlitchedMusicRatio = 0.33f * currentCheckpoint;
        NextCheckpoint();
        UpdateOxygenByLevel();
        RespawnPlayer();
        ExitShop();
    }
}