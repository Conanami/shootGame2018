using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//挂在激光上的脚本，主要是伤害我
public class fps_LaserDamage : MonoBehaviour
{
    public float damage = 30;
    public float delayDamageTime = 1;

    private float lastDamageTime=0;
    private GameObject player;
    void Start()   //初始化里面就是得到，一般玩家都要得到的
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);    //得到玩家
    }

    void OnTriggerStay(Collider other) //可以做到每隔多久伤害一下
    {
        if (other.gameObject == player && Time.time > lastDamageTime + delayDamageTime)
        {
            player.GetComponent<fps_PlayerHealth>().TakeDamage(damage);
            lastDamageTime = Time.time;
        }
    }
}
