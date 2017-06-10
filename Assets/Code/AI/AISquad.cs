using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AISquad
{
	public string ID;
	public List<Character> Members;
	public Household Household;
	public Faction Faction;

	public AISquad()
	{
		Members = new List<Character>();

		Household = GameObject.Find("Household1").GetComponent<Household>();
	}

	public void AddMember(Character newMember)
	{
		if(!Members.Contains(newMember))
		{
			Members.Add(newMember);
			newMember.MyAI.Squad = this;
		}
	}

	public void RemoveMember(Character member)
	{
		if(Members.Contains(member))
		{
			Members.Remove(member);
			member.MyAI.Squad = null;
		}
	}

	public void IssueSquadCommand()
	{
		
		foreach(Character member in Members)
		{
			member.MyAI.BlackBoard.PatrolLoc = Household.transform.position;
			member.MyAI.BlackBoard.PatrolRange = Household.PatrolRange;
			member.MyAI.BlackBoard.CombatRange = Household.CombatRange;
			member.MyAI.BlackBoard.HasPatrolInfo = true;
			member.MyAI.BlackBoard.PatrolNodeIndex = 0;
			member.MyAI.SetDynamicyGoal(GameManager.Inst.NPCManager.DynamicGoalPatrol, 5);
		}


		/*
		Members[1].MyAI.BlackBoard.PatrolLoc = Household.GuardLocs[0];
		Members[1].MyAI.BlackBoard.GuardDirection = Household.GuardDirs[0];
		Members[1].MyAI.BlackBoard.CombatRange = new Vector3(10, 10, 10);
		Members[1].MyAI.BlackBoard.HasPatrolInfo = true;
		Members[1].MyAI.BlackBoard.PatrolNodeIndex = -1;
		Members[1].MyAI.SetDynamicyGoal(GameManager.Inst.NPCManager.DynamicGoalGuard, 5);

		Members[2].MyAI.BlackBoard.PatrolLoc = Household.GuardLocs[1];
		Members[2].MyAI.BlackBoard.GuardDirection = Household.GuardDirs[1];
		Members[2].MyAI.BlackBoard.CombatRange = new Vector3(10, 10, 10);
		Members[2].MyAI.BlackBoard.HasPatrolInfo = true;
		Members[2].MyAI.BlackBoard.PatrolNodeIndex = -1;
		Members[2].MyAI.SetDynamicyGoal(GameManager.Inst.NPCManager.DynamicGoalGuard, 5);


		Members[3].MyAI.BlackBoard.PatrolLoc = Household.transform.position;
		Members[3].MyAI.BlackBoard.PatrolRange = Household.PatrolRange;
		Members[3].MyAI.BlackBoard.CombatRange = Household.CombatRange;
		Members[3].MyAI.BlackBoard.HasPatrolInfo = true;
		Members[3].MyAI.BlackBoard.PatrolNodeIndex = -1;
		Members[3].MyAI.SetDynamicyGoal(GameManager.Inst.NPCManager.DynamicGoalPatrol, 5);
		*/
		/*
		Members[0].MyAI.BlackBoard.PatrolLoc = Household.GuardLocs[0];
		Members[0].MyAI.BlackBoard.GuardDirection = Household.GuardDirs[0];
		Members[0].MyAI.BlackBoard.CombatRange = new Vector3(10, 10, 10);
		Members[0].MyAI.BlackBoard.HasPatrolInfo = true;
		Members[0].MyAI.BlackBoard.PatrolNodeIndex = -1;
		Members[0].MyAI.SetDynamicyGoal(GameManager.Inst.NPCManager.DynamicGoalGuard, 5);
		*/
		/*
		Members[0].MyAI.BlackBoard.PatrolLoc = Household.GuardLocs[0];
		Members[0].MyAI.BlackBoard.GuardDirection = Household.GuardDirs[0];
		Members[0].MyAI.BlackBoard.CombatRange = new Vector3(10, 10, 10);
		Members[0].MyAI.BlackBoard.HasPatrolInfo = true;
		Members[0].MyAI.BlackBoard.PatrolNodeIndex = -1;
		Members[0].MyAI.SetDynamicyGoal(GameManager.Inst.NPCManager.DynamicGoalGuard, 5);

		Members[1].MyAI.BlackBoard.PatrolLoc = Household.GuardLocs[1];
		Members[1].MyAI.BlackBoard.GuardDirection = Household.GuardDirs[1];
		Members[1].MyAI.BlackBoard.CombatRange = new Vector3(10, 10, 10);
		Members[1].MyAI.BlackBoard.HasPatrolInfo = true;
		Members[1].MyAI.BlackBoard.PatrolNodeIndex = -1;
		Members[1].MyAI.SetDynamicyGoal(GameManager.Inst.NPCManager.DynamicGoalGuard, 5);
		*/
	}

	public bool IsAnyOneInvestigating(Vector3 location)
	{
		foreach(Character member in Members)
		{
			GoapAction currentAction = member.MyAI.GetCurrentAction();
			if(currentAction != null && currentAction.Name == "ActionInvestigate")
			{
				Debug.Log("member  " + member + " is doing investigation");
				if(Vector3.Distance(member.MyAI.BlackBoard.HighestDisturbanceLoc, location) < 5)
				{
					Debug.Log("member " + member.name + " is investigating loc " + location);
					return true;
				}
			}
			else if(currentAction != null && currentAction.Name == "ActionCheckCorpse")
			{
				if(Vector3.Distance(member.MyAI.BlackBoard.TargetCorpse.LastKnownPos, location) < 5)
				{
					Debug.Log("member " + member.name + " is checking corpse at loc " + location);
					return true;
				}
			}
		}
		return false;
	}

	public bool IsAnyOneIntimidating(Character target)
	{

		return false;
	}

	public bool IsPatrolNodeTaken(int node)
	{
		foreach(Character member in Members)
		{
			if(member.MyAI.BlackBoard.PatrolNodeIndex == node)
			{
				return true;
			}
		}

		return false;
	}

	public int GetNumberOfTalkers()
	{
		int talkers = 0;
		foreach(Character member in Members)
		{
			if(member.CharacterAudio.isPlaying)
			{
				talkers ++;
			}
		}

		return talkers;
	}

	public bool ShouldIBeQuiet()
	{
		if(Members.Count <= 1)
		{
			return true;
		}

		int talkers = GetNumberOfTalkers();
		float rand = UnityEngine.Random.value;
		//if 0 talkers, 0% chance to be quiet
		//if 1 talkers, 40% chance to be quiet
		//   2          60%
		//   3          100%
		switch(talkers)
		{
		case 0: 
			return false;
			break;
		case 1:
			return rand > 0.4f;
			break;
		case 2:
			return rand > 0.6f;
			break;
		default:
			return true;
			break;
		}
	}

	public void SetSquadAlertLevel(object level)
	{
		foreach(Character member in Members)
		{
			member.MyAI.BlackBoard.GuardLevel = (int)level;
		}
	}


	public void BroadcastMemoryFact(WorkingMemoryFact fact)
	{
		foreach(Character member in Members)
		{
			member.MyAI.WorkingMemory.AddFact(fact);
		}
	}
}
