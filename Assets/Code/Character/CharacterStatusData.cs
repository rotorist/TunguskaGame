using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStatusData
{

	public float WalkSpeed;
	public float StrafeSpeed;
	public float RunSpeed;
	public float SprintSpeed;

	public float WalkSpeedModifier; //0.6 to 1.5
	public float RunSpeedModifier; //0.9 to 1.2
	public float SprintSpeedModifier; //0.9 to 1.1
	public float StrafeSpeedModifier; //0.8 to 1.2

	public float MaxCarryWeight;
	public float CarryWeight;

	public float MaxArmFatigue;
	public float ArmFatigue;

	public float BleedingSpeed;
	public float BleedingDuration;



	public float MaxHealth;
	public float Health;

	public float MaxStamina;
	public float Stamina;
	public float StaminaRestoreSpeed;
	public float StaminaReduceMult;
	public bool IsResting;

	public float MaxEnergy;
	public float Energy;

	public float Infection;
	public float Radiation;//how much player has been irradiated
	public float RadiationDefense;

	public float EyeSight;

	public int Intelligence; //0, 1, 2
	public float MutantMovementBlend;
}
