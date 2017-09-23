using UnityEngine;
using System.Collections;

public class CharacterStealth
{
	public float NoiseLevel;//0 to infinity
	public float NoiseThreat;//0 to 1
	public float Visibility;//0 to infinity
	public float AlmostDetected; //0 to 1

	private float _noiseLingerTimer;
	private Character _parentCharacter;
	private float _baseVisibility;
	private float _weaponVisiBoost;
	private float _detectedVisiBoost;

	public CharacterStealth(Character parent)
	{
		_parentCharacter = parent;
	}

	public void UpdatePerSchedulerFrame()
	{
		
		UpdateBaseVisibility();
	}

	public void UpdatePerFrame()
	{


		UpdateNoiseLevel();
		UpdateVisibilityBoost();

		if(AlmostDetected > 0)
		{
			AlmostDetected -= Time.deltaTime * 3;
		}
		else
		{
			AlmostDetected = 0;
		}


	}

	public void SetNoiseLevel(float noise, float threat)
	{
		if(noise > NoiseLevel)
		{
			NoiseLevel = noise;
			_noiseLingerTimer = 1.1f;
		}

		if(threat > NoiseThreat)
		{
			NoiseThreat = threat;
		}
	}

	public void SetWeaponVisibilityBoost(float boost)
	{
		_weaponVisiBoost = boost;
	}

	public void SetDetectedVisibilityBoost(float boost)
	{
		_detectedVisiBoost = boost;
	}


	public void OnDeath()
	{
		NoiseLevel = 0;
		NoiseThreat = 0;
		AlmostDetected = 0;
		_noiseLingerTimer = 0;

	}







	private void UpdateNoiseLevel()
	{
		_noiseLingerTimer -= Time.deltaTime;

		if(_noiseLingerTimer <= 0)
		{
			NoiseLevel = 0;
		}


	}

	private void UpdateBaseVisibility()
	{
		
		
		Vector3 litPos = _parentCharacter.MyReference.TorsoWeaponMount.transform.position;

		Light sunMoon = GameObject.Find("SunMoon").GetComponent<Light>();
		Vector3 sunDir = sunMoon.transform.forward * -1;

		float ambient = RenderSettings.ambientIntensity;

		//set minimum visibility according to ambient level
		_baseVisibility = 5 + (1 - (1-ambient)/0.8f) * 25;



		//check if under sun or moon
		if(sunMoon.intensity > 0)
		{
			if(!Physics.Raycast(litPos, sunDir))
			{
				_baseVisibility = _baseVisibility + 10 + (1 - (1-sunMoon.intensity)/0.85f) * 10;
			}
		}


		//check if under any light
		GameObject [] lights = GameObject.FindGameObjectsWithTag("Light");
		foreach(GameObject l in lights)
		{
			Light light = l.GetComponent<Light>();
			Vector3 lightDir = l.transform.position - _parentCharacter.transform.position;
			if(lightDir.magnitude > light.range || light.enabled == false)
			{
				continue;
			}


			if(light.type == LightType.Point)
			{
				RaycastHit hit;
				bool isHit = Physics.Raycast(litPos, lightDir, out hit, lightDir.magnitude);
				if(!isHit || hit.collider.gameObject == l)
				{
					_baseVisibility = 25;
				}
			}
			else if(light.type == LightType.Spot)
			{
				//check angle
				if(Vector3.Angle(lightDir * -1, l.transform.forward) < light.spotAngle/2)
				{
					RaycastHit hit;
					bool isHit = Physics.Raycast(litPos, lightDir, out hit, lightDir.magnitude);
					if(!isHit || hit.collider.gameObject == l)
					{
						_baseVisibility = 50;
					}
				}
			}
		}

		if(_parentCharacter.CurrentStance == HumanStances.Crouch || _parentCharacter.CurrentStance == HumanStances.CrouchRun)
		{
			_baseVisibility = _baseVisibility * 0.5f;
		}

		//check if flashlight is on
		if(_parentCharacter.MyReference.Flashlight != null && _parentCharacter.MyReference.Flashlight.IsOn)
		{
			_baseVisibility = 50;
		}



	}

	private void UpdateVisibilityBoost()
	{
		

		if(_weaponVisiBoost > 0)
		{
			_weaponVisiBoost -= Time.deltaTime * 5;
		}

		if(_detectedVisiBoost > 0)
		{
			_detectedVisiBoost -= Time.deltaTime;
		}

		Visibility = _baseVisibility + _weaponVisiBoost + _detectedVisiBoost;

	}

}
