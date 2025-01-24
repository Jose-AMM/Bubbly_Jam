using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    public void PlayGame()
    {
        // fade in black panel
        Debug.Log("Loading game scene.");

        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        Debug.Log("Closing the game.");
        Application.Quit();
    }
}
