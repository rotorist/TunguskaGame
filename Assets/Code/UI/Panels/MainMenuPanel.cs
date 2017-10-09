using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MainMenuPanel : MonoBehaviour 
{
	public UIButton ContinueButton;
	public UIButton NewGameButton;
	public UIButton OptionsButton;
	public UIButton ExitButton;


	// Use this for initialization
	void Start () 
	{
		string fullPath = Application.persistentDataPath + "/TestSave.dat";
		FileStream file;
		if(File.Exists(fullPath))
		{
			ContinueButton.isEnabled = true;
		}
		else
		{
			ContinueButton.isEnabled = false;
		}

		//OptionsButton.isEnabled = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void OnContinueGame()
	{
		SaveNameReference saveNameRef = GameObject.FindObjectOfType<SaveNameReference>();
		saveNameRef.SaveName = "TestSave";
		SaveGameManager saveGameManager = new SaveGameManager();
		string levelName = saveGameManager.LoadLevelName("TestSave");
		SceneManager.LoadScene(levelName);
	}

	public void OnNewGame()
	{
		SaveNameReference saveNameRef = GameObject.FindObjectOfType<SaveNameReference>();
		saveNameRef.SaveName = "";
		saveNameRef.IsNewGame = true;

		SceneManager.LoadScene("Zernaskaya");
	}

	public void OnNewGameGod()
	{
		SaveNameReference saveNameRef = GameObject.FindObjectOfType<SaveNameReference>();
		saveNameRef.IsGodMode = true;
		saveNameRef.SaveName = "";
		saveNameRef.IsNewGame = true;

		SceneManager.LoadScene("Zernaskaya");
	}

	public void OnExitGame()
	{
		Application.Quit();
	}
}
