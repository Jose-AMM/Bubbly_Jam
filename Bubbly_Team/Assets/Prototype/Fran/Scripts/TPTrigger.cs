using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TPTrigger : MonoBehaviour
{
    [SerializeField] private GameObject respawn;

    void OnTriggerEnter2D()
    {
        GameManager.Instance.TPPlayerToPosition(respawn.transform.position);
    }
}