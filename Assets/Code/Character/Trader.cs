using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TraderData
{
	public string CharacterID;
	public int Cash;
	public int Tier;
	public List<GridItemData> TraderInventory;
	public float SupplyRenewTimer;
}

public class Trader : MonoBehaviour 
{
	public int Cash;
	public int Tier;
	public ItemType [] SupplyTypes;
	public ItemType [] DemandTypes;
	public List<GridItemData> TraderInventory;

	public float SupplyRenewTimer { get { return _supplyRenewTimer; } set { _supplyRenewTimer = value; }}

	private float _supplyRenewTimer;


	public void Initialize()
	{
		if(SupplyTypes == null)
		{
			SupplyTypes = new ItemType[0];
		}

		if(DemandTypes == null)
		{
			DemandTypes = new ItemType[0];
		}

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
		foreach(GridItemData saleItem in TraderInventory)
		{
			if(item.ID == saleItem.Item.ID)
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
		foreach(GridItemData saleItem in TraderInventory)
		{
			if(item.ID == saleItem.Item.ID)
			{
				finalPrice = item.BasePrice * 0.1f;
			}
		}

		foreach(ItemType type in DemandTypes)
		{
			if(item.Type == type)
			{
				finalPrice = item.BasePrice * 0.5f;
			}
		}

		if(item.MaxDurability > 1f)
		{
			finalPrice *= (item.Durability / item.MaxDurability);
		}

		return finalPrice;
	}


	public void GenerateSupply()
	{
		TraderInventory = GameManager.Inst.ItemManager.GetTraderInventory(SupplyTypes, Tier);
	}

}
