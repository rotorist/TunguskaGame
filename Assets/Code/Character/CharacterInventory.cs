using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//each party member has an instance of this class
[System.Serializable]
public class CharacterInventorySaveData
{
	public int BackpackCols;
	public int BackpackRows;
	public List<GridItemData> Backpack;
	public Item HeadSlot;
	public Item ArmorSlot;
	public Item RifleSlot;
	public Item SideArmSlot;
	public Item ToolSlot;
	public Item ThrowSlot;
}

public class CharacterInventory : ScriptableObject
{
	public int BackpackCols;
	public int BackpackRows;
	public List<GridItemData> Backpack;
	public Item HeadSlot;
	public Item ArmorSlot;
	public Item RifleSlot;
	public Item SideArmSlot;
	public Item ToolSlot;
	public Item ThrowSlot;

	public CharacterInventory()
	{
		Backpack = new List<GridItemData>();
		BackpackCols = 10;
		BackpackRows = 10;
		HeadSlot = null;
		ArmorSlot = null;
		RifleSlot = null;
		SideArmSlot = null;
		ToolSlot = null;
		ThrowSlot = null;
	}

	public void PostLoad()
	{
		foreach(GridItemData data in Backpack)
		{
			data.Item.PostLoad();
		}
	}

	public GridItemData FindItemInBackpack(string id)
	{
		foreach(GridItemData item in Backpack)
		{
			if(item.Item.ID == id)
			{
				return item;
			}
		}

		return null;
	}

	//returns the first items of type type that it finds
	public GridItemData FindItemInBackpack(ItemType type)
	{
		foreach(GridItemData item in Backpack)
		{
			if(item.Item.Type == type)
			{
				return item;
			}
		}

		return null;
	}

	public List<GridItemData> FindItemsInBackpack(string id)
	{
		List<GridItemData> items = new List<GridItemData>();
		foreach(GridItemData item in Backpack)
		{
			if(item.Item.ID == id)
			{
				items.Add(item);
			}
		}

		return items;
	}

	public int CountItemsInBackpack(string id)
	{
		List<GridItemData> items = FindItemsInBackpack(id);
		int count = 0;
		foreach(GridItemData item in items)
		{
			count += item.Quantity;
		}

		return count;
	}

	public void RemoveItemFromBackpack(Item item)
	{
		List<GridItemData> backpackCopy = new List<GridItemData>(Backpack);
		foreach(GridItemData itemData in backpackCopy)
		{
			if(itemData.Item == item)
			{
				Backpack.Remove(itemData);
			}
		}
	}

	public void RemoveItemFromBackpack(GridItemData item)
	{
		if(Backpack.Contains(item))
		{
			Backpack.Remove(item);
		}
	}


	//returns the actual number of items removed
	public int RemoveItemsFromBackpack(string id, int quantity)
	{
		List<GridItemData> items = FindItemsInBackpack(id);
		int itemsLeft = quantity;

		foreach(GridItemData item in items)
		{
			if(item.Quantity >= itemsLeft)
			{
				item.Quantity -= itemsLeft;
				if(item.Quantity <= 0)
				{
					Backpack.Remove(item);
				}
				return quantity;
			}
			else
			{
				itemsLeft -= item.Quantity;
				Backpack.Remove(item);
			}
		}

		if(itemsLeft > 0)
		{
			return quantity - itemsLeft;
		}
		else
		{
			return quantity;
		}
	}

	public bool FitItemInBodySlot(Item item)
	{
		if(item.Type == ItemType.Helmet && HeadSlot == null)
		{
			HeadSlot = item;
		}
		else if(item.Type == ItemType.Armor && ArmorSlot == null)
		{
			ArmorSlot = item;
		}
		else if(item.Type == ItemType.PrimaryWeapon && RifleSlot == null)
		{
			RifleSlot = item;
		}
		else if(item.Type == ItemType.SideArm && SideArmSlot == null)
		{
			SideArmSlot = item;
		}
		else if(item.Type == ItemType.Thrown && ThrowSlot == null)
		{
			ThrowSlot = item;
		}
		else if(item.Type == ItemType.Tool && ToolSlot == null)
		{
			ToolSlot = item;
		}
		else 
		{
			return false;
		}

		return true;
	}

	public bool FitItemInBackpack(Item item, out int colPos, out int rowPos, out GridItemOrient orientation)
	{
		int [,] grid = new int[BackpackCols, BackpackRows];

		//first fill the grid with information on which block has been occupied already by existing items
		foreach(GridItemData itemData in Backpack)
		{
			int cols = itemData.Item.GridCols;
			int rows = itemData.Item.GridRows;

			if(cols != rows && itemData.Orientation == GridItemOrient.Portrait)
			{
				cols = itemData.Item.GridRows;
				rows = itemData.Item.GridCols;
			}

			for(int x=0; x < cols; x++)
			{
				for(int y=0; y < rows; y++)
				{
					grid[itemData.ColumnPos + x, itemData.RowPos + y] = 1;
				}
			}
		}

		//check each block, if it's not occupied, see if it can fit the item in either orientation
		for(int i=0; i < BackpackCols; i++)
		{
			for(int j=0; j < BackpackRows; j++)
			{
				if(grid[i,j] == 0)
				{
					bool isOccupied = false;
					for(int x=0; x < item.GridCols; x++)
					{
						if(isOccupied || (i+x) >= BackpackCols)
						{
							isOccupied = true;
							break;
						}
						for(int y=0; y < item.GridRows; y++)
						{
							if((j+y) >= BackpackRows || grid[i+x, j+y] == 1)
							{
								isOccupied = true;
								break;
							}
						}
					}

					if(!isOccupied)
					{
						orientation = GridItemOrient.Landscape;
						colPos = i;
						rowPos = j;
						return true;
					}

					//if not square, check portrait
					isOccupied = false;
					if(item.GridCols != item.GridRows)
					{
						for(int x=0; x < item.GridRows; x++)
						{
							if(isOccupied || (i+x) >= BackpackCols)
							{
								isOccupied = true;
								break;
							}
							for(int y=0; y < item.GridCols; y++)
							{
								if((j+y) >= BackpackRows || grid[i+x, j+y] == 1)
								{
									//Debug.Log(" " + (i+x) + ", " + (j+y) + " is occupied");
									isOccupied = true;
									break;
								}
							}
						}

						if(!isOccupied)
						{
							orientation = GridItemOrient.Portrait;
							colPos = i;
							rowPos = j;
							return true;
						}
					}
				}
			}
		}

		colPos = 0;
		rowPos = 0;
		orientation = GridItemOrient.Landscape;
		return false;
	}

}
