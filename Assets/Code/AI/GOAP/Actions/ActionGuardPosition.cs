using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionGuardPosition: GoapAction 
{

	public ActionGuardPosition(string name, string description, float cost)
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

		CsDebug.Inst.CharLog(ParentCharacter, "Start executing Guard" + ParentCharacter.name);
		_executionStopped = false;

		ParentCharacter.MyAI.BlackBoard.GuardLevel = 2;

		UpdateAction();

		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{
		CsDebug.Inst.CharLog(ParentCharacter, "Stop executing Guard");
		_executionStopped = true;
		if(ParentCharacter.CurrentStance == HumanStances.Sprint)
		{
			ParentCharacter.CurrentStance = HumanStances.Run;
		}
		ParentCharacter.SendCommand(CharacterCommands.Idle);
		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;

	}

	public override bool AbortAction (float priority)
	{
		if(ParentCharacter.MyAI.BlackBoard.GuardLevel > 0)
		{
			StopAction();
			return true;
		}
		else
		{
			return false;
		}
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
		if(ParentCharacter.MyAI.BlackBoard.GuardLevel <= 0)
		{
			return true;
		}

		if(ParentCharacter.MyAI.BlackBoard.HighestPersonalThreat > 0 || ParentCharacter.MyAI.BlackBoard.TargetEnemy != null || ParentCharacter.MyAI.BlackBoard.InvisibleEnemy != null)
		{
			return false;
		}

		return true;
	}

	public void UpdateAction()
	{
		if(!CheckAvailability() || _executionStopped)
		{
			return;
		}

		//check if is at guarding position
		if(Vector3.Distance(ParentCharacter.transform.position, ParentCharacter.MyAI.BlackBoard.PatrolLoc) > 1)
		{
			CsDebug.Inst.CharLog(ParentCharacter, "Guard current stance is " + ParentCharacter.CurrentStance + " " + ParentCharacter.name);
			//run to guarding position
			if(ParentCharacter.CurrentStance == HumanStances.Walk)
			{
				ParentCharacter.CurrentStance = HumanStances.Run;
			}

			ParentCharacter.SendCommand(CharacterCommands.StopAim);
			ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.PatrolLoc;
			ParentCharacter.SendCommand(CharacterCommands.GoToPosition);

			if(Vector3.Distance(ParentCharacter.transform.position, ParentCharacter.MyAI.BlackBoard.PatrolLoc) < 3)
			{
				ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAround, ParentCharacter.MyAI.BlackBoard.GuardDirection);
			}
			else
			{
				ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAhead, Vector3.zero);
			}
		}
		else
		{
			//look around
			//Debug.Log(ParentCharacter.Name + " is guarding in place");
			ParentCharacter.MyAI.BlackBoard.GuardLevel = 2;
			ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAround, ParentCharacter.MyAI.BlackBoard.GuardDirection);
			if(ParentCharacter.MyAI.BlackBoard.TargetEnemy == null)
			{
				ParentCharacter.SendCommand(CharacterCommands.StopAim);
			}
		}

		if(CheckActionCompletion() && ParentCharacter.MyAI.BlackBoard.GuardLevel > 0)
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
		else
		{
			return true;
		}
	}


}
