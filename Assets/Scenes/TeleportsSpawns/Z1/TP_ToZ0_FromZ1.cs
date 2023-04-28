using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TP_ToZ0_FromZ1 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            SceneManager.LoadScene(0);
            Spawn_FromZ1_InZ0.Unblock();
        }
       
    }
}
