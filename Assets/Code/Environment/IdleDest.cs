using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleDest : MonoBehaviour 
{
	public IdleDestType Type;
	public bool IsOccupied;
}

public enum IdleDestType
{
	ChairSit,
	GroundSit,
	Stand,
	Smoke,
	Sleep,

}
