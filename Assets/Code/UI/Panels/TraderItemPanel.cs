using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class TraderItemPanel : PanelBase
{
	public InventoryGrid TraderItemGrid;

	private Trader _targetTrader;

	public override void Initialize ()
	{
		TraderItemGrid.Initialize(this);
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

		RefreshItemList(_targetTrader);

	}

	public override void Hide ()
	{
		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;

		if(_targetTrader != null)
		{
			SaveInventoryData(_targetTrader);
		}

		GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.InteractTarget = null;
	}

	public override bool HasInventoryGrids (out List<InventoryGrid> grids)
	{
		grids = new List<InventoryGrid>();
		grids.Add(TraderItemGrid);
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

	private void RefreshItemList(Trader trader)
	{
		//first remove all existing griditems in the inventory
		List<GridItem> traderCopy = new List<GridItem>(TraderItemGrid.Items);
		foreach(GridItem item in traderCopy)
		{
			TraderItemGrid.Items.Remove(item);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(item);
		}
			
		List<GridItemData> items = trader.TraderInventory;
		//now, time to arrange them
		TraderItemGrid.ArrangeGridItems(items);
	}


	private void SaveInventoryData(Trader trader)
	{
		List<GridItemData> datas = trader.TraderInventory;
		datas.Clear();

		foreach(GridItem item in TraderItemGrid.Items)
		{
			GridItemData data = new GridItemData(item.Item, item.ColumnPos, item.RowPos, item.Orientation, item.GetQuantity());
			datas.Add(data);
		}
	}
}
