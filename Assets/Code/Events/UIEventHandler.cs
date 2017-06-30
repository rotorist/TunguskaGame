using UnityEngine;
using System.Collections;

public class UIEventHandler 
{
	#region Singleton 
	private static UIEventHandler _instance;
	public static UIEventHandler Instance	
	{
		get 
		{
			if (_instance == null)
				_instance = new UIEventHandler();

			return _instance;
		}
	}

	public void OnUnloadScene()
	{
		OnOpenWindow = null;
		OnCloseWindow = null;
		OnToggleInventory = null;
		OnLootBody = null;
		OnLootChest = null;
		OnStartDialogue = null;
		OnStartTrading = null;
		OnOpenRestPanel = null;
		OnOpenConfirmPanel = null;
	}

	#endregion

	#region Constructor
	public UIEventHandler()
	{

	}

	#endregion


	public delegate void GeneralUIEventDelegate();
	public static event GeneralUIEventDelegate OnOpenWindow;
	public static event GeneralUIEventDelegate OnCloseWindow;
	public static event GeneralUIEventDelegate OnToggleInventory;
	public static event GeneralUIEventDelegate OnLootBody;
	public static event GeneralUIEventDelegate OnLootChest;
	public static event GeneralUIEventDelegate OnStartDialogue;
	public static event GeneralUIEventDelegate OnStartTrading;
	public static event GeneralUIEventDelegate OnOpenRestPanel;
	public static event GeneralUIEventDelegate OnOpenConfirmPanel;
	public static event GeneralUIEventDelegate OnOpenJournalPanel;


	public void TriggerOpenWindow()
	{
		if(OnOpenWindow != null)
		{
			OnOpenWindow();
		}
	}

	public void TriggerCloseWindow()
	{
		if(OnCloseWindow != null)
		{
			if(GameManager.Inst.UIManager.WindowPanel.InventoryPanel.SelectedItem != null)
			{
				GameManager.Inst.UIManager.SetConsoleText("There's still selected item!");
			}
			else
			{
				OnCloseWindow();
			}
		}
	}

	public void TriggerToggleInventory()
	{
		if(OnToggleInventory != null)
		{
			if(GameManager.Inst.PlayerControl.SelectedPC.ActionState != HumanActionStates.None)
			{
				GameManager.Inst.UIManager.SetConsoleText("Cannot open inventory while busy!");
			}
			else if(GameManager.Inst.UIManager.WindowPanel.InventoryPanel.SelectedItem != null)
			{
				GameManager.Inst.UIManager.SetConsoleText("There's still selected item!");
			}
			else
			{
				OnToggleInventory();
			}
		}
	}

	public void TriggerLootBody()
	{
		if(OnLootBody != null)
		{
			OnLootBody();
		}
	}

	public void TriggerLootChest()
	{
		if(OnLootChest != null)
		{
			OnLootChest();
		}
	}

	public void TriggerDialogue()
	{
		if(OnStartDialogue != null)
		{
			OnStartDialogue();
		}
	}

	public void TriggerTrading()
	{
		if(OnStartTrading != null)
		{
			OnStartTrading();
		}
	}

	public void TriggerResting()
	{
		if(OnOpenRestPanel != null)
		{
			OnOpenRestPanel();
		}
	}

	public void TriggerConfirm()
	{
		if(OnOpenConfirmPanel != null)
		{
			OnOpenConfirmPanel();
		}
	}

	public void TriggerJournal()
	{
		if(OnOpenJournalPanel != null)
		{
			OnOpenJournalPanel();
		}
	}
}
