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
		GameManager.Inst.DBManager.DBHandlerEnvironment.LoadPrimaryEnvironmentSounds(Name, "Morning", "Clear", out PrimarySoundSet, out PrimaryChoices);
		GameManager.Inst.DBManager.DBHandlerEnvironment.LoadSecondaryEnvironmentSounds(Name, "Morning", "Clear", out AmbientSoundSet, out AmbientChoices);
	}

	public void LoadEnvironment()
	{
		
		_lastThreeSounds = new Queue<string>(3);

		float time = GameManager.Inst.WorldManager.CurrentTime;

		if(time >= 0 && time < 60 * 12)
		{
			SunMoonColor = Color.Lerp(new Color(1, 1, 1), new Color(1, 0.984f, 0.918f), time / (60 * 12));
		}
		else if(time >= 60 * 12 && time < 60 * 18)
		{
			SunMoonColor = Color.Lerp(new Color(1, 0.984f, 0.918f), new Color(1, 0.584f, 0.518f), (time - 60 * 12) / (60 * 6));
		}
		else if(time >= 60 * 18 && time < 60 * 20)
		{
			SunMoonColor = Color.Lerp(new Color(1, 0.584f, 0.518f), new Color(1, 1, 1), (time - 60 * 18) / (60 * 6));
		}
		else
		{
			SunMoonColor = new Color(1, 1, 1);
		}

		Light sunMoon = GameObject.Find("SunMoon").GetComponent<Light>();
		sunMoon.color = SunMoonColor;
		sunMoon.intensity = SunMoonIntensity;
		RenderSettings.ambientLight = AmbientLightColor;
		RenderSettings.ambientIntensity = AmbientIntensity;
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

}
