using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawn_FromZ2 : MonoBehaviour
{
    private bool activated = false;

    void Awake()
    {
        if (!activated && GameObject.Find("GameController").GetComponent<GameController>().GameStarted())
        {
            GameObject.Find("Player").transform.GetComponent<NavMeshAgent>().enabled = false;
            GameObject.Find("Player").transform.position = transform.position;
            GameObject.Find("Player").transform.GetComponent<NavMeshAgent>().enabled = true;
            Camera.main.transform.GetComponent<CameraController>().minPos = new Vector2(-30, 18f);
            Camera.main.transform.GetComponent<CameraController>().maxPos = new Vector2(728, 578);
            activated = true;

        }
    }

    public void Unblock() { activated = false; }

}
