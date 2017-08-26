using UnityEngine;
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

	public bool IsRefilledToday { get { return _isRefilledToday; } }
	public bool Expedition1SentToday { get { return _expedition1SentToday; } }
	public bool Expedition2SentToday { get { return _expedition2SentToday; } }
	public int ExpeditionTime1 { get { return _expeditionTime1; } }
	public int ExpeditionTime2 { get { return _expeditionTime2; } }

	public bool IsMemberAlreadyAdded;

	private List<AISquad> _explorerSquads;
	private bool _isRefilledToday;
	private bool _expedition1SentToday;
	private bool _expedition2SentToday;
	private int _expeditionTime1;
	private int _expeditionTime2;


	public void UpdateHouseHoldPerSecond()
	{
		if(CurrentSquad == null || GameManager.Inst.NPCManager.GetFactionData(CurrentSquad.Faction).CharacterType != CharacterType.Human)
		{
			//don't update household for mutants
			return;
		}

		//check if any explorer squad is dead then delete it and send another one
		//if destination is base and reached dest and there is no more members in the bases's current squad, take the base over

		List<AISquad> explorerSquadCopy = new List<AISquad>(_explorerSquads);
		foreach(AISquad squad in explorerSquadCopy)
		{
			if(squad.Members.Count <= 0)
			{
				_explorerSquads.Remove(squad);
				GameManager.Inst.NPCManager.DeleteSquad(squad.ID);
				continue;
			}


		}

		//refill
		int totalMembers = GetAllMembersCount();
		if(!_isRefilledToday && totalMembers < MaxOccupants && Vector3.Distance(GameManager.Inst.PlayerControl.SelectedPC.transform.position, transform.position) > 70)
		{
			RefillHouseHoldSquadMembers();
			_isRefilledToday = true;
		}

		//send expedition
		//only send expedition of max member >= 10
		if(MaxOccupants >= 10)
		{
			float memberRatio = totalMembers * 1.0f / (MaxOccupants * 1f);
			if(GameManager.Inst.WorldManager.CurrentTime >= _expeditionTime1 && _explorerSquads.Count < 2 && !_expedition1SentToday)
			{
				if(memberRatio > 0.5f)
				{
					
					//got enough people, will send
					int maxParticipants = Mathf.FloorToInt(totalMembers * 0.25f);
					if(maxParticipants < 2)
					{
						maxParticipants = 2;
					}
					int participants = UnityEngine.Random.Range(2, maxParticipants);
					SendHumanExplorer(GameManager.Inst.NPCManager.GetRandomHuntNavNode(), participants);
					_expedition1SentToday = true;
				}
			}

			if(GameManager.Inst.WorldManager.CurrentTime >= _expeditionTime2 && _explorerSquads.Count < 2 && !_expedition2SentToday)
			{
				if(memberRatio > 0.5f)
				{
					//got enough people, will send
					int maxParticipants = Mathf.FloorToInt(totalMembers * 0.3f);
					if(maxParticipants < 2)
					{
						maxParticipants = 2;
					}
					int participants = UnityEngine.Random.Range(2, maxParticipants);
					SendHumanExplorer(GameManager.Inst.NPCManager.GetRandomBaseNavNode(CurrentSquad.Faction), participants);
					_expedition2SentToday = true;
				}
			}
		}

	}



	public void Initialize()
	{
		_explorerSquads = new List<AISquad>();

		if(!IsMemberAlreadyAdded)
		{
			RefillHouseHoldSquadMembers();
		}

		//clear guard locs
		foreach(GuardLoc g in GuardLocs)
		{
			g.Guard = null;

		}

		AssignSquadJobs();
		TimerEventHandler.OnOneDayTimer -= OnOneDayTimer;
		TimerEventHandler.OnOneDayTimer += OnOneDayTimer;
	}

	public void SetScheduleData(bool isRefilled, bool exp1Sent, bool exp2Sent, int exp1Time, int exp2Time)
	{
		_isRefilledToday = isRefilled;
		_expedition1SentToday = exp1Sent;
		_expedition2SentToday = exp2Sent;
		_expeditionTime1 = exp1Time;
		_expeditionTime2 = exp2Time;
	}

	public int GetAllMembersCount()
	{
		int count = 0;
		if(CurrentSquad != null)
		{
			count += CurrentSquad.Members.Count;
		}
		foreach(AISquad squad in _explorerSquads)
		{
			count += squad.Members.Count;
		}

		return count;
	}

	public List<AISquad> GetExplorerSquads()
	{
		return _explorerSquads;
	}

	public void OnOneDayTimer()
	{
		if(CurrentSquad == null || CurrentSquad.Members.Count <= 0)
		{
			return;
		}
		//schedule the expeditions for the day
		_expeditionTime1 = 100;//UnityEngine.Random.Range(1, 1440);
		_expeditionTime2 = 200;//UnityEngine.Random.Range(1, 1440);
		_isRefilledToday = false;
		_expedition1SentToday = false;
		_expedition2SentToday = false;
	}

	public void RemoveExplorerSquad(AISquad squad)
	{
		if(_explorerSquads.Contains(squad))
		{
			_explorerSquads.Remove(squad);
		}
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
			int currentMembersCount = GetAllMembersCount();
			if(currentMembersCount < MaxOccupants)
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
				int numberToSpawn = MaxOccupants - currentMembersCount;
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



	public void AssignSquadJobs()
	{
		Debug.Log("is squad null? " + (CurrentSquad == null) + " members " + CurrentSquad.Members.Count + " squad is " + CurrentSquad.ID);
		if(CurrentSquad == null || MaxOccupants <= 0 || CurrentSquad.Members.Count <= 0)
		{
			return;
		}
		Debug.Log("Starting Assign Squad Job to " + CurrentSquad.Faction + " character type " + GameManager.Inst.NPCManager.GetFactionData(CurrentSquad.Faction).CharacterType);
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
			Debug.Log("Starting Assign Squad Job for humans. guard locs count " + GuardLocs.Count);
			//first set patrol range for everyone 
			foreach(Character member in CurrentSquad.Members)
			{
				member.MyAI.BlackBoard.PatrolLoc = transform.position;
				member.MyAI.BlackBoard.PatrolRange = PatrolRange;
				member.MyAI.BlackBoard.CombatRange = CombatRange;
				member.MyAI.BlackBoard.HasPatrolInfo = true;
				member.MyJobs.Clear();
				member.MyJobs.Add(NPCJobs.None);
				member.MyAI.SetDynamicyGoal(GameManager.Inst.NPCManager.DynamicGoalChill, 5);
			}

			if(GuardLocs.Count > 0)
			{
				int guardsCount = CurrentSquad.GetNumberOfGuards();
				Debug.LogError("Current guard count " + guardsCount);
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
				Debug.LogError("Current patrols count " + CurrentSquad.GetNumberOfPatrols());
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



	private void SendHumanExplorer(NavNode destNavNode, int squadSize)
	{
		
		NavNode currentNode = GameManager.Inst.NPCManager.GetNavNodeByHousehold(this);
		if(currentNode == null || destNavNode == null)
		{
			Debug.LogError("No current node or no destNavNode");
			return;
		}

		NavNode nextNavNode = AI.FindNextNavNode(currentNode, destNavNode);

		if(nextNavNode == null)
		{
			Debug.LogError("can't find next nav node");
			return;
		}

		Debug.Log("GOTO setting new destinatoin " + nextNavNode.name + " currentNode " + currentNode.name);

		//create a squad
		AISquad squad = GameManager.Inst.NPCManager.SpawnHumanExplorerSquad(CurrentSquad.Faction, this);
		squad.NextNavNode = nextNavNode;
		squad.DestNavNode = destNavNode;

		_explorerSquads.Add(squad);

		//decide a max participant number
		int participants = squadSize;

		//loop through all members and pick ones who don't have job
		foreach(Character c in CurrentSquad.Members)
		{
			if(participants <= 0)
			{
				break;
			}

			if(c.MyJobs.Contains(NPCJobs.None) && !c.IsEssential)
			{
				squad.Members.Add(c);
				participants --;
				c.MyJobs.Clear();
				c.MyJobs.Add(NPCJobs.Explore);
				c.MyAI.Squad = squad;
				if(squad.Commander == null)
				{
					Debug.Log("Assigned commander to " + c.name);
					//assign commander job
					squad.AssignExpCommanderRole(c, squad);
				}
				else
				{
					Debug.Log("Assigned follower to " + c.name);
					squad.AssignExpFollowerRole(c, squad);
				}
			}
		}

		Debug.LogError("Sending expedition - squad " + squad.ID + " to " + destNavNode.name + " commander " + squad.Commander.name);

		//remove squad member from current squad
		foreach(Character c in squad.Members)
		{
			if(CurrentSquad.Members.Contains(c))
			{
				CurrentSquad.RemoveMember(c);
			}

			c.MyAI.Squad = squad;
			c.SquadID = squad.ID;
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
