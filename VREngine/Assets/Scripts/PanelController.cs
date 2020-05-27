using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PanelController : MonoBehaviour {

    public static PanelController controller;
    private GameObject[] UI;


    private void Awake()
    {
        controller = this;
        UI = GameObject.FindGameObjectsWithTag("UI");
        foreach (GameObject ui in UI)
        {
            Debug.Log(ui.name);
        }
    }

    public GameObject FindUIByName(string name)
    {
        foreach(GameObject obj in UI)
        {
            if (obj.name == name)
                return obj;
        }
        Debug.Log("UI Not Found");
        return null;
    }

    public void ButtonAddListener(string name,UnityAction action)
    {
        GameObject uiObj = FindUIByName(name);
        Debug.Log("Button "+uiObj.name);
        uiObj.GetComponent<Button>().onClick.AddListener(action);
    }
}
