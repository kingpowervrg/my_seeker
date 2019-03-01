using UnityEngine;
using System.Collections;
using AnimatorStateMachineUtil;

public class WestState : MonoBehaviour 
{
	public float speed;

	[StateEnterMethod("Base.WestState")]
	public void WestEnter()
	{
		SendMessage("SetLabel","West");
	}
	

	[StateUpdateMethod("Base.WestState")]
	public void WestUpdate()
	{
		transform.position += speed * Vector3.left * Time.deltaTime;
		
	}
}
