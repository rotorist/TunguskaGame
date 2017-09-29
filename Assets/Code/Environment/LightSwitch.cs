using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour 
{
	public Lamp [] MyLamps;

	public void Toggle()
	{
		if(Vector3.Distance(GameManager.Inst.PlayerControl.SelectedPC.transform.position, transform.position) < 5)
		{
			AudioClip switchSound = GameManager.Inst.SoundManager.GetClip("LightSwitch");
			GameManager.Inst.SoundManager.UI.PlayOneShot(switchSound, 0.2f);
		}
		foreach(Lamp lamp in MyLamps)
		{
			lamp.Toggle();
		}
	}

}
