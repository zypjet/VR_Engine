using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HighlightingSystem;
using VRTK;

public enum EnterMode
{
    ShowMode,
    TrainMode,
    ExamMode

}
public class HasPartTouch
{
    public string name;
    public bool hastouch;
    public bool rightorwrong;
}
public class ModeController : MonoBehaviour {

    public static ModeController controller;
    public static EnterMode CurrentMode = EnterMode.ShowMode;
    public List<HasPartTouch> hasparttouch=new List<HasPartTouch>();
    [System.Serializable]
    public class ProduceInfo
    {
        public AudioClip clip;
        public string AudioMess;
        public string AccordPartName;
        public string Part_Name;
        public string Part_Info;
    }

    public Text CurrentModeTxt;

    public List<ProduceInfo> _produces;
    [HideInInspector]
    public int CurrentTrainIndex=0;//当前抓取零件索引
    [HideInInspector]
    public Transform CurrentShouldGrabPart;

    [Header("目标位置父物体")]
    public Transform ReferEngine;
    [Header("发动机零件父物体")]
    public Transform ParentOfParts;
    [HideInInspector]
    public List<Transform> _Parts=new List<Transform>();
    [HideInInspector]
    public List<Transform> PartTarget = new List<Transform>();

    [Header("当前显示应拿取的文本Text")]
    public Transform StartTrainPanel;
    public Transform TrainingPanel;

    [Header("旋转盘")]
    public Transform RotateWheel;
    public Transform RelateObject;
    public GameObject partsPut;
    public GameObject desk;
    [HideInInspector]
    public AudioSource _audioSource;
    public Transform[] modePanels;
    public float moveDelay = 0.3f;

    private PanelController uiController;
    private List<Text> Alltexts = new List<Text>();
    private Transform partbox;
    [HideInInspector]
    public List<string> ErroInfo = new List<string>();
    private float Score = 100;
    [HideInInspector]
    public bool isTraining = false;
    [HideInInspector]
    public bool isExaming = false;

    public void ExitGameB()
    {
        Application.Quit();
    }

    public void ResetRightOrWrongParts()
    {
        for (int i = 0; i < PartTarget.Count; i++)
        {
            HasPartTouch s = new HasPartTouch();
            s.name = PartTarget[i].gameObject.name;
            s.hastouch = false;
            s.rightorwrong = false;
            hasparttouch.Add(s);
        }
    }
    public void SetRightOrWrong(int index,bool hastouch,bool rightorwrong)
    {
        hasparttouch[index].hastouch = hastouch;
        hasparttouch[index].rightorwrong = rightorwrong;
    }

    private void Awake()
    {
        controller = this;
    }

    void Start()
    {
        partsPut.SetActive(false);
        _audioSource = gameObject.AddComponent<AudioSource>();
        foreach (Transform trans in ParentOfParts)
        {
            _Parts.Add(trans);
        }
        foreach (Transform trans in ReferEngine)
        {
            PartTarget.Add(trans);
        }
        uiController = PanelController.controller;
        BeginAddListener();
        ModePanelSet();
        ResetRightOrWrongParts();//初始化零件的对错结构体
        //StartCoroutine("VRStart");
    }

    //IEnumerator VRStart()
    //{
    //    yield return new WaitForSeconds(1f);
    //    Debug.Log("Hello");
    //    KBEngineApp.getSingleton().player().baseCall("VRStartSet",(byte)1);
    //}

    private void BeginAddListener()
    {
        uiController.ButtonAddListener("ShowModeBtn", () => 
        {           
            ModeShift(EnterMode.ShowMode);
        });
        uiController.ButtonAddListener("TrainingModeBtn", () =>
        {          
            ModeShift(EnterMode.TrainMode);
        });
        uiController.ButtonAddListener("ExamingModeBtn", () =>
        {         
            ModeShift(EnterMode.ExamMode);
        });
        uiController.ButtonAddListener("BreakoutBtn", () =>
        {           
            BreakOrCombine(true);
        });
        uiController.ButtonAddListener("CombineBtn",() =>
        {          
            BreakOrCombine(false);
        });
        uiController.ButtonAddListener("StartTrainBtn", () =>
        {         
            StartTrain();
        });
        uiController.ButtonAddListener("RestartTrainBtn",() =>
        {     
            OnEnterTrainMode();
        });
        uiController.ButtonAddListener("StartExamBtn", () =>
        {        
            StartExam();
        });
        uiController.ButtonAddListener("ExitExamBtn",() => 
        {
            EndExam();
        });
        uiController.ButtonAddListener("ReturnStart",() =>
        {
            ExamCancel();
        });
    }

    public void ModeShift(EnterMode mode)//模式转换
    {
        AllColliderSet(true);
        switch (mode)
        {
            case EnterMode.ShowMode:
                OnEnterShowMode();
                break;
            case EnterMode.TrainMode:
                OnEnterTrainMode();
                break;
            case EnterMode.ExamMode:
                OnEnterExamMode();
                break;
        }
        ModePanelSet();
    }

    //展示模式
    public void OnEnterShowMode()
    {
        desk.SetActive(true);
        partsPut.SetActive(false);
        CurrentMode = EnterMode.ShowMode;
        GoZero();
    }

    //实训模式
    public void OnEnterTrainMode()
    {
        isTraining = false;
        desk.SetActive(true);
        partsPut.SetActive(false);
        CurrentMode = EnterMode.TrainMode;
        PanelScaleSet(uiController.FindUIByName("StartTrain").transform, true);
        PanelScaleSet(uiController.FindUIByName("TrainingPanel").transform, false);
        GoZero();
    }

    public void StartTrain()//按下开始按钮正式开始实训
    {
        AllPartPosGoStart(ParentOfParts);
        AllColliderSet(false);
        isTraining = true;
        _audioSource.Stop();
        CurrentTrainIndex = 0;
        CurrentShouldGrabPart = _Parts[CurrentTrainIndex];
        CurrentShouldGrabPart.GetComponent<EnginePart>().HighLightSet(true);
        PanelScaleSet(uiController.FindUIByName("StartTrain").transform, false);
        PanelScaleSet(uiController.FindUIByName("TrainingPanel").transform, true);
        //播放开始语音   
        NextModel();
    }

    public void NextModel()
    {
        _audioSource.clip = _produces[CurrentTrainIndex].clip;
        _audioSource.Play();
        CurrentShouldGrabPart = _Parts[CurrentTrainIndex];
        CurrentShouldGrabPart.GetComponent<Collider>().enabled = true;
        CurrentShouldGrabPart.GetComponent<EnginePart>().HighLightSet(true);
        for (int i = 0; i < _produces.Count; i++)//更新显示
        {
            if (CurrentShouldGrabPart.name == _produces[i].AccordPartName)
            {
                Text intr = uiController.FindUIByName("introoperationtext").GetComponent<Text>();
                intr.text = _produces[i].AudioMess;
                break;
            }
        }
    }




    public Text TipsTitle;//biaoti
    public void ExamNextModel()
    {
        _audioSource.clip = _produces[CurrentTrainIndex].clip;
        _audioSource.Play();
        CurrentShouldGrabPart = _Parts[CurrentTrainIndex];
        CurrentShouldGrabPart.GetComponent<Collider>().enabled = true;
        for (int i = 0; i < _produces.Count; i++)//更新显示
        {
            if (CurrentShouldGrabPart.name == _produces[i].AccordPartName)
            {
                
                TipsTitle.text = _produces[i].AudioMess;
                break;
            }
        }
    }

    //考核模式
    public void OnEnterExamMode()
    {
        isExaming = false;
       // desk.SetActive(false);
        CurrentTrainIndex = 0;
        CurrentShouldGrabPart = _Parts[CurrentTrainIndex];
        AllHighLightSet(false);
        //partsPut.SetActive(true);
        PanelScaleSet(uiController.FindUIByName("StartExamPlane").transform, true);
        PanelScaleSet(uiController.FindUIByName("ExamPanel").transform, false);
        PanelScaleSet(uiController.FindUIByName("ExamScore").transform, false);
        CurrentMode = EnterMode.ExamMode;
        GoZero();
        ExamNextModel();
        
    }

    public void SetTextInfo()//设置放置区名字
    {
        Transform pc = partsPut.transform.Find("PutPlaceCanvas");
        partbox = partsPut.transform.Find("PutPlace");
        for (int i = 0; i < pc.childCount; i++)
        {
            Alltexts.Add(pc.GetChild(i).Find("Text").GetComponent<Text>());
            Alltexts[i].text = i.ToString();
        }
    }

    public void StartExam()//开始考核
    {
        AllPartPosGoStart(ParentOfParts);
        isExaming = true;
       // SetTextInfo();
        PanelScaleSet(uiController.FindUIByName("StartExamPlane").transform, false);
        PanelScaleSet(uiController.FindUIByName("ExamPanel").transform, true);
        PanelScaleSet(uiController.FindUIByName("ExamScore").transform, false);
    }

    public void EndExam()//结束考核
    {
        
        isExaming = false;
        AllPartPosGoStart(ParentOfParts);
        for (int i = 0; i < hasparttouch.Count; i++)
        {
           
            if (hasparttouch[i].rightorwrong)
            {
                Debug.Log("操作正确");
            }
            else
            {
                Score -= 3.2f;
                for (int j = 0; j <hasparttouch.Count; j++)
                {
                    if (_produces[i].AccordPartName == hasparttouch[i].name)
                    {
                        ErroInfo.Add("操作错误：" + _produces[i].Part_Name+"\n");
                        break;
                    }
                }
            }
        }
        for (int i = 0; i < ErroInfo.Count; i++)
        {
            Text ExamInfoText = uiController.FindUIByName("ExamInfo").GetComponent<Text>();
            ExamInfoText.text += ErroInfo[i];
        }
        if(Score<=1)
        {
            Score = 0;
        }
        int _score = (int)Score;
        Text ScoreText = uiController.FindUIByName("ScoreText").GetComponent<Text>();
        ScoreText.text = "成绩：" + Score;
        PanelScaleSet(uiController.FindUIByName("ExamPanel").transform, false);
        PanelScaleSet(uiController.FindUIByName("ExamScore").transform, true);
    }

    public void ExamCancel()//取消考核
    {
        isExaming = false;
        PanelScaleSet(uiController.FindUIByName("StartExamPlane").transform, true);
        PanelScaleSet(uiController.FindUIByName("ExamPanel").transform, false);
        PanelScaleSet(uiController.FindUIByName("ExamScore").transform, false);
        Score = 100;
        GoZero();
    }

    public void BreakOrCombine(bool isBreak)
    {
        if(isBreak)
            AllPartGoTarget(RelateObject);
        else
            AllPartPosGoStart(ParentOfParts);
    }

    //通用模块
    public EnginePart GetPart(string name)
    {
        foreach(Transform trans in _Parts)
        {
            if(trans.name == name)
            {
                return trans.GetComponent<EnginePart>();
            }
        }
        return null;
    }

    private void GoZero()//恢复
    {
        _audioSource.clip = Resources.Load("音频/进入各模式的语音播报内容/" + CurrentMode.ToString(), typeof(AudioClip)) as AudioClip;
        _audioSource.Play();
        AllPartPosGoStart(ParentOfParts);
        AllHighLightSet(false);
        AllRigClose();
    }

    private void ModePanelSet()
    {
        foreach(Transform mode in modePanels)
        {
            if (mode.name == CurrentMode.ToString())
                PanelScaleSet(mode, true);
            else
                PanelScaleSet(mode, false);
        }
    }

    private void PanelScaleSet(Transform trans,bool isScale)
    {
        if (isScale)
            trans.DOScale(Vector3.one, moveDelay);
        else
            trans.DOScale(Vector3.zero, moveDelay);
    }

    private void AllColliderSet(bool isSet)
    {
        foreach(Transform trans in _Parts)
        {
            Collider part = trans.GetComponent<Collider>();
            if (part != null)
            {
                part.enabled = isSet;
            }
            else
                Debug.Log("EnginePart is Not Found");
        }
    }

    private void AllRigClose()
    {
        foreach(Transform trans in _Parts)
        {
            Rigidbody part = trans.GetComponent<Rigidbody>();
            if (part != null)
            {
                Destroy(part);
            }
            else
                Debug.Log("EnginePart is Not Found");
        }
    }
    
    private void AllHighLightSet(bool isActive)
    {
        foreach(Transform trans in _Parts)
        {
            EnginePart part = trans.GetComponent<EnginePart>();
            if (part != null)
            {
                part.HighLightSet(isActive);
            }
            else
                Debug.Log("EnginePart is Not Found");
        }
    }

    private void AllPartGoTarget(Transform parent)
    {
        for(int i=0;i<PartTarget.Count;i++)
        {
            EnginePart part = _Parts[i].GetComponent<EnginePart>();
            if (part != null)
            {
                part.transform.parent = parent;
                part.MoveTo(PartTarget[i]);
            }
            else
                Debug.Log("EnginePart is Not Found");
        }
    }
    private void AllPartPosGoStart(Transform parent)
    {
        StartCoroutine("GoStart",parent);
    }

    IEnumerator GoStart(Transform parent)
    {
        RotateWheel.GetComponent<ControllRotateWheel>().ResetWheel();
        yield return new WaitForSeconds(0.3f);
        foreach (Transform trans in _Parts)
        {
            EnginePart part = trans.GetComponent<EnginePart>();
            if (part != null)
            {
                trans.parent = parent;
                part.MoveToBegin();
            }
            else
                Debug.Log("EnginePart is Not Found");
        }    
    }

}
