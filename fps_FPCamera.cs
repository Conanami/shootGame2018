using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class fps_FPCamera : MonoBehaviour
{
    public Vector2 mouseLookSensitivity = new Vector2(5, 5);
    public Vector2 rotationXLimit = new Vector2(-87, 87);
    public Vector2 rotationYLimit = new Vector2(-360, 360);
    public Vector2 positionOffset = new Vector3(0, 2, -0.2f);
    private Vector2 currentMouseLook = Vector2.zero;

    private float x_Angle = 0;
    private float y_Angle = 0;
    private fps_PlayerParameter parameter;
    private Transform m_transform;

    void Start()
    {
        parameter = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerParameter>();
        m_transform = transform;     //Camera的transform
        m_transform.localPosition = positionOffset;
        
    }

    void Update()
    {
        UpdateInput();
        
    }

    void LateUpdate()
    {
        Quaternion xQuaternion = Quaternion.AngleAxis(y_Angle, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(0, Vector3.left);
        m_transform.parent.rotation = xQuaternion * yQuaternion;
        //这三行表示的是相机视角的左右移动会带动身体一起左右移动，但身体一直是直立的
        //不会随着相机的上下移动而改变角度

        yQuaternion = Quaternion.AngleAxis(-x_Angle, Vector3.left);
        m_transform.rotation = xQuaternion * yQuaternion;
        //相机本身是要跟着鼠标前后而上下动的

    }
    private void UpdateInput()
    {
        if (parameter.inputSmoothLook == Vector2.zero)
            return;
        GetMouseLook();
        y_Angle += currentMouseLook.x;
        x_Angle += currentMouseLook.y;

        y_Angle = y_Angle < -360 ? y_Angle += 360 : y_Angle;
        y_Angle = y_Angle > 360 ? y_Angle -= 360 : y_Angle;
        y_Angle = Mathf.Clamp(y_Angle,rotationYLimit.x,rotationYLimit.y);

        x_Angle = x_Angle < -360 ? x_Angle += 360 : x_Angle;
        x_Angle = x_Angle > 360 ? x_Angle -= 360 : x_Angle;
        x_Angle = Mathf.Clamp(x_Angle, rotationXLimit.x, rotationXLimit.y);
    }
    private void GetMouseLook()
    {
        currentMouseLook.x = parameter.inputSmoothLook.x;
        currentMouseLook.y = parameter.inputSmoothLook.y;
        currentMouseLook.x *= mouseLookSensitivity.x;
        currentMouseLook.y *= mouseLookSensitivity.y;

        currentMouseLook.y *= -1;

    }
}
