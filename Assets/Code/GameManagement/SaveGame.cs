using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveGame
{
	public CharacterStatus PlayerStatus;
	public CharacterInventory PlayerInventory;
	public List<PlayerStatBoost> PlayerBoosts;//to be inserted into player survival

	public List<PickupItemData> PickupItemDatas;//to be inserted into item manager
	public List<KeyValuePair<string, Trader>> Traders;//string is Character.CharacterID
	public List<KeyValuePair<string, Chest>> Chests;//string is Chest.ChestID
	public List<KeyValuePair<Faction, FactionData>> Factions;//to be injected to npc manager when loading

}
