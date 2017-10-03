using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour
{
	public AnimationCurve MeleeDamageVsEnergy; //damage multiplier based on attacker's energy
	public AnimationCurve MeleeDamageVsDurability;
	public AnimationCurve GunDamageVsDurability;
	public AnimationCurve GunJamVsDurability;
	public AnimationCurve GunAccuVsDurability;
	public AnimationCurve ArmFatigueRecoil;
	public AnimationCurve RarityDropRate;

	public float DurabilityDrainRate; //reduced from durability each time weapon is used
}