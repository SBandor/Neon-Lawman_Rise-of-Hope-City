using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampoVisionTrigger : MonoBehaviour
{
    private bool alertado = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            transform.Find("EnemyBody").GetComponent<EnemyController>().SetPlayerInSight(true);
            if(!alertado)
            {
                transform.Find("EnemyBody").GetComponent<EnemyController>().Expresar("Atencion");
            }     
        } 
    }
   
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            transform.Find("EnemyBody").GetComponent<EnemyController>().SetPlayerInSight(false);
            if(alertado)
            {
                alertado = false;
            }
        }      
    }
}
