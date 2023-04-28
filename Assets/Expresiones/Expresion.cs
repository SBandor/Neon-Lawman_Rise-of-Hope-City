using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expresion : MonoBehaviour
{
    void Update()
    {
        if(!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).loop)
        {
            if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
            {
                Destroy(gameObject);
            }
        }
    }
}
