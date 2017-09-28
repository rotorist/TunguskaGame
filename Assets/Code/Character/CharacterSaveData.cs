using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSaveData 
{
	public int GoapID;
	public string CharacterID;
	public string Name;
	public string Title;
	public string GOName;

	public SerVector3 Pos;
	public SerVector3 Angles;

	public CharacterType CharacterType;
	public string SquadID;
	public Faction Faction;
	public bool IsCommander;
	public bool IsEssential;
	public CharacterStatusData StatusData;
	public List<NPCJobs> Jobs;

	public CharacterInventorySaveData Inventory;


}
