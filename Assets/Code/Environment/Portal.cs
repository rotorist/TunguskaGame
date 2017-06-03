using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour 
{
	public Transform SpawnPoint;
	public Portal OtherPortal;
	public bool IsInterior;
	public string Environment;
	public AudioSource DoorSound;


	void Start()
	{
		DoorSound = GetComponent<AudioSource>();

	}



	public void Enter(HumanCharacter character)
	{
		//adjust environment for player
		if(character.MyAI.ControlType == AIControlType.Player)
		{
			Time.timeScale = 0;
			GameManager.Inst.UIManager.FadingPanel.FadeOutAndInCallBack(1, 1, 1, FadeOutCallBack);
			character.SendCommand(CharacterCommands.Idle);
		}
		else
		{
			character.transform.position = OtherPortal.SpawnPoint.position;
			character.Destination = character.transform.position;
			character.SendCommand(CharacterCommands.Idle);
		}

		DoorSound.Play();
	}

	public void FadeOutCallBack()
	{
		HumanCharacter character = GameManager.Inst.PlayerControl.SelectedPC;
		character.transform.position = OtherPortal.SpawnPoint.position;
		character.Destination = character.transform.position;
		GameManager.Inst.CameraController.ResetCamera();
		GameManager.Inst.WorldManager.ChangeEnvironment(OtherPortal.Environment);
		Time.timeScale = 1;
	}
}
