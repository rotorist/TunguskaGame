using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Household : MonoBehaviour
{
	public Vector3 PatrolRange;
	public Vector3 CombatRange;
	public List<Transform> GuardLocs;
	public List<Transform> PatrolNodes;
	public List<IdleDest> IdleDests;
	public IdleDest CommanderIdleDest;

	public int MaxOccupants;

	public AISquad CurrentSquad;

	public void UpdateHouseHold()
	{
		
	}

	public void Initialize()
	{
		RefillSquadMembers();

	}

	private void RefillSquadMembers()
	{
		if(CurrentSquad == null || MaxOccupants <= 0)
		{
			return;
		}

		if(CurrentSquad.Members.Count < MaxOccupants)
		{
			//first create a commander if there isn't one
			if(!CurrentSquad.IsThereCommander())
			{
				string modelID = GetRandomCharacterModelID(CurrentSquad.Faction);
				HumanCharacter commander = GameManager.Inst.NPCManager.SpawnRandomHumanCharacter(modelID, CurrentSquad, CommanderIdleDest.transform.position);
				commander.IsCommander = true;
				CurrentSquad.AddMember(commander);
			}
			int numberToSpawn = MaxOccupants - CurrentSquad.Members.Count;
			for(int i = 0; i < numberToSpawn; i++)
			{
				Vector3 spawnLoc = FindSpawnLocation();
				string modelID = GetRandomCharacterModelID(CurrentSquad.Faction);
				HumanCharacter character = GameManager.Inst.NPCManager.SpawnRandomHumanCharacter(modelID, CurrentSquad, spawnLoc);
				CurrentSquad.AddMember(character);
			}

		}
	}

	private void AssignSquadJobs()
	{

	}

	private string GetRandomCharacterModelID(Faction faction)
	{
		FactionData factionData = GameManager.Inst.NPCManager.GetFactionData(faction);
		return factionData.MemberModelIDs[UnityEngine.Random.Range(1, factionData.MemberModelIDs.Length)];
	}

	private Vector3 FindSpawnLocation()
	{
		Vector3 result;
		if(AI.RandomPoint(transform.position, PatrolRange, out result))
		{
			return result;
		}
		else
		{
			return transform.position;
		}
	}

}
