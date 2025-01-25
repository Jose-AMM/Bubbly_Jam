using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera Camera;
    [SerializeField] private List<BlackPanelLogic> Interfaces;
    [SerializeField] private GameObject Player; 
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
        foreach (BlackPanelLogic Interface in Interfaces){
        Interface.gameObject.SetActive(true);}

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
            foreach (BlackPanelLogic Interface in Interfaces){
                if (Interface.gameObject.activeSelf == true){
                    Interface.StartFadeOut();
                    return;
                }
            }
        }
    }

    public void DisablePlayer(){
        Player.SetActive(false);
        Player.GetComponent<PlayerMovement>().Stop();
    }
    public void EnablePlayer(){
        Player.GetComponent<PlayerMovement>().AsNew();
        Player.SetActive(true);
    }
}
