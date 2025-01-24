using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMng : MonoBehaviour
{
    [SerializeField]
    List<String> Scenes = new List<String>();
    [SerializeField]
    List<String> GameobjectsToDisable = new List<String>();
    public static SceneMng Instance  { get; private set; }
    Stack<String> SceneStack = new Stack<String>();
    private String CurrentScene;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        
        CurrentScene = SceneManager.GetActiveScene().name;
        foreach (String SceneName in Scenes)
        {
            Debug.Log("Checking " + SceneName);
            Debug.Log("Current Scene " + CurrentScene);
            if (SceneName != CurrentScene)
            { 
                Debug.Log("Starting load " + SceneName);
                StartCoroutine(LoadSceneAsync(SceneName));
            }
        }
        
        //SceneStack.Push(SceneManager.GetActiveScene().name);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void EnableScene(String SceneName)
    {
        foreach (GameObject rootObject in SceneManager.GetSceneByName(CurrentScene).GetRootGameObjects())
        {
            if (GameobjectsToDisable.Contains(rootObject.name))
            {
                rootObject.SetActive(false);
                Debug.Log($"Disabled GameObject '{rootObject.name}' in scene '{CurrentScene}'");
            }
            
        }

        foreach (GameObject rootObject in SceneManager.GetSceneByName(SceneName).GetRootGameObjects())
        {
            if (rootObject.name == "Root")
            {
                rootObject.SetActive(true);
                Debug.Log($"Enabled GameObject '{rootObject.name}' in scene '{SceneName}'");
                return;
            }
        }
    }

    void LoadScene(String SceneName)
    {
        foreach (GameObject rootObject in SceneManager.GetSceneByName(SceneName).GetRootGameObjects())
        {
            if (rootObject.name == "root")
            {
                rootObject.SetActive(false);
                Debug.Log($"Disabled GameObject '{rootObject.name}' in scene '{SceneName}'");
                return;
            }
        }
    }

    private IEnumerator LoadSceneAsync(string SceneName)
    {
        Debug.Log("Added scene: " + SceneName);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);

        while (asyncLoad is { isDone: false })
        {
            yield return null; // Wait for the next frame
        }

        DisableRoot(SceneName);
    }

    private void DisableRoot(String SceneName)
    {
        foreach (GameObject rootObject in SceneManager.GetSceneByName(SceneName).GetRootGameObjects())
        {
            if (rootObject.name == "Root")
            {
                rootObject.SetActive(false);
                Debug.Log($"Disabled GameObject '{rootObject.name}' in scene '{SceneName}'");
                return; // Exit after finding and disabling the target object
            }
        }
    }
}
