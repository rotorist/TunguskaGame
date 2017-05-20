using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour 
{
	public Light LightSource;
	public MeshRenderer BulbOn;
	public MeshRenderer BulbOff;
	public bool IsOn;

	void Start()
	{
		if(IsOn)
		{
			//turn on
			LightSource.enabled = true;
			BulbOn.enabled = true;
			BulbOff.enabled = false;
			IsOn = true;

		}
		else
		{
			//turn off
			LightSource.enabled = false;
			BulbOn.enabled = false;
			BulbOff.enabled = true;
			IsOn = false;
		}
	}


	public void Toggle()
	{
		if(IsOn)
		{
			//turn off
			LightSource.enabled = false;
			BulbOn.enabled = false;
			BulbOff.enabled = true;
			IsOn = false;
		}
		else
		{
			//turn on
			LightSource.enabled = true;
			BulbOn.enabled = true;
			BulbOff.enabled = false;
			IsOn = true;
		}
	}


}
