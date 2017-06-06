using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour 
{
	public bool IsOn;
	public GameObject Object;

	// Use this for initialization
	void Start () 
	{
		Object.SetActive(IsOn);
	}
	
	public void Toggle()
	{
		if(Object.activeSelf)
		{
			Object.SetActive(false);
			IsOn = false;
		}
		else
		{
			Object.SetActive(true);
			IsOn = true;
		}

	}

	public void Toggle(bool isOn)
	{
		if(isOn)
		{
			Object.SetActive(true);
			IsOn = true;
		}
		else
		{
			Object.SetActive(false);
			IsOn = false;
		}
	}


}
