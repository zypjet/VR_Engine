using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ExamModeControl : MonoBehaviour {
    [System.Serializable]
    public struct ExamPart
    {
        public Transform part;
        public Vector3 Pos;//位置
        public Quaternion qua;//角度
    }

    //public ProjectManager _ProjectManager;
    public Text CurrentModeTxt;
    private ModeController _trainmodeControl;

    [HideInInspector]
    public List<ExamPart> _ExamStartparts = new List<ExamPart>();//初始零件的所有信息
    [HideInInspector]
    public List<Text> Alltexts;
    public Transform ParentEngine;//所有零件的父类

    public GameObject Desks;
    public GameObject PartsPut;
    private Transform pc;//partcanvas,画布ui的父对象
    private Transform partbox;

    public bool IsExaming = false;//是否正在答题

    public float TotalTime=30;//总的考核时间
    public float CurrentTime=30;//当前时间
    void Start()
    {
        //_trainmodeControl = GameObject.FindGameObjectWithTag("TrainModeControl").GetComponent<TrainModeControl>();
        UpdateMode();
        GetStart();
    }

    void Update()
    {
        UpdateMode();
    }
    void UpdateTimeInfo()
    {
        if (IsExaming)
        {
            CurrentTime -= Time.deltaTime;
            TimeInfoText.text = "还剩余" + CurrentTime + "分钟";
            if (CurrentTime <= 0)
            {
                IsExaming = false;
            }
        }
    }
    void UpdateMode()
    {
        UpdateTimeInfo();
        //if (_ProjectManager.CurrentMode == EnterMode.ExamMode)
        //{
        //    CurrentModeTxt.text = "当前模式为考核模式";
        //    transform.DOScale(new Vector3(1, 1, 1),0.2f);
        //    PartsPut.SetActive(true);
        //    Desks.SetActive(false);
        //}
        //else
        //{
        //    transform.DOScale(Vector3.zero,0.2f) ;
        //    PartsPut.SetActive(false);
        //    Desks.SetActive(true);
        //}
    }

    public void GetStart()
    {
        GetStartPartInfo();
        SetTextInfo();
    }

    public void GetStartPartInfo()//获取初始零件的所有信息
    {
        for (int i = 0; i < ParentEngine.childCount; i++)
        {
            Transform t = ParentEngine.GetChild(i);
            Vector3 pos = ParentEngine.GetChild(i).position;
            Quaternion q = ParentEngine.GetChild(i).rotation;
            ExamPart e = new ExamPart();
            e.Pos = pos;
            e.qua = q;
            e.part = t;
            _ExamStartparts.Add(e);
        }
        ExamPanel.DOScale(Vector3.zero, 0.2f);
        ExamScorePanel.DOScale(Vector3.zero, 0.2f);
    }
    public void SetTextInfo()//设置放置区名字
    {
        pc = PartsPut.transform.Find("PutPlaceCanvas");
        partbox= PartsPut.transform.Find("PutPlace");

        for (int i = 0; i < pc.childCount; i++)
        {
            Alltexts.Add(pc.GetChild(i).Find("Text").GetComponent<Text>());
            //Alltexts[i].text = _trainmodeControl._produces[i].Part_Name;
            Alltexts[i].text =i.ToString();
        }
    }
    public void ResetPartPosition()//重置零件的位置
    {
        for (int i = 0; i <_ExamStartparts.Count ; i++)
        {
            Rigidbody r = _ExamStartparts[i].part.GetComponent<Rigidbody>();
            if (r != null)
            {
                Destroy(r);
            }
            _ExamStartparts[i].part.DOMove(_ExamStartparts[i].Pos, 0.2f);
            _ExamStartparts[i].part.DORotateQuaternion(_ExamStartparts[i].qua, 0.2f);
        }
    }

    public Transform StartPanel;
    public Transform ExamPanel;
    public Transform ExamScorePanel;
    public Text TimeInfoText;
    public Text ExamInfoText;
    public Text ScoreText;

    //public void StartExamBtn()//开始考核按钮
    //{
    //    for (int i = 0; i < _trainmodeControl.PartStart.Count; i++)
    //    {
    //        Rigidbody r = _trainmodeControl._Parts[i].GetComponent<Rigidbody>();
    //        if (r != null)
    //        {
    //            Destroy(r);
    //        }
    //        _trainmodeControl._Parts[i].SetParent(ParentEngine);
    //        _trainmodeControl._Parts[i].DOMove(_trainmodeControl.PartStart[i].Pos, 1.0f);
    //        _trainmodeControl._Parts[i].DORotateQuaternion(_trainmodeControl.PartStart[i].rotate, 1.0f);
    //    }
    //    StartPanel.DOScale(Vector3.zero, 0.1f);
    //    ExamPanel.DOScale(new Vector3(1, 1, 1), 0.1f);
    //    ExamScorePanel.DOScale(Vector3.zero, 0.1f);
    //    IsExaming = true;
    //}

    [HideInInspector]
    public List<string> ErroInfo=new List<string>();

    private int Score=100;
    public void EndExamBtn()//结束答题，计算分数
    {
        for (int i = 0; i < partbox.childCount; i++)
        {
            PutBox p = partbox.GetChild(i).GetComponent<PutBox>();
            if (p.CountScore() == true)
            {
               
                print("操作正确");
            }
            else
            {
                Score -= 3;
                for (int j = 0; j < _trainmodeControl._produces.Count; j++)
                {
                    if (_trainmodeControl._produces[i].AccordPartName == p.gameObject.name)
                    {
                        ErroInfo.Add("操作错误：" + _trainmodeControl._produces[i].Part_Name);
                        break;
                    }
                }
                
            }
        }
        for (int i = 0; i < ErroInfo.Count; i++)
        {
            ExamInfoText.text += ErroInfo[i];
        }
        ScoreText.text = "成绩：" + Score;
        ExamPanel.DOScale(Vector3.zero, 0.1f);
        ExamScorePanel.DOScale(new Vector3(1, 1, 1), 0.1f);
    }
    public void ReturnBtn()
    {
        StartPanel.DOScale(new Vector3(1, 1, 1), 0.1f);
        ExamPanel.DOScale(Vector3.zero, 0.1f);
        ExamScorePanel.DOScale(Vector3.zero, 0.1f);
        IsExaming = false;
        Score = 100;
        ResetPartPosition();
    }
   
}
