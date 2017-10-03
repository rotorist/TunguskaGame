using UnityEngine;
using System.Collections;

public class MeleeWeapon : Weapon
{
	public bool IsSwung;
	public float Reach;
	public float SharpDamage;
	public float BluntDamage;
	public float Bleeding;
	public MeleeSoundType SoundType;


	private BoxCollider _collider;



	void OnCollisionEnter(Collision collision) 
	{
		if(!IsSwung)
		{
			return;
		}


		//Debug.Log("melee weapon hit: attacker " + Attacker.name);

		Character hitCharacter = collision.collider.GetComponent<Character>();
		Vector3 pos = collision.contacts[0].point;
		Vector3 normal = collision.contacts[0].normal;

		if(hitCharacter != null && Attacker.MyAI.IsCharacterFriendly(hitCharacter))
		{
			return;
		}

		//Debug.Log("hit collider is " + collision.collider.name);
		if(hitCharacter == Attacker)
		{
			return;
		}

		if(hitCharacter == null)
		{
			//show sparks or dust
			GameObject impact = GameManager.Inst.FXManager.LoadFX("WFX_BImpact Metal", 0, FXType.BulletImpact);
			impact.transform.position = transform.position;
			impact.transform.rotation = Quaternion.LookRotation(normal);

			AudioSource audio = GetComponent<AudioSource>();
			if(audio != null)
			{
				int choice = UnityEngine.Random.Range(1, 8);
				float volume = UnityEngine.Random.Range(0.1f, 0.2f);
				audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("melee_block" + choice.ToString()), volume);
			}
		}


		if(hitCharacter != null)
		{
			//Debug.Log("hit somebody! " + hitCharacter.name);
			Vector3 fakeNormal = (hitCharacter.transform.position - Attacker.transform.position).normalized;
			float multiplier = 1;
			if(Attacker.MyAI.ControlType == AIControlType.Player)
			{
				multiplier = GameManager.Inst.Constants.MeleeDamageVsEnergy.Evaluate(Attacker.MyStatus.Energy / Attacker.MyStatus.MaxEnergy);
			}

			Damage damage = new Damage();


			float durabilityMultiplier = 1; 

			if(WeaponItem != null)
			{
				//handle weapon durability
				GameManager.Inst.Constants.MeleeDamageVsDurability.Evaluate(WeaponItem.Durability / WeaponItem.MaxDurability);
				WeaponItem.Durability -= GameManager.Inst.Constants.DurabilityDrainRate;
				if(WeaponItem.Durability < 0)
				{
					WeaponItem.Durability = 0;
				}
			}

			damage.SharpDamage = SharpDamage * multiplier * durabilityMultiplier;
			damage.BluntDamage = BluntDamage * multiplier * durabilityMultiplier;
			damage.Bleeding = Bleeding * multiplier * durabilityMultiplier;
			damage.Type = DamageType.Melee;



			MeleeBlockType blockType = hitCharacter.SendMeleeDamage(damage, fakeNormal, Attacker, 1f);

			if(blockType != MeleeBlockType.NoBlock)
			{
				GameObject impact;
				if(blockType == MeleeBlockType.Metal)
				{
					impact = GameManager.Inst.FXManager.LoadFX("WFX_BImpact Metal", 0, FXType.BulletImpact);
				}
				else
				{
					impact = GameManager.Inst.FXManager.LoadFX("BulletImpactWood", 0, FXType.BulletImpact);
				}

				impact.transform.position = transform.position;
				impact.transform.rotation = Quaternion.LookRotation(normal);

				if(Attacker == GameManager.Inst.PlayerControl.SelectedPC)
				{
					GameManager.Inst.CameraShaker.TriggerScreenShake(0.05f, 0.08f);

				}

				AudioSource audio = GetComponent<AudioSource>();
				if(audio != null)
				{
					int choice = UnityEngine.Random.Range(1, 8);
					float volume = UnityEngine.Random.Range(0.1f, 0.2f);
					if(blockType == MeleeBlockType.Metal)
					{
						audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("melee_block" + choice.ToString()), volume);
					}
					else
					{
						audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("body_hit" + choice.ToString()), volume);
					}
				}

			}
			else
			{
				GameObject impact = null;

				if(pos.y > hitCharacter.transform.position.y + collision.collider.bounds.size.y * 0.75f)
				{
					impact = GameManager.Inst.FXManager.LoadFX("MeleeBlood1", 1, FXType.BloodSpatter);
					impact.transform.position = pos - new Vector3(0, 0.5f, 0);
				}
				else
				{
					impact = GameManager.Inst.FXManager.LoadFX("MeleeBlood2", 1, FXType.BloodSpatter);
					impact.transform.position = pos;
				}

				BloodSpatter blood = impact.GetComponent<BloodSpatter>();
				blood.Source = hitCharacter;
				blood.Offset = pos - hitCharacter.transform.position;
				impact.transform.rotation = Quaternion.LookRotation(normal);

				if(Attacker == GameManager.Inst.PlayerControl.SelectedPC)
				{

					GameManager.Inst.CameraShaker.TriggerScreenShake(0.04f, 0.09f);
					//GameManager.Inst.CameraShaker.TriggerDirectionalShake(0.1f, 0.08f, new Vector3(0.1f, 0.1f, 0));

				}

				AudioSource audio = GetComponent<AudioSource>();
				if(audio != null)
				{
					Debug.Log("playing hit audio");
					int choice = UnityEngine.Random.Range(1, 9);
					float volume = UnityEngine.Random.Range(0.01f, 0.2f);
					audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("body_hit" + choice.ToString()), volume);
				}
			}
		}
	}

	public override void Rebuild (WeaponCallBack callBack, Item weaponItem)
	{
		this.WeaponItem = weaponItem;
		_collider = GetComponent<BoxCollider>();
		_collider.enabled = false;

		if(weaponItem != null)
		{
			this.WeaponItem = weaponItem;
			SharpDamage = (float)weaponItem.GetAttributeByName("Sharp Damage").Value;
			BluntDamage = (float)weaponItem.GetAttributeByName("Blunt Damage").Value;
			Bleeding = (float)weaponItem.GetAttributeByName("_Bleeding").Value;
		}
	}

	public void SwingStart()
	{
		_collider.enabled = true;
		IsSwung = true;

		AudioSource audio = GetComponent<AudioSource>();
		if(audio != null && SoundType == MeleeSoundType.Blade)
		{
			int choice = UnityEngine.Random.Range(1, 5);
			audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("melee_swing_fast" + choice.ToString()), 0.2f);
		}
	}

	public void SwingStop()
	{
		_collider.enabled = false;
		IsSwung = false;
	}
}

