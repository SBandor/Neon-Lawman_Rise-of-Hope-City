using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
