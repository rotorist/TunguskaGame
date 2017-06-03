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

	public string LevelToLoad;
	public string CurrentEnvironmentName;
	public List<Level> Levels;//from world manager

	public List<KeyValuePair<Faction, FactionData>> Factions;//to be injected to npc manager when loading

}
