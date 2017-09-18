using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarMeter : MonoBehaviour 
{
	public UISprite Bar;
	public float BarMaxWidth;
	protected float Value;
	private float _flashSpeed;
	private int _alphaDirection;

	public void Initialize(float originalValue)
	{
		SetValue(originalValue);
		_flashSpeed = 0;
		_alphaDirection = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(_alphaDirection == 1)
		{
			Bar.alpha += Time.deltaTime * _flashSpeed;
			if(Bar.alpha > 1)
			{
				Bar.alpha = 1;
				_alphaDirection = -1;
			}
		}
		else if(_alphaDirection == -1)
		{
			Bar.alpha -= Time.deltaTime * _flashSpeed;
			if(Bar.alpha < 0)
			{
				Bar.alpha = 0;
				_alphaDirection = 1;
			}
		}

	}


	public void SetValue(float value)
	{
		Bar.width = Mathf.FloorToInt(BarMaxWidth * value);
		if(value <= 0)
		{
			Bar.alpha = 0;
		}
		else if(_alphaDirection == 0)
		{
			Bar.alpha = 1;
		}

		Value = value;
	}

	public float GetValue()
	{
		return Value;
	}

	public bool IsFlashing()
	{
		return _alphaDirection != 0;
	}

	public void StartFlashing(float speed)
	{
		_flashSpeed = speed;
		_alphaDirection = -1;
	}

	public void StopFlash()
	{
		_flashSpeed = 0;
		_alphaDirection = 0;
	}
}
