using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HouseholdSaveData 
{
	public string HouseholdName;
	public string CurrentSquadID;
	public int CurrentSquadTier;
	public Faction OwningFaction;

	public bool IsRefilledToday;
	public bool Expedition1SentToday;
	public bool Expedition2SentToday;
	public int ExpeditionTime1;
	public int ExpeditionTime2;

}
