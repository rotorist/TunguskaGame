using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerStatBoost
{
	public PlayerBoostType Type;
	public bool IsEnded;



	[SerializeField]private int _timer;
	[SerializeField]private int _duration;
	[SerializeField]private float _origValue;
	private CharacterStatus _playerStats;

	public PlayerStatBoost(PlayerBoostType type, CharacterStatus playerStats)
	{
		Type = type;
		IsEnded = false;
		_timer = 0;
		_playerStats = playerStats;
	}

	public void PerSecondUpdate()
	{
		if(IsEnded)
		{
			return;
		}
		else if(_timer > _duration)
		{
			//restore original values
			if(Type == PlayerBoostType.MaxStamina)
			{
				_playerStats.MaxStamina = _origValue;
				_timer = -1;
				IsEnded = true;
			}
		}
		else
		{
			_timer ++;
		}
	}

	public void StartBoost(int duration, float boost)
	{
		IsEnded = false;
		_timer = 0;
		_duration = duration;

		if(Type == PlayerBoostType.MaxStamina)
		{
			_origValue = _playerStats.MaxStamina;
			_playerStats.MaxStamina += boost;
		}
	}

	public void PostLoad()
	{
		_playerStats = GameManager.Inst.PlayerControl.SelectedPC.MyStatus;
	}
}

public enum PlayerBoostType
{
	MaxStamina,
	MaxHealth,
	MaxEnergy,
}