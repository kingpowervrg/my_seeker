using UnityEngine;
using System.Collections;

public class UIWHAdaptive : MonoBehaviour
{
    public RectTransform obj;
    private RectTransform myobject;
	// Use this for initialization
	void Start ()
	{
        myobject = gameObject.GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void LateUpdate () {
	    if (obj != null&& myobject!=null)
	    {
	        myobject.sizeDelta = obj.sizeDelta;
	    }
	}
}
