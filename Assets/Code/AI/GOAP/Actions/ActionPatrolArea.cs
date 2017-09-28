using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionPatrolArea : GoapAction 
{
	private bool _isPatrolling;

	public ActionPatrolArea(string name, string description, float cost)
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
		//CsDebug.Inst.CharLog(ParentCharacter, "Starting to execute Patrol Area");
		_isPatrolling = false;

		//for testing
		//ParentCharacter.MyAI.BlackBoard.NavTarget = new Vector3(40, 0, -27);
		//ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = true;

		ParentCharacter.MyAI.BlackBoard.GuardLevel = 2;

		UpdateAction();
		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnActionUpdateTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{
		//CsDebug.Inst.Log("Action patrol area is completed!", CsDLevel.Info, CsDComponent.AI);
		CsDebug.Inst.CharLog(ParentCharacter, "Action patrol area is completed!");
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
			if(ParentCharacter.MyAI.BlackBoard.TargetEnemyThreat > 0.1f)
			{
				return false;
			}
		}

		return true;
	}

	public void UpdateAction()
	{
		if(!CheckAvailability())
		{
			return;
		}



		if(ParentCharacter.MyAI.BlackBoard.IsNavTargetSet)
		{
			CsDebug.Inst.CharLog(ParentCharacter, "Patrol area update action nav target is set. is patrolling " + _isPatrolling);
			//check if is near patrol destination; if so set isNavTargetSet to false
			if(Vector3.Distance(ParentCharacter.transform.position, ParentCharacter.MyAI.BlackBoard.NavTarget) <= 2)
			{
				ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = false;
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
			else if(_isPatrolling)
			{
				if(ParentCharacter.CurrentAnimStateName == "Idle")
				{
					
					ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.NavTarget;
					((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Walk;
					ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
				}
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
				((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Walk;
				ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
				_isPatrolling = true;
			}
		}

		//check if need to pull out weapon
		ParentCharacter.SendCommand(CharacterCommands.SetAlert);
		/*
		if(ParentCharacter.MyAI.BlackBoard.GuardLevel > 1 && ParentCharacter.MyReference.CurrentWeapon == null)
		{
			


		}
		else if(ParentCharacter.MyAI.BlackBoard.GuardLevel <= 1 && ParentCharacter.MyReference.CurrentWeapon != null)
		{
			ParentCharacter.SendCommand(CharacterCommands.Unarm);
		}
		*/

		//check if need to aim
		if(ParentCharacter.MyAI.BlackBoard.GuardLevel > 2 && ParentCharacter.MyReference.CurrentWeapon != null)
		{
			ParentCharacter.SendCommand(CharacterCommands.Aim);
		}
		else
		{
			ParentCharacter.SendCommand(CharacterCommands.StopAim);
		}

		ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAheadAround, Vector3.zero);
		
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
		if(ParentCharacter.MyAI.BlackBoard.GuardLevel <= 2)
		{
			//look for a patrol node that nobody is using
			int i = ParentCharacter.MyAI.BlackBoard.PatrolNodeIndex;
			if(i < 0)
			{
				i = UnityEngine.Random.Range(0, ParentCharacter.MyAI.Squad.Household.PatrolNodes.Count);
				CsDebug.Inst.CharLog(ParentCharacter, "randomly selected i " + i);
			}
			else
			{
				i++;
			}

			for(int j = 0; j < ParentCharacter.MyAI.Squad.Household.PatrolNodes.Count - 1; j++)
			{
				if(i >= ParentCharacter.MyAI.Squad.Household.PatrolNodes.Count)
				{
					i = 0;
				}

				if(!ParentCharacter.MyAI.Squad.IsPatrolNodeTaken(i))
				{
					result = ParentCharacter.MyAI.Squad.Household.PatrolNodes[i].position;
					ParentCharacter.MyAI.BlackBoard.PatrolNodeIndex = i;
					return true;
				}

				i++;

			}

		}

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
}
