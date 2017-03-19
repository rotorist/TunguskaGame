using UnityEngine;
using System.Collections;

public class LeftHandIKControl : MonoBehaviour 
{
	public Transform Target;
	public bool Enabled;
	protected Animator MyAnimator;

	private float _ikWeight;
	private int _ikWeightState; //0=none, -1=decreasing, 1=increasing
	private float _ikWeightRate;

	public float IKWeight;

	public void Initialize()
	{
		MyAnimator = GetComponent<Animator>();

		Enabled = true;
	}

	void Update()
	{
		// Clamping weights
		if(_ikWeightState == -1)
		{
			//_ikWeight = Mathf.Lerp(_ikWeight, 0, _ikWeightRate * Time.deltaTime);
			_ikWeight -= _ikWeightRate * Time.deltaTime;
			if(_ikWeight <= 0.1f)
			{
				_ikWeight = 0;
			}
		}
		else if(_ikWeightState == 1)
		{
			//_ikWeight = Mathf.Lerp(_ikWeight, 1, _ikWeightRate * Time.deltaTime);
			_ikWeight += _ikWeightRate * (_ikWeight + 0.05f) * Time.deltaTime;
			if(_ikWeight >= 0.9f)
			{
				_ikWeight = 1;
			}
		}
		_ikWeight = Mathf.Clamp(_ikWeight, 0f, 1f);

		IKWeight = _ikWeight;
	}

	void OnAnimatorIK()
	{
		if(MyAnimator) 
		{
			
			//if the IK is active, set the position and rotation directly to the goal. 
			if(Enabled) 
			{
				if(Target != null)
				{
					MyAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _ikWeight);
					MyAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _ikWeight);  
					MyAnimator.SetIKPosition(AvatarIKGoal.LeftHand, Target.position);
					MyAnimator.SetIKRotation(AvatarIKGoal.LeftHand, Target.rotation);
				}
			}
		}
	}

	public bool IsEnabled()
	{
		if(_ikWeightState == 1)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void InstantDisable()
	{
		_ikWeight = 0;
		_ikWeightState = -1;
	}

	public void SmoothEnable(float rate)
	{
		_ikWeightRate = rate;
		_ikWeightState = 1;
	}
		

	public void SmoothEnable()
	{
		_ikWeightRate = 1f;
		_ikWeightState = 1;
	}

	public void SmoothDisable(float rate)
	{
		_ikWeightRate = rate;
		_ikWeightState = -1;
	}

	public void SmoothDisable()
	{
		_ikWeightRate = 3;
		_ikWeightState = -1;
	}

}
