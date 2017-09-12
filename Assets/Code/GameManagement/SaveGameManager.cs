using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveGameManager
{
	public SaveGame CurrentSave;

	public void Save(string saveGameName, string newLevelName)
	{
		CurrentSave = new SaveGame();

		//save player location
		CurrentSave.PlayerLocation = new float[3];
		CurrentSave.PlayerLocation[0] = GameManager.Inst.PlayerControl.SelectedPC.transform.position.x;
		CurrentSave.PlayerLocation[1] = GameManager.Inst.PlayerControl.SelectedPC.transform.position.y;
		CurrentSave.PlayerLocation[2] = GameManager.Inst.PlayerControl.SelectedPC.transform.position.z;
		//save player status
		CurrentSave.PlayerStatus = GameManager.Inst.PlayerControl.SelectedPC.MyStatus.Data;
		//save player inventory
		CurrentSave.PlayerInventory = new CharacterInventorySaveData();
		CurrentSave.PlayerInventory.ArmorSlot = GameManager.Inst.PlayerControl.SelectedPC.Inventory.ArmorSlot;
		CurrentSave.PlayerInventory.HeadSlot = GameManager.Inst.PlayerControl.SelectedPC.Inventory.HeadSlot;
		CurrentSave.PlayerInventory.RifleSlot = GameManager.Inst.PlayerControl.SelectedPC.Inventory.RifleSlot;
		CurrentSave.PlayerInventory.SideArmSlot = GameManager.Inst.PlayerControl.SelectedPC.Inventory.SideArmSlot;
		CurrentSave.PlayerInventory.ThrowSlot = GameManager.Inst.PlayerControl.SelectedPC.Inventory.ThrowSlot;
		CurrentSave.PlayerInventory.ToolSlot = GameManager.Inst.PlayerControl.SelectedPC.Inventory.ToolSlot;
		CurrentSave.PlayerInventory.BackpackCols = GameManager.Inst.PlayerControl.SelectedPC.Inventory.BackpackCols;
		CurrentSave.PlayerInventory.BackpackRows = GameManager.Inst.PlayerControl.SelectedPC.Inventory.BackpackRows;
		CurrentSave.PlayerInventory.Backpack = new List<GridItemData>(GameManager.Inst.PlayerControl.SelectedPC.Inventory.Backpack);
		//save player boosts
		CurrentSave.PlayerBoosts = GameManager.Inst.PlayerControl.Survival.GetStatBoosts();
		CurrentSave.PlayerFirstName = GameManager.Inst.PlayerProgress.PlayerFirstName;
		CurrentSave.PlayerLastName = GameManager.Inst.PlayerProgress.PlayerLastName;
		CurrentSave.DiscoveredTopics = new List<string>(GameManager.Inst.PlayerProgress.DiscoveredTopics);
		CurrentSave.JournalEntries = new List<List<string>>(GameManager.Inst.PlayerProgress.JournalEntries);

		//save story conditions
		CurrentSave.ItemConditions = new List<StoryConditionItem>();
		CurrentSave.TriggerConditions = new List<StoryConditionTrigger>();
		foreach(KeyValuePair<string, StoryCondition> storyCondition in GameManager.Inst.QuestManager.StoryConditions)
		{
			if(storyCondition.Value.Type == StoryConditionType.Item)
			{
				CurrentSave.ItemConditions.Add((StoryConditionItem)storyCondition.Value);
			}
			else if(storyCondition.Value.Type == StoryConditionType.Trigger)
			{
				CurrentSave.TriggerConditions.Add((StoryConditionTrigger)storyCondition.Value);
			}
		}

		//save story event handler
		LinkedList<StoryEventListener> [] storyListenerLists = StoryEventHandler.Instance.AllListenerLists;
		CurrentSave.StoryListenerLists = new List<StoryEventListener>[storyListenerLists.Length];
		for(int i=0; i<storyListenerLists.Length; i++)
		{
			List<StoryEventListener> list = StoryEventHandler.Instance.ConvertLinkedListenerToList(storyListenerLists[i]);
			CurrentSave.StoryListenerLists[i] = list;
		}

		CurrentSave.StoryEventList = StoryEventHandler.Instance.ConvertQueueStoryEventToList(StoryEventHandler.Instance.StoryEventQueue);
		CurrentSave.CurrentStoryEvent = StoryEventHandler.Instance.CurrentStoryEvent;
		CurrentSave.IsCurrentEventDone = StoryEventHandler.Instance.IsCurrentEventDone;


		//create new level data after removing existing one
		if(GameManager.Inst.WorldManager.AllLevels.Contains(GameManager.Inst.WorldManager.CurrentLevel))
		{
			GameManager.Inst.WorldManager.AllLevels.Remove(GameManager.Inst.WorldManager.CurrentLevel);
		}

		CurrentSave.CurrentDay = GameManager.Inst.WorldManager.CurrentDay;
		CurrentSave.CurrentTime = GameManager.Inst.WorldManager.CurrentTime;

		Level currentLevel = new Level();
		currentLevel.Name = GameManager.Inst.WorldManager.CurrentLevel.Name;


		//save pickup items in the scene
		//first remove all pickup items from list from current level
		List<PickupItemData> pickupDataList = new List<PickupItemData>();

		//now go through all pickup items in the scene and create pickup data list
		GameObject [] objects = GameObject.FindGameObjectsWithTag("PickupItem");
		foreach(GameObject o in objects)
		{
			PickupItem pickup = o.GetComponent<PickupItem>();
			PickupItemData data = new PickupItemData();
			data.ItemID = pickup.Item.ID;
			data.Quantity = pickup.Quantity;
			data.Pos = new SerVector3(pickup.transform.position);
			data.EulerAngles = new SerVector3(pickup.transform.localEulerAngles);

			pickupDataList.Add(data);
		}

		currentLevel.PickupItemDatas = pickupDataList;


		//save characters in this level
		currentLevel.Characters = new List<CharacterSaveData>();
		GameObject [] characters = GameObject.FindGameObjectsWithTag("NPC");
		foreach(GameObject o in characters)
		{
			//only save character who have household
			Character character = o.GetComponent<Character>();

			if(character.MyAI.Squad == null || character.MyAI.Squad.Household == null || character.MyAI.Squad.Household.CurrentSquad.Faction != character.Faction)
			{
				continue;
			}

			CharacterSaveData saveData = new CharacterSaveData();
			saveData.GoapID = character.GoapID;
			saveData.CharacterID = character.CharacterID;
			saveData.Name = character.Name;
			saveData.Title = character.Title;
			saveData.GOName = character.name;
			saveData.CharacterType = character.CharacterType;
			saveData.SquadID = character.MyAI.Squad.Household.CurrentSquad.ID;
			saveData.Faction = character.Faction;
			saveData.IsCommander = character.IsCommander;
			saveData.IsEssential = character.IsEssential;
			saveData.StatusData = character.MyStatus.Data;
			saveData.Pos = new SerVector3(character.transform.position);

			saveData.Inventory = new CharacterInventorySaveData();
			saveData.Inventory.ArmorSlot = character.Inventory.HeadSlot;
			saveData.Inventory.HeadSlot = character.Inventory.HeadSlot;
			saveData.Inventory.RifleSlot = character.Inventory.RifleSlot;
			saveData.Inventory.SideArmSlot = character.Inventory.SideArmSlot;
			saveData.Inventory.ThrowSlot = character.Inventory.ThrowSlot;
			saveData.Inventory.ToolSlot = character.Inventory.ToolSlot;
			saveData.Inventory.BackpackCols = character.Inventory.BackpackCols;
			saveData.Inventory.BackpackRows = character.Inventory.BackpackRows;
			saveData.Inventory.Backpack = new List<GridItemData>(character.Inventory.Backpack);

			currentLevel.Characters.Add(saveData);
		}


		//save traders for current level
		List<TraderData> traders = new List<TraderData>();
		foreach(HumanCharacter character in GameManager.Inst.NPCManager.HumansInScene)
		{
			Trader trader = character.GetComponent<Trader>();

			if(trader != null)
			{
				TraderData traderData = new TraderData();
				traderData.CharacterID = character.CharacterID;
				traderData.Cash = trader.Cash;
				traderData.Tier = trader.Tier;
				traderData.TraderInventory = trader.TraderInventory;
				traderData.SupplyRenewTimer = trader.SupplyRenewTimer;
				traders.Add(traderData);
			}
		}

		currentLevel.Traders = traders;

		//save chests
		//first remove all chests from list from current level
		List<ChestData> chestDataList = new List<ChestData>();


		//now go through all chests in the scene and create pickup data list
		GameObject [] chests = GameObject.FindGameObjectsWithTag("Chest");
		foreach(GameObject o in chests)
		{
			Chest chest = o.GetComponent<Chest>();
			ChestData data = new ChestData();
			data.ChestID = chest.ChestID;
			data.ColSize = chest.ColSize;
			data.RowSize = chest.RowSize;
			data.Items = chest.Items;
			data.IsLocked = chest.IsLocked;
			data.KeyID = chest.KeyID;

			chestDataList.Add(data);
		}

		currentLevel.ChestDatas = chestDataList;

		//save households
		currentLevel.Households = new List<HouseholdSaveData>();
		Household [] households = (Household[])GameObject.FindObjectsOfType<Household>();
		foreach(Household household in households)
		{
			HouseholdSaveData saveData = new HouseholdSaveData();
			saveData.HouseholdName = household.name;
			if(household.CurrentSquad != null)
			{
				saveData.CurrentSquadID = household.CurrentSquad.ID;
				saveData.CurrentSquadTier = household.CurrentSquad.Tier;
				saveData.OwningFaction = household.CurrentSquad.Faction;
			}
			else
			{
				saveData.CurrentSquadID = "";
				saveData.CurrentSquadTier = 1;
			}

			saveData.IsRefilledToday = household.IsRefilledToday;
			saveData.Expedition1SentToday = household.Expedition1SentToday;
			saveData.Expedition2SentToday = household.Expedition2SentToday;
			saveData.ExpeditionTime1 = household.ExpeditionTime1;
			saveData.ExpeditionTime2 = household.ExpeditionTime2;

			currentLevel.Households.Add(saveData);
		}

		//save doors
		currentLevel.Doors = new List<DoorSaveData>();
		GameObject [] doors = GameObject.FindGameObjectsWithTag("Door");
		foreach(GameObject o in doors)
		{
			Door door = o.GetComponent<Door>();
			DoorSaveData saveData = new DoorSaveData();
			saveData.ID = door.ID;
			saveData.IsLocked = door.IsLocked;
			saveData.IsOpen = door.IsOpen;

			currentLevel.Doors.Add(saveData);
		}


		GameManager.Inst.WorldManager.AllLevels.Add(currentLevel);
		CurrentSave.Levels = GameManager.Inst.WorldManager.AllLevels;
		CurrentSave.CurrentEnvironmentName = GameManager.Inst.WorldManager.CurrentEnvironment.Name;

		//save factions
		CurrentSave.Factions = new List<KeyValuePair<Faction, FactionData>>();
		foreach(KeyValuePair<Faction, FactionData> factionData in GameManager.Inst.NPCManager.AllFactions)
		{
			factionData.Value.PrepareSave();
			CurrentSave.Factions.Add(factionData);
		}


		//build the save file
		if(!String.IsNullOrEmpty(newLevelName))
		{
			CurrentSave.LevelToLoad = newLevelName;
			saveGameName = "Autosave - " + newLevelName;
		}
		else
		{
			CurrentSave.LevelToLoad = GameManager.Inst.WorldManager.CurrentLevelName;
		}

		string fullPath = Application.persistentDataPath + "/" + saveGameName + ".dat";

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file;
		if(File.Exists(fullPath))
		{
			file = File.Open(fullPath, FileMode.Open);
		}
		else
		{
			file = File.Create(fullPath);
		}

		bf.Serialize(file, CurrentSave);

		Debug.Log(saveGameName + " has been saved");
	}

	public string LoadLevelName(string saveGameName)
	{
		string fullPath = Application.persistentDataPath + "/" + saveGameName + ".dat";
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file;
		if(File.Exists(fullPath))
		{
			file = File.Open(fullPath, FileMode.Open);
		}
		else
		{
			return "";
		}

		SaveGame temp = (SaveGame)bf.Deserialize(file);

		return temp.LevelToLoad;
	}


	public bool Load(string saveGameName)
	{
		string fullPath = Application.persistentDataPath + "/" + saveGameName + ".dat";
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file;
		if(File.Exists(fullPath))
		{
			file = File.Open(fullPath, FileMode.Open);
		}
		else
		{
			return false;
		}

		CurrentSave = (SaveGame)bf.Deserialize(file);

		//load level
		GameManager.Inst.WorldManager.AllLevels = CurrentSave.Levels;
		foreach(Level level in GameManager.Inst.WorldManager.AllLevels)
		{
			Debug.Log("Loading levels " + level.Name);
			if(level.Name == CurrentSave.LevelToLoad)
			{
				GameManager.Inst.WorldManager.CurrentLevel = level;
				break;
			}
		}

		GameManager.Inst.WorldManager.CurrentDay = CurrentSave.CurrentDay;
		GameManager.Inst.WorldManager.CurrentTime = CurrentSave.CurrentTime;

		GameManager.Inst.WorldManager.ChangeEnvironment(CurrentSave.CurrentEnvironmentName);

		//load player basics
		GameManager.Inst.PlayerProgress.PlayerFirstName = CurrentSave.PlayerFirstName;
		GameManager.Inst.PlayerProgress.PlayerLastName = CurrentSave.PlayerLastName;
		//load progress
		GameManager.Inst.PlayerProgress.DiscoveredTopics = new List<string>(CurrentSave.DiscoveredTopics);
		GameManager.Inst.PlayerProgress.JournalEntries = new List<List<string>>(CurrentSave.JournalEntries);
		//load player status
		GameManager.Inst.PlayerControl.SelectedPC.MyStatus.Data = CurrentSave.PlayerStatus;
		//load player boosts
		GameManager.Inst.PlayerControl.Survival.SetStatBoosts(CurrentSave.PlayerBoosts);
		//load player inventory
		GameManager.Inst.PlayerControl.SelectedPC.Inventory.ArmorSlot = CurrentSave.PlayerInventory.ArmorSlot;
		GameManager.Inst.PlayerControl.SelectedPC.Inventory.HeadSlot = CurrentSave.PlayerInventory.HeadSlot;
		GameManager.Inst.PlayerControl.SelectedPC.Inventory.RifleSlot = CurrentSave.PlayerInventory.RifleSlot;
		GameManager.Inst.PlayerControl.SelectedPC.Inventory.SideArmSlot = CurrentSave.PlayerInventory.SideArmSlot;
		GameManager.Inst.PlayerControl.SelectedPC.Inventory.ThrowSlot = CurrentSave.PlayerInventory.ThrowSlot;
		GameManager.Inst.PlayerControl.SelectedPC.Inventory.ToolSlot = CurrentSave.PlayerInventory.ToolSlot;
		GameManager.Inst.PlayerControl.SelectedPC.Inventory.BackpackCols = CurrentSave.PlayerInventory.BackpackCols;
		GameManager.Inst.PlayerControl.SelectedPC.Inventory.BackpackRows = CurrentSave.PlayerInventory.BackpackRows;
		GameManager.Inst.PlayerControl.SelectedPC.Inventory.Backpack = new List<GridItemData>(CurrentSave.PlayerInventory.Backpack);
		GameManager.Inst.PlayerControl.SelectedPC.Inventory.PostLoad();

		GameManager.Inst.PlayerControl.SelectedPC.ArmorSystem.SwitchToArmor(GameManager.Inst.PlayerControl.SelectedPC.Inventory.ArmorSlot);
		GameManager.Inst.PlayerControl.SelectedPC.ArmorSystem.SwitchToHelmet(GameManager.Inst.PlayerControl.SelectedPC.Inventory.HeadSlot);
		GameManager.Inst.PlayerControl.SelectedPC.MyAI.WeaponSystem.LoadWeaponsFromInventory(false);

		//place player in last saved location
		GameManager.Inst.PlayerControl.SelectedPC.transform.position = new Vector3(CurrentSave.PlayerLocation[0], CurrentSave.PlayerLocation[1], CurrentSave.PlayerLocation[2]);

		//load story conditions
		GameManager.Inst.QuestManager.StoryConditions = new Dictionary<string, StoryCondition>();
		foreach(StoryConditionItem condition in CurrentSave.ItemConditions)
		{
			GameManager.Inst.QuestManager.StoryConditions.Add(condition.ID, condition);
		}
		foreach(StoryConditionTrigger condition in CurrentSave.TriggerConditions)
		{
			GameManager.Inst.QuestManager.StoryConditions.Add(condition.ID, condition);
		}

		//load story event handler
		LinkedList<StoryEventListener> [] storyListenerLists = StoryEventHandler.Instance.AllListenerLists;

		for(int i=0; i<storyListenerLists.Length; i++)
		{
			storyListenerLists[i].Clear();
			for(int j=0; j<CurrentSave.StoryListenerLists[i].Count; j++)
			{
				storyListenerLists[i].AddLast(CurrentSave.StoryListenerLists[i][j]);

			}
		}

		StoryEventHandler.Instance.StoryEventQueue = StoryEventHandler.Instance.ConvertListStoryEventToQueue(CurrentSave.StoryEventList);
		StoryEventHandler.Instance.CurrentStoryEvent = CurrentSave.CurrentStoryEvent;
		StoryEventHandler.Instance.IsCurrentEventDone = CurrentSave.IsCurrentEventDone;


		//load pickup items
		Level currentLevel = GameManager.Inst.WorldManager.CurrentLevel;
		//remove all existing pickup items
		GameObject [] objects = GameObject.FindGameObjectsWithTag("PickupItem");

		foreach(GameObject o in objects)
		{
			GameObject.Destroy(o);
		}
		//create new ones
		foreach(PickupItemData pickupItemData in currentLevel.PickupItemDatas)
		{
			//create an Item from ItemID
			Item item = GameManager.Inst.ItemManager.LoadItem(pickupItemData.ItemID);
			var resource = Resources.Load(item.PrefabName + "Pickup");
			if(resource != null)
			{
				GameObject pickup = GameObject.Instantiate(resource) as GameObject;
				pickup.transform.position = pickupItemData.Pos.ConvertToVector3();
				pickup.transform.localEulerAngles = pickupItemData.EulerAngles.ConvertToVector3();
				Transform parent = GameManager.Inst.ItemManager.FindPickupItemParent(pickup.transform);
				if(parent != null)
				{
					pickup.transform.parent = parent;
				}
				pickup.GetComponent<PickupItem>().Item = item;
				pickup.GetComponent<PickupItem>().Quantity = pickupItemData.Quantity;
			}
		}

		//load Chests 
		//clear all existing chests in this level and then add new content
		objects = GameObject.FindGameObjectsWithTag("Chest");

		foreach(GameObject o in objects)
		{
			Chest chest = o.GetComponent<Chest>();
			chest.Items.Clear();
			foreach(ChestData chestData in currentLevel.ChestDatas)
			{
				if(chest.ChestID == chestData.ChestID)
				{
					chest.Items = chestData.Items;
					chest.IsLocked = chestData.IsLocked;
					chest.KeyID = chestData.KeyID;
					chest.PostLoad();
				}
			}
		}

		//load doors
		GameObject [] doors = GameObject.FindGameObjectsWithTag("Door");
		foreach(GameObject o in doors)
		{
			Door door = o.GetComponent<Door>();
			foreach(DoorSaveData doorData in currentLevel.Doors)
			{
				if(door.ID == doorData.ID)
				{
					door.IsLocked = doorData.IsLocked;
					door.IsOpen = doorData.IsOpen;
				}
			}
		}

		//load factions
		GameManager.Inst.NPCManager.AllFactions.Clear();
		foreach(KeyValuePair<Faction, FactionData> factionData in CurrentSave.Factions)
		{
			GameManager.Inst.NPCManager.AllFactions.Add(factionData.Key, factionData.Value);
			factionData.Value.PostLoad();
		}
			

		//load household 
		GameManager.Inst.NPCManager.AllSquads.Clear();
		Household [] households = (Household[])GameObject.FindObjectsOfType<Household>();
		foreach(Household household in households)
		{
			foreach(HouseholdSaveData saveData in currentLevel.Households)
			{
				if(household.name == saveData.HouseholdName)
				{
					if(saveData.CurrentSquadID != "")
					{
						//create a new squad
						AISquad squad = new AISquad();
						squad.ID = saveData.CurrentSquadID;
						squad.Tier = saveData.CurrentSquadTier;
						squad.Faction = saveData.OwningFaction;
						squad.Household = household;
						squad.Household.CurrentSquad = squad;
						if(!GameManager.Inst.NPCManager.AllSquads.ContainsKey(squad.ID))
						{
							GameManager.Inst.NPCManager.AllSquads.Add(squad.ID, squad);
						}

					}
					household.IsMemberAlreadyAdded = true;
					household.SetScheduleData(saveData.IsRefilledToday, saveData.Expedition1SentToday, saveData.Expedition2SentToday,
						saveData.ExpeditionTime1, saveData.ExpeditionTime2);
					
				}
			}
		}





		//load characters
		//first remove all existing characters
		GameObject [] npcs = GameObject.FindGameObjectsWithTag("NPC");
		foreach(GameObject npc in npcs)
		{
			npc.tag = "Untagged";
			GameObject.Destroy(npc.gameObject);
		}
		//add new characters from save
		//then reinitialize NPCManager

		npcs = GameObject.FindGameObjectsWithTag("NPC");
		Debug.Log("Number of loading NPC before loading " + npcs.Length);

		foreach(CharacterSaveData characterData in currentLevel.Characters)
		{
			if(characterData.CharacterType == CharacterType.Human)
			{
				HumanCharacter character = GameObject.Instantiate(Resources.Load("HumanCharacter") as GameObject).GetComponent<HumanCharacter>();

				character.CharacterID = characterData.CharacterID;
				character.GoapID = characterData.GoapID;

				character.Initialize();
				character.MyNavAgent.enabled = false;
				character.CharacterType = characterData.CharacterType;
				character.SquadID = characterData.SquadID;
				character.Faction = characterData.Faction;
				character.IsCommander = characterData.IsCommander;
				character.IsEssential = characterData.IsEssential;


				character.MyStatus.Data = characterData.StatusData;
				character.transform.position = characterData.Pos.ConvertToVector3();

				character.MyNavAgent.enabled = true;

				character.Name = characterData.Name;
				character.Title = characterData.Title;
				character.gameObject.name = characterData.GOName;

				character.Inventory = new CharacterInventory();
				character.Inventory.ArmorSlot = characterData.Inventory.ArmorSlot;
				character.Inventory.HeadSlot = characterData.Inventory.HeadSlot;
				character.Inventory.RifleSlot = characterData.Inventory.RifleSlot;
				character.Inventory.SideArmSlot = characterData.Inventory.SideArmSlot;
				character.Inventory.ThrowSlot = characterData.Inventory.ThrowSlot;
				character.Inventory.ToolSlot = characterData.Inventory.ToolSlot;
				character.Inventory.BackpackCols = characterData.Inventory.BackpackCols;
				character.Inventory.BackpackRows = characterData.Inventory.BackpackRows;
				character.Inventory.Backpack = new List<GridItemData>(characterData.Inventory.Backpack);
				character.Inventory.PostLoad();


			}
			else if(characterData.CharacterType == CharacterType.Mutant)
			{
				MutantCharacter character = GameObject.Instantiate(Resources.Load("MutantCharacter") as GameObject).GetComponent<MutantCharacter>();
				character.CharacterID = characterData.CharacterID;
				character.GoapID = characterData.GoapID;

				character.Initialize();
				character.MyNavAgent.enabled = false;
				character.CharacterType = characterData.CharacterType;
				character.SquadID = characterData.SquadID;
				character.Faction = characterData.Faction;
				GameManager.Inst.ItemManager.LoadNPCInventory(character.Inventory, character.Faction, 0);
				character.Inventory.PostLoad();

				character.MyStatus.Data = characterData.StatusData;
				character.transform.position = characterData.Pos.ConvertToVector3();

				character.MyNavAgent.enabled = true;

				character.Name = characterData.Name;
				character.Title = characterData.Title;
				character.gameObject.name = characterData.GOName;


			}
			else if(characterData.CharacterType == CharacterType.Animal)
			{
				MutantCharacter character = GameObject.Instantiate(Resources.Load("MutantAnimal") as GameObject).GetComponent<MutantCharacter>();
				character.CharacterID = characterData.CharacterID;
				character.GoapID = characterData.GoapID;

				character.Initialize();
				character.MyNavAgent.enabled = false;
				character.CharacterType = characterData.CharacterType;
				character.SquadID = characterData.SquadID;
				character.Faction = characterData.Faction;
				GameManager.Inst.ItemManager.LoadNPCInventory(character.Inventory, character.Faction, 0);
				character.Inventory.PostLoad();

				character.MyStatus.Data = characterData.StatusData;
				character.transform.position = characterData.Pos.ConvertToVector3();

				character.MyNavAgent.enabled = true;

				character.Name = characterData.Name;
				character.Title = characterData.Title;
				character.gameObject.name = characterData.GOName;
			}

		}


		GameManager.Inst.NPCManager.Initialize();

		//add player to NPC manager
		GameManager.Inst.NPCManager.AddHumanCharacter(GameManager.Inst.PlayerControl.SelectedPC);

		//load traders
		foreach(HumanCharacter character in GameManager.Inst.NPCManager.HumansInScene)
		{
			Trader trader = character.GetComponent<Trader>();
			if(trader != null)
			{
				foreach(TraderData traderData in currentLevel.Traders)
				{
					if(traderData.CharacterID == character.CharacterID)
					{
						trader.Cash = traderData.Cash;
						trader.Tier = traderData.Tier;
						trader.TraderInventory = traderData.TraderInventory;
						trader.SupplyRenewTimer = traderData.SupplyRenewTimer;
						trader.PostLoad();
					}
				}

			}
		}






		return true;
	}

}
