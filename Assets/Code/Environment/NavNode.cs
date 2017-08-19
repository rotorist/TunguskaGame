using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode : MonoBehaviour 
{
	public NavNodeType Type;
	public Household Household;
	public List<NavNode> Neighbors;
	public bool IsOpenToExpedition;
}

public enum NavNodeType
{
	Waypoint,
	MutantHunt,
	Base,
}