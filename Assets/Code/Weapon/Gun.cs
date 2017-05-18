using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GunReceiver))]
[RequireComponent(typeof(GunBarrel))]
[RequireComponent(typeof(GunStock))]
[RequireComponent(typeof(GunMagazine))]

public class Gun : Weapon 
{

	public Transform MuzzleFlash;
	public Vector3 MuzzleFlashPosition;
	public GunReceiver Receiver;
	public GunBarrel Barrel;
	public GunStock Stock;
	public GunMagazine Magazine;
	public ParticleSystem BrassEject;
	public GameObject MagazineObject;
	public LineRenderer Laser;

	public GunFireModes CurrentFireMode;


	private float _coolDownTimer;
	private bool _isCooledDown;
	private bool _isTriggerDown;

	private int _projectilesPerShot;
	private Item _ammoItem;
	private ParticleSystem _sparks;
	private ParticleSystem _flame;
	private GameObject _bulletOrigin;
	private Light _light;
	private Vector3 _foreGripPos;

	private WeaponCallBack _onSuccessfulShot;

	private bool _isEquipped;
	private bool _pumpStarted;

	void Update()
	{

		if(_isEquipped)
		{
			/*
			//this is a workaround for the bug when particle only follows root object and not child object in a rig
			MuzzleFlash.parent = this.transform;
			MuzzleFlash.localPosition = MuzzleFlashPosition;
			MuzzleFlash.parent = null;
			MuzzleFlash.forward = this.transform.forward * -1;
			*/

			//update cool down timer
			if(!_isCooledDown)
			{
				_coolDownTimer += Time.deltaTime;
				if(CurrentFireMode == GunFireModes.Burst && _coolDownTimer >= 1 / Receiver.BurstFireRate)
				{
					_isCooledDown = true;
					_coolDownTimer = 0;
				}
				else if(CurrentFireMode == GunFireModes.Semi && _coolDownTimer >= 1 / Receiver.SemiFireRate)
				{
					_isCooledDown = true;
					_coolDownTimer = 0;
				}
				else if(CurrentFireMode == GunFireModes.Full && _coolDownTimer >=  1 / Receiver.AutoFireRate)
				{
					_isCooledDown = true;
					_coolDownTimer = 0;
				}
				else if(CurrentFireMode == GunFireModes.Pump)
				{
					if(_coolDownTimer >= 1 / Receiver.ManualFireRate)
					{
						_isCooledDown = true;
						_coolDownTimer = 0;
					}
					else
					{
						//pump action
						if(_coolDownTimer > 1 / Receiver.ManualFireRate * 0.25f && _coolDownTimer < 1 / Receiver.ManualFireRate * 0.5f)
						{
							this.ForeGrip.localPosition = Vector3.Lerp(this.ForeGrip.localPosition, _foreGripPos - new Vector3(0, 0, 0.1f), Time.deltaTime * 12);
							if(!_pumpStarted && Attacker.MyAI.ControlType == AIControlType.Player)
							{
								AudioSource audio = GetComponent<AudioSource>();
								if(audio != null)
								{
									audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(WeaponItem.ID + "_pump"), 0.2f);
								}

								//emit brass
								if(BrassEject != null)
								{
									BrassEject.Emit(1);
								}

								_pumpStarted = true;
							}


						}
						else
						{
							this.ForeGrip.localPosition = Vector3.Lerp(this.ForeGrip.localPosition, _foreGripPos, Time.deltaTime * 12);
							_pumpStarted = false;
						}
					}
				}
				else if(CurrentFireMode == GunFireModes.Bolt && _coolDownTimer >= 1 / Receiver.ManualFireRate)
				{
					_isCooledDown = true;
					_coolDownTimer = 0;
					this.ForeGrip.localPosition = _foreGripPos;
				}
			}

			//fire automatic
			if(CurrentFireMode == GunFireModes.Full && _isTriggerDown && _isCooledDown)
			{
				TriggerPull();
			}

			//update laser
			if(Laser != null && ((HumanCharacter)Attacker).UpperBodyState == HumanUpperBodyStates.Aim)
			{
				Laser.enabled = true;
				RaycastHit hit;
				Vector3 laserDir = Laser.transform.forward;
				if(Physics.Raycast(Laser.transform.position, laserDir, out hit))
				{
					Laser.SetPositions(new Vector3[]{Laser.transform.position, hit.point});
				}
				else
				{
					Laser.SetPositions(new Vector3[]{Laser.transform.position, Laser.transform.position + laserDir * 50});
				}
			}
			else if(Laser != null)
			{
				Laser.enabled = false;
			}
		}
	}


	public override void Rebuild(WeaponCallBack callBack, Item weaponItem)
	{
		Receiver = GetComponent<GunReceiver>();
		Barrel = GetComponent<GunBarrel>();
		Stock = GetComponent<GunStock>();
		Magazine = GetComponent<GunMagazine>();

		this.WeaponItem = weaponItem;

		_isCooledDown = true;

		_onSuccessfulShot = callBack;

		_sparks = MuzzleFlash.FindChild("Sparks").GetComponent<ParticleSystem>();
		_flame = MuzzleFlash.GetComponent<ParticleSystem>();
		_bulletOrigin = MuzzleFlash.FindChild("BulletOrigin").gameObject;
		//_bulletTrail.ParentWeapon = this;
		_light = MuzzleFlash.FindChild("Light").GetComponent<Light>();
		_foreGripPos = this.ForeGrip.localPosition;
		_isEquipped = true;

		Barrel.Accuracy = (float)weaponItem.GetAttributeByName("Accuracy").Value;
		Barrel.Impact = (float)weaponItem.GetAttributeByName("Impact").Value;
		Barrel.MuzzleVelocity = (float)weaponItem.GetAttributeByName("_Muzzle Velocity").Value;
		Barrel.Range = (float)weaponItem.GetAttributeByName("Range").Value;
		Barrel.Handling = (float)weaponItem.GetAttributeByName("Handling").Value;

		Magazine.MaxCapacity = (int)weaponItem.GetAttributeByName("Magazine Size").Value;

		Receiver.Recoil = (float)weaponItem.GetAttributeByName("Recoil").Value;
		ItemAttribute fireDelayAttr = weaponItem.GetAttributeByName("_FireDelay");
		if(fireDelayAttr != null)
		{
			Receiver.FireDelay = (float)fireDelayAttr.Value;
		}
		else
		{
			Receiver.FireDelay = 0;
		}

		switch(Receiver.FireModes[0])
		{
		case GunFireModes.Semi:
			Receiver.SemiFireRate = (float)weaponItem.GetAttributeByName("Rate of Fire").Value;
			break;
		case GunFireModes.Full:
			Receiver.AutoFireRate = (float)weaponItem.GetAttributeByName("Rate of Fire").Value;
			break;
		case GunFireModes.Burst:
			Receiver.BurstFireRate = (float)weaponItem.GetAttributeByName("Rate of Fire").Value;
			break;
		case GunFireModes.Pump:
			Receiver.ManualFireRate = (float)weaponItem.GetAttributeByName("Rate of Fire").Value;
			break;
		}

		_pumpStarted = false;


		Refresh();

	}

	public override void Refresh ()
	{
		Magazine.AmmoLeft = (int)WeaponItem.GetAttributeByName("_LoadedAmmos").Value;
		Magazine.LoadedAmmoID = (string)WeaponItem.GetAttributeByName("_LoadedAmmoID").Value;

		Item ammo = GameManager.Inst.ItemManager.LoadItem(Magazine.LoadedAmmoID);
		_ammoItem = ammo;
		_projectilesPerShot = (int)ammo.GetAttributeByName("_numberOfProjectiles").Value;
	}

	public override float GetTotalLessMoveSpeed()
	{
		float value = 0;
		if(Barrel != null)
		{
			value += Barrel.LessMoveSpeed;
		}

		return value;
	}


	public Item GetAmmoItem()
	{
		return _ammoItem;
	}

	public float GetRecoil()
	{
		float value = 0;
		if(Receiver != null)
		{
			value += Receiver.Recoil;
		}
			

		return value;
	}



	public bool TriggerPull()
	{
		//Debug.Log("trigger pull iscooled down " + _isCooledDown + " ammo left " + Magazine.AmmoLeft);
		if(_isCooledDown && Magazine.AmmoLeft > 0)
		{

			FlashMuzzle();
			_isCooledDown = false;
			_isTriggerDown = true;

			this.Attacker.Stealth.SetNoiseLevel(50, 0.8f);
			this.Attacker.Stealth.SetWeaponVisibilityBoost(20);

			Magazine.AmmoLeft --;
			//update weapon item attributes after each shot
			WeaponItem.SetAttribute("_LoadedAmmos", Magazine.AmmoLeft);
			AudioSource audio = GetComponent<AudioSource>();
			if(audio != null)
			{
				string clipName = WeaponItem.ID + "_shot" + UnityEngine.Random.Range(1, 5).ToString();
				AudioClip clip = GameManager.Inst.SoundManager.GetClip(clipName);
				audio.PlayOneShot(clip, 0.2f);

			}
			//emit brass
			if(Attacker.MyAI.ControlType == AIControlType.Player && BrassEject != null && CurrentFireMode != GunFireModes.Pump)
			{
				BrassEject.Emit(1);
			}

			_onSuccessfulShot();

			return true;
		}


		return false;
	}

	public void TriggerRelease()
	{
		_isTriggerDown = false;
	}





	private void FlashMuzzle()
	{

		_light.enabled = true;
		_flame.Emit(1);
		_sparks.Emit(1);
		//_bulletTrail.Bullets.Emit(1);



		for(int i=0; i< _projectilesPerShot; i++)
		{

			float scatterValue = 0;
			float muzzleVelocity = Barrel.MuzzleVelocity;
			if(_projectilesPerShot > 1)
			{
				scatterValue = (1 - Barrel.Accuracy) / 10;
				//muzzleVelocity = muzzleVelocity + UnityEngine.Random.Range(-1f, 1f) * 1;
				//Debug.Log("shotgun velocity " + muzzleVelocity);
			}
			else 
			{
				if(((HumanCharacter)Attacker).IsHipAiming)
				{
					//TODO: hit firing scatter will be determined later by skills
					scatterValue = (1 - Barrel.Accuracy) / 10 * 2;
				}
				else
				{
					scatterValue = (1 - Barrel.Accuracy) / 10;
				}

				//now add swing/movement penalty
				scatterValue += Attacker.MyAI.WeaponSystem.GetTurnMoveScatter() * (1 - Barrel.Handling)/10;
			}

			//now add low energy penalty for player
			if(Attacker.MyAI.ControlType == AIControlType.Player)
			{
				scatterValue = scatterValue * (1 + (1 - Attacker.MyStatus.Energy / Attacker.MyStatus.MaxEnergy) * 2);
			}

			Vector3 scatter = new Vector3(UnityEngine.Random.Range(-1 * scatterValue, scatterValue), UnityEngine.Random.Range(-1 * scatterValue, scatterValue), 1);

			//Debug.Log(scatterValue + ", " + scatter.magnitude);

			GameObject o = GameObject.Instantiate(Resources.Load("Bullet")) as GameObject;
			Bullet bullet = o.GetComponent<Bullet>();
			bullet.AmmoItem = _ammoItem;
			bullet.transform.position = _bulletOrigin.transform.position;
			Vector3 bulletTarget = _bulletOrigin.transform.TransformPoint(_bulletOrigin.transform.localPosition + scatter);
			bullet.transform.LookAt(bulletTarget);
			bullet.Fire(bullet.transform.forward * muzzleVelocity + Attacker.GetCharacterVelocity(), this, Barrel.Range, 0.7f, Barrel.Impact, 0.3f);//TODO: critial hit chance will be covered in skills
		}

		StartCoroutine(LightsOut(_light));
	}

	IEnumerator LightsOut(Light light)
	{
		yield return new WaitForSeconds(0.05f);
		light.enabled = false;
	}


}
