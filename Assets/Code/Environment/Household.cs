using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Household : MonoBehaviour
{
	public Vector3 PatrolRange;
	public Vector3 CombatRange;
	public List<Transform> GuardLocs;
	public List<Transform> PatrolNodes;
	public List<IdleDest> IdleDests;
}
