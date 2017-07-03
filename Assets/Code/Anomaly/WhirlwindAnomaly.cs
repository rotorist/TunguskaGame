using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlwindAnomaly : MonoBehaviour
{

	void Update () 
	{
		transform.RotateAround(transform.position, transform.up, 80 * Time.deltaTime);
	}

}
