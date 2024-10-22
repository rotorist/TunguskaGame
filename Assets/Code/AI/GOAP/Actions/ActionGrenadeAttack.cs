﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionGrenadeAttack : GoapAction
{

	public ActionGrenadeAttack(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		Debug.Log("Start executing Grenade Attack");
		_executionStopped = false;

		Character target = ParentCharacter.MyAI.BlackBoard.InvisibleEnemy;
		if(target == null)
		{
			return false;
		}


		//if standing, check if there's clearance to target
		if(ParentCharacter.CurrentStance != HumanStances.Crouch || ParentCharacter.CurrentStance != HumanStances.CrouchRun)
		{
			
			if(target != null)
			{
				GameObject myEyes = ParentCharacter.MyReference.Eyes;
				RaycastHit hit;
				float colliderHeight = target.GetComponent<CapsuleCollider>().height;
				Vector3 rayTarget = target.transform.position + Vector3.up * colliderHeight * 0.7f;
				Ray ray = new Ray(myEyes.transform.position, rayTarget - myEyes.transform.position);
				if(Physics.Raycast(ray, out hit))
				{
					float dist = Vector3.Distance(hit.point, myEyes.transform.position);
					if(dist < 5)
					{
						return false;
					}


				}
			}
		}

		if(target != null && Vector3.Distance(target.transform.position, ParentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition) > 5)
		{
			return false;
		}

		ParentCharacter.MyAI.TargetingSystem.SetTargetingMode(AITargetingModes.LookAhead, Vector3.zero);


		UpdateAction();

		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{
		Debug.Log("Stop executing Grenade Attack");
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
		//check if I have grenade
		if(ParentCharacter.Inventory.ThrowSlot == null)
		{
			return false;
		}

		if(ParentCharacter.MyAI.BlackBoard.HighestPersonalThreat >= ParentCharacter.MyAI.Sensor.PersonalThreatHigh)
		{
			return false;
		}

		//only throw grenade if enemy is invisible
		if(ParentCharacter.MyAI.BlackBoard.InvisibleEnemy == null)
		{
			return false;
		}

		//roll a dice
		if(UnityEngine.Random.Range(0, 100) > 80)
		{
			return false;
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

		ParentCharacter.MyAI.Bark("Fire in the hole!");

		if(ParentCharacter.MyAI.BlackBoard.InvisibleEnemy != null)
		{
			ParentCharacter.AimPoint = ParentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition;
			ParentCharacter.SendCommand(CharacterCommands.ThrowGrenade);
		}



		StopAction();
		ParentCharacter.MyEventHandler.TriggerOnActionCompletion();

		/*
		if(CheckActionCompletion())
		{
			StopAction();

		}
		*/
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
	} 

}
