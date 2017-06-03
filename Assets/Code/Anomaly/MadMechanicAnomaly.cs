using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadMechanicAnomaly : MonoBehaviour 
{
	public int State; //0 no effect, -1 degrading, 1 improving
	public EllipsoidParticleEmitter Steam;

	private float _updateTimer;
	private int _nextStateChangeTime;
	private int _stateChangeTimer;
	private float _modRate;
	private List<PickupItem> _weapons;

	// Use this for initialization
	void Start () 
	{
		_weapons = new List<PickupItem>();
		UpdateEffect();
	}
	
	// Update is called once per frame
	void Update () 
	{
		_updateTimer += Time.deltaTime;
		if(_updateTimer > 1)
		{
			_updateTimer = 0;
			UpdateEffect();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		PickupItem pickup = other.GetComponent<PickupItem>();
		if(pickup != null && (pickup.Item.Type == ItemType.PrimaryWeapon || pickup.Item.Type == ItemType.SideArm))
		{
			if(!_weapons.Contains(pickup))
			{
				_weapons.Add(pickup);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		PickupItem pickup = other.GetComponent<PickupItem>();
		if(pickup != null && _weapons.Contains(pickup))
		{
			_weapons.Remove(pickup);
		}
	}

	private void UpdateEffect()
	{
		if(_stateChangeTimer >= _nextStateChangeTime)
		{
			_nextStateChangeTime = UnityEngine.Random.Range(5, 26);
			State = UnityEngine.Random.Range(-1, 2);
			if(State > 0)
			{
				_modRate = UnityEngine.Random.Range(0.5f, 2f);
			}
			else 
			{
				_modRate = UnityEngine.Random.Range(2f, 5f);
			}

			_stateChangeTimer = 0;

			//update anomaly appearance
			if(State == 0)
			{
				Steam.minSize = 0;
				Steam.maxSize = 0;
			}
			else if(State == -1)
			{
				Steam.minSize = 0.3f;
				Steam.maxSize = 0.6f;
			}
			else
			{
				Steam.minSize = 0.1f;
				Steam.maxSize = 0.2f;
			}
		}
		else
		{
			_stateChangeTimer ++;
		}

		foreach(PickupItem weapon in _weapons)
		{
			if(weapon == null)
			{
				_weapons.Remove(weapon);
				return;
			}

			Debug.Log(weapon.name);

			weapon.Item.Durability += _modRate * State;

			if(weapon.Item.Durability > weapon.Item.MaxDurability)
			{
				weapon.Item.Durability = weapon.Item.MaxDurability;
			}
			else if(weapon.Item.Durability < 0)
			{
				//destroy weapon

				GameObject explosion = GameObject.Instantiate(Resources.Load("WFX_Explosion Small")) as GameObject;
				explosion.transform.position = weapon.transform.position;

				AudioSource audio = explosion.GetComponent<AudioSource>();
				string clipName = "explosion_close" + UnityEngine.Random.Range(1, 4).ToString();
				audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(clipName), 0.05f);

				GameObject.Destroy(weapon.gameObject);
			}
		}
	}

}
