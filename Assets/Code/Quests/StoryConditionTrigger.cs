using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryConditionTrigger : StoryCondition
{

	public override void SetValue (int value)
	{
		_value = value;
		if(_value != 0)
		{
			_value = 1;
		}
	}

	public override int GetValue ()
	{
		return _value;

	}

	public override bool Evaluate (int compareValue, int op)
	{
		int value = GetValue();
		//Debug.Log("eval condition current value " + _value + " against " + compareValue);
		return StaticUtility.CompareIntWithOp(value, compareValue, op);
	}
}
