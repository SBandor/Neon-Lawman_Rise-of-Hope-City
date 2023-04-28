using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneda : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            var playerController = collision.transform.parent.GetComponent<PlayerController>();
            playerController.GetComponent<RPG_Stats>().SetDinero(
            playerController.GetComponent<RPG_Stats>().GetDinero() + 50);
            Destroy(gameObject);
        }
    }
}
