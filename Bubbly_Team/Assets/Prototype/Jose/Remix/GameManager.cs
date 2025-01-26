using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera Camera;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] public GameObject Player;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private ManualCamera manualCamera;

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
        DisablePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Camera.m_Lens.OrthographicSize = 5.5f;
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            Camera.m_Lens.OrthographicSize = 10;
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            Camera.m_Lens.OrthographicSize = 15;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            virtualCamera.Follow = null;
            manualCamera.StartShakeCamera();
        }
    }

    public void DisablePlayer()
    {
        Player.GetComponent<PlayerMovement>().Stop();
        Player.GetComponent<JellyfishFloatSimple>().enabled = true;
        Cursor.visible = true;
    }

    public void EnablePlayer()
    {
        Player.GetComponent<PlayerMovement>().AsNew();
        Player.GetComponent<JellyfishFloatSimple>().enabled = false;
        //Player.SetActive(true);
        Cursor.visible = false;
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
        Vector3 playerPos = Player.transform.position;
        Vector3 delta = pos - playerPos;
        Player.transform.position = pos;

        virtualCamera.OnTargetObjectWarped(Player.transform, delta);
    }
}