using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class fps_EnemySight : MonoBehaviour
{
    public float fieldOfViewAngle = 110f;
    public bool playerInSight;
    public Vector3 playerPosition;
    public Vector3 resetPosition = Vector3.zero;

    private NavMeshAgent nav;
    private SphereCollider col;
    private Animator anim;
    private GameObject player;

    private fps_PlayerHealth playerHealth;
    private fps_PlayerControl playerControl;
    private HashIDs hash;

    private void Start()
    {
        nav = this.GetComponent<NavMeshAgent>();
        col = this.GetComponentInChildren<SphereCollider>();  //应为这个侦听collider挂在子物体上的，body
        anim = this.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerHealth = player.GetComponent<fps_PlayerHealth>();
        playerControl = player.GetComponent<fps_PlayerControl>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        fps_GunScript.PlayerShootEvent += ListenPlayer;       //c#添加监听事件，
                                                              //这个在销毁的时候就要取消掉，不然报错
        playerInSight = false;
    }
    private void Update()
    {
        if (playerHealth.hp > 0)            
            anim.SetBool(hash.playerInSightBool, playerInSight);
        else
            anim.SetBool(hash.playerInSightBool, false);
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject == player)
        {
            playerInSight = false;
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);  
            if(angle<fieldOfViewAngle*0.5)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, direction, out hit, col.radius))
                {
                    if(hit.collider.gameObject==player)
                    {
                        playerInSight = true;
                        playerPosition = player.transform.position;
                    }
                }
            }
            if(playerControl.State==PlayerState.Walk || playerControl.State==PlayerState.Run)
            {
                ListenPlayer();
            }
        }
    }
    void ListenPlayer()   //如果只在OnTriggerStay里面用，第一句就是废话，后面看看
    {
        if(Vector3.Distance(player.transform.position,transform.position)<=col.radius)
            playerPosition = player.transform.position;
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject==player)
        {
            playerInSight = false;
        }
    }
    private void OnDestroy()
    {
        fps_GunScript.PlayerShootEvent -= ListenPlayer;
    }
}
