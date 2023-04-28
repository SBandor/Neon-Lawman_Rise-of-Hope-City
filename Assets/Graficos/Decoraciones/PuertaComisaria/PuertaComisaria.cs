using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PuertaComisaria : MonoBehaviour
{
    PlayerController player;
    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if(collision.transform.parent.GetComponent<PlayerController>())
        {
            player = collision.transform.parent.GetComponent<PlayerController>();
            player.DoorContact(true);                                
        }            
    }
    private void OnTriggerStay2D(Collider2D collision)
    {     
        if (player.IsDoorContact() && player.IsDoorInteraction())
        {
            GetComponent<Animator>().SetBool("Abierta", true);
            Invoke("TeleportToComisaria",1.5f);  
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    { 
        if (collision.transform.parent.GetComponent<PlayerController>())
        {       
            player.DoorContact(false);
            player.DoorInteraction(false);
            GetComponent<Animator>().SetBool("Abierta", false);       
        }
    }  
    private void TeleportToComisaria()
    {     
        Spawn_FromZ1_InComsaria.Unblock();
        SceneManager.LoadScene(2);
        player.DoorContact(false);
        player.DoorInteraction(false);
    }
}
