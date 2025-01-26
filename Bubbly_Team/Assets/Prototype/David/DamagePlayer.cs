using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    private OxygenBar oxygenBar;
    [SerializeField] int damage;


    private void Start()
    {
        oxygenBar = GameManager.Instance.Player.GetComponent<OxygenBar>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (oxygenBar != null) 
        {
            oxygenBar.Damaged(damage);
            oxygenBar.InContactWithEnemy(true);
            //player.Damaged(damage);
            //player.InContactWithEnemy(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        oxygenBar.InContactWithEnemy(false);
        //player.InContactWithEnemy(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (oxygenBar != null)
        {
            oxygenBar.Damaged(damage);
            oxygenBar.InContactWithEnemy(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        oxygenBar.InContactWithEnemy(false);
    }
}
