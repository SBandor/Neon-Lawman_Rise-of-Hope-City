using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainTitle : MonoBehaviour
{
    private bool nuevaPartida=false, prologo=false, nextTextLineStoped=false;
    private float timeForPrologo = 3f, timeForNextTextLine=0f, timeForFinalPrologo=4f;
    private float carSpeed=7f;
    private int contadorLineasPrologo = 0, lineasPrologoTotales = 14;
    private List<GameObject> prologoLines = new List<GameObject>();
    [SerializeField]
    private GameObject nitromovil;
    [SerializeField]
    private GameObject meta;
    [SerializeField]
    private GameObject logo;
    [SerializeField]
    private GameObject panelMenus;
    [SerializeField]
    private GameObject backgroundPrologo;
    [SerializeField]
    private GameObject backgroundFader;
    [SerializeField]
    private GameObject prologoLineText;
    [SerializeField]
    private GameObject initialPosition;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void Start()
    {    
        panelMenus.transform.Find("NuevaPartida").GetComponent<Button>().Select();
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void NuevaPartida()
    {
        nuevaPartida = true;
    }
    private void Update()
    {
       if(EventSystem.current.currentSelectedGameObject==null)
       {
            panelMenus.transform.Find("NuevaPartida").GetComponent<Button>().Select();
       }
        if(nuevaPartida && !prologo)
        {
            if (nitromovil.transform.position.x <= meta.transform.position.x && nitromovil!= null)
            {
                nitromovil.transform.Translate(Vector3.right*carSpeed*Time.deltaTime, Space.World);
            }
            else
            {
                nitromovil.transform.Find("ParticleCoche").GetComponent<ParticleSystem>().Stop();
                nitromovil.transform.Find("ParticleTuboEscape").GetComponent<ParticleSystem>().Stop();
                panelMenus.GetComponent<Image>().enabled = false;
                panelMenus.transform.Find("NuevaPartida").GetComponent<Image>().enabled = false;
                panelMenus.transform.Find("NuevaPartida").GetComponent<Button>().enabled = false;
                panelMenus.transform.Find("NuevaPartida").transform.Find("Text").GetComponent<Text>().enabled = false;
                logo.SetActive(false);
                backgroundPrologo.SetActive(true);        
                prologoLineText.SetActive(true);
                prologo = true;          
            }
        }
        if(nuevaPartida && prologo)
        {
            timeForPrologo -= Time.deltaTime;
            backgroundFader.GetComponent<Image>().CrossFadeAlpha(0f,5f,false);
            if (timeForPrologo<=0)
            {    
                if (contadorLineasPrologo > lineasPrologoTotales)
                {
                    timeForFinalPrologo -= Time.deltaTime;      
                    if (timeForFinalPrologo<=0)
                    {
                        SceneManager.LoadScene(0);
                    }        
                }
                if (timeForNextTextLine <= 0 && !nextTextLineStoped)
                {
                    contadorLineasPrologo++;
                    GameObject prologoLine = Instantiate(prologoLineText, initialPosition.transform.position, initialPosition.transform.rotation, initialPosition.transform);
                    prologoLine.GetComponent<TypeWriterEffect>().fullText = PrologoTextLines(contadorLineasPrologo );
                    prologoLine.GetComponent<TypeWriterEffect>().ActivaTypeWriterEffect();
                    prologoLine.name = "PrologoLineText-" + (contadorLineasPrologo );            
                    timeForNextTextLine = 0.6f;
                    nextTextLineStoped = true;
                }   
                if (GameObject.Find("Canvas").transform.Find("Prologo").transform.Find("InitialPosition").transform.Find("PrologoLineText-" + (contadorLineasPrologo)).GetComponent<Text>().text.Length -
                   GameObject.Find("Canvas").transform.Find("Prologo").transform.Find("InitialPosition").transform.Find("PrologoLineText-" + (contadorLineasPrologo )).GetComponent<TypeWriterEffect>().fullText.Length == -1
                   && nextTextLineStoped)
                {            
                   prologoLines.Add(GameObject.Find("Canvas").transform.Find("Prologo").transform.Find("InitialPosition").transform.Find("PrologoLineText-" + contadorLineasPrologo).gameObject);                  
                    nextTextLineStoped = false;
                }
                if(!nextTextLineStoped)
                {
                    timeForNextTextLine -= Time.deltaTime;  
                }
                foreach (GameObject go in prologoLines)
                {
                    go.transform.Translate(Vector2.up * 0.25f * Time.deltaTime);
                    Color newColor = new Color32(190, 0, 21, 255); //color rojo oscuro        
                    go.GetComponent<Text>().color = newColor; //asigna el color al componente Text
                }
            }    
        }
    }
    private string PrologoTextLines(int contadorLineas)
    {
        string textLine = "";
       switch(contadorLineas)
       {
            case 1: textLine = "Hope City,."; break;
            case 2: textLine = "una vez un lugar próspero,."; break;
            case 3: textLine = "ahora una ciudad infestada de bandas criminales...."; break;
            case 4: textLine = "La policía está superada,."; break;
            case 5: textLine = "y las fuerzas del orden público han sido reducidas."; break;
            case 6: textLine = "para dejar paso a la seguridad privada.."; break;
            case 7: textLine = "El caos reina en Hope City...."; break;
            case 8: textLine = "Alguien ha logrado organizar a las bandas."; break;
            case 9: textLine = "y fortalecer su poder...."; break;
            case 10: textLine = "¿Quién es este misterioso benefactor?."; break;
            case 11: textLine = "¿Qué planes tiene para la ciudad?."; break;
            case 12: textLine = "Solo Nick Thunder, un policía que no se rinde ante la injusticia,."; break;
            case 13: textLine = "puede devolver el orden a la ciudad,."; break;
            case 14: textLine = "tras regresar de una excedencia obligatoria...."; break;
            default:break; 
       }
        return textLine;
    }
}
