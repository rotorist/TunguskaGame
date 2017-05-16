using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryObject : MonoBehaviour 
{
	public Transform OnTarget;
	public Transform OffTarget;

	public Transform MovingPart;

	public bool IsOn;
	public bool IsTrigger;
	public bool IsReady;//when IsReady is true, player can trigger it
	public string NotReadyMessage;//When IsReady is false and player tries to trigger, display this message
	public StoryObject NextObject;
	public string OnEvent;
	public string OffEvent;
	public string RequireItemID;
	public int RequireItemQuantity;
	public bool IsConsumingItem;

	private float _delayTimer;
	private bool _delayDone;
	private bool _isTriggered;
	
	// Update is called once per frame
	void Update () 
	{
		

		if(!_delayDone && _isTriggered)
		{
			_delayTimer += Time.deltaTime;
			if(_delayTimer >= 0.5f)
			{
				_delayDone = true;
				_isTriggered = false;

				if(!IsOn || IsTrigger)
				{
					//turn it on
					if(OnEvent.Length > 0)
					{
						GameManager.Inst.QuestManager.StoryEvents[OnEvent].Trigger();
					}

					IsOn = true;
					if(NextObject != null)
					{
						NextObject.IsReady = true;
					}
				}
				else
				{
					//turn it off
					if(OffEvent.Length > 0)
					{
						GameManager.Inst.QuestManager.StoryEvents[OffEvent].Trigger();
					
					}

					IsOn = false;
					if(NextObject != null)
					{
						NextObject.IsReady = false;
					}
				}

			}

			return;
		}


		if(MovingPart != null)
		{
			if(IsOn)
			{
				MovingPart.localPosition = Vector3.MoveTowards(MovingPart.localPosition, OnTarget.localPosition, Time.time * 6);
				MovingPart.localRotation = Quaternion.Lerp(MovingPart.localRotation, OnTarget.localRotation, Time.deltaTime * 6);
			}
			else
			{
				MovingPart.localPosition = Vector3.MoveTowards(MovingPart.localPosition, OffTarget.localPosition, Time.time * 6);
				MovingPart.localRotation = Quaternion.Lerp(MovingPart.localRotation, OffTarget.localRotation, Time.deltaTime * 6);
			}
		}
	}

	public void Interact()
	{
		if(!IsReady)
		{

			return;
		}
		//check if player has required items
		if(RequireItemID.Length > 0)
		{
			CharacterInventory playerInventory = GameManager.Inst.PlayerControl.SelectedPC.Inventory;
			int itemCount = playerInventory.CountItemsInBackpack(RequireItemID);
			if(itemCount >= RequireItemQuantity)
			{
				//remove item from player
				playerInventory.RemoveItemsFromBackpack(RequireItemID, RequireItemQuantity);
			}
			else
			{
				//display message and return
				return;
			}
		}

		_isTriggered = true;


		_delayTimer = 0;
		_delayDone = false;
	}
}
