using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BodyLootPanel : PanelBase
{
	public List<TempSlot> TempSlots;
	public InventoryGrid BackpackGrid;

	public override void Initialize ()
	{
		BackpackGrid.Initialize(this);
		Hide();
	}

	public override void PerFrameUpdate ()
	{


	}

	public override void Show ()
	{
		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;

		RebuildLoot();


	}

	public override void Hide ()
	{
		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;

		SaveInventoryData();

		GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.InteractTarget = null;
	}

	public override bool HasInventoryGrids (out List<InventoryGrid> grids)
	{
		grids = new List<InventoryGrid>();
		grids.Add(BackpackGrid);
		return true;
	}

	public override bool HasBodySlots (out List<BodySlot> bodySlots)
	{
		bodySlots = null;
		return false;
	}

	public override bool HasTempSlots (out List<TempSlot> tempSlots)
	{
		tempSlots = TempSlots;

		return true;
	}


	public void RebuildLoot()
	{
		Character lootTarget = GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.InteractTarget;

		if(lootTarget == null)
		{
			return;
		}

		CharacterInventory loot = lootTarget.Inventory;

		//if not looted, generate loot
		if(!lootTarget.IsLooted)
		{
			//first remove all existing griditems in the backpack
			List<GridItem> backpackCopy = new List<GridItem>(BackpackGrid.Items);
			foreach(GridItem item in backpackCopy)
			{
				BackpackGrid.Items.Remove(item);
				GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(item);
			}

			//get a list of loot from item manager, then add them all to the backpack
			List<GridItemData> items = GameManager.Inst.ItemManager.GetNPCLoot(lootTarget);
			//now, time to arrange them
			BackpackGrid.ArrangeGridItems(items);

			lootTarget.IsLooted = true;

		}

		if(lootTarget.CharacterType == CharacterType.Human)
		{
			GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("OpenLootBody"), 0.1f);
		}
		else
		{
			GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("OpenLootBodyGore"), 0.4f);
		}

		/*

		//remove all existing temp slots
		foreach(TempSlot temp in TempSlots)
		{
			if(temp.Items.Count > 0)
			{
				GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(temp.Items[0]);
				temp.Items.Clear();
			}
		}

		//fill inventory grid with loot from backpack
		List<GridItemData> datas = loot.Backpack;
		foreach(GridItemData data in datas)
		{
			BackpackGrid.AddGridItem(data.Item, data.ColumnPos, data.RowPos, data.Orientation, data.Quantity);
		}

		//fill temp slots with loot from bodyslots

		if(loot.ArmorSlot != null)
		{
			GridItem item = TempSlots[0].LoadGridItem(loot.ArmorSlot.SpriteName, GridItemOrient.Landscape);
			item.Item = loot.ArmorSlot;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToTempSlot(item, TempSlots[0]);
		}

		if(loot.HeadSlot != null)
		{
			GridItem item = TempSlots[1].LoadGridItem(loot.HeadSlot.SpriteName, GridItemOrient.Landscape);
			item.Item = loot.HeadSlot;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToTempSlot(item, TempSlots[1]);
		}


		if(loot.RifleSlot != null)
		{
			GridItem item = TempSlots[2].LoadGridItem(loot.RifleSlot.SpriteName, GridItemOrient.Landscape);
			item.Item = loot.RifleSlot;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToTempSlot(item, TempSlots[2]);
		}


		if(loot.SideArmSlot != null)
		{
			GridItem item = TempSlots[3].LoadGridItem(loot.SideArmSlot.SpriteName, GridItemOrient.Landscape);
			item.Item = loot.SideArmSlot;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToTempSlot(item, TempSlots[3]);
		}

		if(loot.ThrowSlot != null)
		{
			GridItem item = TempSlots[4].LoadGridItem(loot.ThrowSlot.SpriteName, GridItemOrient.Landscape);
			item.Item = loot.ThrowSlot;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToTempSlot(item, TempSlots[4]);
		}

		if(loot.ToolSlot != null)
		{
			GridItem item = TempSlots[5].LoadGridItem(loot.ToolSlot.SpriteName, GridItemOrient.Landscape);
			item.Item = loot.ToolSlot;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToTempSlot(item, TempSlots[5]);
		}
		*/

	}

	private void SaveInventoryData()
	{
		Character lootTarget = GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.InteractTarget;

		if(lootTarget == null)
		{
			return;
		}

		CharacterInventory loot = lootTarget.Inventory;
		List<GridItemData> datas = loot.Backpack;
		datas.Clear();

		foreach(GridItem item in BackpackGrid.Items)
		{
			GridItemData data = new GridItemData(item.Item, item.ColumnPos, item.RowPos, item.Orientation, item.GetQuantity());
			datas.Add(data);
		}

		//save body slot items
		loot.ArmorSlot = null;
		loot.HeadSlot = null;
		loot.RifleSlot = null;
		loot.SideArmSlot = null;
		loot.ThrowSlot = null;
		loot.ToolSlot = null;

		if(TempSlots[0].Items.Count > 0)
		{
			loot.ArmorSlot = TempSlots[0].Items[0].Item;
		}

		if(TempSlots[1].Items.Count > 0)
		{
			loot.HeadSlot = TempSlots[1].Items[0].Item;
		}

		if(TempSlots[2].Items.Count > 0)
		{
			loot.RifleSlot = TempSlots[2].Items[0].Item;
		}

		if(TempSlots[3].Items.Count > 0)
		{
			loot.SideArmSlot = TempSlots[3].Items[0].Item;
		}

		if(TempSlots[4].Items.Count > 0)
		{
			loot.ThrowSlot = TempSlots[4].Items[0].Item;
		}

		if(TempSlots[5].Items.Count > 0)
		{
			loot.ToolSlot = TempSlots[5].Items[0].Item;
		}



	}

}
