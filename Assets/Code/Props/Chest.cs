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

	public void GenerateContent()
	{
		Items = GameManager.Inst.ItemManager.GenerateRandomInventory(null, ColSize, RowSize);
	}

	public void PostLoad()
	{
		foreach(GridItemData data in Items)
		{
			data.Item.PostLoad();
		}
	}
}
