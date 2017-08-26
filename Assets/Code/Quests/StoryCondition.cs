using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class StoryCondition
{
	public string ID;
	public bool IsActive;
	public StoryConditionType Type;

	protected int _value;

	public abstract void SetValue(int value);
	public abstract int GetValue();
	public abstract bool Evaluate(int compareValue, int op); //for op, -2 means less than compareValue, 2 means greater than compareValue, -1 is less-equal, 1 is greater-equal


}

public enum StoryConditionType
{
	Item,
	Trigger,
}