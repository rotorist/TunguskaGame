using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Environment
{
	public string Name;

	public string [] AmbientSoundSet;
	public int [] AmbientChoices;
	public string [] PrimarySoundSet;
	public int [] PrimaryChoices;

	public Color AmbientLightColor;
	public Color SunMoonColor;
	public float SunMoonIntensity;
	public float ShadowIntensity;

	private Queue<string> _lastThreeSounds;

	public Environment(string name)
	{
		Name = name;
	}

	public void LoadEnvironment()
	{
		AmbientSoundSet = new string[]{"animal", "chirp", "horror", "gun", "growl", "drone"};
		AmbientChoices = new int[]    {4,         8,       2,        8,    7,        6      };
		PrimarySoundSet = new string[]{"wind", "blown_leaf", "crow"};
		PrimaryChoices = new int[]    {4,       5,            4    };
		_lastThreeSounds = new Queue<string>(3);

		Light sunMoon = GameObject.Find("SunMoon").GetComponent<Light>();
		sunMoon.color = SunMoonColor;
		sunMoon.intensity = SunMoonIntensity;
		RenderSettings.ambientLight = AmbientLightColor;

	}

	public string GetNextAmbientSound()
	{
		string name = "";
		int iterations = 0;
		do 
		{
			if(UnityEngine.Random.value > 0.75f)
			{
				int sound = UnityEngine.Random.Range(0, AmbientSoundSet.Length);
				int choice = UnityEngine.Random.Range(1, AmbientChoices[sound] + 1);
				name = AmbientSoundSet[sound] + choice.ToString();
			}
			else
			{
				int sound = UnityEngine.Random.Range(0, PrimarySoundSet.Length);
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
