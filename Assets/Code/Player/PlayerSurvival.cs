using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSurvival
{
	private List<PlayerStatBoost> _statBoosts;
	private List<RadiationSource> _staticRadiations;
	private float _totalRadiation;
	private float _radiationLevel;
	private bool _isBandageInUse;
	private float _bandageRate;
	private float _eatenCalories;

	public void Initialize()
	{
		_staticRadiations = new List<RadiationSource>();
		_statBoosts = new List<PlayerStatBoost>();

		GameObject [] objs = GameObject.FindGameObjectsWithTag("Radiation");
		foreach(GameObject o in objs)
		{
			_staticRadiations.Add(o.GetComponent<RadiationSource>());
		}

		TimerEventHandler.OnOneSecondTimer -= PerSecondUpdate;
		TimerEventHandler.OnOneSecondTimer += PerSecondUpdate;
	}
		

	public float GetRadiationLevel()
	{
		return _radiationLevel;
	}

	public float GetEatenCalories()
	{
		return _eatenCalories;
	}

	public List<PlayerStatBoost> GetStatBoosts()
	{
		return _statBoosts;
	}

	public void SetStatBoosts(List<PlayerStatBoost> boosts)
	{
		_statBoosts = boosts;
	}

	public void ResetEatenCalories()
	{
		_eatenCalories = 0;
	}


	public void PerFrameUpdate()
	{
		UpdateRadiation();
		if(_isBandageInUse)
		{
			UpdateBandage();
		}
	}

	public void PerSecondUpdate()
	{
		HumanCharacter player = GameManager.Inst.PlayerControl.SelectedPC;
		//update running speed
		if(player.MyStatus.CarryWeight <= player.MyStatus.MaxCarryWeight)
		{
			float weightRatio = player.MyStatus.CarryWeight / player.MyStatus.MaxCarryWeight;
			player.MyStatus.RunSpeedModifier = 0.8f + (1 - weightRatio) * 0.4f;
			player.MyStatus.SprintSpeedModifier = 1.0f + (1 - weightRatio) * 0.2f;
			player.MyStatus.StaminaReduceMult = Mathf.Clamp01(0.4f + weightRatio * 0.6f);
		}
		else
		{
			//only allow walk and sprint
			player.MyStatus.RunSpeedModifier = 0.8f;
			player.MyStatus.SprintSpeedModifier = 0.7f;
			player.MyStatus.StaminaReduceMult = 1.5f;
		}

		//update stamina restore speed
		player.MyStatus.StaminaRestoreSpeed = 1 + (player.MyStatus.Energy / player.MyStatus.MaxEnergy) * 5;

		//increase energy if there's consumed calories
		if(_eatenCalories > 0)
		{
			float amount = 1;
			_eatenCalories -= amount;
			player.MyStatus.SetEnergy(player.MyStatus.Energy + amount);
		
		}

		//update boosts
		List<PlayerStatBoost> boostCopy = new List<PlayerStatBoost>(_statBoosts);
		foreach(PlayerStatBoost boost in boostCopy)
		{
			if(boost.IsEnded)
			{
				_statBoosts.Remove(boost);
			}
			else
			{
				boost.PerSecondUpdate();
			}
		}
	}

	public bool UseItem(Item item, int quantity)
	{
		if(quantity > item.UseLimit)
		{
			GameManager.Inst.UIManager.SetConsoleText("Cannot use so many at once!");
		}

		if(item.Type == ItemType.Medicine)
		{
			
			string function = item.GetAttributeByName("_Function").Value.ToString();
			float functionValue = (float)item.GetAttributeByName("_FunctionValue").Value;
			if(function == "ReduceBleeding")
			{
				
				if(!_isBandageInUse)
				{
					
					_bandageRate = functionValue * quantity;
					_isBandageInUse = true;

					GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("UseBandages"), 0.4f);
					/*
					HumanCharacter player = GameManager.Inst.PlayerControl.SelectedPC;
					player.MyAI.BlackBoard.AnimationAction = AnimationActions.ApplyBandages;
					player.MyAI.BlackBoard.ActionMovementDest = player.transform.position;
					player.MyAI.BlackBoard.ActionMovementSpeed = 0;

					player.SendCommand(CharacterCommands.PlayAnimationAction);
					*/
				}
				else
				{
					GameManager.Inst.UIManager.SetConsoleText("Bandages are already being applied!");
					return false;
				}
			}
			else if(function == "RestoreHealth")
			{
				HumanCharacter player = GameManager.Inst.PlayerControl.SelectedPC;
				player.MyStatus.Health += functionValue;
				if(player.MyStatus.Health > player.MyStatus.MaxHealth)
				{
					player.MyStatus.Health = player.MyStatus.MaxHealth;
				}
			}
		}
		else if(item.Type == ItemType.Food)
		{
			if(_eatenCalories < 1000)
			{
				float calories = (float)item.GetAttributeByName("Calories").Value;
				_eatenCalories += calories;
				GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("Eat"), 0.4f);
				GameManager.Inst.UIManager.SetConsoleText(calories + " calories of food was consumed.");
				return true;
			}
			else
			{
				GameManager.Inst.UIManager.SetConsoleText("I'm too full.");
				return false;
			}
		}

		return true;
	}

	public void CompleteResting()
	{
		//check if player is near campfire
		bool isNearCampfire = false;
		Vector3 playerPos = GameManager.Inst.PlayerControl.SelectedPC.transform.position;
		GameObject [] fires = GameObject.FindGameObjectsWithTag("Campfire");
		foreach(GameObject fire in fires)
		{
			if(Vector3.Distance(playerPos, fire.transform.position) < 10)
			{
				isNearCampfire = true;
			}
		}

		CharacterStatus myStatus = GameManager.Inst.PlayerControl.SelectedPC.MyStatus;
		float energy = _eatenCalories;
		myStatus.SetEnergy(myStatus.Energy + energy * (isNearCampfire ? 2 : 1));
		_eatenCalories = 0;

		if(myStatus.Health / myStatus.MaxHealth < 0.8f)
		{
			myStatus.Health = myStatus.MaxHealth * 0.8f;
		}

		if(isNearCampfire)
		{
			GameManager.Inst.UIManager.SetConsoleText("You rested near a campfire. Stat boosts!");
			AddStatBoost(PlayerBoostType.MaxStamina, 60, 50);
		}

		myStatus.Stamina = myStatus.MaxStamina;
	}

	public void AddStatBoost(PlayerBoostType type, int duration, float boost)
	{
		//first check if any boost already exists with same type
		//if so remove it
		List<PlayerStatBoost> copy = new List<PlayerStatBoost>(_statBoosts);
		foreach(PlayerStatBoost b in copy)
		{
			if(b.Type == type)
			{
				_statBoosts.Remove(b);
			}
		}

		PlayerStatBoost newBoost = new PlayerStatBoost(type, GameManager.Inst.PlayerControl.SelectedPC.MyStatus);
		_statBoosts.Add(newBoost);
		newBoost.StartBoost(duration, boost);
	}

	public void ClearAllStatBoosts()
	{
		List<PlayerStatBoost> copy = new List<PlayerStatBoost>(_statBoosts);
		foreach(PlayerStatBoost b in copy)
		{

			_statBoosts.Remove(b);

		}
	}



	private void UpdateBandage()
	{
		float reduction = 3 * Time.deltaTime;

		if(_bandageRate > 0)
		{
			GameManager.Inst.PlayerControl.SelectedPC.MyStatus.ReduceBleeding(reduction);
			_bandageRate -= reduction;
		}
		else
		{
			_bandageRate = 0;
			_isBandageInUse = false;
		}


	}

	private void UpdateRadiation()
	{
		_totalRadiation = 0;

		Vector3 playerPos = GameManager.Inst.PlayerControl.SelectedPC.transform.position;
		foreach(RadiationSource r in _staticRadiations)
		{
			float dist = Vector3.Distance(r.transform.position, playerPos);
			if(dist < r.Range)
			{
				_totalRadiation += r.Intensity * (1 - dist / r.Range);
			}
		}

		_radiationLevel = _totalRadiation;
		_totalRadiation -= GameManager.Inst.PlayerControl.SelectedPC.MyStatus.RadiationDefense;
		if(_totalRadiation < 0)
		{
			_totalRadiation = 0;
		}

		GameManager.Inst.PlayerControl.SelectedPC.MyStatus.Radiation += _totalRadiation * Time.deltaTime;

		//reduce health if over 100 rad
		if(GameManager.Inst.PlayerControl.SelectedPC.MyStatus.Radiation >= 100)
		{
			float healthReduction = 10 * (GameManager.Inst.PlayerControl.SelectedPC.MyStatus.Radiation - 100) / 200;
			GameManager.Inst.PlayerControl.SelectedPC.MyStatus.Health -= healthReduction * Time.deltaTime;
			GameManager.Inst.CameraController.SetNoise(0.3f + healthReduction * 0.05f);
		}
		else
		{
			GameManager.Inst.CameraController.SetNoise(0.15f);
		}
			

		//find out which radiation sound to play
		Item detector = GameManager.Inst.PlayerControl.SelectedPC.Inventory.ToolSlot;
		if(detector != null && detector.GetAttributeByName("Measurement").Value == "Radiation")
		{
			float percent = Mathf.Clamp01(_radiationLevel / 30f);
			int quantized = 0;
			if(percent <= 0)
			{
				quantized = 0;
			}
			else if(percent < 0.15f && percent > 0)
			{
				quantized = 1;
			}
			else if(percent < 0.3f)
			{
				quantized = 2;
			}
			else if(percent < 0.5f)
			{
				quantized = 3;
			}
			else if(percent < 0.75f)
			{
				quantized = 4;
			}
			else
			{
				quantized = 5;
			}

			//see if detector sound is already playing at current level
			string currentClip = GameManager.Inst.SoundManager.GetDetectorCurrentClip();
			string newClip = "GeigerCounter" + quantized.ToString();
			if(!newClip.Equals(currentClip))
			{
				GameManager.Inst.SoundManager.PlayDetectorSound(newClip);
			}
		}
	}
}
