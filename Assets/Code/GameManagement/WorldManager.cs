using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManager 
{
	public string CurrentLevelName { get {return CurrentLevel.Name;}}
	public Level CurrentLevel;
	public List<Level> AllLevels;
	public Dictionary<string, Environment> AllEnvironments;
	public TerrainHandler CurrentTerrain;
	public Environment CurrentEnvironment;
	public int CurrentDay;
	public float CurrentTime;

	private int _ambientPlayCounter;
	private int _nextAmbientTime;
	private int _doorIDIndex;

	public void Initialize()
	{
		AllEnvironments = new Dictionary<string, Environment>();

		AllLevels = new List<Level>();
		CurrentLevel = new Level();
		CurrentLevel.Name = "Zernaskaya";
		AllLevels.Add(CurrentLevel);


		CurrentTerrain = GameObject.Find("Terrain").GetComponent<TerrainHandler>();
		CurrentTerrain.Initialize();

		CurrentTime = 240;

		Environment dayWild = new Environment("DayWilderness");
		dayWild.IsInterior = false;
		dayWild.AmbientSoundSet = new string[]{"animal", "chirp", "horror", "gun", "growl", "drone"};
		dayWild.AmbientChoices = new int[]    {4,         8,       2,        8,    7,        6      };
		dayWild.PrimarySoundSet = new string[]{"wind", "blown_leaf", "crow"};
		dayWild.PrimaryChoices = new int[]    {4,       5,            4    };
		dayWild.AmbientLightColor = new Color(0.424f, 0.430f, 0.444f);
		dayWild.AmbientIntensity = 0.7f;
		dayWild.SunMoonColor = new Color(1, 0.984f, 0.918f, 1f);
		dayWild.SunMoonIntensity = 0.7f;
		dayWild.ShadowIntensity = 1;
		AllEnvironments.Add(dayWild.Name, dayWild);

		CurrentEnvironment = dayWild;
		CurrentEnvironment.LoadEnvironment();

		Environment cellar = new Environment("Cellar");
		cellar.IsInterior = true;
		cellar.AmbientSoundSet = new string[]{"dripping", "horror"};
		cellar.AmbientChoices = new int[]    {6,         	2};
		cellar.PrimarySoundSet = new string[]{"dripping", "horror"};
		cellar.PrimaryChoices = new int[]    {6,         	2};
		cellar.AmbientLightColor = new Color(0.1f, 0.1f, 0.1f);
		cellar.AmbientIntensity = 0.1f;
		cellar.SunMoonColor = new Color(0, 0, 0, 0);
		cellar.SunMoonIntensity = 0;
		cellar.ShadowIntensity = 1;
		AllEnvironments.Add(cellar.Name, cellar);

		Environment buildingInterior = new Environment("BuildingInterior");
		buildingInterior.IsInterior = true;
		buildingInterior.AmbientSoundSet = new string[]{"wood_creak", "interior_horror"};
		buildingInterior.AmbientChoices = new int[]    {4,         	1};
		buildingInterior.PrimarySoundSet = new string[]{"wood_creak", "interior_horror"};
		buildingInterior.PrimaryChoices = new int[]    {4,         	1};
		buildingInterior.AmbientLightColor = new Color(0.3f, 0.3f, 0.3f);
		buildingInterior.AmbientIntensity = 0.3f;
		buildingInterior.SunMoonColor = new Color(0, 0, 0, 0);
		buildingInterior.SunMoonIntensity = 0;
		buildingInterior.ShadowIntensity = 1;
		AllEnvironments.Add(buildingInterior.Name, buildingInterior);


		_nextAmbientTime = UnityEngine.Random.Range(3, 6);
		_ambientPlayCounter = 0;

		//assign ID to all doors
		GameObject [] doors = GameObject.FindGameObjectsWithTag("Door");
		foreach(GameObject o in doors)
		{
			Door door = o.GetComponent<Door>();
			if(door != null)
			{
				door.ID = CurrentLevelName + "Door" + _doorIDIndex;
				_doorIDIndex ++;
			}
		}
	}

	public void PerSecondUpdate()
	{
		if(_ambientPlayCounter >= _nextAmbientTime)
		{
			AudioSource selectedSource = GameManager.Inst.SoundManager.Ambient1;
			if(!GameManager.Inst.SoundManager.Ambient1.isPlaying)
			{
				GameManager.Inst.SoundManager.Ambient1.panStereo = UnityEngine.Random.Range(-1f, 1f);
			}
			else if(!GameManager.Inst.SoundManager.Ambient2.isPlaying)
			{
				GameManager.Inst.SoundManager.Ambient2.panStereo = UnityEngine.Random.Range(-1f, 1f);
				selectedSource = GameManager.Inst.SoundManager.Ambient2;
			}
			float volume = UnityEngine.Random.Range(0.04f, 0.09f);
			string nextAmbientSound = CurrentEnvironment.GetNextAmbientSound();
			//Debug.Log("Now playing " + nextAmbientSound);
			selectedSource.PlayOneShot(GameManager.Inst.SoundManager.GetClip(nextAmbientSound), volume);
			_nextAmbientTime = UnityEngine.Random.Range(1, 4);
			_ambientPlayCounter = 0;
		}
		else
		{
			_ambientPlayCounter ++;
		}

		//update time
		CurrentTime ++;
		//convert to hour:minute
		int timeInt = Mathf.FloorToInt(CurrentTime);
		int hour = timeInt / 60;
		int minute = timeInt % 60;
		GameManager.Inst.UIManager.HUDPanel.Clock.text = "Day " + (CurrentDay+1) + "   " + (hour < 10 ? "0" : "") + hour.ToString() + " : " + (minute < 10 ? "0" : "") + minute.ToString();
		if(CurrentTime > 1440)
		{
			CurrentTime = 0;
			CurrentDay ++;
			TimerEventHandler.Instance.TriggerOneDayTimer();
		}

	}

	public void ChangeEnvironment(string name)
	{
		AllEnvironments[name].LoadEnvironment();
		CurrentEnvironment = AllEnvironments[name];
	}


}
