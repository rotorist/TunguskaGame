using UnityEngine;
using System.Collections;

public class HumanAnimStateAction : HumanAnimStateBase
{
	private float _vSpeed;
	private Vector3 _ccVelocity;
	private Vector3 _ccTargetVelocity;
	private float _ccAcceleration;

	// This constructor will create new state taking values from old state
	public HumanAnimStateAction(HumanAnimStateBase state)     
		:this(state.ParentCharacter)
	{

	}

	// this constructor will be used by the other one
	public HumanAnimStateAction(HumanCharacter parentCharacter)
	{
		this.ParentCharacter = parentCharacter;

		Initialize();
	}


	public override void SendCommand (CharacterCommands command)
	{
		if(command == CharacterCommands.AnimationActionDone)
		{
			UpdateState(HumanBodyStates.StandIdle);
			if(ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.ChairSit)
			{
				ParentCharacter.MyAnimator.SetBool("IsChairSitting", false);
			}
		}

	}

	public override void Update ()
	{
		if(ParentCharacter.MyCC != null)
		{
			if((ParentCharacter.MyAI.BlackBoard.ActionMovementDest - ParentCharacter.transform.position).magnitude < 0.1f)
			{
				_ccVelocity = Vector3.Lerp(_ccVelocity, Vector3.zero, Time.deltaTime * 6);
				this.ParentCharacter.MyCC.SimpleMove(_ccVelocity);
			}
			else
			{
				_ccVelocity = Vector3.Lerp(_ccVelocity, _ccTargetVelocity, Time.deltaTime * _ccAcceleration * 0.5f);
				//handle gravity
				_ccVelocity = _ccVelocity + Vector3.down * 20f * Time.deltaTime;
				ParentCharacter.MyCC.SimpleMove(_ccVelocity);
			}

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
		Debug.Log("Initializing Action " + this.ParentCharacter.MyAI.BlackBoard.AnimationAction);
		_vSpeed = this.ParentCharacter.MyAnimator.GetFloat("VSpeed");
		this.ParentCharacter.Destination = this.ParentCharacter.transform.position;
		if(this.ParentCharacter.MyNavAgent != null)
		{
			this.ParentCharacter.MyNavAgent.Stop();
			this.ParentCharacter.MyNavAgent.ResetPath();
			this.ParentCharacter.MyNavAgent.updateRotation = false;
		}
		else if(this.ParentCharacter.MyCC != null)
		{
			this.ParentCharacter.MyCC.SimpleMove(Vector3.zero);
		}

		this.ParentCharacter.MyHeadIK.Weight = 0;
		this.ParentCharacter.CurrentAnimStateName = "Action";


		if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.KnockBack)
		{
			this.ParentCharacter.MyAnimator.SetTrigger("KnockBack");
			MoveCharacter(ParentCharacter.MyAI.BlackBoard.ActionMovementDest, ParentCharacter.MyAI.BlackBoard.ActionMovementSpeed, 60);

		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.KnockForward)
		{
			this.ParentCharacter.MyAnimator.SetTrigger("KnockForward");
			MoveCharacter(ParentCharacter.MyAI.BlackBoard.ActionMovementDest, ParentCharacter.MyAI.BlackBoard.ActionMovementSpeed, 60);
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.ComboAttack)
		{
			//make character face look target

			Vector3 lookDir = this.ParentCharacter.LookTarget.position - this.ParentCharacter.transform.position;
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);
			Quaternion rotation = Quaternion.LookRotation(lookDir);
			this.ParentCharacter.transform.rotation = rotation;

			this.ParentCharacter.MyAnimator.SetTrigger("ComboAttack");

			MoveCharacter(ParentCharacter.MyAI.BlackBoard.ActionMovementDest, ParentCharacter.MyAI.BlackBoard.ActionMovementSpeed, 30);
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.ApplyBandages)
		{
			this.ParentCharacter.MyAnimator.SetTrigger("ApplyBandages");

		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.Landing)
		{
			this.ParentCharacter.MyAnimator.SetTrigger("Land");
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.HardLanding)
		{
			this.ParentCharacter.MyAnimator.SetTrigger("HardLand");
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.ChairSit)
		{
			this.ParentCharacter.MyAnimator.SetFloat("Blend", UnityEngine.Random.value);
			this.ParentCharacter.MyAnimator.SetBool("IsChairSitting", true);
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.ChairStand)
		{
			this.ParentCharacter.MyAnimator.SetFloat("Blend", UnityEngine.Random.value);
			this.ParentCharacter.MyAnimator.SetBool("IsChairSitting", false);
			SetCollider(1.3f, 0.4f, 1);
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.GroundSit)
		{
			this.ParentCharacter.MyAnimator.SetFloat("Blend", UnityEngine.Random.value);
			this.ParentCharacter.MyAnimator.SetBool("IsGroundSitting", true);
			SetCollider(1f, 0.5f, 1);
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.GroundStand)
		{
			this.ParentCharacter.MyAnimator.SetBool("IsGroundSitting", false);
			SetCollider(1.7f, 1f, 1);
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.Sleep)
		{
			if(UnityEngine.Random.value > 0.5f)
			{
				this.ParentCharacter.MyAnimator.SetBool("IsJackingOff", true);
			}
			else
			{
				this.ParentCharacter.MyAnimator.SetBool("IsSleeping", true);
			}
			SetCollider(1.7f, 0.1f, 2);
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.SleepStand)
		{
			this.ParentCharacter.MyAnimator.SetBool("IsSleeping", false);
			this.ParentCharacter.MyAnimator.SetBool("IsJackingOff", false);
			SetCollider(1.7f, 1f, 1);
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.CommanderStand)
		{
			this.ParentCharacter.MyAnimator.SetFloat("Blend", UnityEngine.Random.value);
			this.ParentCharacter.MyAnimator.SetBool("IsCommanderStand", true);
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.AnimationAction == AnimationActions.LostBalance)
		{
			this.ParentCharacter.MyAnimator.SetBool("LostBalance", true);
		}

	}

	private void MoveCharacter(Vector3 dest, float speed, float acceleration)
	{
		if(ParentCharacter.MyNavAgent != null)
		{
			this.ParentCharacter.MyNavAgent.destination = dest;
			this.ParentCharacter.MyNavAgent.speed = speed;
			this.ParentCharacter.MyNavAgent.acceleration = acceleration;
		}
		else if(ParentCharacter.MyCC != null)
		{
			_ccTargetVelocity = (dest - ParentCharacter.transform.position).normalized * speed;
			_ccAcceleration = acceleration;

		}
	}

	private void UpdateState(HumanBodyStates state)
	{
		switch(state)
		{
		case HumanBodyStates.WalkForward:
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateGoForward(this);
			break;
		case HumanBodyStates.StandIdle:
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateIdle(this);
			break;
		case HumanBodyStates.CrouchIdle:
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateSneakIdle(this);
			break;
		}

	}

	private void SetCollider(float height, float center, int direction)
	{
		CapsuleCollider collider = ParentCharacter.GetComponent<CapsuleCollider>();
		collider.height = height;
		collider.center = new Vector3(0, center, 0);
		collider.direction = direction;
	}
}
