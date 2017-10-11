using UnityEngine;
using System.Collections;

public class HumanAnimStateSneakIdle : HumanAnimStateBase
{

	private float _vSpeed;
	private bool _isRotatingBody;
	private float _aimFreelookAngle;
	private float _noAimFreelookAngle;

	private Vector3 _ccVelocity;

	
	// This constructor will create new state taking values from old state
	public HumanAnimStateSneakIdle(HumanAnimStateBase state)     
		:this(state.ParentCharacter)
	{
		
	}
	
	// this constructor will be used by the other one
	public HumanAnimStateSneakIdle(HumanCharacter parentCharacter)
	{
		this.ParentCharacter = parentCharacter;
		
		Initialize();
	}
	
	
	public override void SendCommand (CharacterCommands command)
	{
		switch(command)
		{
		case CharacterCommands.GoToPosition:
			if(this.ParentCharacter.CurrentStance == HumanStances.Sprint)
			{
				UpdateState(HumanBodyStates.WalkForward);
			}
			else
			{
				UpdateState(HumanBodyStates.CrouchWalk);
			}
			break;
		case CharacterCommands.StopCrouch:
			UpdateState(HumanBodyStates.StandIdle);
			break;
		case CharacterCommands.ThrowGrenade:
			//_aimFreelookAngle = 0;
			//_noAimFreelookAngle = 0;
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

		//update character controller, making it stop gradually
		if(this.ParentCharacter.MyCC != null)
		{	
			_ccVelocity = Vector3.Lerp(_ccVelocity, Vector3.zero, Time.deltaTime * 5);
			_ccVelocity = _ccVelocity + Vector3.down * 20f * Time.deltaTime;
			ParentCharacter.MyCC.SimpleMove(_ccVelocity);

			if(!ParentCharacter.MyCC.isGrounded)
			{
				//UpdateState(HumanBodyStates.WalkForward);
			}
			else
			{
				if(ParentCharacter.MyAnimator.GetBool("IsAirborne"))
				{
					UpdateState(HumanBodyStates.WalkForward);
				}
			}
		}


		//update body rotation
		

		
		if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim && !this.ParentCharacter.IsBodyLocked)
		{
			Vector3 lookDir = this.ParentCharacter.AimTarget.position - this.ParentCharacter.transform.position;

			lookDir = new Vector3(lookDir.x, 0, lookDir.z);
			float lookBodyAngle = Vector3.Angle(lookDir, this.ParentCharacter.transform.forward);
			
			if(_isRotatingBody)
			{
				
				Quaternion rotation = Quaternion.LookRotation(lookDir);
				this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
				if(lookBodyAngle < 5)
				{
					_isRotatingBody = false;
				}
			}
			else
			{
				if(lookBodyAngle > _aimFreelookAngle)
				{
					_isRotatingBody = true;
				}
			}
		}
		else if((this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Idle  || this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.HalfAim) && !this.ParentCharacter.IsBodyLocked)
		{
			Vector3 lookDir = this.ParentCharacter.LookTarget.position - this.ParentCharacter.transform.position;
			if(this.ParentCharacter.IsBodyLocked)
			{
				lookDir = this.ParentCharacter.GetLockedAimTarget() - this.ParentCharacter.transform.position;
			}
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);
			float lookBodyAngle = Vector3.Angle(lookDir, this.ParentCharacter.transform.forward);


			if(_isRotatingBody)
			{
				
				Quaternion rotation = Quaternion.LookRotation(lookDir);
				this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
				if(lookBodyAngle < 5)
				{
					_isRotatingBody = false;
				}
			}
			else
			{
				if(lookBodyAngle > _noAimFreelookAngle)
				{
					_isRotatingBody = true;
				}
			}
		}



	}

	public override void FixedUpdate ()
	{
		
	}

	public override bool IsRotatingBody ()
	{
		return _isRotatingBody;
	}
	
	
	
	private void Initialize()
	{
		//Debug.Log("Initializing sneak idle");
		this.ParentCharacter.CurrentAnimStateName = "Sneak Idle";
		this.ParentCharacter.CurrentStance = HumanStances.Crouch;
		_vSpeed = this.ParentCharacter.MyAnimator.GetFloat("VSpeed");
		this.ParentCharacter.MyAnimator.SetBool("IsSneaking", true);
		this.ParentCharacter.Destination = this.ParentCharacter.transform.position;
		if(this.ParentCharacter.MyNavAgent != null)
		{
			this.ParentCharacter.MyNavAgent.Stop();
			this.ParentCharacter.MyNavAgent.ResetPath();
			this.ParentCharacter.MyNavAgent.updateRotation = false;
		}
		this.ParentCharacter.MyHeadIK.Weight = 0.5f;
		_aimFreelookAngle = 45;
		_noAimFreelookAngle = 60;
	}
	
	private void UpdateState(HumanBodyStates state)
	{
		switch(state)
		{
		case HumanBodyStates.CrouchWalk:
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateSneakForward(this);
			break;
		case HumanBodyStates.StandIdle:
			this.ParentCharacter.CurrentStance = HumanStances.Run;
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateIdle(this);
			break;
		case HumanBodyStates.WalkForward:
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateGoForward(this);
			break;
		}
	}
}
