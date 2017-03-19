using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionAttackFromCover: GoapAction 
{
	private bool _isHiding;
	private float _completionTimer;

	public ActionAttackFromCover(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		//check if still in cover
		if(ParentCharacter.MyAI.BlackBoard.SelectedCover == null)
		{
			return false;
		}
		else
		{
			if(Vector3.Distance(ParentCharacter.transform.position, ParentCharacter.MyAI.BlackBoard.SelectedCoverLoc) > 1)
			{
				return false;
			}
		}

		CsDebug.Inst.CharLog(ParentCharacter, "Start executing attack from Cover " + ParentCharacter.name);

		((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Walk;
		_completionTimer = 0;
		UpdateAction();

		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{

		CsDebug.Inst.CharLog(ParentCharacter, "Stop executing attack from Cover" + ParentCharacter.name);

		ParentCharacter.SendCommand(CharacterCommands.StopCrouch);
		ParentCharacter.MyAI.WeaponSystem.StopFiringRangedWeapon();
		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;

	}

	public override bool AbortAction (float priority)
	{
		if(_isHiding && priority >= 1f)
		{
			CsDebug.Inst.CharLog(ParentCharacter, "I'm hiding and aborting, priority " + priority);
			if(ParentCharacter.MyAI.BlackBoard.HighestPersonalThreat >= ParentCharacter.MyAI.Sensor.PersonalThreatHigh)
			{
				ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAround, ParentCharacter.MyAI.BlackBoard.AvgPersonalThreatDir);
			}

			//mark this action as impossible and stop action
			float rate = 0.2f;
			if(ParentCharacter.MyAI.ControlType == AIControlType.PlayerTeam)
			{
				rate = 0.5f;
			}
			ParentCharacter.MyAI.WorkingMemory.AddFact(FactType.FailedAction, this.Name, Vector3.zero, 1, rate);
			StopAction();

			return true;
		}
		else
		{
			UpdateAction();
			return false;
		}


	}

	public override bool CheckActionCompletion()
	{
		if(ParentCharacter.MyAI.BlackBoard.GuardLevel == 0)
		{
			return true;
		}

		if(ParentCharacter.MyAI.ControlType == AIControlType.PlayerTeam)
		{
			return false;
		}



		if(_completionTimer >= 15)
		{
			return true;
		}


		foreach(GoapWorldState state in Effects)
		{

			object result = ParentCharacter.MyAI.EvaluateWorldState(state);
			//CsDebug.Inst.CharLog(ParentCharacter, "Checking if state " + state.Name + " value is " + state.Value + " result: " + result);
			if(!result.Equals(state.Value))
			{
				//CsDebug.Inst.CharLog(ParentCharacter, "result is not equal to effect");
				return false;
			}
		}

		return true;
	}

	public override bool CheckContextPrecondition ()
	{
		if(ParentCharacter.MyAI.BlackBoard.GuardLevel == 0)
		{
			return false;
		}

		if(ParentCharacter.MyAI.ControlType == AIControlType.PlayerTeam)
		{
			return true;
		}

		if(ParentCharacter.MyAI.BlackBoard.TargetEnemyThreat < 0.66f)
		{
			return false;
		}

		//check if personal threat is 0. 
		if(ParentCharacter.MyAI.BlackBoard.HighestPersonalThreat > 0)
		{
			return false;
		}

		return true;
	}

	public void UpdateAction()
	{
		if(!CheckAvailability())
		{
			return;
		}


		ParentCharacter.SendCommand(CharacterCommands.Aim);
		//CsDebug.Inst.CharLog(ParentCharacter, "Target enemy is null? " + (ParentCharacter.MyAI.BlackBoard.TargetEnemy == null) + ParentCharacter.name);
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null)
		{
			//check if is in range
			float dist = Vector3.Distance(ParentCharacter.transform.position, ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position);
			float gunRange = 0;
			if(ParentCharacter.MyReference.CurrentWeapon != null)
			{
				if(ParentCharacter.MyReference.CurrentWeapon.GetComponent<GunBarrel>() != null)
				{
					gunRange = ParentCharacter.MyReference.CurrentWeapon.GetComponent<GunBarrel>().Range;
				}
			}

			if(ParentCharacter.MyAI.WeaponSystem.AIWeaponState != AIWeaponStates.FiringRangedWeapon && dist < gunRange)
			{
				//CsDebug.Inst.CharLog(ParentCharacter, "Start firing weapon " + ParentCharacter.name);
				ParentCharacter.MyAI.WeaponSystem.StartFiringRangedWeapon();
			}

			if(_isHiding)
			{
				ParentCharacter.SendCommand(CharacterCommands.StopCrouch);
				_isHiding = false;
			}
			//reset completion timer when enemy is in sight
			_completionTimer = 0;
		}
		else
		{
			ParentCharacter.MyAI.WeaponSystem.StopFiringRangedWeapon();


			if(_completionTimer >= 1)
			{
				Vector3 direction = ParentCharacter.transform.forward;
				if(ParentCharacter.MyAI.BlackBoard.InvisibleEnemy != null)
				{
					direction = ParentCharacter.MyAI.BlackBoard.InvisibleEnemy.transform.position - ParentCharacter.transform.position;
				}
				ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAround, direction);
			}

			_completionTimer ++;

		}



		//if highest personal threat is high or reloading then hide 
		float threat = ParentCharacter.MyAI.BlackBoard.HighestPersonalThreat;
		if((threat > 0.5f && !_isHiding) || ((HumanCharacter)ParentCharacter).ActionState == HumanActionStates.Reload)
		{
			if(ParentCharacter.ActionState == HumanActionStates.Reload)
			{
				ParentCharacter.MyAI.Bark("Reloading!");
			}
			//CsDebug.Inst.CharLog(ParentCharacter, "Stop firing weapon " + ParentCharacter.name);
			ParentCharacter.MyAI.WeaponSystem.StopFiringRangedWeapon();
			ParentCharacter.SendCommand(CharacterCommands.Crouch);
			_isHiding = true;
		}
		else if(threat <= 0 && _isHiding)
		{
			ParentCharacter.SendCommand(CharacterCommands.StopCrouch);
			_isHiding = false;
		}

		if(CheckActionCompletion())
		{
			StopAction();
			ParentCharacter.MyEventHandler.TriggerOnActionCompletion();
		}

	}




	private bool CheckAvailability()
	{
		if(ParentCharacter.IsBodyLocked)
		{
			return false;
		}

		return true;
	}


}
