using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionMutantBite : GoapAction
{
	private enum BiteState
	{
		None,
		Goto,
		Bite,
	}
	private BiteState _biteState;
	private float _dist;

	public ActionMutantBite(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		CsDebug.Inst.CharLog(ParentCharacter, "Start executing ActionMutantBite " + ParentCharacter.name);

		_executionStopped = false;
		_biteState = BiteState.None;




		UpdateAction();

		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;
		ParentCharacter.MyEventHandler.OnPerFrameTimer -= PerFrameUpdate;
		ParentCharacter.MyEventHandler.OnPerFrameTimer += PerFrameUpdate;

		ParentCharacter.MyAnimEventHandler.OnEndStrangle -= OnEndStrangle;
		ParentCharacter.MyAnimEventHandler.OnEndStrangle += OnEndStrangle;




		return true;
	}

	public override void StopAction()
	{
		CsDebug.Inst.CharLog(ParentCharacter, "Stop executing ActionMutantBite " + ParentCharacter.name);
		_executionStopped = true;
		_biteState = BiteState.None;

		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnPerFrameTimer -= PerFrameUpdate;

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
		if(ParentCharacter.MyAI.BlackBoard.GuardLevel == 0)
		{
			return true;
		}

		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy == null)
		{
			return true;
		}
		else
		{
			float facingAngle = ParentCharacter.MyAI.GetEnemyFacingAngle(ParentCharacter.MyAI.BlackBoard.TargetEnemy);
			if(facingAngle >= 70)
			{
				return true;
			}
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

	public override float GetActionCost ()
	{
		float facingAngle = ParentCharacter.MyAI.GetEnemyFacingAngle(ParentCharacter.MyAI.BlackBoard.TargetEnemy);
		if(facingAngle >= 70)
		{
			return 5;
		}
		else
		{
			return UnityEngine.Random.Range(1f, 6f);
		}
	}

	public override bool CheckContextPrecondition ()
	{

		//Debug.Log("ranged attack precondition; enemy threat is " + ParentCharacter.MyAI.BlackBoard.TargetEnemyThreat);
		if(ParentCharacter.MyAI.BlackBoard.GuardLevel == 0)
		{
			return false;
		}

		Character target = ParentCharacter.MyAI.BlackBoard.TargetEnemy;

		if(target == null)
		{
			return false;
		}

		//check if target's back is facing me
		float facingAngle = ParentCharacter.MyAI.GetEnemyFacingAngle(ParentCharacter.MyAI.BlackBoard.TargetEnemy);
		if(facingAngle >= 70)
		{
			return false;
		}

		//CsDebug.Inst.CharLog(ParentCharacter, "Checking melee attack precondition, pass");
		return true;

	}


	public void UpdateAction()
	{
		if(!CheckAvailability() || _executionStopped)
		{
			return;
		}

		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null)
		{
			


		}

		if(CheckActionCompletion())
		{
			StopAction();

			ParentCharacter.MyEventHandler.TriggerOnActionCompletion();
		}

	}

	public void PerFrameUpdate()
	{
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null)
		{
			//check if is in range
			_dist = Vector3.Distance(ParentCharacter.transform.position, ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position);

			if(_dist > 1.5f && _biteState != BiteState.Bite)
			{
				//go to target enemy
				ParentCharacter.MyAI.BlackBoard.NavTarget = ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position - ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.forward * 0.5f;
				ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.NavTarget;
				ParentCharacter.CurrentStance = HumanStances.Run;
				ParentCharacter.SendCommand(CharacterCommands.GoToPosition);
			}
			else if(_dist > 1.5f && _biteState == BiteState.Bite)
			{
				ParentCharacter.GetComponent<MutantCharacter>().OnCancelStrangle();
				StopAction();
				ParentCharacter.MyEventHandler.TriggerOnActionCompletion();
			}
			else
			{
				if(_biteState != BiteState.Bite)
				{
					//Debug.Log("Ready to bite");
					ParentCharacter.SendCommand(CharacterCommands.Bite);
					_biteState = BiteState.Bite;
				}
			}



		}
	}

	public void OnEndStrangle()
	{
		StopAction();

		ParentCharacter.MyEventHandler.TriggerOnActionCompletion();
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
