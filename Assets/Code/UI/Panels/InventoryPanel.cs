using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InventoryPanel : PanelBase
{
	public UISprite Background;

	public InventoryGrid BackpackGrid;

	public UILabel ItemDescription;
	public UILabel ItemName;
	public UILabel ItemAttributeNames;
	public UILabel ItemAttributeValues;
	public UILabel ItemSpecialAttrGood;
	public UILabel ItemSpecialAttrBad;
	public UILabel ItemPrice;
	public UILabel LabelCarryWeight;

	public GridItem SelectedItem;
	public GridItem FocusedItem;
	public GridItem RightClickedItem;
	public GridItem ReplaceItem;//item that's already in inventory but will be replaced by selected item when clicked

	public InventoryGrid FocusedGrid;
	public BodySlot FocusedBodySlot;
	public TempSlot FocusedTempSlot;


	private WindowPanel _windowPanel;
	private List<GridItem> _selectedItemLastList;

	public override void Initialize ()
	{
		_windowPanel = GameManager.Inst.UIManager.WindowPanel;

		BackpackGrid.Initialize(this);
		RebuildInventory();
		Hide();


	}

	public override void PerFrameUpdate ()
	{
		//make item sprite follow cursor
		if(SelectedItem != null)
		{
			Vector3 pos = Input.mousePosition;
			pos.x = Mathf.Clamp01(pos.x / Screen.width);
			pos.y = Mathf.Clamp01(pos.y / Screen.height);
			SelectedItem.Sprite.transform.position = GameManager.Inst.UIManager.UICamera.ViewportToWorldPoint(pos);
			SelectedItem.Quantity.transform.localPosition = SelectedItem.Sprite.transform.localPosition 
				- new Vector3(SelectedItem.ColumnSize * BackpackGrid.BlockSize / 2f - 4, SelectedItem.RowSize * BackpackGrid.BlockSize / 2f - 4);
			Vector3 centerPos = SelectedItem.Sprite.transform.localPosition;

			//find nearest fitting slot for selected item and move the boundary there
			int fitColumn = 0;
			int fitRow = 0;


			List<InventoryGrid> grids = _windowPanel.FindInventoryGrids();
			if(FindNearestGridSlot(grids, out fitColumn, out fitRow, SelectedItem.IsPlayerOwned))
			{
				SelectedItem.Boundary.transform.parent = FocusedGrid.transform;
				if(SelectedItem.Orientation == GridItemOrient.Landscape)
				{
					SelectedItem.Boundary.pivot = UIWidget.Pivot.BottomLeft;
				}
				else
				{
					SelectedItem.Boundary.pivot = UIWidget.Pivot.TopLeft;
				}

				SelectedItem.Boundary.transform.localEulerAngles = SelectedItem.Sprite.transform.localEulerAngles;

				SelectedItem.Boundary.width = SelectedItem.Sprite.width;
				SelectedItem.Boundary.height = SelectedItem.Sprite.height;
				SelectedItem.Boundary.transform.localPosition = new Vector3(fitColumn * BackpackGrid.BlockSize, fitRow * BackpackGrid.BlockSize, 0);
				SelectedItem.Boundary.alpha = 1;
				SelectedItem.ColumnPos = fitColumn;
				SelectedItem.RowPos = fitRow;

				FocusedBodySlot = null;
			}
			else
			{
				SelectedItem.Boundary.alpha = 0;
				FocusedBodySlot = null;
				if(ReplaceItem != null)
				{
					ReplaceItem.Sprite.alpha = 1;
				}
			}

			List<BodySlot> bodySlots = _windowPanel.FindBodySlots();
			BodySlot fitSlot;
			if(bodySlots.Count > 0)
			{

				if(FindNearestBodySlot(out fitSlot, centerPos, bodySlots, SelectedItem.Item.Type))
				{
					FocusedBodySlot = fitSlot;
					SelectedItem.Boundary.transform.parent = FocusedBodySlot.transform;
					SelectedItem.Boundary.pivot = UIWidget.Pivot.Center;
					SelectedItem.Boundary.transform.localEulerAngles = Vector3.zero;

					SelectedItem.Boundary.width = FocusedBodySlot.Background.width - 10;
					SelectedItem.Boundary.height = FocusedBodySlot.Background.height - 10;
					SelectedItem.Boundary.transform.localPosition = Vector3.zero;


					SelectedItem.Boundary.alpha = 1;
				}
				else
				{
					FocusedBodySlot = null;
				}
			}

			List<TempSlot> tempSlots = _windowPanel.FindTempSlots();
			TempSlot tempSlot;
			if(tempSlots.Count > 0)
			{

				if(FindNearestTempSlot(out tempSlot, centerPos, tempSlots))
				{
					FocusedTempSlot = tempSlot;
					SelectedItem.Boundary.transform.parent = FocusedTempSlot.transform;
					SelectedItem.Boundary.pivot = UIWidget.Pivot.Center;
					SelectedItem.Boundary.transform.localEulerAngles = Vector3.zero;

					SelectedItem.Boundary.width = FocusedTempSlot.Background.width - 10;
					SelectedItem.Boundary.height = FocusedTempSlot.Background.height - 10;
					SelectedItem.Boundary.transform.localPosition = Vector3.zero;


					SelectedItem.Boundary.alpha = 1;
				}
				else
				{
					FocusedTempSlot = null;
				}
			}

		}


	}

	public override void Show ()
	{
		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;


		RebuildInventory();
		RefreshTotalWeight();

		InputEventHandler.OnSelectActiveMember -= OnSelectActiveMember;
		InputEventHandler.OnSelectActiveMember += OnSelectActiveMember;


	}

	public override void Hide ()
	{
		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;


		SaveInventoryData(GameManager.Inst.PlayerControl.Party.SelectedMember);

		RefreshTotalWeight();

		InputEventHandler.OnSelectActiveMember -= OnSelectActiveMember;


	}

	public override bool HasInventoryGrids (out List<InventoryGrid> grids)
	{
		grids = new List<InventoryGrid>();
		grids.Add(BackpackGrid);
		return true;
	}






	public void OnHoverItem(GridItem item)
	{
		ItemName.text = item.Item.Name;

		if(item.Item.Type == ItemType.PrimaryWeapon || item.Item.Type == ItemType.SideArm)
		{
			bool isRanged = (bool)item.Item.GetAttributeByName("_IsRanged").Value;
			if(isRanged)
			{
				string ammoID = (string)item.Item.GetAttributeByName("_LoadedAmmoID").Value;
				string ammoName = GameManager.Inst.ItemManager.GetItemNameFromID(ammoID);
				ItemDescription.text = item.Item.Description + "\n\n" + "Caliber: " + (string)item.Item.GetAttributeByName("_Caliber").Value + "\n" +
					"Currently chambering: " + ammoName;
			}
			else
			{
				ItemDescription.text = item.Item.Description;
			}
		}
		else
		{
			ItemDescription.text = item.Item.Description;
		}

		if(item.Item.Type == ItemType.PrimaryWeapon || item.Item.Type == ItemType.SideArm || item.Item.Type == ItemType.Helmet || item.Item.Type == ItemType.Armor)
		{
			ItemDescription.text = ItemDescription.text + "\n\n" + "Durability: " + Mathf.RoundToInt(item.Item.Durability) + " / " + Mathf.RoundToInt(item.Item.MaxDurability);
		}

		//generate item attribute list
		string attributeNames = "";
		string attributeValues = "";
		string specialAttributesGood = "";
		string specialAttributesBad = "";

		foreach(ItemAttribute attribute in item.Item.Attributes)
		{
			if(attribute.Name[0] == '+')
			{
				specialAttributesGood = specialAttributesGood + attribute.Name + "\n";
			}
			else if(attribute.Name[0] == '-')
			{
				specialAttributesBad = specialAttributesBad + attribute.Name + "\n";
			}
			else if(attribute.Name[0] != '_')
			{
				attributeNames = attributeNames + attribute.Name + "\n";
				attributeValues = attributeValues + attribute.Value.ToString() + "\n";
			}
		}



		attributeNames = attributeNames + "Weight";
		attributeValues = attributeValues + (item.Item.Weight * item.GetQuantity()).ToString();

		ItemAttributeNames.text = attributeNames;
		ItemAttributeValues.text = attributeValues;

		ItemSpecialAttrBad.text = "";
		ItemSpecialAttrGood.text = "";

		if(specialAttributesGood.Length > 0)
		{
			ItemSpecialAttrGood.text = specialAttributesGood.Substring(1, specialAttributesGood.Length - 1);
			ItemSpecialAttrGood.transform.localPosition = ItemAttributeNames.transform.localPosition - new Vector3(0, ItemAttributeNames.height + 5, 0);
		}

		if(specialAttributesBad.Length > 0)
		{
			ItemSpecialAttrBad.text = specialAttributesBad.Substring(1, specialAttributesBad.Length - 1);
			float offset = 0;
			if(specialAttributesGood.Length > 0)
			{
				offset = ItemSpecialAttrGood.height;
			}
			ItemSpecialAttrBad.transform.localPosition = ItemAttributeNames.transform.localPosition - new Vector3(0, ItemAttributeNames.height + offset + 5, 0);
		}

		if(GameManager.Inst.UIManager.WindowPanel.TradingPanel.IsActive)
		{
			if(item.IsPlayerOwned)
			{
				ItemPrice.text = "Sell Price: " + Mathf.FloorToInt(item.PriceTag) + " RU";
			}
			else
			{
				ItemPrice.text = "Purchase Price: " + Mathf.FloorToInt(item.PriceTag) + " RU";
			}
		}
		else
		{
			ItemPrice.text = "";
		}
	}

	public void OnSelectItem(GridItem item)
	{
		if(RightClickedItem == null)
		{
			//no right click menu is open
			if(SelectedItem == null)
			{
				//time to pick item up
				PickupItem(item);

			}
		}
		else
		{
			//right click menu is open, just close the menu and do nothing else

		}
	}

	public void OnPlaceItem(GridItem item)
	{
		if(SelectedItem == item && item.Boundary.alpha == 1)
		{
			
			if(FocusedTempSlot != null)
			{
				//place in temp slot
				GridItem existingItem = null;
				TempSlot temp = FocusedTempSlot;
				if(FocusedTempSlot.Items.Count > 0)
				{
					existingItem = FocusedTempSlot.Items[0];
				}
				FocusedTempSlot.Items.Clear();

				PlaceItemInTempSlot(item);

				if(_selectedItemLastList != null && _selectedItemLastList != temp.Items)
				{
					_selectedItemLastList.Remove(item);
					_selectedItemLastList = null;
				}

				if(existingItem != null && existingItem != item)
				{
					PickupItem(existingItem);
				}
					
			}
			else if(FocusedBodySlot != null)
			{
				//place in body slot
				GridItem existingItem = null;
				BodySlot temp = FocusedBodySlot;
				if(FocusedBodySlot.Items.Count > 0)
				{
					existingItem = FocusedBodySlot.Items[0];
				}


				if(item.Item.Type == ItemType.Ammo && (FocusedBodySlot.AllowedItemType == ItemType.PrimaryWeapon || FocusedBodySlot.AllowedItemType == ItemType.SideArm))
				{
					if(existingItem != null && (int)existingItem.Item.GetAttributeByName("_LoadedAmmos").Value <= 0 && 
						(string)item.Item.GetAttributeByName("_Caliber").Value == (string)existingItem.Item.GetAttributeByName("_Caliber").Value)
					{
						existingItem.Item.SetAttribute("_LoadedAmmoID", item.Item.ID);
						GameManager.Inst.UIManager.SetConsoleText("The weapon is now loading " + item.Item.Name);
						GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("LoadAmmo"), 0.05f);
					}
					else if((int)existingItem.Item.GetAttributeByName("_LoadedAmmos").Value > 0)
					{
						GameManager.Inst.UIManager.SetConsoleText("Weapon is still loaded, unload ammo first.");
						GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("Error1"), 0.05f);
					}
					else if((string)item.Item.GetAttributeByName("_Caliber").Value != (string)existingItem.Item.GetAttributeByName("_Caliber").Value)
					{
						GameManager.Inst.UIManager.SetConsoleText("Caliber doesn't match!");
						GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("Error1"), 0.05f);
					}

				}
				else
				{
					FocusedBodySlot.Items.Clear();
					PlaceItemInBodySlot(item);

					if(_selectedItemLastList != null && _selectedItemLastList != temp.Items)
					{
						_selectedItemLastList.Remove(item);
						_selectedItemLastList = null;
					}


					if(existingItem != null && existingItem != item)
					{

						PickupItem(existingItem);

					}

				}

			}
			else if(FocusedGrid != null)
			{
				
				PlaceItem(item);	

				if(_selectedItemLastList != null)
				{
					_selectedItemLastList.Remove(item);
					_selectedItemLastList = null;
				}

				if(ReplaceItem != null)
				{
					Debug.Log("Replace item is not null");
					//if replace item is same as item then try to combine the two
					if(ReplaceItem.Item.ID == item.Item.ID && item.Item.MaxStackSize > 1)
					{
						int fill = item.Item.MaxStackSize - item.GetQuantity();
						if(fill < ReplaceItem.GetQuantity())
						{
							item.SetQuantity(item.Item.MaxStackSize);
							ReplaceItem.SetQuantity(ReplaceItem.GetQuantity() - fill);

							PickupItem(ReplaceItem);
							ReplaceItem.Sprite.alpha = 1;
							ReplaceItem = null;
						}
						else
						{
							item.SetQuantity(item.GetQuantity() + ReplaceItem.GetQuantity());
							FocusedGrid.Items.Remove(ReplaceItem);
							DestroyItem(ReplaceItem);
						}

						RefreshItemTotals();

						return;
					}

					//if replace item is not the same, but selected item is ammo and replace item is an empty gun that can use the ammo
					//then load the gun with ammo
					if(item.Item.Type == ItemType.Ammo && (ReplaceItem.Item.Type == ItemType.PrimaryWeapon || ReplaceItem.Item.Type == ItemType.SideArm))
					{
						if((bool)ReplaceItem.Item.GetAttributeByName("_IsRanged").Value == true)
						{
							if((int)ReplaceItem.Item.GetAttributeByName("_LoadedAmmos").Value <= 0 && 
								(string)item.Item.GetAttributeByName("_Caliber").Value == (string)ReplaceItem.Item.GetAttributeByName("_Caliber").Value)
							{
								ReplaceItem.Item.SetAttribute("_LoadedAmmoID", item.Item.ID);
								GameManager.Inst.UIManager.SetConsoleText("The weapon is now loading " + item.Item.Name);
								GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("LoadAmmo"), 0.05f);
							}
							else if((int)ReplaceItem.Item.GetAttributeByName("_LoadedAmmos").Value > 0)
							{
								GameManager.Inst.UIManager.SetConsoleText("Weapon is still loaded, unload ammo first.");
								GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("Error1"), 0.05f);
							}
							else if((string)item.Item.GetAttributeByName("_Caliber").Value != (string)ReplaceItem.Item.GetAttributeByName("_Caliber").Value)
							{
								GameManager.Inst.UIManager.SetConsoleText("Caliber doesn't match!");
								GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("Error1"), 0.05f);
							}

							PickupItem(item);
							RefreshItemTotals();
							return;
						}

					}


					PickupItem(ReplaceItem);
					ReplaceItem.Sprite.alpha = 1;
					ReplaceItem = null;


				}


			}
				

		}

		RefreshItemTotals();
	}

	public void OnSelectActiveMember(HumanCharacter prev)
	{
		SaveInventoryData(prev);
		RebuildInventory();
	}

	public void OnOpenSplitMenu(GridItem item)
	{
		if(item.Item.MaxStackSize > 1 || item.Item.IsUsable)
		{
			_windowPanel.SplitItemPanel.Target = item;
			_windowPanel.SplitItemPanel.Show();
		}
	}

	public void OnUnloadAmmo(GridItem item)
	{
		ItemAttribute loadedAmmo = item.Item.GetAttributeByName("_LoadedAmmoID");
		if(loadedAmmo == null)
		{
			return;
		}

		int quantity = (int)item.Item.GetAttributeByName("_LoadedAmmos").Value;
		string ammoID = (string)loadedAmmo.Value;

		if(quantity <= 0)
		{
			return;
		}
		Debug.Log("Ammos in gun " + quantity);
		item.Item.SetAttribute("_LoadedAmmos", 0);

		Item ammo = GameManager.Inst.ItemManager.LoadItem(ammoID);
		CreateSelectedItem(ammo, quantity, item.IsPlayerOwned);
		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("UnloadAmmo"), 0.2f);
	}

	public void OnCloseSplitMenuTake(GridItem item, int quantity)
	{
		_windowPanel.SplitItemPanel.Hide();

		if(quantity > 0 && quantity < item.GetQuantity())
		{
			//reduce the exsiting item's quantity and add new item to selected
			item.SetQuantity(item.GetQuantity() - quantity);
			Item newItem = new Item(item.Item);
			CreateSelectedItem(newItem, quantity, item.IsPlayerOwned);
		}
		else if(quantity >= item.GetQuantity())
		{
			PickupItem(item);
		}


	}

	public void OnCloseSplitMenuUse(GridItem item, int quantity)
	{
		if(quantity > 0 && quantity <= item.GetQuantity())
		{
			//perform consumption. If consumption successful, reduce item quantity and hide panel
			if(GameManager.Inst.PlayerControl.Survival.UseItem(item.Item, quantity))
			{
				int existingQuantity = item.GetQuantity();
				item.SetQuantity(existingQuantity - quantity);
				_windowPanel.SplitItemPanel.Hide();

				if(existingQuantity <= quantity)
				{
					InventoryGrid parentGrid = item.GetParentGrid();
					if(parentGrid != null)
					{
						parentGrid.RemoveGridItem(item);
					}
					DestroyItem(item);
				}
			}


		}


	}

	public void OnOpenNotePaper(GridItem item)
	{
		_windowPanel.NotePaperPanel.NoteID = item.Item.GetAttributeByName("_NoteID").Value.ToString();
		_windowPanel.NotePaperPanel.Show();
	}


	public void AddItemToTempSlot(GridItem item, TempSlot slot)
	{
		item.Sprite.pivot = UIWidget.Pivot.Center;

		item.Sprite.transform.parent = item.Boundary.transform.parent;

		if(item.IsRotatable)
		{
			if(item.Orientation == GridItemOrient.Portrait)
			{
				int temp = item.ColumnSize;
				item.ColumnSize = item.RowSize;
				item.RowSize = temp;

				item.Orientation = GridItemOrient.Landscape;
				item.transform.localEulerAngles = new Vector3(0, 0, 0);
			}

			if(item.Sprite.width > item.Sprite.height)
			{
				item.Sprite.width = slot.Background.width;
				item.Sprite.height = Mathf.FloorToInt(item.Sprite.width * ((item.RowSize * 1f) / item.ColumnSize));
			}
			else
			{
				item.Sprite.height = slot.Background.height;
				item.Sprite.width = Mathf.FloorToInt(item.Sprite.height * ((item.ColumnSize * 1f) / item.RowSize));
			}
		}
		else
		{
			item.Sprite.width = slot.Background.width;
			item.Sprite.height = item.Sprite.width;
		}

		item.Boundary.width = slot.Background.width - 10;
		item.Boundary.height = slot.Background.height - 10;

		item.Sprite.depth = (int)InventoryItemDepth.Normal;
		item.Quantity.depth = item.Sprite.depth + 1;
		item.transform.localPosition = item.Boundary.transform.localPosition;
		item.Quantity.transform.localPosition = slot.transform.localPosition - new Vector3(slot.Background.width/2f - 8, slot.Background.height/2f - 8, 0);
		NGUITools.AddWidgetCollider(item.gameObject);
		item.State = GridItemState.None;
		slot.Items.Add(item);
		slot.PrevColPos = item.ColumnPos;
		slot.PrevRowPos = item.RowPos;
		slot.Owner = GameManager.Inst.PlayerControl.SelectedPC;
		item.IsPlayerOwned = true;
		item.ClearParentGrid();
	}

	public void AddItemToBodySlot(GridItem item, BodySlot slot)
	{
		item.Sprite.pivot = UIWidget.Pivot.Center;

		item.Sprite.transform.parent = item.Boundary.transform.parent;

		if(item.IsRotatable)
		{
			if(item.Item.Type == ItemType.PrimaryWeapon || item.Item.Type == ItemType.SideArm)
			{
				if(item.Orientation == GridItemOrient.Landscape)
				{
					int temp = item.ColumnSize;
					item.ColumnSize = item.RowSize;
					item.RowSize = temp;

					item.Orientation = GridItemOrient.Portrait;
					item.transform.localEulerAngles = new Vector3(0, 0, 90);
				}

				if(((float)item.ColumnSize) / ((float)item.RowSize) < 0.4f)
				{
					item.Sprite.width = slot.Background.height;
					item.Sprite.height = Mathf.FloorToInt(item.Sprite.width * ((item.ColumnSize * 1f) / item.RowSize));
				}
				else
				{
					item.Sprite.width = slot.Background.width;
					item.Sprite.height = Mathf.FloorToInt(item.Sprite.width * ((item.ColumnSize * 1f) / item.RowSize));
				}
			}
			else if(item.Item.Type == ItemType.Armor)
			{
				if(item.Orientation == GridItemOrient.Portrait)
				{
					int temp = item.ColumnSize;
					item.ColumnSize = item.RowSize;
					item.RowSize = temp;

					item.Orientation = GridItemOrient.Landscape;
					item.transform.localEulerAngles = new Vector3(0, 0, 0);
				}

				item.Sprite.width = slot.Background.width;
				item.Sprite.height = Mathf.FloorToInt(item.Sprite.width * ((item.RowSize * 1f) / item.ColumnSize));
			}
			else if(item.Item.Type == ItemType.Thrown || item.Item.Type == ItemType.Tool)
			{
				if(item.Orientation == GridItemOrient.Portrait)
				{
					int temp = item.ColumnSize;
					item.ColumnSize = item.RowSize;
					item.RowSize = temp;

					item.Orientation = GridItemOrient.Landscape;
					item.transform.localEulerAngles = new Vector3(0, 0, 0);
				}

				item.Sprite.height = slot.Background.height;
				item.Sprite.width = Mathf.FloorToInt(item.Sprite.height * ((item.ColumnSize * 1f) / item.RowSize));
			}
		}
		else
		{
			item.Sprite.width = slot.Background.width;
			item.Sprite.height = item.Sprite.width;
		}

		item.Boundary.width = slot.Background.width - 10;
		item.Boundary.height = slot.Background.height - 10;

		item.Sprite.depth = (int)InventoryItemDepth.Normal;
		item.transform.localPosition = item.Boundary.transform.localPosition;
		NGUITools.AddWidgetCollider(item.gameObject);
		item.State = GridItemState.None;

		slot.Items.Add(item);
		item.IsPlayerOwned = true;
		item.ClearParentGrid();
	}

	public void AddUnfitItem(PickupItem pickup)
	{
		CreateSelectedItem(pickup.Item, pickup.Quantity, true);


	}

	public void DestroyItem(GridItem item)
	{
		GameObject.Destroy(item.Boundary.gameObject);
		GameObject.Destroy(item.Quantity.gameObject);
		GameObject.Destroy(item.gameObject);
	}


	public void SaveInventoryData(HumanCharacter character)
	{
		List<GridItemData> datas = character.Inventory.Backpack;
		datas.Clear();

		foreach(GridItem item in BackpackGrid.Items)
		{
			GridItemData data = new GridItemData(item.Item, item.ColumnPos, item.RowPos, item.Orientation, item.GetQuantity());
			datas.Add(data);
		}
	}






	private void RebuildInventory()
	{
		//first remove all existing griditems in the backpack
		List<GridItem> backpackCopy = new List<GridItem>(BackpackGrid.Items);
		foreach(GridItem item in backpackCopy)
		{
			BackpackGrid.Items.Remove(item);
			DestroyItem(item);
		}

		//then load player party inventory's selected member's partyMemberInventory
		List<GridItemData> datas = GameManager.Inst.PlayerControl.Party.SelectedMember.Inventory.Backpack;
		foreach(GridItemData data in datas)
		{
			BackpackGrid.AddGridItem(data.Item, data.ColumnPos, data.RowPos, data.Orientation, data.Quantity);
		}
	}



	private void PickupItem(GridItem item)
	{
		//find out who's the item's container
		GameObject container = item.transform.parent.gameObject;


		if(container.GetComponent<InventoryGrid>() != null)
		{
			_selectedItemLastList = container.GetComponent<InventoryGrid>().Items;

			//if it's not owned by player, don't let player pick up rubles
			if(!container.GetComponent<InventoryGrid>().IsPlayerOwned && item.Item.ID == "rubles")
			{
				return;
			}
		}
		else if(container.GetComponent<BodySlot>() != null)
		{
			_selectedItemLastList = container.GetComponent<BodySlot>().Items;
		}
		else if(container.GetComponent<TempSlot>() != null)
		{
			_selectedItemLastList = container.GetComponent<TempSlot>().Items;
		}

		SelectedItem = item;
		item.Boundary.alpha = 0;
		NormalizeItemSpriteSize(item);
		item.Sprite.pivot = UIWidget.Pivot.Center;
		item.Sprite.transform.parent = _windowPanel.SelectedItemPanel.transform;
		item.Sprite.depth = (int)InventoryItemDepth.Selected;
		item.Quantity.depth = item.Sprite.depth + 1;
		item.Quantity.transform.parent = item.Sprite.transform.parent;
		//item.Sprite.panel = GameManager.Inst.UIManager.WindowPanel.SelectedItemPanel;
		NGUITools.MarkParentAsChanged(_windowPanel.gameObject);
		NGUITools.AddWidgetCollider(item.gameObject);
		item.State = GridItemState.Selected;
		FocusedBodySlot = null;

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("SelectItem"), 0.1f);

	}

	private void PlaceItem(GridItem item)
	{
		if(item.Orientation == GridItemOrient.Landscape)
		{
			item.Sprite.pivot = UIWidget.Pivot.BottomLeft;
		}
		else
		{
			item.Sprite.pivot = UIWidget.Pivot.TopLeft;
		}

		item.Sprite.transform.parent = item.Boundary.transform.parent;
		item.Sprite.depth = (int)InventoryItemDepth.Normal;

		item.transform.localPosition = item.Boundary.transform.localPosition;
		InventoryGrid grid = item.transform.parent.GetComponent<InventoryGrid>();
		grid.GetColumnRowFromLocalPos(item.transform.localPosition, out item.ColumnPos, out item.RowPos);

		item.Quantity.transform.parent = item.Sprite.transform.parent;
		item.Quantity.transform.localPosition = item.Sprite.transform.localPosition + new Vector3(4, 4, 0);

		NGUITools.AddWidgetCollider(item.gameObject);
		NGUITools.MarkParentAsChanged(_windowPanel.gameObject);
		//item.State = GridItemState.None;
		item.Initialize(FocusedGrid);
		FocusedGrid.Items.Add(item);

		SelectedItem = null;
		FocusedBodySlot = null;

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("PlaceItemBackpack"), 0.2f);
	}

	private void PlaceItemInBodySlot(GridItem item)
	{
		AddItemToBodySlot(item, FocusedBodySlot);
		NGUITools.MarkParentAsChanged(_windowPanel.gameObject);
		SelectedItem = null;
		FocusedBodySlot = null;
		if(item.Item.Type == ItemType.PrimaryWeapon || item.Item.Type == ItemType.SideArm)
		{
			if((bool)item.Item.GetAttributeByName("_IsRanged").Value == true)
			{
				GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("PlaceItemWeaponBody"), 0.1f);
			}
			else
			{
				GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("PlaceItemMeleeBody"), 0.1f);
			}
		}
		else if(item.Item.Type == ItemType.Armor)
		{
			GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("PlaceItemArmorBody"), 0.1f);
		}
		else
		{
			GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("PlaceItemBody"), 0.1f);
		}
	}

	private void PlaceItemInTempSlot(GridItem item)
	{
		AddItemToTempSlot(item, FocusedTempSlot);
		NGUITools.MarkParentAsChanged(_windowPanel.gameObject);
		SelectedItem = null;
		FocusedTempSlot = null;

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("PlaceItemBackpack"), 0.2f);
	}


	private void RefreshItemTotals()
	{
		RefreshTotalWeight();
		if(GameManager.Inst.UIManager.WindowPanel.TradingPanel.IsActive)
		{
			TradingPanel tradingPanel = GameManager.Inst.UIManager.WindowPanel.TradingPanel;
			tradingPanel.RefreshSellBuyTotalPrice();
			foreach(GridItem gItem in GameManager.Inst.UIManager.WindowPanel.TraderItemPanel.TraderItemGrid.Items)
			{
				gItem.PriceTag = tradingPanel.GetCurrentTrader().GetSellPrice(gItem.Item);
			}

			foreach(GridItem gItem in BackpackGrid.Items)
			{
				gItem.PriceTag = tradingPanel.GetCurrentTrader().GetBuyPrice(gItem.Item);
			}
		}
	}

	public void RefreshTotalWeight()
	{
		float totalWeight = 0;
		//depending on whether the panel is active. if active, calculate based on backpack and body slots
		//if not active, calculate based on character inventory
		CharacterInventory playerInventory = GameManager.Inst.PlayerControl.SelectedPC.Inventory;
		if(IsActive)
		{
			foreach(GridItem item in BackpackGrid.Items)
			{
				totalWeight += item.Item.Weight * item.GetQuantity();
			}
		}
		else
		{
			foreach(GridItemData item in playerInventory.Backpack)
			{
				totalWeight += item.Item.Weight * item.Quantity;
			}
		}

		if(GameManager.Inst.UIManager.WindowPanel.BodySlotPanel.IsActive)
		{
			foreach(BodySlot slot in GameManager.Inst.UIManager.WindowPanel.BodySlotPanel.BodySlots)
			{
				if(slot.Items.Count > 0)
				{
					totalWeight += slot.Items[0].Item.Weight;
				}
			}
		}
		else
		{
			if(playerInventory.ArmorSlot != null)
			{
				totalWeight += playerInventory.ArmorSlot.Weight;
			}
			if(playerInventory.HeadSlot != null)
			{
				totalWeight += playerInventory.HeadSlot.Weight;
			}
			if(playerInventory.ThrowSlot != null)
			{
				totalWeight += playerInventory.ThrowSlot.Weight;
			}
			if(playerInventory.ToolSlot != null)
			{
				totalWeight += playerInventory.ToolSlot.Weight;
			}
			if(playerInventory.RifleSlot != null)
			{
				totalWeight += playerInventory.RifleSlot.Weight;
			}
			if(playerInventory.SideArmSlot != null)
			{
				totalWeight += playerInventory.SideArmSlot.Weight;
			}
		}

		GameManager.Inst.PlayerControl.SelectedPC.MyStatus.CarryWeight = totalWeight;
		float maxWeight = GameManager.Inst.PlayerControl.SelectedPC.MyStatus.MaxCarryWeight;

		LabelCarryWeight.text = "CARRYING " + totalWeight.ToString() + " / " + maxWeight.ToString() + " kg";
	}




	private void CreateSelectedItem(Item item, int quantity, bool isPlayerOwned)
	{
		GameObject o = GameObject.Instantiate(Resources.Load("ItemSprite_" + item.ID)) as GameObject;
		UISprite sprite = o.GetComponent<UISprite>();
		GridItem gridItem = o.GetComponent<GridItem>();
		o.transform.parent = transform;

		sprite.MakePixelPerfect();
		sprite.width = BackpackGrid.BlockSize * gridItem.ColumnSize;
		sprite.height = BackpackGrid.BlockSize * gridItem.RowSize;
		sprite.depth = (int)InventoryItemDepth.Normal;

		gridItem.Sprite = sprite;
		gridItem.Item = item;

		//apply boundary
		o = GameObject.Instantiate(Resources.Load("ItemBoundary")) as GameObject;
		UISprite boundary = o.GetComponent<UISprite>();
		boundary.transform.parent = transform;
		boundary.MakePixelPerfect();
		boundary.width = sprite.width;
		boundary.height = sprite.height;
		gridItem.Boundary = boundary;

		o = GameObject.Instantiate(Resources.Load("ItemQuantity")) as GameObject;
		o.transform.parent = transform;
		o.transform.localScale = new Vector3(1, 1, 1);
		UILabel quantityLabel = o.GetComponent<UILabel>();
		gridItem.Quantity = quantityLabel;
		gridItem.SetQuantity(quantity);


		SelectedItem = gridItem;
		gridItem.Boundary.alpha = 0;
		NormalizeItemSpriteSize(gridItem);
		gridItem.Sprite.pivot = UIWidget.Pivot.Center;
		gridItem.Sprite.transform.parent = _windowPanel.SelectedItemPanel.transform;
		gridItem.Sprite.depth = (int)InventoryItemDepth.Selected;
		quantityLabel.depth = gridItem.Sprite.depth + 1;
		quantityLabel.transform.parent = gridItem.Sprite.transform.parent;
		//item.Sprite.panel = GameManager.Inst.UIManager.WindowPanel.SelectedItemPanel;
		NGUITools.MarkParentAsChanged(_windowPanel.gameObject);
		NGUITools.AddWidgetCollider(gridItem.gameObject);
		gridItem.State = GridItemState.Selected;
		gridItem.IsPlayerOwned = isPlayerOwned;
	}


	private void NormalizeItemSpriteSize(GridItem item)
	{
		if(item.Orientation == GridItemOrient.Landscape)
		{
			item.Sprite.width = item.ColumnSize * BackpackGrid.BlockSize;
			item.Sprite.height = item.RowSize * BackpackGrid.BlockSize;
		}
		else
		{
			item.Sprite.height = item.ColumnSize * BackpackGrid.BlockSize;
			item.Sprite.width = item.RowSize * BackpackGrid.BlockSize;
		}
	}



	private bool FindNearestGridSlot(List<InventoryGrid> grids, out int fitColumn, out int fitRow, bool isPlayerOwned)
	{
		fitColumn = 0;
		fitRow = 0;

		Vector3 bottomLeftPos = SelectedItem.transform.localPosition - new Vector3(SelectedItem.ColumnSize * BackpackGrid.BlockSize / 2, SelectedItem.RowSize * BackpackGrid.BlockSize / 2, 0);
		Vector3 centerPos = SelectedItem.transform.localPosition;
		//see which grid we are at
		FocusedGrid = null;

		foreach(InventoryGrid grid in grids)
		{
			//check if item is allowed in this grid
			bool isItemAllowed = false;
			if(grid.AllowedItemTypes.Count > 0)
			{
				bool typeMatchFound = false;
				foreach(ItemType type in grid.AllowedItemTypes)
				{
					if(SelectedItem.Item.Type == type)
					{
						typeMatchFound = true;
					}
				}
				if(typeMatchFound)
				{
					isItemAllowed = true;
				}
			}
			else
			{
				isItemAllowed = true;
			}

			if(centerPos.x >= grid.Grid.transform.localPosition.x && centerPos.x <= grid.Grid.transform.localPosition.x + grid.Columns * grid.BlockSize &&
				centerPos.y >= grid.Grid.transform.localPosition.y && centerPos.y <= grid.Grid.transform.localPosition.y + grid.Rows * grid.BlockSize
				&& grid.IsPlayerOwned == isPlayerOwned && isItemAllowed)
			{
				FocusedGrid = grid;
			}
		}
		if(FocusedGrid == null)
		{
			return false;
		}

		//starting from 1 block down-left of bottomLeftPos, check if there's room to fit the item
		int col = 0;
		int row = 0;
		bool result = false;
		int [] xArray = new int[]{0, -1, 0, -1, 1, 0, 1};
		int [] yArray = new int[]{0, -1, -1, 0, 0, 1, 1};
		int count = 0;
		while(!result && count < 7)
		{

			result = FocusedGrid.GetColumnRowFromPos(bottomLeftPos + new Vector3(BackpackGrid.BlockSize * xArray[count], BackpackGrid.BlockSize * yArray[count], 0), out col, out row);
			if(result)
			{
				//check if this col/row can fit the item
				GridItem replace = null;
				if(ReplaceItem != null)
				{
					ReplaceItem.Sprite.alpha = 1;
				}

				if(FocusedGrid.CanItemFitHere(col, row, SelectedItem.ColumnSize, SelectedItem.RowSize, out replace))
				{


					ReplaceItem = replace;
					if(ReplaceItem != null)
					{
						ReplaceItem.Sprite.alpha = 0.5f;
					}

					fitColumn = col;
					fitRow = row;
					return true;
				}
			}

			count++;
		}

		if(!result)
		{
			return false;
		}


		return false;
	}


	private bool FindNearestBodySlot(out BodySlot fitSlot, Vector3 centerPos, List<BodySlot> bodySlots, ItemType itemType)
	{

		fitSlot = null;

		foreach(BodySlot slot in bodySlots)
		{
			if(centerPos.x >= slot.transform.localPosition.x - slot.Background.width/2 && centerPos.x <= slot.transform.localPosition.x + slot.Background.width/2 &&
				centerPos.y >= slot.transform.localPosition.y - slot.Background.height/2 && centerPos.y <= slot.transform.localPosition.y + slot.Background.height/2)
			{
				if(itemType == slot.AllowedItemType || (itemType == ItemType.Ammo && (slot.AllowedItemType == ItemType.PrimaryWeapon || slot.AllowedItemType == ItemType.SideArm)))
				{
					fitSlot = slot;
					return true;
				}
			}
		}

		return false;
	}

	public bool FindNearestTempSlot(out TempSlot tempSlot, Vector3 centerPos, List<TempSlot> tempSlots)
	{
		tempSlot = null;

		foreach(TempSlot slot in tempSlots)
		{
			if(centerPos.x >= slot.transform.localPosition.x - slot.Background.width/2 && centerPos.x <= slot.transform.localPosition.x + slot.Background.width/2 &&
				centerPos.y >= slot.transform.localPosition.y - slot.Background.height/2 && centerPos.y <= slot.transform.localPosition.y + slot.Background.height/2 &&
				!slot.TakeOnly)
			{
				tempSlot = slot;
				return true;
			}
		}

		return false;
	}
}

public enum InventoryItemDepth
{
	Normal = 5,
	Selected = 10,
	NormalPanel = 12,
	SelectedItemPanel = 13,
	Menu = 15,
}