using UnityEngine;
using System.Collections;

public class HeadIKControl : MonoBehaviour 
{

	public Transform LookTarget; 
	public bool Enabled;
	public Animator MyAnimator;
	public float Weight;

	private float _IKPositionWeight;

	private int _ikWeightState; //0=none, -1=decreasing, 1=increasing
	private float _ikWeightRate;

	public void Initialize()
	{
		MyAnimator = GetComponent<Animator>();
		Weight = 1;
		_IKPositionWeight = 1;
		_ikWeightState = 1;

		Enabled = true;
	}
	
	void OnAnimatorIK()
	{
		if(MyAnimator) 
		{
			
			//if the IK is active, set the position and rotation directly to the goal. 
			if(Enabled) 
			{
				//set look target
				if(LookTarget != null)
				{
					MyAnimator.SetLookAtWeight(_IKPositionWeight * Weight);
					MyAnimator.SetLookAtPosition(LookTarget.position);
				}
			}
		}
	}

	void Update()
	{
		if (_IKPositionWeight < 0) return;

		// Clamping weights
		if(_ikWeightState == -1)
		{
			_IKPositionWeight = Mathf.Lerp(_IKPositionWeight, 0, _ikWeightRate * Time.deltaTime);
			if(_IKPositionWeight < 0.01f)
			{
				//IKPositionWeight = 0;
			}
		}
		else if(_ikWeightState == 1)
		{
			_IKPositionWeight = Mathf.Lerp(_IKPositionWeight, 1, _ikWeightRate * Time.deltaTime);
		}

		_IKPositionWeight = Mathf.Clamp(_IKPositionWeight, 0f, 1f);

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

	public void InstantDisable()
	{
		_IKPositionWeight = 0;
		_ikWeightState = -1;
	}

	public void InstantEnable()
	{
		_IKPositionWeight = 1;
		_ikWeightState = 1;
	}
}
