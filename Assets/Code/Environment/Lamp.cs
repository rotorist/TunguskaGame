using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour 
{
	public Light LightSource;
	public MeshRenderer BulbOn;
	public MeshRenderer BulbOff;
	public bool IsOn { get { return _isOn; } }
	private bool _isOn;

	public void Toggle()
	{
		if(_isOn)
		{
			//turn off
			LightSource.enabled = false;
			BulbOn.enabled = false;
			BulbOff.enabled = true;
			_isOn = false;
		}
		else
		{
			//turn on
			LightSource.enabled = true;
			BulbOn.enabled = true;
			BulbOff.enabled = false;
			_isOn = true;
		}
	}


}
