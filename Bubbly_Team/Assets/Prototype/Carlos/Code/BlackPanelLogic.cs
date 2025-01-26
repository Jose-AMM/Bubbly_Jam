using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BlackPanelLogic : MonoBehaviour
{
    private Image fadePanel;
    public bool startActivated = false;
    public float fadeDuration = 1.0f;

    private void Awake()
    {
        fadePanel = GetComponent<Image>();
    }


    private void Start()
    {
        Color startColor = fadePanel.color;

        if (startActivated)
        {
            startColor.a = 1.0f;
        }
        else
        {
            startColor.a = 0.0f;
        }

        fadePanel.color = startColor;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
            StartFadeOut();

        if(Input.GetKeyDown(KeyCode.I))
            StartFadeIn();
    }

    public void StartFadeIn()
    {
        if(fadePanel.color.a == 0.0f)
            StartCoroutine(FadeIn());
    }

    public void StartFadeOut()
    {
        if (fadePanel.color.a == 1.0f)
            StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {

        Color color = fadePanel.color;
        color.a = 0.0f;
        fadePanel.color = color;
        gameObject.SetActive(true);

        float elapsedTime = 0f;
        color = fadePanel.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1;
        fadePanel.color = color;
    }

    private IEnumerator FadeOut()
    {
        Color color = fadePanel.color;
        color.a = 1.0f;
        fadePanel.color = color;
        gameObject.SetActive(true);

        float elapsedTime = 0f;
        color = fadePanel.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 0;
        fadePanel.color = color;

        gameObject.SetActive(false);
    }

}
