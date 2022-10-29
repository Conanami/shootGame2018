using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class fps_EnemyHealth : MonoBehaviour
{
    public float hp = 100;
    public float rebornTime = 5f;

    private Animator anim;
    private HashIDs hash;
    private bool isDead = false;
    private float timer;
    private void Start()
    {
        anim = this.GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

    }
    private void Update()
    {
        if (isDead)
        {
            timer += Time.deltaTime;
            if (timer >= rebornTime)
            {
                Reborn();
                
            }
        }
        else
            timer = 0;
    }
    private void Reborn()
    {
        hp = 100;
        isDead = false;
        timer = 0;
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<fps_EnemySight>().enabled = true;
        GetComponent<fps_EnemyAI>().enabled = true;
        GetComponent<fps_EnemyShoot>().enabled = true;
        GetComponent<fps_EnemyAnimator>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponentInChildren<LineRenderer>().enabled = true;
        GetComponentInChildren<Light>().enabled = true;
        anim.SetBool(hash.deadBool, false);
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0 && isDead == false)
        {
            isDead = true;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<fps_EnemySight>().enabled = false;
            GetComponent<fps_EnemyAI>().enabled = false;
            GetComponent<fps_EnemyShoot>().enabled = false;
            GetComponent<fps_EnemyAnimator>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponentInChildren<LineRenderer>().enabled = false;
            GetComponentInChildren<Light>().enabled = false;
            anim.SetBool(hash.deadBool, true);
            anim.SetBool(hash.playerInSightBool,false);
        }

    }
}
