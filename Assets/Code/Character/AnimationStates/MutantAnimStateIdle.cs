using UnityEngine;
using System.Collections;

public class MutantAnimStateIdle : MutantAnimStateBase
{
	private float _vSpeed;
	private bool _isRotatingBody;
	private bool _isMeleeAttacking;

	// This constructor will create new state taking values from old state
	public MutantAnimStateIdle(MutantAnimStateBase state)     
		:this(state.ParentCharacter)
	{

	}

	// this constructor will be used by the other one
	public MutantAnimStateIdle(MutantCharacter parentCharacter)
	{
		this.ParentCharacter = parentCharacter;

		Initialize();
	}


	public override void SendCommand (CharacterCommands command)
	{
		switch(command)
		{
		case CharacterCommands.GoToPosition:
			UpdateState(MutantBodyStates.Move);
			break;
		}

	}

	public override void Update ()
	{
		if(_vSpeed > 0.3f)
		{
			_vSpeed -= 8 * Time.deltaTime;
		}
		else
		{
			_vSpeed = 0;
		}






		this.ParentCharacter.MyAnimator.SetFloat("VSpeed", _vSpeed);

		if(!this.ParentCharacter.IsBodyLocked)
		{
			Vector3 lookDir = this.ParentCharacter.LookTarget.position - this.ParentCharacter.transform.position;


			lookDir = new Vector3(lookDir.x, 0, lookDir.z);
			float lookBodyAngle = Vector3.Angle(lookDir, this.ParentCharacter.transform.forward);

			if(_isRotatingBody)
			{

				Quaternion rotation = Quaternion.LookRotation(lookDir);
				this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 2);
				if(lookBodyAngle < 5)
				{
					_isRotatingBody = false;
				}
			}
			else
			{
				float aimAngle = 60;
				if(this.ParentCharacter.ActionState == HumanActionStates.Melee || this.ParentCharacter.ActionState == HumanActionStates.Block)
				{
					aimAngle = 5;
				}

				if(lookBodyAngle > aimAngle)
				{
					_isRotatingBody = true;
				}
			}

			this.ParentCharacter.MyAnimator.SetBool("IsRotating", _isRotatingBody);
		}
	}

	public override bool IsRotatingBody ()
	{
		return _isRotatingBody;
	}



	private void Initialize()
	{
		//Debug.Log("Initializing Stand Idle");
		_vSpeed = this.ParentCharacter.MyAnimator.GetFloat("VSpeed");
		this.ParentCharacter.MyAnimator.SetFloat("Blend", UnityEngine.Random.value);
		this.ParentCharacter.Destination = this.ParentCharacter.transform.position;
		this.ParentCharacter.MyNavAgent.Stop();
		this.ParentCharacter.MyNavAgent.ResetPath();
		this.ParentCharacter.MyNavAgent.updateRotation = false;
		this.ParentCharacter.MyHeadIK.Weight = 1;
		this.ParentCharacter.CurrentAnimStateName = "Idle";

	}

	private void UpdateState(MutantBodyStates state)
	{
		switch(state)
		{
		case MutantBodyStates.Move:
			//Debug.Log("switching to move mutant state");
			this.ParentCharacter.CurrentAnimState = new MutantAnimStateMove(this);
			break;
		}

	}
}
