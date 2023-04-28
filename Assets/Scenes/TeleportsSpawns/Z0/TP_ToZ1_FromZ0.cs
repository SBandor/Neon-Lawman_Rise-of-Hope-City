using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class TP_ToZ1_FromZ0 : MonoBehaviour
{
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            SceneManager.LoadScene(1);
            Spawn_FromZ0_ToZ1.Unblock();
        }
       
    }
}
