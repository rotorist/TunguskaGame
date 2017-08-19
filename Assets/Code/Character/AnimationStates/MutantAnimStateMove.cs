using UnityEngine;
using System.Collections;

public class MutantAnimStateMove : MutantAnimStateBase
{
	private float _vSpeed;
	private bool _isWalkingBack;
	private bool _isFirstUpdateDone;

	// This constructor will create new state taking values from old state
	public MutantAnimStateMove(MutantAnimStateBase state)     
		:this(state.ParentCharacter)
	{

	}

	// this constructor will be used by the other one
	public MutantAnimStateMove(MutantCharacter parentCharacter)
	{
		this.ParentCharacter = parentCharacter;

		Initialize();
	}


	public override void SendCommand(CharacterCommands command)
	{
		switch(command)
		{
		case CharacterCommands.Idle:
			UpdateState(MutantBodyStates.Idle);
			break;
		}
	}

	public override void Update()
	{
		float targetVSpeed = 0;
		float velocity = ParentCharacter.GetCharacterVelocity().magnitude;

		if(this.ParentCharacter.CurrentStance == HumanStances.Run)
		{

			targetVSpeed = 1.5f * ParentCharacter.MyStatus.RunSpeedModifier;
			ParentCharacter.Stealth.SetNoiseLevel(10, 0.6f);


		}
		else if(this.ParentCharacter.CurrentStance == HumanStances.Walk)
		{

			targetVSpeed = 0.5f * ParentCharacter.MyStatus.WalkSpeedModifier;


			ParentCharacter.Stealth.SetNoiseLevel(8, 0.6f);
		}


		_vSpeed = Mathf.Lerp(_vSpeed, targetVSpeed, 6 * Time.deltaTime);
		//Debug.Log("VSpeed " + _vSpeed + " target speed " + targetVSpeed);
		this.ParentCharacter.MyAnimator.SetFloat("VSpeed", _vSpeed);

		HandleNavAgentMovement();
	}

	public override bool IsRotatingBody ()
	{
		return false;
	}







	private void Initialize()
	{
		//Debug.Log("initializing walk forward " + "Dest " + this.ParentCharacter.Destination);
		this.ParentCharacter.MyAnimator.SetFloat("VSpeed", 0);
		this.ParentCharacter.MyAnimator.SetFloat("Blend", ParentCharacter.MyStatus.MutantMovementBlend);
		this.ParentCharacter.MyHeadIK.Weight = 0.75f;
		this.ParentCharacter.CurrentAnimStateName = "Moving";
		//this.ParentCharacter.MyHeadIK.solver.SmoothDisable();
		_vSpeed = 0;

		_isFirstUpdateDone = false;
	}

	private void HandleNavAgentMovement()
	{
		UnityEngine.AI.NavMeshAgent agent = this.ParentCharacter.GetComponent<UnityEngine.AI.NavMeshAgent>();
		//set the speed and acceleration
		if(this.ParentCharacter.CurrentStance == HumanStances.Run || this.ParentCharacter.CurrentStance == HumanStances.Walk || _isWalkingBack)
		{
			if(_isWalkingBack)
			{

				agent.speed = this.ParentCharacter.MyStatus.WalkSpeed;
				agent.acceleration = 20;
			}
			else
			{
				if(this.ParentCharacter.CurrentStance == HumanStances.Run)
				{
					agent.speed = this.ParentCharacter.MyStatus.RunSpeed * this.ParentCharacter.MyStatus.RunSpeedModifier;
					agent.acceleration = 20;
				}
				else
				{
					if(this.ParentCharacter.IsAlert())
					{
						agent.speed = this.ParentCharacter.MyStatus.WalkSpeed * this.ParentCharacter.MyStatus.WalkSpeedModifier * 1f;
						agent.acceleration = 12;
					}
					else
					{
						agent.speed = this.ParentCharacter.MyStatus.WalkSpeed * this.ParentCharacter.MyStatus.WalkSpeedModifier;
						agent.acceleration = 6;
					}

				}
			}


		}


		if(agent.destination != this.ParentCharacter.Destination.Value) 
		{
			agent.SetDestination(this.ParentCharacter.Destination.Value);


			//Debug.Log("Character go forward destination is " + agent.destination + " " + agent.velocity.magnitude);
		}

		if(this.ParentCharacter.UpperBodyState == MutantUpperBodyStates.Idle)
		{

			agent.updateRotation = false;


			//check the destination and look angle
			Vector3 lookDir = this.ParentCharacter.LookTarget.position - this.ParentCharacter.transform.position;
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);

			Vector3 destDir = this.ParentCharacter.GetCharacterVelocity().normalized; 
			destDir = new Vector3(destDir.x, 0, destDir.z);
			float lookDestAngle = Vector3.Angle(lookDir, destDir);

			float destRightBodyAngle = Vector3.Angle(destDir, this.ParentCharacter.transform.right);

			this.ParentCharacter.MyAnimator.SetFloat("LookDestAngle", 0);
			this.ParentCharacter.MyAnimator.SetFloat("DestRightBodyAngle", destRightBodyAngle);

			if(lookDestAngle > 90 && this.ParentCharacter.MyAI.BlackBoard.TargetEnemy != null)
			{
				this.ParentCharacter.MyAnimator.SetFloat("LookDestAngle", lookDestAngle);
				_isWalkingBack = true;

				Vector3 direction = destDir * -1 + lookDir.normalized * 0.05f;
				Quaternion rotation = Quaternion.LookRotation(direction);
				this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
			}
			else 
			{
				_isWalkingBack = false;

				Vector3 direction = destDir + lookDir.normalized * 0.05f;
				Quaternion rotation = Quaternion.LookRotation(direction);
				this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
			}

		}



		if(this.ParentCharacter.IsBodyLocked || this.ParentCharacter.IsMoveLocked)
		{
			UpdateState(MutantBodyStates.Idle);
		}


		//go to idle state if very close to destination
		//if(ParentCharacter.name == "HumanCharacter")
		//Debug.LogError("Remaining distance " + this.ParentCharacter.MyNavAgent.remainingDistance + " pending? " + this.ParentCharacter.MyNavAgent.pathPending + " has path? " + this.ParentCharacter.MyNavAgent.path.status);


		if(!this.ParentCharacter.MyNavAgent.pathPending && _isFirstUpdateDone)
		{
			if(this.ParentCharacter.MyAI.BlackBoard.PendingCommand == CharacterCommands.Loot)
			{
				if(this.ParentCharacter.MyNavAgent.remainingDistance <= this.ParentCharacter.MyNavAgent.stoppingDistance)
				{
					UpdateState(MutantBodyStates.Idle);
					this.ParentCharacter.MyAI.BlackBoard.PendingCommand = CharacterCommands.Idle;
					this.ParentCharacter.SendCommand(CharacterCommands.Loot);
				}
			}
			else if(this.ParentCharacter.MyNavAgent.remainingDistance <= this.ParentCharacter.MyNavAgent.stoppingDistance)
			{
				this.ParentCharacter.MyNavAgent.acceleration = 50;
				UpdateState(MutantBodyStates.Idle);

			}
		}


		if(!_isFirstUpdateDone)
		{
			_isFirstUpdateDone = true;
		}
	}


	private void UpdateState(MutantBodyStates state)
	{
		switch(state)
		{
		case MutantBodyStates.Idle:
			//Debug.Log("switching to idle mutant state");
			this.ParentCharacter.CurrentAnimState = new MutantAnimStateIdle(this);
			break;
		}
	}
}
