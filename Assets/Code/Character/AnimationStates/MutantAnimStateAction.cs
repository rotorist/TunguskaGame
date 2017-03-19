using UnityEngine;
using System.Collections;

public class MutantAnimStateAction : MutantAnimStateBase
{
	private float _vSpeed;

	// This constructor will create new state taking values from old state
	public MutantAnimStateAction(MutantAnimStateBase state)     
		:this(state.ParentCharacter)
	{

	}

	// this constructor will be used by the other one
	public MutantAnimStateAction(MutantCharacter parentCharacter)
	{
		this.ParentCharacter = parentCharacter;

		Initialize();
	}


	public override void SendCommand (CharacterCommands command)
	{
		if(command == CharacterCommands.AnimationActionDone)
		{
			UpdateState(MutantBodyStates.Idle);
		}

	}

	public override void Update ()
	{
		
	}

	public override bool IsRotatingBody ()
	{
		return false;
	}



	private void Initialize()
	{
		Debug.Log("Initializing Action");
		_vSpeed = this.ParentCharacter.MyAnimator.GetFloat("VSpeed");
		this.ParentCharacter.Destination = this.ParentCharacter.transform.position;
		this.ParentCharacter.MyNavAgent.Stop();
		this.ParentCharacter.MyNavAgent.ResetPath();
		this.ParentCharacter.MyNavAgent.updateRotation = false;
		this.ParentCharacter.MyHeadIK.Weight = 0;
		this.ParentCharacter.CurrentAnimStateName = "Action";

		if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.KnockBack)
		{
			this.ParentCharacter.MyAnimator.SetTrigger("KnockBack");
			this.ParentCharacter.MyNavAgent.destination = this.ParentCharacter.MyAI.BlackBoard.ActionMovementDest;
			this.ParentCharacter.MyNavAgent.speed = this.ParentCharacter.MyAI.BlackBoard.ActionMovementSpeed;
			this.ParentCharacter.MyNavAgent.acceleration = 60;
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.KnockForward)
		{
			this.ParentCharacter.MyAnimator.SetTrigger("KnockForward");
			this.ParentCharacter.MyNavAgent.destination = this.ParentCharacter.MyAI.BlackBoard.ActionMovementDest;
			this.ParentCharacter.MyNavAgent.speed = this.ParentCharacter.MyAI.BlackBoard.ActionMovementSpeed;
			this.ParentCharacter.MyNavAgent.acceleration = 60;
		}
	}

	private void UpdateState(MutantBodyStates state)
	{
		switch(state)
		{
		case MutantBodyStates.Idle:
			this.ParentCharacter.CurrentAnimState = new MutantAnimStateIdle(this);
			break;
		}

	}
}
