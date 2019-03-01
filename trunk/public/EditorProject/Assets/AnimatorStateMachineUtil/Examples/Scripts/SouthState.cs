using UnityEngine;
using System.Collections;
using AnimatorStateMachineUtil;

public class SouthState : MonoBehaviour 
{
	public float speed;
    
	[StateEnterMethod("Base.SouthState")]
	public void SouthEnter()
	{
		SendMessage("SetLabel","South");
	}

	[StateUpdateMethod("Base.SouthState")]
	public void SouthUpdate()
	{
		transform.position += speed * Vector3.back * Time.deltaTime;
		
	}
}
