using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("ENEMY CONTROLLER COMPONENTS")]
    [SerializeField]
    private ParticleSystem dashTrail;
    [SerializeField]
    private ParticleSystem hitBlood;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private CapsuleCollider2D charColl;
    [SerializeField]
    private LayerMask playerLayerMask;
    public bool movement = false, playerInSight = false;
    [Header("HUD")]
    [SerializeField]
    private GameObject hitNumberPrefab;
    [SerializeField]
    private GameObject hitNumberPosition;
    [SerializeField]
    private Image saludBarra;
    [SerializeField]
    private GameObject saludObj;

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

    private RPG_Stats stats;
    private float timeShowingHUD, timerShowHUD = 1f;
    private float timeToDestroyOnDeath = 3f, timeToAttack=0.25f, timerAttack=0.25f;
    private GameObject player;
    private bool secondAnim, isBlocking, isStunned, isDead, isDeadFinal, isPunch, isKick;
    private float lastDir;
    private int random = 0;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.parent.gameObject;
        stats = GetComponent<RPG_Stats>();
        stats.SetSaludRef(stats.GetSaludActual());
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.SetDestination(player.transform.position);      
    }
    public void SetMovement(bool m) { movement = m; }
    public void SetPlayerInSight(bool p) { playerInSight = p; }
    private void EnemyBehavior()
    {      
        float lastDirection = 0;
        if (agent.remainingDistance <= agent.stoppingDistance || !movement || !playerInSight ) // Cuando estoy al lado del objetivo, freno agent, paro animacion walk y establezco nuevo destino
        {
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
            if (movement && playerInSight)
            {
               
                EnemyBehavior_Combat();
                agent.SetDestination(player.transform.position);
            }
        }
        else if (agent.remainingDistance > agent.stoppingDistance && movement && playerInSight) // Cuando estoy lejos del objetivo, y no esta recibiendo un golpe entonces animacion walk
        {          
            if (!animator.GetBool("isBlocking") )
            {               
                agent.isStopped = false;
               
                if (animator.GetBool("isHittingNoWalk"))
                {
                    animator.SetBool("isWalking", false);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("GetHitDash") ||
                        animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit") ||
                        animator.GetCurrentAnimatorStateInfo(0).IsName("Stunned") ||
                        animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit2") &&
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
                    {
                        animator.SetBool("isHittingNoWalk", false);
                        animator.SetBool("isWalking", true);
                        animator.SetBool("isStunned", false);
                        agent.SetDestination(player.transform.position);                   
                    }                   
                }              
                else
                {                  
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isHittingNoWalk", false);
                    agent.SetDestination(player.transform.position);           
                }
                if (agent.velocity.x > 0) //Recibo input horizontal del agente y determino dirección
                {
                    lastDirection = 1;
                }
                else if (agent.velocity.x < 0)
                {
                    lastDirection = -1;
                }
            }                  
        }
        lastDir = lastDirection; // Aplico los datos recibidos e invierto el sprite según la direccion del objetivo
    }
    private void EnemyBehavior_Combat()
    {       
        random = Random.Range(1, 4);
        timeToAttack -= Time.deltaTime;
        if (!animator.GetBool("isHittingNoWalk") && !animator.GetBool("isStunned"))
        {                       
            animator.SetBool("isHittingNoWalk", true);          
            if (timeToAttack <= 0)
            {               
                if (random == 1)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
                    {
                        animator.SetTrigger("isPunch");
                        FindObjectOfType<AudioManager>().Play("Woosh", 1.45f, 0.15f);
                        isPunch = true;
                    }
                }
                if (random == 2)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
                    {
                        animator.SetTrigger("isKick");
                        FindObjectOfType<AudioManager>().Play("Woosh", 0.85f, 0.15f);
                        isKick = true;
                    }
                }
                if (random == 3)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
                    {
                        animator.SetTrigger("isBlocking");
                        isBlocking = true;
                        FindObjectOfType<AudioManager>().Play("Woosh", 0.85f, 0.15f);
                    }
                }
                timeToAttack = timerAttack;             
            }                       
        }
        else if (animator.GetBool("isHittingNoWalk") && !animator.GetBool("isStunned"))
        {         
            if ((animator.GetCurrentAnimatorStateInfo(0).IsName("PunchLight") ||                    
                        animator.GetCurrentAnimatorStateInfo(0).IsName("KickLight")) &&
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
            {              
                AttackCheck();
               
                agent.SetDestination(player.transform.position);
                animator.SetBool("isWalking", true);
                animator.SetBool("isHittingNoWalk", false);
            }                    
        }            
    }
    
    private void AttackCheck()
    {
        RaycastHit2D targetHit = Physics2D.CapsuleCast(charColl.bounds.center, charColl.bounds.size,
                                                       CapsuleDirection2D.Horizontal, 0f, Vector2.right, 10f, playerLayerMask);     
            if (targetHit.transform.gameObject.tag == "Player")
            {         
                if (!targetHit.transform.parent.GetComponent<PlayerController>().IsBlocking())
                {
                    FindObjectOfType<AudioManager>().Play("Impact", 1, 1);  
                    targetHit.transform.parent.GetComponent<PlayerController>().GetHit();
                    if(isPunch)
                    {
                        if(targetHit.transform.parent.GetComponent<RPG_Stats>().GetArmaduraActual()>=
                        targetHit.transform.parent.GetComponent<RPG_Stats>().GetAbsorcionArmadura())
                        {
                            if(((stats.GetFuerza() + stats.GetTecPuño()) -
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetAbsorcionArmadura())>0)
                            {
                                targetHit.transform.parent.GetComponent<PlayerController>().HitNumber(
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetSaludActual(), ((stats.GetFuerza() + stats.GetTecPuño()) -
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetAbsorcionArmadura()));

                                targetHit.transform.parent.GetComponent<RPG_Stats>().SetSaludActual(
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetSaludActual() - ((stats.GetFuerza() + stats.GetTecPuño()) -
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetAbsorcionArmadura()));

                                targetHit.transform.parent.GetComponent<RPG_Stats>().SetArmaduraActual(
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetArmaduraActual() -
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetAbsorcionArmadura());
                            }
                            else
                            {
                                targetHit.transform.parent.GetComponent<RPG_Stats>().SetArmaduraActual(
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetArmaduraActual() -
                                (stats.GetFuerza() + stats.GetTecPuño()));
                            }                         
                        }
                        else
                        {
                            targetHit.transform.parent.GetComponent<PlayerController>().HitNumber(
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetSaludActual(), ((stats.GetFuerza() + stats.GetTecPuño()) -
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetArmaduraActual()));

                            targetHit.transform.parent.GetComponent<RPG_Stats>().SetSaludActual(
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetSaludActual() - ((stats.GetFuerza() + stats.GetTecPuño()) -
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetArmaduraActual()));

                            targetHit.transform.parent.GetComponent<RPG_Stats>().SetArmaduraActual(0);
                            targetHit.transform.parent.GetComponent<RPG_Stats>().SetAbsorcionArmadura(0);
                        }             
                    }   
                    else if(isKick)
                    {
                        if (targetHit.transform.parent.GetComponent<RPG_Stats>().GetArmaduraActual() >=
                        targetHit.transform.parent.GetComponent<RPG_Stats>().GetAbsorcionArmadura())
                        {
                            if (((stats.GetFuerza() + stats.GetTecPatada()) -
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetAbsorcionArmadura()) > 0)
                            {
                                targetHit.transform.parent.GetComponent<PlayerController>().HitNumber(
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetSaludActual(), ((stats.GetFuerza() + stats.GetTecPatada()) -
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetAbsorcionArmadura()));

                                targetHit.transform.parent.GetComponent<RPG_Stats>().SetSaludActual(
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetSaludActual() - ((stats.GetFuerza() + stats.GetTecPatada()) -
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetAbsorcionArmadura()));

                                targetHit.transform.parent.GetComponent<RPG_Stats>().SetArmaduraActual(
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetArmaduraActual() -
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetAbsorcionArmadura());
                            }
                            else
                            {
                                targetHit.transform.parent.GetComponent<RPG_Stats>().SetArmaduraActual(
                                targetHit.transform.parent.GetComponent<RPG_Stats>().GetArmaduraActual() -
                                (stats.GetFuerza() + stats.GetTecPatada()));
                            }
                        }
                        else
                        {
                            targetHit.transform.parent.GetComponent<PlayerController>().HitNumber(
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetSaludActual(), ((stats.GetFuerza() + stats.GetTecPatada()) -
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetArmaduraActual()));

                            targetHit.transform.parent.GetComponent<RPG_Stats>().SetSaludActual(
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetSaludActual() - ((stats.GetFuerza() + stats.GetTecPatada()) -
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetArmaduraActual()));

                            targetHit.transform.parent.GetComponent<RPG_Stats>().SetArmaduraActual(0);
                            targetHit.transform.parent.GetComponent<RPG_Stats>().SetAbsorcionArmadura(0);
                        }
                    }
                  if (Random.Range(1, 8) == 2) targetHit.transform.parent.GetComponent<PlayerController>().GetStunned();
                }
                else
                {
                    if(isKick)
                    {
                        if (((stats.GetFuerza() + stats.GetTecPatada()) -
                        targetHit.transform.parent.GetComponent<RPG_Stats>().GetTecBloqueo()) > 0)
                        {
                            targetHit.transform.parent.GetComponent<PlayerController>().HitNumber(
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetSaludActual(), ((stats.GetFuerza() + stats.GetTecPatada()) -
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetTecBloqueo()));

                            targetHit.transform.parent.GetComponent<RPG_Stats>().SetSaludActual(
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetSaludActual() - ((stats.GetFuerza() + stats.GetTecPatada()) -
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetTecBloqueo()));
                        }
                    }
                    if (isPunch)
                    {
                        if (((stats.GetFuerza() + stats.GetTecPuño()) -
                        targetHit.transform.parent.GetComponent<RPG_Stats>().GetTecBloqueo()) > 0)
                        {
                            targetHit.transform.parent.GetComponent<PlayerController>().HitNumber(
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetSaludActual(), ((stats.GetFuerza() + stats.GetTecPuño()) -
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetTecBloqueo()));

                            targetHit.transform.parent.GetComponent<RPG_Stats>().SetSaludActual(
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetSaludActual() - ((stats.GetFuerza() + stats.GetTecPuño()) -
                            targetHit.transform.parent.GetComponent<RPG_Stats>().GetTecBloqueo()));
                        }
                    }
                }
            }
        
        isPunch = false;
        isKick = false;
    }
    public bool IsBlocking() { return isBlocking; }
    private void CheckBlocking()
    {      
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Blocking") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.45f)
        {
            isBlocking = false;
        }
    }
    private void LookAtTarget()
    {
        if (lastDir == 1)
        {
            spriteRenderer.flipX = false;
        }
        else if (lastDir == -1)
        {
            spriteRenderer.flipX = true;
        }
       
    }

    public void GetHit()
    {
        animator.SetBool("isHittingNoWalk", true);
        agent.isStopped = true;           
        hitBlood.Play();               
        if (!secondAnim)
        {
            animator.SetTrigger("isHit");
            FindObjectOfType<AudioManager>().Play("ManDead", 1, 1);
            secondAnim = true;
        }
        else
        {
            animator.SetTrigger("isHit2");
            FindObjectOfType<AudioManager>().Play("ManHit", 1, 1);
            secondAnim = false;
        }       
    }
    public void GetHitDash(float direction)
    {      
        animator.SetBool("isHittingNoWalk", true);          
        hitBlood.Play();
        dashTrail.Play();
        FindObjectOfType<AudioManager>().Play("ManDead", 0.85f, 1.25f);
        animator.SetTrigger("isHitDash");
        transform.parent.Translate(new Vector3(direction * 35f, 0f, 0f),Space.World);          
    }

    public void GetStunned()
    {       
        hitBlood.Play();
        animator.SetTrigger("isStunned");
        if(!isStunned)Expresar("Stun");
    }
    private void CheckStunned()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stunned"))
        {
            isStunned = true;
        }
        else
        {
            isStunned = false;
        }
    }
    public bool IsStunned() { return isStunned; }

    private void HUD()
    {
        saludBarra.fillAmount = ((stats.GetSaludActual() * 100) / stats.GetSaludTotal()) / 100;
        //LIMITES
        if (stats.GetSaludActual() > stats.GetSaludTotal()) { stats.SetSaludActual(stats.GetSaludTotal()); }
        if (stats.GetSaludActual() < 0) { stats.SetSaludActual(0); }
        ShowHUD();
        if (stats.GetSaludActual() <= 0) { Death(); }
    }
    private void Death()
    {
        if (!isDead)
        {
                GetComponent<SpriteRenderer>().sortingOrder = 2;
                animator.SetBool("isDead", true);
                agent.enabled = false;
                transform.parent.GetComponent<Rigidbody2D>().simulated = false;
                transform.parent.GetComponent<BoxCollider2D>().enabled = false;
                charColl.enabled = false;           
                transform.parent.GetComponent<SpriteRenderer>().enabled=false;
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                GameController.IncrementarEnemigosDerrotadosZona1();
            }       
            isDead = true;            
        }
        else
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dead") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f)
            {              
                animator.SetBool("isDeadFinal", true);
                if(!isDeadFinal)transform.position += new Vector3(0, -10f, 0);          
                isDeadFinal = true;
            }
        }
        if (isDeadFinal)
        {
            timeToDestroyOnDeath -= Time.deltaTime;
            if (timeToDestroyOnDeath <= 0)
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }
    public void HitNumber(float valorTotal, float valorRestado)
    {
        float valorRestante = valorTotal - valorRestado;
        float valorSustraido = valorTotal - valorRestante;
        GameObject hitNumber = Instantiate(hitNumberPrefab, hitNumberPosition.transform);
        hitNumber.GetComponent<Canvas>().worldCamera = Camera.main;
        hitNumber.transform.Find("DamageNumber").GetComponent<Text>().text = valorSustraido.ToString();
    }
    private void ShowHUD()
    {
        if (stats.GetSaludActual() != stats.GetSaludRef())
        {
            saludObj.SetActive(true);
            stats.SetSaludRef(stats.GetSaludActual());           
        }
        if (saludObj.activeSelf)
        {
            timeShowingHUD -= Time.deltaTime;
            if (timeShowingHUD <= 0)
            {
                saludObj.SetActive(false);
                timeShowingHUD = timerShowHUD;
            }
        }
    }

    public void Expresar(string expresion)
    {
        switch (expresion)
        {
            case "Atencion": Instantiate(atencion, expresionPosition.transform); break;
            case "Exclamacion": Instantiate(exclamacion, expresionPosition.transform); break;
            case "Interrogacion": Instantiate(interrogacion, expresionPosition.transform); break;
            case "Contento": Instantiate(contento, expresionPosition.transform); break;
            case "Enfadado": Instantiate(enfadado, expresionPosition.transform); break;
            case "Cantar": Instantiate(cantar, expresionPosition.transform); break;
            case "Stun": Instantiate(stun, expresionPosition.transform); break;

            case "Hablar":
                Instantiate(hablar, expresionPosition.transform);
                Instantiate(boca, bocaPosition.transform); break;
        }
    }
    public void AcabarExpresionLoop()
    {
        if (expresionPosition.transform.childCount > 0) { Destroy(expresionPosition.transform.GetChild(0).gameObject); }
        if (bocaPosition.transform.childCount > 0) { Destroy(bocaPosition.transform.GetChild(0).gameObject); }
    }

    private void CheckIfAttacking()
    {
        if (animator.GetBool("isHittingNoWalk") &&
          (!animator.GetCurrentAnimatorStateInfo(0).IsName("PunchLight") ||
          !animator.GetCurrentAnimatorStateInfo(0).IsName("Blocking") ||
          !animator.GetCurrentAnimatorStateInfo(0).IsName("KickJump") ||
          !animator.GetCurrentAnimatorStateInfo(0).IsName("KickLight")) &&
          animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
        {
            animator.SetBool("isHittingNoWalk", false);
            isBlocking = false;          
        }
    }

    private void CheckPlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player.transform.position.y > transform.position.y)
        {
            GetComponent<SpriteRenderer>().sortingOrder= 5;
        }
        else
        {
            GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
    }
    private void CheckEnemyPosition()
    {
        List<GameObject> enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        foreach (GameObject enemy in enemies)
        {
            if (enemy != gameObject)
            {
                if (enemy.transform.position.y > transform.position.y)
                {
                    GetComponent<SpriteRenderer>().sortingOrder = 5;
                }
                else
                {
                    GetComponent<SpriteRenderer>().sortingOrder = 3;
                }
            }
        }
        CheckPlayerPosition();
    }

    private void CheckGameTimeProcess()
    {
        if(player.GetComponent<PlayerController>().GamePaused())
        {
            movement = false;
            animator.enabled = false;
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        else
        {
            if (playerInSight) { movement = true; agent.isStopped = false; }
            animator.enabled = true;
            
        }
    }

    private void FixedUpdate()
    {
        HUD();
        if (!isDead)
        {      
            CheckEnemyPosition();
            EnemyBehavior();
            LookAtTarget();           
            CheckBlocking();
            CheckStunned();
            CheckIfAttacking();
            CheckGameTimeProcess();
        }
    }
}
