using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionEquipWeapon : GoapAction
{


	public ActionEquipWeapon(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}


	public override bool CheckActionCompletion ()
	{
		if(ParentCharacter.MyReference.CurrentWeapon != null)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public override bool ExecuteAction ()
	{
		//if there's no focused weapon, then it means character has no weapon and action fails
		/*
		if(ParentCharacter.MyAI.BlackBoard.FocusedWeapon == null)
		{
			WorkingMemoryFact fact = ParentCharacter.MyAI.WorkingMemory.AddFact(FactType.FailedAction, null, Vector3.zero, 1, 0.1f);
			fact.PastAction = this.Name;
			return false;
		}
		*/



		CsDebug.Inst.CharLog(ParentCharacter, "Start equip weapon action " + ParentCharacter.name);

		UpdateAction();

		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnActionUpdateTimer += UpdateAction;



		return true;
	}

	public override void StopAction ()
	{
		ParentCharacter.MyEventHandler.OnActionUpdateTimer -= UpdateAction;

	}

	public override bool AbortAction (float priority)
	{
		return false;
	}

	public void UpdateAction()
	{
		//continue to check if body is locked, i.e. wait till it's not locked
		if(!CheckAvailability())
		{
			return;
		}

		if(ParentCharacter.ActionState != HumanActionStates.SwitchWeapon && ParentCharacter.MyReference.CurrentWeapon == null)
		{
			ParentCharacter.SendCommand(ParentCharacter.MyAI.WeaponSystem.GetBestWeaponChoice());
		}

		if(CheckActionCompletion())
		{
			StopAction();

			ParentCharacter.MyEventHandler.TriggerOnActionCompletion();
		}



	}


	private bool CheckAvailability()
	{
		//check if body is locked
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
