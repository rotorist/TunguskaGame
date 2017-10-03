using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class MapPanel : PanelBase
{
	public UIButton Close;
	public UISprite PencilCircle;
	public UISprite Map;

	private float _mapWorldRatio;

	public override void Initialize ()
	{
		_mapWorldRatio = GetMapWorldRatio();

		Hide();

	}

	public override void PerFrameUpdate ()
	{

	}

	public override void Show ()
	{
		Camera.main.GetComponent<BlurOptimized>().enabled = true;

		Time.timeScale = 0;

		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;

		RearrangePencilCircle();

		InputEventHandler.Instance.State = UserInputState.PopupOpen;

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("OpenSplitMenu"), 0.5f);
	}

	public override void Hide ()
	{
		UIEventHandler.Instance.TriggerCloseWindow();
		Camera.main.GetComponent<BlurOptimized>().enabled = false;
		Time.timeScale = 1;

		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;


		InputEventHandler.Instance.State = UserInputState.Normal;


	}

	public override bool HasBodySlots (out List<BodySlot> bodySlots)
	{
		bodySlots = null;
		return false;
	}

	public override bool HasTempSlots (out List<TempSlot> tempSlots)
	{
		tempSlots = null;

		return false;
	}



	public void OnCloseButtonPress()
	{

		Hide();
	}
		
	private void RearrangePencilCircle()
	{
		Transform centerRef = GameObject.Find("CenterReference").transform;
		Vector3 worldDisplacement = GameManager.Inst.PlayerControl.SelectedPC.transform.position - centerRef.position;
		worldDisplacement = new Vector2(worldDisplacement.x, worldDisplacement.z);
		PencilCircle.transform.localPosition = worldDisplacement * _mapWorldRatio;

	}

	private float GetMapWorldRatio()
	{
		Transform centerRef = GameObject.Find("CenterReference").transform;
		Transform northRef = GameObject.Find("NorthReference").transform;

		return Vector3.Distance(Vector3.zero, PencilCircle.transform.localPosition) / Vector3.Distance(centerRef.position, northRef.position);
	}

}
