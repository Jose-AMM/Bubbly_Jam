using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject InterfacePanel;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log(other.gameObject.name + " triggering door");
        if (other.gameObject.name == "Player"){
            InterfacePanel.SetActive(true);
            InterfacePanel.GetComponent<BlackPanelLogic>().StartFadeIn();
            Destroy(gameObject);
        }
    }
}
