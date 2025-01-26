using UnityEngine;

public class JellyfishFloatSimple : MonoBehaviour
{
    public float verticalAmplitude = 0.5f;
    public float verticalSpeed = 1f;
    public float lateralAmplitude = 0.3f;
    public float lateralSpeed = 0.5f;

    private Vector3 startPosition;
    private float timeOffset;

    void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        float verticalOffset = Mathf.Sin(Time.time * verticalSpeed + timeOffset) * verticalAmplitude;
        float lateralOffset = Mathf.Cos(Time.time * lateralSpeed + timeOffset) * lateralAmplitude;

        transform.position = startPosition + new Vector3(lateralOffset, verticalOffset, 0f);
    }

    void OnEnable()
    {
        startPosition = transform.position;
    }
}
