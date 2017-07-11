using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChestLootPanel : PanelBase
{
	
	public InventoryGrid ChestGrid;

	public override void Initialize ()
	{
		ChestGrid.Initialize(this);
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

		GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.UseTarget = null;
	}

	public override bool HasInventoryGrids (out List<InventoryGrid> grids)
	{
		grids = new List<InventoryGrid>();
		grids.Add(ChestGrid);
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


	public void RebuildLoot()
	{
		GameObject target = GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.UseTarget;

		if(target == null)
		{
			return;
		}

		Chest chest = target.GetComponent<Chest>();

		if(chest == null)
		{
			return;
		}


		//first remove all existing griditems in the backpack
		List<GridItem> chestGridCopy = new List<GridItem>(ChestGrid.Items);
		foreach(GridItem item in chestGridCopy)
		{
			ChestGrid.Items.Remove(item);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(item);
		}

		//fill inventory grid with loot from backpack
		List<GridItemData> chestItems = chest.Items;
		//now, time to arrange them
		ChestGrid.ArrangeGridItems(chestItems);


	}

	private void SaveInventoryData()
	{
		GameObject target = GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.UseTarget;
		//Debug.Log("target is null? " + target == null);
		if(target == null)
		{
			return;
		}

		Chest chest = target.GetComponent<Chest>();

		if(chest == null)
		{
			return;
		}


		List<GridItemData> datas = chest.Items;
		datas.Clear();

		foreach(GridItem item in ChestGrid.Items)
		{
			GridItemData data = new GridItemData(item.Item, item.ColumnPos, item.RowPos, item.Orientation, item.GetQuantity());
			datas.Add(data);
		}



	}

}
