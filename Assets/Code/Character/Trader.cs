using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TraderData : ScriptableObject
{
	public string CharacterID;
	public int Cash;
	public List<GridItemData> TraderInventory;
	public float SupplyRenewTimer;
}

public class Trader : MonoBehaviour 
{
	public int Cash;
	public ItemType [] SupplyTypes;
	public ItemType [] DemandTypes;
	public List<GridItemData> TraderInventory;

	public float SupplyRenewTimer { get { return _supplyRenewTimer; } set { _supplyRenewTimer = value; }}

	private float _supplyRenewTimer;


	public void Initialize()
	{
		TraderInventory = new List<GridItemData>();
		GenerateSupply();
	}

	public void PostLoad()
	{
		foreach(GridItemData data in TraderInventory)
		{
			data.Item.PostLoad();
		}
	}

	public float GetSellPrice(Item item)
	{
		if(item.ID == "rubles")
		{
			return 1f;
		}

		float finalPrice = item.BasePrice;
		foreach(ItemType type in SupplyTypes)
		{
			if(item.Type == type)
			{
				finalPrice = item.BasePrice * 1.5f;
			}
		}

		return finalPrice;
	}

	public float GetBuyPrice(Item item)
	{
		if(item.ID == "rubles")
		{
			return 1f;
		}

		float finalPrice = item.BasePrice;
		foreach(ItemType type in SupplyTypes)
		{
			if(item.Type == type)
			{
				finalPrice = item.BasePrice * 0.2f;
			}
		}

		foreach(ItemType type in DemandTypes)
		{
			if(item.Type == type)
			{
				finalPrice = item.BasePrice * 2;
			}
		}

		return finalPrice;
	}


	private void GenerateSupply()
	{
		TraderInventory = GameManager.Inst.ItemManager.GetTraderInventory(SupplyTypes, 1);
	}

}
