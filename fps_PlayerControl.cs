using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerState
{
        None,
        Idle,
        Walk,
        Crouch,
        Run,
}

public class fps_PlayerControl : MonoBehaviour
{
    private PlayerState state = PlayerState.None;
    public PlayerState State
    {
        get
        {
            if (running)
                state = PlayerState.Run;
            else if (walking)
                state = PlayerState.Walk;
            else if (crouching)
                state = PlayerState.Crouch;
            else
                state = PlayerState.Idle;
            return state;
        }
    }

    //面板中可以修改的参数
    public float sprintSpeed = 10.0f;
    public float sprintJumpSpeed = 8.0f;
    public float normalSpeed = 6.0f;
    public float normalJumpSpeed = 7.0f;
    public float crouchSpeed = 2.0f;
    public float crouchJumpSpeed = 5.0f;
    public float crouchDeltaHeight = 0.5f;  //蹲伏时候身体下降的高度
    public float gravity = 20.0f;

    public float cameraMoveSpeed = 8.0f;
    public AudioClip jumpAudio;

    private float speed;                //当前速度
    private float jumpSpeed;            //当前跳跃速度
    private Transform mainCamera;
    private float standardCamHeight;
    private float crouchCamHeight;
    
    private bool running = false;
    private bool crouching = false;
    private bool stopCrouching = false;
    private bool grounded = false;
    private bool walking = false;

    private CharacterController controller;
    private Vector3 normalControllerCenter = Vector3.zero;
    private float normalControllerHeight = 0.0f;

    private float timer = 0;
    private AudioSource audioSource;
    private fps_PlayerParameter parameter;

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        running = false;
        crouching = false;
        walking = false;
        speed = normalSpeed;
        jumpSpeed = normalJumpSpeed;

        mainCamera = GameObject.FindGameObjectWithTag(Tags.mainCamera).transform;
        standardCamHeight = mainCamera.localPosition.y;
        crouchCamHeight = standardCamHeight - crouchDeltaHeight;
        
        audioSource = this.GetComponent<AudioSource>();
        
        controller = this.GetComponent<CharacterController>();
        normalControllerCenter = controller.center;
        normalControllerHeight = controller.height;

        parameter = this.GetComponent<fps_PlayerParameter>();

    }

    private void FixedUpdate()  //每帧调用的
    {
        UpdateMove();
        AudioManagement();
    }

    private void UpdateMove()   //跟controller有关的，都在这里
    {
        if(grounded)
        {
            moveDirection = new Vector3(parameter.inputMoveVector.x, 0, parameter.inputMoveVector.y);
            moveDirection = transform.TransformDirection(moveDirection);
            //世界坐标向自身坐标转 ，输入按钮是根据它本身的朝向来动的，不是根据世界坐标，是根据它自己的坐标
            moveDirection *= speed;     //平地移动方向速度都有了

            if(parameter.inputJump)    //如果是按下了跳跃
            {
                //y轴获得一个向上的速度
                moveDirection.y = jumpSpeed;
                //播放跳跃音效
                AudioSource.PlayClipAtPoint(jumpAudio, transform.position);
                CurrentSpeed();
            }

        }
        //如果不在地上，那么就要收到重力影响
        moveDirection.y -= gravity*Time.deltaTime;

        //正经动就这句，动起来还要返回一个碰撞flags
        CollisionFlags  flags = controller.Move(moveDirection * Time.deltaTime);

        //如果碰到地面了，就算停下了
        grounded = (flags & CollisionFlags.CollidedBelow) != 0;

        //如果WSAD有按下，就根据是否在跑，是否在蹲伏，改变一下状态
        if((Mathf.Abs(moveDirection.x)>0 && grounded)|| (Mathf.Abs(moveDirection.z) > 0 && grounded))
        {
            if(parameter.inputCrouch)
            {
                running = false;
                crouching = true;
                walking = false;
            }
            else if(parameter.inputSprint) 
            {
                running = true;
                crouching = false;
                walking = false;
            }
            else
            {
                running = false;
                crouching = false;
                walking = true;
            }

        }
        else  //就是没有WASD输入，原地不动了
        {
            if (walking)
                walking = false;
            else if (running)
                running = false;
            else if (parameter.inputCrouch)
                crouching = true;
            else
                crouching = false;

        }

        //蹲伏情况下，还要调整controller的高度和中心点
        if(crouching)
        {
            controller.height = normalControllerHeight - crouchDeltaHeight;
            controller.center = normalControllerCenter - new Vector3(0, crouchDeltaHeight / 2, 0);
        }
        UpdateCrouch();
        CurrentSpeed();
    }
    private void CurrentSpeed()
    {
        //根据状态调整速度
        switch (State)
        {
            case PlayerState.Idle:
                speed = normalSpeed;
                jumpSpeed = normalJumpSpeed;
                break;
            case PlayerState.Walk:
                speed = normalSpeed;
                jumpSpeed = normalJumpSpeed;
                break;
            case PlayerState.Crouch:
                speed = crouchSpeed;
                jumpSpeed = crouchJumpSpeed;
                break;
            case PlayerState.Run:
                speed = sprintSpeed;
                jumpSpeed = sprintJumpSpeed;
                break;
            
        }
    }

    private void AudioManagement()
    {
        if (State == PlayerState.Walk)
        {
            //Pitch（放得快就声音尖了？）：播放音频时速度的变化量 ，默认值1，表示正常的播放速度。
            //（//当<1时，慢速播放；当>1时，快速播放。速度越快，音调越高。）
            audioSource.pitch = 1.0f;
            if (!audioSource.isPlaying)
                audioSource.Play();

        }
        else if (State == PlayerState.Run)
        {
            //Pitch（放得快就声音尖了？）：播放音频时速度的变化量 ，默认值1，表示正常的播放速度。
            //（//当<1时，慢速播放；当>1时，快速播放。速度越快，音调越高。）
            audioSource.pitch = 1.3f;
            if (!audioSource.isPlaying)
                audioSource.Play();

        }
        else
            audioSource.Stop();
    }

    private void UpdateCrouch()
    {
        //调整的是相机
        if(crouching)   //蹲伏状态下
        {
            if(mainCamera.localPosition.y>crouchCamHeight)
            {
                if(mainCamera.localPosition.y-Time.deltaTime*cameraMoveSpeed<crouchCamHeight)
                {
                    mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, crouchCamHeight, mainCamera.localPosition.z);
                }
                else
                    mainCamera.localPosition -= new Vector3(0, Time.deltaTime * cameraMoveSpeed, 0);
            }
            else
                mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, crouchCamHeight, mainCamera.localPosition.z);
        }
        else   //其它状态下
        {
            if (mainCamera.localPosition.y < standardCamHeight)
            {
                if (mainCamera.localPosition.y + Time.deltaTime * cameraMoveSpeed > standardCamHeight)
                {
                    mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, standardCamHeight, mainCamera.localPosition.z);
                }
                else
                    mainCamera.localPosition += new Vector3(0, Time.deltaTime * cameraMoveSpeed, 0);
            }
            else
                mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, standardCamHeight, mainCamera.localPosition.z);

        }
    }
}
