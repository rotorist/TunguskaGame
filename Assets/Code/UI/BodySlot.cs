using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodySlot : MonoBehaviour 
{
	public UISprite Background;
	public List<GridItem> Items;
	public ItemType AllowedItemType;

	public PanelBase ParentPanel;

	public void Initialize()
	{
		Items = new List<GridItem>();
	}

	public GridItem LoadGridItem(string itemID, GridItemOrient orientation)
	{
		GameObject o = GameObject.Instantiate(Resources.Load("ItemSprite_" + itemID)) as GameObject;
		UISprite sprite = o.GetComponent<UISprite>();
		GridItem item = o.GetComponent<GridItem>();
		o.transform.parent = transform;

		sprite.MakePixelPerfect();
		sprite.depth = (int)InventoryItemDepth.Normal;
		sprite.pivot = UIWidget.Pivot.Center;
		sprite.transform.localPosition = Vector3.zero;

		item.Sprite = sprite;

		//apply boundary
		o = GameObject.Instantiate(Resources.Load("ItemBoundary")) as GameObject;
		UISprite boundary = o.GetComponent<UISprite>();
		boundary.transform.parent = transform;
		boundary.MakePixelPerfect();
		boundary.width = sprite.width;
		boundary.height = sprite.height;
		boundary.pivot = UIWidget.Pivot.Center;
		boundary.transform.localPosition = Vector3.zero;

		item.Boundary = boundary;

		//quantity label
		o = GameObject.Instantiate(Resources.Load("ItemQuantity")) as GameObject;
		o.transform.parent = transform;
		o.transform.localScale = new Vector3(1, 1, 1);
		item.Quantity = o.GetComponent<UILabel>();;



		if(orientation == GridItemOrient.Portrait)
		{
			item.ToggleOrientation();
		}

		item.IsPlayerOwned = true;

		return item;

	}

}
