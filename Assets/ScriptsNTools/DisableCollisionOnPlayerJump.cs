using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DisableCollisionOnPlayerJump : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
    }
    void FixedUpdate()
    {
        if (player.transform.GetComponent<PlayerController>().IsJumping())
        {
            GetComponent<NavMeshObstacle>().enabled=false;            
        }
        else
        {
            GetComponent<NavMeshObstacle>().enabled = true;           
        }
    }
}
