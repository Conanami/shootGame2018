using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//背包脚本，挂在玩家身上
public class fps_PlayerInventory : MonoBehaviour
{
    //保存钥匙卡的ID，ID对应门
    private List<int> keysArr;
    void Start()
    {
        keysArr = new List<int>();
    }
    
    public void AddKey(int keyId)
    {
        if(!keysArr.Contains(keyId))
            keysArr.Add(keyId);
        
    }
    public bool HasKey(int doorId)   //是否拥有这个门的钥匙卡
    {
        if(keysArr.Contains(doorId))
        {
            return true;
        }
        return false;
    }
}
