using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public GameObject InterfacePanel;
    //public Scene SceneToAdd;

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

            GameManager.Instance.BlackScreenFadeIn();

            Destroy(gameObject);
        }
    }
}
