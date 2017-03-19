using UnityEngine;
using System.Collections;

public class BloodSpatter : MonoBehaviour 
{

	public Character Source;
	public Vector3 Offset;
	
	// Update is called once per frame
	void Update () 
	{
		if(Source != null)
		{
			transform.position = Source.transform.position + Offset;
		}
	}
}
