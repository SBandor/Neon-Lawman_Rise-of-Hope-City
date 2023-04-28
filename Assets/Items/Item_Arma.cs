using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Arma : MonoBehaviour
{
    public int daño, durabilidad;
    public LayerMask enemyLayer;
    private float timeAlive = 2f;
    private bool isThrown = false;
    private RPG_Stats playerStats;
    
    public void IsThrown() { isThrown = true; }
    public void ReducirDurabilidad() {   durabilidad-=1;  }
    public int GetDaño() { return daño; }
    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").transform.parent.GetComponent<RPG_Stats>();
    }
    void Update()
    {
        if(durabilidad>0)
        {
            if (isThrown)
            {
                RaycastHit2D[] targetHit = Physics2D.CircleCastAll(transform.position, 4, new Vector2(0, 0), enemyLayer);
                int contadorTargets = 0;
                foreach (RaycastHit2D hit in targetHit)
                {
                    if (hit.transform.gameObject.tag == "Enemy")
                    {
                        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                        GetComponent<Rigidbody2D>().angularVelocity = 0f;
                        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                        GetComponent<SpriteRenderer>().sortingOrder = 2;
                        timeAlive = 2f;

                        hit.transform.GetComponent<EnemyController>().GetHit();
                        hit.transform.GetComponent<EnemyController>().HitNumber(
                                   hit.transform.GetComponent<RPG_Stats>().GetSaludActual(), (playerStats.GetFuerza() + daño));

                        hit.transform.GetComponent<RPG_Stats>().SetSaludActual(hit.transform.GetComponent<RPG_Stats>().GetSaludActual() -
                            (playerStats.GetFuerza() + daño));

                        if (Random.Range(1, 4) == 2) { hit.transform.GetComponent<EnemyController>().GetStunned(); }
                        Destroy(transform.gameObject);
                    }
                    else
                    {
                        contadorTargets++;

                        timeAlive -= Time.deltaTime;
                        if (timeAlive <= 0)
                        {
                            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                            GetComponent<Rigidbody2D>().angularVelocity = 0f;
                            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                            GetComponent<SpriteRenderer>().sortingOrder = 2;
                            timeAlive = 2f;
                            durabilidad--;
                            isThrown = false;             
                        }
                    }
                }
            }
        }
        else { Destroy(transform.gameObject); }    
    }
}
