using UnityEngine;
using System.Collections;
using AnimatorStateMachineUtil;

public class EastState : MonoBehaviour 
{

	public float speed;

	[StateEnterMethod("Base.EastState")]
	public void EastEnter()
	{
		SendMessage("SetLabel","East");
	}
	
	[StateUpdateMethod("Base.EastState")]
	public void EastUpdate()
	{
		transform.position += speed * Vector3.right * Time.deltaTime;
		
	}
    
}
