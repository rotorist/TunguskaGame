using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class GameManager : MonoBehaviour 
{

	#region Singleton
	
	public static GameManager Inst;
	//public static string SaveName;

	#endregion

	#region Public Fields


	public Constants Constants;
	public SoundManager SoundManager;
	public EventManager EventManager;
	public FXManager FXManager;
	public NPCManager NPCManager;
	public DBManager DBManager;
	public UIManager UIManager;
	public CursorManager CursorManager;
	public ItemManager ItemManager;
	public QuestManager QuestManager;
	public WorldManager WorldManager;
	public MaterialManager MaterialManager;
	public SaveGameManager SaveGameManager;

	public CameraController CameraController;
	public CameraShaker CameraShaker;
	public PlayerControl PlayerControl;
	public PlayerProgress PlayerProgress;

	public AIScheduler AIScheduler;

	public string AppDataPath;

	public bool GodMode;

	public float AIUpdateRadius;

	#endregion

	void Start()
	{
		
		UnityEngine.Debug.Log("Game Manager Started");
		AppDataPath = Application.dataPath;
		Initialize();

	}

	void Update()
	{
		if(InputEventHandler.Instance.State != UserInputState.Intro)
		{
			EventManager.ManagerPerFrameUpdate();
			PlayerControl.PerFrameUpdate();
			AIScheduler.UpdatePerFrame();
			CursorManager.PerFrameUpdate();
			SoundManager.PerFrameUpdate();
			NPCManager.PerFrameUpdate();
		}

		UIManager.PerFrameUpdate();
	}

	void LateUpdate()
	{
		
	}

	void FixedUpdate()
	{
		MaterialManager.FixedUpdate();
		EventManager.ManagerFixedUpdate();
	}



	public void LoadGame()
	{
		InputEventHandler.Instance.OnUnloadScene();
		TimerEventHandler.Instance.OnUnloadScene();
		UIEventHandler.Instance.OnUnloadScene();

		SaveNameReference saveNameRef = GameObject.FindObjectOfType<SaveNameReference>();
		saveNameRef.SaveName = "TestSave";
		string levelName = SaveGameManager.LoadLevelName("TestSave");
		SceneManager.LoadScene(levelName);
	}



	#region Private Methods

	private void Initialize()
	{
		

		Inst = this;

		SaveNameReference saveNameRef = GameObject.FindObjectOfType<SaveNameReference>();
		if(saveNameRef != null && saveNameRef.IsGodMode)
		{
			GodMode = true;
		}

		Constants = GetComponent<Constants>();
	

		//Initializing CsDebug
		CsDebug debug = GetComponent<CsDebug>();
		debug.Initialize();

		//Initializing DBManager
		DBManager = new DBManager();
		DBManager.Initialize();

		//Initializing Material Manager
		MaterialManager = new MaterialManager();
		MaterialManager.Initialize();

		//Initializing sound manager
		SoundManager = new SoundManager();
		SoundManager.Initialize();

		//Initializing world manager
		WorldManager = new WorldManager();
		WorldManager.Initialize();

		//Initializing Event Manager
		EventManager = new EventManager();
		EventManager.Initialize();

		ItemManager = new ItemManager();
		ItemManager.Initialize();

		//Initializing NPC Manager
		NPCManager = new NPCManager();
		NPCManager.Initialize();



		PlayerControl = new PlayerControl();
		PlayerControl.Initialize();


		PlayerProgress = new PlayerProgress();


		UIManager = new UIManager();
		UIManager.Initialize();

		QuestManager = new QuestManager();
		QuestManager.Initialize();



		SaveGameManager = new SaveGameManager();

		/*
		MutantCharacter mutant1 = NPCManager.SpawnRandomMutantCharacter("Mutant3", 2, new Vector3(-207.591f, 0, 58.15f));
		mutant1.MyAI.BlackBoard.PatrolLoc = mutant1.transform.position;
		mutant1.MyAI.BlackBoard.PatrolRange = new Vector3(2, 2, 2);
		mutant1.MyAI.BlackBoard.CombatRange = new Vector3(40, 20, 20);
		mutant1.MyAI.BlackBoard.HasPatrolInfo = true;

		MutantCharacter mutant1 = GameObject.Find("MutantCharacter").GetComponent<MutantCharacter>();
		mutant1.Initialize();
		mutant1.MyStatus.MaxHealth = 200;
		mutant1.MyStatus.Health = 200;
		mutant1.MyAI.BlackBoard.PatrolLoc = mutant1.transform.position;
		mutant1.MyAI.BlackBoard.PatrolRange = new Vector3(5, 5, 5);
		mutant1.MyAI.BlackBoard.CombatRange = new Vector3(40, 20, 20);
		mutant1.MyAI.BlackBoard.HasPatrolInfo = true;


		MutantCharacter mutant2 = GameObject.Find("MutantCharacter2").GetComponent<MutantCharacter>();
		mutant2.Initialize();
		mutant2.MyStatus.MaxHealth = 100;
		mutant2.MyStatus.Health = 100;
		mutant2.MyAI.BlackBoard.PatrolLoc = new Vector3(60, 0, -33);
		mutant2.MyAI.BlackBoard.PatrolRange = new Vector3(5, 5, 5);
		mutant2.MyAI.BlackBoard.CombatRange = new Vector3(40, 20, 20);
		mutant2.MyAI.BlackBoard.HasPatrolInfo = true;

		mutant2 = GameObject.Find("MutantCharacter3").GetComponent<MutantCharacter>();
		mutant2.Initialize();
		mutant2.MyStatus.MaxHealth = 100;
		mutant2.MyStatus.Health = 100;
		mutant2.MyAI.BlackBoard.PatrolLoc = new Vector3(60, 0, -33);
		mutant2.MyAI.BlackBoard.PatrolRange = new Vector3(5, 5, 5);
		mutant2.MyAI.BlackBoard.CombatRange = new Vector3(40, 20, 20);
		mutant2.MyAI.BlackBoard.HasPatrolInfo = true;

		mutant2 = GameObject.Find("MutantCharacter4").GetComponent<MutantCharacter>();
		mutant2.Initialize();
		mutant2.MyStatus.MaxHealth = 100;
		mutant2.MyStatus.Health = 100;
		mutant2.MyAI.BlackBoard.PatrolLoc = new Vector3(60, 0, -33);
		mutant2.MyAI.BlackBoard.PatrolRange = new Vector3(5, 5, 5);
		mutant2.MyAI.BlackBoard.CombatRange = new Vector3(40, 20, 20);
		mutant2.MyAI.BlackBoard.HasPatrolInfo = true;

		mutant2 = GameObject.Find("MutantCharacter5").GetComponent<MutantCharacter>();
		mutant2.Initialize();
		mutant2.MyStatus.MaxHealth = 100;
		mutant2.MyStatus.Health = 100;
		mutant2.MyAI.BlackBoard.PatrolLoc = new Vector3(60, 0, -33);
		mutant2.MyAI.BlackBoard.PatrolRange = new Vector3(5, 5, 5);
		mutant2.MyAI.BlackBoard.CombatRange = new Vector3(40, 20, 20);
		mutant2.MyAI.BlackBoard.HasPatrolInfo = true;

		mutant2 = GameObject.Find("MutantCharacter6").GetComponent<MutantCharacter>();
		mutant2.Initialize();
		mutant2.MyStatus.MaxHealth = 100;
		mutant2.MyStatus.Health = 100;
		mutant2.MyAI.BlackBoard.PatrolLoc = new Vector3(60, 0, -33);
		mutant2.MyAI.BlackBoard.PatrolRange = new Vector3(5, 5, 5);
		mutant2.MyAI.BlackBoard.CombatRange = new Vector3(40, 20, 20);
		mutant2.MyAI.BlackBoard.HasPatrolInfo = true;
		*/
		//HumanCharacter enemy1 = GameObject.Find("HumanCharacter2").GetComponent<HumanCharacter>();
		//HumanCharacter enemy2 = GameObject.Find("HumanCharacter4").GetComponent<HumanCharacter>();
		//HumanCharacter enemy3 = GameObject.Find("HumanCharacter5").GetComponent<HumanCharacter>();
		//HumanCharacter enemy4 = GameObject.Find("HumanCharacter6").GetComponent<HumanCharacter>();






		CameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
		CameraController.Initialize();

		CameraShaker = CameraController.GetComponent<CameraShaker>();
		CameraShaker.Initialize();

		FXManager = new FXManager();
		FXManager.Initialize(50);

		AIScheduler = new AIScheduler();
		AIScheduler.Initialize();



		CursorManager = new CursorManager();
		CursorManager.Initialize();

		//if save name is empty then it's a new game
		//if save name is not empty then load save game

		if(saveNameRef == null)
		{
			saveNameRef = (GameObject.Instantiate(Resources.Load("SaveNameReference")) as GameObject).GetComponent<SaveNameReference>();
			//saveNameRef.SaveName = "TestSave";
		}
		if(!string.IsNullOrEmpty(saveNameRef.SaveName))
		{
			Debug.Log("Loading save " + saveNameRef.SaveName);
			SaveGameManager.Load(saveNameRef.SaveName);

		}
		if(saveNameRef.IsNewGame)
		{
			//setup new game
			UIEventHandler.Instance.TriggerStartIntro();

			GameManager.Inst.SaveGameManager.Save("TestSave", "");
			saveNameRef.IsNewGame = false;


		}

		//UIEventHandler.Instance.TriggerStartIntro();

		StartCoroutine(DoPerSecond());
		StartCoroutine(DoPerHalfSecond());

		//serialize test
		//SerializeTest sTest = new SerializeTest();
		//sTest.Test.SetNewAddress("2008 Cedar St");
		//sTest.Test.SetNewName("Helen");
		//sTest.Save();
		//sTest.Load();
	}



	private void PerSecondUpdate()
	{
		if(InputEventHandler.Instance.State != UserInputState.Intro)
		{
			TimerEventHandler.Instance.TriggerOneSecondTimer();
			NPCManager.PerSecondUpdate();
			QuestManager.PerSecondUpdate();
			WorldManager.PerSecondUpdate();
		}
	}

	private void PerHalfSecondUpdate()
	{
		if(InputEventHandler.Instance.State != UserInputState.Intro)
		{
			TimerEventHandler.Instance.TriggerHalfSecondTimer();
		}
	}





	#endregion

	#region Coroutines
	IEnumerator DoPerSecond()
	{
		for(;;)
		{
			PerSecondUpdate();
			yield return new WaitForSeconds(1);
		}

	}

	IEnumerator DoPerHalfSecond()
	{
		for(;;)
		{
			PerHalfSecondUpdate();
			yield return new WaitForSeconds(0.5f);
		}

	}


	#endregion
}

