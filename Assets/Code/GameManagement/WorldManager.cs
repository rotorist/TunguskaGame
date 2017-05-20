﻿using UnityEngine;
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

	private int _ambientPlayCounter;
	private int _nextAmbientTime;

	public void Initialize()
	{
		AllEnvironments = new Dictionary<string, Environment>();

		AllLevels = new List<Level>();
		CurrentLevel = new Level();
		CurrentLevel.Name = "Zernaskaya";
		AllLevels.Add(CurrentLevel);


		CurrentTerrain = GameObject.Find("Terrain").GetComponent<TerrainHandler>();
		CurrentTerrain.Initialize();



		Environment dayWild = new Environment("DayWilderness");
		dayWild.AmbientLightColor = new Color(0.424f, 0.430f, 0.444f);
		dayWild.SunMoonColor = new Color(1, 0.984f, 0.918f, 1f);
		dayWild.SunMoonIntensity = 0.7f;
		dayWild.ShadowIntensity = 1;
		AllEnvironments.Add(dayWild.Name, dayWild);

		CurrentEnvironment = dayWild;
		CurrentEnvironment.LoadEnvironment();

		Environment cellar = new Environment("Cellar");
		cellar.AmbientLightColor = new Color(0.1f, 0.1f, 0.1f);
		cellar.SunMoonColor = new Color(0, 0, 0, 0);
		cellar.SunMoonIntensity = 0;
		cellar.ShadowIntensity = 1;
		AllEnvironments.Add(cellar.Name, cellar);

		Environment buildingInterior = new Environment("BuildingInterior");
		buildingInterior.AmbientLightColor = new Color(0.2f, 0.2f, 0.2f);
		buildingInterior.SunMoonColor = new Color(0, 0, 0, 0);
		buildingInterior.SunMoonIntensity = 0;
		buildingInterior.ShadowIntensity = 1;
		AllEnvironments.Add(buildingInterior.Name, buildingInterior);


		_nextAmbientTime = UnityEngine.Random.Range(3, 6);
		_ambientPlayCounter = 0;
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
			float volume = UnityEngine.Random.Range(0.02f, 0.06f);
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

	}

	public void ChangeEnvironment(string name)
	{
		AllEnvironments[name].LoadEnvironment();
	}


}
