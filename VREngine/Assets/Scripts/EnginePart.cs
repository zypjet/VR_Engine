using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;
using DG.Tweening;
using HighlightingSystem;
using UnityEngine.UI;

public class EnginePart : MonoBehaviour {

    private VRTK_InteractableObject_UnityEvents vrtk_interactableobject_events;//零件被抓去的事件
    private float moveDelay;
    private ModeController modeController;
    private PanelController uiController;
    private GameObject beginObj;
    private Transform target;

    private void Awake()
    {
        beginObj = new GameObject();
        beginObj.name = transform.name + "F";
        beginObj.transform.parent = transform.parent;
        beginObj.transform.position = transform.position;
        beginObj.transform.rotation = transform.rotation;
    }

    void Start () {
        moveDelay = 0.8f;
        vrtk_interactableobject_events = GetComponent<VRTK_InteractableObject_UnityEvents>();
        vrtk_interactableobject_events.OnGrab.AddListener((object o, InteractableObjectEventArgs e) => 
        {
            whengrab();
        });
        vrtk_interactableobject_events.OnUngrab.AddListener((object o, InteractableObjectEventArgs e) => 
        {
            whenungrab();
        });
        modeController = ModeController.controller;
        uiController = PanelController.controller;
        for (int i = 0; i < modeController.PartTarget.Count; i++)
        {
            if (gameObject.name == modeController.PartTarget[i].name)
            {
                target = modeController.PartTarget[i];
                break;
            }
        }
    }

    public void whengrab()//当触摸到该零件时
    {
        switch (ModeController.CurrentMode)
        {
            case EnterMode.ShowMode:
                OnGrabShowMode();
                break;
            case EnterMode.TrainMode:
                OnGrabTrainMode();
                break;
            case EnterMode.ExamMode:
                OnGrabExamMode();
                break;
            default:
                break;
        }
    }

    public void whenungrab()//当放开零件时
    {
        switch (ModeController.CurrentMode)
        {
            case EnterMode.ShowMode:
                OnUnGrabShowMode();
                break;
            case EnterMode.TrainMode:
                OnUnGrabTrainMode();
                break;
            case EnterMode.ExamMode:
                OnUnGrabExamMode();
                break;
            default:
                break;
        }
    }

    public void ShowModeMove()
    {
        RemoveRig();
        MoveToBegin();
    }//展示模式中关闭刚体组件，并且回到原位

    private void OnGrabShowMode()//展示模式中触摸到零件
    {
        //播放语音,显示文字
        AudioClip clip = Resources.Load("音频/零件介绍/" + gameObject.name, typeof(AudioClip)) as AudioClip;
        modeController._audioSource.clip = clip;
        modeController._audioSource.Play();
        for (int i = 0; i < modeController._produces.Count; i++)
        {
            if (modeController._produces[i].AccordPartName == gameObject.name)
            {
                uiController.FindUIByName("PartName").GetComponent<Text>().text = modeController._produces[i].Part_Name;
                uiController.FindUIByName("PartInfo").GetComponent<Text>().text = modeController._produces[i].Part_Info;
                break;
            }
        }
    }
    private void OnUnGrabShowMode()//展示模式中触摸到零件
    {
        ShowModeMove();
        modeController._audioSource.Stop();
    }

    private void OnGrabTrainMode()//实训模式中当触摸到零件时
    {
       
    }

    private void OnUnGrabTrainMode()//实训模式中当放开零件时
    {
        RemoveRig();
        if (modeController.isTraining)
        {
            if (gameObject.name == modeController.CurrentShouldGrabPart.name)
            {
                MoveToTarget();
                for (int i = 0; i < modeController.PartTarget.Count; i++)
                {
                    if (gameObject.name == modeController.PartTarget[i].name)
                    {
                        MoveTo(modeController.PartTarget[i]);
                        break;
                    }
                }
                Debug.Log("拿对了");
                if (modeController.CurrentTrainIndex == (modeController._Parts.Count - 1))
                {
                    //实训结束
                    uiController.FindUIByName("introoperationtext").GetComponent<Text>().text = "实训结束，你可以选择重新开始或者选择其他模式";
                }
                else//拿对了零件且拆解没有结束
                {
                    modeController.CurrentTrainIndex += 1;
                    modeController.CurrentShouldGrabPart = modeController._Parts[modeController.CurrentTrainIndex];
                    modeController.NextModel();
                }
                Highlighter h = GetComponent<Highlighter>();
                h.enabled = false;
            }//拿对了零件
            else//拿错了零件
            {
                MoveToBegin();
                Debug.Log("拿错了");
            }
            //如果拿对了零件，零件回到操作台，然后索引+1（此时应判断索引是否超过最大索引，如果即将超过，说明实训结束，此时应给予选项）
            //如果拿错了零件，零件回到发动机本体
        }
        else
        {
            MoveToBegin();
        }
    }

    private void OnGrabExamMode()
    {
       
    }

    //*********************************************************//
    //private void OnUnGrabExamMode()//考核模式
    //{
    //    RemoveRig();
    //    if (modeController.isExaming)
    //    {
    //        transform.parent = ModeController.controller.RelateObject;
    //        Collider c = gameObject.GetComponent<Collider>();
    //        c.isTrigger = false;
    //    }
    //    else
    //    {
    //        MoveToBegin();
    //    }
    //}

    //*********************************************************//
  public bool IsPartBeTouched(string partname)
    {
        for (int i = 0; i < modeController.hasparttouch.Count; i++)
        {
            if(partname==modeController.hasparttouch[i].name&&modeController.hasparttouch[i].hastouch)//如果零件已经被触摸过，就返回true
            {
                return true;
            }
        }
        return false;
    }


    private void OnUnGrabExamMode()//考核模式
    {
        RemoveRig();
        if (modeController.isExaming)//如果已经开始考核
        {   
                bool rightorwrong = false;
                if (gameObject.name == modeController.CurrentShouldGrabPart.name)
                {
                    
                    MoveToTarget();
                    for (int i = 0; i < modeController.PartTarget.Count; i++)
                    {
                        if (gameObject.name == modeController.PartTarget[i].name)
                        {
                            MoveTo(modeController.PartTarget[i]);

                            break;
                        }
                    }
                    Debug.Log("拿对了");
                    rightorwrong = true;
                    if (modeController.CurrentTrainIndex == (modeController._Parts.Count - 1))
                    {
                        modeController.EndExam();
                        //实训结束
                       // uiController.FindUIByName("introoperationtext").GetComponent<Text>().text = "实训结束，你可以选择重新开始或者选择其他模式";
                    }
                    else//拿对了零件且拆解没有结束
                    {
                        modeController.CurrentTrainIndex += 1;
                        modeController.CurrentShouldGrabPart = modeController._Parts[modeController.CurrentTrainIndex];
                        modeController.ExamNextModel();
                    }
                  
                }//拿对了零件
                else//拿错了零件
                {
                    MoveToBegin();//归位
                    Debug.Log("拿错了");
                    rightorwrong = false;
                }
                for (int i = 0; i < modeController.hasparttouch.Count; i++)
                {
                    if (gameObject.name == modeController.hasparttouch[i].name )//如果零件已经被触摸过
                    {
                        modeController.SetRightOrWrong(i, true, rightorwrong);
                    }
                }
         }
            //如果拿对了零件，零件回到操作台，然后索引+1（此时应判断索引是否超过最大索引，如果即将超过，说明实训结束，此时应给予选项）
            //如果拿错了零件，零件回到发动机本体
        
        else
        {
            MoveToBegin();
        }
    }

    public void MoveTo(Transform target)
    {
        transform.DOMove(target.position,moveDelay);
        transform.DORotateQuaternion(target.rotation,moveDelay);
    }

    public void MoveToBegin()
    {
        transform.parent = modeController.ParentOfParts;
        MoveTo(beginObj.transform);
    }

    public void MoveToPlayer(Transform target)
    {
        StartCoroutine("MovePlayer");
    }

    IEnumerator MovePlayer(Transform target)
    {
        while(true)
        {
            MoveTo(target);
            yield return null;
        }
    }

    public void MoveToTarget()
    {
        transform.parent = modeController.RelateObject;  
    }

    public void HighLightSet(bool isActive)
    {
        Highlighter h = GetComponent<Highlighter>();
        if (h == null)
        {
            Debug.Log("Not Found HighLight");
            return;
        }
        h.enabled = isActive;
    }

    public void RemoveRig()
    {
        Rigidbody rig = GetComponent<Rigidbody>();
        if(rig != null)
        {
            Destroy(rig);
        }
    }

}
