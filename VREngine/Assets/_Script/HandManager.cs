using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;


public class HandManager : MonoBehaviour
{
    public GameObject[] UI;

    //[Range(0,4)]
    //public int _tv;

    private VRTK_ControllerEvents handController;
    public GameObject HandL;

    private bool canPressMeun;
    private VRTK_ControllerReference controllerReference;

    public Texture TV;

    private int Tv_count=5;
    private void Awake()
    {
        canPressMeun = false;
        Invoke("ResetPress", 1f);
        DynamicGI.UpdateEnvironment();

        //初始化
        UI[0].SetActive(false);//总菜单
        UI[1].SetActive(false);//展示界面
        UI[2].SetActive(false);//考核界面
        UI[3].SetActive(false);//实训界面
        UI[4].SetActive(false);//视频界面界面
        
        //TV = this.gameObject.GetComponent<UIController>().TV;

    }
    

    private void Update()
    {
        //检测手柄
        GameObject lefttHand = VRTK_DeviceFinder.GetControllerLeftHand(true);
        controllerReference = VRTK_ControllerReference.GetControllerReference(lefttHand);

        
        if (IsMeun() || Input.GetKeyDown(KeyCode.Escape))   //手柄menu键或键盘esc,打开菜单栏
        {
            if (canPressMeun == false)
            {
                UI[0].SetActive(true);
                canPressMeun = true;

                HandL.GetComponent<VRTK_Pointer>().enabled = false;
                OffTV();
            }else{
                UI[0].SetActive(false);
                canPressMeun = false;

                HandL.GetComponent<VRTK_Pointer>().enabled = true;
                OffTV();
            }
        }


        if (IsTriggerAndTouchpad() || Input.GetKeyDown(KeyCode.DownArrow))  //手柄扳机加圆盘或向下按
        {
            //播放视频
            //TV = Resources.Load("Assets/Resources/_Texture/TV " +_tv) as Texture;
            if (Tv_count < 5)
                Tv_count++;
            else
                Tv_count = 0;

            Debug.Log(Tv_count);
            //Tv_count=this.gameObject.GetComponent<UIController>()._tv;
            switch (Tv_count)
            {
                case 0: gameObject.GetComponent<UIController>().ChangeTV(0); break;
                case 1: gameObject.GetComponent<UIController>().ChangeTV(1); break;
                case 2: gameObject.GetComponent<UIController>().ChangeTV(2); break;
                case 3: gameObject.GetComponent<UIController>().ChangeTV(3); break;
                case 4: gameObject.GetComponent<UIController>().ChangeTV(4); break;
                default: gameObject.GetComponent<UIController>().ChangeTV(5); break;

            }
        }
            
        

        //测试按键
        if (Input.GetKeyDown(KeyCode.P))
        {
            //打开视频
            UI[4].SetActive(true);
        }
    }


    public bool IsTriggerAndTouchpad()//同时按下扳机和原配
    {
        if (!VRTK_ControllerReference.IsValid(controllerReference))
        {
            return false;
        }
        if ( VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.Press, controllerReference) 
            && VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
        {
            return true;
        }
        return false;
    }
    public bool IsMeun()//是否按下meun菜单按键
    {
        if (!VRTK_ControllerReference.IsValid(controllerReference))
        {
            return false;
        }
        if ( VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference) )
        {
            return true;
        }
        return false;
    }




    public void OffTV()
    {
        UI[4].SetActive(false);//关闭视频模块
    }
    
    public void OffShow()//关闭展示UI
    {
        UI[1].SetActive(false);//展示界面
        
    }
    public void OffTrain()//关闭实训UI
    {
        
        UI[3].SetActive(false);//实训界面
        
    }
    public void OffExam()//关闭考核UI
    {
        UI[2].SetActive(false);//考核界面
    }
    public void ONShow()//打开展示UI
    {
        UI[1].SetActive(true);//展示界面
    }
    public void ONTrain()//打开实训UI
    {
        UI[3].SetActive(true);//实训界面
    }
    public void ONExam()//打开考核UI
    {
        UI[2].SetActive(true);//考核界面
    }
    public void ONTV()
    {
        UI[4].SetActive(true);//开启视频模块
    }

}
