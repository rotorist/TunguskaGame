using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour 
{
	public Vector3 OffPos;
	public Vector3 OffRot;
	public Vector3 OnPos;
	public Vector3 OnRot;

	public bool IsOn;
	public bool IsReady;//when IsReady is true, player can trigger it
	public string NotReadyMessage;//When IsReady is false and player tries to trigger, display this message
	public InteractiveObject NextObject;
	public string OnEvent;
	public string OffEvent;
	public string RequireItemID;
	public string RequireItemQuantity;
	public bool IsConsumingItem;


	
	// Update is called once per frame
	void Update () 
	{
		if(IsOn)
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, OnPos, Time.deltaTime * 6);
			transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, OnRot, Time.deltaTime * 6);
		}
		else
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, OnPos, Time.deltaTime * 6);
			transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, OnRot, Time.deltaTime * 6);
		}
	}

	public void Interact()
	{

	}
}
