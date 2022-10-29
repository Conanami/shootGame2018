using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//这是挂在钥匙上的脚本
public class fps_KeyPickUP : MonoBehaviour
{
    public AudioClip keyPick;  //拿钥匙的音效
    public int keyId;           //拿到的钥匙ID

    private GameObject player;
    private fps_PlayerInventory playerInventory;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerInventory = player.GetComponent<fps_PlayerInventory>();
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject==player)
        {
            AudioSource.PlayClipAtPoint(keyPick, transform.position);   //播放拿到钥匙的声音
            playerInventory.AddKey(keyId);   //要是放进背包
            Destroy(this.gameObject);  //销毁自身，是gameObject，不是this ， this是脚本
        }
    }
}
