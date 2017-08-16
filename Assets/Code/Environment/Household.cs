﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Household : MonoBehaviour
{
	public Transform DefensePoint;
	public float DefenseRadius;
	public Vector3 PatrolRange;
	public Vector3 CombatRange;
	public List<GuardLoc> GuardLocs;
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
		
		RefillHouseHoldSquadMembers();
		AssignSquadJobs();
	}

	private void RefillHouseHoldSquadMembers()
	{
		if(CurrentSquad == null || MaxOccupants <= 0)
		{
			return;
		}

		if(GameManager.Inst.NPCManager.GetFactionData(CurrentSquad.Faction).CharacterType == CharacterType.Mutant)
		{
			int numberToSpawn = MaxOccupants - CurrentSquad.Members.Count;
			for(int i = 0; i < numberToSpawn; i++)
			{
				Vector3 spawnLoc = FindSpawnLocation();
				string modelID = GetRandomCharacterModelID(CurrentSquad.Faction);
				MutantCharacter character = GameManager.Inst.NPCManager.SpawnRandomMutantCharacter(modelID, CurrentSquad, spawnLoc);
				CurrentSquad.AddMember(character);
			}
		}
		else if(GameManager.Inst.NPCManager.GetFactionData(CurrentSquad.Faction).CharacterType == CharacterType.Animal)
		{
			int numberToSpawn = MaxOccupants - CurrentSquad.Members.Count;
			for(int i = 0; i < numberToSpawn; i++)
			{
				Vector3 spawnLoc = FindSpawnLocation();
				string modelID = GetRandomCharacterModelID(CurrentSquad.Faction);
				MutantCharacter character = GameManager.Inst.NPCManager.SpawnRandomAnimalCharacter(modelID, CurrentSquad, spawnLoc);
				CurrentSquad.AddMember(character);
			}

		}
		else
		{
			if(CurrentSquad.Members.Count < MaxOccupants)
			{
				//first create a commander if there isn't one
				if(!CurrentSquad.IsThereCommander() && CommanderIdleDest != null)
				{
					string modelID = GetRandomCharacterModelID(CurrentSquad.Faction);
					HumanCharacter commander = GameManager.Inst.NPCManager.SpawnRandomHumanCharacter(modelID, CurrentSquad, CommanderIdleDest.transform.position);
					commander.IsCommander = true;
					CurrentSquad.AddMember(commander);
					CurrentSquad.Commander = commander;
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

	}



	private void AssignSquadJobs()
	{
		if(CurrentSquad == null || MaxOccupants <= 0)
		{
			return;
		}

		if(GameManager.Inst.NPCManager.GetFactionData(CurrentSquad.Faction).CharacterType != CharacterType.Human)
		{
			foreach(Character mutant in CurrentSquad.Members)
			{
				mutant.MyAI.BlackBoard.PatrolLoc = transform.position;
				mutant.MyAI.BlackBoard.PatrolRange = PatrolRange;
				mutant.MyAI.BlackBoard.CombatRange = CombatRange;
				mutant.MyAI.BlackBoard.HasPatrolInfo = true;
			}
			return;
		}
		else
		{
			//first set patrol range for everyone 
			foreach(Character member in CurrentSquad.Members)
			{
				member.MyAI.BlackBoard.PatrolLoc = transform.position;
				member.MyAI.BlackBoard.PatrolRange = PatrolRange;
				member.MyAI.BlackBoard.CombatRange = CombatRange;
				member.MyAI.BlackBoard.HasPatrolInfo = true;
			}

			if(GuardLocs.Count > 0)
			{
				int guardsCount = CurrentSquad.GetNumberOfGuards();
				int i = 0;
				while(GuardLocs.Count > guardsCount && i < CurrentSquad.Members.Count)
				{
					if(CurrentSquad.Members[i].MyJobs.Contains(NPCJobs.None) && !CurrentSquad.Members[i].IsCommander)
					{
						int guardLocIndex = GetVacantGuardLocID();
						if(guardLocIndex < 0)
						{
							//can't find vacant guard loc
							break;
						}
						else
						{
							//assign a new guard
							CurrentSquad.Members[i].MyJobs.Clear();
							CurrentSquad.Members[i].MyJobs.Add(NPCJobs.Guard);
							CurrentSquad.Members[i].MyAI.ClearDynamicGoal(5);
							CurrentSquad.Members[i].MyAI.BlackBoard.PatrolLoc = GuardLocs[guardLocIndex].transform.position;
							CurrentSquad.Members[i].MyAI.BlackBoard.GuardDirection = GuardLocs[guardLocIndex].transform.forward;
							CurrentSquad.Members[i].MyAI.BlackBoard.CombatRange = new Vector3(10, 10, 10);
							CurrentSquad.Members[i].MyAI.BlackBoard.HasPatrolInfo = true;
							CurrentSquad.Members[i].MyAI.BlackBoard.PatrolNodeIndex = -1;
							CurrentSquad.Members[i].MyAI.SetDynamicyGoal(GameManager.Inst.NPCManager.DynamicGoalGuard, 5);
							GuardLocs[guardLocIndex].Guard = CurrentSquad.Members[i];
							guardsCount++;
						}
					}

					i++;

				}
			}

			if(PatrolNodes.Count > 0)
			{
				int patrolsCount = CurrentSquad.GetNumberOfPatrols();
				int i = 0;
				while(GuardLocs.Count > patrolsCount && i < CurrentSquad.Members.Count)
				{
					if(CurrentSquad.Members[i].MyJobs.Contains(NPCJobs.None) && !CurrentSquad.Members[i].IsCommander)
					{

						//assign a new guard
						CurrentSquad.Members[i].MyJobs.Clear();
						CurrentSquad.Members[i].MyJobs.Add(NPCJobs.Patrol);
						CurrentSquad.Members[i].MyAI.ClearDynamicGoal(5);
						CurrentSquad.Members[i].MyAI.BlackBoard.PatrolLoc = transform.position;
						CurrentSquad.Members[i].MyAI.BlackBoard.PatrolRange = PatrolRange;
						CurrentSquad.Members[i].MyAI.BlackBoard.CombatRange = CombatRange;
						CurrentSquad.Members[i].MyAI.BlackBoard.HasPatrolInfo = true;
						CurrentSquad.Members[i].MyAI.BlackBoard.PatrolNodeIndex = UnityEngine.Random.Range(0, PatrolNodes.Count);
						CurrentSquad.Members[i].MyAI.SetDynamicyGoal(GameManager.Inst.NPCManager.DynamicGoalPatrol, 5);
						patrolsCount++;

					}

					i++;

				}
			}
		}
	}

	private void SendHumanExplorer()
	{
		//create a squad
		AISquad squad = GameManager.Inst.NPCManager.SpawnHumanExplorerSquad(CurrentSquad.Faction);

		//decide a max participant number
		int participants = UnityEngine.Random.Range(2, 5);

		//loop through all members and pick ones who don't have job
		foreach(Character c in CurrentSquad.Members)
		{
			if(c.MyJobs.Contains(NPCJobs.None))
			{
				squad.Members.Add(c);
				c.MyJobs.Clear();
				c.MyJobs.Add(NPCJobs.Explore);
				if(squad.Commander == null)
				{
					c.IsCommander = true;
					squad.Commander = c;
				}
			}
		}
	}

	private string GetRandomCharacterModelID(Faction faction)
	{
		FactionData factionData = GameManager.Inst.NPCManager.GetFactionData(faction);
		return factionData.MemberModelIDs[UnityEngine.Random.Range(0, factionData.MemberModelIDs.Length)];
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

	private int GetVacantGuardLocID()
	{
		for(int i=0; i<GuardLocs.Count; i++)
		{
			if(GuardLocs[i].Guard == null || GuardLocs[i].Guard.MyStatus.Health <= 0)
			{
				return i;
			}
		}
		return -1;
	}



}
