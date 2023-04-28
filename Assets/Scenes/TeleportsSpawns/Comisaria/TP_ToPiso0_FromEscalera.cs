using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TP_ToPiso0_FromEscalera : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if(!collision.transform.parent.GetComponent<PlayerController>().IsJumping())
            {
                Invoke("LoadScene", 1f);
            }         
        }
    }
   private void LoadScene()
    {
        SceneManager.LoadScene(2);
        Spawn_FromPiso1_InPiso0_Escalera.Unblock();
    }
}
