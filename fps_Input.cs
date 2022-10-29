using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_Input : MonoBehaviour
{
    public class fps_InputAxis          //一个轴里面两个键，正向，负向
    {
        public KeyCode positive;
        public KeyCode negative;
    }
    //按键定义
    public Dictionary<string, KeyCode> buttons = new Dictionary<string, KeyCode>();
    //轴的定义
    public Dictionary<string, fps_InputAxis> axis = new Dictionary<string, fps_InputAxis>();

    public List<string> unityAxis = new List<string>();

    void Start()
    {
        SetupDefaults();
    }
    private void SetupDefaults(string type="")
    {
        if(type=="" || type=="buttons")
        {
            if(buttons.Count==0)
            {
                AddButton("Fire", KeyCode.Mouse0);
                AddButton("Reload", KeyCode.R);
                AddButton("Jump", KeyCode.Space);
                AddButton("Sprint", KeyCode.LeftShift);
                AddButton("Crouch", KeyCode.C);
            }
        }

        if (type == "" || type == "Axis")
        {
            if (axis.Count == 0)
            {
                AddAxis("ForwardBack", KeyCode.W, KeyCode.S);
                AddAxis("LeftRight", KeyCode.A, KeyCode.D);
            }
        }

        if (type == "" || type == "UnityAxis")
        {
            if (unityAxis.Count == 0)
            {
                AddUnityAxis("Mouse X");
                AddUnityAxis("Mouse Y");
                AddUnityAxis("Horizontal");
                AddUnityAxis("Vertical");
            }
        }
    }

    private void AddButton(string n,KeyCode k)
    {
        if (buttons.ContainsKey(n))
            buttons[n] = k;
        else
            buttons.Add(n, k);
    }

    private void AddAxis(string n, KeyCode pk, KeyCode nk)
    {
        if (axis.ContainsKey(n))
            axis[n] = new fps_InputAxis() { positive = pk, negative = nk };
        else
            axis.Add(n, new fps_InputAxis() { positive = pk, negative = nk });
    }

    private void AddUnityAxis(string n)
    {
        if (!unityAxis.Contains(n))
            unityAxis.Add(n);
    }

    public bool GetButton(string button)    //获取按着
    {
        if (buttons.ContainsKey(button))
            return Input.GetKey(buttons[button]);
        return false;
    }

    public bool GetButtonDown(string button)  //获取按下
    {
        if (buttons.ContainsKey(button))
            return Input.GetKeyDown(buttons[button]);
        return false;
    }

    public float GetAxis(string axis)
    {
        if (unityAxis.Contains(axis))
            return Input.GetAxis(axis);
        else
            return 0;
    }

    public float GetAxisRaw(string inAxis)
    {
        if (axis.ContainsKey(inAxis))
        {
            if (Input.GetKey(axis[inAxis].positive))
                return 1;
            else if (Input.GetKey(axis[inAxis].negative))
                return -1;
            return 0;
        }
        else if (unityAxis.Contains(inAxis))
        {
            return Input.GetAxisRaw(inAxis);
        }
        else
            return 0;
    }
}
