using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Environment
{
	public string [] AmbientSoundSet;
	public int [] AmbientChoices;
	public string [] PrimarySoundSet;
	public int [] PrimaryChoices;

	private Queue<string> _lastThreeSounds;

	public void LoadEnvironment()
	{
		AmbientSoundSet = new string[]{"animal", "chirp", "horror", "gun", "growl", "drone"};
		AmbientChoices = new int[]    {4,         8,       2,        8,    7,        6      };
		PrimarySoundSet = new string[]{"wind", "blown_leaf", "crow"};
		PrimaryChoices = new int[]    {4,       5,            4    };
		_lastThreeSounds = new Queue<string>(3);
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
