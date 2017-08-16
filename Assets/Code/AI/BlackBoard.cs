using UnityEngine;
using System.Collections;

public class BlackBoard
{
	public Vector3 AimPoint;

	private Vector3 _navTarget;
	public bool IsNavTargetSet;
	public Vector3 NavTarget
	{
		get { return _navTarget;}
		set { _navTarget = value; }
	}

	private Character _followTarget;
	public Character FollowTarget
	{
		get { return _followTarget; }
		set { _followTarget = value; }
	}

	private Character _targetEnemy;
	public Character TargetEnemy
	{
		get { return _targetEnemy; }
		set { _targetEnemy = value; }
	}

	public bool IsTargetLocked;

	//public bool IsTargetEnemyHittable;
	public float TargetEnemyThreat;

	public Vector3 LastKnownEnemyPosition;
	private Character _invisibleEnemy;
	public Character InvisibleEnemy
	{
		get { return _invisibleEnemy; }
		set { _invisibleEnemy = value; }
	}

	public float HighestPersonalThreat;
	public Vector3 AvgPersonalThreatDir;

	public Cover SelectedCover;
	public Vector3 SelectedCoverLoc;

	public bool HasPatrolInfo;
	private Vector3 _patrolLoc;
	public Vector3 PatrolLoc
	{
		get { return _patrolLoc; }
		set { _patrolLoc = value; }
	}
	public int PatrolNodeIndex;

	private Vector3 _patrolRange;
	public Vector3 PatrolRange
	{
		get { return _patrolRange; }
		set { _patrolRange = value; }
	}

	private Vector3 _combatRange;
	public Vector3 CombatRange
	{
		get { return _combatRange; }
		set { _combatRange = value; }
	}

	private Vector3 _defensePoint;
	public Vector3 DefensePoint
	{
		get { return _defensePoint; }
		set { _defensePoint = value; }
	}

	private float _defenseRadius;
	public float DefenseRadius
	{
		get { return _defenseRadius; }
		set { _defenseRadius = value; }
	}


	private Weapon _focusedWeapon;
	public Weapon FocusedWeapon
	{
		get { return _focusedWeapon; }
		set { _focusedWeapon = value; }
	}

	private Item _equippedWeapon;
	public Item EquippedWeapon
	{
		get { return _equippedWeapon; }
		set { _equippedWeapon = value; }
	}

	public Vector3 HighestDisturbanceLoc;
	public float HighestDisturbanceThreat;
	public object HightestDisturbanceSource;

	public int NumberOfKnownEnemies;

	public int GuardLevel; //0-3; 0 means ignore all events; 1 means not on guard (patrolling); 2 means expecting trouble; 3 means looking for trouble (attacking)
	public Vector3 GuardDirection;
	public int GuardConfigStage; //0, 1, 2

	public bool IsGrenadePending;//this is set when player issues grenade command; after unpausing will be unset
	public Vector3 PendingGrenadeTarget;

	public WorkingMemoryFact TargetCorpse;

	public PickupItem PickupTarget;
	public Character InteractTarget;
	public GameObject UseTarget;
	public CharacterCommands PendingCommand;

	public AnimationActions AnimationAction;
	public Vector3 ActionMovementDest;
	public float ActionMovementSpeed;

	public int NeedToPullOut;
}
