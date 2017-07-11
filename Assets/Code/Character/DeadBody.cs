using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour 
{
	public int PoseNumber;

	void Start()
	{
		Animator myAnimator = GetComponent<Animator>();
		myAnimator.SetInteger("PoseNumber", PoseNumber);
	}
}
