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
	public ContainerSoundType SoundType;
	public bool IsLocked;
	public string KeyItemID;

	void Start()
	{
		DoorSound = GetComponent<AudioSource>();

	}



	public void Enter(HumanCharacter character)
	{
		if(IsLocked)
		{
			//here check if player has key
			int keyCount = GameManager.Inst.PlayerControl.SelectedPC.Inventory.CountItemsInBackpack(KeyItemID);
			if(keyCount > 0)
			{
				//play unlock key sound
				IsLocked = false;
			}
			else
			{
				//play locked door sound
				if(SoundType == ContainerSoundType.Wood)
				{
					AudioClip clip = GameManager.Inst.SoundManager.GetClip("WoodDoorLocked");
					DoorSound.PlayOneShot(clip, 0.6f);
				}
				else
				{
					AudioClip clip = GameManager.Inst.SoundManager.GetClip("MetalDoorLocked");
					DoorSound.PlayOneShot(clip, 0.6f);
				}

				return;
			}
		}

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
		GameManager.Inst.SoundManager.SetMusic(OtherPortal.Environment, GameManager.Inst.WorldManager.IsDayTime);
		Time.timeScale = 1;
	}
}
