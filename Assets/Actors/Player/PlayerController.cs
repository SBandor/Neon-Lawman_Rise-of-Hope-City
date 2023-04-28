using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //COMPONENTES QUE CONFORMAN EL OBJETO PLAYER //////////////////////////////////////////////
    [Header("PLAYER CONTROLLER COMPONENTS")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Rigidbody2D charRB;
    [SerializeField]
    private CapsuleCollider2D charColl;
    [SerializeField]
    private CircleCollider2D attackCollDer;
    [SerializeField]
    private CircleCollider2D attackCollIzq;
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private LayerMask shadowLayerMask;
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private LayerMask itemLayer;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private ParticleSystem hitBlood;
    [SerializeField]
    private GameObject dashTrail;
    //VARIABLES QUE CONTROLAN LAS MECÁNICAS DEL PERSONAJE //////////////////////////////////////////////
    private Player_InputActions playerInputActions;
    private CircleCollider2D attackCollider;
    private float speed, normalSpeed=1f, sprintSpeed=1.5f, jumpForce=120f; // Las diferentes velocidades que puede asumir el jugador.
    private bool isGrounded = false, isSprinting = false, sprintSound = false, isJumping = false, isAttacking=false, isPunch=false, isKick=false,
                 isDoubleJump = false, isBlocking = false, secondAnimHit = false, isDead = false, isItemTargeted=false, itemToEquip=false, isThrowing=false,
                 cameraStoryMode=false, storyMode=false, doorContact=false, doorInteraction=false;
    private float lastDir;
    [SerializeField]
    private bool movement = false, gamePaused=false, estadoMenu=false; //interruptor que permite o impide el movimiento
    public void SetMovement(bool b) { movement = b; }
    public bool GamePaused() { return gamePaused; }

    //COMPONENTES DEL HUD Y VARIABLES DE PARÁMETROS DE PERSONAJE //////////////////////////////////////////////
    private RPG_Stats stats;
    private float timeToUpdateStats, timerUpdateStats = 1;
    private Camera mainCam;
    [Header("HUD COMPONENTS")]
    [SerializeField]
    private GameObject dialogoPanel;
    [SerializeField]
    private GameObject comandosPanel;
    [SerializeField]
    private GameObject menuApartados;
    [SerializeField]
    private GameObject menuPanelMain;
    [SerializeField]
    private GameObject menuEstado;
    [SerializeField]
    private GameObject pausaTitulo;
    [SerializeField]
    private GameObject itemInventario;
    [SerializeField]
    private GameObject itemPanel;
    private GameObject itemEquiped;
    private GameObject ultimoItemSelected;
    private bool descripcionMostrada, seleccionPrimerItem=true;
    private List<GameObject> listaItems = new List<GameObject>();
    public void AddItemToLista(GameObject s) {  listaItems.Add(s); }
    public List<GameObject> GetListaItems() { return listaItems; }
    public GameObject hitNumberPrefab;
    public GameObject hitNumberPosition;
    public Image saludBarra;
    public Image aguanteBarra;
    public Image armaduraBarra;
    public Text saludActualText, saludTotalText, aguanteActualText, aguanteTotalText, dineroText;
    public Text dialogoText, dialogoActor, accionPosible;
    public Text saludDato, aguanteDato, armaduraDato, absorcionArmaduraDato, fuerzaDato, agilidadDato, tecPuñoDato, tecPatadaDato, tecBloqueoDato;

    [Header("ARMAS EQUIPABLES")]
    [SerializeField]
    private Sprite cuchillo_WorldSprite;
    [SerializeField]
    private Sprite cuchillo_combatSprite;
    [SerializeField]
    private Sprite botella_WorldSprite;
    [SerializeField]
    private Sprite botella_combatSprite;
    [SerializeField]
    private Sprite porra_WorldSprite;
    [SerializeField]
    private Sprite porra_combatSprite;
    [SerializeField]
    private Sprite palo_WorldSprite;
    [SerializeField]
    private Sprite palo_combatSprite;
    [SerializeField]
    private Sprite tuberia_WorldSprite;
    [SerializeField]
    private Sprite tuberia_combatSprite;
    [SerializeField]
    private Sprite rocaSprite;
    [SerializeField]
    private Sprite cajaSprite;
    private bool durabilidadReducida = false;

    [Header("EXPRESIONES")]
    [SerializeField]
    private GameObject atencion;
    [SerializeField]
    private GameObject exclamacion;
    [SerializeField]
    private GameObject interrogacion;
    [SerializeField]
    private GameObject contento;
    [SerializeField]
    private GameObject enfadado;
    [SerializeField]
    private GameObject hablar;
    [SerializeField]
    private GameObject cantar;
    [SerializeField]
    private GameObject stun;
    [SerializeField]
    private GameObject boca;
    [SerializeField]
    private GameObject bocaPosition;
    [SerializeField]
    private GameObject expresionPosition;

    //---- FUNCIONES ESPECIALES PARA EVENTOS AUTOMÁTICOS Y CONTEXTUALES ---- //////////////////////////////////////////////////////////////////
    public void CameraStoryMode(bool b) { cameraStoryMode = b; }
    public bool CameraInStoryMode() { return cameraStoryMode; }
    public void StoryMode(bool b) { storyMode = b; }
    public bool InStoryMode() { return storyMode; }
    public void DoorContact(bool b) { doorContact = b; }
    public bool IsDoorContact() { return doorContact; }
    public void DoorInteraction(bool b) { doorInteraction = b; }
    public bool IsDoorInteraction() { return doorInteraction; }
    
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        DontDestroyOnLoad(this);

        attackCollider = attackCollDer;
        stats = GetComponent<RPG_Stats>();
        agent.updateRotation = false; //Rotación de sprite necesaria para utilizar NavMeshPlus
        agent.updateUpAxis = false;

        playerInputActions = new Player_InputActions(); //Instancia singleton de la clase que contiene los mapas y acciones de los inputs. 
        playerInputActions.Player.Sprint.started += SprintOn; //
        playerInputActions.Player.Sprint.canceled += SprintOff;  //
        playerInputActions.Player.Jump.performed += Jump; //Durante la accion jump realizada, ejecutamos la funcion Jump.
        playerInputActions.Player.Punch.performed += Punch; //Durante la accion Punch realizada, ejecutamos la funcion Punch. 
        playerInputActions.Player.Kick.performed += Kick; //Durante la accion Kick realizada, ejecutamos la funcion Kick.
        playerInputActions.Player.Block.performed += Block; //

        playerInputActions.PlayerMenu.PauseMenu.performed += PlayerMenu;
        playerInputActions.PlayerMenu.Seleccionar.performed += SeleccionApartado;
        playerInputActions.PlayerMenu.Cancelar.performed += CancelarSeleccion;

        dialogoText = dialogoPanel.transform.Find("Dialogo").GetComponent<Text>();
        dialogoActor = dialogoPanel.transform.Find("NombreActor").GetComponent<Text>();
        accionPosible = comandosPanel.transform.Find("AccionPosible").GetComponent<Text>();
    }
    //---- FUNCIONES DE MOVIMIENTO ---- //////////////////////////////////////////////////////////////////
    private void CheckMovement()
    {
        if (movement)// Determina si se permite el movimiento al jugador.
        {           
            if (isSprinting && stats.GetAguanteActual()>6)//Determinamos la velocidad de movimiento. 
            {
                if (!sprintSound) { FindObjectOfType<AudioManager>().Play("Woosh", 0.85f, 0.1f); sprintSound = true; }
                speed = sprintSpeed;                                      
                dashTrail.SetActive(true);               
            }
            else
            {
                sprintSound = false;
                speed = normalSpeed;
                dashTrail.SetActive(false);               
            }                                                               
            Movement();                             
        }
    }
    private void Movement()
    {
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();//Vector que mueve al jugador. Recibe del mapa de inputs de Player/accion Movement, un valor tipo Vector2.
        float lastDirection=0;//Ultima dirección tomada.
        
        if (inputVector != Vector2.zero && isGrounded) //Si hay input entonces se activa la animacion de caminar y se determina que dirección se ha tomado.
        {
            // FindObjectOfType<AudioManager>().Play("Step", 0.75f, 0.5f);
            animator.SetBool("isWalking", true);          
            if (inputVector.x < 0) { lastDirection = -1; }
            else if (inputVector.x>0) { lastDirection = 1; }         
        }
        else if(inputVector != Vector2.zero && !isGrounded)
        {
            animator.SetBool("isWalking", false);
            if (inputVector.x < 0) { lastDirection = -1; }
            else if (inputVector.x > 0) { lastDirection = 1; }
        }
        else
        {
            animator.SetBool("isWalking", false); // Si no hay input se vuelve a idle y se desactiva el sprint.          
            animator.SetBool("isSprinting", false);
            isSprinting = false;
        } 

        if (animator.GetBool("isBlocking"))
        {
            //inputVector = Vector2.zero;
            animator.SetBool("isWalking", false);
        }
        else
        {
            if (!isJumping)
            {
                transform.position += new Vector3(inputVector.x * speed, inputVector.y * speed, 0f); // Movemos el jugador segun el input recibido.       
            }
            else
            {
                transform.position += new Vector3(inputVector.x * sprintSpeed, inputVector.y * speed, 0f); // Movemos el jugador segun el input recibido. 
            }                            
        }
               
        if (lastDirection < 0)
        {
            spriteRenderer.flipX = true;
            attackCollDer.enabled = false;
            attackCollIzq.enabled = true;
            attackCollider = attackCollIzq;
        } 
        //Invertimos el sprite en función de la dirección tomada.
        else if (lastDirection>0)
        {
            spriteRenderer.flipX = false;
            attackCollDer.enabled = true;
            attackCollIzq.enabled = false;
            attackCollider = attackCollDer;
        }
        lastDir = lastDirection;            
    }
    private void SprintOn(InputAction.CallbackContext context)
    {
        isSprinting = true; //Si se mantiene shift se activa isSprinting.    
        animator.SetBool("isSprinting", true);
    }
    private void SprintOff(InputAction.CallbackContext context)
    {
        isSprinting = false; //    
        animator.SetBool("isSprinting", false);
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)//Si se presiona el boton Jump y esta tocando la base activamos interruptor en animator para salto y ejecutamos un impulso sobre el pj.
        {
            if (stats.GetAguanteActual() >= 2) stats.SetAguanteActual(stats.GetAguanteActual() - 2);
            isDoubleJump = false;
            animator.SetBool("isJumping", true);
            FindObjectOfType<AudioManager>().Play("Woosh", 1.55f, 0.1f);
            charRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (!isGrounded && animator.GetBool("isJumping") && !isDoubleJump) // Si se presiona el boton Jump y se esta en medio de un salto, se ejecuta un impulso y voltereta
        {
            if(stats.GetAguanteActual()>=4)stats.SetAguanteActual(stats.GetAguanteActual() - 4);
            animator.SetBool("isFliping", true);
            FindObjectOfType<AudioManager>().Play("Woosh", 1.35f, 0.1f);
            if (charRB.velocity.y > 0) { charRB.velocity = Vector2.zero; }
            charRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isDoubleJump = true;
        }
    }
    private bool CheckIsGrounded()
    {     
        RaycastHit2D raycastHit2D = Physics2D.CapsuleCast(charColl.bounds.center, charColl.bounds.size, CapsuleDirection2D.Vertical, 0f, Vector2.down * 25f, 5f, shadowLayerMask);
        return raycastHit2D.collider != null; //Determinamos por medio de un collider si tocamos la base (sombra)
    }
    private void CheckIsJumping()
    {
        ChangeCameraSizeJumping();
        //Si esta tocando la base o suspendido en el aire  definiremos los interruptores del animator en cuestión que desencadenaran la animacion.
        if (isGrounded)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFliping", false);
            spriteRenderer.sortingOrder = 4;
            isJumping = false;        
        }
        else if (!isGrounded && !isDead)
        {
            animator.SetBool("isJumping", true);
            spriteRenderer.sortingOrder = 12;
            speed = sprintSpeed;
            isJumping = true;         
        }
    }
    public bool IsJumping() { return isJumping; }
    private void CheckFloorTransition()
    {
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        if (inputVector != Vector2.zero && !isGrounded)
        {
            agent.enabled = false;
        }
        else if (!isDead)
        {
            agent.enabled = true;
        }      
    }
    private void ChangeCameraSizeJumping()
    {
        mainCam = Camera.main;
        float size = mainCam.orthographicSize;
        if (!cameraStoryMode)
        {
            if (charRB.velocity.y > 0 && mainCam.orthographicSize < 113)
            {
                size += charRB.velocity.y * 0.1f * Time.fixedDeltaTime; // aumenta el tamaño en función de la velocidad      
            }
            else if (charRB.velocity.y < 0 && mainCam.orthographicSize > 110)
            {
                size += charRB.velocity.y * 0.1f * Time.fixedDeltaTime; // reduce el tamaño, pero nunca por debajo del mínimo    
            }
        }     
        mainCam.orthographicSize = size;
    }

    //---- FUNCIONES DE COMBATE ---- //////////////////////////////////////////////////////////////////
    private void Punch(InputAction.CallbackContext context)
    {           
        if (!animator.GetBool("isHittingNoWalk") && stats.GetAguanteActual() >= 5 + (stats.GetTecPuño() / 2))
        {
            animator.SetBool("isHittingNoWalk", true); //Este interruptor sirve para decirle al animator que deje de transicionar a Walk y permita los triggers de Punch
            RaycastHit2D targetHit = Physics2D.CircleCast(attackCollider.bounds.center, attackCollider.radius, new Vector2(lastDir, 0), enemyLayer);
            stats.SetAguanteActual(stats.GetAguanteActual() - 2);     
            if(targetHit.transform.gameObject!=null)
            {
                if (targetHit.transform.GetComponent<EnemyController>() && targetHit.transform.GetComponent<EnemyController>().IsStunned())
                {
                    animator.SetTrigger("isUppercut");
                    FindObjectOfType<AudioManager>().Play("Woosh", 1.45f, 0.25f);
                    isAttacking = true;
                    isPunch = true;
                }
                else if (!targetHit.transform.GetComponent<EnemyController>() || !targetHit.transform.GetComponent<EnemyController>().IsStunned())
                {
                    animator.SetTrigger("isPunch");
                    FindObjectOfType<AudioManager>().Play("Woosh", 1.45f, 0.25f);
                    if (targetHit.transform.GetComponent<EnemyController>()) targetHit.transform.GetComponent<EnemyController>().GetStunned();
                    isAttacking = true;
                    isPunch = true;
                }
            }
            else
            {
                return;
            }  
        }    
    }
    private void Kick(InputAction.CallbackContext context)
    {   
        if (itemEquiped == null)
        {
            if (!animator.GetBool("isHittingNoWalk") && stats.GetAguanteActual()>=5 + (stats.GetTecPatada() / 2))
            {
                animator.SetBool("isHittingNoWalk", true); //Este interruptor sirve para decirle al animator que deje de transicionar a Walk y permita los triggers de Punch
                RaycastHit2D targetHit = Physics2D.CircleCast(attackCollider.bounds.center, attackCollider.radius, new Vector2(lastDir, 0), enemyLayer);           
                stats.SetAguanteActual(stats.GetAguanteActual() - 4);
                if (targetHit.transform.gameObject != null)
                {
                    if (targetHit.transform.GetComponent<EnemyController>() && targetHit.transform.GetComponent<EnemyController>().IsStunned())
                    {
                        animator.SetTrigger("isFrontKick");
                        FindObjectOfType<AudioManager>().Play("Woosh", 0.85f, 0.25f);
                        isAttacking = true;
                        isKick = true;
                    }
                    else if (!targetHit.transform.GetComponent<EnemyController>() || !targetHit.transform.GetComponent<EnemyController>().IsStunned())
                    {
                        animator.SetTrigger("isKick");
                        FindObjectOfType<AudioManager>().Play("Woosh", 0.85f, 0.25f);
                        isAttacking = true;
                        isKick = true;
                    }
                }
                else 
                { 
                    return; 
                }                  
            }
        }
        else
        {
            isThrowing = true;
        }       
    }
    private void OnDrawGizmosSelected()
    {
        // Dibuja un círculo de color rojo en el área del ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCollider.bounds.center, attackCollider.radius);
    }
    private void CheckEndAttack()
    {
        //Si esta finalizando la ejecución de un puñetazo, entonces desactivamos el bloqueo de Walk
        if (animator.GetBool("isHittingNoWalk") && 
            (animator.GetCurrentAnimatorStateInfo(0).IsName("PunchLight") || 
            animator.GetCurrentAnimatorStateInfo(0).IsName("PunchStrong") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("KickJump") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("KickFront") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Uppercut") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("KickLight")) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >0.99f)
        {           
            if (isAttacking)
            {
                AttackCheck();     
            }
            isAttacking = false;
            durabilidadReducida = false;
            if (isPunch) { isPunch = false; }
            else if (isKick) { isKick = false; }
            animator.SetBool("isHittingNoWalk", false);
        }          
    }
    private void AttackCheck()
    {      
        RaycastHit2D[] targetHit = Physics2D.CircleCastAll(attackCollider.bounds.center, attackCollider.radius, new Vector2(0,0),enemyLayer);
        int contadorTargets = 0;
        foreach (RaycastHit2D hit in targetHit)
        {
            if (hit.transform.gameObject.tag == "Enemy")
            {
                FindObjectOfType<AudioManager>().Play("Impact", 1, 1);
                if ((animator.GetBool("isSprinting") || hit.transform.GetComponent<EnemyController>().IsStunned()) 
                    && !hit.transform.GetComponent<EnemyController>().IsBlocking())
                {                   
                    hit.transform.GetComponent<EnemyController>().GetHitDash(lastDir);                   
                    if (isPunch)
                    {
                        if (!durabilidadReducida && itemEquiped!=null)
                        {
                            itemEquiped.GetComponent<Item_Arma>().ReducirDurabilidad();

                            hit.transform.GetComponent<EnemyController>().HitNumber(
                            hit.transform.GetComponent<RPG_Stats>().GetSaludActual(), (stats.GetFuerza() + itemEquiped.GetComponent<Item_Arma>().GetDaño()));

                            hit.transform.GetComponent<RPG_Stats>().SetSaludActual
                            (hit.transform.GetComponent<RPG_Stats>().GetSaludActual() - (stats.GetFuerza() + itemEquiped.GetComponent<Item_Arma>().GetDaño()));

                            durabilidadReducida = true;
                        }
                        else if (itemEquiped==null)
                        {
                            hit.transform.GetComponent<EnemyController>().HitNumber(
                            hit.transform.GetComponent<RPG_Stats>().GetSaludActual(), (stats.GetFuerza() + stats.GetTecPuño()));

                            hit.transform.GetComponent<RPG_Stats>().SetSaludActual
                            (hit.transform.GetComponent<RPG_Stats>().GetSaludActual() - (stats.GetFuerza() + stats.GetTecPuño()));
                        }                  
                    }
                    else if(isKick)
                    {
                        hit.transform.GetComponent<EnemyController>().HitNumber(
                        hit.transform.GetComponent<RPG_Stats>().GetSaludActual(), (stats.GetFuerza() + stats.GetTecPatada()));

                        hit.transform.GetComponent<RPG_Stats>().SetSaludActual
                        (hit.transform.GetComponent<RPG_Stats>().GetSaludActual() - (stats.GetFuerza() + stats.GetTecPatada()));
                    }                                            
                }      
                else if (!animator.GetBool("isSprinting") && !hit.transform.GetComponent<EnemyController>().IsBlocking() 
                    && !hit.transform.GetComponent<EnemyController>().IsStunned())
                {                                                                                                             
                    hit.transform.GetComponent<EnemyController>().GetHit();                   
                    if(isPunch)
                    {
                        if (!durabilidadReducida && itemEquiped != null)
                        {
                            itemEquiped.GetComponent<Item_Arma>().ReducirDurabilidad();

                            hit.transform.GetComponent<EnemyController>().HitNumber(
                            hit.transform.GetComponent<RPG_Stats>().GetSaludActual(), (stats.GetFuerza() + itemEquiped.GetComponent<Item_Arma>().GetDaño()));

                            hit.transform.GetComponent<RPG_Stats>().SetSaludActual
                            (hit.transform.GetComponent<RPG_Stats>().GetSaludActual() - (stats.GetFuerza() + itemEquiped.GetComponent<Item_Arma>().GetDaño()));

                            durabilidadReducida = true;
                            if (Random.Range(1, 4) == 2) { hit.transform.GetComponent<EnemyController>().GetStunned(); }
                        }
                        else if (itemEquiped==null)
                        {
                            hit.transform.GetComponent<EnemyController>().HitNumber(
                            hit.transform.GetComponent<RPG_Stats>().GetSaludActual(), (stats.GetFuerza() + stats.GetTecPuño()));

                            hit.transform.GetComponent<RPG_Stats>().SetSaludActual
                            (hit.transform.GetComponent<RPG_Stats>().GetSaludActual() - (stats.GetFuerza() + stats.GetTecPuño()));
                        }
                        if (Random.Range(1, 4) == 2) { hit.transform.GetComponent<EnemyController>().GetStunned(); }
                    }
                    else if(isKick)
                    {
                        hit.transform.GetComponent<EnemyController>().HitNumber(
                        hit.transform.GetComponent<RPG_Stats>().GetSaludActual(), (stats.GetFuerza() + stats.GetTecPatada()));

                        hit.transform.GetComponent<RPG_Stats>().SetSaludActual
                        (hit.transform.GetComponent<RPG_Stats>().GetSaludActual() - (stats.GetFuerza() + stats.GetTecPatada()));

                        if (Random.Range(1, 4) == 2) { hit.transform.GetComponent<EnemyController>().GetStunned(); }
                    }                 
                }              
            }
            else { contadorTargets++; }
        }
        isAttacking = false;
        isPunch = false;
        isKick = false;
    }
    private void Block (InputAction.CallbackContext context)
    {
        if (itemEquiped == null &&  isItemTargeted )
        {
            itemToEquip = true;
        }
        else if (!isItemTargeted)
        {
            if (dialogoText.text.Length - dialogoText.GetComponent<TypeWriterEffect>().fullText.Length == -1) //Máximo texto posible 127 chars.
            {
                ResetDialogo();
            }
            else if(doorContact)
            {
                doorInteraction = true;
            }
            else 
            {
                if(stats.GetAguanteActual() >= 2 + (stats.GetTecBloqueo() / 2))
                {
                    animator.SetTrigger("isBlocking");
                } 
            }   
        }   
    }
    private void CheckBlocking()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Blocking"))
        {
            isBlocking = true;                     
        }
       else
        {
            isBlocking = false;
        }
    }
    public bool IsBlocking() { return isBlocking; }
    public void GetHit()
    {
        if (!isBlocking)
        {
            animator.SetBool("isHittingNoWalk", true);
            hitBlood.Play();
            if (!secondAnimHit)
            {
                animator.SetTrigger("isHit");
                FindObjectOfType<AudioManager>().Play("ManDead", 1, 1);
                secondAnimHit = true;
            }
            else
            {
                animator.SetTrigger("isHit2");
                FindObjectOfType<AudioManager>().Play("ManHit", 1, 1);
                secondAnimHit = false;
            }
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("Impact", 0.5f, 0.35f);
        }       
    }
    public void GetHitDash(float direction)
    {
        animator.SetBool("isHittingNoWalk", true);
        agent.isStopped = true;
        hitBlood.Play();
        dashTrail.SetActive(true);
        FindObjectOfType<AudioManager>().Play("ManDead", 0.85f, 1.25f);
        animator.SetTrigger("isHitDash");
        transform.Translate(new Vector3(direction * 50f, 0f, 0f), Space.World);       
    }
    public void GetStunned()
    {      
        hitBlood.Play();
        if (!animator.GetBool("isStunned")) Expresar("Stun");
        animator.SetBool("isStunned",true);   
    }
    private void CheckStunned()
    {       
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stunned") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
        {
            animator.SetBool("isStunned", false);        
        }
    }
    public bool IsStunned() { return animator.GetBool("isStunned"); }
    private void CheckIfAttackingOrGettingHit()
    {
        if (animator.GetBool("isHittingNoWalk") &&
            (animator.GetCurrentAnimatorStateInfo(0).IsName("PunchLight") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("PunchStrong") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Uppercut") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("KickJump") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("KickLight") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("KickFront") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("GetHitDash") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit2")) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
        {
            animator.SetBool("isHittingNoWalk", false);
        }
    }
    private void CheckForItem()
    {
        RaycastHit2D[] targetHit = Physics2D.CircleCastAll(attackCollider.bounds.center, attackCollider.radius*4, new Vector2(0, 0), itemLayer);
        int contadorTargets = 0;
        foreach (RaycastHit2D hit in targetHit)
        {
            if (hit.transform.gameObject.tag == "Item")
            {
                isItemTargeted = true;
                if (itemToEquip)
                {
                    switch(hit.transform.gameObject.name)
                    {
                        case "Cuchillo": hit.transform.GetComponent<SpriteRenderer>().sprite= cuchillo_combatSprite; break;
                        case "Botella": hit.transform.GetComponent<SpriteRenderer>().sprite = botella_combatSprite; break;
                        case "Porra": hit.transform.GetComponent<SpriteRenderer>().sprite = porra_combatSprite; break;
                        case "Palo": hit.transform.GetComponent<SpriteRenderer>().sprite = palo_combatSprite; break;
                        case "Tuberia": hit.transform.GetComponent<SpriteRenderer>().sprite = tuberia_combatSprite; break;
                    }
                    itemEquiped = hit.transform.gameObject;
                    itemToEquip = false;
                }
            }
            else { contadorTargets++; isItemTargeted = false;}
        }
        if (itemEquiped != null)
        {
            itemEquiped.transform.SetParent(attackCollider.transform);
            itemEquiped.GetComponent<SpriteRenderer>().sortingOrder = 10;
            itemEquiped.transform.rotation = Quaternion.Euler(0, 0, 0);
            if (attackCollider == attackCollDer)
            {
                switch(itemEquiped.transform.gameObject.name)
                {
                    case "Cuchillo": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-15, 10, 0); break;
                    case "Botella": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-15, 10, 0); break;
                    case "Porra": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-15, 11, 0); break;
                    case "Palo": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-15, 11, 0); break;
                    case "Tuberia": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-12, 11, 0); break;
                    case "Caja": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-15, 0, 0); break;
                    case "Roca": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-15, 0, 0); break;
                }         
                itemEquiped.GetComponent<SpriteRenderer>().flipX = false;
                if (isAttacking && animator.GetCurrentAnimatorStateInfo(0).normalizedTime>0.65f)
                {
                    itemEquiped.transform.rotation = Quaternion.Euler(0, 0, -90);

                    switch(itemEquiped.transform.gameObject.name)
                    {
                        case "Cuchillo": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(7, 2, 0); break;
                        case "Botella": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(7, 2, 0); break;
                        case "Porra": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(7, 2, 0); break;
                        case "Palo": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(8, 2, 0); break;
                        case "Tuberia": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(5, 2, 0); break;
                        case "Caja": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(3, 2, 0); break;
                        case "Roca": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(1, 2, 0); break;
                    }                                                                       
                    //itemEquiped.GetComponent<SpriteRenderer>().sortingOrder = 2;
                }
                if (isThrowing && itemEquiped!=null)
                {
                    animator.SetTrigger("isThrowing");
                    itemEquiped.transform.rotation = Quaternion.Euler(0, 0, -90);
                    itemEquiped.transform.parent.DetachChildren();

                    switch (itemEquiped.transform.gameObject.name)
                    {
                        case "Cuchillo": itemEquiped.GetComponent<SpriteRenderer>().sprite = cuchillo_WorldSprite; break;
                        case "Botella": itemEquiped.transform.GetComponent<SpriteRenderer>().sprite = botella_WorldSprite; break;
                        case "Porra": itemEquiped.transform.GetComponent<SpriteRenderer>().sprite = porra_WorldSprite; break;
                        case "Palo": itemEquiped.transform.GetComponent<SpriteRenderer>().sprite = palo_WorldSprite; break;
                        case "Tuberia": itemEquiped.transform.GetComponent<SpriteRenderer>().sprite = tuberia_WorldSprite; break;

                    }
                    itemEquiped.GetComponent<Item_Arma>().IsThrown();
                    Rigidbody2D rb = itemEquiped.GetComponent<Rigidbody2D>();
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.gravityScale = 0;
                    rb.mass = 0.07f;
                    rb.AddForce(new Vector3(7, 0, 0), ForceMode2D.Impulse);                    
                    rb.freezeRotation = true;                                       
                    itemEquiped = null;                   
                }
            }
            else
            {
                switch (itemEquiped.transform.gameObject.name)
                {
                    case "Cuchillo": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(15, 10, 0); break;
                    case "Botella": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(15, 10, 0); break;
                    case "Porra": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(15, 11, 0); break;
                    case "Palo": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(15, 11, 0); break;
                    case "Tuberia": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(12, 11, 0); break;
                    case "Caja": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(15, 0, 0); break;
                    case "Roca": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(15, 0, 0); break;
                }         
                itemEquiped.GetComponent<SpriteRenderer>().flipX = true;
                if (isAttacking && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.65f)
                {
                    itemEquiped.transform.rotation = Quaternion.Euler(0, 0, 90);
                    switch (itemEquiped.transform.gameObject.name)
                    {
                        case "Cuchillo": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-7, 2, 0); break;
                        case "Botella": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-7, 2, 0); break;
                        case "Porra": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-7, 2, 0); break;
                        case "Palo": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-8, 2, 0); break;
                        case "Tuberia": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-5, 2, 0); break;
                        case "Caja": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-3, 2, 0); break;
                        case "Roca": itemEquiped.transform.position = attackCollider.transform.position + new Vector3(-1, 2, 0); break;
                    }                   
                    //itemEquiped.GetComponent<SpriteRenderer>().sortingOrder = 2;
                }
                if (isThrowing)
                {
                    animator.SetTrigger("isThrowing");
                    itemEquiped.transform.rotation = Quaternion.Euler(0, 0, 90);
                    itemEquiped.transform.parent.DetachChildren();
                    switch (itemEquiped.transform.gameObject.name)
                    {
                        case "Cuchillo": itemEquiped.GetComponent<SpriteRenderer>().sprite = cuchillo_WorldSprite; break;
                        case "Botella": itemEquiped.transform.GetComponent<SpriteRenderer>().sprite = botella_WorldSprite; break;
                        case "Porra": itemEquiped.transform.GetComponent<SpriteRenderer>().sprite = porra_WorldSprite; break;
                        case "Palo": itemEquiped.transform.GetComponent<SpriteRenderer>().sprite = palo_WorldSprite; break;
                        case "Tuberia": itemEquiped.transform.GetComponent<SpriteRenderer>().sprite = tuberia_WorldSprite; break;
                    }
                    itemEquiped.GetComponent<Item_Arma>().IsThrown();
                    Rigidbody2D rb = itemEquiped.GetComponent<Rigidbody2D>();
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.gravityScale = 0;
                    rb.mass = 0.07f;
                    rb.AddForce(new Vector3(-7, 0, 0), ForceMode2D.Impulse);                                     
                    rb.freezeRotation=true;                   
                    itemEquiped = null;
                    isItemTargeted = false;
                }
            }           
        }
        else { isThrowing = false; }
    }

    //---- FUNCIONES DE INTERFAZ Y PARÁMETROS ---- //////////////////////////////////////////////////////////////////
    private void HUD()
    {
        saludBarra.fillAmount = ((stats.GetSaludActual() * 100) / stats.GetSaludTotal()) / 100;
        saludActualText.GetComponent<Text>().text = stats.GetSaludActual().ToString();
        saludTotalText.GetComponent<Text>().text = stats.GetSaludTotal().ToString();
        armaduraBarra.fillAmount = ((stats.GetArmaduraActual() * 100) / stats.GetArmaduraTotal()) / 100;
        aguanteBarra.fillAmount = ((stats.GetAguanteActual() * 100) / stats.GetAguanteTotal()) / 100;
        aguanteActualText.GetComponent<Text>().text = stats.GetAguanteActual().ToString();
        aguanteTotalText.GetComponent<Text>().text = stats.GetAguanteTotal().ToString();
        dineroText.GetComponent<Text>().text = stats.GetDinero().ToString();

        if (dialogoText.text.Length - dialogoText.GetComponent<TypeWriterEffect>().fullText.Length == -1 && !gamePaused) //Máximo texto posible 127 chars.
        {
            accionPosible.text = "Pasar diálogo";
        }
        else if(doorContact)
        {
            accionPosible.text = "Abrir puerta";
        }
        else if(gamePaused)
        {
            accionPosible.text = "Selec.  [C] : Cancel.";
        }
        else
        {
            accionPosible.text = "Bloquear";
        }

        saludDato.text = stats.GetSaludTotal().ToString();
        aguanteDato.text = stats.GetAguanteTotal().ToString();
        armaduraDato.text = stats.GetArmaduraTotal().ToString();
        absorcionArmaduraDato.text = stats.GetAbsorcionArmadura().ToString();
        fuerzaDato.text = stats.GetFuerza().ToString();
        agilidadDato.text = stats.GetAgilidad().ToString();
        tecPuñoDato.text = stats.GetTecPuño().ToString();
        tecPatadaDato.text = stats.GetTecPatada().ToString();
        tecBloqueoDato.text = stats.GetTecBloqueo().ToString();

       
    }
    public void HitNumber(float valorTotal, float valorRestado)
    {
        float valorRestante = valorTotal - valorRestado;
        float valorSustraido = valorTotal - valorRestante;
        GameObject hitNumber = Instantiate(hitNumberPrefab, hitNumberPosition.transform);
        hitNumber.GetComponent<Canvas>().worldCamera = Camera.main;
        hitNumber.transform.Find("DamageNumber").GetComponent<Text>().text = valorSustraido.ToString();       
    }
    private void CalcularStatsPlayer()
    {
        //LIMITES
        if (stats.GetAguanteActual() > stats.GetAguanteTotal()) { stats.SetAguanteActual(stats.GetAguanteTotal()); }
        if (stats.GetAguanteActual() < 0) { stats.SetAguanteActual(0); }
        
        if (stats.GetArmaduraActual() < 0) { stats.SetArmaduraActual(0); }
        if(stats.GetArmaduraActual()>0 && stats.GetArmaduraActual()< stats.GetAbsorcionArmadura()) { stats.SetArmaduraActual(0); }
        if (stats.GetSaludActual() > stats.GetSaludTotal()) { stats.SetSaludActual(stats.GetSaludTotal()); }
        if (stats.GetSaludActual() <= 0) { stats.SetSaludActual(0); Death(); }
        // CALCULOS DE AGUANTE-------------------------------------------------------
        timeToUpdateStats -= Time.deltaTime;
        if (timeToUpdateStats <= 0)
        {
            if (!isSprinting)
            {
                stats.SetAguanteActual(stats.GetAguanteActual() + 2);
                //stats.SetSaludActual(stats.GetSaludActual() + 1);
            }
            else { stats.SetAguanteActual(stats.GetAguanteActual() - 8); }

            timeToUpdateStats = timerUpdateStats;
        }
        //----------------------------------------------------------------------------
        // CALCULOS DE AGILIDAD ----------------------------------------------
        switch(stats.GetAgilidad())
        {
            case 1: normalSpeed = 1f; sprintSpeed = 1.5f; jumpForce = 120f; animator.speed = 1; break;
            case 2: normalSpeed = 2f; sprintSpeed = 4f; jumpForce = 200f; animator.speed = 2f; break;
        }
    }
    public void DialogoHUD(string actor, string text, float delay)
    {
        dialogoText.GetComponent<TypeWriterEffect>().delay = delay;
        dialogoText.GetComponent<TypeWriterEffect>().fullText = text;    
        dialogoText.GetComponent<TypeWriterEffect>().ActivaTypeWriterEffect();
        dialogoActor.text = "[ "+actor+" ]";
    }
    public void AccionPosibleHUD(string accion)
    {
        accionPosible.text = accion;
    }
    public void ResetDialogo()
    {
        dialogoText.text = "";
        dialogoActor.text = "";
        
    }
    private void PlayerMenu(InputAction.CallbackContext context)
    {
        if(!estadoMenu && !storyMode)
        {
            if (!gamePaused)
            {
                
                menuApartados.SetActive(true);
                dialogoPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Confined;
                menuApartados.transform.Find("Estado").GetComponent<Button>().Select();
                Cursor.lockState = CursorLockMode.Locked;
                gamePaused = true;

            }
            else { gamePaused = false; }
        }
       
    }
    private void SeleccionApartado(InputAction.CallbackContext context)
    {
        if(gamePaused && !estadoMenu)
        {
            Button[] buttons = menuApartados.transform.GetComponentsInChildren<Button>();
            foreach(Button button in buttons)
            {           
                if(EventSystem.current.currentSelectedGameObject == button.gameObject)
                {
                    var ped = new PointerEventData(EventSystem.current); //Codigo para seleccionar el botton como si se hiciera con el puntero y así usar sus estados presionado y seleccionado
                    ExecuteEvents.Execute(button.gameObject, ped, ExecuteEvents.pointerEnterHandler);
                    ExecuteEvents.Execute(button.gameObject, ped, ExecuteEvents.submitHandler);
                    if(button.name=="Estado")
                    {
                        menuPanelMain.SetActive(true);
                        menuEstado.SetActive(true);
                        menuApartados.SetActive(false);
                        dialogoPanel.SetActive(true);
                        dialogoPanel.GetComponent<Outline>().enabled = true;
                        MostrarListaItems();            
                    }
                }         
            }
        }
        if (estadoMenu && Cursor.lockState==CursorLockMode.Locked)
        {
            Button[] buttons = itemPanel.transform.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                if (EventSystem.current.currentSelectedGameObject == button.gameObject && ultimoItemSelected== button.gameObject)
                {
                    var ped = new PointerEventData(EventSystem.current); //Codigo para seleccionar el botton como si se hiciera con el puntero y así usar sus estados presionado y seleccionado
                    ExecuteEvents.Execute(button.gameObject, ped, ExecuteEvents.pointerEnterHandler);
                    ExecuteEvents.Execute(button.gameObject, ped, ExecuteEvents.submitHandler);
                    Debug.Log(button.gameObject.name+" seleccionado.");
                    UsarItem(EventSystem.current.currentSelectedGameObject);
                }
                else if (ultimoItemSelected != EventSystem.current.currentSelectedGameObject)
                {                           
                    ultimoItemSelected = EventSystem.current.currentSelectedGameObject.gameObject;                               
                }
            }
        }
    }
    private void MostrarListaItems()
    {
        if (itemPanel.transform.childCount > 0)
        {
            for (int i = 0; i < itemPanel.transform.childCount; i++)
            {
                Destroy(itemPanel.transform.GetChild(i).gameObject);
            }
        }
       if(listaItems.Count>0)
       {
            foreach (GameObject item in listaItems)
            {
                var newItem = Instantiate(itemInventario, itemPanel.transform);
                newItem.name = item.GetComponent<Item>().GetNombre();
                newItem.GetComponent<Item>().SetNombre(item.GetComponent<Item>().GetNombre());
                newItem.transform.Find("Icono").GetComponent<Image>().sprite = item.GetComponent<Item>().GetIcono();
                newItem.transform.Find("Cantidad").GetComponent<Text>().text = item.GetComponent<Item>().GetCantidad().ToString();
                newItem.GetComponent<Item>().SetDescripcion(item.GetComponent<Item>().GetDescripcion());           
            }
            EventSystem.current.SetSelectedGameObject(itemPanel.transform.GetChild(0).gameObject);    
        }     
        estadoMenu = true;
    }
    private void UpdateDatosItemSelected()
    {             
        if (estadoMenu && itemPanel.transform.childCount>0 )
        {
            if(EventSystem.current.currentSelectedGameObject!=null)
            {
                if (!descripcionMostrada && ultimoItemSelected == EventSystem.current.currentSelectedGameObject)
                {
                    DialogoHUD(EventSystem.current.currentSelectedGameObject.GetComponent<Item>().GetNombre(),
                    EventSystem.current.currentSelectedGameObject.GetComponent<Item>().GetDescripcion(), 0.03f);
                    descripcionMostrada = true;
                }
                else if (descripcionMostrada && ultimoItemSelected != EventSystem.current.currentSelectedGameObject)
                {
                    descripcionMostrada = false;
                    Debug.Log("Current object " + EventSystem.current.currentSelectedGameObject.name);
                    ultimoItemSelected = EventSystem.current.currentSelectedGameObject.gameObject;
                }
                else if (seleccionPrimerItem)
                {
                    EventSystem.current.SetSelectedGameObject(itemPanel.transform.GetChild(0).gameObject);
                    seleccionPrimerItem = false;
                }
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(itemPanel.transform.GetChild(0).gameObject);
            }     
        }
        else
        {
            ultimoItemSelected = null;
            seleccionPrimerItem = true;
        }   
    }
    private void CancelarSeleccion(InputAction.CallbackContext context)
    {
        if(estadoMenu)
        {
            ResetDialogo();
            dialogoPanel.SetActive(false);
            dialogoPanel.GetComponent<Outline>().enabled = false;
            menuPanelMain.SetActive(false);
            menuEstado.SetActive(false);
            menuApartados.SetActive(true);
            menuApartados.transform.Find("Estado").GetComponent<Button>().Select();  
            estadoMenu = false;        
        }
    }
    private void UsarItem(GameObject go)
    {
        foreach (GameObject item in listaItems)
        {
            if (item.GetComponent<Item>().GetNombre() == go.GetComponent<Item>().GetNombre())
            {
                if(item.GetComponent<Item>().GetCantidad()==1)
                {
                    listaItems.Remove(item);
                }
                else
                {
                    item.GetComponent<Item>().SetCantidad(item.GetComponent<Item>().GetCantidad() - 1);
                }
                ActivarEfectoItem(item);
                MostrarListaItems();             
                EventSystem.current.SetSelectedGameObject(go);
            }
        }
    }

    //---- FUNCIONES DE PROCESOS DEL JUEGO ---- //////////////////////////////////////////////////////////////////
    private void Death()
    {
        if (!isDead)
        {
            animator.SetBool("isDead", true);
            GetComponent<SpriteRenderer>().enabled = false;
            charColl.enabled = false;
            charRB.simulated = false;
            GetComponent<NavMeshAgent>().enabled = false;
            Invoke("MoveDown", 0.3f);
            isDead = true;
        }
        else
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dead") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
            {
                animator.SetBool("isDeadFinal", true);
            }
        }
    }
    private void MoveDown() { transform.position += new Vector3(0, -10, 0); }
    private void GameTimeProcess()
    {
        if (gamePaused)
        {
            pausaTitulo.GetComponent<Text>().enabled = true;        
            animator.enabled = false;
            agent.isStopped = true;
            charRB.gravityScale = 0;
            charRB.velocity = Vector3.zero;
        }
        else if(!gamePaused)
        {
            pausaTitulo.GetComponent<Text>().enabled = false;
            menuApartados.SetActive(false);
            dialogoPanel.SetActive(true);
            animator.enabled = true;
            agent.isStopped = false;
            charRB.gravityScale = 25;
        } 
    }

    //---- FUNCIONES DE EFECTOS DE ITEMS ---- //////////////////////////////////////////////////////////////////
    private float duracionPotenciadorFuerza = 10f, duracionPotenciadorAgilidad = 10f; 
    private float tiempoRestantePotenciadorFuerza = 0f, tiempoRestantePotenciadorAgilidad = 0f; 
    private void CheckDuracionEfectosItem()
    {
        if (tiempoRestantePotenciadorFuerza > 0)
        {
            tiempoRestantePotenciadorFuerza -= Time.deltaTime; 
            if (tiempoRestantePotenciadorFuerza <= 0)
            {
                stats.SetFuerza(stats.GetFuerza() - 3); 
            }
        }
        if (tiempoRestantePotenciadorAgilidad > 0)
        {
            tiempoRestantePotenciadorAgilidad -= Time.deltaTime;
            if (tiempoRestantePotenciadorAgilidad <= 0)
            {
                stats.SetAgilidad(stats.GetAgilidad() - 1);
            }
        }
    }
    private void ActivarEfectoItem(GameObject itemUsado)
    {
        switch (itemUsado.GetComponent<Item>().GetNombre())
        {
            case "Kit Médico": 
                stats.SetSaludActual(stats.GetSaludActual() + 20); break;
            case "Lata Grande 01": break;
            case "Lata Pequeña 01": break;
            case "Lata Pequeña 02": break;
            case "Lata Pequeña 03": break;
            case "Potenciador de Fuerza":
                stats.SetFuerza(stats.GetFuerza() + 3); 
                tiempoRestantePotenciadorFuerza = duracionPotenciadorFuerza; break; 
            case "Potenciador de Agilidad":
                stats.SetAgilidad(stats.GetAgilidad() + 1); 
                tiempoRestantePotenciadorAgilidad = duracionPotenciadorAgilidad; break;
            case "Potenciador de Salud": break;
            case "Potenciador de Aguante": break;
        }
    }

    //---- FUNCIONES DE EXPRESIONES ---- //////////////////////////////////////////////////////////////////
    public void Expresar(string expresion)
    {
        switch(expresion)
        {
            case "Atencion": Instantiate(atencion,expresionPosition.transform); break;
            case "Exclamacion": Instantiate(exclamacion, expresionPosition.transform); break;
            case "Interrogacion": Instantiate(interrogacion, expresionPosition.transform); break;
            case "Contento": Instantiate(contento, expresionPosition.transform); break;
            case "Enfadado": Instantiate(enfadado, expresionPosition.transform); break;
            case "Cantar": Instantiate(cantar, expresionPosition.transform); break;
            case "Stun": Instantiate(stun, expresionPosition.transform); break;

            case "Hablar": Instantiate(hablar, expresionPosition.transform);
                           Instantiate(boca, bocaPosition.transform);break;
        }
    }
    public void AcabarExpresionLoop()
    {
        if (expresionPosition.transform.childCount>0) { Destroy(expresionPosition.transform.GetChild(0).gameObject); }
        if (bocaPosition.transform.childCount > 0) { Destroy(bocaPosition.transform.GetChild(0).gameObject); }      
    }

    //---- INTEGRACIÓN DE TODAS LAS FUNCIONES ---- //////////////////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        if (movement && !gamePaused)
        {
            CheckDuracionEfectosItem();
            CalcularStatsPlayer();
            playerInputActions.Player.Enable(); //Activamos el mapa relativo a Player, los controles del jugador.      
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("KickJump"))
            {
                //Por si quiero modificar el area de ataque durante la patada de salto para hacerla más precisa
            }
        }
        else if (!movement || gamePaused)
        {
            playerInputActions.Player.Disable();
            UpdateDatosItemSelected();
        }
        if (!isDead)
        {
            CheckMovement();
            CheckForItem();
            GameTimeProcess();
            CheckIsJumping(); //Comprobamos si esta suspendido y definimos animaciones.
            CheckEndAttack(); // Comprobamos si ha finalizado la animacion de ataque y desactivamos el interruptor de bloqueo de Walk. 
            CheckBlocking();
            CheckStunned();
            CheckIfAttackingOrGettingHit();//Comprobar que no este atacando ni recibiendo daño y si no lo estoy, entonces desactivo interruptor de bloqueo de walk por si ha quedado encendido.     
            CheckFloorTransition();
            playerInputActions.PlayerMenu.Enable();
        }
        isGrounded = CheckIsGrounded(); //Guardamos en isGrounded el retorno de la funcion IsGrounded.                
    }
    private void LateUpdate()
    {
        HUD();
    }
}
