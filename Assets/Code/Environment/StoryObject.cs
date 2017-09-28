using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryObject : MonoBehaviour 
{
	public string Name;
	public Transform OnTarget;
	public Transform OffTarget;

	public Transform MovingPart;

	public bool IsOn;
	public bool IsTrigger;
	public bool IsReady;//when IsReady is true, player can trigger it
	public string NotReadyMessage;//When IsReady is false and player tries to trigger, display this message
	public string NoMoreUseMessage;
	public string ConfirmMessage;//for story objects that will consume player's item
	public string TriggerMessage;
	public StoryObject NextObject;
	public string OnEvent;
	public string OffEvent;
	public MachineAudioSource OnSound;
	public string UISound;
	public string RequireItemID;
	public int RequireItemQuantity;
	public bool IsConsumingItem;

	private bool _isNoMoreUse;
	
	// Update is called once per frame
	void Update () 
	{
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
		if(_isNoMoreUse)
		{
			GameManager.Inst.UIManager.SetConsoleText(NoMoreUseMessage);
			return;
		}

		//if there is UI sound, play
		if(!string.IsNullOrEmpty(UISound))
		{
			AudioClip clip = GameManager.Inst.SoundManager.GetClip(UISound);
			GameManager.Inst.SoundManager.UI.PlayOneShot(clip, 0.1f);
		}

		if(!IsReady)
		{
			GameManager.Inst.UIManager.SetConsoleText(NotReadyMessage);
			return;
		}





		//check if player has required items
		if(RequireItemID.Length > 0)
		{
			CharacterInventory playerInventory = GameManager.Inst.PlayerControl.SelectedPC.Inventory;
			int itemCount = playerInventory.CountItemsInBackpack(RequireItemID);
			if(itemCount < RequireItemQuantity)
			{
				//display message and return
				GameManager.Inst.UIManager.SetConsoleText(NotReadyMessage);
				return;
			}
		}

		//if there is confirm message, display it
		if(ConfirmMessage.Length > 0)
		{
			UIEventHandler.Instance.TriggerConfirm();
			GameManager.Inst.UIManager.ConfirmPanel.SetupPanel(ConfirmMessage, InteractConfirmed);
		}
		else
		{
			InteractConfirmed();
		}



	}



	public void InteractConfirmed()
	{
		
		
		if(RequireItemID.Length > 0)
		{
			CharacterInventory playerInventory = GameManager.Inst.PlayerControl.SelectedPC.Inventory;
			int itemCount = playerInventory.CountItemsInBackpack(RequireItemID);
			if(itemCount >= RequireItemQuantity)
			{
				//remove item from player
				playerInventory.RemoveItemsFromBackpack(RequireItemID, RequireItemQuantity);
				_isNoMoreUse = true;
			}
			else
			{
				//display message and return
				GameManager.Inst.UIManager.SetConsoleText(NotReadyMessage);
				return;
			}
		}

		if(!IsOn || IsTrigger)
		{
			//turn it on
			if(OnEvent.Length > 0)
			{
				GameManager.Inst.QuestManager.Scripts[OnEvent].Trigger(new object[]{});
			}

			if(IsTrigger)
			{
				_isNoMoreUse = true;

			}

			IsOn = true;
			if(NextObject != null)
			{
				NextObject.IsReady = true;
			}

			if(OnSound != null)
			{
				OnSound.TurnOn();
			}

			GameManager.Inst.UIManager.SetConsoleText(TriggerMessage);
		}
		else
		{
			//turn it off
			if(OffEvent.Length > 0)
			{
				GameManager.Inst.QuestManager.Scripts[OffEvent].Trigger(new object[]{});

			}

			IsOn = false;
			if(NextObject != null)
			{
				NextObject.IsReady = false;
			}

			if(OnSound != null)
			{
				OnSound.TurnOff();
			}
		}

	}
}
