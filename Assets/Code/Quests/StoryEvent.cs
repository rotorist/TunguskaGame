using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StoryEvent
{
	public string ID;

	public abstract void Trigger();

}

public class StoryEventDoor : StoryEvent
{
	public bool IsOpen;
	public string TargetDoorName;

	public override void Trigger()
	{
		GameObject dO = GameObject.Find(TargetDoorName);
		if(dO == null)
			return;
		Door door = dO.GetComponent<Door>();
		if(door == null)
			return;
		if(IsOpen)
		{
			door.Open(GameManager.Inst.PlayerControl.SelectedPC.transform);
		}
		else
		{
			door.Close();
		}
	}

}