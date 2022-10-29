using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//这是挂在会动的门上的脚本，平移的门，上下左右都可以
public class fps_DoorControl : MonoBehaviour
{
    public int doorId;
    public Vector3 from;
    public Vector3 to;
    public float fadeSpeed = 5;
    public bool requireKey = false;
    public AudioClip doorSwitchClip;
    public AudioClip accessDenyClip;

    private Transform door;
    private GameObject player;
    private AudioSource audioSource;
    private fps_PlayerInventory playerInventory;
    private int count;              //人物的数量
    public int Count
    {
        get
        {
            return count;
        }
        set
        {
            if((count==1&&value==0)||(count==0&&value==1))
            {
                audioSource.clip = doorSwitchClip;
                audioSource.Play();
            }
            count = value;
        }
    }
    void Start()
    {
        if(transform.childCount>0)
        {
            //得到第一个子物体是门
            door = transform.GetChild(0);
        }
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerInventory = player.GetComponent<fps_PlayerInventory>();
        audioSource = this.GetComponent<AudioSource>();
        door.localPosition = from;    
    }

    void Update()
    {
        if (Count > 0)
        {
            door.localPosition = Vector3.Lerp(door.localPosition, to, fadeSpeed * Time.deltaTime);
        }
        else
            door.localPosition = Vector3.Lerp(door.localPosition, from, fadeSpeed * Time.deltaTime);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            if (requireKey)
                if (playerInventory.HasKey(doorId))
                {
                    Count++;
                }
                else
                {
                    audioSource.clip = accessDenyClip;
                    audioSource.Play();
                }
            else
            {
                Count++;
            }

        }
        else if(other.gameObject.tag==Tags.enemy && other is CapsuleCollider) //因为敌人还有一个监听碰撞器
        {
            Count++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player || (other.gameObject.tag == Tags.enemy && other is CapsuleCollider))
        {
            Count = Mathf.Max(0, Count - 1);
        }
    }
}
