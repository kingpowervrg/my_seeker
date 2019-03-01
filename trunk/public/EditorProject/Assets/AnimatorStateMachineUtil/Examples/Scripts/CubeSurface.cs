using UnityEngine;
using System.Collections;

public class CubeSurface : MonoBehaviour 
{
	public Color mainColor ;

	private Material material;
    private Renderer targetRenderer;

	void Start () 
	{
        targetRenderer = GetComponent<Renderer>();
		material = targetRenderer.material = targetRenderer.material;
	}
	
	void Update () 
	{
		material.color = mainColor;
	}
}
