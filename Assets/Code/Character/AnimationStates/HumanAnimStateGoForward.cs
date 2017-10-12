using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class HumanAnimStateGoForward : HumanAnimStateBase
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

	private float _stuckTimer;

	// This constructor will create new state taking values from old state
	public HumanAnimStateGoForward(HumanAnimStateBase state)     
		:this(state.ParentCharacter)
	{
		
	}
	
	// this constructor will be used by the other one
	public HumanAnimStateGoForward(HumanCharacter parentCharacter)
	{
		this.ParentCharacter = parentCharacter;
		
		Initialize();
	}
	
	
	public override void SendCommand(CharacterCommands command)
	{
		switch(command)
		{
		case CharacterCommands.Crouch:
			UpdateState(HumanBodyStates.CrouchWalk);
			break;
		case CharacterCommands.ThrowGrenade:
			UpdateState(HumanBodyStates.StandIdle);
			break;
		case CharacterCommands.Throw:
			UpdateState(HumanBodyStates.StandIdle);
			break;
		case CharacterCommands.LeftAttack:
			UpdateState(HumanBodyStates.StandIdle);
			break;
		case CharacterCommands.RightAttack:
			UpdateState(HumanBodyStates.StandIdle);
			break;
		case CharacterCommands.Block:
			UpdateState(HumanBodyStates.StandIdle);
			break;
		case CharacterCommands.Idle:
			UpdateState(HumanBodyStates.StandIdle);
			break;
		}
	}
	
	public override void Update()
	{
		float targetVSpeed = 0;
		float velocity = ParentCharacter.GetCharacterVelocity().magnitude;


		if(/*(velocity > this.ParentCharacter.MyStatus.WalkSpeed && velocity <= this.ParentCharacter.MyStatus.RunSpeed) &&*/ this.ParentCharacter.CurrentStance == HumanStances.Run)
		{
			if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim && !this.ParentCharacter.IsHipAiming)
			{
				targetVSpeed = 1f  * this.ParentCharacter.MyStatus.StrafeSpeedModifier;
				ParentCharacter.Stealth.SetNoiseLevel(8, 0.6f);
			}
			else if(this.ParentCharacter.ActionState == HumanActionStates.Melee)
			{
				targetVSpeed = 0.5f;
				ParentCharacter.Stealth.SetNoiseLevel(8, 0.6f);
			}
			else
			{
				targetVSpeed = 1.5f * this.ParentCharacter.MyStatus.RunSpeedModifier;
				ParentCharacter.Stealth.SetNoiseLevel(10, 0.6f);
			}

		}
		else if(/*(velocity > 0 && velocity <= this.ParentCharacter.MyStatus.WalkSpeed) &&*/ this.ParentCharacter.CurrentStance == HumanStances.Walk)
		{
			targetVSpeed = 1;
			ParentCharacter.Stealth.SetNoiseLevel(8, 0.6f);
		}
		else if(/*(velocity > this.ParentCharacter.MyStatus.RunSpeed) &&*/ this.ParentCharacter.CurrentStance == HumanStances.Sprint)
		{
			targetVSpeed = 2f * this.ParentCharacter.MyStatus.SprintSpeedModifier;
			ParentCharacter.Stealth.SetNoiseLevel(15, 0.6f);
		}

			
		_vSpeed = Mathf.Lerp(_vSpeed, targetVSpeed, 8 * Time.deltaTime);
		if(velocity < 0.05f)
		{
			_vSpeed = 0;
		}
		//Debug.Log("VSpeed " + _vSpeed + " target speed " + targetVSpeed);
		this.ParentCharacter.MyAnimator.SetFloat("VSpeed", _vSpeed);

		if(ParentCharacter.MyNavAgent != null)
		{
			HandleNavAgentMovement();
		}

	}

	public override void FixedUpdate ()
	{
		if(ParentCharacter.MyCC != null)
		{
			HandleCharacterController();
		}

		HandleTurnMovement();
	}
	
	public override bool IsRotatingBody ()
	{
		return false;
	}





	private void Initialize()
	{
		//Debug.Log("initializing walk forward " + "Dest " + this.ParentCharacter.Destination + " current at " + this.ParentCharacter.transform.position + " " + ParentCharacter.name);
		this.ParentCharacter.CurrentAnimStateName = "Go Forward";
		this.ParentCharacter.MyAnimator.SetFloat("VSpeed", 0);
		this.ParentCharacter.MyAnimator.SetBool("IsSneaking", false);
		this.ParentCharacter.MyHeadIK.Weight = 0.75f;
		//this.ParentCharacter.MyHeadIK.solver.SmoothDisable();
		_vSpeed = 0;

		_isFirstUpdateDone = false;
	}





	private void HandleCharacterController()
	{
		//set the speed and acceleration
		float targetVelocity = 0;

		if(this.ParentCharacter.CurrentStance == HumanStances.Run || this.ParentCharacter.CurrentStance == HumanStances.Walk || _isWalkingBack)
		{
			if(_isWalkingBack && !_isStrafing)
			{
				if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim)
				{
					//walking backward while aiming
					targetVelocity = this.ParentCharacter.MyStatus.StrafeSpeed * this.ParentCharacter.MyStatus.StrafeSpeedModifier;
				}
				else
				{
					if(this.ParentCharacter.GetCurrentAnimWeapon() == WeaponAnimType.Melee)
					{
						targetVelocity = this.ParentCharacter.MyStatus.StrafeSpeed * 1.25f;
					}
					else
					{
						targetVelocity = this.ParentCharacter.MyStatus.WalkSpeed;
					}
				}

				_ccAcceleration = 20;
			}
			else if(_isStrafing)
			{
				if(this.ParentCharacter.IsHipAiming)
				{
					targetVelocity = this.ParentCharacter.MyStatus.RunSpeed * this.ParentCharacter.MyStatus.RunSpeedModifier * 0.9f;
				}
				else
				{
					targetVelocity = this.ParentCharacter.MyStatus.StrafeSpeed * this.ParentCharacter.MyStatus.StrafeSpeedModifier;
				}

				_ccAcceleration = 20;
			}
			else
			{
				if(this.ParentCharacter.CurrentStance == HumanStances.Run)
				{
					if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim)
					{
						if(this.ParentCharacter.IsHipAiming)
						{
							targetVelocity = this.ParentCharacter.MyStatus.RunSpeed * this.ParentCharacter.MyStatus.RunSpeedModifier * 0.9f;
						}
						else
						{
							targetVelocity = this.ParentCharacter.MyStatus.StrafeSpeed;
						}
					}
					else
					{
						if(this.ParentCharacter.ActionState == HumanActionStates.Melee)
						{
							targetVelocity = this.ParentCharacter.MyStatus.WalkSpeed;
						}
						else
						{
							targetVelocity = this.ParentCharacter.MyStatus.RunSpeed * this.ParentCharacter.MyStatus.RunSpeedModifier;
						}
					}
					_ccAcceleration = 20;
				}
				else
				{
					targetVelocity = this.ParentCharacter.MyStatus.WalkSpeed;
					_ccAcceleration = 10;
				}
			}


		}
		else if(this.ParentCharacter.CurrentStance == HumanStances.Sprint && !_isWalkingBack)
		{
			targetVelocity = this.ParentCharacter.MyStatus.SprintSpeed * this.ParentCharacter.MyStatus.SprintSpeedModifier;
			_ccAcceleration = 20;
		}
			
		if(this.ParentCharacter.IsBodyLocked || this.ParentCharacter.IsMoveLocked)
		{
			UpdateState(HumanBodyStates.StandIdle);
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
			ParentCharacter.MyCC.Move(_ccVelocity * Time.fixedDeltaTime);
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
			if(targetDist.magnitude <= 1.5f)
			{
				UpdateState(HumanBodyStates.StandIdle);
				this.ParentCharacter.MyAI.BlackBoard.PendingCommand = CharacterCommands.Idle;
				this.ParentCharacter.SendCommand(CharacterCommands.Talk);
			}
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.PendingCommand == CharacterCommands.Loot)
		{
			Vector3 targetDist = this.ParentCharacter.MyAI.BlackBoard.InteractTarget.transform.position - this.ParentCharacter.transform.position;
			if(targetDist.magnitude <= 1)
			{
				UpdateState(HumanBodyStates.StandIdle);
				this.ParentCharacter.MyAI.BlackBoard.PendingCommand = CharacterCommands.Idle;
				this.ParentCharacter.SendCommand(CharacterCommands.Loot);
			}
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.PendingCommand == CharacterCommands.LootChest)
		{
			Vector3 targetDist = this.ParentCharacter.MyAI.BlackBoard.UseTarget.transform.position - this.ParentCharacter.transform.position;
			if(targetDist.magnitude <= 1)
			{
				UpdateState(HumanBodyStates.StandIdle);
				this.ParentCharacter.MyAI.BlackBoard.PendingCommand = CharacterCommands.Idle;
				this.ParentCharacter.SendCommand(CharacterCommands.LootChest);
			}
		}
		else if(this.ParentCharacter.MyAI.BlackBoard.PendingCommand == CharacterCommands.Pickup)
		{
			Vector3 targetDist = this.ParentCharacter.MyAI.BlackBoard.PickupTarget.transform.position - (this.ParentCharacter.transform.position + new Vector3(0, 0.6f, 0));
			if(targetDist.magnitude <= 1.5f)
			{
				UpdateState(HumanBodyStates.StandIdle);
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



		if(dist.magnitude < 0.2f || ParentCharacter.MyCC.velocity.magnitude <= 0.02f)
		{
			this.ParentCharacter.MyAI.BlackBoard.PendingCommand = CharacterCommands.Idle;
			UpdateState(HumanBodyStates.StandIdle);
		}
	
	}

	private void HandleNavAgentMovement()
	{
		UnityEngine.AI.NavMeshAgent agent = this.ParentCharacter.MyNavAgent;

		//set the speed and acceleration
		if(this.ParentCharacter.CurrentStance == HumanStances.Run || this.ParentCharacter.CurrentStance == HumanStances.Walk || _isWalkingBack)
		{
			if(_isWalkingBack && !_isStrafing)
			{
				if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim)
				{
					//walking backward while aiming
					agent.speed = this.ParentCharacter.MyStatus.StrafeSpeed * this.ParentCharacter.MyStatus.StrafeSpeedModifier;
				}
				else
				{
					if(this.ParentCharacter.GetCurrentAnimWeapon() == WeaponAnimType.Melee)
					{
						agent.speed = this.ParentCharacter.MyStatus.StrafeSpeed * 1.25f;
					}
					else
					{
						agent.speed = this.ParentCharacter.MyStatus.WalkSpeed;
					}
				}

				agent.acceleration = 20;
			}
			else if(_isStrafing)
			{
				if(this.ParentCharacter.IsHipAiming)
				{
					agent.speed = this.ParentCharacter.MyStatus.RunSpeed * this.ParentCharacter.MyStatus.RunSpeedModifier * 0.9f;
				}
				else
				{
					agent.speed = this.ParentCharacter.MyStatus.StrafeSpeed * this.ParentCharacter.MyStatus.StrafeSpeedModifier;
				}

				agent.acceleration = 20;
			}
			else
			{
				if(this.ParentCharacter.CurrentStance == HumanStances.Run)
				{
					if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim)
					{
						if(this.ParentCharacter.IsHipAiming)
						{
							agent.speed = this.ParentCharacter.MyStatus.RunSpeed * this.ParentCharacter.MyStatus.RunSpeedModifier * 0.9f;
						}
						else
						{
							agent.speed = this.ParentCharacter.MyStatus.StrafeSpeed;
						}
					}
					else
					{
						if(this.ParentCharacter.ActionState == HumanActionStates.Melee)
						{
							agent.speed = this.ParentCharacter.MyStatus.WalkSpeed;
						}
						else
						{
							agent.speed = this.ParentCharacter.MyStatus.RunSpeed * this.ParentCharacter.MyStatus.RunSpeedModifier;
						}
					}
					agent.acceleration = 20;
				}
				else
				{
					agent.speed = this.ParentCharacter.MyStatus.WalkSpeed;
					agent.acceleration = 6;
				}
			}


		}
		else if(this.ParentCharacter.CurrentStance == HumanStances.Sprint && !_isWalkingBack)
		{
			agent.speed = this.ParentCharacter.MyStatus.SprintSpeed * this.ParentCharacter.MyStatus.SprintSpeedModifier;
			agent.acceleration = 20;
		}

		if(agent.destination != this.ParentCharacter.Destination.Value)
		{
			agent.SetDestination(this.ParentCharacter.Destination.Value);


			//Debug.Log("Character go forward destination is " + agent.destination + " " + agent.velocity.magnitude);
		}




		if(this.ParentCharacter.IsBodyLocked || this.ParentCharacter.IsMoveLocked)
		{
			UpdateState(HumanBodyStates.StandIdle);
		}
	

		//go to idle state if very close to destination
		//if(ParentCharacter.name == "HumanCharacter")
			//Debug.LogError("Remaining distance " + this.ParentCharacter.MyNavAgent.remainingDistance + " pending? " + this.ParentCharacter.MyNavAgent.pathPending + " has path? " + this.ParentCharacter.MyNavAgent.path.status);


		if(!this.ParentCharacter.MyNavAgent.pathPending && _isFirstUpdateDone)
		{
			
			if(this.ParentCharacter.MyNavAgent.remainingDistance <= this.ParentCharacter.MyNavAgent.stoppingDistance)
			{
				this.ParentCharacter.MyNavAgent.acceleration = 50;
				UpdateState(HumanBodyStates.StandIdle);

			}


		}


		if(this.ParentCharacter.MyNavAgent.remainingDistance > this.ParentCharacter.MyNavAgent.stoppingDistance)
		{
			if(ParentCharacter.name == "HumanCharacterHans")
			{
				//Debug.Log("I'm not moving? " + ParentCharacter.MyNavAgent.velocity.magnitude + " " + ParentCharacter.MyNavAgent.remainingDistance + " / " + ParentCharacter.MyNavAgent.stoppingDistance);
			}
			if(this.ParentCharacter.MyNavAgent.velocity.magnitude < 0.5f)
			{
				if(_stuckTimer < 0.1f)
				{
					_stuckTimer += Time.fixedDeltaTime;
				}
				else
				{
					if(this.ParentCharacter.CurrentDoor != null)
					{
						this.ParentCharacter.CurrentDoor.Open(this.ParentCharacter.transform, false);
					}
					else
					{
						this.ParentCharacter.MyNavAgent.SetDestination(this.ParentCharacter.transform.position + this.ParentCharacter.transform.forward * -0.5f);
					}
					_stuckTimer = 0;
				}
			}
		}



		if(!_isFirstUpdateDone)
		{
			_isFirstUpdateDone = true;
		}

	}


	private void HandleTurnMovement()
	{
		if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Idle || this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.HalfAim || this.ParentCharacter.CurrentStance == HumanStances.Sprint)
		{

			//agent.updateRotation = false;

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

			if(lookDestAngle > 90 && this.ParentCharacter.GetCurrentAnimWeapon() == WeaponAnimType.Melee && this.ParentCharacter.CurrentStance != HumanStances.Sprint)
			{
				this.ParentCharacter.MyAnimator.SetFloat("LookDestAngle", lookDestAngle);
				_isWalkingBack = true;
				_isStrafing = false;

				Vector3 direction = destDir * -1 + lookDir.normalized * 0.05f;
				Quaternion rotation = Quaternion.LookRotation(direction);
				this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
			}
			else 
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
			//agent.updateRotation = false;
			//check the destination and look angle
			Vector3 lookDir = this.ParentCharacter.AimTarget.position - this.ParentCharacter.transform.position;
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);

			Vector3 destDir = this.ParentCharacter.GetCharacterVelocity().normalized; //this.ParentCharacter.Destination.Value - this.ParentCharacter.transform.position;
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
				if(lookDestAngle > 45 && lookDestAngle <= 135 && this.ParentCharacter.CurrentStance != HumanStances.Sprint)
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

					if(direction == Vector3.zero)
					{
						direction = this.ParentCharacter.transform.forward;
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
		//Debug.Log("leaving go forward state, going to " + state);
		switch(state)
		{
		case HumanBodyStates.StandIdle:
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateIdle(this);
			break;
		case HumanBodyStates.CrouchWalk:
			this.ParentCharacter.CurrentStance = HumanStances.Crouch;
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateSneakForward(this);
			break;
		}
	}
	
	

}
