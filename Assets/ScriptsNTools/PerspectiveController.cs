using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveController : MonoBehaviour
{
    [SerializeField]
    private Transform referencePoint;
    public Vector3 minSize;
    public Vector3 maxSize;
    

    private Vector3 originalScale= new Vector3(24.1855602f, 7.83915043f, 14.2530003f);
    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
        player.transform.localScale = originalScale;
        minSize = originalScale;
        maxSize = originalScale / 1.30f;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, referencePoint.position);
        float t = Mathf.Clamp01((distance - 35) / (90 - 20));
        Debug.Log("t:" + t);
        Vector3 size = Vector3.Lerp(maxSize, minSize, t);
        player.transform.localScale = size;
    }
}
