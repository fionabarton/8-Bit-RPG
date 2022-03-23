using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads Scenes, Enemy Tracking, etc.
/// </summary>
public class RPG : MonoBehaviour {
	[Header ("Set in Inspector")]
	public string 				firstScene;

	// BATTLE
	public GameObject 			battleUIGO; // UI
	public GameObject			battleGameObjects; // Player(s), Enemy(s), Scenery etc.

	public bool					canInput;

	// Black Screen
	public SpriteRenderer		blackScreen;

	[Header("Set Dynamically")]
	// Singleton
	private static RPG _S;
	public static RPG S { get { return _S; } set { _S = value; } }

	// DontDestroyOnLoad
	private static bool			exists;

	public bool					paused;

	// Respawn Level
	private Scene 				currentScene;
	public string				currentSceneName;
	public string 				previousSceneName;
	private string				previousPreviousSceneName;

	void Awake() {
		// Singleton
		S = this;

		// DontDestroyOnLoad
		if (!exists) {
			exists = true;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}

		//// Load First Scene
		//SceneManager.LoadScene (firstScene);
	}

	void Start () {
  //      // Change BlackScreen Alpha from 0 to 255
		//// It starts at 0 so it's easier to see things in the editor
  //      Color c = Color.black;
  //      c.a = 1;
  //      blackScreen.color = c;

  //      // Activate Black Screen
  //      blackScreen.enabled = true;

		////LoadSettings ();
		//StartCoroutine("LoadSettingsCo");
	}

	// Load Level
	public void LoadLevel (string levelToLoad){
		previousPreviousSceneName = previousSceneName;

		currentScene = SceneManager.GetActiveScene();
		previousSceneName = currentScene.name;

		// Ensures InteractableCursor is child object of camera, otherwise it can be destroyed on scene change
		InteractableCursor.S.Deactivate();

		canInput = false;

		SceneManager.LoadScene (levelToLoad);
		StartCoroutine ("LoadSettingsCo");
	}
	private IEnumerator LoadSettingsCo (){ // Calls LoadSettings() AFTER scene has changed
		yield return new WaitForSeconds(0.05f);

		// Current Scene
		currentScene = SceneManager.GetActiveScene();
		currentSceneName = currentScene.name;

		// Camera
		CamManager.S.camMode = eCamMode.followAll;
		CamManager.S.ChangeTarget(Player.S.gameObject, false);
		Camera.main.orthographicSize = 5;

		// DISable BATTLE UI & GameObjects
		battleUIGO.SetActive(false);
		battleGameObjects.SetActive(false);

		// Deactivate Dialogue Text Box
		DialogueManager.S.DeactivateTextBox();

		// Deactivate Pause Screen
		ScreenManager.S.UnPause();
        // Deactivate Item Screen
        ScreenManager.S.ItemScreenOff();
		// Deactivate Spells Screen
		ScreenManager.S.SpellsScreenOff();
		// Deactivate Equip Screen
		//ScreenManager.S.EquipScreenOff();
		EquipScreen.S.gameObject.SetActive(false);
		// Deactivate Save Screen
		ScreenManager.S.SaveScreenOff();
		// Deactivate Shop Screen
		ScreenManager.S.ShopScreenOff();

		// Deactivate PlayerButtons
		ScreenManager.S.playerButtonsGO.SetActive(false);

		// Deactivate Sub Menu 
		SubMenu.S.gameObject.SetActive(false);

		// Randomly Spawn Objects
		ObjectPool.S.SpawnObjects(currentSceneName);
		// Open/Close Chests
		ChestManager.S.SetObjects();
		// Unlock/Lock Doors
		DoorManager.S.SetObjects();
		// Deactivate Key Items (Keys, etc.)
		KeyItemManager.S.SetObjects();

		// EnemyManager: populate and clear lists of enemies in currentScene
		if (currentSceneName != "Battle") {
			EnemyManager.S.PopulateEnemyGOList();
		} 
		if (currentSceneName != "Battle" && previousSceneName != "Battle") {
			EnemyManager.S.PopulateEnemyDeadList();
			EnemyManager.S.enemyPositions.Clear();
		}

		////////////// Rain //////////////
		RainSpawner.S.StopCoroutine("FixedUpdateCoroutine");
		switch (currentSceneName){
			case "Overworld_1":
			case "Town_1":
			case "Area_1":
			case "Area_2":
				if (Random.value > 0.5f){
					switch (previousSceneName){
						case "Cave_1":
							RainSpawner.S.isRaining = false;
							RainSpawner.S.darkFilter.SetActive(false);
							break;
						default:
							RainSpawner.S.StartCoroutine("FixedUpdateCoroutine");
							RainSpawner.S.isRaining = true;
							RainSpawner.S.darkFilter.SetActive(true);
							break;
					}
				}else{
					goto default;
				}
				break;
			default:
				RainSpawner.S.isRaining = false;
				RainSpawner.S.darkFilter.SetActive(false);
				break;
		}

		////////////// Music //////////////
		switch (currentSceneName){
			case "Town_1":
				if (!RainSpawner.S.isRaining){
					AudioManager.S.PlaySong(true, 0);
				}else{
					AudioManager.S.PlaySong(true, 1);
				}
				break;
			case "Overworld_1":
			case "Cave_1":
				AudioManager.S.PlaySong(true, 4);
				break;
			case "Battle":
				AudioManager.S.PlaySong(true, 2);
				break;
			case "Shop_1":
			case "Motel_1":
				AudioManager.S.PlaySong(true, 3);
				break;
			default:
				break;
		}

		// Set up Player, Camera, etc. for Scene 
		switch (currentSceneName){
			// Menus
			case "Title_Screen":
			// Battle
			case "Battle":
			// Arcade
			case "PacMan":
			case "SpaceInvaders":
			case "WhackAMole":
			case "CraneMachine":
			case "LuckyHit":
			case "QTE":
			// Platformer
			case "__PreLoadSMB":
			case "_Levels":
			case "_White-Box":
				// Deactivate Player gameObject
				Player.S.gameObject.SetActive(false);

				// Freeze Camera
				CamManager.S.camMode = eCamMode.freezeCam;

				// Reset Cam Position 
				Vector3 tPos = Vector3.zero;
				tPos.z = -10;
				CamManager.S.transform.position = tPos;

				// Handle more specific settings
				switch (currentSceneName){
					case "Battle":
						// Enable BATTLE UI & GameObjects
						battleUIGO.SetActive(true);
						battleGameObjects.SetActive(true);

						// Initialize Battle (reset input bools, set turn initiative, update battle UI Text)
						Battle.S.InitializeBattle();
						break;
					case "PacMan":
						// Camera
						Camera.main.orthographicSize = 7;
						break;
					case "LuckyHit":
						// Camera
						Camera.main.orthographicSize = 10;
						break;
				}
				break;
			default:
				// Set Player Position to Respawn Position
				Player.S.gameObject.transform.position = Player.S.respawnPos;

				Player.S.gameObject.SetActive(true);
				Player.S.canMove = true;

				// Reset EnemyManager if New Scene
				if (currentSceneName != previousPreviousSceneName) {
					EnemyManager.S.enemiesBattled.Clear();
				}
				EnemyManager.S.DetermineWhichEnemiesAreDead();
				EnemyManager.S.DeactivateDeadEnemies();
				EnemyManager.S.SetEnemyPositions();

				if(previousSceneName == "Battle") {
					EnemyManager.S.SetEnemyMovement();
				}
				
				// Freeze Camera
				if (currentSceneName == "Motel_1" || currentSceneName == "Shop_1"){
					// Freeze Camera
					CamManager.S.camMode = eCamMode.freezeCam;
				}
			break;
		}

		// Wait 0.05f seconds
		yield return new WaitForSeconds(0.05f);

		canInput = true;

		// Deactivate Black Screen
		blackScreen.enabled = false;

		// Open Curtains 
		if(currentSceneName == "Battle") {
			BattleCurtain.S.Open();
		}
	}

	public void StartBattle(List<EnemyStats> eStats) {
		Battle.S.ImportEnemyStats(eStats);

		// Set Respawn Position
		Player.S.respawnPos = Player.S.gameObject.transform.position;

		// Freeze Player
		Player.S.canMove = false;
		Player.S.mode = eRPGMode.idle;

		// Freeze all NPC & Enemies
		paused = true;

		// Audio: Start Battle
		AudioManager.S.PlaySong(true, 5);

		// Close Curtains
		BattleCurtain.S.Close();

		// Delay, then Load Scene
		Invoke("LoadBattleScene", 1.25f);
	}

	void LoadBattleScene() {
		LoadLevel ("Battle");

		// Add Update & Fixed Update Delegate
		UpdateManager.updateDelegate += Battle.S.Loop;
		UpdateManager.fixedUpdateDelegate += Battle.S.FixedLoop;
	}

	// ************ Add/Subtract PLAYER HP ************ \\
	public void AddSubtractPlayerHP(int ndx, bool addOrSubtract, int amount){
		if (addOrSubtract) { Stats.S.HP[ndx] += amount; } else { Stats.S.HP[ndx] -= amount; }

		// Prevent going above Max HP & below Min HP
		if (Stats.S.HP[ndx] > Stats.S.maxHP[ndx]) {
			Stats.S.HP[ndx] = Stats.S.maxHP[ndx];
		} else if (Stats.S.HP[ndx] <= 0){
			Stats.S.HP[ndx] = 0;
		}

		// Update Health Bars
		ProgressBars.S.playerHealthBarsCS[ndx].UpdateBar(Stats.S.HP[ndx], Stats.S.maxHP[ndx]);
	}

	public void AddPlayerHP(int ndx, int amount) {
		AddSubtractPlayerHP(ndx, true, amount);
	}
	public void SubtractPlayerHP(int ndx, int amount) {
		AddSubtractPlayerHP(ndx, false, amount);
	}

	// ************ Add/Subtract PLAYER MP ************ \\
	public void AddSubtractPlayerMP(int ndx, bool addOrSubtract, int amount) {
		if (addOrSubtract) { Stats.S.MP[ndx] += amount; } else { Stats.S.MP[ndx] -= amount; }

		// Prevent going above Max MP & below Min MP
		if (Stats.S.MP[ndx] > Stats.S.maxMP[ndx]) {
			Stats.S.MP[ndx] = Stats.S.maxMP[ndx];
		} else if (Stats.S.MP[ndx] <= 0) {
			Stats.S.MP[ndx] = 0;
		}
		
		// Update Magic Bars
		ProgressBars.S.playerMagicBarsCS[ndx].UpdateBar(Stats.S.MP[ndx], Stats.S.maxMP[ndx]);
	}

	public void AddPlayerMP(int ndx, int amount) {
		AddSubtractPlayerMP(ndx, true, amount);
	}
	public void SubtractPlayerMP(int ndx, int amount) {
		AddSubtractPlayerMP(ndx, false, amount);
	}

	// ************ Add/Subtract ENEMY HP ************ \\
	public void AddSubtractEnemyHP(int ndx, bool addOrSubtract, int amount){
		if (addOrSubtract) { Battle.S.enemyStats[ndx].HP += amount; } else { Battle.S.enemyStats[ndx].HP -= amount; }

		// Prevent going above Max HP & below Min HP
		if (Battle.S.enemyStats[ndx].HP > Battle.S.enemyStats[ndx].maxHP) {
			Battle.S.enemyStats[ndx].HP = Battle.S.enemyStats[ndx].maxHP;
		} else if (Battle.S.enemyStats[ndx].HP <= 0){
			Battle.S.enemyStats[ndx].HP = 0;
		}

		// Update Health Bars
		ProgressBars.S.enemyHealthBarsCS[ndx].UpdateBar(Battle.S.enemyStats[ndx].HP, Battle.S.enemyStats[ndx].maxHP);
	}

	public void AddEnemyHP(int ndx, int amount) {
		AddSubtractEnemyHP(ndx, true, amount);
	}
	public void SubtractEnemyHP(int ndx, int amount) {
		AddSubtractEnemyHP(ndx, false, amount);
	}

	// ************ Add/Subtract ENEMY MP ************ \\
	public void AddSubtractEnemyMP(int ndx, bool addOrSubtract, int amount) {
		if (addOrSubtract) { Battle.S.enemyStats[ndx].MP += amount; } else { Battle.S.enemyStats[ndx].MP -= amount; }

		// Prevent going above Max MP & below Min MP
		if (Battle.S.enemyStats[ndx].MP > Battle.S.enemyStats[ndx].maxMP) {
			Battle.S.enemyStats[ndx].MP = Battle.S.enemyStats[ndx].maxMP;
		} else if (Battle.S.enemyStats[ndx].MP <= 0) {
			Battle.S.enemyStats[ndx].MP = 0;
		}
	}

	public void AddEnemyMP(int ndx, int amount) {
		AddSubtractEnemyMP(ndx, true, amount);
	}
	public void SubtractEnemyMP(int ndx, int amount) {
		AddSubtractEnemyMP(ndx, false, amount);
	}

	// ************ Add/Subtract ENEMY MP ************ \\
	public void InstantiateFloatingScore(GameObject gameObject, int amount, Color color) {
		// Get and position Floating Score game object
		GameObject floatingScore = ObjectPool.S.GetPooledObject("FloatingScore");
		ObjectPool.S.PosAndEnableObj(floatingScore, gameObject);

		// Display and color Floating Score text
		if (floatingScore != null) {
			// Get text components (one for colored text in center, four for the black outline)
			Text[] texts = floatingScore.GetComponentsInChildren<Text>();
			for(int i = 0; i < texts.Length; i++) {
				// Display text
				texts[i].text = amount.ToString();
				// Set color of text in center
				if(i == texts.Length - 1) {
					texts[i].color = color;
				}
			}
		}
	}
}
