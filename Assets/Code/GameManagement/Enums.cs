

public enum PlayerMoveDirEnum
{
	Stop,
	Left,
	Right,
	Up,
	Down,
}

public enum HumanBodyStates
{
	StandIdle,
	WalkForward,
	WalkAiming,//when aiming opposite of movement direction, must be walkAiming
	RunForward,
	RunAiming,
	Sprint,
	CrouchIdle,
	CrouchWalk,
	CrouchWalkAiming,
	Knock,
}

public enum MutantBodyStates
{
	Idle,
	Move,
}

public enum HumanUpperBodyStates
{
	None,
	Idle,
	Aim,
	HalfAim,
}

public enum MutantUpperBodyStates
{
	None,
	Idle,
	Aim,
}

public enum HumanActionStates
{
	None,
	Reload,
	Twitch,
	Throw,
	SwitchWeapon,
	Strangle,
	DisguiseBody,
	Kick,
	Melee,
	Block,
}

public enum CharacterCommands
{
	Idle,
	GoToPosition,
	GoDirection,
	Walk,
	StopWalk,
	Sprint,
	StopSprint,
	Crouch,
	StopCrouch,
	Aim,
	HipAim,
	StopAim,
	PullTrigger,
	ReleaseTrigger,
	Reload,
	FinishReload,
	CancelReload,
	Unarm,
	SwitchWeapon1,
	SwitchWeapon2,
	SwitchThrown,
	SwitchTool,
	Kick,
	Throw,
	LowThrow,
	ThrowGrenade,
	FinishThrow,
	UseTool,
	Cancel,
	Die,
	Talk,
	Pickup,
	Loot,
	LootChest,
	Use,
	SetAlert,
	IdleAction,
	LeftAttack,
	RightAttack,
	QuickAttack,
	Block,
	RunningAttack,
	PlayAnimationAction,
	AnimationActionDone,
	Bite,
}

public enum HumanStances
{
	Walk,
	Crouch,
	CrouchRun,
	Run,
	Sprint,
}

public enum PartyTasks
{
	Default,
	GoToGuard,
	SprintToGuard,
	Follow,
	AttackTarget,
	Grenade,
	Cancel,
	ToggleCrouch,
	ToggleHoldFire,
}


public enum WeaponAnimType
{
	Unarmed=0,
	Pistol=1,
	Longgun=2,
	Grenade=-1,
	Tool=-2,
	Melee=3,
}

public enum ReloadAnimType
{
	SemiAuto,
	Revolver,
	Rifle,
	Shotgun,
}

public enum FXType
{
	BulletHole,
	BulletImpact,
	BloodSpatter,
	Explosion,
}

public enum GunFireModes
{
	Semi,
	Full,
	Burst,
	Pump,
	Bolt,
}

public enum GunCalibers
{
	m9x19,
	m9x18,
	m9x39,
	m762x39,
	m762x54R,
	m556x45,
	i45,
	i357,
	i44,
	i308,
	g12,
}

public enum DamageType
{
	Bullet,
	Explosive,
	Melee,
}

public enum AIControlType
{
	Player,
	PlayerTeam,
	NPC,
}

public enum Faction
{
	Civilian,
	FactionA,
	FactionB,
	Player,
	Bootleggers,
	Loner,
	Military,
	Legionnaires,
	Mutants,
	Animals,
}

public enum ParseStates
{
	Normal,
	Response,

}

public enum AnimationActions
{
	ComboAttack,
	KnockBack,
	KnockForward,
	ChairSit,
	ChairStand,
	GroundSit,
	GroundStand,
	Sleep,
	SleepStand,
	ApplyBandages,
	Landing,
	HardLanding,
	CommanderStand,
	LostBalance,
}

public enum CharStateType
{
	Health,
	Stamina,
	Energy,
	Infection,
	Radiation,
}

public enum WeaponHolster
{
	Back,
	Side,
}

public enum MeleeSoundType
{
	Limb,
	Blade,
	Blunt,
}

public enum VocalSet
{
	player,
	bandit,
	soldier,
	zombiemale,
	zombiefemale,
	scythe,
}

public enum VocalType
{
	Surprise,
	Injury,
	Death,
	Coverfire,
	Grenade,
	Search,
}

public enum FloorType
{
	dirt,
	mud,
	grass,
	rock,
	leaf,
	metal,
	hardwood,
	softwood,
	sand,
}

public enum BlendMode
{
	Opaque,
	Cutout,
	Fade,
	Transparent
}

public enum AimedObjectType
{
	None,
	Door,
	Friendly,
	Enemy,
	PickupItem,
	Body,
	Chest,
	Interactive,
	LightSwitch,
}

public enum LogicOperator
{
	And,
	Or,

}

public enum DoorType
{
	Rotating,
	Sliding,

}

public enum NPCJobs
{
	None,
	Trader,
	Mechanic,
	Commander,
	Guard,
	Patrol,
}