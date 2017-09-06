using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChestLootPanel : PanelBase
{
	
	public InventoryGrid ChestGrid;

	private Chest _currentChest;

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

		_currentChest = chest;

		if(_currentChest.SoundType == ContainerSoundType.Wood)
		{
			GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("OpenContainerWood"), 0.5f);
		}
		else if(_currentChest.SoundType == ContainerSoundType.Metal)
		{
			GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("MetalDoorOpen"), 0.2f);
		}
		else if(_currentChest.SoundType == ContainerSoundType.Body)
		{
			GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("OpenLootBody"), 0.15f);
		}

		RebuildLoot();


	}

	public override void Hide ()
	{
		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;

		SaveInventoryData();

		if(_currentChest != null)
		{
			if(_currentChest.SoundType == ContainerSoundType.Wood)
			{
				GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("CloseContainerWood"), 0.5f);
			}
			else if(_currentChest.SoundType == ContainerSoundType.Metal)
			{
				GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("MetalDoorClose"), 0.2f);
			}
			else if(_currentChest.SoundType == ContainerSoundType.Body)
			{
				GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("CloseLootBody"), 0.15f);
			}
		}

		GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.UseTarget = null;

		_currentChest = null;
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
		

		if(_currentChest == null)
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
		List<GridItemData> chestItems = _currentChest.Items;
		//now, time to arrange them
		ChestGrid.ArrangeGridItems(chestItems);


	}

	private void SaveInventoryData()
	{
		GameObject target = GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.UseTarget;
		//Debug.Log("target is null? " + target == null);
		if(_currentChest == null)
		{
			return;
		}



		List<GridItemData> datas = _currentChest.Items;
		datas.Clear();

		foreach(GridItem item in ChestGrid.Items)
		{
			GridItemData data = new GridItemData(item.Item, item.ColumnPos, item.RowPos, item.Orientation, item.GetQuantity());
			datas.Add(data);
		}



	}

}
