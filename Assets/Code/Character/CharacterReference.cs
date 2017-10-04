using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

[RequireComponent(typeof(AnimationEventHandler))]
[RequireComponent(typeof(LeftHandIKControl))]
[RequireComponent(typeof(HeadIKControl))]
[RequireComponent(typeof(AimIK))]




public class CharacterReference : MonoBehaviour
{

	public GameObject HelmetMount;
	public GameObject RightHandWeaponMount;
	public GameObject TorsoWeaponMount;
	public GameObject SlingWeaponMount;
	public GameObject HolsterWeaponMount;
	public GameObject CurrentWeapon;
	public GameObject Eyes;
	public Character ParentCharacter;
	public FlashLight Flashlight;
	public GameObject FlashlightMount;
	public Weapon FixedMeleeLeft;
	public Weapon FixedMeleeRight;
	public BoxCollider DeathCollider;
	public CapsuleCollider LiveCollider;
	public FootKickCollider RightFoot;
	public VocalSet VocalSet;
	public bool IsHeavy;
	public Renderer [] FadingRenderers;
	public int GoapID;
}
