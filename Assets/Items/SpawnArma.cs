using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArma : MonoBehaviour
{
    [SerializeField]
    private GameObject cuchillo;
    [SerializeField]
    private GameObject botella;
    [SerializeField]
    private GameObject palo;
    [SerializeField]
    private GameObject porra;
    [SerializeField]
    private GameObject tuberia;
    [SerializeField]
    private GameObject roca;
    [SerializeField]
    private GameObject caja;

    private void Start()
    {
        if(PlayerPrefs.GetInt(gameObject.name) == 0)
        {
            GameObject go;
            switch (gameObject.name)
            {
                case "SpawnCuchillo": 
                    go= Instantiate(cuchillo, transform.position,cuchillo.transform.rotation);
                    go.name = "Cuchillo";
                    break;
                case "SpawnBotella":
                    go = Instantiate(botella, transform.position, botella.transform.rotation);
                    go.name = "Botella";               
                    break;
                case "SpawnPalo":
                    go = Instantiate(palo, transform.position, palo.transform.rotation);
                    go.name = "Palo";
                    break;
                case "SpawnPorra":
                    go = Instantiate(porra, transform.position, porra.transform.rotation);
                    go.name = "Porra";
                    break;
                case "SpawnTuberia":
                    go = Instantiate(tuberia, transform.position, tuberia.transform.rotation);
                    go.name = "Tuberia";
                    break;
                case "SpawnRoca":
                    go = Instantiate(roca, transform.position, roca.transform.rotation);
                    go.name = "Roca";
                    break;
                case "SpawnCaja":
                    go = Instantiate(caja, transform.position, caja.transform.rotation);
                    go.name = "Caja";
                    break;
            }
            PlayerPrefs.SetInt(gameObject.name, 1);
        }  
    }
}
