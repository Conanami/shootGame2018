using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//这是挂在出口上的脚本
public class fps_ExitTrigger : MonoBehaviour
{
    public float timeToInactivePlayer = 2.0f;
    public float timeToRestart = 5.0f;

    private GameObject player;
    private bool playerInExit;
    private FadeInOut fader;
    private float timer=0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
        fader = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<FadeInOut>();
    }
    void Update()
    {
        if (playerInExit)
            playerInExitProcess();
    }

    void OnTriggerEnter(Collider other)
    { 
        if(other.gameObject == player)
        {
            playerInExit = true;

        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject==player)
        {
            playerInExit = false;
            timer = 0;
        }
    }

    private void playerInExitProcess()
    {
        timer += Time.deltaTime;
        if (timer >= timeToInactivePlayer)
            player.GetComponent<fps_PlayerHealth>().DisableInput();
        if (timer >= timeToRestart)
            fader.EndScene();
        
    }

}
