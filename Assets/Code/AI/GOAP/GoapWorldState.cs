using UnityEngine;
using System.Collections;
using System;

public class GoapWorldState
{
	public int ID;
	public string Name;
	public WorldStateOperator Operator;
	public object Value;


	public GoapWorldState(int id, string name, WorldStateOperator op, object value)
	{
		ID = id;
		Name = name;
		Operator = op;
		Value = value;
	}



	public static bool Compare(GoapWorldState state1, GoapWorldState state2)
	{
		//CsDebug.Inst.Log("GoapWorldState/Compare: Comparing state1 " + state1.Name + " " + state1.Operator + " " + state1.Value + 
		//				" vs state 2 "+ state2.Name + " " + state2.Operator + " " + state2.Value, CsDLevel.Trace, CsDComponent.AI);
		//returns true if state1 includes or equals to state2
		if(state1.ID != state2.ID)
		{
			return false;
		}


		switch(state1.Operator)
		{
		case WorldStateOperator.Equals:
			return state1.Value.Equals(state2.Value);
			break;
		case WorldStateOperator.NotEquals:
			return state1.Value.Equals(state2.Value);
			break;
		case WorldStateOperator.Greater:
			return Convert.ToSingle(state1.Value) < Convert.ToSingle(state2.Value);
			break;
		case WorldStateOperator.Less:
			return Convert.ToSingle(state1.Value) > Convert.ToSingle(state2.Value);
			break;
		}

		return false;
	}

	public bool CombineValue(GoapWorldState targetState)
	{
		//tries to combine the value of target state to this state. returns true if combine successful. returns false if combine unsuccessful and there's conflict

		//if the value and operator combination of state is broader or equal to set, then we can assign the value
		//of state to s. If state's value is less broad or not equal, then we have a conflict.
		//if state's value is less braod than set, then we keep the value of s.

		//CsDebug.Inst.Log("GoapWorldState/CombineValue: checking target state " + targetState.Name + " = " + targetState.Value 
		//				+ "vs myself " + Name + " = " + Value, CsDLevel.Trace, CsDComponent.AI);
		if(Operator != targetState.Operator)
		{
			return false;
		}

		switch(Operator)
		{
		case WorldStateOperator.Equals:
			return Value.Equals(targetState.Value);
			break;
		case WorldStateOperator.NotEquals:
			return Value.Equals(targetState.Value);
			break;
		case WorldStateOperator.Greater:
			if(Convert.ToSingle(targetState.Value) < Convert.ToSingle(Value))
			{
				Value = targetState.Value;
			}
			return true;
			break;
		case WorldStateOperator.Less:
			if(Convert.ToSingle(targetState.Value) > Convert.ToSingle(Value))
			{
				Value = targetState.Value;
			}
			return true;
			break;
		}

		return false;
	}
}

public enum WorldStateOperator
{
	Equals,
	NotEquals,
	Greater,
	Less,
}