using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera Camera;
    [SerializeField] private BlackPanelLogic BlackPanel;
    [SerializeField] private GameObject Player; 
    [SerializeField] private List<GameObject> ShopPrefabs;
    private GameObject InstantiatedShop;
    public static GameManager Instance  { get; private set; }


    void Awake(){
        
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
        if (Input.GetKey(KeyCode.F)){
            ExitShop("Triangle");
        }
    }

    public Vector2 GetPlayerPos(){
        return new Vector2(Player.transform.position.x, Player.transform.position.y);
    }
    public void DisablePlayer(){
        Player.SetActive(false);
        Player.GetComponent<PlayerMovement>().Stop();
        //Player.transform.Find("Canvas/OxigenBar").GetComponent<OxygenBar>().StopDrowning();
    }
    public void EnablePlayer(){
        Player.SetActive(true);
        Player.GetComponent<PlayerMovement>().AsNew();
        GetOxygen();
        //Player.transform.Find("Canvas/OxigenBar").GetComponent<OxygenBar>().StartDrowning();
    }

    public void GetOxygen(){
        Player.transform.Find("Canvas/OxigenBar").GetComponent<OxygenBar>().AddOxygen(100.0f);
        SoundManager.Instance.PlaySound("Gasp", 0.2f);
    }

    public void EnterShop(String PrefabName){
        SoundManager.Instance.StopSounds();
        SoundManager.Instance.SetVolume("Horror", 0.0f, 1.0f);
        SoundManager.Instance.BeginClip("Goofy", 0.0f);
        SoundManager.Instance.SetVolume("Goofy", 1.0f, 1.0f);
        //SoundManager.Instance.PlaySound("ShopVFX", 1.0f);
        StartCoroutine(LoadShop(PrefabName));
    }

    public void ExitShop(String PrefabName){
        SoundManager.Instance.StopSounds();
        SoundManager.Instance.SetVolume("Horror", 1.0f, 1.0f);
        SoundManager.Instance.SetVolume("Goofy", 0.0f, 1.0f);
        StartCoroutine(UnloadShop(PrefabName));
    }

    IEnumerator LoadShop(String PrefabName){
        BlackPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0);
        BlackPanel.StartFadeIn();
        DisablePlayer();
        yield return new WaitForSeconds(BlackPanel.fadeDuration);
        Debug.Log("Load Prefab: " + PrefabName);
        foreach (GameObject ShopPrefab in ShopPrefabs){
            if (ShopPrefab.name == PrefabName){
                InstantiatedShop = Instantiate(ShopPrefab, Vector3.zero, Quaternion.identity);
            }  
        }
        yield return new WaitForSeconds(0.2f);
        BlackPanel.StartFadeOut();
        yield return new WaitForSeconds(BlackPanel.fadeDuration);
        Debug.Log("Start dialogue");
    }
    IEnumerator UnloadShop(String PrefabName){
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
}
