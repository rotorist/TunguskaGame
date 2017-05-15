using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour 
{
	public DoorType Type;
	public bool IsOpen;
	public Transform DoorPanel;
	public Transform ClosedTarget;
	public Transform OpenTarget1;
	public Transform OpenTarget2;
	public float OperationSpeed;
	public List<HumanCharacter> HumansInTrigger;

	private Transform _rotateTarget;
	
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
			}
		}

	}

	void OnTriggerEnter(Collider other)
	{
		HumanCharacter human = other.GetComponent<HumanCharacter>();
		if(human != null)
		{
			human.CurrentDoor = this;
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
		IsOpen = true;
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
		}
	}

	public void Close()
	{
		IsOpen = false;
	}
}
