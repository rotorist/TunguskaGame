using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class TradingPanel : PanelBase
{

	public InventoryGrid BuyGrid;
	public InventoryGrid SellGrid;
	public InventoryGrid PlayerBackpack;
	public InventoryGrid TraderItems;
	public UILabel BuyPriceLabel;
	public UILabel SellPriceLabel;
	public UILabel PlayerMoney;
	public UILabel TraderMoney;
	public UILabel PlayerName;
	public UILabel TraderName;

	public int BuyPrice;
	public int SellPrice;

	private Trader _targetTrader;

	public override void Initialize ()
	{
		BuyGrid.Initialize(this);
		SellGrid.Initialize(this);
		Hide();
	}

	public override void PerFrameUpdate ()
	{


	}

	public override void Show ()
	{
		Character target = GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.InteractTarget;
		_targetTrader = null;
		if(target != null)
		{
			_targetTrader = target.GetComponent<Trader>();
		}

		if(target == null || _targetTrader == null)
		{
			return;
		}

		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;

		PlayerName.text = GameManager.Inst.PlayerProgress.PlayerFirstName;
		TraderName.text = target.GetComponent<Character>().Name;

		//first remove all existing griditems in the inventory
		List<GridItem> buyCopy = new List<GridItem>(BuyGrid.Items);
		foreach(GridItem item in buyCopy)
		{
			BuyGrid.Items.Remove(item);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(item);
		}

		//first remove all existing griditems in the inventory
		List<GridItem> sellCopy = new List<GridItem>(SellGrid.Items);
		foreach(GridItem item in sellCopy)
		{
			SellGrid.Items.Remove(item);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(item);
		}


		//update price tag on all inventories
		foreach(GridItem gItem in PlayerBackpack.Items)
		{
			gItem.PriceTag = _targetTrader.GetBuyPrice(gItem.Item);
		}

		foreach(GridItem gItem in TraderItems.Items)
		{
			gItem.PriceTag = _targetTrader.GetSellPrice(gItem.Item);
		}

		UpdateTraderMoneyDisplay();
	}

	public override void Hide ()
	{
		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;

		//if anything is in sell grid, throw them all on the ground
		List<GridItem> sellCopy = new List<GridItem>(SellGrid.Items);
		foreach(GridItem item in sellCopy)
		{
			var resource = Resources.Load(item.Item.PrefabName + "Pickup");
			if(resource != null)
			{
				GameObject pickup = GameObject.Instantiate(resource) as GameObject;
				pickup.transform.position = GameManager.Inst.PlayerControl.SelectedPC.transform.position 
											+ new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 1f, UnityEngine.Random.Range(-0.2f, 0.2f));
				pickup.GetComponent<PickupItem>().Item = item.Item;
				pickup.GetComponent<PickupItem>().Quantity = item.GetQuantity();
			}

			SellGrid.Items.Remove(item);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(item);
		}

		GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.InteractTarget = null;
	}

	public override bool HasInventoryGrids (out List<InventoryGrid> grids)
	{
		grids = new List<InventoryGrid>();
		grids.Add(BuyGrid);
		grids.Add(SellGrid);
		return true;
	}

	public override bool HasBodySlots (out List<BodySlot> bodySlots)
	{
		bodySlots = null;
		return false;
	}

	public override bool HasTempSlots (out List<TempSlot> tempSlots)
	{
		tempSlots = null;
		return false;
	}

	public Trader GetCurrentTrader()
	{
		return _targetTrader;
	}



	public void OnTradeButtonPress()
	{
		//check if trading is possible
		int playerMoney = GameManager.Inst.UIManager.WindowPanel.InventoryPanel.BackpackGrid.GetItemQuantityByID("rubles");
		if(BuyPrice > SellPrice)
		{
			int difference = BuyPrice - SellPrice;

			if(playerMoney < difference)
			{
				return;
			}
			else
			{
				GameManager.Inst.UIManager.WindowPanel.InventoryPanel.BackpackGrid.RemoveItemsByID("rubles", difference);
				_targetTrader.Cash += difference;
			}
		}

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("Trade"), 0.2f);


		//update inventory panel to player inventory
		HumanCharacter player = GameManager.Inst.PlayerControl.SelectedPC;
		GameManager.Inst.UIManager.WindowPanel.InventoryPanel.SaveInventoryData(player);

		//first remove all items from sell
		List<GridItem> sellCopy = new List<GridItem>(SellGrid.Items);
		foreach(GridItem item in sellCopy)
		{
			SellGrid.Items.Remove(item);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(item);
		}


		//then take each one of the bought items and insert into backpack;
		//if anything won't fit, place it in the sell grid
		List<GridItem> buyCopy = new List<GridItem>(BuyGrid.Items);
		foreach(GridItem item in buyCopy)
		{
			int colPos;
			int rowPos;
			GridItemOrient orientation;

			if(player.Inventory.FitItemInBackpack(item.Item, out colPos, out rowPos, out orientation))
			{
				Debug.Log("Found backpack fit " + colPos + ", " + rowPos + " orientation " + orientation);

				GameManager.Inst.UIManager.WindowPanel.InventoryPanel.BackpackGrid.AddGridItem(item.Item, colPos, rowPos, orientation, item.GetQuantity());
				GameManager.Inst.UIManager.WindowPanel.InventoryPanel.SaveInventoryData(player);
			}
			else
			{
				SellGrid.AddGridItem(item.Item, item.ColumnPos, item.RowPos, item.Orientation, item.GetQuantity());
			}

			//if it's rubles then reduce trader cash
			if(item.Item.ID == "rubles")
			{
				_targetTrader.Cash -= item.GetQuantity();
			}

			BuyGrid.Items.Remove(item);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(item);
		}

		BuyPriceLabel.text = "0 RU";
		SellPriceLabel.text = "0 RU";
		GameManager.Inst.UIManager.WindowPanel.InventoryPanel.RefreshTotalWeight();

		UpdateTraderMoneyDisplay();
	}

	public void RefreshSellBuyTotalPrice()
	{

		//clear rubles in buy grid
		float buyPrice = 0;

		List<GridItem> buyGridCopy = new List<GridItem>(BuyGrid.Items);
		foreach(GridItem gItem in buyGridCopy)
		{
			gItem.PriceTag = _targetTrader.GetSellPrice(gItem.Item);
				
			if(gItem.Item.ID == "rubles")
			{
				BuyGrid.RemoveGridItem(gItem);
				GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(gItem);
			}
			else
			{
				buyPrice += gItem.PriceTag * gItem.GetQuantity();
			}
		}

		int buyPriceInt = Mathf.CeilToInt(buyPrice);



		float sellPrice = 0;
		foreach(GridItem gItem in SellGrid.Items)
		{
			gItem.PriceTag = _targetTrader.GetBuyPrice(gItem.Item);
			sellPrice += gItem.PriceTag * gItem.GetQuantity();
		}
		int sellPriceInt = Mathf.CeilToInt(sellPrice);
		SellPrice = sellPriceInt;
		SellPriceLabel.text = sellPriceInt.ToString() + " RU";


		//automatically generate rubles in buy grid if player's item worth more than sells item
		if(sellPriceInt >= buyPriceInt)
		{
			int quantity = sellPriceInt - buyPriceInt;
			if(quantity > _targetTrader.Cash)
			{
				quantity = _targetTrader.Cash;
			}

			buyPriceInt += quantity;
			if(quantity > 0)
			{
				Item rubleItem = GameManager.Inst.ItemManager.LoadItem("rubles");
				int colPos;
				int rowPos;
				GridItemOrient orientation;
				if(BuyGrid.FitItemInGrid(rubleItem, out colPos, out rowPos, out orientation))
				{
					GridItem gItem = BuyGrid.AddGridItem(rubleItem, colPos, rowPos, orientation, quantity);
					if(gItem != null)
					{
						gItem.PriceTag = 1;
					}
				}
			}
		}

		BuyPrice = buyPriceInt;
		BuyPriceLabel.text = buyPriceInt.ToString() + " RU";
	}




	private void UpdateTraderMoneyDisplay()
	{
		int playerMoney = GameManager.Inst.UIManager.WindowPanel.InventoryPanel.BackpackGrid.GetItemQuantityByID("rubles");
		PlayerMoney.text = playerMoney.ToString();
		TraderMoney.text = _targetTrader.Cash.ToString();
	}

}
