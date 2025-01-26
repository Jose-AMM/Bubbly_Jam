using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteGlitchAnim : MonoBehaviour
{
    [Header("Sprites Configuration")]
    public Image uiImage; // Image del objeto UI
    public Sprite baseSprite; // Sprite base (est�tico)
    public List<Sprite> glitchSprites; // Lista de sprites para el efecto glitch

    [Header("Glitch Settings")]
    [Tooltip("Duraci�n m�nima entre cambios de sprite.")]
    public float minGlitchInterval = 0.05f;

    [Tooltip("Duraci�n m�xima entre cambios de sprite.")]
    public float maxGlitchInterval = 0.2f;

    [Tooltip("Duraci�n del efecto glitch en segundos.")]
    public float glitchDuration = 1.0f;

    [Tooltip("Radio m�ximo para el offset aleatorio del glitch.")]
    public float maxOffsetRadius = 5.0f;

    [Tooltip("Tiempo m�nimo entre glitches autom�ticos.")]
    public float minTimeBetweenGlitches = 2.0f;

    [Tooltip("Tiempo m�ximo entre glitches autom�ticos.")]
    public float maxTimeBetweenGlitches = 5.0f;

    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private bool isGlitching = false;
    private float nextGlitchTime;

    private void Start()
    {
        if (uiImage == null)
        {
            uiImage = GetComponent<Image>();
        }

        rectTransform = uiImage.GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        if (baseSprite != null)
        {
            uiImage.sprite = baseSprite;
        }

        ScheduleNextGlitch();
    }

    private void Update()
    {
        if (!isGlitching && Time.time >= nextGlitchTime)
        {
            StartGlitch();
            ScheduleNextGlitch();
        }
    }

    private void ScheduleNextGlitch()
    {
        nextGlitchTime = Time.time + Random.Range(minTimeBetweenGlitches, maxTimeBetweenGlitches);
    }

    public void StartGlitch()
    {
        if (!isGlitching)
        {
            StartCoroutine(GlitchEffect());
        }
    }

    private IEnumerator GlitchEffect()
    {
        isGlitching = true;

        float elapsedTime = 0f;
        while (elapsedTime < glitchDuration)
        {
            // Cambiar a un sprite aleatorio de la lista o al base
            Sprite newSprite = (Random.value > 0.5f && glitchSprites.Count > 0)
                ? glitchSprites[Random.Range(0, glitchSprites.Count)]
                : baseSprite;

            uiImage.sprite = newSprite;

            // Aplicar un offset aleatorio dentro del radio permitido
            Vector2 randomOffset = Random.insideUnitCircle * maxOffsetRadius;
            rectTransform.anchoredPosition = originalPosition + new Vector3(randomOffset.x, randomOffset.y, 0);

            // Esperar un tiempo aleatorio antes de cambiar de nuevo
            float waitTime = Random.Range(minGlitchInterval, maxGlitchInterval);
            yield return new WaitForSeconds(waitTime);

            elapsedTime += waitTime;
        }

        // Restaurar el sprite base y la posici�n original al finalizar
        uiImage.sprite = baseSprite;
        rectTransform.anchoredPosition = originalPosition;
        isGlitching = false;
    }
}
