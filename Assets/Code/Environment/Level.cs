using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
	public string Name;
	public List<PickupItemData> PickupItemDatas;
	public List<ChestData> ChestDatas;
	public List<TraderData> Traders;//string is Character.CharacterID

	public List<CharacterSaveData> Characters;
	public List<HouseholdSaveData> Households;
	public List<DoorSaveData> Doors;

}
