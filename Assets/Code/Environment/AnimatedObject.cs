using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedObject : MonoBehaviour 
{
	public AnimatedObjectType MovementType;
	public float Speed;

	
	// Update is called once per frame
	void Update () 
	{
		switch(MovementType)
		{
		case AnimatedObjectType.Spin:
			transform.RotateAround(transform.position, transform.forward, Speed * Time.deltaTime);
			break;
		}
	}
}

public enum AnimatedObjectType
{
	Backforth,
	Swing,
	Spin,
}