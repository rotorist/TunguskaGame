using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NPCManager
{
	public GoapGoal DynamicGoalGuard;
	public GoapGoal DynamicGoalFollow;
	public GoapGoal DynamicGoalPatrol;
	public GoapGoal DynamicGoalExplore;
	public GoapGoal DynamicGoalChill;

	public List<NavNode> _allNavNodes;
	public List<NavNode> _navNodeBases;
	public List<NavNode> _navNodeMutants;

	private float _spawnTimer;

	private int _counter;
	private int _randomSquadIndex;

	public List<HumanCharacter> HumansInScene
	{
		get { return _humansInScene; }
	}

	public List<MutantCharacter> MutantsInScene
	{
		get { return _mutantsInScene; }
	}

	public List<Character> AllCharacters
	{
		get { return _allCharacters; }
	}

	public Dictionary<Faction, FactionData> AllFactions
	{
		get { return _allFactions; }
	}

	public Dictionary<string, AISquad> AllSquads
	{
		get { return _allSquads; }
	}

	private List<HumanCharacter> _humansInScene;
	private List<MutantCharacter> _mutantsInScene;
	private List<Character> _allCharacters;
	private Dictionary<string, Household> _allHouseHolds;
	private Dictionary<Faction, FactionData> _allFactions;
	private Dictionary<Character, int> _deadBodies;
	private Dictionary<Character, float> _fadeTimers;
	private Dictionary<string, AISquad> _allSquads;
	private int _characterIndex;
	private int _houseHoldIndex;
	private int _squadIndex;

	public void Initialize()
	{
		_allCharacters = new List<Character>();
		_humansInScene = new List<HumanCharacter>();
		_mutantsInScene = new List<MutantCharacter>();
		_deadBodies = new Dictionary<Character, int>();
		_fadeTimers = new Dictionary<Character, float>();
		_allNavNodes = new List<NavNode>();
		_navNodeBases = new List<NavNode>();
		_navNodeMutants = new List<NavNode>();

		DynamicGoalGuard = GameManager.Inst.DBManager.DBHandlerAI.GetGoalByID(6);
		DynamicGoalGuard.Priority = 5;
		DynamicGoalFollow = GameManager.Inst.DBManager.DBHandlerAI.GetGoalByID(8);
		DynamicGoalFollow.Priority = 5;
		DynamicGoalPatrol = GameManager.Inst.DBManager.DBHandlerAI.GetGoalByID(2);
		DynamicGoalPatrol.Priority = 5;
		DynamicGoalExplore = GameManager.Inst.DBManager.DBHandlerAI.GetGoalByID(12);
		DynamicGoalExplore.Priority = 5;
		DynamicGoalChill = GameManager.Inst.DBManager.DBHandlerAI.GetGoalByID(10);
		DynamicGoalChill.Priority = 5;

		GameObject [] navNodes = GameObject.FindGameObjectsWithTag("NavNode");
		foreach(GameObject o in navNodes)
		{
			NavNode node = o.GetComponent<NavNode>();
			_allNavNodes.Add(node);
			if(node.Type == NavNodeType.Base)
			{
				_navNodeBases.Add(node);
			}
			else if(node.Type == NavNodeType.MutantHunt)
			{
				_navNodeMutants.Add(node);
			}
		}


		if(_allFactions == null)
		{
			_allFactions = GameManager.Inst.DBManager.DBHandlerCharacter.LoadFactionData();
		}
		/*
		FactionData newFaction1 = new FactionData();
		newFaction1.Name = "Player";
		newFaction1.MemberModelIDs = new string[]{"Bandit"};
		newFaction1.FactionID = Faction.Player;

		FactionData newFaction2 = new FactionData();
		newFaction2.Name = "The Legionnaires";
		newFaction2.MemberModelIDs = new string[]{"Legionnaire1", "Legionnaire2", "Legionnaire3"};
		newFaction2.FactionID = Faction.Legionnaires;

		FactionData newFaction3 = new FactionData();
		newFaction3.Name = "Millitary";
		newFaction3.MemberModelIDs = new string[]{"Bandit"};
		newFaction3.FactionID = Faction.Military;

		FactionData newFaction4 = new FactionData();
		newFaction4.Name = "Mutants";
		newFaction4.MemberModelIDs = new string[]{"Mutant3", "Mutant4"};
		newFaction4.FactionID = Faction.Mutants;

		FactionData newFaction5 = new FactionData();
		newFaction5.Name = "Civilian";
		newFaction5.MemberModelIDs = new string[]{"Bandit"};
		newFaction5.FactionID = Faction.Civilian;

		FactionData newFaction6 = new FactionData();
		newFaction6.Name = "Bootleggers";
		newFaction6.MemberModelIDs = new string[]{"Bootlegger1", "Bootlegger2", "Bootlegger3", "Bootlegger4"};
		newFaction6.FactionID = Faction.Bootleggers;

		newFaction2.AddRelationshipEntry(Faction.Player, 0);
		newFaction2.AddRelationshipEntry(Faction.Mutants, 0);

		newFaction3.AddRelationshipEntry(Faction.Player, 0);
		newFaction3.AddRelationshipEntry(Faction.Mutants, 0);

		newFaction4.AddRelationshipEntry(Faction.Player, 0);
		newFaction4.AddRelationshipEntry(Faction.Legionnaires, 0);
		newFaction4.AddRelationshipEntry(Faction.Military, 0);

		newFaction5.AddRelationshipEntry(Faction.Player, 0.5f);

		newFaction6.AddRelationshipEntry(Faction.Player, 0.5f);

		_allFactions.Add(newFaction1.FactionID, newFaction1);
		_allFactions.Add(newFaction2.FactionID, newFaction2);
		_allFactions.Add(newFaction3.FactionID, newFaction3);
		_allFactions.Add(newFaction4.FactionID, newFaction4);
		_allFactions.Add(newFaction5.FactionID, newFaction5);
		_allFactions.Add(newFaction6.FactionID, newFaction6);
		*/


		_allHouseHolds = new Dictionary<string, Household>();
		Household [] households = GameObject.FindObjectsOfType<Household>();
		foreach(Household h in households)
		{
			_allHouseHolds.Add(h.name, h);
		}

		if(_allSquads == null)
		{
			_allSquads = new Dictionary<string, AISquad>();
			LoadAISquads();
		}




		//initialize preset characters
		GameObject [] characters = GameObject.FindGameObjectsWithTag("NPC");
		//Debug.LogError("Number of NPC in scene " + characters.Length + " number of squads " + _allSquads.Count);
		foreach(GameObject o in characters)
		{
			Character c = o.GetComponent<Character>();
			if(c != null)
			{
				c.Initialize();
				if(!string.IsNullOrEmpty(c.SquadID))
				{
					
					_allSquads[c.SquadID].AddMember(c);
					//Debug.Log(c.SquadID + " now has members " + _allSquads[c.SquadID].Members.Count);
				}
			}

			//load preset inventory
			if(c.PresetInventory != null && c.PresetInventory.IsEnabled)
			{
				GameManager.Inst.ItemManager.LoadNPCInventory(c.Inventory, c.PresetInventory);

			}

			c.MyAI.WeaponSystem.LoadWeaponsFromInventory(false);

		}

		foreach(Household h in _allHouseHolds.Values)
		{
			//Debug.LogError("initializing household " + h.name);
			h.Initialize();
		}

		//Debug.LogError("Number of NPC in NPC Manager " + _allCharacters.Count);
	}

	public void PerSecondUpdate()
	{
		//build a dead body list
		//remove dead bodies that have been dead for some time
		/*
		List<Character> allCharactersCopy = new List<Character>(_allCharacters);
		foreach(Character c in allCharactersCopy)
		{
			if(c.MyStatus.Health <= 0)
			{
				if(_deadBodies.ContainsKey(c))
				{
					_deadBodies[c] ++;
					if(_deadBodies[c] > 45)
					{
						if(c.IsHuman)
						{
							_humansInScene.Remove((HumanCharacter)c);
						}
						else
						{
							_mutantsInScene.Remove((MutantCharacter)c);
						}
						_allCharacters.Remove(c);
						_deadBodies.Remove(c);
						GameObject.Destroy(c.gameObject);
					}
				}
				else
				{
					_deadBodies.Add(c, 0);
				}

			}
		}
		*/
		//update one household at a time
		if(_houseHoldIndex < _allHouseHolds.Count)
		{
			string key = _allHouseHolds.Keys.ElementAt(_houseHoldIndex);
			_allHouseHolds[key].UpdateHouseHoldPerSecond();
			_houseHoldIndex ++;
		}
		else
		{
			_houseHoldIndex = 0;
		}



		string keySquad = _allSquads.Keys.ElementAt(_squadIndex);
		if(_allSquads.ContainsKey(keySquad))
		{
			_allSquads[keySquad].UpdateSquadPerSecond();
		}
		_squadIndex ++;
		if(_squadIndex >= _allSquads.Count)
		{
			_squadIndex = 0;
		}

	}

	public void PerFrameUpdate()
	{
		HandleCharacterHiding();
	}

	public void AddHumanCharacter(HumanCharacter character)
	{
		_humansInScene.Add(character);
		_allCharacters.Add(character);
	}

	public void AddMutantCharacter(MutantCharacter character)
	{
		_mutantsInScene.Add(character);
		_allCharacters.Add(character);
	}

	public void RemoveHumanCharacter(HumanCharacter character)
	{
		if(_humansInScene.Contains(character))
		{
			_humansInScene.Remove(character);

		}

		if(_deadBodies.ContainsKey((Character)character))
		{
			_deadBodies.Remove((Character)character);
		}
		_allCharacters.Remove((Character)character);
		if(_fadeTimers.ContainsKey((Character)character))
		{
			_fadeTimers.Remove((Character)character);
		}
		GameObject.Destroy(character.gameObject);
	}

	public void RemoveMutantCharacter(MutantCharacter character)
	{
		if(_mutantsInScene.Contains(character))
		{
			_mutantsInScene.Remove(character);

		}

		if(_deadBodies.ContainsKey((Character)character))
		{
			_deadBodies.Remove((Character)character);
		}
		_allCharacters.Remove((Character)character);
		if(_fadeTimers.ContainsKey((Character)character))
		{
			_fadeTimers.Remove((Character)character);
		}
		GameObject.Destroy(character.gameObject);
	}

	public int GetLivingHumansCount()
	{
		int i = 0;
		foreach(HumanCharacter h in _humansInScene)
		{
			if(h.MyStatus.Health > 0)
			{
				i++;
			}
		}

		return i;
	}

	public int GetLivingMutantsCount()
	{
		int i = 0;
		foreach(MutantCharacter m in _mutantsInScene)
		{
			if(m.MyStatus.Health > 0)
			{
				i++;
			}
		}

		return i;
	}

	public int GetNearbyEnemyCount(float distance)
	{
		int enemies = 0;
		Vector3 playerPos = GameManager.Inst.PlayerControl.SelectedPC.transform.position;
		foreach(MutantCharacter m in _mutantsInScene)
		{
			if(m.MyStatus.Health > 0 && Vector3.Distance(playerPos, m.transform.position) < distance)
			{
				enemies ++;
			}
		}

		foreach(HumanCharacter h in _humansInScene)
		{
			if(h.MyStatus.Health > 0 && h.MyAI.IsCharacterEnemy(GameManager.Inst.PlayerControl.SelectedPC) < 1 && Vector3.Distance(playerPos, h.transform.position) < distance)
			{
				enemies ++;
			}
		}

		return enemies;
	}



	public NavNode GetNavNodeByName(string name)
	{
		foreach(NavNode node in _allNavNodes)
		{	
			if(name == node.name)
			{
				return node;
			}
		}
		Debug.Log("Found no nav node for " + name);
		return null;
	}


	public NavNode GetRandomBaseNavNode(Faction myFaction)
	{
		List<NavNode> candidates = new List<NavNode>();
		foreach(NavNode node in _navNodeBases)
		{
			if(node.IsOpenToExpedition)
			{
				if(node.Household.CurrentSquad == null || node.Household.CurrentSquad.Faction != myFaction)
				candidates.Add(node);
			}
		}

		if(candidates.Count > 0)
		{
			int rand = UnityEngine.Random.Range(0, candidates.Count);
			return candidates[rand];
		}
		else
		{
			return null;
		}
	}

	public NavNode GetRandomHuntNavNode()
	{
		List<NavNode> candidates = new List<NavNode>();
		foreach(NavNode node in _navNodeMutants)
		{
			if(node.IsOpenToExpedition)
			{
				candidates.Add(node);
			}
		}

		if(candidates.Count > 0)
		{
			int rand = UnityEngine.Random.Range(0, candidates.Count);
			return candidates[rand];
		}
		else
		{
			return null;
		}
	}


	public HumanCharacter SpawnRandomHumanCharacter(string name, AISquad squad, Vector3 loc)
	{
		HumanCharacter character = GameObject.Instantiate(Resources.Load("HumanCharacter") as GameObject).GetComponent<HumanCharacter>();



		character.CharacterID = name;

		character.Initialize();
		character.MyNavAgent.enabled = false;
		character.CharacterType = CharacterType.Human;

		character.SquadID = squad.ID;
		character.Faction = squad.Faction;
		GameManager.Inst.ItemManager.LoadNPCInventory(character.Inventory, squad.Faction, squad.Tier);
		character.MyAI.WeaponSystem.LoadWeaponsFromInventory(false);

		character.MyStatus.MaxHealth = 100;
		character.MyStatus.Health = 100;

		character.transform.position = loc;
		//character.transform.position = new Vector3(70.76f, 3.296f, -23.003f);

		character.MyNavAgent.enabled = true;

		character.Name = GameManager.Inst.DBManager.DBHandlerDialogue.GetRandomName();

		character.gameObject.name = character.gameObject.name + _counter.ToString();
		_counter ++;



		return character;
	}

	public MutantCharacter SpawnRandomMutantCharacter(string name, AISquad squad, Vector3 loc)
	{
		MutantCharacter character = GameObject.Instantiate(Resources.Load("MutantCharacter") as GameObject).GetComponent<MutantCharacter>();

		character.CharacterID = name;

		character.Initialize();
		character.CharacterType = CharacterType.Mutant;
		character.MyNavAgent.enabled = false;
		character.Faction = squad.Faction;
		character.SquadID = squad.ID;

		GameManager.Inst.ItemManager.LoadNPCInventory(character.Inventory, squad.Faction, 0);
		character.MyAI.WeaponSystem.LoadWeaponsFromInventory(false);

		character.MyStatus = GetMutantStatus(character);

		character.transform.position = loc;

		character.MyNavAgent.enabled = true;

		character.gameObject.name = character.gameObject.name + _counter.ToString();
		_counter ++;



		return character;
	}

	public MutantCharacter SpawnRandomAnimalCharacter(string name, AISquad squad, Vector3 loc)
	{
		MutantCharacter character = GameObject.Instantiate(Resources.Load("MutantAnimal") as GameObject).GetComponent<MutantCharacter>();

		character.CharacterID = name;

		character.Initialize();
		character.CharacterType = CharacterType.Animal;
		character.MyNavAgent.enabled = false;
		character.Faction = squad.Faction;
		character.SquadID = squad.ID;

		GameManager.Inst.ItemManager.LoadNPCInventory(character.Inventory, squad.Faction, 0);
		character.MyAI.WeaponSystem.LoadWeaponsFromInventory(false);

		character.transform.position = loc;

		character.MyNavAgent.enabled = true;

		character.gameObject.name = character.gameObject.name + _counter.ToString();
		_counter ++;



		return character;
	}

	public AISquad SpawnHumanExplorerSquad(Faction faction, Household household)
	{
		AISquad squad = new AISquad();
		squad.ID = faction.ToString() + _randomSquadIndex.ToString();
		if(household != null && household.CurrentSquad != null)
		{
			squad.Tier = household.CurrentSquad.Tier;
		}
		else
		{
			squad.Tier = 1;
		}
		_randomSquadIndex ++;
		squad.Faction = faction;
		squad.Household = household;
		_allSquads.Add(squad.ID, squad);

		return squad;
	}

	public void DeleteSquad(string squadID)
	{
		if(_allSquads.ContainsKey(squadID))
		{
			_allSquads.Remove(squadID);
		}
	}

	public List<NavNode> GetAllNavNodes()
	{
		return _allNavNodes;
	}

	public NavNode GetNavNodeByHousehold(Household h)
	{
		foreach(NavNode node in _allNavNodes)
		{
			if(node.Type == NavNodeType.Base && node.Household == h)
			{
				return node;
			}
		}

		return null;
	}

	public FactionData GetFactionData(Faction id)
	{
		if(_allFactions.ContainsKey(id))
		{
			return _allFactions[id];
		}

		return null;
	}

	public void HideCharacterPublic(Character character)
	{
		HideCharacter(character);
	}

	public void OnHumanDeath(Character killer, Character victim)
	{
		if(killer == null || victim == null || killer.Faction == victim.Faction)
		{
			return;
		}

		FactionData killerFaction = GetFactionData(killer.Faction);
		FactionData victimFaction = GetFactionData(victim.Faction);

		victimFaction.ReduceRelationshipByID(killer.Faction, 0.25f);
		killerFaction.ReduceRelationshipByID(victim.Faction, 0.25f);
	}


	private CharacterStatus GetMutantStatus(MutantCharacter character)
	{
		CharacterStatus status = character.MyStatus;

		if(character.CharacterID == "Mutant1")
		{
			status.MaxHealth = 200;
			status.Health = 200;
			status.WalkSpeed = 1;
			status.RunSpeed = 4.0f;
			status.SprintSpeed = 5.0f;
			status.WalkSpeedModifier = 0.9f;
			status.RunSpeedModifier = 1.1f;
			status.SprintSpeedModifier = 1.1f;
			status.EyeSight = 1.5f;
			status.Intelligence = 2;
			status.MutantMovementBlend = 0;
		}
		else
		{
			status.MaxHealth = UnityEngine.Random.Range(80f, 150f);
			status.Health = status.MaxHealth;

			status.MutantMovementBlend = UnityEngine.Random.Range(-1, 3);
			if(status.MutantMovementBlend == 0)
			{
				status.WalkSpeed = 0.8f;
				status.RunSpeed = UnityEngine.Random.Range(0.9f, 1.5f);
				status.SprintSpeed = 5.2f;
				status.WalkSpeedModifier = 1;
				status.RunSpeedModifier = 0.8f;
				status.SprintSpeedModifier = 1.1f;
							
			}
			else if(status.MutantMovementBlend == 2)
			{
				status.WalkSpeed = 1f;
				status.RunSpeed = 2f;
				status.SprintSpeed = 5.2f;
				status.WalkSpeedModifier = 1;
				status.RunSpeedModifier = 1f;
				status.SprintSpeedModifier = 1.1f;
			}
			else if(status.MutantMovementBlend == 1)
			{
				status.WalkSpeed = 0.8f;
				status.RunSpeed = 0.9f;
				status.SprintSpeed = 5.2f;
				status.WalkSpeedModifier = 1;
				status.RunSpeedModifier = 0.8f;
				status.SprintSpeedModifier = 1.1f;
			}
			else if(status.MutantMovementBlend == -1)
			{
				status.WalkSpeed = 1f;
				status.RunSpeed = 0.8f;
				status.SprintSpeed = 5.2f;
				status.WalkSpeedModifier = 1;
				status.RunSpeedModifier = 1f;
				status.SprintSpeedModifier = 1.1f;
			}

			status.EyeSight = 1.5f;
			status.Intelligence = 2;

		}


		return status;
	}

	private void RandomizeMutantDamage(Character mutant)
	{

	}
		

	private void HandleCharacterHiding()
	{
		if(_allCharacters.Count <= 1)
		{
			return;
		}

		if(_characterIndex < _allCharacters.Count && _allCharacters[_characterIndex] != null && _allCharacters[_characterIndex].MyAI.ControlType != AIControlType.Player)
		{
			RaycastHit buildingHit;
			bool isVisible = true;




			if(Physics.Raycast(_allCharacters[_characterIndex].transform.position, Vector3.down, out buildingHit, 200, (1 << 9 | 1 << 8 | 1 << 10)))
			{
				BuildingComponent component = buildingHit.collider.GetComponent<BuildingComponent>();
				if(component != null && component.Building.IsActive)
				{
					//means player is also in this building
					if(component.IsHidden)
					{
						//hide NPC as well
						isVisible = false;
						_allCharacters[_characterIndex].IsInHiddenBuilding = true;

					}
					else
					{
						_allCharacters[_characterIndex].IsInHiddenBuilding = false;

					}

				}

			}




			/*
			Vector3 playerEye = GameManager.Inst.PlayerControl.SelectedPC.MyReference.Eyes.transform.position;
			//skip this character if too far away or is dead
			int iterations = 0;
			while((_allCharacters[_characterIndex].MyStatus.Health <= 0 || Vector3.Distance(playerEye, _allCharacters[_characterIndex].transform.position) > 50
				|| _allCharacters[_characterIndex].MyAI.ControlType == AIControlType.Player) && iterations <= _allCharacters.Count)
			{
				iterations ++;
				_characterIndex ++;
				if(_characterIndex >= _allCharacters.Count)
				{
					_characterIndex = 0;
				}

				if(iterations > _allCharacters.Count)
				{
					return;
				}
			}



			float colliderHeight = _allCharacters[_characterIndex].GetComponent<CapsuleCollider>().height;

			Vector3 rayTarget = _allCharacters[_characterIndex].transform.position + Vector3.up * colliderHeight * 0.7f;
			Vector3 raycastDir = rayTarget - playerEye;
			//do a raycast to see if player can see it
			RaycastHit hit;
			bool isVisible = false;
			if(Physics.Raycast(playerEye, raycastDir, out hit, 50, ~(1 << 12)))
			{
				if(hit.collider.gameObject == _allCharacters[_characterIndex].gameObject)
				{
					isVisible = true;
				}
				else
				{
					isVisible = false;
				}


			}
			else
			{
				isVisible = false;
			}
			*/

			/*
			if(isVisible && _allCharacters[_characterIndex].IsHidden)
			{
				//unfade character
				ShowCharacter(_allCharacters[_characterIndex]);
			}
			else if(!isVisible && !_allCharacters[_characterIndex].IsHidden)
			{
				//fade character
				Debug.Log("Hiding character " + _allCharacters[_characterIndex].name);
				HideCharacter(_allCharacters[_characterIndex]);
			}
			*/

		}

		_characterIndex ++;
		if(_characterIndex >= _allCharacters.Count)
		{
			_characterIndex = 0;
		}


	}

	private void HideCharacter(Character character)
	{
		/*
		if(!_fadeTimers.ContainsKey(character))
		{
			_fadeTimers.Add(character, 0);
		}

		if(_fadeTimers[character] < 3)
		{
			_fadeTimers[character] += Time.deltaTime;
			return;
		}
		*/

		//hide weapons
		Renderer [] childRenderers = character.GetComponentsInChildren<Renderer>();
		foreach(Renderer r in childRenderers)
		{
			r.enabled = false;
		}

		character.IsHidden = true;

		if(_fadeTimers.ContainsKey(character))
		{
			_fadeTimers[character] = 0;
		}
	}

	private void ShowCharacter(Character character)
	{
		Renderer [] childRenderers = character.GetComponentsInChildren<Renderer>();
		foreach(Renderer r in childRenderers)
		{
			r.enabled = true;
		}

		character.IsHidden = false;

		if(_fadeTimers.ContainsKey(character))
		{
			_fadeTimers[character] = 0;
		}
	}

	private void LoadAISquads()
	{
		AISquad squad1 = new AISquad();
		squad1.ID = "zsk_sidorovich";
		squad1.Tier = 1;
		squad1.Faction = Faction.Bootleggers;
		squad1.Household = _allHouseHolds["HouseHoldSidorovich"];
		squad1.Household.CurrentSquad = squad1;
		if(!_allSquads.ContainsKey(squad1.ID))
		{
			_allSquads.Add(squad1.ID, squad1);
		}

		AISquad squad2 = new AISquad();
		squad2.ID = "zsk_hans";
		squad2.Tier = 1;
		squad2.Faction = Faction.Bootleggers;
		squad2.Household = _allHouseHolds["HouseHoldHans"];
		squad2.Household.CurrentSquad = squad2;
		if(!_allSquads.ContainsKey(squad2.ID))
		{
			_allSquads.Add(squad2.ID, squad2);
		}

		AISquad squad3 = new AISquad();
		squad3.ID = "zsk_village_bootleggers";
		squad3.Tier = 1;
		squad3.Faction = Faction.Bootleggers;
		squad3.Household = _allHouseHolds["HouseHoldVillageBootleggers"];
		squad3.Household.CurrentSquad = squad3;
		if(!_allSquads.ContainsKey(squad3.ID))
		{
			_allSquads.Add(squad3.ID, squad3);
		}

		AISquad squad4 = new AISquad();
		squad4.ID = "zsk_barn_legionnaires";
		squad4.Tier = 1;
		squad4.Faction = Faction.Legionnaires;
		squad4.Household = _allHouseHolds["HouseHoldBarn"];
		squad4.Household.CurrentSquad = squad4;
		if(!_allSquads.ContainsKey(squad4.ID))
		{
			_allSquads.Add(squad4.ID, squad4);
		}

		AISquad squad5 = new AISquad();
		squad5.ID = "zsk_church_ghouls";
		squad5.Tier = 1;
		squad5.Faction = Faction.Mutants;
		squad5.Household = _allHouseHolds["HouseHoldChurch"];
		squad5.Household.CurrentSquad = squad5;
		if(!_allSquads.ContainsKey(squad5.ID))
		{
			_allSquads.Add(squad5.ID, squad5);
		}

		AISquad squad6 = new AISquad();
		squad6.ID = "zsk_sewer_ghouls";
		squad6.Tier = 1;
		squad6.Faction = Faction.Scythes;
		squad6.Household = _allHouseHolds["HouseHoldSewer"];
		squad6.Household.CurrentSquad = squad6;
		if(!_allSquads.ContainsKey(squad6.ID))
		{
			_allSquads.Add(squad6.ID, squad6);
		}

		AISquad squad7 = new AISquad();
		squad7.ID = "zsk_priest";
		squad7.Tier = 1;
		squad7.Faction = Faction.Civilian;
		squad7.Household = _allHouseHolds["HouseHoldPriest"];
		squad7.Household.CurrentSquad = squad7;
		if(!_allSquads.ContainsKey(squad7.ID))
		{
			_allSquads.Add(squad7.ID, squad7);
		}

		AISquad squad8 = new AISquad();
		squad8.ID = "zsk_artyom";
		squad8.Tier = 1;
		squad8.Faction = Faction.Civilian;
		squad8.Household = _allHouseHolds["HouseHoldArtyom"];
		squad8.Household.CurrentSquad = squad8;
		if(!_allSquads.ContainsKey(squad8.ID))
		{
			_allSquads.Add(squad8.ID, squad8);
		}

		AISquad squad9 = new AISquad();
		squad9.ID = "zsk_wolves";
		squad9.Tier = 1;
		squad9.Faction = Faction.Animals;
		squad9.Household = _allHouseHolds["HouseHoldWolf"];
		squad9.Household.CurrentSquad = squad9;
		if(!_allSquads.ContainsKey(squad9.ID))
		{
			_allSquads.Add(squad9.ID, squad9);
		}

		AISquad squad10 = new AISquad();
		squad10.ID = "zsk_army";
		squad10.Tier = 2;
		squad10.Faction = Faction.Military;
		squad10.Household = _allHouseHolds["HouseHoldRoadBlock"];
		squad10.Household.CurrentSquad = squad10;
		if(!_allSquads.ContainsKey(squad10.ID))
		{
			_allSquads.Add(squad10.ID, squad10);
		}

		AISquad squad11 = new AISquad();
		squad11.ID = "zsk_train_legionnaires";
		squad11.Tier = 2;
		squad11.Faction = Faction.Legionnaires;
		squad11.Household = _allHouseHolds["HouseHoldTrainStation"];
		squad11.Household.CurrentSquad = squad11;
		if(!_allSquads.ContainsKey(squad11.ID))
		{
			_allSquads.Add(squad11.ID, squad11);
		}

		AISquad squad12 = new AISquad();
		squad12.ID = "zsk_cheslav";
		squad12.Tier = 1;
		squad12.Faction = Faction.Civilian;
		squad12.Household = _allHouseHolds["HouseHoldCheslav"];
		squad12.Household.CurrentSquad = squad12;
		if(!_allSquads.ContainsKey(squad12.ID))
		{
			_allSquads.Add(squad12.ID, squad12);
		}

		AISquad squad13 = new AISquad();
		squad13.ID = "zsk_woods_ghouls";
		squad13.Tier = 1;
		squad13.Faction = Faction.Mutants;
		squad13.Household = _allHouseHolds["HouseHoldWoodsMutant"];
		squad13.Household.CurrentSquad = squad13;
		if(!_allSquads.ContainsKey(squad13.ID))
		{
			_allSquads.Add(squad13.ID, squad13);
		}

		AISquad squad14 = new AISquad();
		squad14.ID = "zsk_wolves2";
		squad14.Tier = 1;
		squad14.Faction = Faction.Animals;
		squad14.Household = _allHouseHolds["HouseHoldWolf2"];
		squad14.Household.CurrentSquad = squad14;
		if(!_allSquads.ContainsKey(squad14.ID))
		{
			_allSquads.Add(squad14.ID, squad14);
		}

		AISquad squad15 = new AISquad();
		squad15.ID = "zsk_wolves2";
		squad15.Tier = 1;
		squad15.Faction = Faction.Animals;
		squad15.Household = _allHouseHolds["HouseHoldWolf3"];
		squad15.Household.CurrentSquad = squad15;
		if(!_allSquads.ContainsKey(squad15.ID))
		{
			_allSquads.Add(squad15.ID, squad15);
		}
	}
}
