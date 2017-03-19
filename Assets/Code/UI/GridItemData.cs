using UnityEngine;
using System.Collections;

//This is a class that holds data used to 
//save and load a grid item
[System.Serializable]
public class GridItemData
{
	public int ColumnPos;
	public int RowPos;
	public int Quantity;
	public GridItemOrient Orientation;

	public Item Item;

	public GridItemData(Item item, int colPos, int rowPos, GridItemOrient orientation, int quantity)
	{
		ColumnPos = colPos;
		RowPos = rowPos;
		Orientation = orientation;
		Item = item;
		Quantity = quantity;
	}

}
