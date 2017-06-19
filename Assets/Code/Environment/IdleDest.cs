using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleDest : MonoBehaviour 
{
	public IdleDestType Type;
	public bool IsOccupied;

	public int GetStayTimeout()
	{
		switch(Type)
		{
		case IdleDestType.ChairSit:
			return UnityEngine.Random.Range(60, 300);
		case IdleDestType.CommanderStand:
			return 300;
		case IdleDestType.GroundSit:
			return UnityEngine.Random.Range(120, 300);
		case IdleDestType.Sleep:
			return UnityEngine.Random.Range(300, 500);
		default:
			return UnityEngine.Random.Range(100, 200);
		}
	}
}

public enum IdleDestType
{
	Stand,
	ChairSit,
	GroundSit,
	Trade,
	Smoke,
	Sleep,
	CommanderStand,
}
