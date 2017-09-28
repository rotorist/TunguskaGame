using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Environment
{
	public string Name;
	public bool IsInterior;

	public List<string> AmbientSoundSet;
	public List<int> AmbientChoices;
	public List<string> PrimarySoundSet;
	public List<int> PrimaryChoices;

	public Color AmbientLightColor;
	public float AmbientIntensity;
	public Color SunMoonColor;
	public float SunMoonIntensity;
	public float ShadowIntensity;

	private Queue<string> _lastThreeSounds;

	public Environment(string name)
	{
		Name = name;
		//load sounds
		//GameManager.Inst.DBManager.DBHandlerEnvironment.LoadPrimaryEnvironmentSounds(Name, "Day", "Clear", out PrimarySoundSet, out PrimaryChoices);
		//GameManager.Inst.DBManager.DBHandlerEnvironment.LoadSecondaryEnvironmentSounds(Name, "Day", "Clear", out AmbientSoundSet, out AmbientChoices);
	}

	public void LoadEnvironment()
	{
		
		_lastThreeSounds = new Queue<string>(3);
		UpdateSounds();
		UpdateLighting();
	}

	public string GetNextAmbientSound()
	{
		string name = "";
		int iterations = 0;
		do 
		{
			if(UnityEngine.Random.value > 0.75f)
			{
				int sound = UnityEngine.Random.Range(0, AmbientSoundSet.Count);
				int choice = UnityEngine.Random.Range(1, AmbientChoices[sound] + 1);
				name = AmbientSoundSet[sound] + choice.ToString();
			}
			else
			{
				int sound = UnityEngine.Random.Range(0, PrimarySoundSet.Count);
				int choice = UnityEngine.Random.Range(1, PrimaryChoices[sound] + 1);
				name = PrimarySoundSet[sound] + choice.ToString();
			}

			iterations ++;
		}
		while (_lastThreeSounds.Contains(name) && iterations < 15);

		_lastThreeSounds.Enqueue(name);

		return name;
	}

	public void UpdateSounds()
	{
		//load sounds
		string time = "";
		if(GameManager.Inst.WorldManager.IsDayTime)
		{
			time = "Day";
		}
		else
		{
			time = "Night";
		}
		GameManager.Inst.DBManager.DBHandlerEnvironment.LoadPrimaryEnvironmentSounds(Name, time, "Clear", out PrimarySoundSet, out PrimaryChoices);
		GameManager.Inst.DBManager.DBHandlerEnvironment.LoadSecondaryEnvironmentSounds(Name, time, "Clear", out AmbientSoundSet, out AmbientChoices);
	}

	public void UpdateLighting()
	{
		if(!IsInterior)
		{

			float time = GameManager.Inst.WorldManager.CurrentTime;
			float timePercent = 1;
			if(time >= 5 && time < 60 * 8)
			{
				timePercent = (time - 60 * 5) / (60 * 3);
				SunMoonColor = Color.Lerp(new Color(1, 1, 1), new Color(1, 0.884f, 0.818f), timePercent);
				SunMoonIntensity = Mathf.Lerp(0.19f, 0.5f, timePercent); 
				AmbientLightColor = Color.Lerp(new Color(0.2f, 0.25f, 0.36f), new Color(0.44f, 0.45f, 0.47f), timePercent);
				AmbientIntensity = Mathf.Lerp(0.2f, 0.5f, timePercent);
			}
			else if(time >= 60 * 8 && time < 60 * 13)
			{
				timePercent = (time - 60 * 8) / (60 * 5);
				SunMoonColor = Color.Lerp(new Color(1, 0.884f, 0.818f), new Color(1, 0.984f, 0.918f), timePercent);
				SunMoonIntensity = Mathf.Lerp(0.5f, 0.85f, timePercent);
				AmbientLightColor = new Color(0.44f, 0.45f, 0.47f);
				AmbientIntensity = Mathf.Lerp(0.5f, 0.8f, timePercent);
			}
			else if(time >= 60 * 12 && time < 60 * 18)
			{
				timePercent = (time - 60 * 13) / (60 * 6);
				SunMoonColor = Color.Lerp(new Color(1, 0.984f, 0.918f), new Color(0.88f, 0.71f, 0.34f), timePercent);
				SunMoonIntensity = Mathf.Lerp(0.85f, 0.5f, timePercent);
				AmbientLightColor = new Color(0.44f, 0.45f, 0.47f);
				AmbientIntensity = Mathf.Lerp(0.8f, 0.5f, timePercent);
			}
			else if(time >= 60 * 18 && time < 60 * 21)
			{
				timePercent = (time - 60 * 18) / (60 * 3);
				SunMoonColor = Color.Lerp(new Color(0.88f, 0.71f, 0.34f), new Color(1, 1, 1), timePercent);
				SunMoonIntensity = Mathf.Lerp(0.5f, 0.19f, timePercent);
				AmbientLightColor = Color.Lerp(new Color(0.44f, 0.45f, 0.47f), new Color(0.2f, 0.25f, 0.36f), timePercent);
				AmbientIntensity = Mathf.Lerp(0.5f, 0.2f, timePercent);
			}
			else
			{
				SunMoonColor = new Color(1, 1, 1);
				SunMoonIntensity = 0.19f;
				AmbientLightColor = new Color(0.2f, 0.25f, 0.36f);
				AmbientIntensity = 0.2f;
			}

		}

		Light sunMoon = GameObject.Find("SunMoon").GetComponent<Light>();
		sunMoon.color = SunMoonColor;
		sunMoon.intensity = SunMoonIntensity;
		RenderSettings.ambientLight = AmbientLightColor;
		RenderSettings.ambientIntensity = AmbientIntensity;
	}

}
