using UnityEngine;
using System.Collections;

public class CharacterEventHandler : MonoBehaviour 
{
	public delegate void AIEventDelegate();
	public event AIEventDelegate OnHalfSecondTimer;
	public event AIEventDelegate OnOneSecondTimer;
	public event AIEventDelegate OnActionUpdateTimer;
	public event AIEventDelegate OnPerFrameTimer;
	public event AIEventDelegate OnCurrentActionComplete;
	public event AIEventDelegate OnTakingHit;
	public event AIEventDelegate OnPutAwayWeapon;

	public delegate void AIEventDelegateString(string stringValue);
	public event AIEventDelegateString OnSelectWeapon;

	//AI events

	public void TriggerOnHalfSecondTimer()
	{
		if(OnHalfSecondTimer != null)
		{
			OnHalfSecondTimer();
		}
	}

	public void TriggerOnOneSecondTimer()
	{
		if(OnOneSecondTimer != null)
		{
			OnOneSecondTimer();
		}
	}

	public void TriggerOnActionUpdateTimer()
	{
		if(OnActionUpdateTimer != null)
		{
			OnActionUpdateTimer();
		}
	}

	public void TriggerOnPerFrameTimer()
	{
		if(OnPerFrameTimer != null)
		{
			OnPerFrameTimer();
		}
	}

	public void TriggerOnActionCompletion()
	{
		if(OnCurrentActionComplete != null)
		{
			OnCurrentActionComplete();
		}
	}

	public void TriggerOnTakingHit()
	{
		if(OnTakingHit != null)
		{
			OnTakingHit();
		}
	}

	public void TriggerOnPutAwayWeapon()
	{
		if(OnPutAwayWeapon != null)
		{
			OnPutAwayWeapon();
		}
	}





	public void TriggerOnSelectWeapon(string weaponID)
	{
		if(OnSelectWeapon != null)
		{
			OnSelectWeapon(weaponID);
		}
	}


}
