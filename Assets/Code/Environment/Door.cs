using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class Door : MonoBehaviour 
{
	public DoorType Type;
	public bool IsMetal;
	public bool IsOpen;
	public bool IsLocked;
	public string KeyID;
	public Transform DoorPanel;
	public Transform ClosedTarget;
	public Transform OpenTarget1;
	public Transform OpenTarget2;
	public float OperationSpeed;
	public List<HumanCharacter> HumansInTrigger;
	public AudioSource DoorSound;
	private Transform _rotateTarget;

	void Start()
	{
		DoorSound = GetComponent<AudioSource>();

	}

	// Update is called once per frame
	void Update () 
	{
		/*
		if(DoorPanel == null)
		{
			Debug.Log("updating door " + this.name);
			GameObject.Find("Sphere").transform.position = this.transform.position;
		}
		*/

		if(!IsOpen)
		{
			if(Type == DoorType.Rotating)
			{
				DoorPanel.transform.localRotation = Quaternion.Slerp(DoorPanel.transform.localRotation, ClosedTarget.localRotation, Time.deltaTime * OperationSpeed);
			}
			else if(Type == DoorType.Sliding)
			{
				DoorPanel.transform.localPosition = Vector3.MoveTowards(DoorPanel.transform.localPosition, ClosedTarget.localPosition, Time.deltaTime * OperationSpeed);
				if(DoorSound != null && Vector3.Distance(DoorPanel.transform.localPosition, ClosedTarget.localPosition) < 0.01f)
				{
					DoorSound.Stop();
				}
			}
		}
		else
		{
			if(Type == DoorType.Rotating)
			{
				DoorPanel.transform.localRotation = Quaternion.Slerp(DoorPanel.transform.localRotation, _rotateTarget.localRotation, Time.deltaTime * OperationSpeed);
			}
			else if(Type == DoorType.Sliding)
			{
				DoorPanel.transform.localPosition = Vector3.MoveTowards(DoorPanel.transform.localPosition, OpenTarget1.localPosition, Time.deltaTime * OperationSpeed);
				if(DoorSound != null && Vector3.Distance(DoorPanel.transform.localPosition, OpenTarget1.localPosition) < 0.01f)
				{
					DoorSound.Stop();
				}
			}
		}

	}

	void OnTriggerEnter(Collider other)
	{
		//Debug.Log("collision! " + other.name);
		HumanCharacter human = other.GetComponent<HumanCharacter>();
		if(human != null)
		{
			human.CurrentDoor = this;
			//Debug.Log("assigning current door for " + human.name);
		}
	}

	void OnTriggerExit(Collider other)
	{
		HumanCharacter human = other.GetComponent<HumanCharacter>();
		if(human != null)
		{
			human.CurrentDoor = null;
		}
	}

	public void Open(Transform opener)
	{
		if(IsOpen)
		{
			return;
		}

		//check if it's locked
		if(IsLocked)
		{
			//here check if player has key
			//

			//play locked door sound
			if(!IsMetal)
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

		IsOpen = true;
		if(DoorSound != null)
		{
			if(Type == DoorType.Sliding)
			{
				DoorSound.Play();
			}
			else if(Type == DoorType.Rotating)
			{
				if(!IsMetal)
				{
					AudioClip clip = GameManager.Inst.SoundManager.GetClip("WoodDoorOpen");
					DoorSound.PlayOneShot(clip, 0.8f);
				}
				else 
				{
					AudioClip clip = GameManager.Inst.SoundManager.GetClip("MetalDoorOpen");
					DoorSound.PlayOneShot(clip, 0.6f);
				}
			}
		}

		if(Type == DoorType.Rotating)
		{
			//calculate the angle between opener-door and door-up
			Vector3 openerDoorLine = opener.position - transform.position;
			float angle = Vector3.Angle(openerDoorLine, transform.up);
			if(angle < 90)
			{
				_rotateTarget = OpenTarget1;
			}
			else
			{
				_rotateTarget = OpenTarget2;
			}

			//also set navmesh obstacle to carve
			NavMeshObstacle obstacle = DoorPanel.GetComponent<NavMeshObstacle>();
			obstacle.carving = true;
		}
	}

	public void Close()
	{
		if(!IsOpen)
		{
			return;
		}

		IsOpen = false;
		if(DoorSound != null)
		{
			if(Type == DoorType.Sliding)
			{
				DoorSound.Play();
			}
			else if(Type == DoorType.Rotating)
			{
				if(!IsMetal)
				{
					AudioClip clip = GameManager.Inst.SoundManager.GetClip("WoodDoorClose");
					DoorSound.PlayOneShot(clip, 0.8f);
				}
				else 
				{
					AudioClip clip = GameManager.Inst.SoundManager.GetClip("MetalDoorClose");
					DoorSound.PlayOneShot(clip, 0.6f);
				}

				//also set navmesh obstacle to non-carve
				NavMeshObstacle obstacle = DoorPanel.GetComponent<NavMeshObstacle>();
				obstacle.carving = false;
			}
		}
	}
}
