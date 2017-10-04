using UnityEngine;
using System.Collections;

public class PlayerTimedAction 
{
	public delegate void StartMethodDelegate();
	public delegate void EndMethodDelegate();
	public delegate void CancelMethodDelegate();

	public EndMethodDelegate EndMethod;
	public CancelMethodDelegate CancelMethod;

	private float _timedActionTimer;
	private float _timedActionDuration;
	private bool _isInAction;
	private bool _cancelRequested;

	private GameObject _lastAimedObject;

	private PlayerControl _playerControl;

	public PlayerTimedAction(PlayerControl control)
	{
		_playerControl = control;
	}

	public void PerFrameUpdate()
	{
		if(_isInAction)
		{
			_timedActionTimer += Time.deltaTime;
			if(_timedActionTimer >= _timedActionDuration)
			{
				EndMethod();
				_isInAction = false;
			}
		}
		else
		{
			_timedActionTimer = 0;

		}
	}


	public void StartTimedAction(StartMethodDelegate start, EndMethodDelegate end, CancelMethodDelegate cancel, float duration)
	{
		if(_isInAction)
		{
			//if there's another timed action going on, don't start a new one
			return;
		}

		_timedActionDuration = duration;
		_timedActionTimer = 0;
		_lastAimedObject = _playerControl.GetAimedObject();
		_cancelRequested = false;
		EndMethod = end;
		CancelMethod = cancel;

		start();

		_isInAction = true;
	}

	public void CancelTimedAction()
	{
		if(_isInAction)
		{
			CancelMethod();
			if(!_cancelRequested)
			{
				//this is not a delayed cancel
				_isInAction = false;
			}
		}
	}


	#region Disguise Body Methods

	public void StartDisguiseBody()
	{
		_playerControl.SelectedPC.MyAnimator.SetBool("IsChecking", true);
	}

	public void EndDisguiseBody()
	{
		Debug.Log("Finished disguising body");
		_playerControl.SelectedPC.MyAnimator.SetBool("IsChecking", false);
		GameObject dirtPile = GameObject.Instantiate(Resources.Load("DirtPile")) as GameObject;

		Character aimedCharacter = _lastAimedObject.GetComponent<Character>();
		Vector3 dir = aimedCharacter.MyReference.TorsoWeaponMount.transform.up;
		dir = new Vector3(dir.x, 0, dir.z) * -1;
		Quaternion rotation = Quaternion.LookRotation(dir);
		dirtPile.transform.position = _lastAimedObject.transform.position;
		dirtPile.transform.rotation = rotation;

		aimedCharacter.Stealth.Visibility = 3;
	}

	public void CancelDisguiseBody()
	{
		Debug.Log("Cancelled disguising body");
		_playerControl.SelectedPC.MyAnimator.SetBool("IsChecking", false);

	}

	#endregion

	#region Aim Then Shoot methods
	public void StartAimThenShoot()
	{
		GameManager.Inst.PlayerControl.SelectedPC.SendCommand(CharacterCommands.HipAim);
	}

	public void EndAimThenShoot()
	{
		GameManager.Inst.PlayerControl.SelectedPC.MyAI.WeaponSystem.StartFiringRangedWeapon();
		if(_cancelRequested)
		{
			GameManager.Inst.PlayerControl.SelectedPC.MyAI.WeaponSystem.StopFiringRangedWeapon();

		}
	}

	public void CancelAimThenShoot()
	{
		GameManager.Inst.PlayerControl.SelectedPC.SendCommand(CharacterCommands.StopAim);
		//_cancelRequested = true;
	}

	#endregion




}
