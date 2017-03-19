using UnityEngine;
using System.Collections;

public class Noise
{
	public Vector3? Location;
	public float Volume;
	public NoiseTypeEnum NoiseType;
	public bool Enabled;
	public Character SourceCharacter;

	public Noise()
	{
		Location = null;
		Volume = 0;
		NoiseType = NoiseTypeEnum.Steps;
		Enabled = false;
		SourceCharacter = null;
	}
}

public enum NoiseTypeEnum
{
	Gunfire,
	Steps,
	HumanSpeech,
	HumanShout,
	ZombieMoan,
	ZombieShout,
	Impact,
}