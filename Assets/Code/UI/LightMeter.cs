using UnityEngine;
using System.Collections;

public class LightMeter : MonoBehaviour 
{
	public UISprite [] Lights;
	protected float Value;

	private float _flashTimer;
	private float _flashDuration;
	private bool _isLastLightOn;


	public void Initialize(float originalValue)
	{
		SetValue(originalValue);
		_flashTimer = 0;
		_flashDuration = 0;
	}

	public void SetValue(float value)
	{
		int numberOfLights = Mathf.CeilToInt(value * Lights.Length);
		for(int i = 0; i < Lights.Length; i++)
		{
			if(i == numberOfLights - 1 && _flashDuration > 0)
			{
				continue;
			}

			if(i < numberOfLights)
			{
				Lights[i].alpha = 1;
			}
			else
			{
				Lights[i].alpha = 0;
			}
		}

		Value = value;
	}

	public void StartFlashingLastLight(float duration)
	{
		_flashDuration = duration;
	}

	public void StopFlashingLastLight()
	{
		_flashDuration = 0;
	}

	void Update()
	{
		if(_flashDuration > 0 && Value > 0)
		{
			//handle flashing

			UISprite currentLight = Lights[Mathf.CeilToInt(Value * Lights.Length) - 1];
			//Debug.Log("Current light " + currentLight.name + " " + currentLight.alpha + " duration " + _flashDuration);

			if(_flashTimer < _flashDuration)
			{
				_flashTimer += Time.unscaledDeltaTime;
			}
			else
			{
				if(_isLastLightOn)
				{
					//turn off
					currentLight.alpha = 0;
					_isLastLightOn = false;
				}
				else
				{
					//turn on
					currentLight.alpha = 1;
					_isLastLightOn = true;
				}

				_flashTimer = 0;
			}
		}
	}
}
