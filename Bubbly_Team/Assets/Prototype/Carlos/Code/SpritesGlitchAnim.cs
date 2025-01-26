using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteGlitchAnim : MonoBehaviour
{
    [Header("Sprites Configuration")]
    public SpriteRenderer spriteRenderer; // SpriteRenderer del objeto
    public Sprite baseSprite; // Sprite base (estático)
    public List<Sprite> glitchSprites; // Lista de sprites para el efecto glitch

    [Header("Glitch Settings")]
    [Tooltip("Duración mínima entre cambios de sprite.")]
    public float minGlitchInterval = 0.05f;

    [Tooltip("Duración máxima entre cambios de sprite.")]
    public float maxGlitchInterval = 0.2f;

    [Tooltip("Duración del efecto glitch en segundos.")]
    public float glitchDuration = 1.0f;

    [Tooltip("Radio máximo para el offset aleatorio del glitch.")]
    public float maxOffsetRadius = 0.5f;

    [Tooltip("Tiempo mínimo entre glitches automáticos.")]
    public float minTimeBetweenGlitches = 2.0f;

    [Tooltip("Tiempo máximo entre glitches automáticos.")]
    public float maxTimeBetweenGlitches = 5.0f;

    private Transform spriteTransform;
    private Vector3 originalPosition;
    private bool isGlitching = false;
    private float nextGlitchTime;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        spriteTransform = spriteRenderer.transform;
        originalPosition = spriteTransform.localPosition;

        if (baseSprite != null)
        {
            spriteRenderer.sprite = baseSprite;
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

            spriteRenderer.sprite = newSprite;

            // Aplicar un offset aleatorio dentro del radio permitido
            Vector2 randomOffset = Random.insideUnitCircle * maxOffsetRadius;
            spriteTransform.localPosition = originalPosition + new Vector3(randomOffset.x, randomOffset.y, 0);

            // Esperar un tiempo aleatorio antes de cambiar de nuevo
            float waitTime = Random.Range(minGlitchInterval, maxGlitchInterval);
            yield return new WaitForSeconds(waitTime);

            elapsedTime += waitTime;
        }

        // Restaurar el sprite base y la posición original al finalizar
        spriteRenderer.sprite = baseSprite;
        spriteTransform.localPosition = originalPosition;
        isGlitching = false;
    }
}
