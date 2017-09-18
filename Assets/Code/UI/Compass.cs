using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour 
{
	public Transform CompassFace;
	public Transform CameraController;
	public Transform NorthReference;

	//private float _currentAngle;
	
	public void Initialize()
	{
		CameraController = GameObject.Find("CameraController").transform;
		NorthReference = GameObject.Find("NorthReference").transform;
	}

	public void PerFrameUpdate () 
	{
		if(CameraController != null && NorthReference != null)
		{
			float angle = Vector3.Angle(CameraController.forward, NorthReference.forward);
			float dotProduct = Vector3.Dot(CameraController.right, NorthReference.forward);
			float modifier = 1;
			if(dotProduct > 0)
			{
				modifier = -1;
			}
			//_currentAngle = Mathf.Lerp(_currentAngle, angle * modifier , Time.deltaTime * 6);
			//+ Mathf.Sin(Mathf.Deg2Rad * (angle * modifier)) * 20
			CompassFace.localEulerAngles = new Vector3(0, 0, angle * modifier);
		}
	}
}
