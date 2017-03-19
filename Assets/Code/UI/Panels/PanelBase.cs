using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelBase : MonoBehaviour 
{
	public bool IsActive;

	public virtual void Initialize()
	{

	}

	public virtual void PerFrameUpdate()
	{

	}

	public virtual void Show()
	{

	}

	public virtual void Hide()
	{

	}

	public virtual bool HasInventoryGrids(out List<InventoryGrid> grids)
	{
		grids = null;
		return false;
	}

	public virtual bool HasBodySlots(out List<BodySlot> bodySlots)
	{
		bodySlots = null;
		return false;
	}

	public virtual bool HasTempSlots(out List<TempSlot> tempSlots)
	{
		tempSlots = null;
		return false;
	}
}
