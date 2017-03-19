using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public abstract class Character : MonoBehaviour
{
	public int GoapID;
	public string CharacterID;
	public GameObject Model;

	public bool IsHuman;
	public Vector3? Destination;
	public Vector3 AimPoint;

	public Animator MyAnimator;

	public Faction Faction;
	public int LonerSquadID;


	public CharacterReference MyReference;
	public CharacterEventHandler MyEventHandler;
	public AnimationEventHandler MyAnimEventHandler;

	public string CurrentAnimStateName;

	public CharacterStatus MyStatus;
	public bool IsBodyLocked;
	public bool IsMoveLocked;

	public ArmorSystem ArmorSystem;
	public AI MyAI;

	public UnityEngine.AI.NavMeshAgent MyNavAgent;


	public Transform AimTarget;
	public Transform AimTargetRoot;
	public Transform AimTransform;
	public Transform LookTarget;

	public HumanStances CurrentStance;
	public HumanActionStates ActionState;
	public CharacterStealth Stealth;

	public CharacterInventory Inventory;

	public DamageType DeathReason;
	public bool IsLooted;

	public AudioSource CharacterAudio;

	public bool IsHidden;

	public delegate void DelayCallBack(object parameter);

	public abstract bool IsAlive
	{
		get;
	}
		
	public abstract void Initialize();
	public abstract void LoadCharacterModel(string prefabName);
	public abstract void SendDelayCallBack(float delay, DelayCallBack callback, object parameter);
	public abstract void SendCommand(CharacterCommands command);
	public abstract bool SendDamage(Damage damage, Vector3 hitNormal, Character attacker, Weapon attackerWeapon);
	public abstract bool SendMeleeDamage (Damage damage, Vector3 hitNormal, Character attacker, float knockBackChance);
	public abstract void Unhook();
	public abstract void PlayVocal(VocalType vocalType);
	public abstract Vector3 GetCharacterVelocity();

	public abstract void OnSuccessfulHit(Character target);
	/*
	public void HandleNavAgentMovement()
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agent.SetDestination(Destination.Value);
		
		if(IsSneaking)
		{
			agent.speed = MovingSpeed * 0.3f;
		}
		else
		{
			
			agent.speed = MovingSpeed * MovingSpeedMultiplier;
		}
		
		if(IsSneaking)
		{
			Noise.Volume = 0.2f;
		}
		else
		{
			Noise.Volume = 0.4f;
		}
		Noise.Location = transform.position;
		SoundEventHandler.Instance.TriggerNoiseEvent(Noise);
		
		if(!IsWeaponFiring && !IsWeaponAiming)
		{
			GetComponent<NavMeshAgent>().updateRotation = true;
		}
	}
	*/

}
