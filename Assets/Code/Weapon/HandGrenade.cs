using UnityEngine;
using System.Collections;

public class HandGrenade : ThrownObject
{
	public Explosive Explosive;
	public float FuseTimer;
	
	// Update is called once per frame
	void Update () 
	{
		if(Explosive.IsEnabled)
		{
			FuseTimer -= Time.deltaTime;
			if(FuseTimer <= 0)
			{
				Explosive.TriggerExplosion();
			}

			GetComponent<TrailRenderer>().time = 1.5f;
		}
	}
		

	void OnCollisionEnter(Collision collision)
	{
		float speed = GetComponent<Rigidbody>().velocity.magnitude;
		PhysicMaterial material = collision.collider.sharedMaterial;
		AudioSource audio = GetComponent<AudioSource>();
		if(audio != null)
		{
			float volume = Mathf.Clamp01(speed / 6f) * 0.1f;

			if(material != null && material.name == "Terrain")
			{
				int dropSound = UnityEngine.Random.Range(1, 5);
				audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("grass_impact" + dropSound.ToString()), volume);
			}
			else
			{
				int dropSound = UnityEngine.Random.Range(1, 5);
				audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("grenade_impact" + dropSound.ToString()), volume);
			}
		}
	}

	public void SetExplosive(Item myItem)
	{
		FuseTimer = (float)myItem.GetAttributeByName("Fuse Timer").Value;
		Explosive.BlastDamage = (float)myItem.GetAttributeByName("Damage").Value;
		Explosive.Range = (float)myItem.GetAttributeByName("Effective Radius").Value;
	}
}
