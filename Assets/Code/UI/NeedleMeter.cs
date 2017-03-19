using UnityEngine;
using System.Collections;

public class NeedleMeter : MonoBehaviour 
{
	protected float Value;
	protected float SecondaryValue;
	public Transform Needle;
	public Transform SecondaryNeedle;
	public float AngleLow;
	public float AngleHigh;
	public float AngleLowSec;
	public float AngleHighSec;


	public void Initialize(float originalValue)
	{
		SetValue(originalValue);
	}

	public void SetValue(float value)
	{
		Needle.transform.eulerAngles = new Vector3(0, 0, (AngleHigh - AngleLow) * value + AngleLow);
		Value = value;
	}

	public void SetSecondaryValue(float value)
	{
		if(SecondaryNeedle != null)
		{
			SecondaryNeedle.transform.eulerAngles = new Vector3(0, 0, (AngleHighSec - AngleLowSec) * value + AngleLowSec);
			SecondaryValue = value;
		}
	}

	public float GetValue()
	{
		return Value;
	}

	public float GetSecondaryValue()
	{
		return SecondaryValue;
	}
}
