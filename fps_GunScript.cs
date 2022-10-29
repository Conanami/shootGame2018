using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void PlayerShoot();
public class fps_GunScript : MonoBehaviour
{
    public static event PlayerShoot PlayerShootEvent;
    public float fireRate = 0.1f;
    public int damage = 40;
    public float reloadTime = 1.5f;
    public float flashRate = 0.02f;
    public AudioClip fireAudio;
    public AudioClip reloadAudio;
    public AudioClip damageAudio;
    public AudioClip dryFireAudio;
    public GameObject explosion;
    public int bulletCount = 30;
    public int chargerBulletCount = 60;
    public Text bulletText;
    
    private string reloadAnim = "Reload";
    private string fireAnim = "Single_Shot";
    private string walkAnim = "Walk";
    private string runAnim = "Run";
    private string jumpAnim = "Jump";
    private string idleAnim = "Idle";

    private Animation anim;
    private float nextFireTime = 0.0f;
    private MeshRenderer flash;
    private int currentBullet;
    private int currentChargerBullet;
    private fps_PlayerParameter parameter;
    private fps_PlayerControl playerControl;

    private void Start()
    {
        parameter = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerParameter>();
        playerControl = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerControl>();
        anim = this.GetComponent<Animation>();
        flash = this.transform.Find("muzzle_flash").GetComponent<MeshRenderer>();
        flash.enabled = false;
        currentBullet = bulletCount;
        currentChargerBullet = chargerBulletCount;
        bulletText.text = currentBullet + "/" + currentChargerBullet;
    }
    private void Update()
    {
        if (parameter.inputReload && currentBullet < bulletCount) //装弹
            Reload();
        if (parameter.inputFire && !anim.IsPlaying(reloadAnim))  //开火
            Fire();
        else if (!anim.IsPlaying(reloadAnim))   //每个状态不同的拿枪动画
            StateAnim(playerControl.State);
    }

    void Fire()
    {
        if(Time.time>nextFireTime)   //标准时间间隔的写法
        {
            if(currentBullet<=0)
            {
                Reload();
                nextFireTime = Time.time + fireRate;   //一秒钟打出10个子弹,fireRate决定射击速度，是枪的特质
                return;
            }
            currentBullet--;
            nextFireTime = Time.time + fireRate;
            bulletText.text = currentBullet + "/" + currentChargerBullet;

            DamageEnemy();
            if (PlayerShootEvent != null)
                PlayerShootEvent();
            FireEffect();   //声音动画效果

        }
    }

    void FireEffect()
    {
        //播放声音，播放动画，都是必须的
        AudioSource.PlayClipAtPoint(fireAudio, transform.position);
        anim.Rewind(fireAnim);
        anim.Play(fireAnim);
        StartCoroutine(Flash());
    }
    private void PlayerStateAnim(string animName)
    {
        if(!anim.IsPlaying(animName))
        {
            anim.Rewind(animName);
            anim.Play(animName);
        }
    }
    private void StateAnim(PlayerState state)
    {
        switch (state)
        {
            
            case PlayerState.Idle:
                PlayerStateAnim(idleAnim);
                break;
            case PlayerState.Walk:
                PlayerStateAnim(walkAnim);
                break;
            case PlayerState.Crouch:
                PlayerStateAnim(walkAnim);
                break;
            case PlayerState.Run:
                PlayerStateAnim(runAnim);
                break;
            
        }
    }
    void EnemyDamageEffect(RaycastHit hit)
    {
        AudioSource.PlayClipAtPoint(damageAudio, hit.transform.position);
        GameObject go = Instantiate(explosion, hit.point, Quaternion.identity);
        Destroy(go, 3);
        hit.transform.GetComponent<fps_EnemyHealth>().TakeDamage(damage);

    }
    private void DamageEnemy()
    {
        //从相机向屏幕中心发射一条射线
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit))
        {
            //射线击中了敌人
            if(hit.transform.tag==Tags.enemy && hit.collider is CapsuleCollider)
            {
                EnemyDamageEffect(hit);
                
            }
        }
    }
    IEnumerator Flash()   //射击火光的闪烁
    {
        flash.enabled = true;
        yield return new WaitForSeconds(flashRate);
        flash.enabled = false;
    }

    void Reload()
    {
        if(!anim.IsPlaying(reloadAnim))
        {
            if(currentChargerBullet>0)
            {
                StartCoroutine(ReloadFinish());
                //虽然是先执行，但是因为是个协程，所以等到换子弹动画放完后，才刷新子弹数量
            }
            else
            {
                //播放射击动画，但是没子弹了，放空的声音，也没有射击闪光
                anim.Rewind(fireAnim);
                anim.Play(fireAnim);
                AudioSource.PlayClipAtPoint(dryFireAudio, transform.position);
                return;
            }
            AudioSource.PlayClipAtPoint(reloadAudio, transform.position);
            ReloadAnim();

        }
    }
    IEnumerator ReloadFinish()    //用一个协程来完成reload以后的子弹数量变化和重新显示
    {
        yield return new WaitForSeconds(reloadTime);     // yield return 就是先停下来，然后把控制权交给上级
        if(currentChargerBullet>=bulletCount-currentBullet)
        {
            currentChargerBullet -= bulletCount - currentBullet;
            currentBullet = bulletCount;
        }
        else
        {
            currentBullet += currentChargerBullet;
            currentChargerBullet = 0;
        }
        bulletText.text = currentBullet + "/" + currentChargerBullet;
    }
    void ReloadAnim()   //播放换子弹动画，确保规定时间内换完，所以要设定speed
    {
        anim.Stop(reloadAnim);
        anim[reloadAnim].speed = anim[reloadAnim].length/reloadTime;
        anim.Rewind(reloadAnim);
        anim.Play(reloadAnim);
    }
}
