using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActionFlankTarget : GoapAction
{
	private Vector3 flankDest;

	public ActionFlankTarget(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		Debug.LogError("Start executing Flank Target " + ParentCharacter.name);
		_executionStopped = false;



		//must either have target or invisible target
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy == null && ParentCharacter.MyAI.BlackBoard.InvisibleEnemy == null)
		{
			return false;
		}

			

		//find a location behind invisible enemy
		Character target = ParentCharacter.MyAI.BlackBoard.InvisibleEnemy;
		if(target == null)
		{
			target = ParentCharacter.MyAI.BlackBoard.TargetEnemy;
		}


		if(Vector3.Distance(target.transform.position, ParentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition) > 5)
		{
			return false;
		}

		Vector3 lineOfSight = ParentCharacter.transform.position - target.transform.position;
		Vector3 direction = Vector3.Cross(lineOfSight, Vector3.up).normalized + -1 * lineOfSight.normalized;
		if(!AI.RandomPoint(target.transform.position + direction.normalized * 15, new Vector3(5, 2, 5), out flankDest))
		{
			StopAction();
		}

		ParentCharacter.SendCommand(CharacterCommands.StopAim);
		ParentCharacter.MyAI.Bark("Anu cheeki breeki\n iv damke!");
		UpdateAction();

		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{
		_executionStopped = true;
		ParentCharacter.MyAI.WorkingMemory.AddFact(FactType.FailedAction, this.Name, Vector3.zero, 1, 0.2f);
		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;

	}

	public override bool AbortAction (float priority)
	{
		if(priority >= 1)
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
		if(Vector3.Distance(ParentCharacter.transform.position, flankDest) < 1)
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

	public override float GetActionCost ()
	{
		return this.Cost + UnityEngine.Random.Range(0, 300) / 100;
	}

	public void UpdateAction()
	{
		if(!CheckAvailability() || _executionStopped)
		{
			return;
		}

		ParentCharacter.CurrentStance = HumanStances.Run;
		ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAheadAround, Vector3.zero);
		ParentCharacter.Destination = flankDest;
		ParentCharacter.SendCommand(CharacterCommands.GoToPosition);

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
		else
		{
			return true;
		}
	}
}
