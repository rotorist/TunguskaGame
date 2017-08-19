using UnityEngine;
using System.Collections;

public class TimerEventHandler 
{

	#region Singleton 
	private static TimerEventHandler _instance;
	public static TimerEventHandler Instance	
	{
		get 
		{
			if (_instance == null)
				_instance = new TimerEventHandler();

			return _instance;
		}
	}

	public void OnUnloadScene()
	{
		OnHalfSecondTimer = null;
		OnOneSecondTimer = null;
		OnFiveSecondTimer = null;
	}

	#endregion

	#region Constructor
	public TimerEventHandler()
	{
		
	}

	#endregion

	public delegate void TimerEventDelegate();
	public static event TimerEventDelegate OnHalfSecondTimer;
	public static event TimerEventDelegate OnOneSecondTimer;
	public static event TimerEventDelegate OnFiveSecondTimer;
	public static event TimerEventDelegate OnOneDayTimer;

	public void TriggerOneSecondTimer()
	{
		if(OnOneSecondTimer != null)
		{
			OnOneSecondTimer();
		}
	}

	public void TriggerHalfSecondTimer()
	{
		if(OnHalfSecondTimer != null)
		{
			OnHalfSecondTimer();
		}
	}

	public void TriggerOneDayTimer()
	{
		if(OnOneDayTimer != null)
		{
			OnOneDayTimer();
		}
	}
}
