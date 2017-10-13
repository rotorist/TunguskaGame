using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMarker : MonoBehaviour 
{
	public Projector Projector;
	private float _size;
	
	// Update is called once per frame
	void Update () 
	{
		_size = Mathf.Lerp(_size, 1,  Time.deltaTime * 1.2f);
		if(_size > 0.4f)
		{
			Projector.enabled = false;
			_size = -0.1f;
		}

		if(_size > 0.1f)
		{
			Projector.enabled = true;
			Projector.orthographicSize = _size;
		}
	}
}
