using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionGoToLocation: GoapAction 
{
	private bool _isCoverFound;

	public ActionGoToLocation(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		Debug.Log("Start executing go to location" + ParentCharacter.name);
		_executionStopped = false;

		UpdateAction();

		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{
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

		return false;
	}

	public override bool CheckActionCompletion()
	{

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

	public void UpdateAction()
	{
		if(!CheckAvailability() || _executionStopped)
		{
			return;
		}

		//check if need to pull out weapon
		if(ParentCharacter.MyAI.BlackBoard.GuardLevel > 1 && ParentCharacter.MyReference.CurrentWeapon == null)
		{
			ParentCharacter.SendCommand(ParentCharacter.MyAI.WeaponSystem.GetBestWeaponChoice());


		}
		else if(ParentCharacter.MyAI.BlackBoard.GuardLevel <= 1 && ParentCharacter.MyReference.CurrentWeapon != null)
		{
			ParentCharacter.SendCommand(CharacterCommands.StopAim);
			ParentCharacter.SendCommand(CharacterCommands.Unarm);
		}

		//check if need to aim down sight
		if(ParentCharacter.MyAI.BlackBoard.GuardLevel > 2)
		{
			ParentCharacter.SendCommand(CharacterCommands.Aim);
		}

		//if is exploring, check if near next node, if so find next node if not at destination
		if(ParentCharacter.MyJobs.Contains(NPCJobs.Explore) && ParentCharacter.IsCommander)
		{
			NavNode nextNode = ParentCharacter.MyAI.Squad.NextNavNode;
			//Debug.Log("GOTO next node is " + nextNode.name + " dist " + Vector3.Distance(ParentCharacter.transform.position, nextNode.transform.position));
			if(nextNode != null)
			{
				if(AI.IsPositionInArea(ParentCharacter.transform.position, nextNode.transform.position, ParentCharacter.MyAI.BlackBoard.PatrolRange))
				{
					NavNode next = AI.FindNextNavNode(nextNode, ParentCharacter.MyAI.Squad.DestNavNode);
					if(next != null)
					{
						ParentCharacter.MyAI.Squad.NextNavNode = next;
						ParentCharacter.MyAI.BlackBoard.PatrolLoc = ParentCharacter.MyAI.Squad.NextNavNode.transform.position;
					}
				}
			}
		}
			
		ParentCharacter.CurrentStance = HumanStances.Walk;
		ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAheadAround, Vector3.zero);
		ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.PatrolLoc;
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
