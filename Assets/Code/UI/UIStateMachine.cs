using UnityEngine;
using System.Collections;

public class UIStateMachine
{
	public UIStateBase State;
	public UIManager UIManager;

	public void Initialize()
	{
		UIManager = GameManager.Inst.UIManager;
		State = new UIStateNormal(this);
	}
}

public abstract class UIStateBase
{
	public UIStateMachine SM;
	public abstract void BeginState();
	public abstract void EndState();

	public virtual void UpdateState()
	{

	}
}

public class UIStateNormal : UIStateBase
{

	public UIStateNormal(UIStateMachine sm)
	{
		SM = sm;
		BeginState();
	}

	public override void BeginState ()
	{
		//setup panels
		SM.UIManager.HideAllPanels();
		SM.UIManager.BarkPanel.Show();
		SM.UIManager.HUDPanel.Show();

		//subscribe events
		UIEventHandler.OnToggleInventory -= OnToggleInventory;
		UIEventHandler.OnToggleInventory += OnToggleInventory;
		UIEventHandler.OnLootBody -= OnLootBody;
		UIEventHandler.OnLootBody += OnLootBody;
		UIEventHandler.OnLootChest -= OnLootChest;
		UIEventHandler.OnLootChest += OnLootChest;
		UIEventHandler.OnStartDialogue -= OnStartDialogue;
		UIEventHandler.OnStartDialogue += OnStartDialogue;
		UIEventHandler.OnStartTrading -= OnStartTrading;
		UIEventHandler.OnStartTrading += OnStartTrading;
		UIEventHandler.OnOpenRestPanel -= OnOpenRestPanel;
		UIEventHandler.OnOpenRestPanel += OnOpenRestPanel;
		UIEventHandler.OnOpenConfirmPanel -= OnOpenConfirmPanel;
		UIEventHandler.OnOpenConfirmPanel += OnOpenConfirmPanel;
	}

	public override void EndState ()
	{

		UIEventHandler.OnToggleInventory -= OnToggleInventory;
		UIEventHandler.OnLootBody -= OnLootBody;
		UIEventHandler.OnLootChest -= OnLootChest;
		UIEventHandler.OnStartDialogue -= OnStartDialogue;
		UIEventHandler.OnStartTrading -= OnStartTrading;
		UIEventHandler.OnOpenRestPanel -= OnOpenRestPanel;
		UIEventHandler.OnOpenConfirmPanel -= OnOpenConfirmPanel;
	}


	public void OnToggleInventory()
	{
		
		EndState();
		SM.State = new UIStateInventory(SM);
	}

	public void OnLootBody()
	{
		EndState();
		SM.State = new UIStateLootBody(SM);
	}

	public void OnLootChest()
	{
		EndState();
		SM.State = new UIStateLootChest(SM);
	}

	public void OnStartDialogue()
	{
		EndState();
		SM.State = new UIStateDialogue(SM);
	}

	public void OnStartTrading()
	{
		EndState();
		SM.State = new UIStateTrading(SM);
	}

	public void OnOpenRestPanel()
	{
		EndState();
		SM.State = new UIStateResting(SM);
	}

	public void OnOpenConfirmPanel()
	{
		EndState();
		SM.State = new UIStateConfirm(SM);
	}

}

public class UIStateInventory : UIStateBase
{
	public UIStateInventory(UIStateMachine sm)
	{
		SM = sm;
		BeginState();
	}

	public override void BeginState ()
	{
		
		//setup panels
		SM.UIManager.HideAllPanels();
		SM.UIManager.HUDPanel.Show();
		SM.UIManager.WindowPanel.Show();
		SM.UIManager.WindowPanel.InventoryPanel.Show();
		SM.UIManager.WindowPanel.BodySlotPanel.Show();

		SM.UIManager.WindowPanel.SetBackground(false);

		//subscribe events
		UIEventHandler.OnToggleInventory -= OnToggleInventory;
		UIEventHandler.OnToggleInventory += OnToggleInventory;
		UIEventHandler.OnCloseWindow -= OnToggleInventory;
		UIEventHandler.OnCloseWindow += OnToggleInventory;

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("OpenInventory"), 0.1f);
	}

	public override void EndState ()
	{
		UIEventHandler.OnToggleInventory -= OnToggleInventory;
		UIEventHandler.OnCloseWindow -= OnToggleInventory;

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("CloseInventory"), 0.1f);
	}

	public void OnToggleInventory()
	{
		EndState();
		SM.State = new UIStateNormal(SM);
	}
		

}

public class UIStateLootBody : UIStateBase
{
	public UIStateLootBody(UIStateMachine sm)
	{
		SM = sm;
		BeginState();
	}

	public override void BeginState ()
	{
		//setup panels
		SM.UIManager.HideAllPanels();
		SM.UIManager.HUDPanel.Show();
		SM.UIManager.WindowPanel.Show();
		SM.UIManager.WindowPanel.InventoryPanel.Show();
		SM.UIManager.WindowPanel.BodySlotPanel.Show();
		SM.UIManager.WindowPanel.BodyLootPanel.Show();

		SM.UIManager.WindowPanel.SetBackground(true);

		//subscribe events
		UIEventHandler.OnCloseWindow -= OnCloseWindow;
		UIEventHandler.OnCloseWindow += OnCloseWindow;
	}

	public override void EndState ()
	{
		UIEventHandler.OnCloseWindow -= OnCloseWindow;

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("CloseLootBody"), 0.1f);
	}

	public void OnCloseWindow()
	{
		EndState();
		SM.State = new UIStateNormal(SM);
	}
}

public class UIStateLootChest : UIStateBase
{
	public UIStateLootChest(UIStateMachine sm)
	{
		SM = sm;
		BeginState();
	}

	public override void BeginState ()
	{
		//setup panels
		SM.UIManager.HideAllPanels();
		SM.UIManager.HUDPanel.Show();
		SM.UIManager.WindowPanel.Show();
		SM.UIManager.WindowPanel.InventoryPanel.Show();
		SM.UIManager.WindowPanel.BodySlotPanel.Show();
		SM.UIManager.WindowPanel.ChestLootPanel.Show();

		SM.UIManager.WindowPanel.SetBackground(true);

		//subscribe events
		UIEventHandler.OnCloseWindow -= OnCloseWindow;
		UIEventHandler.OnCloseWindow += OnCloseWindow;

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("OpenContainer"), 0.5f);
	}

	public override void EndState ()
	{
		UIEventHandler.OnCloseWindow -= OnCloseWindow;

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("CloseContainer"), 0.25f);
	}

	public void OnCloseWindow()
	{
		EndState();
		SM.State = new UIStateNormal(SM);
	}
}

public class UIStateDialogue : UIStateBase
{
	public UIStateDialogue(UIStateMachine sm)
	{
		SM = sm;
		BeginState();
	}

	public override void BeginState ()
	{
		//setup panels
		SM.UIManager.HideAllPanels();
		SM.UIManager.HUDPanel.Show();
		SM.UIManager.DialoguePanel.Show();


		//subscribe events
		UIEventHandler.OnCloseWindow -= OnCloseWindow;
		UIEventHandler.OnCloseWindow += OnCloseWindow;
	}

	public override void EndState ()
	{
		UIEventHandler.OnCloseWindow -= OnCloseWindow;
	}

	public void OnCloseWindow()
	{
		EndState();
		SM.State = new UIStateNormal(SM);
	}
}

public class UIStateResting : UIStateBase
{
	public UIStateResting(UIStateMachine sm)
	{
		SM = sm;
		BeginState();
	}

	public override void BeginState ()
	{
		//setup panels
		SM.UIManager.HideAllPanels();
		SM.UIManager.HUDPanel.Show();
		SM.UIManager.RestingPanel.Show();


		//subscribe events
		UIEventHandler.OnCloseWindow -= OnCloseWindow;
		UIEventHandler.OnCloseWindow += OnCloseWindow;
	}

	public override void EndState ()
	{
		UIEventHandler.OnCloseWindow -= OnCloseWindow;
	}

	public void OnCloseWindow()
	{
		EndState();
		SM.State = new UIStateNormal(SM);
	}
}

public class UIStateConfirm : UIStateBase
{
	public UIStateConfirm(UIStateMachine sm)
	{
		SM = sm;
		BeginState();
	}

	public override void BeginState ()
	{
		//setup panels
		SM.UIManager.HideAllPanels();
		SM.UIManager.HUDPanel.Show();
		SM.UIManager.ConfirmPanel.Show();


		//subscribe events
		UIEventHandler.OnCloseWindow -= OnCloseWindow;
		UIEventHandler.OnCloseWindow += OnCloseWindow;
	}

	public override void EndState ()
	{
		UIEventHandler.OnCloseWindow -= OnCloseWindow;
	}

	public void OnCloseWindow()
	{
		EndState();
		SM.State = new UIStateNormal(SM);
	}
}

public class UIStateTrading : UIStateBase
{
	public UIStateTrading(UIStateMachine sm)
	{
		SM = sm;
		BeginState();
	}

	public override void BeginState ()
	{
		//setup panels
		SM.UIManager.HideAllPanels();
		SM.UIManager.HUDPanel.Show();
		SM.UIManager.WindowPanel.Show();
		SM.UIManager.WindowPanel.InventoryPanel.Show();
		SM.UIManager.WindowPanel.TraderItemPanel.Show();
		SM.UIManager.WindowPanel.TradingPanel.PlayerBackpack = SM.UIManager.WindowPanel.InventoryPanel.BackpackGrid;
		SM.UIManager.WindowPanel.TradingPanel.TraderItems = SM.UIManager.WindowPanel.TraderItemPanel.TraderItemGrid;
		SM.UIManager.WindowPanel.TradingPanel.Show();

		SM.UIManager.WindowPanel.SetBackground(true);

		//subscribe events
		UIEventHandler.OnCloseWindow -= OnCloseWindow;
		UIEventHandler.OnCloseWindow += OnCloseWindow;
	}

	public override void EndState ()
	{
		UIEventHandler.OnCloseWindow -= OnCloseWindow;
	}

	public void OnCloseWindow()
	{
		EndState();
		SM.State = new UIStateNormal(SM);
	}
}