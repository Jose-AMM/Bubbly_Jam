using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickable : MonoBehaviour
{
    [SerializeField] 
    private String SceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("E");
    }
    
    void OnMouseDown(){
        SceneMng.Instance.EnableScene(SceneName);
    }
}
