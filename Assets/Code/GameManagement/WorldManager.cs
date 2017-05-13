using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManager 
{
	public string CurrentLevelName { get {return CurrentLevel.Name;}}
	public Level CurrentLevel;
	public List<Level> AllLevels;
	public TerrainHandler CurrentTerrain;
	public Environment CurrentEnvironment;

	private int _ambientPlayCounter;
	private int _nextAmbientTime;

	public void Initialize()
	{
		AllLevels = new List<Level>();
		CurrentLevel = new Level();
		CurrentLevel.Name = "Zernaskaya";
		AllLevels.Add(CurrentLevel);


		CurrentTerrain = GameObject.Find("Terrain").GetComponent<TerrainHandler>();
		CurrentTerrain.Initialize();


		CurrentEnvironment = new Environment();
		CurrentEnvironment.LoadEnvironment();
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



}
