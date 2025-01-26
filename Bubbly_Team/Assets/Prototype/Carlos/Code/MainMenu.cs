using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("MainMenu Out Anim")]
    public Transform targetPosition;
    public float floatDurationMM = 2.0f;
    public float floatAmplitudeMM = 0.5f;
    public float floatSpeedMM = 2.0f;
    public float lateralAmplitudeMM = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayGame()
    {
        if (targetPosition == null)
        {
            Debug.LogError("TargetPosition no asignado. Asigna un Transform en el inspector.");
            return;
        }
        
        Cursor.visible = false;

        StartCoroutine(FloatAndMove());
    }

    private IEnumerator FloatAndMove()
    {
        Vector3 startPosition = gameObject.transform.position;
        Vector3 endPosition = targetPosition.position;

        float elapsedTime = 0f;

        while (elapsedTime < floatDurationMM)
        {
            elapsedTime += Time.deltaTime;

            // Movimiento ondulante vertical.
            float verticalOffset = Mathf.Sin(elapsedTime * floatSpeedMM) * floatAmplitudeMM;

            // Movimiento ondulante lateral (zigzag).
            float lateralOffset = Mathf.Sin(elapsedTime * floatSpeedMM * 0.5f) * lateralAmplitudeMM;

            // Lerp para moverse hacia la posici�n objetivo.
            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / floatDurationMM);
            newPosition.y += verticalOffset;
            newPosition.x += lateralOffset;

            gameObject.transform.position = newPosition;

            yield return null;
        }

        gameObject.transform.position = endPosition;

        //Debug.Log("Animation Complete. Ready to start the game!");

        GameManager.Instance.EnablePlayer();
        GameManager.Instance.MakeCameraFollowPlayer();
    }


    public void QuitGame()
    {
        Debug.Log("Closing the game.");
        Application.Quit();
    }
}
