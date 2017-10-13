using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;

public class HumanCharacter : Character
{
	public LeftHandIKControl MyLeftHandIK;
	public AimIK MyAimIK;
	public HeadIKControl MyHeadIK;

	public CharacterMarkerSet Markers;

	public CharacterController MyCC;

	public HumanAnimStateBase CurrentAnimState;

	public HumanUpperBodyStates UpperBodyState;
	public string CurrentGoal;
	public string CurrentAction;
	public Door CurrentDoor;

	public Trader Trader;

	public override bool IsAlive 
	{
		get 
		{
			return this.MyStatus.Health > 0;
		}
	}

	public bool IsHipAiming
	{
		get
		{
			return _isHipAiming;
		}
	}


	private bool _isHipAiming;
	private bool _isThrowing;
	private bool _isSwitchingWeapon;
	private Vector3 _throwTarget;
	private Vector3 _throwDir;
	private ThrownObject _thrownObjectInHand;
	private Tool _toolInHand;
	private Character _strangleTarget;
	private Item _weaponToSwitch;
	private bool _isLowThrow;
	private int _meleeStrikeStage;//0, 1, 2
	private bool _isComboAttack;
	private bool _layerDState;//false = decreasing; true = increasing
	private float _blockTimer;
	private bool _isDeathTriggered;
	private float _twitchTimer;
	private int _voiceStyle;
	private float _footStepTimer;
	private float _footStepTimeout;
	private GameObject _backpackProp;
	private float _fadeTimer;

	#region Utilities

	void Update()
	{
		CurrentAnimState.Update();

		if(MyStatus.Health > 0)
		{
			UpdateLookDirection();

			UpdateDestBodyAngle();

			MyStatus.UpdateBodyStatusEveryone();
			if(GameManager.Inst.PlayerControl.SelectedPC == this)
			{
				MyStatus.UpdateBodyStatusPlayer();


			}

			_footStepTimer += Time.deltaTime;
			_footStepTimeout = 0.1f;

			MyAI.AlwaysPerFrameUpdate();

		}

		if(MyStatus.Health <= 0 && !_isDeathTriggered)
		{
			MyStatus.Health = 0;
			Debug.LogError("Human Death");
			OnDeath();
		}

		if(ActionState == HumanActionStates.Block)
		{
			_blockTimer += Time.deltaTime;
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

		GoapGoal goal = MyAI.GetCurrentGoal();
		if(goal != null)
		{
			CurrentGoal = goal.Name;
		}
		else
		{
			CurrentGoal = "NONE";
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

		if(MyReference.Flashlight != null)
		{
			if(UpperBodyState == HumanUpperBodyStates.Aim || UpperBodyState == HumanUpperBodyStates.HalfAim)
			{
				MyReference.Flashlight.transform.LookAt(AimTarget);
			}

		}

	}

	

	
	void LateUpdate() 
	{
		//adjust aim direction for recoil
		if(this.MyReference.CurrentWeapon != null && UpperBodyState == HumanUpperBodyStates.Aim)
		{

			Vector3 originalDir = AimTransform.transform.forward;
			Vector3 dir = new Vector3(originalDir.x, 0, originalDir.z);
			MyAimIK.solver.axis = Vector3.Lerp(MyAimIK.solver.transform.InverseTransformDirection(originalDir), MyAimIK.solver.transform.InverseTransformDirection(dir), Time.deltaTime * 0.1f);

			//recover from recoil
			float recoverRate = 8 * (1 - (this.MyStatus.ArmFatigue / this.MyStatus.MaxArmFatigue));
			AimTarget.localPosition = Vector3.Lerp(AimTarget.localPosition, new Vector3(0, 0, 2), Time.deltaTime * recoverRate);

		}

		

	}

	void FixedUpdate()
	{
		CurrentAnimState.FixedUpdate();
	}



	#endregion

	#region overrides

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
		UpperBodyState = HumanUpperBodyStates.Idle;
		ActionState = HumanActionStates.None;



		Markers = new CharacterMarkerSet();



		this.MyStatus = GetComponent<CharacterStatus>();
		this.MyStatus.Initialize();
		this.MyStatus.ParentCharacter = this;

		this.Stealth = new CharacterStealth(this);

		if(this.Inventory == null)
		{
			this.Inventory = new CharacterInventory();
		}
		this.PresetInventory = transform.GetComponent<PresetInventory>();

		this.MyJobs = new List<NPCJobs>();
		this.CharacterAudio = GetComponent<AudioSource>();
		_voiceStyle = UnityEngine.Random.Range(1, 3);
		//_voiceStyle = 1;

		//each time a human char is initialized it's added to NPC manager's list of human characters to keep track of
		GameManager.Inst.NPCManager.AddHumanCharacter(this);

		CurrentAnimState = new HumanAnimStateIdle(this);
		//SendCommand(CharacterCommands.Unarm);

		this.GoapID = this.MyReference.GoapID;
		MyAI = GetComponent<AI>();
		MyAI.Initialize(this);

		MyCC = GetComponent<CharacterController>();

		this.ArmorSystem = new ArmorSystem(this);

		_meleeStrikeStage = 0;
		_isLowThrow = false;

		Trader = GetComponent<Trader>();
		if(Trader != null)
		{
			MyJobs.Add(NPCJobs.Trader);
			Trader.Initialize();
		}
		else if(IsCommander)
		{
			MyJobs.Add(NPCJobs.Commander);
		}
		else
		{
			MyJobs.Add(NPCJobs.None);
		}
	}


	public override void LoadCharacterModel(string prefabName)
	{
		//Debug.Log("loading char " + prefabName);
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

		if(this.MyReference.RightFoot != null)
		{
			this.MyReference.RightFoot.Attacker = this;
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
		this.MyAnimator.SetBool("IsAiming", false);
		this.MyAnimator.SetBool("IsSneaking", false);

		this.Model = o;
		this.Model.name = prefabName;

		if(this.MyAI != null)
		{
			this.MyAI.BlackBoard.EquippedWeapon = null;


		}

		//load flashlight
		if(this.MyReference.FlashlightMount != null)
		{
			GameObject flo;
			if(this.MyReference.VocalSet == VocalSet.player)
			{

				flo = GameObject.Instantiate(Resources.Load("FlashLightPlayer")) as GameObject;

			}
			else
			{
				flo = GameObject.Instantiate(Resources.Load("FlashLight")) as GameObject;

			}
			this.MyReference.FlashlightMount.transform.localEulerAngles = new Vector3(18, 0, 0);
			MyReference.Flashlight = flo.GetComponent<FlashLight>();
			flo.transform.parent = this.MyReference.FlashlightMount.transform;
			flo.transform.localPosition = Vector3.zero;
			flo.transform.localEulerAngles = Vector3.zero;
		}


		//subscribe events
		this.MyAnimEventHandler.OnLongGunPullOutFinish -= OnLongGunPullOutFinish;
		this.MyAnimEventHandler.OnLongGunPullOutFinish += OnLongGunPullOutFinish;

		this.MyAnimEventHandler.OnLongGunPutAwayFinish -= OnLongGunPutAwayFinish;
		this.MyAnimEventHandler.OnLongGunPutAwayFinish += OnLongGunPutAwayFinish;

		this.MyAnimEventHandler.OnPistolPullOutFinish -= OnPistolPullOutFinish;
		this.MyAnimEventHandler.OnPistolPullOutFinish += OnPistolPullOutFinish;

		this.MyAnimEventHandler.OnPistolPutAwayFinish -= OnPistolPutAwayFinish;
		this.MyAnimEventHandler.OnPistolPutAwayFinish += OnPistolPutAwayFinish;

		this.MyAnimEventHandler.OnGrenadePullOutFinish -= OnGrenadePullOutFinish;
		this.MyAnimEventHandler.OnGrenadePullOutFinish += OnGrenadePullOutFinish;

		this.MyAnimEventHandler.OnMeleePullOutFinish -= OnMeleePullOutFinish;
		this.MyAnimEventHandler.OnMeleePullOutFinish += OnMeleePullOutFinish;
		this.MyAnimEventHandler.OnMeleePutAwayFinish -= OnMeleePutAwayFinish;
		this.MyAnimEventHandler.OnMeleePutAwayFinish += OnMeleePutAwayFinish;

		this.MyAnimEventHandler.OnReloadFinish -= OnReloadFinish;
		this.MyAnimEventHandler.OnReloadFinish += OnReloadFinish;

		this.MyAnimEventHandler.OnThrowFinish -= OnThrowFinish;
		this.MyAnimEventHandler.OnThrowFinish += OnThrowFinish;

		this.MyAnimEventHandler.OnThrowLeaveHand -= OnThrowLeaveHand;
		this.MyAnimEventHandler.OnThrowLeaveHand += OnThrowLeaveHand;

		this.MyAnimEventHandler.OnStartStrangle -= OnStartStrangle;
		this.MyAnimEventHandler.OnStartStrangle += OnStartStrangle;

		this.MyAnimEventHandler.OnEndStrangle -= OnEndStrangle;
		this.MyAnimEventHandler.OnEndStrangle += OnEndStrangle;

		this.MyAnimEventHandler.OnDeath -= OnDeath;
		this.MyAnimEventHandler.OnDeath += OnDeath;

		this.MyAnimEventHandler.OnStrangledDeath -= OnStrangledDeath;
		this.MyAnimEventHandler.OnStrangledDeath += OnStrangledDeath;

		this.MyAnimEventHandler.OnHitReover -= OnInjuryRecover;
		this.MyAnimEventHandler.OnHitReover += OnInjuryRecover;

		this.MyAnimEventHandler.OnSwitchWeapon -= OnSwitchWeapon;
		this.MyAnimEventHandler.OnSwitchWeapon += OnSwitchWeapon;

		this.MyAnimEventHandler.OnFinishTakeObject -= OnTakeObjectFinish;
		this.MyAnimEventHandler.OnFinishTakeObject += OnTakeObjectFinish;
		this.MyAnimEventHandler.OnFinishInteract -= OnFinishInteract;
		this.MyAnimEventHandler.OnFinishInteract += OnFinishInteract;

		this.MyAnimEventHandler.OnMeleeStrikeHalfWay -= OnMeleeStrikeHalfWay;
		this.MyAnimEventHandler.OnMeleeStrikeHalfWay += OnMeleeStrikeHalfWay;

		this.MyAnimEventHandler.OnMeleeComboStageOne -= OnMeleeComboStageOne;
		this.MyAnimEventHandler.OnMeleeComboStageOne += OnMeleeComboStageOne;
		this.MyAnimEventHandler.OnMeleeComboStageTwo -= OnMeleeComboStageTwo;
		this.MyAnimEventHandler.OnMeleeComboStageTwo += OnMeleeComboStageTwo;

		this.MyAnimEventHandler.OnMeleeStrikeLeftFinish -= OnMeleeStrikeLeftFinish;
		this.MyAnimEventHandler.OnMeleeStrikeLeftFinish += OnMeleeStrikeLeftFinish;

		this.MyAnimEventHandler.OnMeleeStrikeRightFinish -= OnMeleeStrikeRightFinish;
		this.MyAnimEventHandler.OnMeleeStrikeRightFinish += OnMeleeStrikeRightFinish;

		this.MyAnimEventHandler.OnMeleeBlockFinish -= OnMeleeBlockFinish;
		this.MyAnimEventHandler.OnMeleeBlockFinish += OnMeleeBlockFinish;

		this.MyAnimEventHandler.OnAnimationActionEnd -= OnAnimationActionEnd;
		this.MyAnimEventHandler.OnAnimationActionEnd += OnAnimationActionEnd;

		this.MyAnimEventHandler.OnFootStep -= OnFootStep;
		this.MyAnimEventHandler.OnFootStep += OnFootStep;
	}

	public override void Unhook()
	{
		this.MyAnimEventHandler.OnLongGunPullOutFinish -= OnLongGunPullOutFinish;

		this.MyAnimEventHandler.OnLongGunPutAwayFinish -= OnLongGunPutAwayFinish;

		this.MyAnimEventHandler.OnPistolPullOutFinish -= OnPistolPullOutFinish;

		this.MyAnimEventHandler.OnPistolPutAwayFinish -= OnPistolPutAwayFinish;

		this.MyAnimEventHandler.OnGrenadePullOutFinish -= OnGrenadePullOutFinish;

		this.MyAnimEventHandler.OnMeleePullOutFinish -= OnMeleePullOutFinish;

		this.MyAnimEventHandler.OnMeleePutAwayFinish -= OnMeleePutAwayFinish;


		this.MyAnimEventHandler.OnReloadFinish -= OnReloadFinish;


		this.MyAnimEventHandler.OnThrowFinish -= OnThrowFinish;

		this.MyAnimEventHandler.OnThrowLeaveHand -= OnThrowLeaveHand;

		this.MyAnimEventHandler.OnStartStrangle -= OnStartStrangle;

		this.MyAnimEventHandler.OnEndStrangle -= OnEndStrangle;

		this.MyAnimEventHandler.OnDeath -= OnDeath;

		this.MyAnimEventHandler.OnStrangledDeath -= OnStrangledDeath;

		this.MyAnimEventHandler.OnHitReover -= OnInjuryRecover;

		this.MyAnimEventHandler.OnSwitchWeapon -= OnSwitchWeapon;

		this.MyAnimEventHandler.OnFinishTakeObject -= OnTakeObjectFinish;
		this.MyAnimEventHandler.OnFinishInteract -= OnFinishInteract;
		this.MyAnimEventHandler.OnMeleeStrikeHalfWay -= OnMeleeStrikeHalfWay;
		this.MyAnimEventHandler.OnMeleeComboStageOne -= OnMeleeComboStageOne;
		this.MyAnimEventHandler.OnMeleeComboStageTwo -= OnMeleeComboStageTwo;

		this.MyAnimEventHandler.OnMeleeStrikeLeftFinish -= OnMeleeStrikeLeftFinish;

		this.MyAnimEventHandler.OnMeleeStrikeRightFinish -= OnMeleeStrikeRightFinish;

		this.MyAnimEventHandler.OnMeleeBlockFinish -= OnMeleeBlockFinish;

		this.MyAnimEventHandler.OnAnimationActionEnd -= OnAnimationActionEnd;

		this.MyAnimEventHandler.OnFootStep -= OnFootStep;

	}
		
	public override void PlayVocal (VocalType vocalType)
	{
		CharacterAudio.Stop();

		bool shouldBeQuiet = true;
		if(MyAI.Squad != null)
		{
			shouldBeQuiet = MyAI.Squad.ShouldIBeQuiet();
		}


		if(vocalType == VocalType.Surprise)
		{
			if(UnityEngine.Random.value > 0f && !shouldBeQuiet)
			{
				int choice = UnityEngine.Random.Range(1, 7);
				CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(MyReference.VocalSet.ToString() + "_" + _voiceStyle + "_surprise" + choice), 0.2f);
			}
		}

		else if(vocalType == VocalType.Injury)
		{
			if(UnityEngine.Random.value > 0f)
			{
				int choice = UnityEngine.Random.Range(1, 5);
				CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(MyReference.VocalSet.ToString() + "_" + _voiceStyle + "_injury" + choice), 0.3f);
			}
		}

		else if(vocalType == VocalType.Coverfire && !shouldBeQuiet)
		{
			if(UnityEngine.Random.value > 0f)
			{
				int choice = UnityEngine.Random.Range(1, 5);
				CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(MyReference.VocalSet.ToString() + "_" + _voiceStyle + "_coverfire" + choice), 0.3f);
			}
		}

		else if(vocalType == VocalType.Grenade)
		{
			if(UnityEngine.Random.value > 0f && !shouldBeQuiet)
			{
				int choice = UnityEngine.Random.Range(1, 6);
				CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(MyReference.VocalSet.ToString() + "_" + _voiceStyle + "_grenade" + choice), 0.3f);
			}
		}

		else if(vocalType == VocalType.Search)
		{
			if(UnityEngine.Random.value > 0f && !shouldBeQuiet)
			{
				int choice = UnityEngine.Random.Range(1, 7);
				CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(MyReference.VocalSet.ToString() + "_" + _voiceStyle + "_search" + choice), 0.2f);
			}
		}

		else if(vocalType == VocalType.Death)
		{
			if(UnityEngine.Random.value > 0f)
			{
				int choice = UnityEngine.Random.Range(1, 5);
				CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(MyReference.VocalSet.ToString() + "_" + _voiceStyle + "_death" + choice), 0.3f);
			}
		}


	}

	public override Vector3 GetCharacterVelocity()
	{
		if(MyNavAgent != null)
		{
			return MyNavAgent.velocity;
		}
		else if(MyCC != null)
		{
			return MyCC.velocity;
		}

		return Vector3.zero;
	}

	public override void SendCommand(CharacterCommands command)
	{

		if(command == CharacterCommands.PlayAnimationAction)
		{
			SendCommand(CharacterCommands.StopAim);
			IsBodyLocked = true;

			Weapon currWeapon = MyAI.WeaponSystem.GetCurrentWeapon();
			if(currWeapon != null && currWeapon.IsTwoHanded)
			{
				MyLeftHandIK.InstantDisable();

			}

			if(ActionState == HumanActionStates.Reload)
			{
				SendCommand(CharacterCommands.CancelReload);
			}

			CurrentAnimState = new HumanAnimStateAction(this);

		}

		if(command == CharacterCommands.AnimationActionDone)
		{
			Weapon currWeapon = MyAI.WeaponSystem.GetCurrentWeapon();
			if(currWeapon != null && currWeapon.IsTwoHanded && GetCurrentAnimWeapon() != WeaponAnimType.Pistol)
			{
				MyLeftHandIK.SmoothEnable(10);
			}

			if(MyCC != null)
			{
				MyCC.Move(Vector3.zero);

			}

			IsBodyLocked = false;
			IsMoveLocked = false;

			CapsuleCollider collider = GetComponent<CapsuleCollider>();
			collider.height = 1.7f;
			collider.center = new Vector3(0, 1, 0);
			collider.direction = 1;

		}


		if(!IsBodyLocked && !IsMoveLocked && MyAI.ControlType == AIControlType.Player)
		{
			CurrentAnimState.SendCommand(command);
		}
		else if(!IsBodyLocked && !IsMoveLocked && MyAI.ControlType != AIControlType.Player)
		{
			
			CurrentAnimState.SendCommand(command);
		}


		//following commands are not given by AI or user. All commands that will unlock the body go here
		if(command == CharacterCommands.StopAim)
		{
			//Debug.Log("Stop AIM " + this.name);
			if(ActionState == HumanActionStates.None)
			{
				MyAI.WeaponSystem.StopFiringRangedWeapon();
				UpperBodyState = HumanUpperBodyStates.Idle;
				MyAimIK.solver.SmoothDisable(6);
				MyHeadIK.SmoothEnable(6);
				MyAnimator.SetBool("IsAiming", false);
				if(MyReference.FlashlightMount != null)
				{
					MyReference.FlashlightMount.transform.localEulerAngles = new Vector3(18, 0, 0);
					MyReference.Flashlight.transform.localEulerAngles = Vector3.zero;
				}

				if(GetCurrentAnimWeapon() == WeaponAnimType.Pistol || GetCurrentAnimWeapon() == WeaponAnimType.Grenade)
				{
					MyLeftHandIK.InstantDisable();
				}
			}
			else
			{
				UpperBodyState = HumanUpperBodyStates.Idle;
			}

			_isHipAiming = false;
			//Debug.LogError("stopping aim " + ActionState + " " + this.name);
		}

		if(command == CharacterCommands.Cancel)
		{
			if(ActionState == HumanActionStates.Strangle)
			{
				ActionState = HumanActionStates.None;
				MyAnimator.SetTrigger("Cancel");

				Vector3 lineOfSight = _strangleTarget.transform.position - transform.position;
				transform.position = _strangleTarget.transform.position - lineOfSight.normalized;

				if(_strangleTarget.IsAlive)
				{
					_strangleTarget.IsBodyLocked = false;
					_strangleTarget.MyAnimator.SetTrigger("Cancel");
					_strangleTarget = null;
				}

				IsBodyLocked = false;
				MyNavAgent.enabled = true;

			}
		}

		if(command == CharacterCommands.RightAttack)
		{
			if(MyStatus.IsResting)
			{
				return;
			}

			ActionState = HumanActionStates.Melee;
			_meleeStrikeStage = 0;

			MyAnimator.SetTrigger("RightAttack");
			IsMoveLocked = true;
			MyHeadIK.SmoothDisable(9);

			if(GameManager.Inst.PlayerControl.SelectedPC == this)
			{
				MyStatus.ChangeStamina(-10 * MyStatus.StaminaReduceMult);
			}

			Debug.Log("starting right attack");
		}

		if(command == CharacterCommands.LeftAttack)
		{
			if(MyStatus.IsResting)
			{
				return;
			}

			ActionState = HumanActionStates.Melee;
			_meleeStrikeStage = 0;

			Vector3 lookDir = LookTarget.position - transform.position;
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);
			Vector3 velocity = GetCharacterVelocity();
			Vector3 destDir = velocity.normalized; 
			destDir = new Vector3(destDir.x, 0, destDir.z);
			float lookDestAngle = Vector3.Angle(lookDir, destDir);

			if(velocity.magnitude > 0.05f && lookDestAngle < 70)
			{
				//MyAnimator.SetTrigger("ComboAttack");
				MyAI.BlackBoard.AnimationAction = AnimationActions.ComboAttack;
				MyAI.BlackBoard.ActionMovementDest = transform.position + (this.LookTarget.position - transform.position).normalized * 2f;
				MyAI.BlackBoard.ActionMovementSpeed = 1.5f;

				SendCommand(CharacterCommands.PlayAnimationAction);
				_isComboAttack = true;
			}
			else
			{
				MyAnimator.SetTrigger("LeftAttack");
				IsMoveLocked = true;
			}
			MyHeadIK.SmoothDisable(9);

			if(GameManager.Inst.PlayerControl.SelectedPC == this)
			{
				MyStatus.ChangeStamina(-10 * MyStatus.StaminaReduceMult);
			}

			Debug.Log("starting left attack ");
		}

		if(command == CharacterCommands.Block)
		{
			ActionState = HumanActionStates.Block;
			this.MyLeftHandIK.Target = MyReference.CurrentWeapon.GetComponent<Weapon>().ForeGrip;
			this.MyLeftHandIK.SmoothEnable(20);
			_meleeStrikeStage = 0;
			MyAnimator.SetTrigger("Block");
			IsMoveLocked = true;
			_blockTimer = 0;

		}

		if(command == CharacterCommands.Kick)
		{
			if(MyStatus.IsResting)
			{
				return;
			}

			if(ActionState == HumanActionStates.None)
			{
				//kick direction
				Vector3 lookDir = LookTarget.position - transform.position;
				lookDir = new Vector3(lookDir.x, 0, lookDir.z);
				transform.rotation = Quaternion.LookRotation(lookDir);

				if(MyReference.RightFoot != null)
				{
					MyReference.RightFoot.SetActive(true);
				}

				if(UpperBodyState == HumanUpperBodyStates.Aim)
				{
					if(GetCurrentAnimWeapon() == WeaponAnimType.Longgun || GetCurrentAnimWeapon() == WeaponAnimType.Pistol)
					{
						MyAimIK.solver.SmoothDisable(12);
						MyLeftHandIK.SmoothDisable();
					}
				}

				MyHeadIK.SmoothDisable();

				ActionState = HumanActionStates.Melee;
				MyAnimator.SetTrigger("Kick");
				IsBodyLocked = true;

				if(GameManager.Inst.PlayerControl.SelectedPC == this)
				{
					MyStatus.ChangeStamina(-7 * MyStatus.StaminaReduceMult);
				}
			}
		}





		if(IsBodyLocked)
		{
			return;
		}

		//following commands are given by AI or user, and can be locked


		if(command == CharacterCommands.Crouch)
		{
			CapsuleCollider collider = GetComponent<CapsuleCollider>();
			collider.height = 1.35f;
			collider.center = new Vector3(0, 0.3f, 0);

		}

		if(command == CharacterCommands.StopCrouch)
		{
			CapsuleCollider collider = GetComponent<CapsuleCollider>();
			collider.height = 1.7f;
			collider.center = new Vector3(0, 1, 0);
		}

	
		if((command == CharacterCommands.Aim || command == CharacterCommands.HipAim) && CurrentStance != HumanStances.Sprint)
		{
			//if(MyAI.ControlType != AIControlType.Player)
			//Debug.Log("AIM - action state " + ActionState + " weapon type " + GetCurrentAnimWeapon() + " upper body state " + UpperBodyState);

			if((ActionState == HumanActionStates.None || ActionState == HumanActionStates.Twitch) && GetCurrentAnimWeapon() != WeaponAnimType.Unarmed 
				&& (UpperBodyState != HumanUpperBodyStates.Aim || !MyAnimator.GetBool("IsAiming")))
			{

				if(GetCurrentAnimWeapon() == WeaponAnimType.Grenade || GetCurrentAnimWeapon() == WeaponAnimType.Tool)
				{
					MyLeftHandIK.SmoothDisable(6);
					UpperBodyState = HumanUpperBodyStates.HalfAim;
				}
				else
				{
					
					UpperBodyState = HumanUpperBodyStates.Aim;
					if(GetCurrentAnimWeapon() == WeaponAnimType.Pistol)
					{
						MyLeftHandIK.InstantDisable();
					}
					MyLeftHandIK.SmoothEnable(6);
					MyAimIK.solver.InstantDisable();
					MyAimIK.solver.CurvedEnable(6f);
					//if(GetCurrentAnimWeapon() != WeaponAnimType.Melee)
					{
						MyHeadIK.InstantDisable();
					}
					MyAnimator.SetBool("IsAiming", true);
					if(MyAI.ControlType != AIControlType.Player)
						Debug.LogError("Animation parameter IsAiming has been set");
					
				}
					
				/*
				//draw a new grenade if there isn't one
				if(GetCurrentAnimWeapon() == WeaponAnimType.Grenade)
				{
					if(_thrownObjectInHand == null)
					{
						DrawNextGrenade();
					}
				}
				*/

			}
			else if(ActionState == HumanActionStates.SwitchWeapon && GetCurrentAnimWeapon() != WeaponAnimType.Melee && GetCurrentAnimWeapon() != WeaponAnimType.Unarmed)
			{
				
				if(UpperBodyState == HumanUpperBodyStates.Aim && !MyAnimator.GetBool("IsAiming"))
				{
					
					MyLeftHandIK.InstantDisable();
					MyLeftHandIK.SmoothEnable(6);
					MyAimIK.solver.InstantDisable();
					MyAimIK.solver.SmoothEnable(6f);
					MyHeadIK.InstantDisable();
					MyAnimator.SetBool("IsAiming", true);
					//if(MyAI.ControlType != AIControlType.Player)
					//	Debug.LogError("Animation parameter IsAiming has been set");
					if(MyReference.Flashlight != null)
					{
						MyReference.FlashlightMount.transform.localEulerAngles = new Vector3(18, 0, 0);
					}
				}
				else
				{
					UpperBodyState = HumanUpperBodyStates.Aim;
				}

			}
			else if(GetCurrentAnimWeapon() == WeaponAnimType.Unarmed)
			{
				SendCommand(MyAI.WeaponSystem.GetBestWeaponChoice());

			}


			if(command == CharacterCommands.HipAim)
			{
				_isHipAiming = true;
			}
			else if(command == CharacterCommands.Aim)
			{
				_isHipAiming = false;
			}
		}



		if(command == CharacterCommands.Sprint)
		{
			if(UpperBodyState == HumanUpperBodyStates.Aim || UpperBodyState == HumanUpperBodyStates.HalfAim)
			{
				SendCommand(CharacterCommands.StopAim);
			}
			/*
			if(CurrentStance == HumanStances.Crouch || CurrentStance == HumanStances.CrouchRun)
			{
				CurrentStance = HumanStances.CrouchRun;
			}
			else*/
			{
				CurrentStance = HumanStances.Sprint;
				MyAimIK.solver.SmoothDisable();
				MyHeadIK.InstantDisable();
			}
		}

		if(command == CharacterCommands.StopSprint)
		{
			if(UpperBodyState == HumanUpperBodyStates.Aim)
			{
				MyAimIK.solver.SmoothEnable();
			}

			if(CurrentStance == HumanStances.CrouchRun || CurrentStance == HumanStances.Crouch)
			{
				CurrentStance = HumanStances.Crouch;
			}
			else
			{
				CurrentStance = HumanStances.Run;
			}
			MyHeadIK.SmoothEnable();
		}

		if(command == CharacterCommands.SwitchWeapon2)
		{
			if((ActionState == HumanActionStates.None || ActionState == HumanActionStates.Twitch) && MyAI.WeaponSystem.PrimaryWeapon != null)
			{
				CsDebug.Inst.CharLog(this, "Start switching weapon2");
				MyLeftHandIK.SmoothDisable(15);
				MyAimIK.solver.SmoothDisable(9);

				//SwitchWeapon(Inventory.RifleSlot);
				_weaponToSwitch = Inventory.RifleSlot;
				if(MyAI.WeaponSystem.PrimaryWeapon.IsRanged)
				{
					MyAnimator.SetInteger("WeaponType", 2);
				}
				else
				{
					MyAnimator.SetInteger("WeaponType", 3);
				}



				ActionState = HumanActionStates.SwitchWeapon;
			}
		}

		if(command == CharacterCommands.SwitchWeapon1)
		{
			if((ActionState == HumanActionStates.None || ActionState == HumanActionStates.Twitch) && MyAI.WeaponSystem.SideArm != null)
			{
				if(UpperBodyState == HumanUpperBodyStates.Aim)
				{
					//MyLeftHandIK.SmoothEnable();
				}
				else
				{
					
				}
				MyLeftHandIK.SmoothDisable(15);
				MyAimIK.solver.SmoothDisable(9);
				//SwitchWeapon(Inventory.SideArmSlot);
				_weaponToSwitch = Inventory.SideArmSlot;
				if(MyAI.WeaponSystem.SideArm.IsRanged)
				{
					MyAnimator.SetInteger("WeaponType", 1);
				}
				else
				{
					MyAnimator.SetBool("IsAiming", false);
					MyAnimator.SetInteger("WeaponType", 3);
				}


				ActionState = HumanActionStates.SwitchWeapon;
			}
		}

		if(command == CharacterCommands.SwitchThrown)
		{
			if(ActionState == HumanActionStates.None)
			{

				MyLeftHandIK.SmoothDisable(6);
				MyAimIK.solver.SmoothDisable(9);
				MyAnimator.SetInteger("WeaponType", -1);
				MyAnimator.SetBool("IsAiming", false);
				//SwitchWeapon(Inventory.ThrowSlot);
				_weaponToSwitch = Inventory.ThrowSlot;
				if(_weaponToSwitch == null)
				{
					_weaponToSwitch = new Item();
					_weaponToSwitch.ID = "rock";
					_weaponToSwitch.Type = ItemType.Thrown;
				}


				ActionState = HumanActionStates.SwitchWeapon;
			}
		}

		if(command == CharacterCommands.SwitchTool)
		{
			if(ActionState == HumanActionStates.None && Inventory.ToolSlot != null)
			{
				GameObject.Destroy(this.MyReference.CurrentWeapon);
				if(_thrownObjectInHand != null)
				{
					GameObject.Destroy(_thrownObjectInHand.gameObject);
				}

				MyLeftHandIK.SmoothDisable(6);
				MyAimIK.solver.SmoothDisable(9);
				MyAnimator.SetInteger("WeaponType", -2);
				MyAnimator.SetBool("IsAiming", false);
				//SwitchWeapon("ThrowingRock");
				_weaponToSwitch = Inventory.ToolSlot;



				ActionState = HumanActionStates.SwitchWeapon;
			}
		}

		if(command == CharacterCommands.Unarm)
		{
			if(ActionState == HumanActionStates.None)
			{
				//Debug.Log("Unarm " + this.name);
				MyLeftHandIK.SmoothDisable();
				UpperBodyState = HumanUpperBodyStates.Idle;
				MyAimIK.solver.SmoothDisable();
				MyHeadIK.SmoothEnable();
				MyAnimator.SetBool("IsAiming", false);
				MyAnimator.SetInteger("WeaponType", 0);
				//SwitchWeapon(null);
				_weaponToSwitch = null;

				MyEventHandler.TriggerOnPutAwayWeapon();

				ActionState = HumanActionStates.SwitchWeapon;

				if(MyAI.ControlType == AIControlType.Player)
				{
					GameManager.Inst.SoundManager.PlayerAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("holster"), 0.2f);
				}
			}
		}

		if(command == CharacterCommands.PullTrigger)
		{
			if(ActionState != HumanActionStates.None || UpperBodyState != HumanUpperBodyStates.Aim)
			{
				return;
			}

			if(GetCurrentAnimWeapon() == WeaponAnimType.Longgun || GetCurrentAnimWeapon() == WeaponAnimType.Pistol)
			{
				//
				this.MyReference.CurrentWeapon.GetComponent<Gun>().TriggerPull();


			}
		}

		if(command == CharacterCommands.ReleaseTrigger)
		{
			if(ActionState != HumanActionStates.None || UpperBodyState != HumanUpperBodyStates.Aim)
			{
				return;
			}

			if(GetCurrentAnimWeapon() == WeaponAnimType.Longgun || GetCurrentAnimWeapon() == WeaponAnimType.Pistol)
			{
				//
				this.MyReference.CurrentWeapon.GetComponent<Gun>().TriggerRelease();


			}
		}


		if(command == CharacterCommands.Reload)
		{
			if(ActionState == HumanActionStates.None && this.MyReference.CurrentWeapon != null)
			{
				GunMagazine magazine = this.MyReference.CurrentWeapon.GetComponent<GunMagazine>();
				if(magazine != null && magazine.AmmoLeft < magazine.MaxCapacity)
				{
					GridItemData ammo = this.Inventory.FindItemInBackpack(magazine.LoadedAmmoID);
					if(MyAI.ControlType != AIControlType.Player || ammo != null || (this.MyReference.CurrentWeapon.GetComponent<Gun>().IsReloadToUnjam && magazine.AmmoLeft > 0))
					{
						if(GetCurrentAnimWeapon() == WeaponAnimType.Longgun || GetCurrentAnimWeapon() == WeaponAnimType.Pistol)
						{
							MyAimIK.solver.SmoothDisable(12);
							MyAnimator.SetInteger("ReloadType", (int)magazine.ReloadType);
							MyAnimator.SetTrigger("Reload");
							
							MyLeftHandIK.SmoothDisable();


						}

						MyHeadIK.SmoothDisable();
							
						ActionState = HumanActionStates.Reload;

						if(MyAI.ControlType == AIControlType.Player)
						{
							Gun gun = this.MyReference.CurrentWeapon.GetComponent<Gun>();
							AudioSource audio = gun.GetComponent<AudioSource>();
							if(audio != null)
							{
								audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(gun.GunshotSoundName + "_reload"), 0.1f);
							}
						}

					}
				}

				GameObject magObject = this.MyReference.CurrentWeapon.GetComponent<Gun>().MagazineObject;
				if(magObject != null)
				{
					magObject.GetComponent<MeshRenderer>().enabled = false;
				}

			}
		}



		if(command == CharacterCommands.CancelReload)
		{
			if(ActionState == HumanActionStates.Reload && this.MyReference.CurrentWeapon != null)
			{
				
				Debug.Log("cancel reload");
				ActionState = HumanActionStates.None;

				if(UpperBodyState == HumanUpperBodyStates.Aim)
				{
					MyAimIK.solver.SmoothEnable();
					MyAnimator.SetTrigger("CancelReload");
				}
				else
				{
					MyAnimator.SetTrigger("CancelReload");
				}

				if(MyAnimator.GetInteger("WeaponType") == (int)WeaponAnimType.Longgun)
				{
					MyLeftHandIK.SmoothEnable();
				}
				else
				{
					Debug.Log("done reloading pistol " + UpperBodyState);
					if(UpperBodyState == HumanUpperBodyStates.Aim)
					{
						MyLeftHandIK.SmoothEnable();
					}
					else
					{
						//MyLeftHandIK.SmoothDisable();
						SendCommand(CharacterCommands.StopAim);
					}
				}

				MyHeadIK.SmoothEnable();

				GameObject magObject = this.MyReference.CurrentWeapon.GetComponent<Gun>().MagazineObject;
				if(magObject != null)
				{
					magObject.GetComponent<MeshRenderer>().enabled = true;
				}

				if(MyAI.ControlType == AIControlType.Player)
				{
					Gun gun = this.MyReference.CurrentWeapon.GetComponent<Gun>();
					AudioSource audio = gun.GetComponent<AudioSource>();
					if(audio != null)
					{
						audio.Stop();
					}
				}

			}
		}



		if(command == CharacterCommands.Throw || command == CharacterCommands.LowThrow)
		{
			if(ActionState != HumanActionStates.None)
			{
				return;
			}

			if(_thrownObjectInHand == null)
			{
				DrawNextGrenade();
			}



			MyAimIK.solver.SmoothDisable(15);
			if(command == CharacterCommands.LowThrow)
			{
				MyAnimator.SetTrigger("LowThrow");
				_isLowThrow = true;
			}
			else
			{
				MyAnimator.SetTrigger("Throw");
				_isLowThrow = false;
			}

			_thrownObjectInHand.GetComponent<Rigidbody>().isKinematic = false;
			_thrownObjectInHand.GetComponent<Rigidbody>().useGravity = false;
			_throwTarget = this.AimPoint;
			Vector3 lookDir = this.AimPoint - transform.position;
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);
			Quaternion rotation = Quaternion.LookRotation(lookDir);
			transform.rotation = rotation;
			IsBodyLocked = true;

		}

		if(command == CharacterCommands.ThrowGrenade)
		{
			if(ActionState != HumanActionStates.None)
			{
				return;
			}

			if(UpperBodyState != HumanUpperBodyStates.Aim)
			{
				//MyAimIK.solver.transform = this.MyReference.TorsoWeaponMount.transform;
			}

			SendCommand(CharacterCommands.CancelReload);

			if(this.MyReference.CurrentWeapon != null && MyAnimator.GetInteger("WeaponType") == (int)WeaponAnimType.Longgun)
			{
				MyLeftHandIK.SmoothEnable();
			}

			//MyHeadIK.SmoothDisable(1);

			//move weapon to torso mount so that right hand is free
			if(this.MyReference.CurrentWeapon != null)
			{
				this.MyReference.CurrentWeapon.transform.parent = this.MyReference.TorsoWeaponMount.transform;
			}
			MyAnimator.SetTrigger("ThrowGrenade");

			_throwTarget = this.AimPoint;

			_throwDir = this.AimPoint - transform.position;
			IsBodyLocked = true;
			Quaternion rotation = Quaternion.LookRotation(new Vector3(_throwDir.x, 0, _throwDir.z));
			transform.rotation = rotation;

			ActionState = HumanActionStates.Throw;

			string grenadeName = "rgd5grenade";
			if(UnityEngine.Random.value > 0.75f)
			{
				grenadeName = "f1grenade";
			}

			Item grenadeItem = GameManager.Inst.ItemManager.LoadItem(grenadeName);
			_thrownObjectInHand = ((GameObject)GameObject.Instantiate(Resources.Load(grenadeName))).GetComponent<ThrownObject>();
			Explosive explosive = _thrownObjectInHand.GetComponent<Explosive>();
			if(explosive != null)
			{
				explosive.Attacker = this;

			}
			HandGrenade grenade = _thrownObjectInHand.GetComponent<HandGrenade>();
			if(grenade != null)
			{
				grenade.SetExplosive(grenadeItem);
			}

			_thrownObjectInHand.GetComponent<Rigidbody>().isKinematic = true;

			_thrownObjectInHand.transform.parent = this.MyReference.RightHandWeaponMount.transform;
			_thrownObjectInHand.transform.localPosition = _thrownObjectInHand.InHandPosition;
			_thrownObjectInHand.transform.localEulerAngles = _thrownObjectInHand.InHandRotation;

			PlayVocal(VocalType.Grenade);
		}

		if(command == CharacterCommands.UseTool)
		{
			if(ActionState != HumanActionStates.None || MyAI.BlackBoard.TargetEnemy == null)
			{
				return;
			}

			//check if the target enemy is close enough and angle between character and enemy is less than 45
			if(Vector3.Distance(MyAI.BlackBoard.TargetEnemy.transform.position, transform.position) > 2 || 
				Vector3.Angle(MyAI.BlackBoard.TargetEnemy.transform.forward, transform.forward) > 45)
			{
				return;
			}

			Vector3 lineOfSight = MyAI.BlackBoard.TargetEnemy.transform.position - transform.position;
			//check if angle between character facing and target line of sight is less than 45
			if(Vector3.Angle(lineOfSight, transform.forward) > 45)
			{
				return;
			}

			//stop movement
			SendCommand(CharacterCommands.Idle);
			IsBodyLocked = true;
			MyNavAgent.enabled = false;

			//place player right behind target
			transform.position = MyAI.BlackBoard.TargetEnemy.transform.position - lineOfSight.normalized * 0.25f;

			//align player facing direction to enemy's
			lineOfSight = new Vector3(lineOfSight.x, 0, lineOfSight.z);
			Quaternion rotation = Quaternion.LookRotation(lineOfSight);
			transform.rotation = rotation;

			MyAnimator.SetTrigger("Strangle");
			_strangleTarget = MyAI.BlackBoard.TargetEnemy;

			SendCommand(CharacterCommands.StopAim);
			ActionState = HumanActionStates.Strangle;
		}

		if(command == CharacterCommands.Pickup)
		{
			if(ActionState != HumanActionStates.None || MyAI.BlackBoard.PickupTarget == null)
			{
				return;
			}

			/*
			//move weapon to torso mount so that right hand is free
			if(this.MyReference.CurrentWeapon != null && MyAnimator.GetInteger("WeaponType") == (int)WeaponAnimType.Longgun)
			{
				this.MyReference.CurrentWeapon.transform.parent = this.MyReference.TorsoWeaponMount.transform;
			}
			*/

			//disable left hand IK
			MyLeftHandIK.InstantDisable();

			MyAnimator.SetTrigger("TakeObject");


		}

		if(command == CharacterCommands.Loot)
		{
			if(ActionState != HumanActionStates.None)
			{
				return;
			}

			Character target = MyAI.BlackBoard.InteractTarget;

			if(target != null && target.MyStatus.Health <= 0)
			{
				if(this.MyAI.ControlType == AIControlType.Player)
				{
					UIEventHandler.Instance.TriggerLootBody();

				}
			}


		}

		if(command == CharacterCommands.LootChest)
		{
			if(ActionState != HumanActionStates.None)
			{
				return;
			}
			GameObject useTarget = MyAI.BlackBoard.UseTarget;

			if(useTarget != null)
			{
				//open chest
				Chest chest = useTarget.GetComponent<Chest>();
				if(chest != null)
				{
					//check if it's locked
					if(chest.IsLocked && chest.KeyID.Length > 0)
					{
						//here check if player has key
						int keyCount = GameManager.Inst.PlayerControl.SelectedPC.Inventory.CountItemsInBackpack(chest.KeyID);
						if(keyCount <= 0 && chest.AudioSource != null)
						{
							//play locked door sound
							if(chest.SoundType == ContainerSoundType.Wood)
							{
								AudioClip clip = GameManager.Inst.SoundManager.GetClip("WoodDoorLocked");
								chest.AudioSource.PlayOneShot(clip, 0.6f);
							}
							else if(chest.SoundType == ContainerSoundType.Metal)
							{
								AudioClip clip = GameManager.Inst.SoundManager.GetClip("MetalDoorLocked");
								chest.AudioSource.PlayOneShot(clip, 0.6f);
							}

							return;
						}
					}
					//open UI 
					UIEventHandler.Instance.TriggerLootChest();
				}
			}

		}


		if(command == CharacterCommands.Use)
		{
			if(ActionState != HumanActionStates.None)
			{
				return;
			}
				
			GameObject useTarget = MyAI.BlackBoard.UseTarget;

			Vector3 lookDir = useTarget.transform.position - transform.position;
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);
			transform.rotation = Quaternion.LookRotation(lookDir);

			MyAnimator.SetTrigger("Interact");



		}

		if(command == CharacterCommands.Talk)
		{
			if(ActionState != HumanActionStates.None)
			{
				return;
			}

			Character target = MyAI.BlackBoard.InteractTarget;

			if(target != null && target.MyStatus.Health > 0)
			{
				
				if(this.MyAI.ControlType == AIControlType.Player)
				{
					
					UIEventHandler.Instance.TriggerDialogue();

				}
			}

		}

		if(command == CharacterCommands.SetAlert)
		{
			float ambient = RenderSettings.ambientIntensity;

			if(MyAI.BlackBoard.GuardLevel == 1)
			{
				//weapon holstered; only turn on flash light (if has one) when completely dark
				if(MyReference.CurrentWeapon != null)
				{
					SendCommand(CharacterCommands.Unarm);
				}
				if(MyReference.Flashlight != null)
				{
					if(ambient <= 0.3f && MyNavAgent != null && MyNavAgent.velocity.magnitude > 0.5f)
					{
						MyReference.Flashlight.Toggle(true);
					}
					else
					{
						MyReference.Flashlight.Toggle(false);
					}
				}
			}
			else if(MyAI.BlackBoard.GuardLevel >= 2)
			{
				if(MyReference.CurrentWeapon == null)
				{
					SendCommand(MyAI.WeaponSystem.GetBestWeaponChoice());
				}

				if(MyReference.Flashlight != null)
				{
					if(ambient <= 0.5f)
					{
						MyReference.Flashlight.Toggle(true);
					}
					else
					{
						MyReference.Flashlight.Toggle(false);
					}
				}
			}

		}



	}



	public override bool SendDamage(Damage damage, Vector3 hitNormal, Character attacker, Weapon attackerWeapon)
	{
		
		
		if(MyAI.ControlType == AIControlType.Player)
		{
			//player do'nt stumble
			OnInjury(hitNormal, false);
		}
		else
		{
			OnInjury(hitNormal, damage.IsCritical);
		}

		if(attacker != null && MyAI.ControlType != AIControlType.Player)
		{
			MyAI.Sensor.OnTakingDamage(attacker);
		}

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
			finalDamage *= 1.5f;
			Debug.Log("Is character inventory headslot null? " + (this.Inventory.HeadSlot == null));
			if(this.Inventory.HeadSlot != null)
			{
				float armorRating = (float)this.Inventory.HeadSlot.GetAttributeByName("Armor").Value;
				float coverage = (float)this.Inventory.HeadSlot.GetAttributeByName("Coverage").Value;
				float chance = UnityEngine.Random.value;

				if(damage.Type == DamageType.Bullet)
				{
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
				else if(damage.Type == DamageType.Explosive)
				{
					finalDamage *= 1 - coverage;
				}
			}

		}
		else if(this.Inventory.ArmorSlot != null)
		{
			Debug.Log("Is character inventory armor null? " + (this.Inventory.ArmorSlot.Name));
			float armorRating = (float)this.Inventory.ArmorSlot.GetAttributeByName("Armor").Value;
			float coverage = (float)this.Inventory.ArmorSlot.GetAttributeByName("Coverage").Value;
			float chance = UnityEngine.Random.value;
			if(damage.Type == DamageType.Bullet)
			{
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
			else if(damage.Type == DamageType.Explosive)
			{
				finalDamage *= 1 - coverage;
			}

		}

		if(finalDamage > 0 && damage.Bleeding > 0)
		{
			MyStatus.AddBleeding(damage.Bleeding);
		}

		if(MyAI.ControlType == AIControlType.Player && GameManager.Inst.GodMode)
		{
			
		}
		else
		{
			MyStatus.Health -= finalDamage;
		}

		if(MyAI.ControlType == AIControlType.Player)
		{
			GameManager.Inst.CameraShaker.TriggerScreenShake(0.1f, 0.06f);
			float damageRatio = finalDamage / MyStatus.Health;
			if(damageRatio < 0.3f)
			{
				GameManager.Inst.UIManager.HUDPanel.SetBloodLevel(1);
			}
			else if(damageRatio < 0.7f)
			{
				GameManager.Inst.UIManager.HUDPanel.SetBloodLevel(2);
			}
			else
			{
				GameManager.Inst.UIManager.HUDPanel.SetBloodLevel(3);
			}
		}

		DeathReason = damage.Type;
		if(attacker != null && attacker.CharacterType == CharacterType.Human)
		{
			Killer = attacker;
		}
		//GameManager.Inst.UIManager.BarkPanel.AddFloater(this, Mathf.FloorToInt(damage).ToString(), true);
		




		return false;
	}

	public override MeleeBlockType SendMeleeDamage (Damage damage, Vector3 hitNormal, Character attacker, float knockBackChance)
	{
		if(MyAI.ControlType == AIControlType.Player && GameManager.Inst.GodMode)
		{
			return MeleeBlockType.SoftArmor;
		}

		float attackerAngle = Vector3.Angle(transform.forward, transform.position - attacker.transform.position);
		if(ActionState == HumanActionStates.Block && attackerAngle > 135)
		{
			this.MyLeftHandIK.InstantDisable();
			MyAnimator.SetTrigger("BlockSuccess");
			_twitchTimer = 0.5f;
			ActionState = HumanActionStates.Twitch;

			if(MyAI.ControlType == AIControlType.Player)
			{
				GameManager.Inst.CameraShaker.TriggerScreenShake(0.07f, 0.09f);
			}

			Debug.Log("block timer " + _blockTimer);
			if(_blockTimer < 0.25f)
			{
				//face attacker
				Vector3 lookDir = attacker.transform.position - transform.position;
				lookDir = new Vector3(lookDir.x, 0, lookDir.z);
				transform.rotation = Quaternion.LookRotation(lookDir);

				ActionState = HumanActionStates.Melee;
				if(MyReference.RightFoot != null)
				{
					MyReference.RightFoot.SetActive(true);
				}
				MyAnimator.SetTrigger("Kick");
				IsBodyLocked = true;
			}

			return MeleeBlockType.Metal;
		}
		else
		{
			OnInjury(hitNormal, UnityEngine.Random.value <= knockBackChance);
			if(MyAI.ControlType != AIControlType.Player)
			{
				MyAI.Sensor.OnTakingDamage(attacker);
			}

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

			if(finalDamage > 0 && damage.Bleeding > 0)
			{
				MyStatus.AddBleeding(damage.Bleeding);
			}

			MyStatus.Health -= finalDamage;
			DeathReason = damage.Type;
			if(attacker.CharacterType == CharacterType.Human)
			{
				Killer = attacker;
			}

			if(MyAI.ControlType == AIControlType.Player)
			{
				GameManager.Inst.CameraShaker.TriggerScreenShake(0.1f, 0.1f);
				float damageRatio = finalDamage / MyStatus.Health;
				if(damageRatio < 0.3f)
				{
					GameManager.Inst.UIManager.HUDPanel.SetBloodLevel(1);
				}
				else if(damageRatio < 0.7f)
				{
					GameManager.Inst.UIManager.HUDPanel.SetBloodLevel(2);
				}
				else
				{
					GameManager.Inst.UIManager.HUDPanel.SetBloodLevel(3);
				}
			}

			if(finalDamage <= 0)
			{
				Debug.Log("no damage");
				return MeleeBlockType.SoftArmor;
			}
		}



		return MeleeBlockType.NoBlock;
	}





	public override void SendDelayCallBack (float delay, DelayCallBack callback, object parameter)
	{
		StartCoroutine(WaitAndCallback(delay, callback, parameter));
	}


	#endregion




	#region Getters



	public WeaponAnimType GetCurrentAnimWeapon()
	{
		return (WeaponAnimType)MyAnimator.GetInteger("WeaponType");
	}


	public bool IsThrowing()
	{
		return _isThrowing;
	}

	public bool IsComboAttack()
	{
		return _isComboAttack;
	}

	public Vector3 GetLockedAimTarget()
	{
		return _throwTarget;
	}

	public int GetMeleeStrikeStage()
	{
		return _meleeStrikeStage;
	}

	public ThrownObject GetThrownObjectInHand()
	{
		return _thrownObjectInHand;
	}

	public Tool GetToolInHand()
	{
		return _toolInHand;
	}

	#endregion


	#region Animation Events

	public void OnSuccessfulShot()
	{
		MyAnimator.SetTrigger("Shoot");
		StartCoroutine(WaitAndMuzzleClimb(0.02f));
		this.MyStatus.ArmFatigue += 1f;
		if(this.MyStatus.ArmFatigue > this.MyStatus.MaxArmFatigue)
		{
			this.MyStatus.ArmFatigue = this.MyStatus.MaxArmFatigue;
		}

		if(MyAI.ControlType == AIControlType.Player)
		{
			GameManager.Inst.CameraShaker.TriggerScreenShake(0.08f, 0.025f);
		}
	}

	public override void OnSuccessfulHit (Character target)
	{
		if(GameManager.Inst.PlayerControl.SelectedPC == this)
		{
			GameManager.Inst.CursorManager.OnHitMarkerShow();
		}
		else
		{
			if(target.MyStatus.Health <= 0)
			{
				MyAI.Bark("Target is down!");
			}
		}
	}


	public void OnThrowLeaveHand()
	{
		_thrownObjectInHand.transform.parent = null;
		Vector3 distance = _throwTarget - transform.position;

		float magnitude = Mathf.Clamp(distance.magnitude, 9, 17);
		if(MyStatus.IsResting)
		{
			magnitude = 7;
		}
		Vector3 direction = distance.normalized;

		/*
		if(UpperBodyState == HumanUpperBodyStates.HalfAim)
		{
			//direction = MyAimIK.solver.transform.forward;
			direction = distance.normalized;
		}
		*/




		//_thrownObjectInHand.transform.position = this.MyReference.RightHandWeaponMount.transform.position;// + direction * 1f;
		//Time.timeScale = 0;
		Vector3 throwForce = (direction * 2 + Vector3.up).normalized * (magnitude * 0.8f);
		if(_isLowThrow)
		{
			throwForce = direction.normalized * (magnitude * 0.8f);
		}

		_thrownObjectInHand.GetComponent<Rigidbody>().useGravity = true;
		_thrownObjectInHand.GetComponent<Rigidbody>().AddForce(throwForce, ForceMode.Impulse);
		_thrownObjectInHand.GetComponent<Rigidbody>().AddTorque((transform.right + transform.up) * 6, ForceMode.Impulse);
		_thrownObjectInHand.IsThrown = true;
		Explosive explosive = _thrownObjectInHand.GetComponent<Explosive>();
		if(explosive != null)
		{
			explosive.IsEnabled = true;
			//if explosive, add pin sound
			int pinSound = UnityEngine.Random.Range(1, 3);
			GameManager.Inst.SoundManager.PlayerAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("grenade_pin" + pinSound.ToString()), 0.1f);
		}



		_thrownObjectInHand = null;

		if(this.Inventory.ThrowSlot != null)
		{
			//remove either one of the items in backpack or remove the one in bodyslot
			GridItemData itemInBackpack = this.Inventory.FindItemInBackpack(this.Inventory.ThrowSlot.ID);
			if(itemInBackpack != null)
			{
				Item itemToRemove = itemInBackpack.Item;
				this.Inventory.RemoveItemFromBackpack(itemToRemove);
			}
			else
			{
				this.Inventory.ThrowSlot = null;
				MyEventHandler.TriggerOnSelectWeapon("rock");
			}
		}

		if(GameManager.Inst.PlayerControl.SelectedPC == this)
		{
			MyStatus.ChangeStamina(-10 * magnitude/15  * MyStatus.StaminaReduceMult);
			GameManager.Inst.UIManager.HUDPanel.OnUpdateTotalAmmo();
		}


	}

	public void OnThrowFinish()
	{
		IsBodyLocked = false;

		ActionState = HumanActionStates.None;

		if(UpperBodyState == HumanUpperBodyStates.HalfAim)
		{
			SendCommand(CharacterCommands.Aim);
		}
		else
		{
			SendCommand(CharacterCommands.StopAim);
		}

		MyHeadIK.SmoothEnable();

		//move weapon back to right hand mount
		if(this.MyReference.CurrentWeapon != null)
		{
			Weapon myWeapon = this.MyReference.CurrentWeapon.GetComponent<Weapon>();
			myWeapon.transform.parent = MyReference.RightHandWeaponMount.transform;
			myWeapon.transform.localPosition = myWeapon.InHandPosition;
			myWeapon.transform.localEulerAngles = myWeapon.InHandAngles;
		}



	}

	public void OnReloadFinish()
	{
		if(ActionState == HumanActionStates.Reload && this.MyReference.CurrentWeapon != null)
		{
			
			ActionState = HumanActionStates.None;

			if(UpperBodyState == HumanUpperBodyStates.Aim)
			{
				MyAimIK.solver.SmoothEnable(1f);
			}
			else
			{
				SendCommand(CharacterCommands.StopAim);
				MyHeadIK.SmoothEnable();
			}

			if(MyAnimator.GetInteger("WeaponType") == (int)WeaponAnimType.Longgun)
			{
				MyLeftHandIK.SmoothEnable(6);
			}
			else
			{
				if(UpperBodyState == HumanUpperBodyStates.Aim)
				{
					MyLeftHandIK.SmoothEnable(6);
				}
				else
				{
					MyLeftHandIK.SmoothDisable();
				}
			}



			Gun gun = this.MyReference.CurrentWeapon.GetComponent<Gun>();
			if(gun != null)
			{
				int quantity = 0;

				//if it's a not shotgun then load entire mag
				if(gun.Magazine.ReloadType != ReloadAnimType.Shotgun)
				{

					if(MyAI.ControlType == AIControlType.Player || MyAI.ControlType == AIControlType.PlayerTeam)
					{
						quantity = this.Inventory.RemoveItemsFromBackpack(gun.Magazine.LoadedAmmoID, gun.Magazine.MaxCapacity - gun.Magazine.AmmoLeft);
					}
					else
					{
						quantity = gun.Magazine.MaxCapacity;
					}


					gun.WeaponItem.SetAttribute("_LoadedAmmos", gun.Magazine.AmmoLeft + quantity);
					gun.Refresh();

					if(MyAI.ControlType == AIControlType.Player)
					{
						GameManager.Inst.UIManager.HUDPanel.OnUpdateTotalAmmo();
					}

					if(gun.MagazineObject != null)
					{
						gun.MagazineObject.GetComponent<MeshRenderer>().enabled = true;
					}
				}
				else
				{
					//if it's a shotgun then only load one
					if(MyAI.ControlType == AIControlType.Player)
					{
						quantity = this.Inventory.RemoveItemsFromBackpack(gun.Magazine.LoadedAmmoID, 1);
					}
					else
					{
						quantity = 1;
					}

					gun.WeaponItem.SetAttribute("_LoadedAmmos", gun.Magazine.AmmoLeft + quantity);
					gun.Refresh();

					if(MyAI.ControlType == AIControlType.Player)
					{
						GameManager.Inst.UIManager.HUDPanel.OnUpdateTotalAmmo();
					}

					//if not fully loaded then reload again
					if(gun.Magazine.AmmoLeft < gun.Magazine.MaxCapacity)
					{
						SendCommand(CharacterCommands.Reload);
					}



				}


				

			}

		}

	}

	public void OnTakeObjectFinish()
	{
		//if there's pickup item highlighted by the hand cursor, pick it up
		PickupItem pickup = MyAI.BlackBoard.PickupTarget;
		if(pickup != null)
		{
			int colPos;
			int rowPos;
			GridItemOrient orientation;
			/*
				if(this.Inventory.FitItemInBodySlot(pickup.Item))
				{
					GameObject.Destroy(MyAI.BlackBoard.PickupTarget.GetSparkleObject());
					GameObject.Destroy(MyAI.BlackBoard.PickupTarget.gameObject);
				}
				*/
			pickup.Item.Durability = pickup.Durability * pickup.Item.MaxDurability;

			if(this.Inventory.FitItemInBackpack(pickup.Item, out colPos, out rowPos, out orientation))
			{
				Debug.Log("Found backpack fit " + colPos + ", " + rowPos + " orientation " + orientation);


				GridItemData itemData = new GridItemData(pickup.Item, colPos, rowPos, orientation, pickup.Quantity);
				this.Inventory.Backpack.Add(itemData);
				GameObject.Destroy(MyAI.BlackBoard.PickupTarget.GetSparkleObject());
				GameObject.Destroy(MyAI.BlackBoard.PickupTarget.gameObject);
				if(this.MyAI.ControlType == AIControlType.Player)
				{
					GameManager.Inst.PlayerControl.Party.RefreshAllMemberWeight();
				}

				//add a backpack prefab to the back of character
				RefreshBackpackProp();
			}
			else
			{
				Debug.Log("Wont fit");
				if(this.MyAI.ControlType == AIControlType.Player)
				{
					UIEventHandler.Instance.TriggerToggleInventory();
					GameManager.Inst.UIManager.WindowPanel.InventoryPanel.AddUnfitItem(pickup);
					GameObject.Destroy(MyAI.BlackBoard.PickupTarget.GetSparkleObject());
					GameObject.Destroy(MyAI.BlackBoard.PickupTarget.gameObject);
				}
			}

			GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("SelectItem"), 0.1f);
		}

		MyAI.BlackBoard.PickupTarget = null;

		//restore left hand IK
		if(this.MyReference.CurrentWeapon != null && this.MyReference.CurrentWeapon.GetComponent<Weapon>().IsTwoHanded)
		{
			/*
			Weapon myWeapon = this.MyReference.CurrentWeapon.GetComponent<Weapon>();
			myWeapon.transform.parent = MyReference.RightHandWeaponMount.transform;
			myWeapon.transform.localPosition = myWeapon.InHandPosition;
			myWeapon.transform.localEulerAngles = myWeapon.InHandAngles;
			*/
			MyLeftHandIK.SmoothEnable(6);
		}
	}

	public void OnFinishInteract()
	{
		GameObject useTarget = MyAI.BlackBoard.UseTarget;
		if(useTarget.tag == "Door")
		{
			Door door = useTarget.GetComponent<Door>();
			if(door != null)
			{
				if(door.IsOpen)
				{
					door.Close(false);
				}
				else
				{
					door.Open(transform, false);
				}


			}


		}
		else if(useTarget.tag == "Portal")
		{
			Portal portal = useTarget.GetComponent<Portal>();
			portal.Enter(this);
		}
		else if(useTarget.tag == "Interactive")
		{
			StoryObject so = useTarget.GetComponent<StoryObject>();
			if(so != null)
			{
				so.Interact();
			}
		}
		else if(useTarget.tag == "LightSwitch")
		{
			LightSwitch lightSwitch = useTarget.GetComponent<LightSwitch>();
			if(lightSwitch != null)
			{
				lightSwitch.Toggle();
			}
		}
		else if(useTarget.tag == "SerumLab")
		{
			UIEventHandler.Instance.TriggerSerumCraft();
		}
	}

	public void OnLongGunPullOutFinish()
	{
		
		ActionState = HumanActionStates.None;
		if(UpperBodyState == HumanUpperBodyStates.Aim)
		{
			UpperBodyState = HumanUpperBodyStates.None;
			SendCommand(CharacterCommands.Aim);
		}

		MyAI.BlackBoard.NeedToPullOut = 0;

		this.MyLeftHandIK.SmoothEnable(6);
	}

	public void OnLongGunPutAwayFinish()
	{
		ActionState = HumanActionStates.None;

		Debug.Log("OnLong gun put away, need to pull out " + MyAI.BlackBoard.NeedToPullOut);

		if(MyAI.BlackBoard.NeedToPullOut == 2)
		{
			SendCommand(CharacterCommands.SwitchWeapon2);
			MyAI.BlackBoard.NeedToPullOut = 0;
		}
	}
		
	public void OnMeleePullOutFinish()
	{
		MyAI.BlackBoard.NeedToPullOut = 0;
		ActionState = HumanActionStates.None;

	}

	public void OnMeleePutAwayFinish()
	{
		ActionState = HumanActionStates.None;

		if(MyAI.BlackBoard.NeedToPullOut == 1)
		{
			SendCommand(CharacterCommands.SwitchWeapon1);
			MyAI.BlackBoard.NeedToPullOut = 0;
		}
		else if(MyAI.BlackBoard.NeedToPullOut == 2)
		{
			SendCommand(CharacterCommands.SwitchWeapon2);
			MyAI.BlackBoard.NeedToPullOut = 0;
		}


	}

	public void OnPistolPullOutFinish()
	{
		ActionState = HumanActionStates.None;
		if(UpperBodyState == HumanUpperBodyStates.Aim && GetCurrentAnimWeapon() != WeaponAnimType.Tool)
		{
			UpperBodyState = HumanUpperBodyStates.None;
			SendCommand(CharacterCommands.Aim);
		}
		else
		{
			MyLeftHandIK.SmoothDisable(6);
		}
		MyAI.BlackBoard.NeedToPullOut = 0;

	}

	public void OnPistolPutAwayFinish()
	{
		ActionState = HumanActionStates.None;

		if(MyAI.BlackBoard.NeedToPullOut == 1)
		{
			SendCommand(CharacterCommands.SwitchWeapon1);
			MyAI.BlackBoard.NeedToPullOut = 0;
		}


	}

	public void OnGrenadePullOutFinish()
	{
		ActionState = HumanActionStates.None;

		MyAI.BlackBoard.NeedToPullOut = 0;
		MyLeftHandIK.SmoothDisable(6);

	}

	public void OnStartStrangle()
	{
		if(MyAI.BlackBoard.TargetEnemy == null || Vector3.Distance(MyAI.BlackBoard.TargetEnemy.transform.position, transform.position) > 0.5f)
		{
			//cancel the strangle
			MyAnimator.SetTrigger("Cancel");
		}

		Character target = MyAI.BlackBoard.TargetEnemy;
		target.SendCommand(CharacterCommands.Idle);
		target.SendCommand(CharacterCommands.StopAim);
		target.IsBodyLocked = true;



		//align enemy facing direction to player's
		Vector3 lineOfSight = target.transform.position - transform.position;
		lineOfSight = new Vector3(lineOfSight.x, 0, lineOfSight.z);
		Quaternion rotation = Quaternion.LookRotation(lineOfSight);
		target.transform.rotation = rotation;
		target.MyAnimator.SetTrigger("GetStrangled");


		Stealth.SetNoiseLevel(10, 0.8f);
	}

	public void OnEndStrangle()
	{
		if(ActionState == HumanActionStates.Strangle)
		{
			ActionState = HumanActionStates.None;

			Vector3 lineOfSight = _strangleTarget.transform.position - transform.position;
			transform.position = _strangleTarget.transform.position - lineOfSight.normalized;

			_strangleTarget.IsBodyLocked = true;

			IsBodyLocked = false;
			MyNavAgent.enabled = true;
			_strangleTarget = null;
		}
	}

	public void OnInjury(Vector3 normal, bool isKnockBack)
	{
		if(ActionState == HumanActionStates.None)
		{
			if(!isKnockBack || normal == Vector3.zero)
			{
				this.MyAnimator.SetTrigger("Injure");
				if(MyAnimator.GetBool("IsAiming"))
				{
					MyAimIK.solver.IKPositionWeight = 0;
					MyAimIK.solver.SmoothEnable(1);
				}

				if(MyLeftHandIK.IsEnabled() && MyAnimator.GetInteger("WeaponType") == (int)WeaponAnimType.Longgun)
				{
					MyLeftHandIK.InstantDisable();
					MyLeftHandIK.SmoothEnable(1);
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
					MyAI.BlackBoard.ActionMovementDest = transform.position + normal.normalized * 1f; //transform.position - transform.forward * 1;
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

				PlayVocal(VocalType.Injury);
			}
		}

		_twitchTimer = 0.5f;
		ActionState = HumanActionStates.Twitch;
	}

	public void OnInjuryRecover()
	{
		_twitchTimer = 0;
		ActionState = HumanActionStates.None;
	}

	public void OnDeath()
	{
		PlayVocal(VocalType.Death);
		GameManager.Inst.NPCManager.OnHumanDeath(Killer, (Character)this);
		MyAI.OnDeath();
		Stealth.OnDeath();
		float posture = UnityEngine.Random.value;

		this.MyAnimator.SetFloat("DeathPosture", posture);
		this.MyAnimator.SetFloat("Blend", UnityEngine.Random.value);
		this.MyAnimator.SetBool("IsDead", true);

		CurrentAnimState = new HumanAnimStateDeath(this);
		IsBodyLocked = true;
		MyAimIK.solver.SmoothDisable(9);
		MyLeftHandIK.SmoothDisable(12);
		MyHeadIK.SmoothDisable(9);
		if(MyNavAgent != null)
		{
			MyNavAgent.enabled = false;
		}
		if(MyReference.Flashlight != null)
		{
			MyReference.Flashlight.Toggle(false);
		}
		MyReference.LiveCollider.enabled = false;
		MyReference.DeathCollider.enabled = true;
		MyReference.DeathCollider.gameObject.layer = 18;

		_isDeathTriggered = true;
		/*
		CapsuleCollider collider = GetComponent<CapsuleCollider>();
		collider.height = 0.5f;
		collider.radius = 0.6f;
		collider.center = new Vector3(0, 0, 0);
		collider.isTrigger = true;
		*/

		if(MyAI.ControlType == AIControlType.Player)
		{
			UIEventHandler.Instance.TriggerCloseWindow();
			GameManager.Inst.UIManager.HUDPanel.OnDeath();
			GameManager.Inst.CameraController.IsLocked = true;
			GameManager.Inst.UIManager.FadingPanel.FadeOutAndInCallBack(6, 1, 6, GameManager.Inst.LoadGame);
		}

		Unhook();
	}

	public void OnStrangledDeath()
	{
		MyStatus.Health = 0;
		GameManager.Inst.NPCManager.OnHumanDeath(Killer, (Character)this);
		MyAI.OnDeath();
		Stealth.OnDeath();

		CurrentAnimState = new HumanAnimStateDeath(this);
		IsBodyLocked = true;
		MyAimIK.solver.SmoothDisable(9);
		MyLeftHandIK.SmoothDisable(12);
		MyHeadIK.SmoothDisable(9);
		MyNavAgent.enabled = false;
		if(MyReference.Flashlight != null)
		{
			MyReference.Flashlight.Toggle(false);
		}

		CapsuleCollider collider = GetComponent<CapsuleCollider>();
		collider.height = 0.5f;
		collider.radius = 0.6f;
		collider.center = new Vector3(0, 0, 0);
		collider.isTrigger = true;


	}

	public void OnSwitchWeapon()
	{
		
		//first destroy the current weapon prefab
		GameObject.Destroy(this.MyReference.CurrentWeapon);
		this.MyReference.CurrentWeapon = null;
		if(_thrownObjectInHand != null)
		{
			GameObject.Destroy(_thrownObjectInHand.gameObject);
			_thrownObjectInHand = null;
		}

		if(_toolInHand != null)
		{
			GameObject.Destroy(_toolInHand.gameObject);
			_toolInHand = null;
		}

		this.MyStatus.ResetSpeedModifier();

		if(_weaponToSwitch != null && _weaponToSwitch.Type == ItemType.Thrown)
		{

			DrawNextGrenade();
			GameManager.Inst.SoundManager.PlayerAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("grenade_equip"), 0.1f);

		}
		else if(_weaponToSwitch != null && _weaponToSwitch.Type == ItemType.Tool)
		{
			GameObject obj = GameObject.Instantiate(Resources.Load(_weaponToSwitch.PrefabName) as GameObject);
			Tool tool = obj.GetComponent<Tool>();
			tool.transform.parent = MyReference.RightHandWeaponMount.transform;
			tool.transform.localPosition = tool.InHandPosition;
			tool.transform.localEulerAngles = tool.InHandAngles;
			_toolInHand = tool;
			GameManager.Inst.SoundManager.PlayerAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("tool_equip"), 0.2f);
		}
		else if(_weaponToSwitch != null)
		{
			
			//clear weapon in body
			if(_weaponToSwitch.Type == ItemType.PrimaryWeapon)
			{
				MyAI.WeaponSystem.ClearPrimaryWeapon();
			}
			else if(_weaponToSwitch.Type == ItemType.SideArm)
			{
				MyAI.WeaponSystem.ClearSideArm();
			}

			//load a new weapon and set position/rotation/parent
			GameObject obj = GameObject.Instantiate(Resources.Load(_weaponToSwitch.PrefabName) as GameObject);
			Weapon newWeapon = obj.GetComponent<Weapon>();
			newWeapon.transform.parent = MyReference.RightHandWeaponMount.transform;
			newWeapon.transform.localPosition = newWeapon.InHandPosition;
			newWeapon.transform.localEulerAngles = newWeapon.InHandAngles;

			/*
			GunMagazine magazine = newWeapon.GetComponent<GunMagazine>();
			if(magazine != null)
			{
				magazine.AmmoLeft = (int)_weaponToSwitch.GetAttributeByName("_LoadedAmmos").Value;
				magazine.LoadedAmmoID = (string)_weaponToSwitch.GetAttributeByName("_LoadedAmmoID").Value;
			}
			*/

			newWeapon.Attacker = this;
			newWeapon.Rebuild(OnSuccessfulShot, _weaponToSwitch);

			bool isRanged = (bool)_weaponToSwitch.GetAttributeByName("_IsRanged").Value;
			if(isRanged)
			{
				this.MyAimIK.solver.transform = newWeapon.AimTransform;
				AimTransform = newWeapon.AimTransform;
				this.MyLeftHandIK.Target = newWeapon.ForeGrip;
			}
			else
			{
				this.MyAimIK.solver.transform = MyReference.TorsoWeaponMount.transform;
				AimTransform = MyReference.TorsoWeaponMount.transform;
			}

			this.MyReference.CurrentWeapon = obj;

			float lessSpeed = newWeapon.GetTotalLessMoveSpeed();

			this.MyStatus.RunSpeedModifier = Mathf.Clamp(this.MyStatus.RunSpeedModifier - lessSpeed, 0.9f, 1.2f);
			this.MyStatus.SprintSpeedModifier = Mathf.Clamp(this.MyStatus.SprintSpeedModifier, 0.9f, 1.1f);
			this.MyStatus.StrafeSpeedModifier = Mathf.Clamp(this.MyStatus.StrafeSpeedModifier - lessSpeed, 0.9f, 1.2f);

			if(MyAI.ControlType == AIControlType.Player)
			{
				if(!isRanged)
				{
					GameManager.Inst.SoundManager.PlayerAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("melee_equip"), 0.1f);
				}
				else if(_weaponToSwitch.Type == ItemType.PrimaryWeapon)
				{	
					GameManager.Inst.SoundManager.PlayerAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("longgun_equip"), 0.1f);
				}
				else
				{
					GameManager.Inst.SoundManager.PlayerAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("handgun_equip"), 0.1f);
				}
			}
		}
		else
		{
			this.MyReference.CurrentWeapon = null;
		}

		this.MyAI.BlackBoard.EquippedWeapon = _weaponToSwitch;

		this.MyAI.WeaponSystem.LoadWeaponsFromInventory(false);

		if(IsHidden)
		{
			HideCharacter(false);
		}

		if(_weaponToSwitch != null)
		{
			MyEventHandler.TriggerOnSelectWeapon(_weaponToSwitch.ID);
		}
	}

	public void OnMeleeStrikeHalfWay()
	{
		ActionState = HumanActionStates.Melee;
		_meleeStrikeStage = 1;
		if(MyReference.CurrentWeapon != null)
		{
			MyReference.CurrentWeapon.GetComponent<MeleeWeapon>().SwingStart();
		}



		Debug.Log("on strike half way ");
	}

	public void OnMeleeComboStageOne()
	{
		ActionState = HumanActionStates.Melee;
		_meleeStrikeStage = 1;
		if(MyReference.CurrentWeapon != null)
		{
			MyReference.CurrentWeapon.GetComponent<MeleeWeapon>().SwingStart();
		}
	}

	public void OnMeleeComboStageTwo()
	{
		ActionState = HumanActionStates.Melee;
		_meleeStrikeStage = 1;
		MyReference.CurrentWeapon.GetComponent<MeleeWeapon>().SwingStop();
		OnAnimationActionEnd();
		//IsMoveLocked = true;
		_isComboAttack = false;
	}



	public void OnMeleeStrikeLeftFinish()
	{
		ActionState = HumanActionStates.None;
		_meleeStrikeStage = 0;
		IsMoveLocked = false;
		_isComboAttack = false;
		MyHeadIK.SmoothEnable(9);
		MyReference.CurrentWeapon.GetComponent<MeleeWeapon>().SwingStop();
		Debug.Log("strike left finished");
	}

	public void OnMeleeStrikeRightFinish()
	{
		ActionState = HumanActionStates.None;
		_meleeStrikeStage = 0;
		IsMoveLocked = false;
		IsBodyLocked = false;
		_isComboAttack = false;

		MyHeadIK.SmoothEnable(9);
		if(MyReference.CurrentWeapon != null)
		{
			if(MyReference.CurrentWeapon.GetComponent<MeleeWeapon>() != null)
			{
				MyReference.CurrentWeapon.GetComponent<MeleeWeapon>().SwingStop();
			}
			else
			{
				if(UpperBodyState == HumanUpperBodyStates.Aim)
				{
					MyAimIK.solver.SmoothEnable(1f);
				}
				else
				{
					SendCommand(CharacterCommands.StopAim);
					MyHeadIK.SmoothEnable();
				}

				if(MyAnimator.GetInteger("WeaponType") == (int)WeaponAnimType.Longgun)
				{
					MyLeftHandIK.SmoothEnable(6);
				}
				else
				{
					if(UpperBodyState == HumanUpperBodyStates.Aim)
					{
						MyLeftHandIK.SmoothEnable(6);
					}
					else
					{
						MyLeftHandIK.SmoothDisable();
					}
				}
			}
		}

		if(MyReference.RightFoot != null)
		{
			MyReference.RightFoot.SetActive(false);
		}

		Debug.Log("strike right finished");
	}

	public void OnMeleeBlockFinish()
	{
		this.MyLeftHandIK.InstantDisable();
		ActionState = HumanActionStates.None;
		_meleeStrikeStage = 0;
		IsMoveLocked = false;
	}

	public void OnAnimationActionEnd()
	{
		SendCommand(CharacterCommands.AnimationActionDone);
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


		if(MyAI.ControlType == AIControlType.Player)
		{
			if(!MyCC.isGrounded)
			{
				return;
			}
			float speed = MyCC.velocity.magnitude;
			string type = "run";
			float volume = MyCC.velocity.magnitude / 5 * 0.3f;
			if(speed < 2.5f)
			{
				type = "walk";
			}

			int choice = UnityEngine.Random.Range(1, 7);
			FloorType floorType = GameManager.Inst.PlayerControl.CurrentFloorType;
			this.CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(type + "_" + floorType.ToString() + choice), volume);
		}
		else
		{
			int choice = UnityEngine.Random.Range(1, 7);
			this.CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("run_dirt" + choice), 0.3f);
		}
	}

	public void OnFallLanding(float speed, float threshold)
	{
		float volume = speed / threshold * 0.3f;
		int choice1 = UnityEngine.Random.Range(1, 7);
		int choice2 = UnityEngine.Random.Range(1, 7);
		int choice3 = UnityEngine.Random.Range(1, 4);
		FloorType floorType = GameManager.Inst.PlayerControl.CurrentFloorType;
		this.CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("run_" + floorType.ToString() + choice1), volume);
		this.CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("run_" + floorType.ToString() + choice2), volume);
		if(speed > threshold)
		{
			this.CharacterAudio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("fall_bonebreak" + choice3), 0.15f);
		}
	}

	#endregion

	#region Public functions

	public void RefreshBackpackProp()
	{
		//if there's already a backpack, destroy it first
		if(_backpackProp != null)
		{
			GameObject.Destroy(_backpackProp);
		}
		//check if inventory has backpack items. if so, take the first one and generate a prop
		GridItemData item = this.Inventory.FindItemInBackpack(ItemType.SupplyPack);
		if(item != null)
		{
			GameObject pack = GameObject.Instantiate(Resources.Load(item.Item.SpriteName)) as GameObject;
			pack.transform.parent = MyReference.SlingWeaponMount.transform;
			pack.transform.localPosition = new Vector3(0, 0.03f, -0.301f);
			pack.transform.localEulerAngles = new Vector3(-8.37f, 0, 0);
			_backpackProp = pack;
		}
	}

	#endregion

	#region Private functions

	private void DrawNextGrenade()
	{
		//first remove anything in hand already
		if(_thrownObjectInHand != null)
		{
			GameObject.Destroy(_thrownObjectInHand.gameObject);
		}

		//check what kind of item is in thrown slot
		Item throwItem = this.Inventory.ThrowSlot;
		if(throwItem == null)
		{
			//we are going to throw a rock
			_thrownObjectInHand = ((GameObject)GameObject.Instantiate(Resources.Load("ThrowingRock"))).GetComponent<ThrownObject>();
			MyEventHandler.TriggerOnSelectWeapon("rock");
		}
		else
		{
			
			_thrownObjectInHand = ((GameObject)GameObject.Instantiate(Resources.Load(throwItem.PrefabName))).GetComponent<ThrownObject>();
		}


		_thrownObjectInHand.ThrownItem = throwItem;
		_thrownObjectInHand.Thrower = this;
		Explosive explosive = _thrownObjectInHand.GetComponent<Explosive>();
		if(explosive != null)
		{
			explosive.Attacker = this;
		}

		_thrownObjectInHand.GetComponent<Rigidbody>().isKinematic = true;

		_thrownObjectInHand.transform.parent = this.MyReference.RightHandWeaponMount.transform;
		_thrownObjectInHand.transform.localPosition = _thrownObjectInHand.InHandPosition;
		_thrownObjectInHand.transform.localEulerAngles = _thrownObjectInHand.InHandRotation;

		HandGrenade grenadeComponent = _thrownObjectInHand.GetComponent<HandGrenade>();
		if(grenadeComponent != null)
		{
			grenadeComponent.SetExplosive(throwItem);
		}

		//this.MyAimIK.solver.transform = _thrownObjectInHand.transform.Find("AimTransform");
		//AimTransform = this.MyAimIK.solver.transform;

		this.MyAimIK.solver.transform = this.MyReference.TorsoWeaponMount.transform;
		AimTransform = this.MyAimIK.solver.transform;

	}



	private void UpdateLayerWeights()
	{
		if(_layerDState)
		{
			this.MyAnimator.SetLayerWeight(this.MyAnimator.GetLayerIndex("FullBodyOverride-D"), 
				Mathf.Lerp(this.MyAnimator.GetLayerWeight(this.MyAnimator.GetLayerIndex("FullBodyOverride-D")), 1, Time.deltaTime * 1));
		}
		else
		{
			this.MyAnimator.SetLayerWeight(this.MyAnimator.GetLayerIndex("FullBodyOverride-D"), 
				Mathf.Lerp(this.MyAnimator.GetLayerWeight(this.MyAnimator.GetLayerIndex("FullBodyOverride-D")), 0, Time.deltaTime * 1));
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
		Vector3 destDir = Vector3.zero;

		destDir = this.GetCharacterVelocity(); //this.Destination.Value - transform.position;


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

		if(MyReference.Flashlight != null)
		{
			MyReference.Flashlight.Light.intensity = 0;
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

		if(MyReference.Flashlight != null)
		{
			MyReference.Flashlight.Light.intensity = 2.4f;
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
			if(playerAngle > 80 && playerDist > 3f && MyStatus.Health > 0)
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

	#endregion


	#region Coroutines

	IEnumerator WaitAndCallback(float waitTime, Character.DelayCallBack callback, object parameter)
	{
		yield return new WaitForSeconds(waitTime);
		callback(parameter);
	}

	IEnumerator WaitAndMuzzleClimb(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		if(this.MyReference.CurrentWeapon != null)
		{
			float recoil = this.MyReference.CurrentWeapon.GetComponent<Gun>().GetRecoil();
			/*
			if(MyAI.ControlType == AIControlType.Player)
			{
				MyStatus.ChangeStamina(Mathf.Clamp(recoil, 0, 0.8f) * 5 * -1);
				if(MyStatus.IsResting)
				{
					recoil *= 1.5f;
				}
			}
			*/
			if(AimTarget.localPosition.y < 0.1f)
			{
				float armFatigueRate = this.MyStatus.ArmFatigue / this.MyStatus.MaxArmFatigue;
				float climb = Mathf.Clamp(recoil * (GameManager.Inst.Constants.ArmFatigueRecoil.Evaluate(armFatigueRate)), 0, 5);
				AimTarget.localPosition += new Vector3(0, climb, 0);
			}
			else
			{
				float maxSpread = Mathf.Clamp(recoil * (this.MyStatus.ArmFatigue / this.MyStatus.MaxArmFatigue), 0, 0.5f) / 2;
				AimTarget.localPosition = new Vector3(0, AimTarget.localPosition.y, 2);
				//AimTarget.localPosition += new Vector3(UnityEngine.Random.Range(-1 * maxSpread, maxSpread), 0, 0);
			}


		}

	}

	IEnumerator WaitAndCreateThrowObject(float waitTime, string objectName)
	{
		yield return new WaitForSeconds(waitTime);



	}

	#endregion
}
