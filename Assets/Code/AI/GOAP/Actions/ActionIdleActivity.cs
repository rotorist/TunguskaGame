using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIdleActivity : GoapAction
{
	private bool _hasIdleDests;
	private bool _hasReachedDest;

	private int _switchActivityTime;
	private int _idleTimer;
	private IdleDest _currentIdleDest;



	public ActionIdleActivity(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		Debug.Log("Start executing idle activity" + ParentCharacter.name);
		_executionStopped = false;

		_hasReachedDest = false;
		_idleTimer = 0;
		_switchActivityTime = 1;
		if(ParentCharacter.MyAI.Squad.Household.IdleDests.Count <= 0)
		{
			_hasIdleDests = false;
		}
		else
		{
			_hasIdleDests = true;
		}

		UpdateAction();

		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{
		Debug.Log("Stopping idle activity " + ParentCharacter.name);
		ParentCharacter.SendCommand(CharacterCommands.Idle);
		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;

		//reset animations
		ResetAnimation();

		//reset idle dest
		if(_currentIdleDest != null)
		{
			_currentIdleDest.IsOccupied = false;
		}
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

	public override bool CheckContextPrecondition ()
	{
		if(ParentCharacter.MyAI.Squad == null)
		{
			return false;
		}



		//CsDebug.Inst.CharLog(ParentCharacter, "Checking idle activity precondition, pass");
		return true;

	}

	public void UpdateAction()
	{
		if(!CheckAvailability() || _executionStopped)
		{
			return;
		}



		if(_hasIdleDests)
		{
			if(_currentIdleDest == null || _idleTimer >= _switchActivityTime)
			{
				//find next idle activity

				List<IdleDest> dests = ParentCharacter.MyAI.Squad.Household.IdleDests;

				int rnd = UnityEngine.Random.Range(0, dests.Count);
				if(dests[rnd] == _currentIdleDest)
				{
					//no change, just reset the timers
					//_switchActivityTime = UnityEngine.Random.Range(5, 20);
					//_idleTimer = 0;
				}
				else
				{
					//reset animation and walk to the new dest
					_currentIdleDest = dests[rnd];
					ResetAnimation();
					ParentCharacter.CurrentStance = HumanStances.Walk;
					ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAhead, Vector3.zero);
					ParentCharacter.Destination = _currentIdleDest.transform.position;
					ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
					_idleTimer = 0;
					_switchActivityTime = UnityEngine.Random.Range(5, 10);
					_currentIdleDest.IsOccupied = false;
					_hasReachedDest = false;
				}
			}
			else if(_currentIdleDest != null)
			{
				//check if near dest
				if(ParentCharacter.MyNavAgent.remainingDistance <= ParentCharacter.MyNavAgent.stoppingDistance && !_hasReachedDest)
				{
					ParentCharacter.transform.position = _currentIdleDest.transform.position;
					Vector3 lookDir = _currentIdleDest.transform.forward;
					lookDir = new Vector3(lookDir.x, 0, lookDir.z);
					ParentCharacter.transform.rotation = Quaternion.LookRotation(lookDir);
					if(_currentIdleDest.Type == IdleDestType.ChairSit)
					{
						ParentCharacter.MyAI.BlackBoard.AnimationAction = AnimationActions.ChairSit;
						ParentCharacter.SendCommand(CharacterCommands.PlayAnimationAction);
						_currentIdleDest.IsOccupied = true;
					}

					_hasReachedDest = true;
				}
			}
		}
		else
		{
			//simply find a location around household to stand or sit
		}

		_idleTimer ++;

		/*
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

		//ParentCharacter.CurrentStance = HumanStances.Run;
		ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAheadAround, Vector3.zero);
		ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.PatrolLoc;
		ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
		*/

		if(CheckActionCompletion())
		{
			StopAction();
			ParentCharacter.MyEventHandler.TriggerOnActionCompletion();
		}

	}




	private bool CheckAvailability()
	{
		return true;
	}

	private void ResetAnimation()
	{
		ParentCharacter.SendCommand(CharacterCommands.AnimationActionDone);
	}
}
