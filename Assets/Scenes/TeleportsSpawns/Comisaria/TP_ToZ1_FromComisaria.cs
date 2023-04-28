using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TP_ToZ1_FromComisaria : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Invoke("LoadScene", 1f);
        }
    }
    private void LoadScene()
    {
        SceneManager.LoadScene(1);
        Spawn_FromComisaria_InZ1.Unblock();
    }
}
