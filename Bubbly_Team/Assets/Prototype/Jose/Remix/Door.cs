using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{

    public String PrefabName;
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
            GameManager.Instance.EnterShop(PrefabName);
            var collider = gameObject.GetComponent(typeof(Collider2D)) as Collider2D;
            collider.enabled = false;
        }
    }

    private IEnumerator Recover()
    {
        yield return new WaitForSeconds(60.0f);
        var collider = gameObject.GetComponent(typeof(Collider2D)) as Collider2D;
        collider.enabled = true;
    } 
}
