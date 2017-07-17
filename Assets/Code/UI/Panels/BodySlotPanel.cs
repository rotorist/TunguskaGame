using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BodySlotPanel : PanelBase
{
	public List<BodySlot> BodySlots;
	public List<TempSlot> DiscardSlots;


	public override void Initialize ()
	{
		

		foreach(BodySlot slot in BodySlots)
		{
			slot.Initialize();
		}

		RebuildInventory();

		Hide();
	}

	public override void PerFrameUpdate ()
	{
		
	
	}

	public override void Show ()
	{
		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;

		RebuildInventory();

		InputEventHandler.OnSelectActiveMember -= OnSelectActiveMember;
		InputEventHandler.OnSelectActiveMember += OnSelectActiveMember;

	
	}

	public override void Hide ()
	{
		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;

		SaveInventoryData(GameManager.Inst.PlayerControl.Party.SelectedMember);

		//refresh each member's weapon loadout
		GameManager.Inst.PlayerControl.SelectedPC.ArmorSystem.SwitchToArmor(GameManager.Inst.PlayerControl.SelectedPC.Inventory.ArmorSlot);
		GameManager.Inst.PlayerControl.SelectedPC.ArmorSystem.SwitchToHelmet(GameManager.Inst.PlayerControl.SelectedPC.Inventory.HeadSlot);
		GameManager.Inst.PlayerControl.SelectedPC.MyAI.WeaponSystem.LoadWeaponsFromInventory(true);


		DropDiscardedItems();

		InputEventHandler.OnSelectActiveMember -= OnSelectActiveMember;
	}

	public override bool HasBodySlots (out List<BodySlot> bodySlots)
	{
		bodySlots = BodySlots;

		return true;
	}

	public override bool HasTempSlots (out List<TempSlot> tempSlots)
	{
		tempSlots = DiscardSlots;

		return true;
	}

	public void OnSelectActiveMember(HumanCharacter prev)
	{
		SaveInventoryData(prev);
		RebuildInventory();
		DropDiscardedItems();
	}





	private void DropDiscardedItems()
	{
		foreach(TempSlot slot in DiscardSlots)
		{
			if(slot.Items.Count > 0)
			{
				//create an instance of pickup item near the owner
				GridItem gridItem = slot.Items.First();
				var resource = Resources.Load(gridItem.Item.PrefabName + "Pickup");
				if(resource != null)
				{
					GameObject pickup = GameObject.Instantiate(resource) as GameObject;
					pickup.transform.position = slot.Owner.transform.position + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 1f, UnityEngine.Random.Range(-0.2f, 0.2f));
					Transform parent = GameManager.Inst.ItemManager.FindPickupItemParent(pickup.transform);
					if(parent != null)
					{
						pickup.transform.parent = parent;
					}
					pickup.GetComponent<PickupItem>().Item = gridItem.Item;
					pickup.GetComponent<PickupItem>().Quantity = gridItem.GetQuantity();
				}


				GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(gridItem);
				slot.Items.Clear();
			}
		}
	}

	private void SaveInventoryData(HumanCharacter character)
	{
		CharacterInventory inventory = character.Inventory;

		inventory.ArmorSlot = null;
		inventory.HeadSlot = null;
		inventory.RifleSlot = null;
		inventory.SideArmSlot = null;
		inventory.ThrowSlot = null;
		inventory.ToolSlot = null;

		if(BodySlots[0].Items.Count > 0)
		{
			inventory.ArmorSlot = BodySlots[0].Items[0].Item;
		}

		if(BodySlots[1].Items.Count > 0)
		{
			inventory.HeadSlot = BodySlots[1].Items[0].Item;
		}

		if(BodySlots[2].Items.Count > 0)
		{
			inventory.RifleSlot = BodySlots[2].Items[0].Item;
		}

		if(BodySlots[3].Items.Count > 0)
		{
			inventory.SideArmSlot = BodySlots[3].Items[0].Item;
		}

		if(BodySlots[4].Items.Count > 0)
		{
			inventory.ThrowSlot = BodySlots[4].Items[0].Item;
		}

		if(BodySlots[5].Items.Count > 0)
		{
			inventory.ToolSlot = BodySlots[5].Items[0].Item;
		}
	}

	private void RebuildInventory()
	{
		//first remove all existing griditems in bodyslots
		foreach(BodySlot slot in BodySlots)
		{
			if(slot.Items.Count > 0)
			{
				//Debug.Log("clearing body slot " + slot.name);
				GridItem temp = slot.Items[0];
				GameObject.Destroy(temp.Boundary.gameObject);
				GameObject.Destroy(temp.Quantity.gameObject);
				slot.Items.Clear();
				GameObject.Destroy(temp.gameObject);
			}
		}

		CharacterInventory inventory = GameManager.Inst.PlayerControl.SelectedPC.Inventory;

		if(inventory.ArmorSlot != null)
		{
			GridItem item = BodySlots[0].LoadGridItem(inventory.ArmorSlot.SpriteName, GridItemOrient.Landscape);
			item.Item = inventory.ArmorSlot;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToBodySlot(item, BodySlots[0]);
		}

		if(inventory.HeadSlot != null)
		{
			GridItem item = BodySlots[1].LoadGridItem(inventory.HeadSlot.SpriteName, GridItemOrient.Landscape);
			item.Item = inventory.HeadSlot;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToBodySlot(item, BodySlots[1]);
		}

		if(inventory.RifleSlot != null)
		{
			GridItem item = BodySlots[2].LoadGridItem(inventory.RifleSlot.SpriteName, GridItemOrient.Landscape);
			item.Item = inventory.RifleSlot;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToBodySlot(item, BodySlots[2]);
			//Debug.Log("rebuilding rifle slot " + item.Item.GetAttributeByName("_LoadedAmmos").Value);
		}

		if(inventory.SideArmSlot != null)
		{
			GridItem item = BodySlots[3].LoadGridItem(inventory.SideArmSlot.SpriteName, GridItemOrient.Landscape);
			item.Item = inventory.SideArmSlot;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToBodySlot(item, BodySlots[3]);
		}

		if(inventory.ThrowSlot != null)
		{
			GridItem item = BodySlots[4].LoadGridItem(inventory.ThrowSlot.SpriteName, GridItemOrient.Landscape);
			item.Item = inventory.ThrowSlot;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToBodySlot(item, BodySlots[4]);
		}

		if(inventory.ToolSlot != null)
		{
			GridItem item = BodySlots[5].LoadGridItem(inventory.ToolSlot.SpriteName, GridItemOrient.Landscape);
			item.Item = inventory.ToolSlot;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToBodySlot(item, BodySlots[5]);
		}
	}
}
