using UnityEngine;
using System.Collections;

public class StoryConditionItem : StoryCondition
{
	public string ItemID;

	public override void SetValue (int value)
	{
		
	}

	public override int GetValue ()
	{
		int count = GameManager.Inst.PlayerControl.SelectedPC.Inventory.CountItemsInBackpack(ItemID);

		return count;

	}

	public override bool Evaluate (int compareValue, int op)
	{
		if(!IsActive)
		{
			return false;
		}

		int value = GetValue();

		return StaticUtility.CompareIntWithOp(value, compareValue, op);
	}

}
