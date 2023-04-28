using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("GAME CONTROLLER COMPONENTS")]
    [SerializeField]
    private GameObject playerObject;
    [SerializeField]
    private GameObject startPosition;
    [SerializeField]
    private GameObject startGamePosition;
    [SerializeField]
    private GameObject particleCocheFlipPosition;
    [SerializeField] 
    private GameObject particleTuboEscapeFlipPosition;
    [SerializeField]
    private GameObject mainCameraObject;
    [SerializeField]
    private GameObject audioManagerObject;
   
    private bool gameStarted = false, introTaxi=false, introTaxi2=false, introTaxi3=false, introTaxiFinal=false;
    private bool flipDrift = false, taxiRotated = false, taxiRotatedB = false, nitroDialogo=false;
    private float carSpeed = 80f;
    public bool GameStarted() { return gameStarted; }
    [Header("INTRODUCCIÓN TAXI")]
    [SerializeField]
    private GameObject nitromovil;
    [SerializeField] 
    private GameObject introTaxiMeta1;
    [SerializeField] 
    private GameObject introTaxiMeta2;
    [SerializeField] 
    private GameObject introTaxiMeta3;
    [SerializeField]
    private GameObject nitroExpresionPosition;
    [SerializeField]
    private GameObject nitroBocaPosition;
    [SerializeField]
    private GameObject contento;
    [SerializeField]
    private GameObject hablar;
    [SerializeField]
    private GameObject boca;
    [Header("ENEMIGOS DE ZONA")]
    [SerializeField]
    private GameObject enemy1_Object;
    static int spawnEnemigos_Zona1 = 0, enemigosDerrotados_Zona1=0, enemigosTotales_Zona1=3;
    public static void IncrementarEnemigosDerrotadosZona1() { enemigosDerrotados_Zona1++; }

    void Awake()
    {
        if (FindObjectOfType(typeof(PlayerController)) == null)
        {
            DontDestroyOnLoad(this);
            GameObject go=  Instantiate(playerObject, startPosition.transform.position, startPosition.transform.rotation);
            go.name = "Player";
            Instantiate(mainCameraObject, startPosition.transform.position, startPosition.transform.rotation);
            if (FindObjectOfType(typeof(AudioManager)) == null) Instantiate(audioManagerObject, Vector3.zero, Quaternion.Euler(Vector3.zero));
            Camera.main.transform.GetComponent<CameraController>().enabled = true;
            Camera.main.transform.GetComponent<CameraController>().target = GameObject.Find("Player").transform;
            Camera.main.transform.GetComponent<CameraController>().minPos = new Vector2(-868, -63);
            Camera.main.transform.GetComponent<CameraController>().maxPos = new Vector2(-533, 63);
            Camera.main.orthographicSize = 180f;
            introTaxi = true;
        }
        else { Destroy(this); }        
    }

    private void Start()
    {
        Spawn_FromZ1_InZ0.Unblock();
    }
    private void Update()
    {
        IntroduccionTaxi();
        if (gameStarted)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                spawnEnemigos_Zona1 = 0;                  
            }
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {             
                if ((enemigosTotales_Zona1-enemigosDerrotados_Zona1)> 0 && spawnEnemigos_Zona1+1<=3)
                {               
                    Transform enemySpawn1 = GameObject.Find("EnemySpawn"+(spawnEnemigos_Zona1 + 1).ToString()).transform;
                    GameObject go = Instantiate(enemy1_Object, enemySpawn1.position, enemySpawn1.rotation);
                    go.name = "Enemy";              
                    spawnEnemigos_Zona1++;       
                }               
            }          
        }
    }

    void IntroduccionTaxi()
    {
        GameObject player = GameObject.Find("Player");     
        float size = Camera.main.orthographicSize;    
        if (introTaxi && !introTaxi2)
        {         
            if (nitromovil.transform.position.x <= introTaxiMeta1.transform.position.x)
            {
                player.GetComponent<PlayerController>().CameraStoryMode(true);
                player.GetComponent<PlayerController>().StoryMode(true);
                player.GetComponent<PlayerController>().SetMovement(false);
                player.GetComponent<SpriteRenderer>().enabled = false;
                player.transform.Find("PlayerBody").GetComponent<SpriteRenderer>().enabled = false;
                player.transform.Find("PlayerBody").GetComponent<Collider2D>().enabled = false;
                player.transform.Find("PlayerBody").GetComponent<Rigidbody2D>().simulated = false;
                player.GetComponent<NavMeshAgent>().enabled = false;
                player.transform.position = startPosition.transform.position;
                nitromovil.transform.Translate(Vector3.right * carSpeed * Time.deltaTime, Space.World);
            }
            else
            {
                nitromovil.transform.Find("Ruedas").GetComponent<Animator>().SetBool("Stop", true);
                nitromovil.transform.Find("Nitro").GetComponent<Animator>().SetBool("Stop", true);
                nitromovil.transform.Find("Prota").GetComponent<Animator>().SetBool("Stop", true);
                Invoke("PlayerExitTaxi", 4f);
                introTaxi2 = true;
                player.transform.position = startGamePosition.transform.position;
                player.GetComponent<PlayerController>().CameraStoryMode(false);            
            }     
        }

        if(introTaxi2 && introTaxi)
        {
            if (Camera.main.orthographicSize > 110f)
            {
                size += -30 * Time.deltaTime;            
            }
            else 
            { 
                Camera.main.orthographicSize = 110f;                         
                introTaxi = false;
                introTaxi2 = false;
                introTaxi3 = true;              
            }
            Camera.main.orthographicSize = size;           
        }
        if(introTaxi3 && !introTaxiFinal)
        {
            if (!nitroDialogo ) NitroDialogo();
            if (nitroDialogo)
            {         
                Invoke("TaxiFlipDrift", 8f);           
            }
        }       
    }
    void PlayerExitTaxi()
    {
        nitromovil.transform.Find("Prota").GetComponent<SpriteRenderer>().enabled = false;
        GameObject player = GameObject.Find("Player");
        player.GetComponent<SpriteRenderer>().enabled = true;
        player.transform.Find("PlayerBody").GetComponent<SpriteRenderer>().enabled = true;
        player.transform.Find("PlayerBody").GetComponent<Collider2D>().enabled = true;
        player.transform.Find("PlayerBody").GetComponent<Rigidbody2D>().simulated = true;
        player.GetComponent<NavMeshAgent>().enabled = true;
        
    }
    void TaxiFlipDrift()
    {      
        if (!taxiRotated) 
        {
            nitromovil.transform.Find("Ruedas").GetComponent<Animator>().SetBool("Stop", false);
            nitromovil.transform.Rotate(new Vector3(0, 0, 45), Space.World);
            nitromovil.transform.position = new Vector3(nitromovil.transform.position.x - 20, nitromovil.transform.position.y + 70, nitromovil.transform.position.z);
            taxiRotated = true;          
        }     
        if (nitromovil.transform.position.x <= introTaxiMeta2.transform.position.x && taxiRotated && !flipDrift)
        {            
            nitromovil.transform.Translate(new Vector2(1, 1)*carSpeed*Time.deltaTime, Space.World);         
        }
        else if (nitromovil.transform.position.x > introTaxiMeta2.transform.position.x && taxiRotated && !flipDrift)
        {
            AcabarExpresionLoop();
            Expresar("Contento");
            nitromovil.transform.position = new Vector3(nitromovil.transform.position.x , nitromovil.transform.position.y - 70, nitromovil.transform.position.z);
            nitromovil.transform.Find("ParticleCoche").transform.position = particleCocheFlipPosition.transform.position;
            nitromovil.transform.Find("ParticleTuboEscape").transform.position = particleTuboEscapeFlipPosition.transform.position;
            var psCocheMain = nitromovil.transform.Find("ParticleCoche").GetComponent<ParticleSystem>().main;
            var psCocheEmission = nitromovil.transform.Find("ParticleCoche").GetComponent<ParticleSystem>().emission;
            var psTuboEscapeMain = nitromovil.transform.Find("ParticleTuboEscape").GetComponent<ParticleSystem>().main;
            psCocheMain.startLifetime = 2f;
            psCocheEmission.rateOverDistance = 70f;
            psTuboEscapeMain.startLifetime = 1f;

            PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
            playerController.ResetDialogo();
            
            flipDrift = true;                     
        }
        if(flipDrift )
        {
            if (!taxiRotatedB) 
            { 
                nitromovil.transform.Rotate(new Vector3(0, 0, -90), Space.World);
                Invoke("TaxiFlipDriftFinal", 0.15f);
                nitromovil.transform.Find("Nitro").GetComponent<SpriteRenderer>().enabled = false;
                nitromovil.transform.Find("Coche").GetComponent<SpriteRenderer>().flipX = true;
                nitromovil.transform.Find("Ruedas").GetComponent<SpriteRenderer>().flipX = true;
                nitromovil.transform.Find("Cartel").GetComponent<SpriteRenderer>().flipX = true;             
                taxiRotatedB = true; 
            }          
            if (nitromovil.transform.position.x >= introTaxiMeta3.transform.position.x && taxiRotatedB)
            {             
                nitromovil.transform.Translate(Vector2.left * (carSpeed*3) * Time.deltaTime, Space.World);
                GameObject.Find("Player").GetComponent<PlayerController>().SetMovement(true);
                GameObject.Find("Player").GetComponent<PlayerController>().StoryMode(false);
                gameStarted = true;
                introTaxiFinal = true;
            }                   
        }       
    }
    void TaxiFlipDriftFinal() { nitromovil.transform.Rotate(new Vector3(0, 0, 45), Space.World);}
    void NitroDialogo()
    {
        PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        //Max char por frase 127.    
        playerController.DialogoHUD("Nitro",
        "- Lo siento, pero solo puedo acercarte hasta aquí. La cosa se ha puesto peligrosa más adelante..",0.05f);
        Expresar("Hablar");
        nitroDialogo = true;       
    }
    //---- FUNCIONES DE EXPRESIONES ---- //////////////////////////////////////////////////////////////////
    public void Expresar(string expresion)
    {
        switch (expresion)
        {
           
            case "Contento": Instantiate(contento, nitroExpresionPosition.transform,false); break;
            case "Hablar":
                Instantiate(hablar, nitroExpresionPosition.transform,false);
                Instantiate(boca, nitroBocaPosition.transform,false); break;
        }
    }
    public void AcabarExpresionLoop()
    {
        if (nitroExpresionPosition.transform.childCount > 0) { Destroy(nitroExpresionPosition.transform.GetChild(0).gameObject); }
        if (nitroBocaPosition.transform.childCount > 0) { Destroy(nitroBocaPosition.transform.GetChild(0).gameObject); }
    }
}

    

