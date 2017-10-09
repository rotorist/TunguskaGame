using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosive : MonoBehaviour 
{
	public Character Attacker;
	public float Range;
	public float BlastDamage;
	public AnimationCurve DamageModifier;
	public bool IsEnabled;

	public void TriggerExplosion()
	{
		GetComponent<CapsuleCollider>().enabled = false;
		GameObject explosion = GameObject.Instantiate(Resources.Load("WFX_Explosion StarSmoke")) as GameObject;
		explosion.transform.position = transform.position;

		AudioSource audio = explosion.GetComponent<AudioSource>();
		string clipName = "explosion_close" + UnityEngine.Random.Range(1, 4).ToString();
		audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(clipName), 0.2f);

		RaycastHit [] hits = Physics.SphereCastAll(transform.position, Range, Vector3.up);
		if(hits.Length > 0)
		{
			foreach(RaycastHit hit in hits)
			{
				Character c = hit.collider.GetComponent<Character>();
				if(c != null && c.MyStatus.Health > 0)
				{
					
					//now do a raycast check
					RaycastHit checkHit;
					float colliderHeight = c.GetComponent<CapsuleCollider>().height;
					Vector3 rayTarget = c.transform.position + Vector3.up * colliderHeight * 0.65f;
					Ray ray = new Ray(transform.position, rayTarget - transform.position);
					if(Physics.Raycast(ray, out checkHit))
					{
						if(checkHit.collider.gameObject == c.gameObject)
						{
							
							//now apply damage
							float dist = Vector3.Distance(c.transform.position, transform.position);
							dist = Mathf.Clamp(dist / Range, 0, 1);
							float modifer = DamageModifier.Evaluate(dist);

							Damage deliveredDamage = new Damage();
							deliveredDamage.Type = DamageType.Explosive;
							deliveredDamage.BlastDamage = BlastDamage * modifer; 
							if(dist < Range * 0.5f)
							{
								deliveredDamage.IsCritical = true;
							}
							c.SendDamage(deliveredDamage, checkHit.normal * -1, Attacker, null);

							if(c.MyAI.ControlType == AIControlType.Player)
							{
								//ear ring effect
								GameManager.Inst.SoundManager.StartPlayerEarRingEffect(1 - dist);
							}
						}
					}


				}
			}
		}

		//now send a disturbance to all human characters within sound range
		List<HumanCharacter> humans = GameManager.Inst.NPCManager.HumansInScene;
		foreach(HumanCharacter human in humans)
		{

			if(Vector3.Distance(transform.position, human.transform.position) <= 30)
			{
				human.MyAI.Sensor.OnReceiveDisturbance(0.85f, this, transform.position, Attacker);
			}
		}

		//shake camera for player depending on distance
		float playerDist = Vector3.Distance(transform.position, GameManager.Inst.PlayerControl.SelectedPC.transform.position);
		if(playerDist < 20)
		{
			float modifier = 1 - Mathf.Clamp01(playerDist / 20);
			GameManager.Inst.CameraShaker.TriggerScreenShake(0.4f, 0.4f * modifier);
		}

		GameObject.Destroy(gameObject);
	}
}
