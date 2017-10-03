using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionIntimidateTarget : GoapAction
{

	public ActionIntimidateTarget(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy == null)
		{
			return false;
		}
		Debug.Log("Start executing Intimidate Target action" + ParentCharacter.name);

		ParentCharacter.MyAI.BlackBoard.NavTarget = ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position;
		ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = true;
		((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Run;

		ParentCharacter.MyAI.BlackBoard.GuardLevel = 2;


		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnActionUpdateTimer += UpdateAction;

		UpdateAction();

		return true;
	}

	public override void StopAction()
	{
		Debug.Log("Stop executing Intimidate Target action");
		//ParentCharacter.SendCommand(CharacterCommands.StopAim);
		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
	}

	public override bool AbortAction (float priority)
	{
		if(priority > 0.5f)
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
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy == null)
		{
			return true;
		}

		//check if enemy threat level is high
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemyThreat >= 1)
		{
			return true;
		}

		float dist = Vector3.Distance(ParentCharacter.MyAI.BlackBoard.DefensePoint, ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position);
		if(dist > ParentCharacter.MyAI.BlackBoard.DefenseRadius * 2f)
		{
			return true;
		}

		foreach(GoapWorldState state in Effects)
		{

			object result = ParentCharacter.MyAI.EvaluateWorldState(state);
			//Debug.Log("Checking if state " + state.Name + " value is " + state.Value + " result: " + result);
			if(!result.Equals(state.Value))
			{
				//Debug.Log("result is not equal to effect");
				return false;
			}
		}

		return true;
	}

	public override bool CheckContextPrecondition ()
	{
		//here we are going to check if the target in blackboard is worthy of intimidating. Only intimidate if 
		//the target is low/medium threat
		//Debug.Log("Checking precondition for intimidate, target enemy threat is " + ParentCharacter.MyAI.BlackBoard.TargetEnemyThreat + " guard level " +
		//	ParentCharacter.MyAI.BlackBoard.GuardLevel + " I am " + ParentCharacter.name);

		if(ParentCharacter.MyAI.BlackBoard.TargetEnemyThreat >= 0.66f)
		{
			return false;
		}

		//check if enemy target is within defend radius

		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null)
		{
			float dist = Vector3.Distance(ParentCharacter.MyAI.BlackBoard.DefensePoint, ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position);
			if(dist < ParentCharacter.MyAI.BlackBoard.DefenseRadius)
			{
				return false;
			}
		}
		else
		{
			return false;
		}


		return true;
	}




	public void UpdateAction()
	{
		//continue to check if body is locked, i.e. wait till it's not locked
		if(!CheckAvailability())
		{
			return;
		}

		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy!= null && 
			Vector3.Distance(ParentCharacter.transform.position, ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position) > 8 &&
			ParentCharacter.MyAI.BlackBoard.TargetEnemyThreat >= 0.33f && 
			!ParentCharacter.MyJobs.Contains(NPCJobs.Guard) &&
			Vector3.Distance(ParentCharacter.MyAI.BlackBoard.DefensePoint, ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position) < ParentCharacter.MyAI.BlackBoard.DefenseRadius * 2f)
		{
			ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position;
			ParentCharacter.SendCommand(CharacterCommands.StopAim);
			ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
		}
		else
		{
			ParentCharacter.SendCommand(CharacterCommands.Idle);
			ParentCharacter.SendCommand(CharacterCommands.Aim);
		}




		if(CheckActionCompletion())
		{
			StopAction();

			ParentCharacter.MyEventHandler.TriggerOnActionCompletion();
		}
	}

	private bool CheckAvailability()
	{
		//check if body is locked
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
