using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoapAction 
{
	public float Cost;
	public string Name;
	public string Description;
	public Character ParentCharacter;
	//public GoapGoal Goal;
	protected List<GoapWorldState> _preconditions;
	protected List<GoapWorldState> _effects;
	protected bool _executionStopped;
	protected bool _readyForCompletion;

	public List<GoapWorldState> Preconditions 
	{
		get { return _preconditions; }
	}



	public List<GoapWorldState> Effects
	{
		get { return _effects; }
	}



	public GoapAction()
	{
		
	}

	public virtual bool ExecuteAction()
	{
		return true;
	}

	public virtual void StopAction()
	{
		return;
	}

	public virtual bool AbortAction(float priority)
	{
		return true;
	}

	public virtual bool CheckActionCompletion()
	{
		return true;
	}

	public virtual bool CheckContextPrecondition()
	{
		return true;
	}

	public virtual float GetActionCost()
	{
		return Cost;
	}

	/*
	public void AddContextPrecondition(List<GoapWorldState> conditions)
	{
		_contextPreconditions.AddRange(conditions);
	}
	*/

	public void AddPrecondition(GoapWorldState condition)
	{
		_preconditions.Add(condition);
	}

	public void AddPrecondition(List<GoapWorldState> conditions)
	{
		_preconditions.AddRange(conditions);
	}
		

	public void AddEffect(List<GoapWorldState> effects)
	{
		_effects.AddRange(effects);
	}

	public void AddEffect(GoapWorldState effect)
	{
		_effects.Add(effect);
	}
}
