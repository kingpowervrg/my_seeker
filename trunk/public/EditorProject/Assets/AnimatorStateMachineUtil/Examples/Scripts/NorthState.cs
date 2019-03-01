using UnityEngine;
using System.Collections;
using AnimatorStateMachineUtil;

public class NorthState : MonoBehaviour 
{

	public float speed;
    
	[StateEnterMethod("Base.NorthState")]
	public void NorthEnter()
	{
		SendMessage("SetLabel","North");
	}

	[StateUpdateMethod("Base.NorthState")]
	public void NorthUpdate()
	{
		transform.position += speed * Vector3.forward * Time.deltaTime;
	}
}
