using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionTakeAttackCover : GoapAction 
{
	private bool _isCoverFound;

	public ActionTakeAttackCover(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		CsDebug.Inst.CharLog(ParentCharacter, "Start executing Take Attack Cover" + ParentCharacter.name);

		_isCoverFound = false;
		List<Cover> candidates = new List<Cover>();

		//get a list of all covers 
		GameObject [] allCovers = GameObject.FindGameObjectsWithTag("Cover");
		//if cover is nearby, try searching for a spot in navmesh that's behind cover from personal threat direction
		foreach(GameObject cover in allCovers)
		{
			//only choose ones for attacking
			if(!cover.GetComponent<Cover>().IsForShooting)
			{
				continue;
			}

			//if is player team, only choose covers very close
			if(ParentCharacter.MyAI.ControlType == AIControlType.PlayerTeam && Vector3.Distance(cover.transform.position, ParentCharacter.transform.position) > 2)
			{
				continue;
			}

			//only choose if distance between cover and agent is shorter than distance between agent and target
			//and if distance between cover and target is greater than 5
			if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null)
			{
				float distAgentCover = Vector3.Distance(cover.transform.position, ParentCharacter.transform.position);
				float distAgentTarget = Vector3.Distance(ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position, ParentCharacter.transform.position);
				float distCoverTarget = Vector3.Distance(ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position, cover.transform.position);

				if(distAgentCover > distAgentTarget || distCoverTarget < 5)
				{
					continue;
				}
			}


			if(Vector3.Distance(ParentCharacter.transform.position, cover.transform.position) < 10)
			{
				_isCoverFound = true;
				candidates.Add(cover.GetComponent<Cover>());


			}
		}

		float minDist = 100000;
		foreach(Cover cover in candidates)
		{
			//find the closest cover
			if(Vector3.Distance(cover.transform.position, ParentCharacter.transform.position) < minDist)
			{
				ParentCharacter.MyAI.BlackBoard.SelectedCover = cover.GetComponent<Cover>();
				minDist = Vector3.Distance(cover.transform.position, ParentCharacter.transform.position);
			}
		}

		if(_isCoverFound)
		{
			Cover selectedCover = ParentCharacter.MyAI.BlackBoard.SelectedCover;
			Debug.Log("found cover " + selectedCover.name);
			Vector3 coverDir = ParentCharacter.MyAI.BlackBoard.AvgPersonalThreatDir;
			//if there's no threat dir, use target enemy dir
			if(coverDir == Vector3.zero && ParentCharacter.MyAI.BlackBoard.TargetEnemy != null)
			{
				coverDir = (ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position - ParentCharacter.transform.position).normalized;
			}

			Vector3 searchCenter = selectedCover.transform.position - coverDir * 1;
			Vector3 result;
			if(AI.RandomPoint(searchCenter, new Vector3(0.5f, 1, 0.5f), out result))
			{
				ParentCharacter.MyAI.BlackBoard.SelectedCoverLoc = result;

			}
			else
			{
				_isCoverFound = false;
			}

		}

		if(!_isCoverFound)
		{
			//mark this action as impossible and stop action
			Debug.Log("Cover not found! " + ParentCharacter.name);
			ParentCharacter.MyAI.WorkingMemory.AddFact(FactType.FailedAction, this.Name, Vector3.zero, 1, 0.4f);
			return false;
		}

		ParentCharacter.MyAI.Bark("Cover me!");
		ParentCharacter.MyAI.CallForHelp(null);
		UpdateAction();

		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnActionUpdateTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{
		Debug.Log("Stop executing Take attack cover " + ParentCharacter.name);
		ParentCharacter.SendCommand(CharacterCommands.Idle);
		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
		ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = false;

	}

	public override bool AbortAction (float priority)
	{
		if(ParentCharacter.MyAI.ControlType != AIControlType.PlayerTeam && priority >= 1)
		{
			//mark this action as impossible and stop action
			ParentCharacter.MyAI.WorkingMemory.AddFact(FactType.FailedAction, this.Name, Vector3.zero, 1, 0.4f);
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

	public override bool CheckContextPrecondition ()
	{
		if(ParentCharacter.MyAI.ControlType == AIControlType.PlayerTeam)
		{
			return true;
		}

		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null 
			&& Vector3.Distance(ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position, ParentCharacter.transform.position) < 5)
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
			

		if(_isCoverFound)
		{

			ParentCharacter.SendCommand(CharacterCommands.StopAim);
			ParentCharacter.GetComponent<HumanCharacter>().CurrentStance = HumanStances.Sprint;
			ParentCharacter.MyAI.BlackBoard.NavTarget = ParentCharacter.MyAI.BlackBoard.SelectedCoverLoc;
			//GameObject.Find("Marker1").transform.position = ParentCharacter.MyAI.BlackBoard.SelectedCoverLoc;
			//GameObject.Find("Sphere").transform.position = ParentCharacter.MyAI.BlackBoard.SelectedCover.transform.position;
			ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.NavTarget;
			ParentCharacter.SendCommand(CharacterCommands.GoToPosition);

		}


		if(CheckActionCompletion())
		{
			StopAction();
			Character target = ParentCharacter.MyAI.BlackBoard.TargetEnemy;
			if(target == null)
			{
				target = ParentCharacter.MyAI.BlackBoard.InvisibleEnemy;
			}

			if(target != null)
			{
				ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAround, target.transform.position - ParentCharacter.transform.position);
			}
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
