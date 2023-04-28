using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawn_FromPiso0_InPiso1_Escalera : MonoBehaviour
{
    private static bool activated = true;

    void Awake()
    {
        if (!activated && GameObject.Find("GameController").GetComponent<GameController>().GameStarted())
        {
            GameObject.Find("Player").transform.GetComponent<NavMeshAgent>().enabled = false;
            GameObject.Find("Player").transform.position = transform.position;
            GameObject.Find("Player").transform.GetComponent<NavMeshAgent>().enabled = true;
            Camera.main.transform.GetComponent<CameraController>().minPos = new Vector2(-495, 424);
            Camera.main.transform.GetComponent<CameraController>().maxPos = new Vector2(329, 493);
            activated = true;

        }
    }

    public static void Unblock() { activated = false; }
}
