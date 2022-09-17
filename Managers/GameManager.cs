using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads Scenes, Enemy Tracking, etc.
/// </summary>
public class GameManager : MonoBehaviour {
	[Header("Set in Inspector")]
	public string firstScene;

	public bool canInput;

	public SubMenu gameSubMenu;
	public SubMenu pauseSubMenu;

	[Header("Set Dynamically")]
	public bool paused;

	// Respawn Level
	public string currentScene;
	public string previousScene;
	private string previousPreviousScene;

	private static GameManager _S;
	public static GameManager S { get { return _S; } set { _S = value; } }

	// DontDestroyOnLoad
	private static bool exists;

	void Awake() {
		S = this;

		// DontDestroyOnLoad
		if (!exists) {
			exists = true;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	void Start() {
		// Load First Scene
		LoadLevel(firstScene);

        // Add Loop() to UpdateManager
        UpdateManager.updateDelegate += Loop;
	}

	public void Loop() {
        if (Input.GetKeyDown(KeyCode.Y)) {
            if (!SaveMenu.S.gameObject.activeInHierarchy) {
				SaveMenu.S.Activate();
            } else {
				SaveMenu.S.Deactivate();
            }
        }

		// Pause Screen input
		if (!Items.S.menu.gameObject.activeInHierarchy &&
            !Spells.S.menu.gameObject.activeInHierarchy &&
            !EquipMenu.S.gameObject.activeInHierarchy &&
            !ShopMenu.S.gameObject.activeInHierarchy &&
            !OptionsMenu.S.gameObject.activeInHierarchy &&
			!SaveMenu.S.gameObject.activeInHierarchy) {

            if (!Player.S.isBattling) {
                if (currentScene != "Title_Screen") {
                    if (!PauseMenu.S.gameObject.activeInHierarchy) {
                        if (Input.GetButtonDown("Pause")) {
                            PauseMenu.S.Pause();

                            //AddSubtractPlayerHP(0, false, 12);
                            //AddSubtractPlayerHP(1, false, 19);
                            //AddSubtractPlayerHP(2, false, 20);

                            //AddSubtractPlayerMP(0, false, 2);
                            //AddSubtractPlayerMP(1, false, 2);
                            //AddSubtractPlayerMP(2, false, 2);

                            //PauseMenu.S.UpdateGUI();
                        }
                    } else {
                        if (Input.GetButtonDown("Pause") || Input.GetButtonDown("SNES Y Button")) {
                            PauseMenu.S.UnPause(true);
                        }
                    }
                }
            }
        }
    }

	// Load Level
	public void LoadLevel(string levelToLoad) {
		previousPreviousScene = previousScene;
		previousScene = SceneManager.GetActiveScene().name;

		// Ensures InteractableCursor is child object of camera, otherwise it can be destroyed on scene change
		InteractableCursor.S.Deactivate();

		canInput = false;

		// Disable player collider
		Player.S.coll.enabled = false;

		SceneManager.LoadScene(levelToLoad);
		StartCoroutine("LoadSettingsCo");
	}
	private IEnumerator LoadSettingsCo() { // Calls LoadSettings() AFTER scene has changed
		yield return new WaitForSeconds(0.05f);

		// Current Scene
		currentScene = SceneManager.GetActiveScene().name;

		// Record that the player has visited this location
		WarpManager.S.HasVisited(currentScene);

		// Camera
		CamManager.S.camMode = eCamMode.followAll;
		CamManager.S.ChangeTarget(Player.S.gameObject, false);
		Camera.main.orthographicSize = 6;

		// Dectivate battle UI and gameobjects
		Battle.S.UI.battleMenu.SetActive(false);
		Battle.S.UI.battleGameObjects.SetActive(false);

		// Deactivate Dialogue Text Box
		DialogueManager.S.DeactivateTextBox();

		// Deactivate screens
		PauseMenu.S.UnPause();
		Items.S.menu.Deactivate();
		Spells.S.menu.Deactivate();
		EquipMenu.S.Deactivate();
        OptionsMenu.S.Deactivate();
        SaveMenu.S.Deactivate();
        ShopMenu.S.Deactivate();
        TitleMenu.S.Deactivate();

        // Deactivate Sub Menus
        gameSubMenu.gameObject.SetActive(false);
		pauseSubMenu.gameObject.SetActive(false);

		// Randomly Spawn Objects
		ObjectPool.S.SpawnObjects(currentScene);
		// Open/Close Chests
		ChestManager.S.SetObjects();
		// Unlock/Lock Doors
		DoorManager.S.SetObjects();
		// Deactivate Key Items (Keys, etc.)
		KeyItemManager.S.SetObjects();

		// Get all enemy gameObjects and swap layer sprite renderers
		if (currentScene != "Battle") {
			// If poisoned, activate overworld poisoned icons
			StatusEffects.S.SetOverworldPoisonIcons();
		}

		////////////// Music //////////////
		switch (currentScene) {
			case "Title_Screen":
				AudioManager.S.PlaySong(eSongName.soap);
				break;
			case "Playground":
				AudioManager.S.PlaySong(eSongName.nineteenForty);
				break;
			case "New Scene":
				AudioManager.S.PlaySong(eSongName.things);
				break;
			case "Battle":
				AudioManager.S.PlaySong(eSongName.ninja);
				break;
			default:
				break;
		}

		// Set up Player, Camera, etc. for Scene 
		switch (currentScene) {
			case "Title_Screen":
				TitleMenu.S.Activate();
				break;
			default:
				// Set Player Position to Respawn Position
				Player.S.gameObject.transform.position = Player.S.respawnPos;

				Player.S.canMove = true;

				// Freeze Camera
				if (currentScene == "Motel_1" || currentScene == "Shop_1") {
					// Freeze Camera
					CamManager.S.camMode = eCamMode.freezeCam;

				} else {
					// Set camera to Player position
					Camera.main.transform.position = Player.S.gameObject.transform.position;
				}
				break;
		}

		// Wait 0.05f seconds
		yield return new WaitForSeconds(0.05f);

		canInput = true;

		// Enable player collider
		Player.S.coll.enabled = true;

		// Deactivate Black Screen
		ColorScreen.S.anim.Play("Clear Screen", 0, 0);

		// Open Curtains 
		if (!Curtain.S.isOpen) {
			Curtain.S.Open();
		}
	}

	// ************ Add/Subtract PLAYER HP ************ \\
	public void AddSubtractPlayerHP(int ndx, bool addOrSubtract, int amount, bool playerFlickerClip = false) {
		if (addOrSubtract) { Party.S.stats[ndx].HP += amount; } else { Party.S.stats[ndx].HP -= amount; }

		// Prevent going above Max HP & below Min HP
		if (Party.S.stats[ndx].HP > Party.S.stats[ndx].maxHP) {
			Party.S.stats[ndx].HP = Party.S.stats[ndx].maxHP;
		} else if (Party.S.stats[ndx].HP <= 0) {
			Party.S.stats[ndx].HP = 0;
		}

		// Update party stats and stat frame animations
		Battle.S.UI.UpdatePartyStats(ndx, playerFlickerClip);

		// Update Health Bars
		//ProgressBars.S.playerHealthBarsCS[ndx].UpdateBar(Party.S.stats[ndx].HP, Party.S.stats[ndx].maxHP);
	}

	public void AddPlayerHP(int ndx, int amount, bool playerFlickerClip = false) {
		AddSubtractPlayerHP(ndx, true, amount, playerFlickerClip);
	}
	public void SubtractPlayerHP(int ndx, int amount, bool playerFlickerClip = false) {
		AddSubtractPlayerHP(ndx, false, amount, playerFlickerClip);
	}

	// ************ Add/Subtract PLAYER MP ************ \\
	public void AddSubtractPlayerMP(int ndx, bool addOrSubtract, int amount, bool playerFlickerClip = false) {
		if (addOrSubtract) { Party.S.stats[ndx].MP += amount; } else { Party.S.stats[ndx].MP -= amount; }

		// Prevent going above Max MP & below Min MP
		if (Party.S.stats[ndx].MP > Party.S.stats[ndx].maxMP) {
			Party.S.stats[ndx].MP = Party.S.stats[ndx].maxMP;
		} else if (Party.S.stats[ndx].MP <= 0) {
			Party.S.stats[ndx].MP = 0;
		}

		// Update party stats and stat frame animations
		Battle.S.UI.UpdatePartyStats(ndx, playerFlickerClip);

		// Update Magic Bars
		//ProgressBars.S.playerMagicBarsCS[ndx].UpdateBar(Party.S.stats[ndx].MP, Party.S.stats[ndx].maxMP, false);
	}

	public void AddPlayerMP(int ndx, int amount) {
		AddSubtractPlayerMP(ndx, true, amount);
	}
	public void SubtractPlayerMP(int ndx, int amount) {
		AddSubtractPlayerMP(ndx, false, amount);
	}

	// ************ Add/Subtract ENEMY HP ************ \\
	public void AddSubtractEnemyHP(int ndx, bool addOrSubtract, int amount) {
		if (addOrSubtract) { Battle.S.enemyStats[ndx].HP += amount; } else { Battle.S.enemyStats[ndx].HP -= amount; }

		// Prevent going above Max HP & below Min HP
		if (Battle.S.enemyStats[ndx].HP > Battle.S.enemyStats[ndx].maxHP) {
			Battle.S.enemyStats[ndx].HP = Battle.S.enemyStats[ndx].maxHP;
		} else if (Battle.S.enemyStats[ndx].HP <= 0) {
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

	// ************************ \\
	public void InstantiateFloatingScore(GameObject gameObject, string message, Color color) {
		// Get and position Floating Score game object
		GameObject floatingScore = ObjectPool.S.GetPooledObject("FloatingScore");
		ObjectPool.S.PosAndEnableObj(floatingScore, gameObject);

		// Display and color Floating Score text
		if (floatingScore != null) {
			// Get text components (one for colored text in center, four for the black outline)
			Text[] texts = floatingScore.GetComponentsInChildren<Text>();
			for (int i = 0; i < texts.Length; i++) {
				// Display text
				texts[i].text = message;
				// Set color of text in center
				if (i == texts.Length - 1) {
					if(message != "0") {
						texts[i].color = color;
					} else {
						texts[i].color = Color.white;
					}
				}
			}
		}
	}
}