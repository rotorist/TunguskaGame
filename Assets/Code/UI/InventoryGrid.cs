using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InventoryGrid : MonoBehaviour 
{
	public int Columns;
	public int Rows;
	public int GridWidth;
	public bool IsPlayerOwned;
	public UISprite Grid;
	public List<ItemType> AllowedItemTypes;

	public PanelBase ParentPanel;

	public int BlockSize
	{
		get { return GridWidth / Columns; }
	}

	public List<GridItem> Items;


	public void Initialize(PanelBase parent)
	{
		Items = new List<GridItem>();
		Grid = GetComponent<UISprite>();

		ParentPanel = parent;
	}

	public GridItem AddGridItem(Item item, int colPos, int rowPos, GridItemOrient orientation, int quantity)
	{
		//don't allow rubles to show up in non player owned inventory
		if(item.ID == "rubles" && ParentPanel == GameManager.Inst.UIManager.WindowPanel.TraderItemPanel)
		{
			return null;
		}

		//check if similar item already here
		foreach(GridItem i in Items)
		{
			if(i.Item.ID == item.ID)
			{
				int existingQuantity = i.GetQuantity();
				if(quantity <= i.Item.MaxStackSize - existingQuantity)
				{
					i.SetQuantity(quantity + existingQuantity);
					return i;
				}
				else
				{
					quantity = quantity - (i.Item.MaxStackSize - existingQuantity);
					i.SetQuantity(i.Item.MaxStackSize);
				}
			}
		}

		//Debug.Log("orientation is " + orientation);
		GridItem item1 = LoadGridItem(item.SpriteName, orientation);
		item1.ColumnPos = colPos;
		item1.RowPos = rowPos;
		item1.Sprite.transform.localPosition = new Vector3(BlockSize * item1.ColumnPos, BlockSize * item1.RowPos, 0);
		item1.Boundary.transform.localPosition = item1.Sprite.transform.localPosition;
		item1.Quantity.transform.localPosition = item1.Sprite.transform.localPosition + new Vector3(4, 4, 0);
		item1.SetQuantity(quantity);
		item1.Item = item;
		Items.Add(item1);
		item1.Initialize(this);
		item1.IsPlayerOwned = IsPlayerOwned;

		return item1;
	}

	public void RemoveGridItem(GridItem item)
	{
		if(Items.Contains(item))
		{
			Items.Remove(item);
		}
	}

	public bool ArrangeGridItems(List<GridItemData> inputItems)
	{
		//first sort by item's column size
		inputItems = inputItems.OrderByDescending(x => x.Item.GridCols).ToList();

		//now one by one add to grid
		for(int i=0; i<inputItems.Count; i++)
		{
			GridItem replaceItem;
			bool isAdded = false;


			for(int x=0; x<Columns; x++)
			{
				if(isAdded)
				{
					break;
				}

				for(int y=Rows-1; y>=0; y--)
				{
					if(isAdded)
					{
						break;
					}

					if(CanItemFitHere(x, y, inputItems[i].Item.GridCols,  inputItems[i].Item.GridRows, out replaceItem) && replaceItem == null)
					{
						AddGridItem(inputItems[i].Item, x, y, GridItemOrient.Landscape, inputItems[i].Quantity);
						isAdded = true;
					}
				}
			}

			/*
			if(!isAdded)
			{
				//can't find any more room, return
				return false;
			}
			*/
		}

		return true;
	}

	public int GetItemQuantityByID(string id)
	{
		int quantity = 0;
		foreach(GridItem gItem in Items)
		{
			if(gItem.Item.ID == id)
			{
				quantity += gItem.GetQuantity();
			}
		}

		return quantity;
	}

	public void RemoveItemsByID(string id, int quantity)
	{
		int quantityLeft = quantity;
		List<GridItem> itemsCopy = new List<GridItem>(Items);
		foreach(GridItem gItem in itemsCopy)
		{
			if(quantityLeft <= 0)
			{
				break;
			}

			if(gItem.Item.ID == id)
			{
				if(gItem.GetQuantity() <= quantityLeft)
				{
					quantityLeft -= gItem.GetQuantity();
					RemoveGridItem(gItem);
					GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(gItem);
				}
				else
				{
					gItem.SetQuantity(gItem.GetQuantity() - quantityLeft);
					return;
				}
			}
		}
	}

	public bool GetColumnRowFromPos(Vector3 pos, out int column, out int row)
	{
		column = 0;
		row = 0;

		Vector3 distance = pos - Grid.transform.localPosition;

		if(distance.x < -0.5f * BlockSize || distance.y < -0.5f * BlockSize)
		{
			return false;
		}
		else
		{
			float x = distance.x / BlockSize;
			float y = distance.y / BlockSize;

			column = Mathf.RoundToInt(x);
			row = Mathf.RoundToInt(y);

			return true;
		}
	}

	public bool GetColumnRowFromLocalPos(Vector3 pos, out int column, out int row)
	{
		column = 0;
		row = 0;

		Vector3 distance = pos;

		if(distance.x < 0 || distance.y < 0)
		{
			return false;
		}
		else
		{
			float x = distance.x / BlockSize;
			float y = distance.y / BlockSize;

			column = Mathf.RoundToInt(x);
			row = Mathf.RoundToInt(y);

			return true;
		}
	}

	public bool CanItemFitHere(int column, int row, int colSize, int rowSize, out GridItem replaceItem)
	{
		replaceItem = null;

		int overlapItems = 0;
		GridItem temp = null;
		GridItem overlapItem1 = null;

		for(int x=column; x<column + colSize; x++)
		{
			for(int y=row; y<row + rowSize; y++)
			{
				if(x >= Columns || y >= Rows)
				{
					return false;
				}

				if(IsBlockOccupied(x, y, out temp))
				{
					if(overlapItem1 == null)
					{
						//overlapitem1 hasn't been set yet, set it
						overlapItem1 = temp;
						overlapItems ++;
					}
					else if(overlapItem1 != temp)
					{
						//there's another overlap item
						overlapItems ++;
					}
				}
			}
		}

		if(overlapItems <= 1)
		{
			replaceItem = overlapItem1;
			return true;
		}
		else
		{
			return false;
		}
	}


	public bool FitItemInGrid(Item item, out int colPos, out int rowPos, out GridItemOrient orientation)
	{
		int [,] grid = new int[Columns, Rows];

		//first fill the grid with information on which block has been occupied already by existing items
		foreach(GridItem gItem in Items)
		{
			int cols = gItem.ColumnSize;
			int rows = gItem.RowSize;

			if(cols != rows && gItem.Orientation == GridItemOrient.Portrait)
			{
				cols = gItem.RowSize;
				rows = gItem.ColumnSize;
			}

			for(int x=0; x < cols; x++)
			{
				for(int y=0; y < rows; y++)
				{
					grid[gItem.ColumnPos + x, gItem.RowPos + y] = 1;
				}
			}
		}

		//check each block, if it's not occupied, see if it can fit the item in either orientation
		for(int i=0; i < Columns; i++)
		{
			for(int j=0; j < Rows; j++)
			{
				if(grid[i,j] == 0)
				{
					bool isOccupied = false;
					for(int x=0; x < item.GridCols; x++)
					{
						if(isOccupied || (i+x) >= Columns)
						{
							isOccupied = true;
							break;
						}
						for(int y=0; y < item.GridRows; y++)
						{
							if((j+y) >= Rows || grid[i+x, j+y] == 1)
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
							if(isOccupied || (i+x) >= Columns)
							{
								isOccupied = true;
								break;
							}
							for(int y=0; y < item.GridCols; y++)
							{
								if((j+y) >= Rows || grid[i+x, j+y] == 1)
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






	private bool IsBlockOccupied(int col, int row, out GridItem occupier)
	{
		occupier = null;
		foreach(GridItem item in Items)
		{
			if(item.State == GridItemState.Selected)
			{
				//ignore item that has been selected
				continue;
			}

			if(col >= item.ColumnPos && col < item.ColumnPos + item.ColumnSize &&
				row >= item.RowPos && row < item.RowPos + item.RowSize)
			{
				occupier = item;
				return true;
			}
		}

		return false;
	}

	private GridItem LoadGridItem(string itemID, GridItemOrient orientation)
	{
		GameObject o = GameObject.Instantiate(Resources.Load("ItemSprite_" + itemID)) as GameObject;
		UISprite sprite = o.GetComponent<UISprite>();
		GridItem item = o.GetComponent<GridItem>();
		o.transform.parent = transform;


		sprite.MakePixelPerfect();
		sprite.width = BlockSize * item.ColumnSize;
		sprite.height = BlockSize * item.RowSize;
		sprite.depth = (int)InventoryItemDepth.Normal;
		item.Sprite = sprite;

		//apply boundary
		o = GameObject.Instantiate(Resources.Load("ItemBoundary")) as GameObject;
		UISprite boundary = o.GetComponent<UISprite>();
		boundary.transform.parent = transform;
		boundary.MakePixelPerfect();
		boundary.width = sprite.width;
		boundary.height = sprite.height;
		item.Boundary = boundary;

		//quantity label
		o = GameObject.Instantiate(Resources.Load("ItemQuantity")) as GameObject;
		o.transform.parent = transform;
		o.transform.localScale = new Vector3(1, 1, 1);
		item.Quantity = o.GetComponent<UILabel>();
		item.Quantity.depth = item.Sprite.depth + 1;

		if(orientation == GridItemOrient.Portrait)
		{
			item.ToggleOrientation();
		}



		return item;

	}


}

