using UnityEngine;
using System.Collections;

public class WaveDefenseQuest : QuestBase
{

	private int _interWaveTimer;
	private bool _isWaveStarted;


	public override void StartQuest ()
	{
		
		this.Stage = 6;
		_interWaveTimer = 5;

	}

	public override void StopQuest ()
	{
		
	}

	public override void PerSecondUpdate ()
	{
		
		if(_interWaveTimer > 0)
		{
			_interWaveTimer --;
		}

		if(_interWaveTimer >= 0)
		{
			GameManager.Inst.UIManager.HUDPanel.Clock.text = "00:00:0" + (_interWaveTimer);
		}

		if(_interWaveTimer <= 0 && !_isWaveStarted)
		{
			if(this.Stage == 0)
			{
				int waveSize = 1;
				Faction faction = Faction.Legionnaires;
				SpawnWave(waveSize, faction, false);
			}
			else if(this.Stage <= 2)
			{
				int waveSize = 1;
				Faction faction = Faction.Mutants;
				SpawnWave(waveSize, faction, false);
			}
			else if(this.Stage <= 4)
			{
				int waveSize = 1;
				Faction faction = Faction.Legionnaires;
				SpawnWave(waveSize, faction, false);
			}
			else if(this.Stage == 5)
			{
				int waveSize = 3;
				Faction faction = Faction.Mutants;
				SpawnWave(waveSize, faction, true);
			}
			else if(this.Stage <= 7)
			{
				int waveSize = UnityEngine.Random.Range(1, 4);
				Faction faction = Faction.Military;
				SpawnWave(waveSize, faction, false);
			}
			else
			{
				int waveSize = UnityEngine.Random.Range(3, 6);
				float rand = UnityEngine.Random.value;
				if(rand < 0.4f)
				{
					Faction faction = Faction.Mutants;
					SpawnWave(waveSize, faction, true);
				}
				else if(rand < 0.75f)
				{
					Faction faction = Faction.Legionnaires;
					SpawnWave(waveSize, faction, true);
				}
				else
				{
					Faction faction = Faction.Military;
					SpawnWave(waveSize, faction, true);
				}
			}

			GameManager.Inst.UIManager.HUDPanel.WaveIndicator.text = "Wave " + this.Stage + " Incoming!";
			GameManager.Inst.UIManager.HUDPanel.WaveIndicator.color = Color.yellow;
			_isWaveStarted = true;
		}

		//check if wave is cleared
		if(Stage > 0 && _isWaveStarted && GameManager.Inst.NPCManager.GetLivingHumansCount() <= 1 && GameManager.Inst.NPCManager.GetLivingMutantsCount() <= 0)
		{
			GameManager.Inst.UIManager.HUDPanel.WaveIndicator.text = "Wave " + this.Stage + " Completed!";
			GameManager.Inst.UIManager.HUDPanel.WaveIndicator.color = Color.green;
			_interWaveTimer = 5;

			this.Stage ++;
			_isWaveStarted = false;

		}
		else if(Stage == 0)
		{
			if(GameManager.Inst.PlayerControl.SelectedPC.transform.position.z > -2)
			{
				_interWaveTimer = 5;

				this.Stage = 6;
				_isWaveStarted = false;
			}
		}
	}


	private void SpawnWave(int waveSize, Faction faction, bool isMultiSpawn)
	{
		AISquad enemySquad = new AISquad();
		enemySquad.Faction = faction;

		//select a spawn point
		GameObject [] spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
		int point = UnityEngine.Random.Range(0, spawnPoints.Length);



		for(int i=0; i<waveSize; i++)
		{
			if(isMultiSpawn)
			{
				point = UnityEngine.Random.Range(0, spawnPoints.Length);
			}

			if(faction == Faction.Legionnaires)
			{
				enemySquad.AddMember(GameManager.Inst.NPCManager.SpawnRandomHumanCharacter("Bandit", enemySquad, spawnPoints[point].transform.position 
					+ new Vector3(UnityEngine.Random.value * 3, 0, UnityEngine.Random.value * 3)));
			}
			else if(faction == Faction.Military)
			{
				enemySquad.AddMember(GameManager.Inst.NPCManager.SpawnRandomHumanCharacter("Military", enemySquad, spawnPoints[point].transform.position 
					+ new Vector3(UnityEngine.Random.value * 3, 0, UnityEngine.Random.value * 3)));
			}
			else if(faction == Faction.Mutants)
			{
				string mutantName;
				int goapID;

				if(this.Stage < 7)
				{
					if(UnityEngine.Random.value > 0.6f)
					{
						mutantName = "Mutant1";
						goapID = 3;
					}
					else
					{
						mutantName = "Mutant1";
						goapID = 3;
					}
				}
				else
				{
					if(UnityEngine.Random.value < 0.3f)
					{
						mutantName = "Mutant1";
						goapID = 3;
					}
					else
					{
						if(UnityEngine.Random.value > 0.6f)
						{
							mutantName = "Mutant4";
							goapID = 2;
						}
						else
						{
							mutantName = "Mutant3";
							goapID = 2;
						}
					}
				}

				MutantCharacter mutant = GameManager.Inst.NPCManager.SpawnRandomMutantCharacter(mutantName, enemySquad, spawnPoints[point].transform.position 
					+ new Vector3(UnityEngine.Random.value * 3, 0, UnityEngine.Random.value * 3));



				mutant.MyAI.BlackBoard.PatrolLoc = new Vector3(62.7f, 0, -15);
				mutant.MyAI.BlackBoard.PatrolRange = new Vector3(30, 5, 15);
				mutant.MyAI.BlackBoard.CombatRange = new Vector3(40, 20, 20);
				mutant.MyAI.BlackBoard.HasPatrolInfo = true;
			}

		}



		enemySquad.IssueSquadCommand();
	}
}
