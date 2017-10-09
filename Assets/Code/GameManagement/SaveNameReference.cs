using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveNameReference : MonoBehaviour 
{

	public string SaveName;
	public bool IsNewGame;
	public bool IsGodMode;

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}
}
