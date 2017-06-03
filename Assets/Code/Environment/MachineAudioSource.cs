using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineAudioSource : MonoBehaviour 
{
	
	public AudioClip RunningSound;
	public AudioSource Source;
	public float TargetVolume;
	public bool InitialState;
	public bool IsOn;



	void Start()
	{
		if(InitialState)
		{
			TurnOn();
		}
		else
		{
			TurnOff();
		}

	}

	void Update()
	{
		if(IsOn && Source.volume < TargetVolume)
		{
			Source.volume += Time.deltaTime * 1;
		}
		else if(!IsOn && Source.volume > 0)
		{
			Source.volume -= Time.deltaTime * 2;
		}
	}

	public void TurnOn()
	{
		Source.clip = RunningSound;
		IsOn = true;
		Source.Play();
	}

	public void TurnOff()
	{
		IsOn = false;
	}

}
