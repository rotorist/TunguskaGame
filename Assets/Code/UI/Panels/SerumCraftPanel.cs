using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class SerumCraftPanel : PanelBase
{
	public TempSlot ProductSlot;
	public UISlider TemperatureSlider;
	public UILabel TemperatureLabel;
	public UIButton CraftButton;
	public InventoryGrid IngredientGrid;

	private float _temperature; //0=low, 0.25=medium low, 0.5=medium, 0.75=medium high, 1=high

	public override void Initialize ()
	{
		IngredientGrid.Initialize(this);
		Hide();
	}

	public override void PerFrameUpdate ()
	{
		

	}

	public override void Show ()
	{

		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;

		InputEventHandler.OnPopupMouseWheel -= OnMouseWheelInput;
		InputEventHandler.OnPopupMouseWheel += OnMouseWheelInput;

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("OpenSplitMenu"), 0.5f);
	}

	public override void Hide ()
	{
		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;

		InputEventHandler.OnPopupMouseWheel -= OnMouseWheelInput;


	}

	public override bool HasInventoryGrids (out List<InventoryGrid> grids)
	{
		grids = new List<InventoryGrid>();
		grids.Add(IngredientGrid);
		return true;
	}

	public override bool HasBodySlots (out List<BodySlot> bodySlots)
	{
		bodySlots = null;
		return false;
	}

	public override bool HasTempSlots (out List<TempSlot> tempSlots)
	{
		tempSlots = new List<TempSlot>();
		tempSlots.Add(ProductSlot);

		return true;
	}

	public void OnSliderValueChange()
	{
		//_hours = 3 + Mathf.RoundToInt(HoursSlider.value * 5);
		//HoursLabel.text = "Hours To Rest: " + _hours.ToString();
		_temperature = TemperatureSlider.value;
		string temperatureDisplay = "";
		if(_temperature == 0)
		{
			temperatureDisplay = "Low";
		}
		else if(_temperature == 0.25f)
		{
			temperatureDisplay = "Medium-Low";
		}
		else if(_temperature == 0.5f)
		{
			temperatureDisplay = "Medium";
		}
		else if(_temperature == 0.75f)
		{
			temperatureDisplay = "Medium-High";
		}
		else if(_temperature == 1f)
		{
			temperatureDisplay = "High";
		}

		TemperatureLabel.text = "Temperature: " + temperatureDisplay;
	}

	public void OnMouseWheelInput(float movement)
	{
		float normalizedMovement = 0;
		if(movement > 0)
		{
			normalizedMovement = 1;
		}
		else
		{
			normalizedMovement = -1;
		}

		//HoursSlider.value += normalizedMovement /  6;
	}

	public void OnCraftButtonClick()
	{
		string hint;
		Item serum = GameManager.Inst.DBManager.DBHandlerItem.CheckSerumRecipe(IngredientGrid.Items, _temperature, out hint);
		GameManager.Inst.UIManager.SetConsoleText(hint);
		if(serum != null)
		{
			GridItem item = ProductSlot.LoadGridItem(serum.SpriteName, GridItemOrient.Landscape);
			item.Item = serum;
			item.SetQuantity(1);
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddItemToTempSlot(item, ProductSlot);
		}

		//remove all items from ingredients
		List<GridItem> ingredientsCopy = new List<GridItem>(IngredientGrid.Items);
		foreach(GridItem item in ingredientsCopy)
		{
			GameManager.Inst.UIManager.WindowPanel.InventoryPanel.DestroyItem(item);
		}
		IngredientGrid.Items.Clear();

	}

	public bool IsIngredientGridEmpty()
	{
		return IngredientGrid.Items.Count <= 0;
	}

	public bool IsProductSlotEmpty()
	{
		return ProductSlot.Items.Count <= 0;
	}
}
