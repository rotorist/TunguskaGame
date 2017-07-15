using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIScheduler
{
	private int _charIndex;
	private int _oneSecIndex;
	private int _halfSecIndex;

	public void Initialize()
	{
		TimerEventHandler.OnOneSecondTimer -= OnOneSecondTimer;
		TimerEventHandler.OnOneSecondTimer += OnOneSecondTimer;

		TimerEventHandler.OnHalfSecondTimer -= OnHalfSecondTimer;
		TimerEventHandler.OnHalfSecondTimer += OnHalfSecondTimer;
	}

	public void OnOneSecondTimer()
	{
		//start calling each AI's OnOneSecondTimer one per frame
		_oneSecIndex = 0;
	}

	public void OnHalfSecondTimer()
	{
		
		_halfSecIndex = 0;
	}

	public void UpdatePerFrame()
	{
		//call each AI's per frame udpate
		List<Character> characters = GameManager.Inst.NPCManager.AllCharacters;
		if(_charIndex >= characters.Count)
		{
			_charIndex = 0;
		}
		if(_charIndex >= 0 && characters[_charIndex] != null && characters[_charIndex].MyAI.ControlType != AIControlType.Player)
		{
			while(Vector3.Distance(GameManager.Inst.PlayerControl.SelectedPC.transform.position, characters[_charIndex].transform.position) >= GameManager.Inst.AIUpdateRadius)
			{
				_charIndex ++;
				if(_charIndex >= characters.Count)
				{
					_charIndex = -1000;
					break;
				}
			}

			if(_charIndex >= 0 && _charIndex < characters.Count)
			{
				characters[_charIndex].MyEventHandler.TriggerOnPerFrameTimer();
			}

			_charIndex ++;

		}




		if(_oneSecIndex >= 0 && characters.Count > _oneSecIndex && characters[_oneSecIndex] != null && characters[_oneSecIndex].MyAI.ControlType != AIControlType.Player)
		{
			while(Vector3.Distance(GameManager.Inst.PlayerControl.SelectedPC.transform.position, characters[_oneSecIndex].transform.position) >= GameManager.Inst.AIUpdateRadius)
			{
				_oneSecIndex ++;
				if(_oneSecIndex >= characters.Count)
				{
					_oneSecIndex = -1000;
					break;
				}
			}

			if(_oneSecIndex >= 0)
			{
				characters[_oneSecIndex].MyEventHandler.TriggerOnOneSecondTimer();

				characters[_oneSecIndex].MyEventHandler.TriggerOnActionUpdateTimer();
			}

			_oneSecIndex ++;

		}




		//Debug.LogError(Time.time + " Triggering half second timer for index " + _halfSecIndex);

		if(_halfSecIndex >= 0 && characters.Count > _halfSecIndex && characters[_halfSecIndex] != null && characters[_halfSecIndex].MyAI.ControlType != AIControlType.Player)
		{
			while(Vector3.Distance(GameManager.Inst.PlayerControl.SelectedPC.transform.position, characters[_halfSecIndex].transform.position) >= GameManager.Inst.AIUpdateRadius)
			{
				_halfSecIndex ++;
				if(_halfSecIndex >= characters.Count)
				{
					_halfSecIndex = -1000;
					break;
				}
			}
			if(_halfSecIndex >= 0)
			{
				characters[_halfSecIndex].MyEventHandler.TriggerOnHalfSecondTimer();

			}

			_halfSecIndex ++;
		}


	}
}
