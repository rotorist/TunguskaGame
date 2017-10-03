using UnityEngine;
using System.Collections;


public class CharacterStatus : MonoBehaviour
{
	public Character ParentCharacter;

	public CharacterStatusData Data;

	public float WalkSpeed { get { return Data.WalkSpeed; } set { Data.WalkSpeed = value; } }
	public float StrafeSpeed { get { return Data.StrafeSpeed; } set { Data.StrafeSpeed = value; } }
	public float RunSpeed { get { return Data.RunSpeed; } set { Data.RunSpeed = value; } }
	public float SprintSpeed { get { return Data.SprintSpeed; } set { Data.SprintSpeed = value; } }

	public float WalkSpeedModifier { get { return Data.WalkSpeedModifier; } set { Data.WalkSpeedModifier = value; } } //0.6 to 1.5
	public float RunSpeedModifier { get { return Data.RunSpeedModifier; } set { Data.RunSpeedModifier = value; } } //0.9 to 1.2
	public float SprintSpeedModifier { get { return Data.SprintSpeedModifier; } set { Data.SprintSpeedModifier = value; } } //0.9 to 1.1
	public float StrafeSpeedModifier { get { return Data.StrafeSpeedModifier; } set { Data.StrafeSpeedModifier = value; } } //0.8 to 1.2

	public float MaxCarryWeight { get { return Data.MaxCarryWeight; } set { Data.MaxCarryWeight = value; } }
	public float CarryWeight { get { return Data.CarryWeight; } set { Data.CarryWeight = value; } }

	public float MaxArmFatigue { get { return Data.MaxArmFatigue; } set { Data.MaxArmFatigue = value; } }
	public float ArmFatigue { get { return Data.ArmFatigue; } set { Data.ArmFatigue = value; } }

	public float BleedingSpeed { get { return Data.BleedingSpeed; } set { Data.BleedingSpeed = value; } }
	public float BleedingDuration { get { return Data.BleedingDuration; } set { Data.BleedingDuration = value; } }



	public float MaxHealth { get { return Data.MaxHealth; } set { Data.MaxHealth = value; } }
	public float Health { get { return Data.Health; } set { Data.Health = value; } }

	public float MaxStamina { get { return Data.MaxStamina; } set { Data.MaxStamina = value; } }
	public float Stamina { get { return Data.Stamina; } set { Data.Stamina = value; } }
	public float StaminaRestoreSpeed { get { return Data.StaminaRestoreSpeed; } set { Data.StaminaRestoreSpeed = value; } }
	public float StaminaReduceMult { get { return Data.StaminaReduceMult; } set { Data.StaminaReduceMult = value; } }
	public bool IsResting { get { return Data.IsResting; } set { Data.IsResting = value; } }

	public float MaxEnergy { get { return Data.MaxEnergy; } set { Data.MaxEnergy = value; } }
	public float Energy { get { return Data.Energy; } set { Data.Energy = value; } }

	public float Infection { get { return Data.Infection; } set { Data.Infection = value; } }
	public float Radiation { get { return Data.Radiation; } set { Data.Radiation = value; } } //how much player has been irradiated
	public float RadiationDefense { get { return Data.RadiationDefense; } set { Data.RadiationDefense = value; } }

	public float EyeSight { get { return Data.EyeSight; } set { Data.EyeSight = value; } }

	public int Intelligence { get { return Data.Intelligence; } set { Data.Intelligence = value; } } //0, 1, 2
	public float MutantMovementBlend { get { return Data.MutantMovementBlend; } set { Data.MutantMovementBlend = value; } }



	private float _prevMovementSpeed;
	private float _staminaStunTimer;


	public void Initialize()
	{
		Data = new CharacterStatusData();
		
		WalkSpeed = 1f;
		StrafeSpeed = 1.75f;
		RunSpeed = 4f;
		SprintSpeed = 5.2f;

		ArmFatigue = 0;
		MaxArmFatigue = 5;

		ResetSpeedModifier();

		MaxStamina = 100;
		Stamina = 100;
		StaminaRestoreSpeed = 6;
		StaminaReduceMult = 1;

		MaxHealth = 160;
		Health = 160;

		Energy = 3000;
		MaxEnergy = 3000;

		EyeSight = 1.5f;

		Intelligence = 2;

		RadiationDefense = 10;

		MaxCarryWeight = 40.0f;
	}

	public void ResetSpeedModifier()
	{
		WalkSpeedModifier = 1f;
		RunSpeedModifier = 1f;
		SprintSpeedModifier = 1.1f;
		StrafeSpeedModifier = 1.2f;

	}

	public void UpdateBodyStatusEveryone()
	{
		//arm fatigue
		ArmFatigue -= Time.deltaTime * 3;
		if(ArmFatigue < 0)
		{
			ArmFatigue = 0;
		}

		//bleeding
		if(BleedingDuration > 0)
		{
			BleedingDuration -= Time.deltaTime;
			Health -= BleedingSpeed * Time.deltaTime;
		}
		else
		{
			BleedingDuration = 0;
		}



	}

	public void UpdateBodyStatusPlayer()
	{
		HumanCharacter player = GameManager.Inst.PlayerControl.SelectedPC;
		float movementSpeed = player.GetComponent<CharacterController>().velocity.magnitude;

		//stamina
		if(player.CurrentStance == HumanStances.Sprint && movementSpeed > 0.1f)
		{
			//reduce stamina
			float delta = Time.deltaTime * 20 * StaminaReduceMult;
			Stamina -= delta;
			Energy -= delta * 1f;
			if(Stamina <= 0)
			{
				player.SendCommand(CharacterCommands.StopSprint);
			}
		}
		else if(MaxCarryWeight < CarryWeight && movementSpeed > 0.1f)
		{
			float delta = Time.deltaTime * 5 * StaminaReduceMult;
			Stamina -= delta;

			Energy -= delta * 1f;

		}
		else if(player.ActionState == HumanActionStates.None && (player.UpperBodyState == HumanUpperBodyStates.None || player.UpperBodyState == HumanUpperBodyStates.Idle))
		{
			float delta = 0;
			if(movementSpeed > 0.1f)
			{
				delta = Time.deltaTime * StaminaRestoreSpeed * 0.5f;
				Stamina += delta;
			}
			else
			{
				delta = Time.deltaTime * StaminaRestoreSpeed;
				Stamina += delta;
			}


		}

		if(Stamina < 0)
		{
			Stamina = 0;
			IsResting = true;
		}
		else if(Stamina > MaxStamina)
		{
			Stamina = MaxStamina;
		}

		if(IsResting && Stamina > 20)
		{
			IsResting = false;
		}

		if(Energy < 0)
		{
			Energy = 0;
		}
		else if(Energy > MaxEnergy)
		{
			Energy = MaxEnergy;
		}
	}

	public void AddBleeding(float bleedingValue)
	{
		//first calculate the chance of bleeding based on bleeding value
		if(UnityEngine.Random.value > bleedingValue)
		{
			return;
		}
		
		BleedingDuration = UnityEngine.Random.Range(5f, 30f) * bleedingValue;
		//multiply bleeding value by 10 and randomize within 0.8x - 1.2x
		BleedingSpeed = BleedingSpeed + UnityEngine.Random.Range(5 * bleedingValue * 0.8f, 5 * bleedingValue * 1.2f) * (1 - BleedingSpeed / 5);

		if(ParentCharacter.MyAI.ControlType == AIControlType.Player)
		{
			BleedingDuration *= 3;
			BleedingSpeed /= 3;
		}

		BleedingSpeed = Mathf.Clamp(BleedingSpeed, 0, 5);
	}

	public void ReduceBleeding(float value)
	{
		BleedingSpeed -= value;
		if(BleedingSpeed <= 0)
		{
			BleedingSpeed = 0;
			BleedingDuration = 0;
		}
	}

	public void ChangeStamina(float delta)
	{
		Stamina = Stamina + delta;
		if(Stamina > MaxStamina)
		{
			Stamina = MaxStamina;
		}
		else if(Stamina < 0)
		{
			Stamina = 0;
			IsResting = true;
		}

		if(delta < 0)
		{
			Energy += delta * 1f;
			if(Energy < 0)
			{
				Energy = 0;
			}
		}
	}

	public void SetEnergy(float energy)
	{
		if(energy > MaxEnergy)
		{
			Energy = MaxEnergy;
		}
		else if(energy < 0)
		{
			Energy = 0;
		}
		else
		{
			Energy = energy;
		}
	}
}
