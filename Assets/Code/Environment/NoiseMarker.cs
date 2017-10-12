using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMarker : MonoBehaviour 
{
	public Projector Projector;

	
	// Update is called once per frame
	void Update () 
	{
		Projector.orthographicSize = Mathf.Lerp(Projector.orthographicSize, 1,  Time.deltaTime * 1f);
		if(Projector.orthographicSize > 0.6f)
		{
			Projector.orthographicSize = 0.2f;
		}
	}
}
