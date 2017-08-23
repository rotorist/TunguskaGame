using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class StoryConditionItem : StoryCondition
{
	public string ItemID;
	public bool IsForDurability;

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

		if(!IsForDurability)
		{
			int value = GetValue();

			return StaticUtility.CompareIntWithOp(value, compareValue, op);
		}
		else
		{
			//get a list of all items with ItemID
			//see if any of them has durability percent greater than DurabilityPercent
			float desiredDurabilityPercent = Convert.ToSingle(compareValue) / 100f;
			HumanCharacter player = GameManager.Inst.PlayerControl.SelectedPC;
			List<GridItemData> items = player.Inventory.FindItemsInBackpack(ItemID);
			foreach(GridItemData item in items)
			{
				float durability = item.Item.Durability / item.Item.MaxDurability;
				if(durability >= desiredDurabilityPercent)
				{
					return true;
				}
			}

		}

		return false;
	}

}
