using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoapGoal 
{
	public string Name;
	public List<GoapWorldState> GoalStates;

	//every goal is defined as a set of world states related to 
	//gameobject TargetObject, or applied at location TargetLocation with range TargetRange
	//public GameObject TargetObject;
	//public Vector3 TargetLoc;
	//public Vector3 Range; //targetLoc.x plus and minus Range.x, targetLoc.y plus and minus Range.y, etc.

	public int Priority; //lower number = higher priority
}
