using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlwindAnomaly : MonoBehaviour
{
	public float KillRadius;
	public AudioSource Audio;


	public ParticleSystem Leaves1;
	public ParticleSystem Leaves2;

	private Character _target;
	private float _spinTimer;
	private float _particleChangeRate;
	private float _emissionRate;
	void Update () 
	{
		transform.RotateAround(transform.position, transform.up, 60 * Time.deltaTime);
		if(_target != null)
		{
			transform.RotateAround(transform.position, transform.up, 60 * Time.deltaTime);
			Vector3 dist = _target.transform.position - transform.position;
			float y = dist.y;


			if(y < 0.5f && dist.magnitude < KillRadius)
			{
				if(_spinTimer <= 0)
				{
					Damage damage = new Damage();
					damage.Type = DamageType.Melee;
					damage.SharpDamage = 10000;
					_target.SendMeleeDamage(damage, Vector3.zero, _target, 0);
					Audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("WhirlwindSpin"), 0.5f);
					GameObject dust = GameManager.Inst.FXManager.LoadFX("WhirlwindInwardSmoke", 2, FXType.Explosion);
					dust.transform.position = transform.position + new Vector3(0, 0.5f, 0);

				}

				_target.transform.RotateAround(_target.transform.position, _target.transform.up, 600 * Time.deltaTime);

				_spinTimer += Time.deltaTime;
				if(_spinTimer > 2)
				{
					ExplodeCharacter();
				}


			}
		}

		//adjust leaf rate
		ParticleSystem.EmissionModule emission1 = Leaves1.emission;
		ParticleSystem.EmissionModule emission2 = Leaves2.emission;

		_emissionRate += _particleChangeRate;

		if(_emissionRate < 2)
		{
			emission1.rateOverTime = new ParticleSystem.MinMaxCurve(2);
			emission2.rateOverTime = new ParticleSystem.MinMaxCurve(2);
		}
		else
		{
			emission1.rateOverTime = new ParticleSystem.MinMaxCurve(_emissionRate);
			emission2.rateOverTime = new ParticleSystem.MinMaxCurve(_emissionRate);
		}
		if(_emissionRate > 3)
		{
			_particleChangeRate = UnityEngine.Random.Range(-0.01f, -0.002f);

		}
		else if(_emissionRate <= 0)
		{
			_particleChangeRate = UnityEngine.Random.Range(0.002f, 0.01f);
		}


	}

	void OnTriggerEnter(Collider other)
	{


		Character character = other.GetComponent<Character>();

		if(_target == null && character != null && character.MyStatus.Health > 0)
		{
			Damage damage = new Damage();
			damage.Type = DamageType.Explosive;
			damage.BlastDamage = 10;
			damage.IsCritical = true;
			character.SendDamage(damage, (transform.position - other.transform.position).normalized, null, null);

			_target = character;
			_spinTimer = 0;

		}
		else
		{
			Rigidbody rb = other.GetComponent<Rigidbody>();
			if(rb != null)
			{
				Vector3 dist = transform.position - other.transform.position;
				dist = new Vector3(dist.x, 0, dist.z).normalized;
				dist += new Vector3(0, 1, 0);
				rb.AddForce(dist * 8, ForceMode.Impulse);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(_target != null && other.gameObject == _target.gameObject)
		{
			_target = null;
		}
	}



	private void ExplodeCharacter()
	{
		
		_spinTimer = 0;

		GameObject b1 = GameManager.Inst.FXManager.LoadFX("MeleeBlood1", 1, FXType.BloodSpatter);
		GameObject b2 = GameManager.Inst.FXManager.LoadFX("MeleeBlood1", 1, FXType.BloodSpatter);

		BloodSpatter blood1 = b1.GetComponent<BloodSpatter>();
		BloodSpatter blood2 = b2.GetComponent<BloodSpatter>();
		blood1.transform.rotation = Quaternion.LookRotation(Vector3.left);
		blood2.transform.rotation = Quaternion.LookRotation(Vector3.right);
		blood1.transform.position = _target.transform.position + new Vector3(0, 1, 0);
		blood2.transform.position = _target.transform.position + new Vector3(0, 1, 0);
		blood1.transform.localScale = new Vector3(5, 5, 5);
		blood2.transform.localScale = new Vector3(5, 5, 5);

		GameObject explosion = GameObject.Instantiate(Resources.Load("BloodMist")) as GameObject;
		explosion.transform.position = _target.transform.position;

		Audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("airburst1"), 0.15f);
		if(_target.MyAI.ControlType == AIControlType.Player)
		{
			GameManager.Inst.CameraShaker.TriggerScreenShake(0.2f, 0.8f);
			GameManager.Inst.PlayerControl.SelectedPC.IsHidden = true;
		}
		else
		{
			if(_target.GetComponent<HumanCharacter>() != null)
			{
				GameManager.Inst.NPCManager.RemoveHumanCharacter((HumanCharacter)_target);
			}
			else if(_target.GetComponent<MutantCharacter>() != null)
			{
				GameManager.Inst.NPCManager.RemoveMutantCharacter((MutantCharacter)_target);
			}
		}
		_target = null;

	}

}
