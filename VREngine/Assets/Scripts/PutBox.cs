using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
     
	}
	
	// Update is called once per frame
	void Update () {
        //ExplosionDamage(transform.position, 0.5f);
    }
   public bool CountScore()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].GetComponent<EnginePart>() != null)
            {
                if (hitColliders[i].name == gameObject.name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }
}
