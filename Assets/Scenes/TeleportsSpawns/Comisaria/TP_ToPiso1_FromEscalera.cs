using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TP_ToPiso1_FromEscalera : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!collision.transform.parent.GetComponent<PlayerController>().IsJumping())
            {
                Invoke("LoadScene", 1f);
            }
            

        }

    }
    private void LoadScene()
    {
        SceneManager.LoadScene(3);
        Spawn_FromPiso0_InPiso1_Escalera.Unblock();
    }
}
