using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour 
{
	public Lamp MyLamp;

	public void Toggle()
	{
		MyLamp.Toggle();
	}

}
