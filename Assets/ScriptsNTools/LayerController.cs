using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject escaleraFrente;
    [SerializeField]
    private GameObject muroEscalera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        escaleraFrente.GetComponent<SpriteRenderer>().sortingOrder = 5;
        muroEscalera.GetComponent<SpriteRenderer>().sortingOrder = 5;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        escaleraFrente.GetComponent<SpriteRenderer>().sortingOrder = 2;
        muroEscalera.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

}
