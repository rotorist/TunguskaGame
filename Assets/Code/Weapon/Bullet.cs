using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	public Weapon ParentWeapon;
	public Vector3 Velocity;
	public float Range;
	public float TotalDamage;
	public float Attack;
	public float CriticalChance;
	public float Penetration;
	public float Bleeding;
	public Item AmmoItem;

	public AnimationCurve DamageDropCurve;

	private float _distTraveled;
	private float _destroyTimer;
	private bool _isDestroyed;
	private Rigidbody _rigidbody;

	void Update()
	{
		if(!_isDestroyed)
		{
			_distTraveled += Velocity.magnitude * Time.deltaTime;


			if(_distTraveled > 3)
			{
				GetComponent<TrailRenderer>().enabled = true;
			}

			if(_distTraveled > Range)
			{
				GameObject.Destroy(this.gameObject);
			}
		}
		else
		{
			if(!_rigidbody.isKinematic)
			{
				_rigidbody.velocity = Vector3.zero;
				GetComponent<BoxCollider>().enabled = false;
				_rigidbody.isKinematic = true;
			}

			if(_destroyTimer < 0.5f)
			{
				_destroyTimer += Time.deltaTime;
			}
			else
			{
				GameObject.Destroy(this.gameObject);
			}
		}
	}

	public void Fire(Vector3 velocity, Weapon parentWeapon, float range, float attackRating, float impact, float critical)
	{
		_distTraveled = 0;
		_destroyTimer = 0;
		_isDestroyed = false;

		Range = range;
		ParentWeapon = parentWeapon;
		Velocity = velocity;

		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.velocity = velocity;

		TotalDamage = impact + (float)AmmoItem.GetAttributeByName("Damage").Value;
		Attack = attackRating;
		Penetration = (float)AmmoItem.GetAttributeByName("Penetration").Value;
		Bleeding = (float)AmmoItem.GetAttributeByName("_Bleeding").Value;
		CriticalChance = critical;

		GetComponent<TrailRenderer>().enabled = false;
	}

	void OnCollisionEnter(Collision collision) 
	{
		Character hitCharacter = collision.collider.GetComponent<Character>();
		DeathCollider deathCollider = collision.collider.GetComponent<DeathCollider>();

		if(hitCharacter == ParentWeapon.Attacker)
		{
			return;
		}

		if(hitCharacter != null && ParentWeapon.Attacker.MyAI.IsCharacterFriendly(hitCharacter))
		{
			return;
		}

		if(collision.collider.GetComponent<Bullet>() != null)
		{
			return;
		}


		Vector3 pos = collision.contacts[0].point;
		Vector3 normal = collision.contacts[0].normal;


		if(hitCharacter != null)
		{
			//calculate damage based on attack rating. higher the attack rating more likely the damage is near totalDamage
			float x = UnityEngine.Random.value;
			float yMax = EvaluateDamageCurve(Attack * 10, 1);
			float y = EvaluateDamageCurve(Attack * 10, x);
			float damage = TotalDamage * (y / yMax);

			float distPercent = _distTraveled / Range;
			float distDamageDrop = DamageDropCurve.Evaluate(distPercent) * damage;

			float randCritical = UnityEngine.Random.value;

			Damage deliveredDamage = new Damage();
			deliveredDamage.Type = DamageType.Bullet;
			deliveredDamage.KineticDamage = TotalDamage;
			deliveredDamage.Bleeding = Bleeding;
			deliveredDamage.Penetration = Penetration;

			if(randCritical <= CriticalChance)
			{
				deliveredDamage.IsCritical = true;
			}

			hitCharacter.SendDamage(deliveredDamage, normal * -1, ParentWeapon.Attacker, ParentWeapon);


			GameObject impact = GameManager.Inst.FXManager.LoadFX("GunshotBlood" + UnityEngine.Random.Range(1, 4).ToString(), 1, FXType.BloodSpatter);
			Debug.DrawRay(pos, normal, new Color(0, 1, 0), 3);
			impact.transform.position = pos;
			impact.transform.rotation = Quaternion.LookRotation(normal);
			BloodSpatter blood = impact.GetComponent<BloodSpatter>();
			blood.Source = hitCharacter;
			blood.Offset = pos - hitCharacter.transform.position;
			ParentWeapon.Attacker.OnSuccessfulHit(hitCharacter);

		}
		else
		{

			int upperBound = 5;
			PhysicMaterial material = collision.collider.sharedMaterial;
			GameObject impact = null;

			string clipName = "";
			string holeName = "";

			if(deathCollider != null)
			{
				impact = GameManager.Inst.FXManager.LoadFX("BulletImpactSand", 0, FXType.BulletImpact);
				clipName = "bullet_sand";
			}
			else if(material == null)
			{
				impact = GameManager.Inst.FXManager.LoadFX("BulletImpactConcrete", 0, FXType.BulletImpact);
				clipName = "bullet_default";
				upperBound = 10;
				holeName = "Bullet_Hole_Concrete";

			}
			else if(material.name == "Metal")
			{
				impact = GameManager.Inst.FXManager.LoadFX("BulletImpactMetal", 0, FXType.BulletImpact);
				clipName = "bullet_metal";
				holeName = "Bullet_Hole_Metal";
			}
			else if(material.name == "Terrain")
			{
				FloorType floorType = GameManager.Inst.WorldManager.CurrentTerrain.GetMainFloorType(this.transform.position);
				if(floorType == FloorType.rock)
				{
					impact = GameManager.Inst.FXManager.LoadFX("BulletImpactConcrete", 0, FXType.BulletImpact);
					clipName = "bullet_default";
					holeName = "Bullet_Hole_Concrete";
				}
				else if(floorType == FloorType.sand)
				{
					impact = GameManager.Inst.FXManager.LoadFX("BulletImpactSand", 0, FXType.BulletImpact);
					clipName = "bullet_sand";
				}
				else
				{
					impact = GameManager.Inst.FXManager.LoadFX("BulletImpactDirt", 0, FXType.BulletImpact);
					clipName = "bullet_dirt";
				}
			}
			else if(material.name == "SoftWood" || material.name == "Wood")
			{
				impact = GameManager.Inst.FXManager.LoadFX("BulletImpactWood", 0, FXType.BulletImpact);
				clipName = "bullet_wood";
				holeName = "Bullet_Hole_Wood";
			}
			else if(material.name == "Concrete")
			{
				impact = GameManager.Inst.FXManager.LoadFX("BulletImpactConcrete", 0, FXType.BulletImpact);
				clipName = "bullet_default";
				upperBound = 10;
				holeName = "Bullet_Hole_Concrete";
			}
			else
			{
				impact = GameManager.Inst.FXManager.LoadFX("BulletImpactConcrete", 0, FXType.BulletImpact);
				clipName = "bullet_default";
				upperBound = 10;
				holeName = "Bullet_Hole_Concrete";
			}

			//bullet hole
			if(holeName != "")
			{
				GameObject hole = GameManager.Inst.FXManager.LoadFX(holeName, 30, FXType.BulletHole);
				hole.transform.position = pos + normal * 0.02f;
				hole.transform.rotation = Quaternion.LookRotation(normal * -1);
			}



			int clipChoice = UnityEngine.Random.Range(1, upperBound);
			clipName = clipName + clipChoice.ToString();
			float volume = UnityEngine.Random.Range(0.12f, 0.2f);
			AudioSource audio = impact.GetComponent<AudioSource>();
			if(audio != null && Vector3.Distance(this.transform.position, GameManager.Inst.PlayerControl.SelectedPC.transform.position) < 10)
			{
				audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(clipName), volume);
			}

			impact.transform.position = pos;
			impact.transform.rotation = Quaternion.LookRotation(normal);



		}

		Rigidbody otherRB = collision.collider.GetComponent<Rigidbody>();

		if (otherRB) 
		{

			Vector3 force = normal * Velocity.magnitude * 1;
			otherRB.AddForceAtPosition(force, pos);


		}

		//_rigidbody.velocity = _rigidbody.velocity * 0.4f;
		GameObject.Destroy(this.gameObject);
	}

	private float EvaluateDamageCurve(float steepness, float x)
	{
		return -1 * Mathf.Exp(-1 * steepness * x) + 1;
	}
}
