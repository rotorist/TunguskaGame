using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThrowingRock : ThrownObject 
{

	private float _lifeTime; //time to live
	private float _lifeTimer;
	private float _onGroundTime; //time to live after first collision
	private float _groundTimer;

	private bool _hasCollided;

	private Dictionary<HumanCharacter,float> _notifyTargets;


	// Use this for initialization
	void Start () 
	{
		_lifeTime = 5;
		_onGroundTime = 10;
		_hasCollided = false;

		_notifyTargets = new Dictionary<HumanCharacter, float>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(IsThrown)
		{
			_lifeTimer += Time.deltaTime;
			if(_lifeTimer >= _lifeTime)
			{
				//destroy the rock
				GameObject.Destroy(gameObject);
			}
		}
		if(_hasCollided)
		{
			_groundTimer += Time.deltaTime;
			if(_groundTimer >= _onGroundTime)
			{
				//destroy the rock
				GameObject.Destroy(gameObject);
			}

			Dictionary<HumanCharacter,float> copy = new Dictionary<HumanCharacter, float>(_notifyTargets);
			foreach(KeyValuePair<HumanCharacter,float> entry in copy)
			{
				if(entry.Value < _groundTimer)
				{
					entry.Key.MyAI.Sensor.OnReceiveDisturbance(0.2f, this.Thrower, transform.position, this.Thrower);
					_notifyTargets[entry.Key] = _onGroundTime + 1;
				}
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if(!_hasCollided && IsThrown)
		{
			_hasCollided = true;

			//now send a disturbance to all human characters within sound range
			List<HumanCharacter> humans = GameManager.Inst.NPCManager.HumansInScene;
			foreach(HumanCharacter human in humans)
			{
				if(Vector3.Distance(transform.position, human.transform.position) <= 10)
				{
					
					_notifyTargets.Add(human, UnityEngine.Random.Range(0, 1000)/1000f);
				}
			}
		}

		float speed = GetComponent<Rigidbody>().velocity.magnitude;
		PhysicMaterial material = collision.collider.sharedMaterial;
		AudioSource audio = GetComponent<AudioSource>();
		if(audio != null)
		{
			float volume = Mathf.Clamp01(speed / 6f) * 0.1f;

			if(material != null && material.name == "Terrain")
			{
				int dropSound = UnityEngine.Random.Range(1, 5);
				audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("grass_impact" + dropSound.ToString()), volume * 0.6f);
			}
			else
			{
				int dropSound = UnityEngine.Random.Range(1, 5);
				audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("rock_impact" + dropSound.ToString()), volume * 3f);
			}
		}
	}

}
