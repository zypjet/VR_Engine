using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShowModeControl : MonoBehaviour {
    public struct PartsInfo
    {
        public Vector3 Pos;
        public Quaternion rotate;
        public GameObject _gameobject;
        public Vector3 euler;
    }
    //public ProjectManager _ProjectManager;
    public Text CurrentModeTxt;

    [Header("目标位置父物体")]
    public Transform ReferEngine;
    [Header("发动机零件父物体")]
    public Transform ParentOfParts;
    [HideInInspector]
    public List<PartsInfo> PartStart = new List<PartsInfo>();
    [HideInInspector]
    public List<PartsInfo> PartTarget = new List<PartsInfo>();
    [HideInInspector]
    public List<Transform> Allparts=new List<Transform>();//所有的零件


    public Transform RelateObject;//存放子物体的父物体
    [HideInInspector]
    public List<PartsInfo> RelatePosAndRoa=new List<PartsInfo>();//相对坐标和角度


   

    [HideInInspector]
    public int BreakOrConmbine=1;//0代表已拆解，1代表已集合，默认是1
	void Start () {
     
       StartCoroutine(GetStart());//
	}
	
	
	void Update () {
        UpdateMode();
        //print(BreakOrConmbine);
	}
    IEnumerator GetStart()
    {
        SetParts();
        UpdateMode();
        yield return new WaitForSeconds(0.1f);
        //StartCoroutine(GetRelatePos());

    }
    IEnumerator GetRelatePos()
    {
        RelateObject.SetParent(ParentOfParts);
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < Allparts.Count; i++)
        {
            Vector3 pos = Allparts[i].position - RelateObject.transform.position;
            Quaternion roa =Quaternion.Euler(Allparts[i].eulerAngles - RelateObject.transform.eulerAngles);
            Vector3 eu = Allparts[i].eulerAngles - RelateObject.transform.eulerAngles;
            PartsInfo p = new PartsInfo();
            p.Pos = pos;
            p.rotate = roa;
            p.euler = eu;
            RelatePosAndRoa.Add(p);
            
        }
        print("GETRELATE:" + RelatePosAndRoa.Count);
    }
    void UpdateMode()
    {
        //if (_ProjectManager.CurrentMode == EnterMode.ShowMode)
        //{
        //    CurrentModeTxt.text = "当前模式为展示模式";
        //    transform.DOScale(new Vector3(1, 1, 1), 0.2f);
        //}
        //else
        //{
        //    transform.DOScale(Vector3.zero, 0.2f);
        //}
    }
    void SetParts()
    {
        for (int i = 0; i <ParentOfParts.childCount ; i++)//获取初始零件的位置和旋转
        {
            Allparts.Add(ParentOfParts.GetChild(i));
            PartsInfo pi = new PartsInfo();
            pi.Pos = ParentOfParts.GetChild(i).position;
            pi.rotate = ParentOfParts.GetChild(i).rotation;
            pi._gameobject = ParentOfParts.GetChild(i).gameObject;
            PartStart.Add(pi);
        }
        for (int i = 0; i < ReferEngine.childCount; i++)//获取目标的位置和旋转
        {
            PartsInfo pi = new PartsInfo();
            pi.Pos = ReferEngine.GetChild(i).position;
            pi.rotate = ReferEngine.GetChild(i).rotation;
            pi._gameobject = ReferEngine.GetChild(i).gameObject;
           PartTarget.Add(pi);
            //print(PartTargetPositions[i]);
        }

        //print("Allparts:" + Allparts.Count + "____" + "PartTarget:" + PartTarget.Count);
    }
    public void OneKeyToBreakUpBtn()//一键拆解
    {
       
        for (int i = 0; i < PartTarget.Count; i++)
        {
            Allparts[i].DOMove(PartTarget[i].Pos, 1.0f);
            Allparts[i].DORotateQuaternion(PartTarget[i].rotate, 1.0f);
            Allparts[i].SetParent(RelateObject);
        }
        BreakOrConmbine = 0;
    }
    [Header("旋转盘")]
    public Transform RotateWheel;
    public void OnKeyToCombineBtn()//一键归位
    {
        for (int i = 0; i < PartStart.Count; i++)
        {
            Allparts[i].SetParent(ParentOfParts);
            Allparts[i].DOMove(PartStart[i].Pos, 1.0f);
            Allparts[i].DORotateQuaternion(PartStart[i].rotate, 1.0f);

        }
        Quaternion q = Quaternion.Euler(-90, 0, 0);
        RotateWheel.rotation = q;
        BreakOrConmbine = 1;
    }
    public void SetAudioAndTextInfo()//设置语音和文字信息，在展示模式下，拿起零件，会更新语音和文本信息
    {

    }
   
}
