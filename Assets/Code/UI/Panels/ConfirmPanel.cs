using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class ConfirmPanel : PanelBase
{
	public UILabel MessageLabel;
	public UIButton Yes;
	public UIButton Cancel;

	public delegate void ConfirmCallBack();


	private ConfirmCallBack _onConfirmation;


	public override void Initialize ()
	{

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


	public void SetupPanel(string message, ConfirmCallBack callBack)
	{
		_onConfirmation = callBack;
		MessageLabel.text = message;
	}


	public void OnCancelButtonPress()
	{

		Hide();
	}

	public void OnYesButtonPress()
	{
		Hide();
		_onConfirmation();
	}


}
