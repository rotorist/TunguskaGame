using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveGame
{
	public float [] PlayerLocation;
	public CharacterStatusData PlayerStatus;
	public CharacterInventory PlayerInventory;
	public List<PlayerStatBoost> PlayerBoosts;//to be inserted into player survival
	//below is for playerprogress
	public string PlayerFirstName;
	public string PlayerLastName;
	public List<string> DiscoveredTopics;
	public List<List<string>> JournalEntries;



	public string LevelToLoad;
	public string CurrentEnvironmentName;
	public int CurrentDay;
	public int CurrentTime;
	public List<Level> Levels;//from world manager

	public List<StoryConditionItem> ItemConditions;
	public List<StoryConditionTrigger> TriggerConditions;

	public List<KeyValuePair<Faction, FactionData>> Factions;//to be injected to npc manager when loading

}
