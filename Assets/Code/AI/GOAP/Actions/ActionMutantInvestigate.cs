using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionMutantInvestigate: GoapAction 
{
	private bool _hasStarted;
	private bool _willInvestigate;

	public ActionMutantInvestigate(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		CsDebug.Inst.CharLog(ParentCharacter, "Start executing Mutant Investigate " + ParentCharacter.name);

		_hasStarted = false;
		_willInvestigate = false;
		_executionStopped = false;
		UpdateAction();

		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{
		CsDebug.Inst.CharLog(ParentCharacter, "Stop executing Investigate " + ParentCharacter.name);
		_executionStopped = true;
		ParentCharacter.MyAI.WorkingMemory.RemoveFact(FactType.Disturbance);
		ParentCharacter.Destination = ParentCharacter.transform.position;
		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;

	}

	public override bool AbortAction (float priority)
	{
		StopAction();
		return true;
	}

	public override bool CheckActionCompletion()
	{
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null || ParentCharacter.MyAI.BlackBoard.InvisibleEnemy != null || ParentCharacter.MyAI.BlackBoard.HighestPersonalThreat > 0)
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
				//when done investigating with no findings, set guard level down
				ParentCharacter.MyAI.BlackBoard.GuardLevel = 1;
				return false;
			}
		}

		return true;
	}


	public override bool CheckContextPrecondition ()
	{
		//check if there's current target or personal threat
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null || ParentCharacter.MyAI.BlackBoard.InvisibleEnemy != null || ParentCharacter.MyAI.BlackBoard.HighestPersonalThreat > 0)
		{
			//Debug.Log("Checking investigate context precondition, target enemy or personal threat present");
			return false;
		}

		//now if highest disturbance >= 1 then check if there's focused weapon
		if(ParentCharacter.MyAI.BlackBoard.HighestDisturbanceThreat >= 1 && ParentCharacter.MyAI.BlackBoard.FocusedWeapon == null)
		{
			//Debug.Log("Checking investigate context precondition, focused weapon is null");
			//return false;
		}

		//check if disturbance is within range
		if(ParentCharacter.MyAI.BlackBoard.HighestDisturbanceThreat < 0.8f &&
			!AI.IsPositionInArea(ParentCharacter.MyAI.BlackBoard.HighestDisturbanceLoc, 
				ParentCharacter.MyAI.BlackBoard.PatrolLoc,
				ParentCharacter.MyAI.BlackBoard.CombatRange))
		{
			//Debug.Log("disturbance is outside range. disturbance threat is " + ParentCharacter.MyAI.BlackBoard.HighestDisturbanceThreat);
			return false;
		}

		//check if other squad members are doing the same thing
		if(ParentCharacter.MyAI.Squad != null && ParentCharacter.MyAI.Squad.IsAnyOneInvestigating(ParentCharacter.MyAI.BlackBoard.HighestDisturbanceLoc) &&
			ParentCharacter.MyAI.BlackBoard.HighestDisturbanceThreat < 1)
		{
			//Debug.Log("someone else is doing it " + ParentCharacter.name);
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

		if(!_hasStarted)
		{
			//Debug.Log("is in area " + ParentCharacter.MyAI.BlackBoard.HighestDisturbanceLoc + " " + ParentCharacter.MyAI.BlackBoard.PatrolLoc + " " + ParentCharacter.MyAI.BlackBoard.CombatRange);
			bool isInArea = AI.IsPositionInArea(ParentCharacter.MyAI.BlackBoard.HighestDisturbanceLoc, 
				ParentCharacter.MyAI.BlackBoard.PatrolLoc,
				ParentCharacter.MyAI.BlackBoard.CombatRange);


			bool isPlayerPartyMember = GameManager.Inst.PlayerControl.Party.Members.Contains(ParentCharacter.GetComponent<HumanCharacter>());


			//Debug.Log("Investigating... is in area? " + isInArea + ", disturbance threat " + ParentCharacter.MyAI.BlackBoard.HighestDisturbanceThreat);
			/** threats
			 * 0 - look at direction
			 * 1 - not alarmed, walk to check
			 * 2 - alarmed, run to check
			 * 
			 * 0 - when threat < 0.3 or when disturb is outside combat range
			 * 1 - when threat > 0.3 and < 0.6 and disturb is inside combat range or inside immediate distance
			 * 2 - when threat > 0.6
			**/

			/*
			if(isPlayerPartyMember)
			{
				_willInvestigate = false;
				Vector3 disturbDist = ParentCharacter.MyAI.BlackBoard.HighestDisturbanceLoc - ParentCharacter.transform.position;
				ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAround, disturbDist);
			}
			*/

			float disturbThreat = ParentCharacter.MyAI.BlackBoard.HighestDisturbanceThreat;
			Vector3 disturbDist = ParentCharacter.MyAI.BlackBoard.HighestDisturbanceLoc - ParentCharacter.transform.position;
			bool isImmediateDistance = disturbDist.magnitude < 15;
			if(disturbThreat > 0.6f)
			{
				//Debug.Log("Investigating: run to check " + ParentCharacter.name);
				//weapon drawn, flashlight on, run to check
				_willInvestigate = true;
				ParentCharacter.MyAI.BlackBoard.GuardLevel = 2;
				ParentCharacter.SendCommand(CharacterCommands.SetAlert);
				ParentCharacter.CurrentStance = HumanStances.Run;


				ParentCharacter.SendCommand(CharacterCommands.StopAim);

				ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.HighestDisturbanceLoc;
				ParentCharacter.SendCommand(CharacterCommands.GoToPosition);

			}
			else 
			{
				if((isInArea || isImmediateDistance) && disturbThreat >= 0.3f)
				{
					if(disturbThreat < 0.6f)
					{
						//weapon holstered, walk to check
						ParentCharacter.SendCommand(CharacterCommands.SetAlert);
						_willInvestigate = true;
						ParentCharacter.CurrentStance = HumanStances.Walk;
						ParentCharacter.SendCommand(CharacterCommands.StopAim);
					}


					ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.HighestDisturbanceLoc;
					ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
				}
				else
				{
					//only look at direction
					_willInvestigate = false;
					ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAround, disturbDist);
				}
			}



			_hasStarted = true;
		}

		if(Vector3.Distance(ParentCharacter.transform.position, ParentCharacter.Destination.Value) < 1 && _willInvestigate)
		{
			ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAround, Vector3.zero);

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
