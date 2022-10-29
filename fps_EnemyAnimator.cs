using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class fps_EnemyAnimator : MonoBehaviour
{
    public float deadZone = 5f;  //转到5度以内，就直接设置成正方向

    private Transform player;
    private fps_EnemySight enemySight;
    private NavMeshAgent nav;
    private HashIDs hash;
    private AnimatorSetup animSetup;
    private Animator anim;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        enemySight = this.GetComponent<fps_EnemySight>();
        nav = this.GetComponent<NavMeshAgent>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        anim = this.GetComponent<Animator>();
        animSetup = new AnimatorSetup(anim, hash);

        nav.updateRotation = false;  //寻路组件的旋转，要禁用掉。通过动画来控制。
        anim.SetLayerWeight(1, 1f);  //Shooting层，权重为1
        anim.SetLayerWeight(2, 1f);  //grab层，权重为1 , 因为设定了骨骼 avater mask，所以权重为1，不会受其它动画影响。

        deadZone *= Mathf.Deg2Rad;    //DeadZone角度转弧度，取决于后面函数怎么用。

    }
    private void Update()
    {
        NavAnimSetup();
    }

    private void OnAnimatorMove()
    {
        nav.velocity = anim.deltaPosition / Time.deltaTime;   //速度设置为上一帧动画的速度
        transform.rotation = anim.rootRotation;               //有根的动画，让 transform的旋转跟着动画走

    }

    private float FindAngle(Vector3 fromV , Vector3 toV,Vector3 upV)
    {
        if (toV == Vector3.zero)
            return 0f;
        float angle = Vector3.Angle(fromV, toV);
        Vector3 normal = Vector3.Cross(fromV, toV);
        angle *= Mathf.Sign(Vector3.Dot(normal, upV));
        angle *= Mathf.Deg2Rad;
        return angle;
    }
    void NavAnimSetup()
    {
        float speed;
        float angle;
        if(enemySight.playerInSight)
        {
            speed = 0;
            angle = FindAngle(transform.forward, player.position - transform.position, transform.up);

        }
        else
        {
            speed = Vector3.Project(nav.desiredVelocity, transform.forward).magnitude;
            angle = FindAngle(transform.forward, nav.desiredVelocity, transform.up);
            if(Mathf.Abs(angle)<deadZone)
            {
                transform.LookAt(transform.position + nav.desiredVelocity);
                angle = 0;
            }
        }
        animSetup.Setup(speed, angle);
    }

}
