using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public  Texture[] TV;
    //public Texture TV_null;
    //[Range(0,5)]
    private int _tv = 5;

    public RawImage UI_Image;
    public Text[] text;

    

    
    private void Start()
    {
        //初始化
       

    }
    private void Update()
    {
        //ChangeTV(_tv);
    }

    public void ChangeTV(int a)
    {
        if (text[0].text == "取下活塞及曲轴")
        {
            UI_Image.texture = TV[0];
        }
        else if (text[0].text == "拆汽缸盖")
        {
            UI_Image.texture = TV[1];
        }
        else if (text[1].text == "喷油点火")
        {
            UI_Image.texture = TV[2];
        }
        else if (text[1].text == "活塞连杆运动")
        {
            UI_Image.texture = TV[3];
        }
        else if (text[1].text == "进气排气")
        {
            UI_Image.texture = TV[4];
        }else {
            UI_Image.texture = TV[a];
        }

        
        
    }

    
    

}
