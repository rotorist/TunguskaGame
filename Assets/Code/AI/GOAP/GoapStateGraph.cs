using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoapStateGraph 
{
	public GoapStateGraphNode Goal;
	public HashSet<GoapStateGraphNode> Nodes;

	public GoapStateGraphNode AddNode(List<GoapWorldState> unsatisfied, List<GoapWorldState> satisfied)
	{
		GoapStateGraphNode node = new GoapStateGraphNode(unsatisfied, satisfied);
		Nodes.Add(node);
		return node;
	}
}

public class GoapStateGraphNode
{
	public List<GoapWorldState> SatisfiedStates;
	public List<GoapWorldState> UnsatisfiedStates;
	public float HeuristicCost;
	public float CostSinceStart;
	public GoapStateGraphNode CameFrom;
	public GoapAction CameFromAction;

	public GoapStateGraphNode(List<GoapWorldState> unsatisfied, List<GoapWorldState> satisfied)
	{
		SatisfiedStates = satisfied;
		UnsatisfiedStates = unsatisfied;
		CostSinceStart = float.PositiveInfinity;
		CameFrom = null;
		CameFromAction = null;
	}
}