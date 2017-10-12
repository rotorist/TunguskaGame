using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;

public class MutantCharacter : Character
{
	public LeftHandIKControl MyLeftHandIK;
	public AimIK MyAimIK;
	public HeadIKControl MyHeadIK;

	public MutantAnimStateBase CurrentAnimState;


	public MutantUpperBodyStates UpperBodyState;
	public bool IsRangedCapable;

	public string CurrentAction;


	public override bool IsAlive 
	{
		get 
		{
			return this.MyStatus.Health > 0;
		}
	}



	private Character _strangleTarget;
	private int _meleeStrikeStage;//0, 1, 2, 3
	private int _meleeSide; //0=left, 1=right
	private float _twitchTimer;
	private float _footStepTimer;
	private float _footStepTimeout;


	void Update()
	{
		CurrentAnimState.Update();

		_footStepTimer += Time.deltaTime;
		_footStepTimeout = 0.1f;


		if(MyStatus.Health > 0)
		{
			UpdateLookDirection();

			UpdateDestBodyAngle();

			MyStatus.UpdateBodyStatusEveryone();
			if(MyStatus.Health <= 0)
			{
				MyStatus.Health = 0;
				OnDeath(Vector3.zero);
			}

			MyAI.AlwaysPerFrameUpdate();
		}

		bool isAlert = IsAlert();

		if(isAlert)
		{
			if(MyAnimator.GetInteger("AlertLevel") <= 1)
			{
				MyAnimator.SetInteger("AlertLevel", 2);
				//Debug.LogError("setting alert level to TWO");
			}
		}
		else
		{
			if(MyAnimator.GetInteger("AlertLevel") > 1)
			{
				MyAnimator.SetInteger("AlertLevel", 1);
				//Debug.LogError("setting alert level to ONE");
			}
		}

		if(ActionState == HumanActionStates.Twitch)
		{
			if(_twitchTimer > 0)
			{
				_twitchTimer -= Time.deltaTime;
			}
			else
			{
				_twitchTimer = 0;
				OnInjuryRecover();
			}
		}

		GoapAction action = MyAI.GetCurrentAction();
		if(action != null)
		{
			CurrentAction = action.Name;
		}
		else
		{
			CurrentAction = "NONE";
		}

		UpdateFading();
	}

	public override void Initialize()
	{
		//load aim target and look target
		GameObject aimTarget = (GameObject)GameObject.Instantiate(Resources.Load("IKAimTargetRoot"));
		GameObject lookTarget = (GameObject)GameObject.Instantiate(Resources.Load("IKLookTarget"));
		AimTargetRoot = aimTarget.transform;
		AimTarget = AimTargetRoot.Find("IKAimTarget").transform;
		LookTarget = lookTarget.transform;


		LoadCharacterModel(this.CharacterID);




		this.MyEventHandler = GetComponent<CharacterEventHandler>();

		this.Destination = transform.position;
		MyNavAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		UpperBodyState = MutantUpperBodyStates.Idle;
		ActionState = HumanActionStates.None;


		this.MyStatus = GetComponent<CharacterStatus>();
		this.MyStatus.ParentCharacter = this;
		//this.MyStatus.Initialize();


		this.MyStatus.RunSpeedModifier = 1f;

		this.Stealth = new CharacterStealth(this);

		this.Inventory = new CharacterInventory();
		this.CharacterAudio = GetComponent<AudioSource>();

		//each time a human char is initialized it's added to NPC manager's list of human characters to keep track of
		GameManager.Inst.NPCManager.AddMutantCharacter(this);

		CurrentAnimState = new MutantAnimStateIdle(this);
		//SendCommand(CharacterCommands.Unarm);

		this.GoapID = this.MyReference.GoapID;
		MyAI = GetComponent<AI>();
		MyAI.Initialize(this);

		this.ArmorSystem = new ArmorSystem(this);

		_meleeStrikeStage = 0;


	
	}

	public override void LoadCharacterModel (string prefabName)
	{
		if(this.Model != null)
		{
			GameObject.Destroy(this.Model);
		}

		GameObject o = GameObject.Instantiate(Resources.Load(prefabName)) as GameObject;
		o.transform.parent = this.transform;
		o.transform.localPosition = Vector3.zero;
		o.transform.localEulerAngles = Vector3.zero;

		this.MyAnimator = o.transform.GetComponent<Animator>();
		this.MyReference = o.transform.GetComponent<CharacterReference>();
		this.MyAnimEventHandler = o.transform.GetComponent<AnimationEventHandler>();

		this.MyReference.ParentCharacter = this;
		this.MyReference.LiveCollider = transform.GetComponent<CapsuleCollider>();
		if(this.MyReference.DeathCollider != null)
		{
			this.MyReference.DeathCollider.GetComponent<DeathCollider>().ParentCharacter = this;
			this.MyReference.DeathCollider.enabled = false;
		}

		if(this.MyReference.FixedMeleeLeft != null)
		{
			this.MyReference.FixedMeleeLeft.Attacker = this;
			this.MyReference.FixedMeleeLeft.Rebuild(null, null);
		}

		if(this.MyReference.FixedMeleeRight != null)
		{
			this.MyReference.FixedMeleeRight.Attacker = this;
			this.MyReference.FixedMeleeRight.Rebuild(null, null);
		}

		this.MyAimIK = o.transform.GetComponent<AimIK>();
		this.MyAimIK.solver.target = AimTarget;
		this.MyAimIK.solver.IKPositionWeight = 0;
		this.MyAimIK.solver.SmoothDisable();

		this.MyLeftHandIK = o.transform.GetComponent<LeftHandIKControl>();
		this.MyLeftHandIK.Initialize();

		this.MyHeadIK = o.transform.GetComponent<HeadIKControl>();
		this.MyHeadIK.Initialize();
		this.MyHeadIK.LookTarget = LookTarget;


		//setup animator parameters initial values
		this.MyAnimator.SetInteger("AlertLevel", 0);

		this.Model = o;
		this.Model.name = prefabName;

		if(this.MyAI != null)
		{
			this.MyAI.BlackBoard.EquippedWeapon = null;
		}


		//subscribe events
		this.MyAnimEventHandler.OnMeleeStrikeLeftFinish -= OnMeleeStrikeFinish;
		this.MyAnimEventHandler.OnMeleeStrikeLeftFinish += OnMeleeStrikeFinish;

		this.MyAnimEventHandler.OnMeleeStrikeRightFinish -= OnMeleeStrikeFinish;
		this.MyAnimEventHandler.OnMeleeStrikeRightFinish += OnMeleeStrikeFinish;

		this.MyAnimEventHandler.OnMeleeBlocked -= OnMeleeBlocked;
		this.MyAnimEventHandler.OnMeleeBlocked += OnMeleeBlocked;

		this.MyAnimEventHandler.OnHitReover -= OnInjuryRecover;
		this.MyAnimEventHandler.OnHitReover += OnInjuryRecover;

		this.MyAnimEventHandler.OnAnimationActionEnd -= OnAnimationActionEnd;
		this.MyAnimEventHandler.OnAnimationActionEnd += OnAnimationActionEnd;

		this.MyAnimEventHandler.OnMeleeStrikeHalfWay -= OnMeleeStrikeHalfWay;
		this.MyAnimEventHandler.OnMeleeStrikeHalfWay += OnMeleeStrikeHalfWay;

		this.MyAnimEventHandler.OnStartStrangle -= OnStartStrangle;
		this.MyAnimEventHandler.OnStartStrangle += OnStartStrangle;
		this.MyAnimEventHandler.OnEndStrangle -= OnEndStrangle;
		this.MyAnimEventHandler.OnEndStrangle += OnEndStrangle;

		this.MyAnimEventHandler.OnFootStep -= OnFootStep;
		this.MyAnimEventHandler.OnFootStep += OnFootStep;
	}

	public override void Unhook ()
	{
		this.MyAnimEventHandler.OnMeleeStrikeLeftFinish -= OnMeleeStrikeFinish;

		this.MyAnimEventHandler.OnMeleeStrikeRightFinish -= OnMeleeStrikeFinish;

		this.MyAnimEventHandler.OnMeleeBlocked -= OnMeleeBlocked;

		this.MyAnimEventHandler.OnHitReover -= OnInjuryRecover;

		this.MyAnimEventHandler.OnAnimationActionEnd -= OnAnimationActionEnd;

		this.MyAnimEventHandler.OnMeleeStrikeHalfWay -= OnMeleeStrikeHalfWay;

		this.MyAnimEventHandler.OnStartStrangle -= OnStartStrangle;

		this.MyAnimEventHandler.OnEndStrangle -= OnEndStrangle;

		this.MyAnimEventHandler.OnFootStep -= OnFootStep;

	}

	public override void PlayVocal (VocalType vocalType)
	{
		if(vocalType == VocalType.Surprise)
		{
			if(UnityEngine.Random.value > 0f)
			{
				int choice = UnityEngine.Random.Range(1, 4);
				CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(MyReference.VocalSet.ToString() + "_surprise" + choice), 0.5f);
			}
		}

		if(vocalType == VocalType.Injury)
		{
			if(UnityEngine.Random.value > 0f)
			{
				int choice = UnityEngine.Random.Range(1, 4);
				CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(MyReference.VocalSet.ToString() + "_injury" + choice), 0.6f);
			}
		}

		if(vocalType == VocalType.Death)
		{
			if(UnityEngine.Random.value > 0f)
			{
				int choice = UnityEngine.Random.Range(1, 4);
				CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(MyReference.VocalSet.ToString() + "_death" + choice), 0.6f);
			}
		}
	}


	public override Vector3 GetCharacterVelocity()
	{
		return  MyNavAgent.velocity;
	}

	public override void SendCommand (CharacterCommands command)
	{
		if(command == CharacterCommands.PlayAnimationAction)
		{
			CurrentAnimState = new MutantAnimStateAction(this);
			IsBodyLocked = true;
		}

		if(command == CharacterCommands.AnimationActionDone)
		{
			//Debug.Log("Action Done");
			IsBodyLocked = false;
		}





		if(!IsBodyLocked && !IsMoveLocked)
		{
			CurrentAnimState.SendCommand(command);
		}

		if(IsBodyLocked)
		{
			return;
		}

		/*
		if(command == CharacterCommands.IdleAction)
		{
			if(UnityEngine.Random.value > 0.5f)
			{
				this.MyAnimator.SetTrigger("Agonize");
			}
			else
			{
				this.MyAnimator.SetTrigger("Convulse");
			}
		}
		*/

		if(command == CharacterCommands.RunningAttack)
		{
			this.MyAnimator.SetTrigger("RunningAttack");
			_meleeSide = 1;
			ActionState = HumanActionStates.Melee;
		}

		if(command == CharacterCommands.LeftAttack)
		{
			this.MyAnimator.SetTrigger("LeftAttack");
			_meleeSide = 0;
			ActionState = HumanActionStates.Melee;
		}

		if(command == CharacterCommands.RightAttack)
		{
			this.MyAnimator.SetTrigger("RightAttack");
			_meleeSide = 1;
			ActionState = HumanActionStates.Melee;
		}

		if(command == CharacterCommands.QuickAttack)
		{
			this.MyAnimator.SetTrigger("QuickAttack");
			_meleeSide = 1;
			ActionState = HumanActionStates.Melee;
		}

		if(command == CharacterCommands.Bite)
		{
			if(ActionState != HumanActionStates.None || MyAI.BlackBoard.TargetEnemy == null)
			{
				return;
			}




			//Debug.Log("Start biting");
			Vector3 lineOfSight = MyAI.BlackBoard.TargetEnemy.transform.position - transform.position;


			//stop movement
			SendCommand(CharacterCommands.Idle);
			IsBodyLocked = true;
			MyNavAgent.enabled = false;

			//place my position right behind target
			transform.position = MyAI.BlackBoard.TargetEnemy.transform.position - lineOfSight.normalized * 1f;

			//align my facing direction to enemy's
			lineOfSight = new Vector3(lineOfSight.x, 0, lineOfSight.z);
			Quaternion rotation = Quaternion.LookRotation(lineOfSight);
			transform.rotation = rotation;

			MyAnimator.SetTrigger("Bite");
			_strangleTarget = MyAI.BlackBoard.TargetEnemy;


			ActionState = HumanActionStates.Strangle;
		}
	}

	public override bool SendDamage(Damage damage, Vector3 hitNormal, Character attacker, Weapon attackerWeapon)
	{


		OnInjury(hitNormal, damage.IsCritical);
		MyAI.Sensor.OnTakingDamage(attacker);

		float finalDamage = 0;
		if(damage.Type == DamageType.Bullet)
		{
			finalDamage = damage.KineticDamage;
		}
		else if(damage.Type == DamageType.Explosive)
		{
			finalDamage = damage.BlastDamage;
		}

		if(damage.IsCritical)
		{
			PlayVocal(VocalType.Injury);
			finalDamage *= 1.5f;
			if(this.Inventory.HeadSlot != null)
			{
				float armorRating = (float)this.Inventory.HeadSlot.GetAttributeByName("Armor").Value;
				float coverage = (float)this.Inventory.HeadSlot.GetAttributeByName("Coverage").Value;
				float chance = UnityEngine.Random.value;
				if(chance < coverage)
				{
					//covered, calculate armor rating vs penetration
					if(damage.Penetration >= armorRating)
					{
						//penetrated the armor
						finalDamage = finalDamage * Mathf.Clamp01((damage.Penetration - armorRating) / armorRating);
					}
					else
					{
						//not penetrated
						return true;
					}
				}


			}
		}
		else if(this.Inventory.ArmorSlot != null)
		{
			float armorRating = (float)this.Inventory.ArmorSlot.GetAttributeByName("Armor").Value;
			float coverage = (float)this.Inventory.ArmorSlot.GetAttributeByName("Coverage").Value;
			float chance = UnityEngine.Random.value;
			if(chance < coverage)
			{
				//covered, calculate armor rating vs penetration
				if(damage.Penetration >= armorRating)
				{
					//penetrated the armor
					finalDamage = finalDamage * Mathf.Clamp01((damage.Penetration - armorRating) / armorRating);
				}
				else
				{
					//not penetrated
					return true;
				}
			}
			else
			{
				//uncovered area are less vulnerable. this does NOT apply to critical (head) hits.
				finalDamage *= 0.5f;
			}

		}
			

		if(finalDamage > 0 && damage.Bleeding > 0)
		{
			MyStatus.AddBleeding(damage.Bleeding);
		}

		MyStatus.Health -= finalDamage;

		DeathReason = damage.Type;

		if(MyStatus.Health <= 0)
		{
			MyStatus.Health = 0;
			OnDeath(hitNormal);
		}


		return false;
	}

	public override MeleeBlockType SendMeleeDamage (Damage damage, Vector3 hitNormal, Character attacker, float knockBackChance)
	{
		MyEventHandler.TriggerOnTakingHit();
		float attackerAngle = Vector3.Angle(transform.forward, transform.position - attacker.transform.position);
		if(ActionState == HumanActionStates.None && attackerAngle > 135 && UnityEngine.Random.value < 0.4f)
		{
			MyAnimator.SetTrigger("BlockSuccess");
			return MeleeBlockType.SoftArmor;
		}
		else
		{
			OnInjury(hitNormal, UnityEngine.Random.value <= knockBackChance);
			MyAI.Sensor.OnTakingDamage(attacker);
			float finalDamage = damage.SharpDamage + damage.BluntDamage;

			if(damage.IsCritical)
			{
				finalDamage *= 1.5f;
			}

			if(this.Inventory.ArmorSlot != null)
			{
				float padding = (float)this.Inventory.ArmorSlot.GetAttributeByName("Padding").Value;
				float armorRating = (float)this.Inventory.ArmorSlot.GetAttributeByName("Armor").Value;
				float coverage = (float)this.Inventory.ArmorSlot.GetAttributeByName("Coverage").Value;
				float chance = UnityEngine.Random.value;
				if(chance < coverage)
				{
					//when hitting armor, only blunt damage applies
					finalDamage = damage.BluntDamage - padding + damage.SharpDamage - armorRating;
					if(finalDamage < 0)
					{
						finalDamage = 0;
					}
				}
				else
				{
					finalDamage = damage.SharpDamage + damage.BluntDamage;
				}

			}

			if(finalDamage > 0)
			{
				PlayVocal(VocalType.Injury);
			}

			if(finalDamage > 0 && damage.Bleeding > 0)
			{
				MyStatus.AddBleeding(damage.Bleeding);
			}

			MyStatus.Health -= finalDamage;
			DeathReason = damage.Type;

			if(MyStatus.Health <= 0)
			{
				MyStatus.Health = 0;
				OnDeath(hitNormal);
			}

		}


		return MeleeBlockType.NoBlock;
	}



	public override void SendDelayCallBack (float delay, DelayCallBack callback, object parameter)
	{
		//StartCoroutine(WaitAndCallback(delay, callback, parameter));
	}

	public override void OnSuccessfulHit (Character target)
	{
		SendCommand(CharacterCommands.AnimationActionDone);
	}


	public bool IsAlert()
	{
		//Debug.Log(MyAI.BlackBoard.GuardLevel);
		if(MyAI.BlackBoard.GuardLevel > 1)
		{
			return true;
		}

		return false;
	}


	public void OnInjury(Vector3 normal, bool isKnockBack)
	{
		if(!isKnockBack)
		{
			if(normal != Vector3.zero)
			{
				normal = new Vector3(normal.x, 0, normal.z);
				float bodyAngle = Vector3.Angle(transform.right, normal * -1);

				if(bodyAngle <= 90)
				{
					this.MyAnimator.SetInteger("HitType", 0);
				}
				else
				{
					this.MyAnimator.SetInteger("HitType", 1);
				}
			}


			this.MyAnimator.SetTrigger("Hit");

			if(MyAnimator.GetBool("IsAiming"))
			{
				MyAimIK.solver.IKPositionWeight = 0;
				MyAimIK.solver.SmoothEnable(1);
			}


			MyHeadIK.Weight = 0;
			MyHeadIK.SmoothEnable(1);
		}
		else
		{
			//if normal is in front of character then knock back, other knock forward

			normal = new Vector3(normal.x, 0, normal.z);
			float bodyAngle = Vector3.Angle(transform.forward, normal * -1);

			Vector3 lookDir = normal;


			if(bodyAngle <= 90)
			{
				lookDir = normal * -1;
				MyAI.BlackBoard.AnimationAction = AnimationActions.KnockBack;
				MyAI.BlackBoard.ActionMovementDest = transform.position + normal.normalized * 1; //transform.position - transform.forward * 1;
				MyAI.BlackBoard.ActionMovementSpeed = 1.5f;
			}
			else
			{
				MyAI.BlackBoard.AnimationAction = AnimationActions.KnockForward;
				MyAI.BlackBoard.ActionMovementDest = transform.position + normal.normalized * 1.2f; //transform.position + transform.forward * 1.5f;
				MyAI.BlackBoard.ActionMovementSpeed = 2f;
			}

			lookDir = new Vector3(lookDir.x, 0, lookDir.z);
			transform.rotation = Quaternion.LookRotation(lookDir);

			SendCommand(CharacterCommands.PlayAnimationAction);
		}



		_twitchTimer = 0.5f;
		ActionState = HumanActionStates.Twitch;
	}

	public void OnInjuryRecover()
	{
		_twitchTimer = 0;
		ActionState = HumanActionStates.None;
	}

	public void OnDeath(Vector3 normal)
	{
		PlayVocal(VocalType.Death);
		MyAI.OnDeath();
		Stealth.OnDeath();
		float posture = UnityEngine.Random.Range(0.1f, 200)/200f;

		int direction = 1;
		if(normal != Vector3.zero)
		{
			normal = new Vector3(normal.x, 0, normal.z);

			float angleRight = Vector3.Angle(normal, transform.right);
			float angleForward = Vector3.Angle(normal, transform.forward);


			if(angleRight < 60)
			{
				direction = 3;
			}
			else if(angleRight > 120)
			{
				direction = 2;
			}
			else
			{
				if(UnityEngine.Random.value > 0.5f)
				{
					direction = 0;
				}
				else
				{
					direction = 1;
				}
			}
		}

		this.MyAnimator.SetInteger("DeathDirection", direction);
		this.MyAnimator.SetFloat("Blend", posture);
		this.MyAnimator.SetBool("IsDead", true);

		CurrentAnimState = new MutantAnimStateDeath(this);
		IsBodyLocked = true;
		MyAimIK.solver.SmoothDisable(9);
		MyLeftHandIK.SmoothDisable(12);
		MyHeadIK.SmoothDisable(9);
		MyNavAgent.enabled = false;
		MyReference.LiveCollider.enabled = false;
		MyReference.DeathCollider.enabled = true;
		MyReference.DeathCollider.gameObject.layer = 18;

		/*
		CapsuleCollider collider = GetComponent<CapsuleCollider>();
		collider.height = 0.5f;
		collider.radius = 0.6f;
		collider.center = new Vector3(0, 0, 0);
		collider.isTrigger = true;
		*/

		Unhook();
	}


	public void OnMeleeStrikeHalfWay()
	{
		ActionState = HumanActionStates.Melee;
		if(_meleeSide == 0)
		{
			MyReference.FixedMeleeLeft.GetComponent<MeleeWeapon>().SwingStart();
		}
		else if(_meleeSide == 1)
		{
			MyReference.FixedMeleeRight.GetComponent<MeleeWeapon>().SwingStart();
		}


	}

	public void OnMeleeStrikeFinish()
	{
		MyReference.FixedMeleeRight.GetComponent<MeleeWeapon>().SwingStop();
		MyReference.FixedMeleeLeft.GetComponent<MeleeWeapon>().SwingStop();
		ActionState = HumanActionStates.None;
	}

	public void OnMeleeBlocked()
	{
		Debug.Log("Blocked!");
		ActionState = HumanActionStates.None;
		MyAnimator.SetTrigger("Blocked");
	}

	public void OnAnimationActionEnd()
	{
		SendCommand(CharacterCommands.AnimationActionDone);
	}

	public void OnStartStrangle()
	{
		if(_strangleTarget == null)
		{
			MyAnimator.SetTrigger("CancelBite");
			MyAnimEventHandler.TriggerOnEndStrangle();
			return;
		}

		Character target = _strangleTarget;
		target.SendCommand(CharacterCommands.Idle);
		target.SendCommand(CharacterCommands.StopAim);
		target.IsBodyLocked = true;



		//align enemy facing direction to player's
		Vector3 lineOfSight = target.transform.position - transform.position;
		lineOfSight = new Vector3(lineOfSight.x, 0, lineOfSight.z);
		Quaternion rotation = Quaternion.LookRotation(lineOfSight);
		target.transform.rotation = rotation;
		target.MyAnimator.SetTrigger("GetStrangled");
		target.MyStatus.AddBleeding(0.5f);


		Stealth.SetNoiseLevel(10, 0.8f);
	}

	public void OnEndStrangle()
	{
		if(ActionState == HumanActionStates.Strangle)
		{
			ActionState = HumanActionStates.None;

			Vector3 lineOfSight = _strangleTarget.transform.position - transform.position;
			transform.position = _strangleTarget.transform.position - lineOfSight.normalized;

			if(_strangleTarget != null && _strangleTarget.IsAlive)
			{
				_strangleTarget.IsBodyLocked = false;
				_strangleTarget.MyAnimator.SetTrigger("Cancel");
				_strangleTarget = null;
			}


			IsBodyLocked = false;
			MyNavAgent.enabled = true;
			_strangleTarget = null;
		}
	}

	public void OnCancelStrangle()
	{
		MyAnimator.SetTrigger("CancelBite");
		OnEndStrangle();
	}

	public void OnFootStep()
	{

		string currentClip = "";
		if(this.CharacterAudio.clip != null)
		{
			currentClip = this.CharacterAudio.clip.name.Substring(0, 3);
		}
		else
		{
			currentClip = "";
		}

		if(_footStepTimer < _footStepTimeout || currentClip == "run" || currentClip == "walk")
		{
			return;
		}
		else
		{
			_footStepTimer = 0;
		}


		if(MyReference.IsHeavy)
		{
			int choice = UnityEngine.Random.Range(1, 5);
			this.CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("run_mutant" + choice), 0.2f);
		}
		else
		{
			int choice = UnityEngine.Random.Range(1, 7);
			this.CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("run_dirt" + choice), 0.2f);
		}
	}






	private void UpdateLookDirection()
	{
		//get the direction of look on the xz plane
		Vector3 lookDir = LookTarget.position - transform.position;
		lookDir = new Vector3(lookDir.x, 0, lookDir.z);
		float lookBodyAngle = Vector3.Angle(lookDir, transform.right);

		//manipulate lookBodyAngle so it's not linear
		//float controlValue = lookBodyAngle * lookBodyAngle / 100;
		this.MyAnimator.SetFloat("LookBodyAngle", lookBodyAngle);



	}

	private void UpdateDestBodyAngle()
	{
		//get the direction of destination on the xz plane
		Vector3 destDir = GetCharacterVelocity(); //this.Destination.Value - transform.position;
		destDir = new Vector3(destDir.x, 0, destDir.z);
		float destBodyAngle = Vector3.Angle(destDir, transform.right);

		//manipulate destBodyAngle so it's not linear
		float controlValue = destBodyAngle;
		this.MyAnimator.SetFloat("DestBodyAngle", controlValue);
		//Debug.Log(destBodyAngle.ToString() + " " + controlValue);
	}


	private void HideCharacter(bool isInstant)
	{
		Renderer [] childRenderers = GetComponentsInChildren<Renderer>();
		List<Renderer> fadingRenders = new List<Renderer>();
		foreach(Renderer renderer in MyReference.FadingRenderers)
		{
			fadingRenders.Add(renderer);
		}
		foreach(Renderer r in childRenderers)
		{
			if(fadingRenders.Contains(r))
			{
				GameManager.Inst.MaterialManager.StartFadingMaterial(r, isInstant, false, 2);
			}
			else
			{
				r.enabled = false;
			}
		}
	}

	private void ShowCharacter()
	{
		Renderer [] childRenderers = GetComponentsInChildren<Renderer>();
		List<Renderer> fadingRenders = new List<Renderer>();
		foreach(Renderer renderer in MyReference.FadingRenderers)
		{
			fadingRenders.Add(renderer);
		}
		foreach(Renderer r in childRenderers)
		{
			if(fadingRenders.Contains(r))
			{
				GameManager.Inst.MaterialManager.StartUnfadingMaterial(r, false, false, 4);
				r.enabled = true;
			}
			else
			{
				r.enabled = true;
			}
		}
	}

	private void UpdateFading()
	{
		float playerDist = Vector3.Distance(transform.position, GameManager.Inst.PlayerControl.SelectedPC.transform.position);
		if(playerDist < 70 && MyAI.ControlType != AIControlType.Player)
		{
			Vector3 playerLineOfSight = GameManager.Inst.PlayerControl.SelectedPC.MyReference.Flashlight.transform.forward;//GameManager.Inst.PlayerControl.SelectedPC.LookTarget.transform.position - GameManager.Inst.PlayerControl.SelectedPC.transform.position;
			if(GameManager.Inst.PlayerControl.SelectedPC.MyCC.velocity.magnitude > 0.1f)
			{
				playerLineOfSight = GameManager.Inst.PlayerControl.SelectedPC.transform.forward;
			}
			playerLineOfSight = new Vector3(playerLineOfSight.x, 0, playerLineOfSight.z);
			float playerAngle = Vector3.Angle(playerLineOfSight, transform.position - GameManager.Inst.PlayerControl.SelectedPC.transform.position);
			if(playerAngle > 80 && playerDist > 1.5f && MyStatus.Health > 0)
			{
				IsOutOfSight = true;
			}
			else
			{
				IsOutOfSight = false;
			}

			if(!IsHidden)
			{
				if(IsInHiddenBuilding)
				{
					HideCharacter(true);
					IsHidden = true;
				}
				else if(IsOutOfSight)
				{
					HideCharacter(false);
					IsHidden = true;
				}

			}
			else if(IsHidden && !IsInHiddenBuilding && !IsOutOfSight)
			{

				ShowCharacter();

				IsHidden = false;
			}
		}

		float speed = GetCharacterVelocity().magnitude;

		if(IsHidden && speed > 0.2f)
		{
			if(MyNoiseMarker == null)
			{
				MyNoiseMarker = GameObject.Instantiate(Resources.Load("NoiseMarker")) as GameObject;
			}
		}
		else
		{
			if(MyNoiseMarker != null)
			{
				GameObject.Destroy(MyNoiseMarker);
				MyNoiseMarker = null;
			}
		}

		if(MyNoiseMarker != null)
		{
			MyNoiseMarker.transform.position = transform.position + new Vector3(0, 2f, 0);
		}
	}
}
