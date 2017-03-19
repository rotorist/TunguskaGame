using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionSearchEnemy : GoapAction
{
	private bool _isSearchDestSet;
	private Vector3 _searchDest;
	private float _searchTimer;
	private float _searchDuration;
	private float _nextTalkTime;

	public ActionSearchEnemy(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		if(ParentCharacter.MyAI.BlackBoard.InvisibleEnemy == null)
		{
			return false;
		}
			
		CsDebug.Inst.CharLog(ParentCharacter, "Start executing Search Enemy");

		ParentCharacter.MyAI.BlackBoard.NavTarget = ParentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition;
		ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = true;

		WorkingMemoryFact fact = ParentCharacter.MyAI.WorkingMemory.FindExistingFact (FactType.KnownEnemy, ParentCharacter.MyAI.BlackBoard.InvisibleEnemy);
		float threat = fact.ThreatLevel;

		//if threat is low, just run towards last known location
		//if threat is high, keep aiming and go carefully towards a distance away from the last known location
		CsDebug.Inst.CharLog(ParentCharacter, "Action search enemy threat is " + threat);
		if (threat >= 0.66f)
		{

			Vector3 dist = ParentCharacter.MyAI.BlackBoard.InvisibleEnemy.transform.position - ParentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition;

			_isSearchDestSet = SelectSearchDestination(ParentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition + dist.normalized * -4, new Vector3(3, 3, 3), out _searchDest);



			((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Walk;


		}
		else
		{

			_searchDest = ParentCharacter.MyAI.BlackBoard.NavTarget;
			_isSearchDestSet = true;
			ParentCharacter.SendCommand(CharacterCommands.StopAim);
			((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Run;
		}

		ParentCharacter.MyAI.BlackBoard.GuardLevel = 2;

		if(ParentCharacter.ActionState != HumanActionStates.SwitchWeapon && ParentCharacter.MyReference.CurrentWeapon == null)
		{
			ParentCharacter.SendCommand(CharacterCommands.SetAlert);
		}


		_searchDuration = UnityEngine.Random.Range(20, 40);
		_searchTimer = 0;
		_nextTalkTime = UnityEngine.Random.Range(3, 7);
			
		UpdateAction();

		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnActionUpdateTimer += UpdateAction;

		return true;
	}

	public override void StopAction()
	{
		CsDebug.Inst.CharLog(ParentCharacter, "Search enemy is completed!");
		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;

		ParentCharacter.MyAI.TargetingSystem.Mode = AITargetingModes.LookAhead;
		((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Walk;
		//ParentCharacter.SendCommand(CharacterCommands.StopAim);
		ParentCharacter.SendCommand(CharacterCommands.Idle);



	}

	public override bool AbortAction (float priority)
	{
		if(priority > 0.9f)
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
		CsDebug.Inst.CharLog(ParentCharacter, "Checking completion for search enemy");

		if(_searchTimer > _searchDuration)
		{
			return true;
		}

		//if both enemy target and invisible targets are null, complete
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy == null && ParentCharacter.MyAI.BlackBoard.InvisibleEnemy == null)
		{
			CsDebug.Inst.CharLog(ParentCharacter, "Both target enemy and invisible enemy are null");
			return true;
		}

		foreach(GoapWorldState state in Effects)
		{

			object result = ParentCharacter.MyAI.EvaluateWorldState(state);
			//CsDebug.Inst.CharLog(ParentCharacter, "Checking if state " + state.Name + " value is " + state.Value + " result: " + result);
			if(!result.Equals(state.Value))
			{
				CsDebug.Inst.CharLog(ParentCharacter, "result is not equal to effect");
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


		ParentCharacter.SendCommand(CharacterCommands.Aim);

		CsDebug.Inst.CharLog(ParentCharacter, "is search dest set? " + _isSearchDestSet);
		if(_isSearchDestSet)
		{

			if(Vector3.Distance(ParentCharacter.transform.position, _searchDest) > 1)
			{
				ParentCharacter.Destination = _searchDest;
				ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
				CsDebug.Inst.CharLog(ParentCharacter, "Sent command to go to " + ParentCharacter.Destination);
				if(ParentCharacter.GetCharacterVelocity().magnitude <= 0)
				{
					_isSearchDestSet = false;
				}
			}
			else
			{
				_isSearchDestSet = false;
				ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = false;
			}
		}
		else
		{
			//search random locations
			Vector3 searchCenter = ParentCharacter.transform.position;
			Vector3 searchRange = new Vector3(5, 5, 5);
			if(ParentCharacter.MyAI.BlackBoard.InvisibleEnemy != null)
			{
				if(_searchTimer < 10)
				{
					searchCenter = ParentCharacter.MyAI.BlackBoard.InvisibleEnemy.transform.position;
					searchRange += new Vector3(_searchTimer/2, 0, _searchTimer/2);
				}
				else
				{
					searchCenter = ParentCharacter.MyAI.BlackBoard.PatrolLoc;
					searchRange = ParentCharacter.MyAI.BlackBoard.PatrolRange;
				}
			}
				
			ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = SelectSearchDestination(searchCenter, new Vector3(5, 5, 5), out _searchDest);
			_isSearchDestSet = ParentCharacter.MyAI.BlackBoard.IsNavTargetSet;
			if(ParentCharacter.MyAI.BlackBoard.IsNavTargetSet)
			{
				ParentCharacter.MyAI.BlackBoard.NavTarget = _searchDest;
				ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.NavTarget;
				((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Walk;

				ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
				//CsDebug.Inst.CharLog(ParentCharacter, "Sent command to go to " + ParentCharacter.Destination);
				ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAheadAround, Vector3.zero);
			}
		}
			
		if(_searchTimer >= _nextTalkTime)
		{
			ParentCharacter.PlayVocal(VocalType.Search);
			_nextTalkTime = _searchTimer + UnityEngine.Random.Range(5, 10);
		}


		_searchTimer ++;

		//check if patrol is complete
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

	private bool SelectSearchDestination(Vector3 center, Vector3 range, out Vector3 result)
	{
		//first find a random position within range

		if(AI.RandomPoint(center, range, out result))
		{
			return true;
		}

		result = Vector3.zero;
		return false;
	}
}
