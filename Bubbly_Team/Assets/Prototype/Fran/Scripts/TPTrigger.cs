using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TPTrigger : MonoBehaviour
{
    [SerializeField] private GameObject respawn;

    void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.Instance.TPPlayerToPosition(respawn.transform.position);
    }
}