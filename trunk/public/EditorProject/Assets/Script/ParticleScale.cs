using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScale : MonoBehaviour {

    public Vector3 scale = Vector3.zero;
	// Use this for initialization
	void Start () {

        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform tran = transform.GetChild(i);
            tran.localScale = new Vector3(tran.localScale.x * scale.x, tran.localScale.y * scale.y, tran.localScale.z * scale.z);
        }
	}
	
}
