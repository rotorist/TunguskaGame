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
	private IdleDest _traderIdleDest;


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

		_currentIdleDest = null;
		_hasReachedDest = false;
		_hasIdleDests = false;
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

		foreach(IdleDest dest in ParentCharacter.MyAI.Squad.Household.IdleDests)
		{
			if(dest.Type == IdleDestType.Trade)
			{
				_traderIdleDest = dest;
			}
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
			//if is trader, look if player is nearby
			if(ParentCharacter.MyJobs.Contains(NPCJobs.Trader) && _traderIdleDest != null && _currentIdleDest != _traderIdleDest)
			{
				Vector3 playerDist = _traderIdleDest.transform.position - GameManager.Inst.PlayerControl.SelectedPC.transform.position;
				Vector3 playerDistXZ = new Vector3(playerDist.x, 0, playerDist.z);
				if(playerDistXZ.magnitude < 2 && playerDist.y < 1)
				{
					//reset animation and walk to the new dest
					if(_currentIdleDest != null)
					{	
						_currentIdleDest.IsOccupied = false;
					}
					_currentIdleDest = _traderIdleDest;
					_currentIdleDest.IsOccupied = true;
					ResetAnimation();

					_idleTimer = 0;
					_switchActivityTime = 30;

					_hasReachedDest = false;
				}
			}

			if(_currentIdleDest == null || _idleTimer >= _switchActivityTime)
			{
				//find next idle activity

				List<IdleDest> dests = ParentCharacter.MyAI.Squad.Household.IdleDests;

				int rnd = UnityEngine.Random.Range(0, dests.Count);
				if(dests[rnd] == _currentIdleDest || dests[rnd].IsOccupied)
					// || (_currentIdleDest != null && _currentIdleDest.Type == dests[rnd].Type))
				{
					//no change, try again next update
				}
				else
				{
					//reset animation and walk to the new dest
					if(_currentIdleDest != null)
					{	
						_currentIdleDest.IsOccupied = false;
					}
					_currentIdleDest = dests[rnd];
					_currentIdleDest.IsOccupied = true;
					ResetAnimation();

					_idleTimer = 0;
					_switchActivityTime = UnityEngine.Random.Range(15, 30);

					_hasReachedDest = false;
				}
			}
			else if(_currentIdleDest != null)
			{
				//check if near dest
				if(ParentCharacter.MyNavAgent.remainingDistance <= ParentCharacter.MyNavAgent.stoppingDistance && _hasReachedDest)
				{
					
					if(_currentIdleDest.Type == IdleDestType.ChairSit && !ParentCharacter.MyAnimator.GetBool("IsChairSitting"))
					{
						ParentCharacter.transform.position = _currentIdleDest.transform.position;
						Vector3 lookDir = _currentIdleDest.transform.forward;
						lookDir = new Vector3(lookDir.x, 0, lookDir.z);
						ParentCharacter.transform.rotation = Quaternion.LookRotation(lookDir);
						ParentCharacter.MyAI.BlackBoard.AnimationAction = AnimationActions.ChairSit;
						ParentCharacter.SendCommand(CharacterCommands.PlayAnimationAction);
						_currentIdleDest.IsOccupied = true;
					}
					else if(_currentIdleDest.Type == IdleDestType.GroundSit && !ParentCharacter.MyAnimator.GetBool("IsGroundSitting"))
					{
						ParentCharacter.transform.position = _currentIdleDest.transform.position;
						Vector3 lookDir = _currentIdleDest.transform.forward;
						lookDir = new Vector3(lookDir.x, 0, lookDir.z);
						ParentCharacter.transform.rotation = Quaternion.LookRotation(lookDir);
						ParentCharacter.MyAI.BlackBoard.AnimationAction = AnimationActions.GroundSit;
						ParentCharacter.SendCommand(CharacterCommands.PlayAnimationAction);
						_currentIdleDest.IsOccupied = true;
					}
					else if(_currentIdleDest.Type == IdleDestType.Sleep && !ParentCharacter.MyAnimator.GetBool("IsSleeping") && !ParentCharacter.MyAnimator.GetBool("IsJackingOff"))
					{
						ParentCharacter.transform.position = _currentIdleDest.transform.position;
						Vector3 lookDir = _currentIdleDest.transform.forward;
						lookDir = new Vector3(lookDir.x, 0, lookDir.z);
						ParentCharacter.transform.rotation = Quaternion.LookRotation(lookDir);
						ParentCharacter.MyAI.BlackBoard.AnimationAction = AnimationActions.Sleep;
						ParentCharacter.SendCommand(CharacterCommands.PlayAnimationAction);
						_currentIdleDest.IsOccupied = true;
					}

				}
				else if(!_hasReachedDest)
				{
					ParentCharacter.CurrentStance = HumanStances.Walk;
					ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAhead, Vector3.zero);
					ParentCharacter.Destination = _currentIdleDest.transform.position;
					ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
					if(Vector3.Distance(ParentCharacter.transform.position, _currentIdleDest.transform.position) < ParentCharacter.MyNavAgent.stoppingDistance * 1.5f)
					{
						_hasReachedDest = true;
					}
				}
			}
		}
		else
		{
			//simply find a location around household to stand or sit
		}

		if(_hasReachedDest)
		{
			_idleTimer ++;
		}

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
		if(ParentCharacter.MyAnimator.GetBool("IsChairSitting"))
		{
			ParentCharacter.MyAI.BlackBoard.AnimationAction = AnimationActions.ChairStand;
			ParentCharacter.SendCommand(CharacterCommands.PlayAnimationAction);
		}
		else if(ParentCharacter.MyAnimator.GetBool("IsGroundSitting"))
		{
			ParentCharacter.MyAI.BlackBoard.AnimationAction = AnimationActions.GroundStand;
			ParentCharacter.SendCommand(CharacterCommands.PlayAnimationAction);
		}
		else if(ParentCharacter.MyAnimator.GetBool("IsSleeping") || ParentCharacter.MyAnimator.GetBool("IsJackingOff"))
		{
			ParentCharacter.MyAI.BlackBoard.AnimationAction = AnimationActions.SleepStand;
			ParentCharacter.SendCommand(CharacterCommands.PlayAnimationAction);
		}
		else
		{
			ParentCharacter.SendCommand(CharacterCommands.AnimationActionDone);
		}
	}
}
