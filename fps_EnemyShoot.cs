using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//脚本是挂在机器人身上的
public class fps_EnemyShoot : MonoBehaviour
{
    public float maxDamage = 12;
    public float minDamage = 5;
    public AudioClip shotClip;
    public float flashIntensity = 3f;
    public float fadeSpeed = 10f;

    private Animator anim;
    private HashIDs hash;
    private LineRenderer laserShotLine;
    private Light laserShotLight;
    private SphereCollider col;
    private fps_PlayerHealth playerHealth;
    private Transform player;
    private bool shooting;
    private float scaledDamage;

    private void Start()
    {
        anim = this.GetComponent<Animator>();
        laserShotLine = this.GetComponentInChildren<LineRenderer>();
        laserShotLight = laserShotLine.gameObject.GetComponent<Light>();
        col = this.GetComponentInChildren<SphereCollider>();
        playerHealth = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerHealth>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;

        laserShotLight.intensity = 0;
        laserShotLine.enabled = false;

        scaledDamage = maxDamage - minDamage;

        

    }

    private void Update()
    {
        //这种用动画状态机的值来控制的，好像比那种startcoroutine要好。
        float shot = anim.GetFloat(hash.shotFloat);  
        if (shot > 0.5 && !shooting)
        {
            Shoot();
        }
        else if (shot < 0.5)
        {
            NoShootEffect();
        }
    }
    private void OnAnimatorIK(int layerIndex)
    {
        float aimWeight = anim.GetFloat(hash.aimWeightFloat); // aimWeight和shot是显示动画进度的参数
        //在动画curve里面可以找到
        anim.SetIKPosition(AvatarIKGoal.RightHand, player.position + Vector3.up * 1.5f);  //举枪动作
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, aimWeight);
    }
    private void Shoot()
    {
        shooting = true;
        float fractionalDistance = (col.radius-Vector3.Distance(transform.position,player.position))/ col.radius;
        float damage = minDamage + scaledDamage * fractionalDistance;
        playerHealth.TakeDamage(damage);
        ShootEffect();
    }

    private void ShootEffect()  
    {
        laserShotLine.SetPosition(0, laserShotLine.transform.position);
        laserShotLine.SetPosition(1, player.position+Vector3.up*1.5f);
        laserShotLine.enabled = true;
        laserShotLight.intensity = flashIntensity;

        AudioSource.PlayClipAtPoint(shotClip, laserShotLine.transform.position);

    }

    private void NoShootEffect()
    {
        shooting = false;
        laserShotLine.enabled = false;
        laserShotLight.intensity = Mathf.Lerp(laserShotLight.intensity, 0, fadeSpeed * Time.deltaTime);
    }
}
