using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingEntrance : MonoBehaviour 
{
	public BuildingComponent ParentComponent;
	public bool IsActive;

	void OnTriggerEnter(Collider other)
	{
		//Debug.Log("collision! " + other.name);
		HumanCharacter human = other.GetComponent<HumanCharacter>();
		if(human != null && human.MyAI.ControlType == AIControlType.Player)
		{
			GameManager.Inst.PlayerControl.CurrentEntrance = this;
		}
	}

	void OnTriggerExit(Collider other)
	{
		HumanCharacter human = other.GetComponent<HumanCharacter>();
		if(human != null && human.MyAI.ControlType == AIControlType.Player)
		{
			GameManager.Inst.PlayerControl.CurrentEntrance = null;
		}
	}
}
