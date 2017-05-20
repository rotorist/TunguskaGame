using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour 
{
	public Lamp [] MyLamps;

	public void Toggle()
	{
		foreach(Lamp lamp in MyLamps)
		{
			lamp.Toggle();
		}
	}

}
