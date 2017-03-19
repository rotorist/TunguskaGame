using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GoapPlanner
{
	
	private AI _parentAI;
	private List<GoapWorldState> _evaluatedStates;

	public GoapPlanner(AI parentAI)
	{
		_parentAI = parentAI;
	}

	public Queue<GoapAction> GetActionQueue(GoapGoal myGoal, List<GoapAction> availableActions)
	{
			
		GoapStateGraphNode goal = new GoapStateGraphNode(myGoal.GoalStates, new List<GoapWorldState>());
		goal.CostSinceStart = 0;
		goal.HeuristicCost = GetHeuristic(goal);
		goal.UnsatisfiedStates = myGoal.GoalStates;

		List<GoapStateGraphNode> openNodes = new List<GoapStateGraphNode>();
		openNodes.Add(goal);


		List<GoapStateGraphNode> closedNodes = new List<GoapStateGraphNode>();

		GoapStateGraphNode resultFirstNode = null;

		_evaluatedStates = new List<GoapWorldState>();

		//evaluate world states in goal conditions
		foreach(GoapWorldState state in myGoal.GoalStates)
		{
			CheckWorldState(state);
		}

		//Debug.Log("starting goap planner while loop " + _parentAI.name);

		while(openNodes.Count() > 0)
		{
			//starting from the graph goal node, for each world state look for a action that will satisfy it.
			//when an action is found, create a new graph node, add action effect to satified states, and add action preconditions to unsatisfied states
			//then sort all neighbors into a list ordered by cost+heuristic
			//for each neighbor in the list, starting from the one with lowest cost+heuristic, 


			//find the lowest f_score node in open nodes
			GoapStateGraphNode bestOpenNode = openNodes[0];
			foreach(GoapStateGraphNode node in openNodes)
			{
				if(node.HeuristicCost < bestOpenNode.HeuristicCost)
				{
					bestOpenNode = node;

				}
			}

			CsDebug.Inst.Log("======Evaluating best open node with cost " + bestOpenNode.CostSinceStart, CsDLevel.Debug, CsDComponent.AI);
			if(bestOpenNode.CameFromAction != null)
			{
				CsDebug.Inst.Log("Came from Action " + bestOpenNode.CameFromAction.Name, CsDLevel.Debug, CsDComponent.AI);
			}

			//evaluate the current world state for states in current node
			foreach(GoapWorldState s in bestOpenNode.UnsatisfiedStates)
			{
				CheckWorldState(s);
				CsDebug.Inst.Log("Current Unsatisfied State: " + s.Name + s.Operator + s.Value, CsDLevel.Debug, CsDComponent.AI);
			}
			foreach(GoapWorldState s in bestOpenNode.SatisfiedStates)
			{
				CheckWorldState(s);
				//CsDebug.Inst.Log("Current Satisfied State: " + s.Name + s.Operator + s.Value, CsDLevel.Debug, CsDComponent.AI);
			}


			//if all unsatisfied states can be solved by current world state then we have found a path
			if(CompareWorldStatesContain(_parentAI.GetCurrentWorldState(), bestOpenNode.UnsatisfiedStates))
			{
				//search complete, found a path!
				CsDebug.Inst.Log("Found best path!", CsDLevel.Info, CsDComponent.AI);
				resultFirstNode = bestOpenNode;
				break;
			}

			openNodes.Remove(bestOpenNode);
			closedNodes.Add(bestOpenNode);

			//find all neighbors
			foreach(GoapAction action in availableActions)
			{
				CsDebug.Inst.Log("------Starting to evaluate action " + action.Name, CsDLevel.Debug, CsDComponent.AI);
				List<GoapWorldState> usefulEffects = CompareWorldStatesContainAny(bestOpenNode.UnsatisfiedStates, action.Effects);
				if(usefulEffects == null)
				{
					CsDebug.Inst.Log("Action has no useful effects!", CsDLevel.Debug, CsDComponent.AI);
					//found conflict, or action is marked as not viable from previous attempts
					continue;
				}

				if(!_parentAI.CheckActionViable(action))
				{
					CsDebug.Inst.Log("Action has been failed before!", CsDLevel.Debug, CsDComponent.AI);
					//found conflict, or action is marked as not viable from previous attempts
					continue;
				}

				if(!action.CheckContextPrecondition())
				{
					CsDebug.Inst.Log("Action is not viable!", CsDLevel.Debug, CsDComponent.AI);
					//found conflict, or action is marked as not viable from previous attempts
					continue;
				}

				else if(usefulEffects.Count > 0)
				{
					CsDebug.Inst.Log("Found action " + action.Name + " that satisfies preconditions set containing " + bestOpenNode.UnsatisfiedStates.Count(), CsDLevel.Debug, CsDComponent.AI);
					CsDebug.Inst.Log("Current best open node unsatisfied states contains " + bestOpenNode.UnsatisfiedStates[0].Name + "=" + bestOpenNode.UnsatisfiedStates[0].Value, CsDLevel.Trace, CsDComponent.AI);

					//neighbor's effect is removed from neighbor unsatisfied list and added to the satisfied list
					//any precondition that hasn't been resolved or listed is added to the neighbor's unsatisfied list
					//this way we don't have any dupilicate states
					List<GoapWorldState> neighborUnsatisfied = new List<GoapWorldState>(bestOpenNode.UnsatisfiedStates);
					List<GoapWorldState> neighborSatisfied = new List<GoapWorldState>(bestOpenNode.SatisfiedStates);

					bool isNoConflict = true;

					foreach(GoapWorldState effect in usefulEffects)
					{
						RemoveStateFromSet(neighborUnsatisfied, effect);
						isNoConflict = AddStateToSet(neighborSatisfied, effect);

						if(!isNoConflict)
						{
							//CsDebug.Inst.Log("Found conflict when adding effect " + effect.Name + effect.Value + "to neighbor satisfied set", CsDLevel.Trace, CsDComponent.AI);
							continue;
						}
					}


					isNoConflict = true;


					foreach(GoapWorldState condition in action.Preconditions)
					{
						//is this condition already in the neighborSatisfied list?
						//is this condition already in the neighborUnsatisfied list?
						//if both answer is no, add the new condition to neighborUnsatisfied list


						bool isInSatisfied = CompareWorldStatesContain(neighborSatisfied, new List<GoapWorldState>{condition});

						bool isInUnsatisfied = CompareWorldStatesContain(neighborUnsatisfied, new List<GoapWorldState>{condition});

						if(!isInSatisfied && !isInUnsatisfied)
						{
							isNoConflict = AddStateToSet(neighborUnsatisfied, condition);
							if(!isNoConflict)
							{
								//CsDebug.Inst.Log("Found conflict when adding condition " + condition.Name + condition.Value + "to neighbor UNsatisfied set", CsDLevel.Trace, CsDComponent.AI);
								continue;
							}
						}
					}


					//CsDebug.Inst.Log("Successfully added preconditions for action " + action.Name, CsDLevel.Debug, CsDComponent.AI);

					//now we check if the above neighbor unsatisfied+satisfied combination exists in any open or closed nodes
					//if in closed nodes, we skip this neighbor. if in open nodes, we will calculate its heuristic cost
					//if not in any set, create a new node
					bool isInClosed = false;
					foreach(GoapStateGraphNode node in closedNodes)
					{
						if(CompareWorldStatesEqual(node.UnsatisfiedStates, neighborUnsatisfied) && CompareWorldStatesEqual(node.SatisfiedStates, neighborSatisfied))
						{
							isInClosed = true;
						}
					}

					if(isInClosed)
					{
						continue;
					}

					//CsDebug.Inst.Log("checking if node already exists in openNodes", CsDLevel.Debug, CsDComponent.AI);
					GoapStateGraphNode openNode = null;
					foreach(GoapStateGraphNode node in openNodes)
					{
						
						if(CompareWorldStatesEqual(node.UnsatisfiedStates, neighborUnsatisfied) && CompareWorldStatesEqual(node.SatisfiedStates, neighborSatisfied))
						{
							//CsDebug.Inst.Log("Node already exists", CsDLevel.Debug, CsDComponent.AI);
							openNode = node;
						}
					}

					float tentativeCost = bestOpenNode.CostSinceStart + action.GetActionCost();

					if(openNode != null)
					{
						//CsDebug.Inst.Log("Node exists, check its cost", CsDLevel.Debug, CsDComponent.AI);
						if(tentativeCost >= openNode.CostSinceStart)
						{
							//CsDebug.Inst.Log("Invalid path!", CsDLevel.Debug, CsDComponent.AI);
							//this is NOT a good path
							continue;
						}



					}
					else
					{
						//add a new node into openNodes
						openNode = new GoapStateGraphNode(neighborUnsatisfied, neighborSatisfied);
						CsDebug.Inst.Log("Adding a new neighbor node with " + neighborUnsatisfied.Count() + " unsatisfied conditions", CsDLevel.Debug, CsDComponent.AI);
						openNodes.Add(openNode);
					}

					CsDebug.Inst.Log("~~~~~~~~~Found a valid path, assigning neighbor parameters" + action.Name, CsDLevel.Info, CsDComponent.AI);
					openNode.CameFrom = bestOpenNode;
					openNode.CameFromAction = action;
					openNode.CostSinceStart = tentativeCost;
					openNode.HeuristicCost = openNode.CostSinceStart + GetHeuristic(openNode);
				}
			}

		}

		if(resultFirstNode == null)
		{
			//CsDebug.Inst.Log("No path found! Exiting planner.", CsDLevel.Info, CsDComponent.AI);
			return null;
		}

		Queue<GoapAction> actions = new Queue<GoapAction>();
		GoapStateGraphNode currentNode = resultFirstNode;

		//CsDebug.Inst.Log("Start enqueueing actions", CsDLevel.Debug, CsDComponent.AI);

		while(currentNode.CameFrom != null)
		{
			//CsDebug.Inst.Log("Enqueueing action " + currentNode.CameFromAction.Name, CsDLevel.Info, CsDComponent.AI);
			//Debug.Log("Enqueueing action " + currentNode.CameFromAction.Name);
			actions.Enqueue(currentNode.CameFromAction);
			currentNode = currentNode.CameFrom;
		}

		return actions;

	}





	private object CheckWorldState(GoapWorldState state)
	{
		//first check if the state has already been checked, i.e. in the evaluated states list
		//if not in the list run parent AI evaluate world state
		//if in the list just return current world state
		foreach(GoapWorldState s in _evaluatedStates)
		{
			if(state.Name == s.Name)
			{
				return s.Value;
			}
		}
		object value = _parentAI.EvaluateWorldState(state);
		_evaluatedStates.Add(state);

		return value;
	}
	


	private bool CompareWorldStatesContain(List<GoapWorldState> set1, List<GoapWorldState> set2)
	{
		//returns true if all states in set2 exists in set1
		//CsDebug.Inst.Log("Set1 contains " + set1.Count + " states", CsDLevel.Debug, CsDComponent.AI);
		foreach(GoapWorldState state2 in set2)
		{
			bool matchFound = false;
			foreach(GoapWorldState state1 in set1)
			{
				if(GoapWorldState.Compare(state1, state2) == false)
				{
					CsDebug.Inst.Log("GoapPlanner/CompareWorldStatesContain: state 1 " + state1.Name + state1.Value + " not equal to state 2 " + state2.Name + state2.Value, CsDLevel.Debug, CsDComponent.AI);
				}
				else
				{
					CsDebug.Inst.Log("GoapPlanner/CompareWorldStatesContain: state 1 " + state1.Name + state1.Value + " equals to state 2 " + state2.Name + state2.Value, CsDLevel.Debug, CsDComponent.AI);
					matchFound = true;
				}
			}

			if(!matchFound)
			{
				return false;
			}
		}

		return true;
	}

	private List<GoapWorldState> CompareWorldStatesContainAny(List<GoapWorldState> set1, List<GoapWorldState> set2)
	{
		//returns any state in set2 that's part of set1
		//returns null if there's conflict
		List<GoapWorldState> result = new List<GoapWorldState>();
		foreach(GoapWorldState state2 in set2)
		{
			foreach(GoapWorldState state1 in set1)
			{
				//Debug.Log("GoapPlanner/CompareWorldStatesContainAny: Comparing state1 " + state1.Name + state1.Value + " vs state2 " + state2.Name + state2.Value);
				if(state1.Name == state2.Name)
				{
					if(state1.Operator != state2.Operator)
					{
						//CsDebug.Inst.Log("GoapPlanner/CompareWorldStatesContain: Operator mismatch!");
						return null;
					}
					else if(GoapWorldState.Compare(state1, state2) == true)
					{
						//Debug.Log("GoapPlanner/CompareWorldStatesContainAny: state 1 " + state1.Name + " equals to state 2 " + state2.Name);
						result.Add(state2);
					}
					else
					{
						//Debug.Log("GoapPlanner/CompareWorldStatesContainAny: States with same name and operator have different values!");
						return null;
					}
				}

			}

		}

		return result;
	}

	private bool CompareWorldStatesEqual(List<GoapWorldState> set1, List<GoapWorldState> set2)
	{
		foreach(GoapWorldState state1 in set1)
		{
			bool matchFound = false;
			foreach(GoapWorldState state2 in set2)
			{
				if(state1.Name == state2.Name && state1.Operator == state2.Operator && state1.Value == state2.Value)
				{
					//CsDebug.Inst.Log("GoapPlanner/CompareWorldStatesEqual: state1 equals state2: " + state1.Name + state1.Operator + state1.Value);
					matchFound = true;
				}

			}

			if(!matchFound)
			{
				return false;
			}
		}

		return true;
	}

	private void RemoveStateFromSet(List<GoapWorldState> set, GoapWorldState state)
	{
		List<GoapWorldState> copy = new List<GoapWorldState>(set);
		foreach(GoapWorldState s in copy)
		{
			if(s.Name == state.Name)
			{
				set.Remove(s);
			}
		}
	}

	private bool AddStateToSet(List<GoapWorldState> set, GoapWorldState state)
	{
		//CsDebug.Inst.Log("GoapPlanner/AddStateToSet: Adding state " + state.Name + " = " + state.Value + " to set");
		//returns true if no conflict; false if there's conflict
		foreach(GoapWorldState s in set)
		{
			//CsDebug.Inst.Log("GoapPlanner/AddStateToSet: checking state " + state.Name + "=" + state.Value + " against set state "
			//	+ s.Name + "=" + s.Value);
			if(s.Name == state.Name)
			{
				//duplicates found, try to combine
				if(s.Operator != state.Operator)
				{
					//CsDebug.Inst.Log("GoapPlanner/AddStateToSet: operator mismatch!");
					return false;
				}


				return s.CombineValue(state);
			}
		}
		//no duplicates found, add it
		//CsDebug.Inst.Log("GoapPlanner/AddStateToSet: No duplicate found, adding state");
		set.Add(state);
		return true;
	}

	private float GetHeuristic(GoapStateGraphNode node)
	{

		return 0;
	}
}
