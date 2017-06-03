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

public class StoryEventToggleComponent : StoryEvent
{
	public bool IsOn;
	public string TargetName;
	public GameObject Target;

	public void Initialize()
	{
		GameObject target = GameObject.Find(TargetName);
		Target = target;
		if(target == null)
		{
			return;
		}
		target.SetActive(false);
	}

	public override void Trigger ()
	{
		if(Target.activeSelf)
		{
			Target.SetActive(false);
			IsOn = false;
		}
		else
		{
			Target.SetActive(true);
			IsOn = true;
		}
	}
}