using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawn_FromZ0_ToZ1 : MonoBehaviour
{
    private static bool activated = false;

    void Awake()
    {
        if (!activated && GameObject.Find("GameController").GetComponent<GameController>().GameStarted())
        {
            GameObject.Find("Player").transform.GetComponent<NavMeshAgent>().enabled = false;
            GameObject.Find("Player").transform.position = transform.position;
            GameObject.Find("Player").transform.GetComponent<NavMeshAgent>().enabled = true;
            Camera.main.transform.GetComponent<CameraController>().minPos = new Vector2(-168, -63);
            Camera.main.transform.GetComponent<CameraController>().maxPos = new Vector2(168, 63);
            activated = true;

        }
    }

    public static void Unblock() { activated = false; }

}
