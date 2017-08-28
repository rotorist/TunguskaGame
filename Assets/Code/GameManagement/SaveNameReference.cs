using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveNameReference : MonoBehaviour 
{

	public string SaveName;

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}
}
