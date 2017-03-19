using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionHideInCover: GoapAction 
{
	private bool _isCoverFound;

	public ActionHideInCover(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		Debug.Log("Start executing Hide in Cover " );
		Vector3 faceDir = ParentCharacter.GetCharacterVelocity().normalized * -1;
		ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAround, faceDir);


		ParentCharacter.SendCommand(CharacterCommands.Crouch);


		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnActionUpdateTimer += UpdateAction;

		UpdateAction();




		return true;
	}

	public override void StopAction()
	{
		Debug.Log("Stopping executing Hide in Cover");
		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
		ParentCharacter.SendCommand(CharacterCommands.StopCrouch);
		ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAhead, Vector3.zero);


	}

	public override bool AbortAction (float priority)
	{
		if(priority > 0.5f)
		{
			//mark this action as impossible and stop action
			ParentCharacter.MyAI.WorkingMemory.AddFact(FactType.FailedAction, this.Name, Vector3.zero, 1, 0.2f);
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
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null)
		{
			ParentCharacter.MyAI.WorkingMemory.AddFact(FactType.FailedAction, this.Name, Vector3.zero, 1, 0.2f);
			Debug.Log("Found enemy while in cover, adding failed action");
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
		//return false;
		if(ParentCharacter.MyAI.BlackBoard.HighestPersonalThreat < 1f)
		{
			return false;
		}

		if(ParentCharacter.MyAI.BlackBoard.GuardLevel > 2)
		{
			//return false;
		}

		if(UnityEngine.Random.value < 0.4f)
		{
			return false;
		}


		Debug.Log("Checking ranged attack precondition, pass " + ParentCharacter.name);
		return true;
	}

	public void UpdateAction()
	{
		if(!CheckAvailability())
		{
			return;
		}
			
		ParentCharacter.SendCommand(CharacterCommands.Aim);


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
