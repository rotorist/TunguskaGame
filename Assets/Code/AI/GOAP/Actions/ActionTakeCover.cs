using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionTakeCover : GoapAction 
{
	private bool _isCoverFound;

	public ActionTakeCover(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		Debug.Log("Start executing Take Cover" + ParentCharacter.name);

		_isCoverFound = false;


		//get a list of all covers 
		GameObject [] allCovers = GameObject.FindGameObjectsWithTag("Cover");
		//if cover is nearby, try searching for a spot in navmesh that's behind cover from personal threat direction
		foreach(GameObject cover in allCovers)
		{

			//make sure the cover is behind player
			Vector3 coverDir = ParentCharacter.MyAI.BlackBoard.AvgPersonalThreatDir;
			//if there's no threat dir, use target enemy dir
			if(coverDir == Vector3.zero && ParentCharacter.MyAI.BlackBoard.TargetEnemy != null)
			{
				coverDir = (ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position - ParentCharacter.transform.position).normalized;
			}

			float angle = Vector3.Angle(coverDir, cover.transform.position - ParentCharacter.transform.position);
			if(Vector3.Distance(ParentCharacter.transform.position, cover.transform.position) < 10 && angle >= 90)
			{
				Vector3 searchCenter = cover.transform.position - coverDir * 1;
				Vector3 result;
				if(AI.RandomPoint(searchCenter, new Vector3(0.5f, 1, 0.5f), out result))
				{
					_isCoverFound = true;
					ParentCharacter.MyAI.BlackBoard.SelectedCover = cover.GetComponent<Cover>();
					ParentCharacter.MyAI.BlackBoard.SelectedCoverLoc = result;
					break;
				}
				else 
				{
					continue;
				}
			}
		}

		if(!_isCoverFound)
		{
			//mark this action as impossible and stop action
			Debug.Log("Cover not found!");
			ParentCharacter.MyAI.WorkingMemory.AddFact(FactType.FailedAction, this.Name, Vector3.zero, 1, 0.4f);
			return false;
		}

		ParentCharacter.MyAI.Bark("I'm hurt! Help me!");
		ParentCharacter.MyAI.CallForHelp(null);

		UpdateAction();

		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnActionUpdateTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{
		Debug.Log("Stop executing Take cover");

		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
		ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = false;

	}

	public override bool AbortAction (float priority)
	{
		if(priority >= 1)
		{
			//mark this action as impossible and stop action
			ParentCharacter.MyAI.WorkingMemory.AddFact(FactType.FailedAction, this.Name, Vector3.zero, 1, 0.4f);
			ParentCharacter.SendCommand(CharacterCommands.Idle);
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
		if(!CheckAvailability())
		{
			return;
		}

		if(ParentCharacter.MyReference.CurrentWeapon == null)
		{
			ParentCharacter.SendCommand(ParentCharacter.MyAI.WeaponSystem.GetBestWeaponChoice());

		}

		if(_isCoverFound)
		{

				ParentCharacter.SendCommand(CharacterCommands.StopAim);
				ParentCharacter.SendCommand(CharacterCommands.StopCrouch);
				ParentCharacter.GetComponent<HumanCharacter>().CurrentStance = HumanStances.Sprint;


			ParentCharacter.MyAI.BlackBoard.NavTarget = ParentCharacter.MyAI.BlackBoard.SelectedCoverLoc;
			//GameObject.Find("Marker1").transform.position = ParentCharacter.MyAI.BlackBoard.SelectedCoverLoc;
			//GameObject.Find("Sphere").transform.position = ParentCharacter.MyAI.BlackBoard.SelectedCover.transform.position;
			ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.NavTarget;
			ParentCharacter.SendCommand(CharacterCommands.GoToPosition);

			ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAround, ParentCharacter.MyAI.BlackBoard.AvgPersonalThreatDir);
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
		else
		{
			return true;
		}
	}


}
