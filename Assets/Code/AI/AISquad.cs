using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AISquad
{
	public string ID;
	public List<Character> Members;
	public Household Household;
	public NavNode DestNavNode;
	public NavNode NextNavNode;
	public Faction Faction;
	public Character Commander;

	public AISquad()
	{
		Members = new List<Character>();

		//Household = GameObject.Find("Household1").GetComponent<Household>();
	}

	public void UpdateSquadPerSecond()
	{
		if(Members.Count > 0 && Members[0].MyJobs.Contains(NPCJobs.Explore))
		{
			//elect new commander
			Character commander = null;
			foreach(Character c in Members)
			{
				if(c.IsCommander)
				{
					commander = c;
				}
			}
			if(commander == null)
			{
				Debug.Log("electing new commander");
				AssignExpCommanderRole(Members[0], this);
				commander = Members[0];
			}
			foreach(Character c in Members)
			{
				if(!c.IsCommander)
				{
					AssignExpFollowerRole(c, this);
				}
			}



			//if household no longer belongs to my faction then make everyone go back to the household
			if(Household.CurrentSquad == null || Household.CurrentSquad.Faction != Faction)
			{
				foreach(Character member in Members)
				{
					DestNavNode = GameManager.Inst.NPCManager.GetNavNodeByHousehold(Household);
					NextNavNode = AI.FindNextNavNode(NextNavNode, DestNavNode);
					AssignExpCommanderRole(Commander, this);
				}
			}


			if(AI.IsPositionInArea(Commander.transform.position, DestNavNode.transform.position, Commander.MyAI.BlackBoard.PatrolRange))
			{
				if(DestNavNode.Type == NavNodeType.MutantHunt)
				{
					//done, go back to household if it exists

				}
				else if(DestNavNode.Type == NavNodeType.Base)
				{
					if(DestNavNode.Household == Household && DestNavNode.Household.CurrentSquad != null && DestNavNode.Household.CurrentSquad.Faction == Faction)
					{
						//merge the squad into current squad (gone home)
					}
					else
					{
						Debug.Log("Check if base is empty!");
						if(DestNavNode.Household.CurrentSquad == null || DestNavNode.Household.CurrentSquad.Members.Count <= 0)
						{
							Debug.Log("Taking over base!");
							//we can take this household over
							GameManager.Inst.NPCManager.DeleteSquad(DestNavNode.Household.CurrentSquad.ID);
							List<Character> membersCopy = new List<Character>(Members);
							Members.Clear();
							DestNavNode.Household.CurrentSquad = this;

							//tell household to remove me from explorer squads
							Household.RemoveExplorerSquad(this);

							foreach(Character c in membersCopy)
							{
								c.MyJobs.Clear();
								c.MyJobs.Add(NPCJobs.None);
								AddMember(c);
							}

							Household = DestNavNode.Household;
							Household.AssignSquadJobs();
							DestNavNode = null;
							NextNavNode = null;
						}
					}
				}
			}
		}
	}

	public void AddMember(Character newMember)
	{
		if(!Members.Contains(newMember))
		{
			Members.Add(newMember);
			newMember.MyAI.Squad = this;

			if(Household != null)
			{
				newMember.MyAI.BlackBoard.DefensePoint = Household.DefensePoint.position;
				newMember.MyAI.BlackBoard.DefenseRadius = Household.DefenseRadius;
				newMember.MyAI.BlackBoard.PatrolLoc = Household.transform.position;
				newMember.MyAI.BlackBoard.PatrolRange = Household.PatrolRange;
				newMember.MyAI.BlackBoard.CombatRange = Household.CombatRange;
				newMember.MyAI.BlackBoard.HasPatrolInfo = true;
				newMember.MyAI.BlackBoard.PatrolNodeIndex = -1;
			}
		}
	}

	public void RemoveMember(Character member)
	{

		if(Members.Contains(member))
		{
			Members.Remove(member);
			member.MyAI.Squad = null;
		}
		Debug.Log("Removing member from squad " + ID + " remaining: " + Members.Count);
		if(Members.Count <= 0)
		{
			//raise the squad death event
			StoryEventHandler.Instance.EnqueueStoryEvent(StoryEventType.OnSquadDeath, this, new object[]{ID});
		}


	}

	public void AssignExpCommanderRole(Character c, AISquad squad)
	{
		c.IsCommander = true;
		squad.Commander = c;
		c.MyAI.BlackBoard.PatrolLoc = squad.NextNavNode.transform.position;
		c.MyAI.BlackBoard.PatrolRange = new Vector3(10, 5, 10);
		c.MyAI.BlackBoard.CombatRange = new Vector3(25, 5, 25);
		c.MyAI.BlackBoard.DefensePoint = c.transform.position;
		c.MyAI.BlackBoard.DefenseRadius = 10;

		c.MyAI.SetDynamicyGoal(GameManager.Inst.NPCManager.DynamicGoalExplore, 5);
	}

	public void AssignExpFollowerRole(Character c, AISquad squad)
	{
		c.MyAI.BlackBoard.FollowTarget = squad.Commander;
		c.MyAI.BlackBoard.PatrolLoc = c.transform.position;
		c.MyAI.BlackBoard.PatrolRange = new Vector3(10, 5, 10);
		c.MyAI.BlackBoard.CombatRange = new Vector3(25, 5, 25);
		c.MyAI.BlackBoard.DefensePoint = c.transform.position;
		c.MyAI.BlackBoard.DefenseRadius = 10;

		c.MyAI.SetDynamicyGoal(GameManager.Inst.NPCManager.DynamicGoalFollow, 5);
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

	public bool IsAnyOnePlayingGuitar()
	{
		foreach(Character member in Members)
		{
			GoapAction currentAction = member.MyAI.GetCurrentAction();
			if(currentAction != null && currentAction.Name == "ActionIdleActivity")
			{
				
				ActionIdleActivity action = (ActionIdleActivity)currentAction;
				if(action.SmallAction == SmallActionType.Guitar)
				{
					//Debug.Log("member  " + member + " is playing guitar");
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

	public int GetNumberOfGuards()
	{
		int guards = 0;
		foreach(Character member in Members)
		{
			if(member.MyJobs.Contains(NPCJobs.Guard))
			{
				guards ++;
			}
		}

		return guards;
	}

	public int GetNumberOfPatrols()
	{
		int patrols = 0;
		foreach(Character member in Members)
		{
			if(member.MyJobs.Contains(NPCJobs.Patrol))
			{
				patrols ++;
			}
		}

		return patrols;
	}



	public bool IsThereCommander()
	{
		foreach(Character member in Members)
		{
			if(member.IsCommander)
			{
				return true;
			}
		}

		return false;
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
