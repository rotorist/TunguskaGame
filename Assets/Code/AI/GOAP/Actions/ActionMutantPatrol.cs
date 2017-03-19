using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionMutantPatrol : GoapAction 
{
	private bool _isPatrolling;
	private float _actionTimer;
	private float _actionTimeout;
	private float _idleTimer;
	private float _idleDuration;

	public ActionMutantPatrol(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		if( !ParentCharacter.MyAI.BlackBoard.HasPatrolInfo)
		{
			return false;
		}
		CsDebug.Inst.CharLog(ParentCharacter, "Starting to execute Mutant Patrol");
		_isPatrolling = false;
		_idleDuration = 0;
		//for testing
		//ParentCharacter.MyAI.BlackBoard.NavTarget = new Vector3(40, 0, -27);
		//ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = true;

		UpdateAction();
		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnActionUpdateTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{
		//CsDebug.Inst.Log("Action patrol area is completed!", CsDLevel.Info, CsDComponent.AI);
		CsDebug.Inst.CharLog(ParentCharacter, "Action Mutant patrol is completed!");
		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
		ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = false;
		if(ParentCharacter.MyReference.CurrentWeapon != null)
		{
			ParentCharacter.SendCommand(CharacterCommands.StopAim);
		}
		ParentCharacter.SendCommand(CharacterCommands.Idle);


	}

	public override bool AbortAction (float priority)
	{
		StopAction();
		return true;
	}

	public override bool CheckActionCompletion()
	{
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
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null || ParentCharacter.MyAI.BlackBoard.InvisibleEnemy != null)
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

		//if is patrolling, select a random location and go there. after 
		//reaching destination, set patrolling to false, and wait for random seconds
		//then patrol again.


		if(_idleTimer >= _idleDuration)
		{
			_isPatrolling = true;
		}
		else
		{
			_idleTimer = _idleTimer + 1;
			_isPatrolling = false;
		}

		if(_isPatrolling)
		{
			HandlePatrolUpdate();
			ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAhead, Vector3.zero);
		}
		else
		{
			HandleIdleUpdate();
			ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAround, Vector3.zero);
		}

		//check if need to pull out weapon
		ParentCharacter.SendCommand(CharacterCommands.SetAlert);


		//check if need to aim
		if(ParentCharacter.MyAI.BlackBoard.GuardLevel > 2 && ((MutantCharacter)ParentCharacter).IsRangedCapable)
		{
			ParentCharacter.SendCommand(CharacterCommands.Aim);
		}
		else
		{
			ParentCharacter.SendCommand(CharacterCommands.StopAim);
		}



		//check if patrol is complete
		if(CheckActionCompletion())
		{
			StopAction();
			ParentCharacter.MyEventHandler.TriggerOnActionCompletion();
		}
	}




	private bool CheckAvailability()
	{
		if(ParentCharacter.IsBodyLocked || !ParentCharacter.MyAI.BlackBoard.HasPatrolInfo)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	private bool SelectPatrolDestination(out Vector3 result)
	{

		//find a random position within range
		Vector3 randPoint;
		bool isFound = AI.RandomPoint(ParentCharacter.MyAI.BlackBoard.PatrolLoc, ParentCharacter.MyAI.BlackBoard.PatrolRange, out randPoint);
		if(isFound)
		{
			result = randPoint;
			return true;
		}
		else
		{
			result = Vector3.zero;
			return false;
		}
	}

	private void HandlePatrolUpdate()
	{
		if(ParentCharacter.MyAI.BlackBoard.IsNavTargetSet)
		{
			CsDebug.Inst.CharLog(ParentCharacter, "Patrol area update action nav target is set. is patrolling " + _isPatrolling);
			//check if is near patrol destination; if so set isNavTargetSet to false
			if(Vector3.Distance(ParentCharacter.transform.position, ParentCharacter.MyAI.BlackBoard.NavTarget) <= 2)
			{
				ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = false;
				_isPatrolling = false;
				_idleDuration = UnityEngine.Random.Range(5f, 20f);
				_idleTimer = 0;
			}
			else if(!_isPatrolling)
			{
				ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.NavTarget;
				((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Walk;
				ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
				if(ParentCharacter.GetCharacterVelocity().magnitude <= 0)
				{
					ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = false;
				}
				_isPatrolling = true;
			}
		}
		else
		{
			CsDebug.Inst.CharLog(ParentCharacter, "Patrol area update action nav target is not set");
			Vector3 result;
			ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = SelectPatrolDestination(out result);
			if(ParentCharacter.MyAI.BlackBoard.IsNavTargetSet)
			{
				ParentCharacter.MyAI.BlackBoard.NavTarget = result;
				ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.NavTarget;
				ParentCharacter.CurrentStance = HumanStances.Walk;
				ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
				_isPatrolling = true;
			}
		}
	}

	private void HandleIdleUpdate()
	{
		if(_actionTimer >= _actionTimeout)
		{
			ParentCharacter.SendCommand(CharacterCommands.IdleAction);
			_actionTimer = 0;
			_actionTimeout = UnityEngine.Random.Range(15f, 30f);
		}
		else
		{
			_actionTimer = _actionTimer + 1;
		}
	}
}
