using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemColectable : MonoBehaviour
{
    private bool matchFound = false;
    void Awake()
    {
        if (PlayerPrefs.GetInt(gameObject.name)==1)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PlayerPrefs.GetInt(gameObject.name)==0)
        {
            if (collision.tag == "Player")
            {
                var playerController = collision.transform.parent.GetComponent<PlayerController>();
                foreach (GameObject item in playerController.GetListaItems())
                {
                    if (item.GetComponent<Item>().GetNombre().Equals(gameObject.GetComponent<Item>().GetNombre()))
                    {          
                        item.GetComponent<Item>().SetCantidad(item.GetComponent<Item>().GetCantidad() + 1);
                        matchFound = true;
                    }
                }
                if (!matchFound)
                {
                    playerController.AddItemToLista(gameObject);
                }
                gameObject.transform.SetParent(GameObject.Find("ItemsEscena").transform);
                gameObject.SetActive(false);       
                PlayerPrefs.SetInt(gameObject.name, 1);
            }
        }
    }       
}
