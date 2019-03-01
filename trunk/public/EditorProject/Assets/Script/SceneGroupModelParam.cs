using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGroupModelParam : MonoBehaviour {

    private SceneModelParam[] modelParam;
    public float minY, maxY;
    public bool isStart = false;
	// Use this for initialization
	void Start () {
        modelParam = GetComponentsInChildren<SceneModelParam>();
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
