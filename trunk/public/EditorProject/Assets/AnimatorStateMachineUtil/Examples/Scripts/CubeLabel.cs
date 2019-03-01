using UnityEngine;
using System.Collections;

public class CubeLabel : MonoBehaviour 
{
	public TextMesh textMesh;

	void Start () 
	{
	
	}
	
	void SetLabel(string label) 
	{
		textMesh.text = label;
	}
}
