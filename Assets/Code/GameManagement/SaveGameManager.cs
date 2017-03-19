using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveGameManager
{
	public SaveGame CurrentSave;
	public string SaveGameName;

	public void Save(bool isLoadingLevel)
	{
		CurrentSave = new SaveGame();

		//save player status
		CurrentSave.PlayerStatus = GameManager.Inst.PlayerControl.SelectedPC.MyStatus;
		//save player inventory
		CurrentSave.PlayerInventory = GameManager.Inst.PlayerControl.SelectedPC.Inventory;
		//save player boosts
		CurrentSave.PlayerBoosts = GameManager.Inst.PlayerControl.Survival.GetStatBoosts();

		//save pickup items in the scene
		//first remove all pickup items from list from current level
		List<PickupItemData> pickupDataList = GameManager.Inst.ItemManager.PickupItemDatas;
		List<PickupItemData> pickupDataListCopy = new List<PickupItemData>(pickupDataList);
		foreach(PickupItemData data in pickupDataListCopy)
		{
			if(data.LevelName == GameManager.Inst.WorldManager.CurrentLevelName)
			{
				pickupDataList.Remove(data);
			}
		}

		//now go through all pickup items in the scene and create pickup data list
		GameObject [] objects = GameObject.FindGameObjectsWithTag("PickupItem");
		foreach(GameObject o in objects)
		{
			PickupItem pickup = o.GetComponent<PickupItem>();
			PickupItemData data = new PickupItemData();
			data.ItemID = pickup.Item.ID;
			data.Quantity = data.Quantity;
			data.LevelName = GameManager.Inst.WorldManager.CurrentLevelName;
			data.Pos = pickup.transform.position;
			data.EulerAngles = pickup.transform.localEulerAngles;

			pickupDataList.Add(data);
		}

		//save traders
		CurrentSave.Traders = new List<KeyValuePair<string, Trader>>();
		foreach(HumanCharacter character in GameManager.Inst.NPCManager.HumansInScene)
		{
			Trader trader = character.GetComponent<Trader>();
			if(trader != null)
			{
				KeyValuePair<string, Trader> traderKV = new KeyValuePair<string, Trader>(character.CharacterID, trader);
				CurrentSave.Traders.Add(traderKV);
			}
		}

		//save chests
		//first remove all chests from list from current level
		List<ChestData> chestDataList = GameManager.Inst.ItemManager.ChestDatas;
		List<ChestData> chestDataListCopy = new List<ChestData>(chestDataList);
		foreach(ChestData data in chestDataListCopy)
		{
			if(data.LevelName == GameManager.Inst.WorldManager.CurrentLevelName)
			{
				chestDataList.Remove(data);
			}
		}

		//now go through all chests in the scene and create pickup data list
		GameObject [] chests = GameObject.FindGameObjectsWithTag("Chest");
		foreach(GameObject o in chests)
		{
			Chest chest = o.GetComponent<Chest>();
			ChestData data = new ChestData();
			data.LevelName = GameManager.Inst.WorldManager.CurrentLevelName;
			data.ChestID = chest.ChestID;
			data.ColSize = chest.ColSize;
			data.RowSize = chest.RowSize;
			data.Items = chest.Items;

			chestDataList.Add(data);
		}


		//save factions
		CurrentSave.Factions = new List<KeyValuePair<Faction, FactionData>>();
		foreach(KeyValuePair<Faction, FactionData> factionData in GameManager.Inst.NPCManager.AllFactions)
		{
			CurrentSave.Factions.Add(factionData);
		}


		//build the save file
		if(isLoadingLevel)
		{
			SaveGameName = "Autosave - " + GameManager.Inst.WorldManager.CurrentLevelName;
		}

		string fullPath = Application.persistentDataPath + "/" + SaveGameName + ".dat";

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
	}

	public void Load()
	{

	}

}
