using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ChestData
{
	public string ChestID;
	public List<GridItemData> Items;
	public int ColSize;
	public int RowSize;
}

public class Chest : MonoBehaviour 
{
	public string ChestID;
	public List<GridItemData> Items;
	public int ColSize;
	public int RowSize;

	public string [] PresetItemIDs;
	public int [] PresetItemQuantity;

	public void GenerateContent()
	{
		if(PresetItemIDs.Length > 0 && PresetItemIDs.Length == PresetItemQuantity.Length)
		{
			Items = GameManager.Inst.ItemManager.GeneratePresetChest(PresetItemIDs, PresetItemQuantity, ColSize, RowSize);
		}
		else
		{
			Items = GameManager.Inst.ItemManager.GenerateRandomChestInventory(null, ColSize, RowSize);
		}
	}

	public void PostLoad()
	{
		foreach(GridItemData data in Items)
		{
			data.Item.PostLoad();
		}
	}
}
