using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System;

public class AI : MonoBehaviour 
{
	public GoapGoal CurrentGoal { get { return _currentGoal; } }

	public AIControlType ControlType;
	public AISensor Sensor;
	public GoapPlanner Planner;
	public WorkingMemory WorkingMemory;
	public BlackBoard BlackBoard;
	public AITargeting TargetingSystem;
	public AIWeapon WeaponSystem;
	public AISquad Squad;

	public bool DebugModeOn;

	private Character _parentCharacter;
	private List<GoapGoal> _goals;
	private List<GoapAction> _actions;
	private GoapGoal _currentGoal;
	private GoapAction _currentAction;
	private List<GoapWorldState> _currentWorldStates;
	private Queue<GoapAction> _actionQueue;


	
	// Update is called from AI scheduler
	public void PerFrameUpdate() 
	{
		if(_parentCharacter.MyStatus.Health > 0)
		{
			Sensor.UpdatePerFrame();
			_parentCharacter.Stealth.UpdatePerSchedulerFrame();
		}
	}

	//this update is called directly without going through scheduler
	public void AlwaysPerFrameUpdate()
	{
		if(ControlType != AIControlType.Player)
		{
			TargetingSystem.UpdatePerFrame();
			_parentCharacter.Stealth.UpdatePerFrame();
		}
		WeaponSystem.UpdatePerFrame();
	}

	public void Initialize(Character character)
	{
		_parentCharacter = character;

		WorkingMemory = new WorkingMemory();
		WorkingMemory.Initialize(_parentCharacter);

		BlackBoard = new BlackBoard();
		Sensor = new AISensor();
		Sensor.Initialize(_parentCharacter);
		TargetingSystem = new AITargeting();
		TargetingSystem.Initialize(_parentCharacter);
		WeaponSystem = new AIWeapon();
		WeaponSystem.Initialize(_parentCharacter);
		Planner = new GoapPlanner(this);


		_goals = GameManager.Inst.DBManager.DBHandlerAI.GetCharacterGoalSet(_parentCharacter.GoapID);
		_actions = GameManager.Inst.DBManager.DBHandlerAI.GetCharacterActionSet(_parentCharacter.GoapID);
		_currentWorldStates = new List<GoapWorldState>();

		_parentCharacter.MyEventHandler.OnCurrentActionComplete -= OnCurrentActionComplete;
		_parentCharacter.MyEventHandler.OnCurrentActionComplete += OnCurrentActionComplete;
		_parentCharacter.MyEventHandler.OnPerFrameTimer -= PerFrameUpdate;
		_parentCharacter.MyEventHandler.OnPerFrameTimer += PerFrameUpdate;

		//update parent character for each action
		foreach(GoapAction action in _actions)
		{
			action.ParentCharacter = _parentCharacter;
		}

		//BlackBoard.PatrolLoc = new Vector3(63.9f, 0.3f, -13.3f);
		//BlackBoard.PatrolRange = new Vector3(30, 10, 15);

		if(ControlType != AIControlType.Player)
		{
			BlackBoard.GuardLevel = 1;
			_parentCharacter.SendCommand(CharacterCommands.SetAlert);
		}

		_currentGoal = null;
		_currentAction = null;


		_parentCharacter.MyEventHandler.OnOneSecondTimer += OnOneSecondTimer;
	
	}




	public bool IsCharacterInParty(Character c)
	{
		
		if(c == null)
		{
			return false;
		}

		if(c == _parentCharacter)
		{
			return true;
		}

		if(Squad != null && Squad.Members.Contains(c))
		{
			return true;
		}

		return false;
	}
	/*
	public int GetCharacterRelationship(Character c)
	{
		if(_parentCharacter.Faction == c.Faction)
		{
			if(_parentCharacter.Faction != Faction.Loner)
			{
				return 4;
			}
			else
			{
				if(_parentCharacter.SquadID == c.SquadID)
				{
					return 4;
				}
				else
				{
					return (Convert.ToInt32(c.SquadID) + Convert.ToInt32(_parentCharacter.SquadID)) > 30 ? 1 : 2; //loner relationships are random
				}
			}
		}
		else
		{
			FactionData myFaction = GameManager.Inst.NPCManager.GetFactionData(_parentCharacter.Faction);
			FactionData otherFaction = GameManager.Inst.NPCManager.GetFactionData(c.Faction);

			if(myFaction == null || otherFaction == null)
			{
				return 1;
			}

			float relationship =  myFaction.GetRelationshipByID(c.Faction);
			if(relationship <= 0.25)
			{
				return 1;
			}
			else if(relationship <= 0.5)
			{
				return 2;
			}
			else if(relationship <= 0.75)
			{
				return 3;
			}
			else
			{
				return 4;
			}

		}
	}
	*/
	public int IsCharacterEnemy(Character c) //0=enemy, 1=negative neutral, 2=positive neutral, 3=friendly
	{
		if(_parentCharacter.Faction == c.Faction)
		{
			if(_parentCharacter.Faction != Faction.Loner)
			{
				return 3;
			}
			else
			{
				if(_parentCharacter.SquadID == c.SquadID)
				{
					return 3;
				}
				else
				{
					int rand = (Convert.ToInt32(c.SquadID) + Convert.ToInt32(_parentCharacter.SquadID)); //loner relationships are random
					if(rand > 30)
					{
						return 0;
					}
					else
					{
						return 2;
					}
				}
			}
		}
		else
		{
			FactionData myFaction = GameManager.Inst.NPCManager.GetFactionData(_parentCharacter.Faction);
			FactionData otherFaction = GameManager.Inst.NPCManager.GetFactionData(c.Faction);

			if(myFaction == null || otherFaction == null)
			{
				return 0;
			}

			float relationship = myFaction.GetRelationshipByID(c.Faction);
			if(relationship <= 0.25f)
			{
				return 0;
			}
			else if(relationship <= 0.5f)
			{
				return 1;
			}
			else if(relationship <= 0.75f)
			{
				return 2;
			}
			else if(relationship > 0.75f)
			{
				return 3;
			}

		}


		return 0;
	}


	//only return true when in same faction
	public bool IsCharacterFriendly(Character c)
	{
		if(_parentCharacter.Faction == c.Faction)
		{
			if(_parentCharacter.Faction != Faction.Loner)
			{
				return true;
			}
			else
			{
				if(_parentCharacter.SquadID == c.SquadID)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		return false;
	}

	public void Bark(string text)
	{
		//GameManager.Inst.UIManager.BarkPanel.AddBark(_parentCharacter, text);
	}

	public void CallForHelp(Character target)
	{
		//Debug.LogError(_parentCharacter.name + " is calling for help");
		if(Squad == null)
		{
			return;
		}

		//now send a disturbance to all  characters in squad

		foreach(Character c in Squad.Members)
		{
			if(c == _parentCharacter)
			{
				continue;
			}

			if(Vector3.Distance(c.transform.position, transform.position) > 20)
			{
				continue;
			}

			if(target == null)
			{
				c.MyAI.Sensor.OnReceiveDisturbance(1f, this, transform.position, _parentCharacter);
			}
			else
			{
				if(c.MyAI.BlackBoard.TargetEnemy == null || c.MyAI.BlackBoard.TargetEnemy == target)
				{
					c.MyAI.BlackBoard.TargetEnemy = target;
					c.MyAI.BlackBoard.TargetEnemyThreat = 1;
					c.MyAI.BlackBoard.GuardLevel = 2;
				}


			}
		}
	}

	public void SetDynamicyGoal(GoapGoal newGoal, int priority)
	{
		//CsDebug.Inst.CharLog(_parentCharacter, "Setting top priority goal " + newGoal.Name + " for " + _parentCharacter.name);
		List<GoapGoal> goalsCopy = new List<GoapGoal>(_goals);
		foreach(GoapGoal goal in goalsCopy)
		{
			if(goal.Priority == priority)
			{
				_goals.Remove(goal);
			}
		}

		_goals.Add(newGoal);

		if(_currentAction != null)
		{
			_currentAction.StopAction();
		}
		_currentAction = null;
		_currentGoal = newGoal;
		FindAndExecuteAction();
	}

	public void ClearDynamicGoal(int priority)
	{
		//CsDebug.Inst.CharLog(_parentCharacter, "Clearing top priority goal for " + _parentCharacter.name);
		List<GoapGoal> goalsCopy = new List<GoapGoal>(_goals);
		foreach(GoapGoal goal in goalsCopy)
		{
			if(goal.Priority == priority)
			{
				if(_currentGoal == goal)
				{
					_currentAction.StopAction();
				}
				_goals.Remove(goal);
				_currentAction = null;
				_currentGoal = null;
				FindAndExecuteAction();
			}
		}

	}

	public GoapGoal GetDynamicGoal(int priority)
	{
		foreach(GoapGoal goal in _goals)
		{
			if(goal.Priority == priority)
			{
				return goal;
			}
		}

		return null;
	}

	public GoapAction GetCurrentAction()
	{
		return _currentAction;
	}

	public GoapGoal GetCurrentGoal()
	{
		return _currentGoal;
	}

	public void ForceStopCurrentAction()
	{
		if(_currentAction != null)
		{
			_currentAction.StopAction();
		}

		_currentAction = null;
		_currentGoal = null;
	}

	public void SetCurrentWorldState(GoapWorldState state, object newValue)
	{
		GoapWorldState existing = (from s in _currentWorldStates where s.Name == state.Name select s).FirstOrDefault();
		if(existing != null)
		{
			existing.Value = newValue;
		}
		else
		{
			//if a world state with same name is not found in current list of world states, create a new one and add to current world states
			GoapWorldState newState = new GoapWorldState(state.ID, state.Name, state.Operator, newValue);
			_currentWorldStates.Add(newState);
		}
	}

	public List<GoapWorldState> GetCurrentWorldState()
	{
		return _currentWorldStates;
	}


	public bool CheckActionViable(GoapAction action)
	{
		//check if working memory's list of failed actions
		WorkingMemoryFact fact = WorkingMemory.FindExistingFact(FactType.FailedAction, action.Name);
		if(fact != null)
		{
			//CsDebug.Inst.CharLog(_parentCharacter, "Determined action " + action.Name + " as not viable");
			return false;
		}
		else
		{
			return true;
		}
	
		return true;
	}

	public object EvaluateWorldState(GoapWorldState state)
	{
		if(state.Name == "IsThreatInSightInArea")
		{
			//loop through all known enemies whose confidence is 1 (in sight)
			//return true if any is found

			List<WorkingMemoryFact> knownEnemies = WorkingMemory.FindExistingFactOfType(FactType.KnownEnemy);
			foreach(WorkingMemoryFact fact in knownEnemies)
			{
				if(fact.Confidence >= 1 && fact.ThreatLevel > 0.1f)
				{
					//if enemy is in sight, return true even if not in area
					CsDebug.Inst.CharLog(_parentCharacter, "Evaluating IsThreatInSightInArea, enemy in sight " + ((Character)fact.Target).name);
					SetCurrentWorldState(state, true);
					return true;
				}

			}

			//now loop through all known neutrals whose confidence is 1
			//return true if any is within area
			List<WorkingMemoryFact> knownNeutrals = WorkingMemory.FindExistingFactOfType(FactType.KnownNeutral);
			foreach(WorkingMemoryFact fact in knownNeutrals)
			{
				if(fact.Confidence >= 1  && fact.ThreatLevel > 0.1f)
				{
					Vector3 position = ((Character)fact.Target).transform.position;

					//check if the neutral is within patrol range
					Vector3 targetLoc = BlackBoard.PatrolLoc;
					Vector3 targetRange = BlackBoard.CombatRange;
					if(IsPositionInArea(position, targetLoc, targetRange))
					{
						CsDebug.Inst.CharLog(_parentCharacter, "Evaluation IsThreatInSightInArea, true, visible neutrals in area");
						SetCurrentWorldState(state, true);
						return true;
					}
				}
			}

			CsDebug.Inst.CharLog(_parentCharacter, "Evaluation IsThereThreatInArea, false");
			SetCurrentWorldState(state, false);
			return false;
		}

		else if(state.Name == "IsThereInvisibleEnemy")
		{
			bool result = (_parentCharacter.MyAI.BlackBoard.InvisibleEnemy != null);
			SetCurrentWorldState(state, result);

			return result;
		}

		else if(state.Name == "IsTargetAThreat")
		{


			Character target = BlackBoard.TargetEnemy;

			if(BlackBoard.TargetEnemyThreat < 0.1f)
			{
				SetCurrentWorldState(state, false);
				return false;
			}

			if(target != null && target.MyStatus.Health <= 0)
			{
				SetCurrentWorldState(state, false);
				return false;
			}

			if(target == null && BlackBoard.InvisibleEnemy != null)
			{
				target = BlackBoard.InvisibleEnemy;
			}

			if(target == null)
			{
				SetCurrentWorldState(state, false);
				return false;
			}

			WorkingMemoryFact fact = WorkingMemory.FindExistingFact(FactType.KnownEnemy, target);

			if(fact != null && fact.Confidence >= 1)
			{
				SetCurrentWorldState(state, true);
				return true;
			}
			else if(fact != null && fact.ThreatLevel >= 0.6f)
			{
				SetCurrentWorldState(state, true);
				return true;
			}
			else if(fact != null)
			{
				//check if last known pos is inside area
				Vector3 position = fact.LastKnownPos;

				//check if the enemy is within patrol range
				Vector3 targetLoc = BlackBoard.PatrolLoc;
				Vector3 targetRange = BlackBoard.CombatRange;
				if(IsPositionInArea(position, targetLoc, targetRange))
				{
					SetCurrentWorldState(state, true);
					return true;
				}
			}
	

			SetCurrentWorldState(state, false);
			return false;

		}

		else if(state.Name == "IsTherePersonalThreat")
		{
			bool result = BlackBoard.HighestPersonalThreat > 0;
			SetCurrentWorldState(state, result);
			CsDebug.Inst.CharLog(_parentCharacter, "Is there personal threat? " + BlackBoard.HighestPersonalThreat);
			return result;
		}

		else if(state.Name == "IsThereCriticalPersonalThreat")
		{
			bool result = BlackBoard.HighestPersonalThreat >= 1;
			SetCurrentWorldState(state, result);
			return result;
		}

		else if(state.Name == "IsRangedWeaponEquipped")
		{
			if(BlackBoard.EquippedWeapon != null)
			{
				SetCurrentWorldState(state, true);
				return true;
			}
			else
			{
				SetCurrentWorldState(state, false);
				return false;
			}
		}

		else if(state.Name == "IsThreatInSight")
		{
			List<WorkingMemoryFact> knownEnemies = WorkingMemory.FindExistingFactOfType(FactType.KnownEnemy);
			foreach(WorkingMemoryFact fact in knownEnemies)
			{
				//Debug.Log("Is Threat In Sight fact confidence " + fact.Confidence);
				if(fact.Confidence >= 1 && fact.ThreatLevel > 0.1f)
				{
					SetCurrentWorldState(state, true);
					return true;
				}
			}

			SetCurrentWorldState(state, false);
			return false;
		}

		else if(state.Name == "IsAmmoAvailable")
		{
			SetCurrentWorldState(state, true);
			return true;
		}

		else if(state.Name == "IsBehindCover")
		{
			bool result = true;

			if(BlackBoard.SelectedCover == null)
			{
				result = false;
			}
			else
			{
				result = Vector3.Distance(_parentCharacter.transform.position, BlackBoard.SelectedCoverLoc) <= 2 ? true : false;
			}

			SetCurrentWorldState(state, result);
			return result;
		}

		else if(state.Name == "IsBehindAttackCover")
		{
			bool result = true;

			if(BlackBoard.SelectedCover == null)
			{
				result = false;
			}
			else
			{
				if(Vector3.Distance(_parentCharacter.transform.position, BlackBoard.SelectedCoverLoc) <= 1 && BlackBoard.SelectedCover.IsForShooting)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}

			SetCurrentWorldState(state, result);
			return result;
		}

		else if(state.Name == "IsThereDisturbance")
		{
			bool result = BlackBoard.HighestDisturbanceThreat > 0;
			SetCurrentWorldState(state, result);
			CsDebug.Inst.CharLog(_parentCharacter, "Is there disturbance? " + BlackBoard.HighestDisturbanceThreat);
			return result;
		}

		else if(state.Name == "IsNearDefendPosition")
		{
			SetCurrentWorldState(state, false);
			return false;

			if(!BlackBoard.HasPatrolInfo)
			{
				SetCurrentWorldState(state, false);
				return false;
			}

			if(Vector3.Distance(_parentCharacter.transform.position, BlackBoard.PatrolLoc) > 1)
			{
				SetCurrentWorldState(state, false);
				return false;
			}



			SetCurrentWorldState(state, true);
			return true;
		}

		else if(state.Name == "IsInTargetArea")
		{
			bool result = IsPositionInArea(_parentCharacter.transform.position, BlackBoard.PatrolLoc, BlackBoard.PatrolRange);
			//CsDebug.Inst.CharLog(_parentCharacter, "Checking IsInTargetArea, result is " + result);
			SetCurrentWorldState(state, result);
			return result;
		}

		else if(state.Name == "IsNearFollowingTarget")
		{
			SetCurrentWorldState(state, false);
			return false;
		}

		else if(state.Name == "IsThereUncheckedCorpse")
		{
			if(BlackBoard.TargetCorpse != null && BlackBoard.TargetCorpse.ThreatLevel > 0 && Vector3.Distance(BlackBoard.TargetCorpse.LastKnownPos, _parentCharacter.transform.position) < 30)
			{
				SetCurrentWorldState(state, true);
				return true;
			}

			SetCurrentWorldState(state, false);
			return false;
		}
		else if(state.Name == "AlwaysFalse")
		{
			SetCurrentWorldState(state, false);
			return false;
		}

		return null;
	}



	#region events
	//AI Events
	public void OnImportantEvent(float priority) //priority ranges from 0 to 1. 1 is highest priority
	{
		//Debug.Log("On important event " + priority);
		StartCoroutine(WaitAndCheckImportantEvent(0.1f, priority));

	}

	IEnumerator WaitAndCheckImportantEvent(float waitTime, float priority)
	{
		yield return new WaitForSeconds(waitTime);

		if(ControlType == AIControlType.Player)
		{
			yield return 0;
		}

		//when an important event happens, stop current action and reevaluate all the goals and pick the highest priority goal
		//Debug.Log("Triggering important event " + _parentCharacter.name);
		if(_currentAction != null)
		{
			if(_currentAction.AbortAction(priority))
			{
				_currentAction = null;
				_currentGoal = null;
				FindAndExecuteAction();
			}
		}
		else
		{
			_currentGoal = null;
			FindAndExecuteAction();
		}
	}


	public void OnCurrentActionComplete()
	{
		/*	
		//if no known enemies then lower guard level
		if(BlackBoard.NumberOfKnownEnemies <= 0)
		{
			if(_parentCharacter.MyAI.BlackBoard.GuardLevel > 1)
			{
				_parentCharacter.MyAI.BlackBoard.GuardLevel = 1;
				_parentCharacter.SendCommand(CharacterCommands.SetAlert);
			}

		}
		*/

		StartCoroutine(WaitAndExecuteNextAction(0.1f));

	}

	IEnumerator WaitAndExecuteNextAction(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		//Debug.Log("Triggering action complete " + _parentCharacter.name);

		if(_actionQueue != null && _actionQueue.Count > 0)
		{
			_currentAction = _actionQueue.Dequeue();
			//CsDebug.Inst.CharLog(_parentCharacter, "After action completion, dequeued new action " + _currentAction.Name);
			if(_currentAction.ExecuteAction())
			{
				//action executed successfully
				yield return 0;
			}
			else
			{
				_currentAction = null;
				FindAndExecuteAction();
			}
		}
		else
		{
			//CsDebug.Inst.CharLog(_parentCharacter, "No more actions, evaluating goal");
			//now there are no more actions; evaluate the goal again. if goal is reached, then find new goal; if goal has not reached, rerun planner
			//CsDebug.Inst.CharLog(_parentCharacter, "After action completion, looking for new action " + _currentAction.Name);
			_currentAction = null;
			FindAndExecuteAction();
		}

	}

	public void OnOneSecondTimer()
	{
		//check if there is current goal. if there is none then get one
		if(ControlType != AIControlType.Player && _currentGoal == null)
		{
			_currentAction = null;

			FindAndExecuteAction();
		}
	}

	public void OnDeath()
	{
		if(_currentAction != null)
		{
			_currentAction.AbortAction(1);
			_currentAction = null;
		}

		_currentGoal = null;

		_parentCharacter.MyEventHandler.OnOneSecondTimer -= OnOneSecondTimer;
		_parentCharacter.MyEventHandler.OnCurrentActionComplete -= OnCurrentActionComplete;
		_parentCharacter.MyEventHandler.OnPerFrameTimer -= PerFrameUpdate;

		TargetingSystem.OnDeath();
		Sensor.OnDeath();
		WorkingMemory.OnDeath();

		if(Squad != null)
		{
			Squad.RemoveMember(_parentCharacter);
		}
	}
	#endregion


	private void AddGoal(GoapGoal goal)
	{
		if(_goals.Count < 5)
		{
			_goals.Add(goal);
		}
	}

	private GoapGoal GetNextGoal()
	{
		GoapGoal result = null;
		if(_currentGoal == null)
		{
			//get the highest priority goal
			GoapGoal goal = _goals.OrderBy(p => p.Priority).FirstOrDefault();
			result = goal;

		}
		else
		{
			//get the goal that's next highest than current goal
			var intermediate = (from g in _goals where g.Priority >= _currentGoal.Priority && g != _currentGoal orderby g.Priority select g);
			GoapGoal goal = intermediate.FirstOrDefault();

			result = goal;
		}

		return result;
	}

	private bool EvaluateGoal(GoapGoal goal)
	{
		//check which goal's conditions are met

		bool isGoalMet = true;
		foreach(GoapWorldState state in goal.GoalStates)
		{
			object value = EvaluateWorldState(state);
			CsDebug.Inst.CharLog(_parentCharacter, "Evaluating Goal; current value " + value + " target value " + state.Value);
			if(!UnityEngine.Object.Equals(value, state.Value))
			{
				CsDebug.Inst.CharLog(_parentCharacter, "Evaluating goal result: values don't match");
				//this goal isn't met!
				isGoalMet = false;
				return isGoalMet;
			}
		}

		return isGoalMet;
	}


	private void FindAndExecuteAction()
	{
		//check if there is current goal and if the current goal is met
		//if met, find next goal
		//if not met, check if there's current action. if there is action leave it alone. if no action then calculate planner and execute first action
		//if no current goal, get next goal

		CsDebug.Inst.CharLog(_parentCharacter, "Start finding and executing action");

		if(_currentGoal == null)
		{
			CsDebug.Inst.CharLog(_parentCharacter, "no current goal, getting a new one");
			_currentGoal = GetNextGoal();

			CsDebug.Inst.CharLog(_parentCharacter, "found new goal " + _currentGoal.Name);
			_currentAction = null;
		}

		int counter = _goals.Count;
		while(counter > 0 && _currentGoal != null)
		{
			CsDebug.Inst.CharLog(_parentCharacter, "Find&Execute: checking goal " + _currentGoal.Name);
			bool needNextGoal = false;
			if(_currentAction == null && _currentGoal != null)
			{
				if(!EvaluateGoal(_currentGoal))
				{
					CsDebug.Inst.CharLog(_parentCharacter, "current goal " + _currentGoal.Name + " is not met, running planner, character " + _parentCharacter.name);
					_actionQueue = Planner.GetActionQueue(_currentGoal, _actions);
					if(_actionQueue != null && _actionQueue.Count > 0)
					{
						_currentAction = _actionQueue.Dequeue();
						CsDebug.Inst.CharLog(_parentCharacter, "Found current action " + _currentAction.Name + " for goal " + _currentGoal.Name); 
						if(_currentAction.ExecuteAction())
						{
							//action executed successfully
							return;
						}
					}
					//failed to find action for current goal, get next goal
					CsDebug.Inst.CharLog(_parentCharacter, "Failed to find action for goal " + _currentGoal.Name);
					needNextGoal = true;
				}
				else
				{
					CsDebug.Inst.CharLog(_parentCharacter, "Goal is already met: " + _currentGoal.Name);
					needNextGoal = true;
				}
			}
			else
			{
				//there's action; leave it alone
				return;
			}

			if(needNextGoal)
			{
				_currentGoal = GetNextGoal();

				_currentAction = null;

				if(_currentGoal != null)
				{
					CsDebug.Inst.CharLog(_parentCharacter, "getting next goal; result: " + _currentGoal.Name);
				}
				else
				{
					CsDebug.Inst.CharLog(_parentCharacter, "getting next goal; result: null");
				}
			}


			counter --;
		}
	}

	public static NavNode FindNextNavNode(NavNode start, NavNode destination)
	{
		if(start == null || destination == null || start == destination)
		{
			Debug.Log("start is null " + (start == null) + " dest is null " + (destination == null) + " start=dest " + (start == destination));
			return null;
		}

		Debug.Log("AI Pathfinding start " + start.name + " destination " + destination.name);

		List<NavNode> closedSet = new List<NavNode>();
		List<NavNode> openSet = new List<NavNode>();
		openSet.Add(start);

		LinkedList<NavNode> result = new LinkedList<NavNode>();
		LinkedListNode<NavNode> currentResultNode;

		Dictionary<NavNode,float> gScores = new Dictionary<NavNode, float>();
		Dictionary<NavNode,float> fScores = new Dictionary<NavNode, float>();

		List<NavNode> allNodes = GameManager.Inst.NPCManager.GetAllNavNodes();
		foreach(NavNode n in allNodes)
		{
			gScores.Add(n, Mathf.Infinity);
			fScores.Add(n, Mathf.Infinity);
		}

		gScores[start] = 0;
		fScores[start] = Vector3.Distance(start.transform.position, destination.transform.position);

		while(openSet.Count > 0)
		{
			NavNode bestNode = openSet[0];
			foreach(NavNode node in openSet)
			{
				if(fScores[node] < fScores[bestNode])
				{
					bestNode = node;
				}
			}

			if(bestNode == destination)
			{
				result.AddFirst(bestNode);
				break;
			}

			openSet.Remove(bestNode);
			closedSet.Add(bestNode);

			foreach(NavNode neighbor in bestNode.Neighbors)
			{
				if(closedSet.Contains(neighbor))
				{
					continue;
				}

				if(!openSet.Contains(neighbor))
				{
					openSet.Add(neighbor);
				}

				float tentativeGScore = gScores[bestNode] + Vector3.Distance(bestNode.transform.position, neighbor.transform.position);
				//Debug.Log("best node " + bestNode.name + " tentative g " + tentativeGScore + " neighbor g " + gScores[neighbor] + " " + neighbor.name);
				if(tentativeGScore >= gScores[neighbor])
				{
					continue;
				}

				if(!result.Contains(bestNode))
				{
					result.AddFirst(bestNode);
				}
				gScores[neighbor] = tentativeGScore;
				fScores[neighbor] = gScores[neighbor] + Vector3.Distance(neighbor.transform.position, destination.transform.position);

			}
		}


		currentResultNode = result.First;
		while(currentResultNode != null)
		{
			Debug.Log("pathfinding result " + currentResultNode.Value.name);
			currentResultNode = currentResultNode.Next;
		}

		Debug.Log("Going to return pathfinding result " + result.Last.Previous.Value);
		return result.Last.Previous.Value;
	}





	public static bool RandomPoint(Vector3 center, Vector3 range, out Vector3 result) 
	{
		for (int i = 0; i < 10; i++) 
		{
			Vector3 randomPoint = new Vector3(UnityEngine.Random.Range(center.x - range.x, center.x + range.x),
												UnityEngine.Random.Range(center.y - range.y, center.y + range.y),
												UnityEngine.Random.Range(center.z - range.z, center.z + range.z));
			UnityEngine.AI.NavMeshHit hit;
			if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas)) 
			{
				result = hit.position;
				return true;
			}
		}
		result = Vector3.zero;
		return false;
	}

	public static bool IsPositionInArea(Vector3 position, Vector3 targetLoc, Vector3 targetRange)
	{
		if(targetRange.x == targetRange.z)
		{
			//circular area
			if(Vector3.Distance(position, targetLoc) < targetRange.x)
			{
				return true;
			}
		}
		else
		{
			if(position.x <= targetLoc.x + targetRange.x && position.x >= targetLoc.x - targetRange.x
				&& position.y <= targetLoc.y + targetRange.y && position.y >= targetLoc.y - targetRange.y
				&& position.z <= targetLoc.z + targetRange.z && position.z >= targetLoc.z - targetRange.z)
			{
				return true;
			}
		}
		return false;
	}

	public float GetEnemyFacingAngle(Character enemy)
	{
		Vector3 los = enemy.transform.position - _parentCharacter.transform.position;
		return Vector3.Angle(los, enemy.transform.forward);
	}

}
