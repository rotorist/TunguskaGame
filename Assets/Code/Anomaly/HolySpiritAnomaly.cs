using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolySpiritAnomaly : MonoBehaviour 
{
	public LighteningScript Arc;
	public UnityEngine.AI.NavMeshAgent Agent;
	public AudioSource Sound;

	private List<GameObject> _targets;
	private float _updateTimer;
	private float _updateTimeout;


	void Start()
	{
		_targets = new List<GameObject>();

		//Find a list of metal objects around it
		RaycastHit [] hits = Physics.SphereCastAll(transform.position, 30, Vector3.up);
		if(hits.Length > 0)
		{
			foreach(RaycastHit hit in hits)
			{
				if(hit.collider.sharedMaterial != null && hit.collider.sharedMaterial.name == "Metal")
				{
					_targets.Add(hit.collider.gameObject);
				}
			}
		}

		_updateTimeout = 1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//only call update if player is near
		if(Vector3.Distance(GameManager.Inst.PlayerControl.SelectedPC.transform.position, transform.position) > 30)
		{
			Sound.Stop();
			return;
		}
		else
		{
			if(!Sound.isPlaying)
			{
				Sound.Play();
			}
		}

		_updateTimer += Time.deltaTime;
		if(_updateTimer > _updateTimeout)
		{
			
			SlowUpdate();

		}
	}

	private void SlowUpdate()
	{
		_updateTimer = 0;
		_updateTimeout = UnityEngine.Random.Range(1f, 4f);



		//find a new destination to move to
		Vector3 newDest;
		if(AI.RandomPoint(transform.position, new Vector3(15f, 1f, 15f), out newDest))
		{
			Agent.destination = newDest;

		}

		HumanCharacter player = GameManager.Inst.PlayerControl.SelectedPC;

		//check if player is within range and visible
		if(Vector3.Distance(player.transform.position, transform.position) < 5)
		{
			RaycastHit checkHit;
			float colliderHeight = player.GetComponent<CapsuleCollider>().height;
			Vector3 rayTarget = player.transform.position + Vector3.up * colliderHeight * 0.5f;
			Ray ray = new Ray(transform.position, rayTarget - transform.position);
			if(Physics.Raycast(ray, out checkHit))
			{
				if(checkHit.collider.gameObject == player.gameObject)
				{
					//now move lightening target to player and apply damage
					Arc.target = player.MyReference.TorsoWeaponMount;
					Damage damage = new Damage();
					damage.Type = DamageType.Explosive;
					damage.BlastDamage = 25;
					damage.IsCritical = true;
					player.SendDamage(damage, checkHit.normal * -1, null, null);
					PlayLighteningSound();
					return;
				}

			}
		}

		float dist = 30;
		GameObject candidate = null;
		foreach(GameObject target in _targets)
		{
			float tempDist = Vector3.Distance(target.transform.position, transform.position);
			if(tempDist < dist)
			{
				candidate = target;
				dist = tempDist;
			}

			//turn off any lights
			Lamp lamp = target.GetComponent<Lamp>();
			if(lamp != null && lamp.IsOn)
			{
				lamp.Toggle();
			}
		}

		if(candidate != null)
		{
			Arc.target = candidate;
			PlayLighteningSound();
			//check if target is a lamp
			Lamp lamp = candidate.GetComponent<Lamp>();
			if(lamp != null && !lamp.IsOn)
			{
				lamp.Toggle();
			}
			return;
		}



	}

	private void PlayLighteningSound()
	{
		AudioClip clip = GameManager.Inst.SoundManager.GetClip("ElectricShock");
		Sound.PlayOneShot(clip, 1f);
	}
}
