using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [SerializeField]
    private float MinDistance;
    [SerializeField]
    private float MaxDistance;
    [SerializeField]
    private float BaseVolumen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float DistanceToPlayer = Vector2.Distance(GameManager.Instance.GetPlayerPos(), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));
        DistanceToPlayer = Mathf.Clamp(DistanceToPlayer, MinDistance, MaxDistance);
        //Debug.Log(DistanceToPlayer);
        gameObject.GetComponent<AudioSource>().volume = BaseVolumen * ((MaxDistance - DistanceToPlayer) / (MaxDistance - MinDistance));
    }
}
