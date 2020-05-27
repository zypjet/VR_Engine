using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using DG.Tweening;

public class ControllRotateWheel : MonoBehaviour {

	public Transform RotateWheel;
	public Transform Engine;


	void Start () {
	}

	// Update is called once per frame
	void Update () {
		EngineRotateWithWheel ();
	}

    public void ResetWheel()
    {
        transform.DORotate(new Vector3(-90,0,0),0.3f);
    }

    private void EngineRotateWithWheel()
	{
		float wheel_X = RotateWheel.eulerAngles.x;
		Quaternion q = Quaternion.Euler (wheel_X, Engine.eulerAngles.y, Engine.eulerAngles.z);
		Engine.transform.rotation=q;
	}
}
