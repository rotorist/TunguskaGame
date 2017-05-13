using UnityEngine;
using System.Collections;

public class HumanAnimStateDeath : HumanAnimStateBase
{
	private Vector3 _ccVelocity;

	// This constructor will create new state taking values from old state
	public HumanAnimStateDeath(HumanAnimStateBase state)     
		:this(state.ParentCharacter)
	{

	}

	// this constructor will be used by the other one
	public HumanAnimStateDeath(HumanCharacter parentCharacter)
	{
		this.ParentCharacter = parentCharacter;

		Initialize();
	}

	public override void SendCommand (CharacterCommands command)
	{


	}



	public override void Update ()
	{
		if(this.ParentCharacter.MyCC != null)
		{
			_ccVelocity = Vector3.Lerp(_ccVelocity, Vector3.zero, Time.deltaTime * 6);
			this.ParentCharacter.MyCC.SimpleMove(_ccVelocity);
		}

	}

	public override void FixedUpdate ()
	{
		
	}

	public override bool IsRotatingBody ()
	{
		return false;
	}



	private void Initialize()
	{
		Debug.Log("Initializing Stand Idle");
		this.ParentCharacter.Destination = this.ParentCharacter.transform.position;

		if(this.ParentCharacter.MyNavAgent != null)
		{
			this.ParentCharacter.MyNavAgent.acceleration = 3;
			this.ParentCharacter.MyNavAgent.Stop();
			this.ParentCharacter.MyNavAgent.ResetPath();
			this.ParentCharacter.MyNavAgent.updateRotation = false;
		}
		else if(this.ParentCharacter.MyCC != null)
		{
			_ccVelocity = this.ParentCharacter.MyCC.velocity;
		}
	}

	private void UpdateState(HumanBodyStates state)
	{


	}
}
