using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class NotePaperPanel : PanelBase
{
	public string Message;

	public UILabel MessageLabel;
	public UIButton Close;



	public override void Initialize ()
	{

		Hide();
	}

	public override void PerFrameUpdate ()
	{

	}

	public override void Show ()
	{
		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;

		MessageLabel.text = Message;

		InputEventHandler.Instance.State = UserInputState.PopupOpen;

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("OpenSplitMenu"), 0.5f);
	}

	public override void Hide ()
	{
		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;

		InputEventHandler.Instance.State = UserInputState.WindowsOpen;


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



}
