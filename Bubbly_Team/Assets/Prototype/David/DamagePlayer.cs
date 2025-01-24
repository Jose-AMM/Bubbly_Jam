using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    //TO DO: Hacer GameManager Instance para conseguir el player
    [SerializeField] OxygenBar player;
    [SerializeField] int damage;

    //TO DO: Modificar todo esto cuando tengamos el player y el movimiento
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Arreglar collision con el player
        if (player != null) 
        {
            player.Damaged(damage);
            player.InContactWithEnemy(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        player.InContactWithEnemy(false);
    }
}
