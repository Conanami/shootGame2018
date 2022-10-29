using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class fps_EnemyAI : MonoBehaviour
{
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float patrolWaitTime = 1.0f;
    public float chaseWaitTime = 5.0f;
    public Transform[] patrolWayPoint;      //巡逻路点数组

    private Transform player;
    private fps_EnemySight enemySight;
    private NavMeshAgent nav;
    private fps_PlayerHealth playerHealth;
    private float chaseTimer;
    private float patrolTimer;
    private int wayPointIndex;

    private void Start()
    {
        nav = this.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        enemySight = this.GetComponent<fps_EnemySight>();
        playerHealth = player.gameObject.GetComponent<fps_PlayerHealth>();

    }

    private void Update()
    {
        if (enemySight.playerInSight && playerHealth.hp > 0)
        {
            Shooting();
        }
        else if (enemySight.playerPosition != enemySight.resetPosition && playerHealth.hp > 0)
        {
            Chasing();
        }
        else
            Patrolling();
    }

    private void Shooting()  //具体开枪在 EnemyShoot脚本中，这里只需要不寻路了，直接停下就行。
    {
        nav.SetDestination(transform.position);
    }

    private void Chasing()
    {
        Vector3 sightDeltaPos = enemySight.playerPosition - transform.position;
        //先计算玩家与敌人之间的距离
        //如果距离平方大于4，就设定为追击目标
        if(sightDeltaPos.sqrMagnitude>4f)
        {
            nav.destination = enemySight.playerPosition;   //设定为自动寻路目标
        }
        nav.speed = chaseSpeed;   //设定速度
        //如果追到该点
        if (nav.remainingDistance < nav.stoppingDistance)
        {
            //就在该点开始等待
            chaseTimer += Time.deltaTime;
            if (chaseTimer > chaseWaitTime)
            {
                //等了超过时间，就说明追丢了，重新更新玩家位置
                enemySight.playerPosition = enemySight.resetPosition;
                chaseTimer = 0;
            }
        }
        else   //否则就说明还没追到，timer清零
            chaseTimer = 0;

    }

    private void Patrolling()
    {
        nav.speed = patrolSpeed;
        //如果到达了一个巡逻点，等 patrolWaitTime后，就切换到下一个巡逻点
        if(nav.destination == enemySight.resetPosition || nav.remainingDistance<nav.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;
            if(patrolTimer>patrolWaitTime)
            {
                if (wayPointIndex >= patrolWayPoint.Length - 1)
                {
                    wayPointIndex = 0;
                }
                else
                    wayPointIndex++;
                patrolTimer = 0;
            }
            
        }
        else
            patrolTimer = 0;
        
        nav.SetDestination(patrolWayPoint[wayPointIndex].position);
    }
}
