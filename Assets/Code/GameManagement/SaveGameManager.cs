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
		CurrentSave.PlayerInventory = GameManager.Inst.PlayerControl.SelectedPC.Inventory;
		//save player boosts
		CurrentSave.PlayerBoosts = GameManager.Inst.PlayerControl.Survival.GetStatBoosts();


		//create new level data after removing existing one
		if(GameManager.Inst.WorldManager.AllLevels.Contains(GameManager.Inst.WorldManager.CurrentLevel))
		{
			GameManager.Inst.WorldManager.AllLevels.Remove(GameManager.Inst.WorldManager.CurrentLevel);
		}

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
			data.Quantity = data.Quantity;
			data.Pos = new SerVector3(pickup.transform.position);
			data.EulerAngles = new SerVector3(pickup.transform.localEulerAngles);

			pickupDataList.Add(data);
		}

		currentLevel.PickupItemDatas = pickupDataList;


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

			chestDataList.Add(data);
		}

		currentLevel.ChestDatas = chestDataList;

		GameManager.Inst.WorldManager.AllLevels.Add(currentLevel);
		CurrentSave.Levels = GameManager.Inst.WorldManager.AllLevels;


		//save factions
		CurrentSave.Factions = new List<KeyValuePair<Faction, FactionData>>();
		foreach(KeyValuePair<Faction, FactionData> factionData in GameManager.Inst.NPCManager.AllFactions)
		{
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

		//load player status
		GameManager.Inst.PlayerControl.SelectedPC.MyStatus.Data = CurrentSave.PlayerStatus;
		//load player boosts
		GameManager.Inst.PlayerControl.Survival.SetStatBoosts(CurrentSave.PlayerBoosts);
		//load player inventory
		GameManager.Inst.PlayerControl.SelectedPC.Inventory = CurrentSave.PlayerInventory;
		GameManager.Inst.PlayerControl.SelectedPC.Inventory.PostLoad();
		//place player in last saved location
		GameManager.Inst.PlayerControl.SelectedPC.transform.position = new Vector3(CurrentSave.PlayerLocation[0], CurrentSave.PlayerLocation[1], CurrentSave.PlayerLocation[2]);


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
					chest.PostLoad();
				}
			}
		}

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
