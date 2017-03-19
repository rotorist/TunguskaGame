using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Household : MonoBehaviour
{
	public Vector3 PatrolRange;
	public Vector3 CombatRange;
	public List<Vector3> GuardLocs;
	public List<Vector3> GuardDirs;
	public List<Vector3> PatrolNodes;
}
