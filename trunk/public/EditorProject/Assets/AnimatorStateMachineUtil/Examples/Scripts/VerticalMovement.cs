using UnityEngine;
using System.Collections;
using AnimatorStateMachineUtil;

public class VerticalMovement : MonoBehaviour 
{
	private Direction dir = Direction.Up;
	private Animator animator;
	
	private enum Direction {
		Up,
		Down
	}

	void Start () 
	{
		animator = GetComponent<Animator>();
	}
	
	[StateEnterMethod("Base.UpState0")]
	[StateEnterMethod("Base.UpState1")]
	public void UpStateEnter () 
	{
		dir = Direction.Down;
		SendMessage("SetLabel","Up");
	}
	
	[StateEnterMethod("Base.DownState0")]
	[StateEnterMethod("Base.DownState1")]
	public void DownStateEnter()
	{
		dir = Direction.Up;
		SendMessage("SetLabel","Down");
	}
	
	[StateUpdateMethod("Base.UpState0")]
	[StateUpdateMethod("Base.UpState1")]
	public void UpStateUpdate () 
	{
		Debug.Log("UpStateUpdate");
		transform.Translate( Vector3.up * Time.deltaTime );
	}
	
	[StateUpdateMethod("Base.DownState0")]
	[StateUpdateMethod("Base.DownState1")]
	public void DownStateUpdate()
	{
		Debug.Log("DownStateUpdate");
		transform.Translate( Vector3.down * Time.deltaTime );
	}
	
	void OnMouseDown()
	{		
		string trigger = dir==Direction.Up ? "Up":"Down";
		Debug.Log (trigger);
		animator.SetTrigger(trigger);
	}
}
