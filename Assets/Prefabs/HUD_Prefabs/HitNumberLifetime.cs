using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitNumberLifetime : MonoBehaviour
{
    private float speed=10f;
    private float timeMovingHitNumber=0.5f, timerHitNumber = 0.5f;
    void Update()
    {
       transform.Translate(new Vector3(0, 1*speed, 0)*Time.deltaTime);
        timeMovingHitNumber -= Time.deltaTime;
        if (timeMovingHitNumber <= 0)
        {
            timeMovingHitNumber = timerHitNumber;
            Destroy(gameObject);
        }
    }
}
