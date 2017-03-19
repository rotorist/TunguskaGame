using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDPanel : PanelBase 
{
	//public List<HUDPartyMember> MembersBrief;
	//public List<GameObject> MemberSlots;
	//public List<UISprite> CommandButtons;
	public Transform LeftHUDAnchor;
	public Transform RightHUDAnchor;
	public Transform CenterHUDAnchor;
	public UILabel Console;
	public UISprite Aperture;
	public NeedleMeter EnergyMeter;
	public NeedleMeter StaminaMeter;
	public LightMeter HealthMeter;
	public NeedleMeter RadioInfectMeter;
	public NeedleMeter GeigerCounter;
	public UISprite RadiationSymbol;
	public UISprite InfectionSymbol;
	public UISprite FlipSwitch1;
	public UISprite FlipSwitch2;
	public UISprite WeaponSpriteAnchor;
	public UILabel AmmoType;
	public UILabel MagAmmo;
	public UILabel TotalAmmo;
	public UILabel Clock;


	public UISprite [] IndicatorSprites;

	public UILabel WaveIndicator;
	public UILabel HelpText;

	private bool _isShowingRadiation;
	private UISprite _weaponSprite;
	private string _magAmmoSymbol;
	private bool _isShowingGeiger;
	public Dictionary<PlayerBoostType, UISprite> _boostIndicators;

	public struct HUDPartyMember
	{
		public HumanCharacter Member;
		public UISprite Picture;
		public UISprite HealthBar;
		public GameObject Slot;
	}

	public override void Initialize ()
	{
		/*
		RebuildPartySlots();

		InputEventHandler.OnIssueTaskComplete -= OnCommandComplete;
		InputEventHandler.OnIssueTaskComplete += OnCommandComplete;
		InputEventHandler.OnGamePause -= UpdateButtonState;
		InputEventHandler.OnGamePause += UpdateButtonState;
		InputEventHandler.OnGameUnpause -= UpdateButtonState;
		InputEventHandler.OnGameUnpause += UpdateButtonState;
		InputEventHandler.OnSelectActiveMember -= OnSelectActiveMember;
		InputEventHandler.OnSelectActiveMember += OnSelectActiveMember;


		UIEventHandler.OnOpenWindow -= UpdateButtonState;
		UIEventHandler.OnOpenWindow += UpdateButtonState;
		UIEventHandler.OnCloseWindow -= UpdateButtonState;
		UIEventHandler.OnCloseWindow += UpdateButtonState;

		UpdateButtonState();
		*/

		EnergyMeter.Initialize(1);
		StaminaMeter.Initialize(1);
		HealthMeter.Initialize(1);
		RadioInfectMeter.Initialize(0.7f);
		GeigerCounter.Initialize(0);
		_isShowingRadiation = true;
		RadiationSymbol.alpha = 0.75f;
		InfectionSymbol.alpha = 0;
		FlipSwitch1.alpha = 1;
		FlipSwitch2.alpha = 0;

		NGUITools.SetActive(HelpText.gameObject, false);

		OnUpdateTotalAmmo();

		_boostIndicators = new Dictionary<PlayerBoostType, UISprite>();
		_boostIndicators.Add(PlayerBoostType.MaxStamina, IndicatorSprites[0]);

		CharacterEventHandler characterEvent = GameManager.Inst.PlayerControl.SelectedPC.MyEventHandler;
		characterEvent.OnSelectWeapon -= OnPullOut;
		characterEvent.OnSelectWeapon += OnPullOut;
		characterEvent.OnPutAwayWeapon -= OnPutAway;
		characterEvent.OnPutAwayWeapon += OnPutAway;

	}

	public override void PerFrameUpdate ()
	{
		UpdateBodyStatus();
		//RefreshCommandButtons();
		UpdateAperture();
		UpdateGeigerCounter();
		UpdateBoostIndicators();
	}

	public void SetConsoleText(string text)
	{
		Console.text = text;
	}

	public void OnButtonPress()
	{
		if(UIButton.current.name == "FlipSwitch")
		{
			if(_isShowingRadiation)
			{
				_isShowingRadiation = false;
				RadiationSymbol.alpha = 0;
				InfectionSymbol.alpha = 1;
				FlipSwitch1.alpha = 0;
				FlipSwitch2.alpha = 1;
			}
			else
			{
				_isShowingRadiation = true;
				RadiationSymbol.alpha = 0.75f;
				InfectionSymbol.alpha = 0;
				FlipSwitch1.alpha = 1;
				FlipSwitch2.alpha = 0;
			}
		}

		if(UIButton.current.name == "LongButtonOption")
		{
			if(NGUITools.GetActive(HelpText))
			{
				NGUITools.SetActive(HelpText.gameObject, false);
			}
			else
			{
				NGUITools.SetActive(HelpText.gameObject, true);
			}
		}
	}
		

	public void OnPullOut(string weaponID)
	{
		Debug.Log("Pulling out weapon " + weaponID);
		_weaponSprite = LoadItemSprite(weaponID);

		OnUpdateTotalAmmo();

		if(weaponID == "geigercounter")
		{
			_isShowingGeiger = true;
		}
	}

	public void OnPutAway()
	{
		if(_weaponSprite != null)
		{
			GameObject.Destroy(_weaponSprite.gameObject);
			_weaponSprite = null;
		}

		AmmoType.text = "";
		TotalAmmo.text = "";
		MagAmmo.text = "";

		_isShowingGeiger = false;

	}



	public void OnUpdateTotalAmmo()
	{
		
		GameObject currentWeapon = GameManager.Inst.PlayerControl.SelectedPC.MyReference.CurrentWeapon;
		Item equippedWeapon = GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.EquippedWeapon;

		if(currentWeapon == null && equippedWeapon != null)
		{

			//Item throwItem = GameManager.Inst.PlayerControl.SelectedPC.Inventory.ThrowSlot;

			if(equippedWeapon.Type == ItemType.Thrown)
			{

				int ammoInBackpack = GameManager.Inst.PlayerControl.SelectedPC.Inventory.CountItemsInBackpack(equippedWeapon.ID) + 1;
				TotalAmmo.text = ammoInBackpack.ToString();

			}
			else
			{
				AmmoType.text = "";
				TotalAmmo.text = "";
				MagAmmo.text = "";
			}


				
		}
		else if(currentWeapon != null)
		{
			Weapon myWeapon = GameManager.Inst.PlayerControl.SelectedPC.MyReference.CurrentWeapon.GetComponent<Weapon>();

			if(myWeapon != null && myWeapon.IsRanged)
			{
				GunMagazine mag = myWeapon.GetComponent<GunMagazine>();
				_magAmmoSymbol = "I";
				if(mag.MaxCapacity <= 20)
				{
					MagAmmo.fontSize = 25;
				}
				else if(mag.MaxCapacity <= 30)
				{
					MagAmmo.fontSize = 22;
				}
				else if(mag.MaxCapacity <= 45)
				{
					MagAmmo.fontSize = 16;
				}
				else
				{
					MagAmmo.fontSize = 25;
					_magAmmoSymbol = "=";
				}

				Item ammo = myWeapon.GetComponent<Gun>().GetAmmoItem();
				int ammoInBackpack = GameManager.Inst.PlayerControl.SelectedPC.Inventory.CountItemsInBackpack(ammo.ID);
				TotalAmmo.text = ammoInBackpack.ToString();
				AmmoType.text = ammo.Name; 

			}
			else if(myWeapon != null && !myWeapon.IsRanged)
			{
				AmmoType.text = "";
				TotalAmmo.text = "";
				MagAmmo.text = "";
			}

			OnUpdateMagAmmo();
		}
		else
		{
			OnPutAway();
		}



	}

	public void OnUpdateMagAmmo()
	{
		if(GameManager.Inst.PlayerControl.SelectedPC.MyReference.CurrentWeapon != null)
		{
			Weapon myWeapon = GameManager.Inst.PlayerControl.SelectedPC.MyReference.CurrentWeapon.GetComponent<Weapon>();
			if(myWeapon != null && myWeapon.IsRanged)
			{
				GunMagazine mag = myWeapon.GetComponent<GunMagazine>();
				string ammoCount = "";
				if(mag.MaxCapacity > 45)
				{
					int numberOfSymbols = Mathf.FloorToInt((mag.AmmoLeft * 1f) / mag.MaxCapacity * 10f);

					for(int i=0; i<numberOfSymbols; i++)
					{
						ammoCount = ammoCount + _magAmmoSymbol;
					}
				}
				else
				{
					for(int i=0; i<mag.AmmoLeft; i++)
					{
						ammoCount = ammoCount + _magAmmoSymbol;
					}
				}

				MagAmmo.text = ammoCount;
			}
		}
		else
		{
			MagAmmo.text = "";
		}
	}


	/*
	public void UpdateButtonState()
	{
		if(GameManager.Inst.PlayerControl.IsGamePaused && InputEventHandler.Instance.State == UserInputState.Normal && 
			GameManager.Inst.PlayerControl.SelectedPC.MyAI.ControlType == AIControlType.Player)
		{

			SetButtonState(-1);
		}
		else
		{
			SetButtonState(-1);
		}
	}



	public void OnSelectActiveMember(HumanCharacter prev)
	{
		UpdateButtonState();
	}


	public void OnMemberSlotClick()
	{
		//if player is aiming then don't do anything
		if(GameManager.Inst.PlayerControl.SelectedPC.UpperBodyState == HumanUpperBodyStates.Aim 
			|| GameManager.Inst.PlayerControl.SelectedPC.UpperBodyState == HumanUpperBodyStates.HalfAim)
		{
			return;
		}


		foreach(HUDPartyMember m in MembersBrief)
		{
			if(m.Slot == UIButton.current.gameObject)
			{
				GameManager.Inst.PlayerControl.Party.SetActiveMember(m.Member);
			}
		}
	}

	public void OnWindowPanelOpen()
	{
		SetButtonState(-1);
	}

	public void OnWindowPanelClose()
	{
		if(GameManager.Inst.PlayerControl.SelectedPC.MyAI.ControlType == AIControlType.Player && GameManager.Inst.PlayerControl.IsGamePaused)
		{
			SetButtonState(-1);
		}
		else
		{
			SetButtonState(-1);
		}
	}

	
	public void OnCommandSelectGoto()
	{
		GameManager.Inst.PlayerControl.Party.ClearTaskForSelectedMember();
		GameManager.Inst.CursorManager.SetCursorState(CursorState.Aim);
		InputEventHandler.OnIssueTaskLMB -= GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		InputEventHandler.OnIssueTaskLMB += GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		GameManager.Inst.PlayerControl.Party.SelectedMemberTask = PartyTasks.GoToGuard;
		//disable all other buttons except cancel
		SetButtonState(0);
	}

	public void OnCommandSelectSprintTo()
	{
		GameManager.Inst.PlayerControl.Party.ClearTaskForSelectedMember();
		GameManager.Inst.CursorManager.SetCursorState(CursorState.Aim);
		InputEventHandler.OnIssueTaskLMB -= GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		InputEventHandler.OnIssueTaskLMB += GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		GameManager.Inst.PlayerControl.Party.SelectedMemberTask = PartyTasks.SprintToGuard;
		SetButtonState(1);
	}

	public void OnCommandSelectGrenade()
	{
		GameManager.Inst.PlayerControl.Party.ClearTaskForSelectedMember();
		GameManager.Inst.CursorManager.SetCursorState(CursorState.Aim);
		InputEventHandler.OnIssueTaskLMB -= GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		InputEventHandler.OnIssueTaskLMB += GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		GameManager.Inst.PlayerControl.Party.SelectedMemberTask = PartyTasks.Grenade;
		SetButtonState(4);
	}

	public void OnCommandSelectAttack()
	{
		GameManager.Inst.PlayerControl.Party.ClearTaskForSelectedMember();
		GameManager.Inst.CursorManager.SetCursorState(CursorState.Aim);
		InputEventHandler.OnIssueTaskLMB -= GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		InputEventHandler.OnIssueTaskLMB += GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		GameManager.Inst.PlayerControl.Party.SelectedMemberTask = PartyTasks.AttackTarget;
		SetButtonState(3);
	}

	public void OnCommandSelectFollow()
	{
		GameManager.Inst.PlayerControl.Party.ClearTaskForSelectedMember();
		GameManager.Inst.CursorManager.SetCursorState(CursorState.Aim);
		InputEventHandler.OnIssueTaskLMB -= GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		InputEventHandler.OnIssueTaskLMB += GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		GameManager.Inst.PlayerControl.Party.SelectedMemberTask = PartyTasks.Follow;
		SetButtonState(2);
	}

	public void OnCommandSelectCancel()
	{
		GameManager.Inst.CursorManager.SetCursorState(CursorState.Default);
		GameManager.Inst.PlayerControl.Party.SelectedMemberTask = PartyTasks.Default;
		GameManager.Inst.PlayerControl.Party.ClearTaskForSelectedMember();
		SetButtonState(8);
	}

	public void OnCommandSelectToggleCrouch()
	{
		if(GameManager.Inst.PlayerControl.SelectedPC.CurrentStance == HumanStances.Crouch)
		{
			GameManager.Inst.PlayerControl.SelectedPC.SendCommand(CharacterCommands.StopCrouch);
			CommandButtons[6].spriteName = "CommandStand";
			CommandButtons[6].GetComponent<UIButton>().normalSprite = "CommandStand";
		}
		else
		{
			GameManager.Inst.PlayerControl.SelectedPC.SendCommand(CharacterCommands.Crouch);
			CommandButtons[6].spriteName = "CommandCrouch";
			CommandButtons[6].GetComponent<UIButton>().normalSprite = "CommandCrouch";
		}
	}

	public void OnCommandSelectToggleHoldFire()
	{
		if(GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.GuardLevel > 0)
		{
			GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.GuardLevel = 0;
			CommandButtons[7].spriteName = "CommandHoldFire";
			CommandButtons[7].GetComponent<UIButton>().normalSprite = "CommandHoldFire";
		}
		else
		{
			GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.GuardLevel = 2;
			CommandButtons[7].spriteName = "CommandFireAtWill";
			CommandButtons[7].GetComponent<UIButton>().normalSprite = "CommandFireAtWill";
		}
	}

	public void OnCommandComplete()
	{
		GameManager.Inst.CursorManager.SetCursorState(CursorState.Default);
		InputEventHandler.OnIssueTaskRMB -= GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		InputEventHandler.OnIssueTaskRMB += GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		InputEventHandler.OnIssueTaskLMB -= GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		GameManager.Inst.PlayerControl.Party.SelectedMemberTask = PartyTasks.Default;

		SetButtonState(8);
	}
	*/


	private void UpdateAperture()
	{
		GameObject o = GameManager.Inst.PlayerControl.SelectedPC.MyReference.CurrentWeapon;
		if(o == null || InputEventHandler.Instance.State != UserInputState.Normal)
		{
			NGUITools.SetActive(Aperture.gameObject, false);
			return;
		}

		Weapon weapon = o.GetComponent<Weapon>();

		if(weapon != null && weapon.IsScoped && GameManager.Inst.PlayerControl.SelectedPC.UpperBodyState == HumanUpperBodyStates.Aim)
		{

			NGUITools.SetActive(Aperture.gameObject, true);


			Aperture.transform.position = GameManager.Inst.CursorManager.ActiveCursor.transform.position;
		}
		else
		{
			NGUITools.SetActive(Aperture.gameObject, false);
		}
	}



	private void UpdateBodyStatus()
	{
		Character player = GameManager.Inst.PlayerControl.SelectedPC;

		//health
		HealthMeter.SetValue(Mathf.Clamp01(player.MyStatus.Health/player.MyStatus.MaxHealth));

		if(player.MyStatus.BleedingDuration > 0)
		{
			HealthMeter.StartFlashingLastLight(0.1f + 0.4f * (5 - player.MyStatus.BleedingSpeed)/5);
		}
		else
		{
			HealthMeter.StopFlashingLastLight();
		}

		//stamina
		float targetStamina = Mathf.Clamp01(player.MyStatus.Stamina / player.MyStatus.MaxStamina);
		StaminaMeter.SetValue(Mathf.Lerp(StaminaMeter.GetValue(), targetStamina, Time.deltaTime * 5));

		//energy
		float targetEnergy = Mathf.Clamp01(player.MyStatus.Energy / player.MyStatus.MaxEnergy);
		EnergyMeter.SetValue(Mathf.Lerp(EnergyMeter.GetValue(), targetEnergy, Time.deltaTime * 5));

		//radiation
		if(RadiationSymbol.alpha > 0)
		{
			float targetRadiation = Mathf.Clamp01(player.MyStatus.Radiation / 300);
			RadioInfectMeter.SetValue(Mathf.Lerp(RadioInfectMeter.GetValue(), targetRadiation, Time.deltaTime * 5));
		}
	}

	private void UpdateGeigerCounter()
	{
		if(_isShowingGeiger)
		{
			GeigerCounter.transform.localPosition = Vector3.Lerp(GeigerCounter.transform.localPosition, new Vector3(0, 100, 0), 10 * Time.deltaTime);
			float targetRadiation = GameManager.Inst.PlayerControl.Survival.GetRadiationLevel();
			targetRadiation += UnityEngine.Random.Range(-4f, 4f) * targetRadiation / 30;
			targetRadiation = Mathf.Clamp01(targetRadiation / 30);
			GeigerCounter.SetValue(Mathf.Lerp(GeigerCounter.GetValue(), targetRadiation, Time.deltaTime * 10));

			float radiationDefense = GameManager.Inst.PlayerControl.SelectedPC.MyStatus.RadiationDefense;
			radiationDefense = Mathf.Clamp01(radiationDefense / 30);
			GeigerCounter.SetSecondaryValue(radiationDefense);
		}
		else
		{
			GeigerCounter.transform.localPosition = Vector3.Lerp(GeigerCounter.transform.localPosition, new Vector3(0, -130, 0), 7 * Time.deltaTime);
		}
	}

	private void UpdateBoostIndicators()
	{
		foreach(KeyValuePair<PlayerBoostType, UISprite> indicator in _boostIndicators)
		{
			indicator.Value.alpha = 0;
		}

		List<PlayerStatBoost> boosts = GameManager.Inst.PlayerControl.Survival.GetStatBoosts();
		foreach(PlayerStatBoost boost in boosts)
		{
			if(_boostIndicators.ContainsKey(boost.Type))
			{
				if(!boost.IsEnded)
				{
					_boostIndicators[boost.Type].alpha = 1;
				}
			}
		}
	}

	private UISprite LoadItemSprite(string itemID)
	{
		OnPutAway();

		GameObject o = GameObject.Instantiate(Resources.Load("ItemSprite_" + itemID)) as GameObject;
		UISprite sprite = o.GetComponent<UISprite>();
		GridItem item = o.GetComponent<GridItem>();
		o.transform.parent = WeaponSpriteAnchor.transform;
		sprite.pivot = UIWidget.Pivot.Center;
		o.transform.localPosition = Vector3.zero;


		sprite.MakePixelPerfect();
		//adjust size round 1
		sprite.width = WeaponSpriteAnchor.width;
		sprite.height = Mathf.FloorToInt(item.Sprite.width * ((item.RowSize * 1f) / item.ColumnSize));
		//adjust size round 2
		if(sprite.height > WeaponSpriteAnchor.height)
		{
			sprite.height = WeaponSpriteAnchor.height;
			sprite.width = Mathf.FloorToInt(item.Sprite.height * ((item.ColumnSize * 1f) / item.RowSize));
		}

		sprite.depth = (int)InventoryItemDepth.Normal;

		Destroy(o.GetComponent<GridItem>());
		Destroy(o.GetComponent<BoxCollider>());

		return sprite;
	}

	/*
	private void RebuildPartySlots()
	{
		//load all the members from player party
		int count = GameManager.Inst.PlayerControl.Party.Members.Count;
		MembersBrief = new List<HUDPartyMember>();
		int i = 0;
		foreach(HumanCharacter member in GameManager.Inst.PlayerControl.Party.Members)
		{
			GameObject slot = MemberSlots[i];
			GameObject o = GameObject.Instantiate(Resources.Load("CharPic" + member.CharacterID)) as GameObject;
			o.transform.parent = slot.transform;
			o.transform.localPosition = Vector3.zero;
			UISprite pic = o.GetComponent<UISprite>();
			pic.MakePixelPerfect();
			pic.width = 100;
			pic.height = 100;


			o = GameObject.Instantiate(Resources.Load("HealthBar")) as GameObject;
			o.transform.parent = slot.transform;
			o.transform.localPosition = new Vector3(-50, -50, 0);
			UISprite bar = o.GetComponent<UISprite>();
			bar.MakePixelPerfect();
			bar.width = 100;
			bar.height = 15;

			HUDPartyMember m = new HUDPartyMember();
			m.Member = member;
			m.Slot = slot;
			m.Picture = pic;
			m.HealthBar = bar;

			MembersBrief.Add(m);

			i++;
		}
	}

	private void RefreshCommandButtons()
	{
		if(GameManager.Inst.PlayerControl.SelectedPC.CurrentStance == HumanStances.Crouch)
		{
			CommandButtons[6].spriteName = "CommandCrouch";
			CommandButtons[6].GetComponent<UIButton>().normalSprite = "CommandCrouch";
		}
		else
		{
			CommandButtons[6].spriteName = "CommandStand";
			CommandButtons[6].GetComponent<UIButton>().normalSprite = "CommandStand";
		}

		if(GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.GuardLevel > 0)
		{
			CommandButtons[7].spriteName = "CommandFireAtWill";
			CommandButtons[7].GetComponent<UIButton>().normalSprite = "CommandFireAtWill";

		}
		else
		{
			CommandButtons[7].spriteName = "CommandHoldFire";
			CommandButtons[7].GetComponent<UIButton>().normalSprite = "CommandHoldFire";
		}
	}

	private void SetButtonState(int buttonEnabled)
	{
		if(buttonEnabled < 0)
		{
			//disable and hide all buttons
			foreach(UISprite button in CommandButtons)
			{
				
				button.color = new Color(255, 255, 255);
				button.alpha = 1;
				button.GetComponent<UIButton>().enabled = false;
				NGUITools.SetActive(button.gameObject, false);
			}

			NGUITools.SetActive(Console.gameObject, true);
		}
		else if(buttonEnabled > 7)
		{
			//enable and show all buttons
			foreach(UISprite button in CommandButtons)
			{
				button.alpha = 1;
				button.color = new Color(255, 255, 255);
				button.GetComponent<UIButton>().enabled = true;
				NGUITools.SetActive(button.gameObject, true);
			}

			NGUITools.SetActive(Console.gameObject, false);
		}
		else
		{
			foreach(UISprite button in CommandButtons)
			{
				if(button != CommandButtons[buttonEnabled] && button != CommandButtons[5]) //let 5 which is cancel button show
				{
					button.alpha = 1;
					button.color = new Color(0, 0, 0);
					button.GetComponent<UIButton>().enabled = false;
				}
				else
				{
					button.alpha = 1;
					button.color = new Color(255, 255, 255);
					button.GetComponent<UIButton>().enabled = true;
				}
			}
		}
	}

	*/
}

