using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocEntry : MonoBehaviour 
{
	public UILabel LocName;
	public Vector3 Location;

	public void Initialize()
	{
		LocName.MakePixelPerfect();
	}

	public void OnClick()
	{
		GameManager.Inst.PlayerControl.SelectedPC.transform.position = Location;
	}

}
