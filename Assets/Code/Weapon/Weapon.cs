using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{
	public Character Attacker;
	public Vector3 InHandPosition;
	public Vector3 InHandAngles;
	public Vector3 InHolsterPosition;
	public Vector3 InHolsterAngles;
	public WeaponHolster HolsterLocation;
	public Transform AimTransform;
	public Transform ForeGrip;
	public bool IsRanged;
	public bool IsScoped;
	public bool IsTwoHanded;
	public int AimPosition;//0 is torso, 1 is head
	public Item WeaponItem;

	public delegate void WeaponCallBack();

	public virtual void Rebuild(WeaponCallBack callBack, Item weaponItem)
	{

	}

	public virtual void Refresh()
	{

	}

	public virtual float GetTotalLessMoveSpeed()
	{
		return 0;
	}
}

