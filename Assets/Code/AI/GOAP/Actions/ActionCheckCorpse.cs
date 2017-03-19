using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionCheckCorpse: GoapAction 
{
	private float _checkTimer;

	public ActionCheckCorpse(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		if(ParentCharacter.MyAI.BlackBoard.TargetCorpse == null || ParentCharacter.MyAI.BlackBoard.TargetCorpse.ThreatLevel <= 0)
		{
			return false;
		}

		Debug.Log("Start executing check corpse " + ParentCharacter.name);


		_checkTimer = 0;

		UpdateAction();

		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{
		Debug.Log("Stop executing check corpse " + ParentCharacter.name);
		_executionStopped = true;
		ParentCharacter.MyAnimator.SetBool("IsChecking", false);
		if(ParentCharacter.MyAI.BlackBoard.TargetCorpse != null)
		{
			ParentCharacter.MyAI.BlackBoard.TargetCorpse.ThreatLevel = 0;
			ParentCharacter.MyAI.BlackBoard.TargetCorpse = null;
		}
		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;

	}

	public override bool AbortAction (float priority)
	{
		StopAction();
		return true;
	}

	public override bool CheckActionCompletion()
	{
		if(ParentCharacter.MyAI.BlackBoard.TargetCorpse == null)
		{
			return true;
		}

		if(ParentCharacter.MyAI.BlackBoard.TargetCorpse.ThreatLevel <= 0)
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
		//check if there's current target or personal threat
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null || ParentCharacter.MyAI.BlackBoard.HighestPersonalThreat > 0 || ParentCharacter.MyAI.BlackBoard.InvisibleEnemy != null)
		{
			//Debug.Log("Checking check corpose context precondition, target enemy or personal threat present");
			return false;
		}

		if(ParentCharacter.MyAI.BlackBoard.TargetCorpse != null && ParentCharacter.MyAI.BlackBoard.TargetCorpse.ThreatLevel <= 0)
		{
			return false;
		}

		if(ParentCharacter.MyAI.BlackBoard.TargetCorpse != null)
		{
			//check if other squad members are doing the same thing
			if(ParentCharacter.MyAI.Squad.IsAnyOneInvestigating(ParentCharacter.MyAI.BlackBoard.TargetCorpse.LastKnownPos))
			{
				Debug.Log("someone else is doing it, I am " + ParentCharacter.name);
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

		if(ParentCharacter.MyAI.BlackBoard.TargetCorpse != null)
		{
			Vector3 distance = ParentCharacter.MyAI.BlackBoard.TargetCorpse.LastKnownPos - ParentCharacter.transform.position;
			if(distance.magnitude > 2)
			{
				ParentCharacter.CurrentStance = HumanStances.Run;
				ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAhead, Vector3.zero);
				Vector3 dest = ParentCharacter.MyAI.BlackBoard.TargetCorpse.LastKnownPos - distance.normalized * 0.5f;
				ParentCharacter.Destination = dest;
				ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
			}
			else
			{
				if(ParentCharacter.MyAI.IsCharacterFriendly((Character)ParentCharacter.MyAI.BlackBoard.TargetCorpse.Target))
				{
					//if corpse and I are friendly, then I'll check corpse and then raise alert
					if(ParentCharacter.GetCharacterVelocity().magnitude <= 0.1f)
					{
						ParentCharacter.MyAnimator.SetBool("IsChecking", true);
					}

					_checkTimer ++;

					if(_checkTimer > 3)
					{
						//notify everyone on the team to draw weapon and do random patrol
						ParentCharacter.MyAI.Bark("Somebody killed him! Search perimeter!");
						ParentCharacter.MyAI.BlackBoard.GuardLevel = 3;
						ParentCharacter.SendDelayCallBack(2, ParentCharacter.MyAI.Squad.SetSquadAlertLevel, 3);
						ParentCharacter.MyAI.Squad.BroadcastMemoryFact(ParentCharacter.MyAI.BlackBoard.TargetCorpse);
						StopAction();

						ParentCharacter.MyEventHandler.TriggerOnActionCompletion();
						return;
					}
				}
				else
				{
					//not in same faction, will consider it as a container to loot
				}
			}

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
