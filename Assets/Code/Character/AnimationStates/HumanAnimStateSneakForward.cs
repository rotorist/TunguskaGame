using UnityEngine;
using System.Collections;

public class HumanAnimStateSneakForward : HumanAnimStateBase
{

	private float _vSpeed;
	private bool _isWalkingBack;
	private bool _isStrafing;
	private float _aimFreelookAngle;
	private float _noAimFreelookAngle;
	private bool _isFirstUpdateDone;

	private Vector3 _ccVelocity;
	private Vector3 _ccAirVelocity;
	private Vector3 _ccTargetVelocity;
	private float _ccAcceleration;

	private float _highestAirV;
	private float _airborneTimer;
	
	// This constructor will create new state taking values from old state
	public HumanAnimStateSneakForward(HumanAnimStateBase state)     
		:this(state.ParentCharacter)
	{
		
	}
	
	// this constructor will be used by the other one
	public HumanAnimStateSneakForward(HumanCharacter parentCharacter)
	{
		this.ParentCharacter = parentCharacter;
		
		Initialize();
	}
	
	
	public override void SendCommand(CharacterCommands command)
	{
		switch(command)
		{
		case CharacterCommands.StopCrouch:
			Debug.Log("Stop Crouch during sneak forward");
			UpdateState(HumanBodyStates.StandIdle);
			break;
		case CharacterCommands.Idle:
			UpdateState(HumanBodyStates.CrouchIdle);
			break;
		case CharacterCommands.ThrowGrenade:
			UpdateState(HumanBodyStates.CrouchIdle);
			break;
		case CharacterCommands.Throw:
			UpdateState(HumanBodyStates.CrouchIdle);
			break;
		}
	}
	
	public override void Update()
	{
		float targetVSpeed = 0;
		float velocity = ParentCharacter.GetCharacterVelocity().magnitude;

		if(velocity > 0 && velocity <= 1.5f)//(this.ParentCharacter.CurrentStance == HumanStances.Crouch)
		{
			targetVSpeed = 0.9f;
		}
		else if(velocity > 1.5f)//(this.ParentCharacter.CurrentStance == HumanStances.CrouchRun)
		{
			targetVSpeed = 1.3f;
		}
		
		_vSpeed = Mathf.Lerp(_vSpeed, targetVSpeed, 6 * Time.deltaTime);
		//Debug.Log("VSpeed " + _vSpeed);
		this.ParentCharacter.MyAnimator.SetFloat("VSpeed", _vSpeed);


		if(ParentCharacter.MyNavAgent != null)
		{
			HandleNavAgentMovement();
		}


		HandleTurnMovement();
	}

	public override void FixedUpdate ()
	{
		if(ParentCharacter.MyCC != null)
		{
			HandleCharacterController();
		}
	}
	
	public override bool IsRotatingBody ()
	{
		return false;
	}
	
	
	
	
	
	private void Initialize()
	{
		//Debug.Log("initializing sneak forward " + "Dest " + this.ParentCharacter.Destination);
		this.ParentCharacter.CurrentAnimStateName = "Sneak Forward";
		this.ParentCharacter.CurrentStance = HumanStances.Crouch;
		this.ParentCharacter.MyAnimator.SetFloat("VSpeed", 0);
		this.ParentCharacter.MyAnimator.SetBool("IsSneaking", true);
		this.ParentCharacter.MyHeadIK.Weight = 0.5f;
		_vSpeed = 0;
		_isFirstUpdateDone = false;
	}


	private void HandleCharacterController()
	{
		//set the speed and acceleration
		float targetVelocity = 0;

		if(this.ParentCharacter.CurrentStance == HumanStances.Crouch)
		{
			if(_isWalkingBack)
			{
				targetVelocity = 1f;
				_ccAcceleration = 20;
			}
			else
			{
				if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim && !this.ParentCharacter.IsHipAiming)
				{
					targetVelocity = 1.3f;
					_ccAcceleration = 10;
				}
				else
				{
					targetVelocity = 2f;
					_ccAcceleration = 10;
				}
			}
		}
		else if(this.ParentCharacter.CurrentStance == HumanStances.Sprint)//CrouchRun && !_isWalkingBack && !_isStrafing)
		{
			//agent.speed = 2.2f;
			//agent.acceleration = 20;
			UpdateState(HumanBodyStates.WalkForward);
		}




		Vector3 dist = ParentCharacter.Destination.Value - ParentCharacter.transform.position;
		//handle falling



		if(!ParentCharacter.MyCC.isGrounded)
		{
			//ParentCharacter.MyCC.Move(Vector3.zero);
			_ccAirVelocity = _ccAirVelocity + Vector3.down * 19f * Time.fixedDeltaTime;
			_ccVelocity = _ccVelocity - _ccVelocity * Time.fixedDeltaTime * 3;
			if(_ccVelocity.magnitude < 0.1f)
			{
				_ccVelocity = Vector3.zero;
			}
			ParentCharacter.MyCC.Move((_ccAirVelocity + _ccVelocity) * Time.fixedDeltaTime);
			if(ParentCharacter.MyCC.velocity.magnitude > _highestAirV)
			{
				_highestAirV = ParentCharacter.MyCC.velocity.magnitude;
			}
		}
		else
		{
			//handle non falling
			_ccTargetVelocity = targetVelocity * dist.normalized;
			_ccVelocity = Vector3.Lerp(_ccVelocity, _ccTargetVelocity, Time.fixedDeltaTime * _ccAcceleration * 0.35f);
			_ccVelocity += Vector3.down * 19f * Time.fixedDeltaTime;
			ParentCharacter.MyCC.SimpleMove(_ccVelocity);
			//Debug.Log("moving character " + _ccVelocity);
		}

		//handle falling animation
		bool isAlreadyFalling = ParentCharacter.MyAnimator.GetBool("IsAirborne");
		if(!ParentCharacter.MyCC.isGrounded)
		{

			if(!isAlreadyFalling && GameManager.Inst.PlayerControl.GroundDistance > 0.5f)
			{
				if(_airborneTimer < 0.1f)
				{
					_airborneTimer += Time.fixedDeltaTime;
				}
				else
				{
					_airborneTimer = 0;
					ParentCharacter.MyAnimator.SetBool("IsAirborne", true);
					_ccAirVelocity = Vector3.zero;
					_ccVelocity = new Vector3(_ccVelocity.x, 0, _ccVelocity.z);
					_highestAirV = 0;
				}
			}
		}
		else
		{
			//handle stop falling
			if(isAlreadyFalling)
			{
				if(_highestAirV > 8f)
				{
					ParentCharacter.MyAI.BlackBoard.AnimationAction = AnimationActions.HardLanding;
				}
				else
				{
					ParentCharacter.MyAI.BlackBoard.AnimationAction = AnimationActions.Landing;
				}
				ParentCharacter.MyAI.BlackBoard.ActionMovementDest = ParentCharacter.transform.position;
				ParentCharacter.MyAI.BlackBoard.ActionMovementSpeed = 0;
				ParentCharacter.SendCommand(CharacterCommands.PlayAnimationAction);

				ParentCharacter.MyAnimator.SetBool("IsAirborne", false);
				ParentCharacter.Destination = ParentCharacter.transform.position;
				ParentCharacter.OnFallLanding(_highestAirV, 8f);
			}
		}

		if(this.ParentCharacter.MyAI.BlackBoard.PendingCommand == CharacterCommands.Talk)
		{
			Vector3 targetDist = this.ParentCharacter.MyAI.BlackBoard.InteractTarget.transform.position - this.ParentCharacter.transform.position;
			if(targetDist.magnitude <= 0.8f)
			{
				UpdateState(HumanBodyStates.CrouchIdle);
				this.ParentCharacter.MyAI.BlackBoard.PendingCommand = CharacterCommands.Idle;
				this.ParentCharacter.SendCommand(CharacterCommands.Talk);
			}
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.PendingCommand == CharacterCommands.Loot)
		{
			Vector3 targetDist = this.ParentCharacter.MyAI.BlackBoard.InteractTarget.transform.position - this.ParentCharacter.transform.position;
			if(targetDist.magnitude <= 1)
			{
				UpdateState(HumanBodyStates.CrouchIdle);
				this.ParentCharacter.MyAI.BlackBoard.PendingCommand = CharacterCommands.Idle;
				this.ParentCharacter.SendCommand(CharacterCommands.Loot);
			}
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.PendingCommand == CharacterCommands.LootChest)
		{
			Vector3 targetDist = this.ParentCharacter.MyAI.BlackBoard.UseTarget.transform.position - this.ParentCharacter.transform.position;
			if(targetDist.magnitude <= 1)
			{
				UpdateState(HumanBodyStates.CrouchIdle);
				this.ParentCharacter.MyAI.BlackBoard.PendingCommand = CharacterCommands.Idle;
				this.ParentCharacter.SendCommand(CharacterCommands.LootChest);
			}
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.PendingCommand == CharacterCommands.Pickup)
		{
			Vector3 targetDist = this.ParentCharacter.MyAI.BlackBoard.PickupTarget.transform.position - (this.ParentCharacter.transform.position + new Vector3(0, 0.6f, 0));
			if(targetDist.magnitude <= 1.5f)
			{
				UpdateState(HumanBodyStates.CrouchIdle);
				this.ParentCharacter.MyAI.BlackBoard.PendingCommand = CharacterCommands.Idle;
				this.ParentCharacter.SendCommand(CharacterCommands.Pickup);
			}
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.PendingCommand == CharacterCommands.Use)
		{
			Vector3 targetDist = this.ParentCharacter.MyAI.BlackBoard.UseTarget.transform.position - this.ParentCharacter.transform.position;
			if(targetDist.magnitude <= 1.5f)
			{
				UpdateState(HumanBodyStates.StandIdle);
				this.ParentCharacter.MyAI.BlackBoard.PendingCommand = CharacterCommands.Idle;
				this.ParentCharacter.SendCommand(CharacterCommands.Use);
			}
		}
		else if(dist.magnitude < 0.2f || ParentCharacter.MyCC.velocity.magnitude <= 0.02f)
		{
			UpdateState(HumanBodyStates.CrouchIdle);
		}

	}

	private void HandleNavAgentMovement()
	{
		UnityEngine.AI.NavMeshAgent agent = this.ParentCharacter.MyNavAgent;


		//set the speed and acceleration
		if(this.ParentCharacter.CurrentStance == HumanStances.Crouch)
		{
			if(_isWalkingBack)
			{
				agent.speed = 1f;
				agent.acceleration = 20;
			}
			else
			{
				agent.speed = 2f;
				agent.acceleration = 20;
			}
		}
		else if(this.ParentCharacter.CurrentStance == HumanStances.Sprint)//CrouchRun && !_isWalkingBack && !_isStrafing)
		{
			//agent.speed = 2.2f;
			//agent.acceleration = 20;
			UpdateState(HumanBodyStates.WalkForward);
		}

		
		agent.SetDestination(this.ParentCharacter.Destination.Value);



		//go to idle state if very close to destination

		if(!this.ParentCharacter.MyNavAgent.pathPending && _isFirstUpdateDone)
		{


			if(this.ParentCharacter.MyNavAgent.remainingDistance <= this.ParentCharacter.MyNavAgent.stoppingDistance)
			{
				UpdateState(HumanBodyStates.CrouchIdle);
			}
		}


		if(!_isFirstUpdateDone)
		{
			_isFirstUpdateDone = true;
		}
	}

	private void HandleTurnMovement()
	{
		if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Idle || this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.HalfAim)
		{

			_isStrafing = false;

			//check the destination and look angle
			Vector3 lookDir = this.ParentCharacter.LookTarget.position - this.ParentCharacter.transform.position;
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);

			Vector3 destDir = this.ParentCharacter.GetCharacterVelocity().normalized;
			destDir = new Vector3(destDir.x, 0, destDir.z);
			float lookDestAngle = Vector3.Angle(lookDir, destDir);
			float destRightBodyAngle = Vector3.Angle(destDir, this.ParentCharacter.transform.right);

			this.ParentCharacter.MyAnimator.SetFloat("LookDestAngle", 0);
			this.ParentCharacter.MyAnimator.SetFloat("DestRightBodyAngle", destRightBodyAngle);
			/*
			if(lookDestAngle > 90)
			{
				_isWalkingBack = true;
				_isStrafing = false;
				
				Vector3 direction = destDir * -1 + lookDir.normalized * 0.05f;
				Quaternion rotation = Quaternion.LookRotation(direction);
				this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
			}
			else */
			{
				_isWalkingBack = false;
				_isStrafing = false;

				Vector3 direction = destDir + lookDir.normalized * 0.05f;
				Quaternion rotation = Quaternion.LookRotation(direction);
				this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
			}
		}
		else if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim)
		{

			//check the destination and look angle
			Vector3 lookDir = this.ParentCharacter.LookTarget.position - this.ParentCharacter.transform.position;
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);

			Vector3 destDir = this.ParentCharacter.Destination.Value - this.ParentCharacter.transform.position;
			destDir = new Vector3(destDir.x, 0, destDir.z);

			float lookDestAngle = Vector3.Angle(lookDir, destDir);
			float destRightBodyAngle = Vector3.Angle(destDir, this.ParentCharacter.transform.right);
			this.ParentCharacter.MyAnimator.SetFloat("LookDestAngle", lookDestAngle);
			this.ParentCharacter.MyAnimator.SetFloat("DestRightBodyAngle", destRightBodyAngle);
			//Debug.Log("look dest angle " + lookDestAngle);
			//if destination and look dir angle greater than 90 it means we are walking backwards. when
			//walking backwards disable agent update rotation and manually align rotation to opposite of destDir
			//when holding weapon and aiming, then it's 45 degrees so we will go into strafe mode
			WeaponAnimType weaponType = (WeaponAnimType)this.ParentCharacter.MyAnimator.GetInteger("WeaponType");

			if(weaponType == WeaponAnimType.Pistol || weaponType == WeaponAnimType.Longgun || weaponType == WeaponAnimType.Grenade)
			{
				if(lookDestAngle > 45 && lookDestAngle <= 135)
				{
					//strafe
					_isStrafing = true;
					_isWalkingBack = false;

					Vector3 direction = Vector3.zero;
					//check if body is turning left or right by checking the angle between lookdir and cross(up, destdir)
					Vector3 crossUpDestDir = Vector3.Cross(Vector3.up, destDir);
					float lookCrossDirAngle = Vector3.Angle(lookDir, crossUpDestDir);

					if(lookCrossDirAngle > 90)
					{
						direction = crossUpDestDir * -1;
					}
					else
					{
						direction = crossUpDestDir;
					}

					Quaternion rotation = Quaternion.LookRotation(direction);
					this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
				}
				else if(lookDestAngle > 135)
				{
					//walk back
					_isWalkingBack = true;
					_isStrafing = false;

					Vector3 direction = destDir * -1 + lookDir.normalized * 0.05f;
					Quaternion rotation = Quaternion.LookRotation(direction);
					this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
				}
				else
				{
					//walk forward
					_isWalkingBack = false;
					_isStrafing = false;

					Vector3 direction = destDir + lookDir.normalized * 0.05f;
					Quaternion rotation = Quaternion.LookRotation(direction);
					this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
				}
			}




		}
	}
	
	private void UpdateState(HumanBodyStates state)
	{
		switch(state)
		{
		case HumanBodyStates.StandIdle:
			this.ParentCharacter.CurrentStance = HumanStances.Run;
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateIdle(this);
			break;
		case HumanBodyStates.CrouchIdle:
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateSneakIdle(this);
			break;
		case HumanBodyStates.WalkForward:
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateGoForward(this);
			break;
		}
	}
}
