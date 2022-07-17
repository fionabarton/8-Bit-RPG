using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PauseMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	// Stats
	public List<Text> playerNameText;
	public List<Text> statsNumText;
	public Text fileStatsNumText;
	public List<GameObject> playerGO;

	// Items, Equip, Spells, Save Buttons
	public List<GameObject> buttonGO; // 0: Items, 1: Equip, 2: Spells, 3: Options, 4: Save
	public List<Button> buttonCS; // 0: Items, 1: Equip, 2: Spells, 3: Options, 4: Save

	// Player buttons
	public List<Button> playerNameButtons;
	public List<Animator> playerAnims;

	[Header("Set Dynamically")]
	// Stats
	public int seconds;
	public int minutes;

	// Resets timer
	float timeDone;

	// Account for time that PauseScreen is not active
	float timeWhenEnabled;
	float timeWhenDisabled;

	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

	// Ensures audio is only played once when button is selected
	public GameObject previousSelectedGameObject;

	public GameObject previousSelectedSubMenuGameObject;

	private static PauseMenu _S;
	public static PauseMenu S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;
	}

	void Start() {
		UnPause();
    }

	void OnEnable() {
		canUpdate = true;

		try {
			// Display Player Stats (Level, HP, MP, EXP)
			UpdateGUI();

            // Deactivate all player gameObjects 
            for (int i = 0; i < playerGO.Count; i++) {
                playerGO[i].SetActive(false);
            }

            // Activate player gameObjects depending on party amount
            for (int i = 0; i <= Party.S.partyNdx; i++) {
                playerGO[i].SetActive(true);
            }

            // Activate Cursor
            ScreenCursor.S.cursorGO[0].SetActive(true);

            // Account for time that PauseScreen is not active
            timeWhenEnabled = Time.time;
			float tTime = timeWhenEnabled - timeWhenDisabled;
			minutes += (int)tTime / 60;
			seconds += (int)tTime % 60;

			StartCoroutine("FixedUpdateCoroutine");
		}
		catch (Exception e) {
			Debug.Log(e);
		}
	}

	void OnDisable() {
		// Deactivate screen cursors
		Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

		// Account for time that PauseScreen is not active
		timeWhenDisabled = Time.time;

		StopCoroutine("FixedUpdateCoroutine");
	}

	public void Loop() {
		if (GameManager.S.paused) {
			// Reset canUpdate
			if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f) {
				canUpdate = true;
			}

			if (canUpdate) {
				for (int i = 0; i < buttonGO.Count; i++) {
					if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == buttonGO[i]) {
						// Set Cursor Position to Selected Button
						Utilities.S.PositionCursor(buttonGO[i], 160);

						// Set selected button text color	
						buttonGO[i].gameObject.GetComponentInChildren<Text>().color = new Color32(205, 208, 0, 255);

						// Audio: Selection (when a new gameObject is selected)
						Utilities.S.PlayButtonSelectedSFX(ref previousSelectedGameObject);
					} else {
						// Set selected button text color	
						buttonGO[i].gameObject.GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
					}
				}
				canUpdate = false;
			}
		}
	}

	public IEnumerator FixedUpdateCoroutine() {
		// If Active
		if (isActiveAndEnabled) {
			// Increment seconds & reset timer
			if (timeDone <= Time.time) {
				seconds += 1;
				timeDone = 1 + Time.time;
			}

			// Increment minutes & reset seconds
			if (seconds > 59) {
				minutes += 1;
				seconds = 0;
			}

			// Display Time, Step Count, & Gold
			Time_Steps_Gold_TXT();
		}
		yield return new WaitForFixedUpdate();
		StartCoroutine("FixedUpdateCoroutine");
	}

	// ************ PAUSE ************ \\
	public void Pause() {
		// If SubMenu enabled when Paused, re-select this GO when Unpaused
		//previousSelectedSubMenuGameObject = null;
		//for (int i = 0; i < GameManager.S.gameSubMenu.buttonGO.Count; i++) {
		//	if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == GameManager.S.gameSubMenu.buttonGO[i]) {
		//		previousSelectedSubMenuGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
		//	}
		//	GameManager.S.gameSubMenu.buttonCS[i].interactable = false;
		//}

		gameObject.SetActive(true);

        // Buttons Interactable
        Utilities.S.ButtonsInteractable(buttonCS, true);

        // Set Selected Gameobject (Pause Screen: Items Button)
        Utilities.S.SetSelectedGO(buttonGO[0]);

        // Initialize previously selected GameObject
        previousSelectedGameObject = buttonGO[0];

        // Freeze player
        GameManager.S.paused = true;
		Blob.S.canMove = false;

        // Activate PauseMessage
        PauseMessage.S.DisplayText("Welcome to the Pause Screen!");

        // Audio: Confirm
        AudioManager.S.PlaySFX(eSoundName.confirm);

		// Update Delgate
		UpdateManager.updateDelegate += Loop;
	}

	public void UnPause(bool playSound = false) {
		// Unfreeze player
		GameManager.S.paused = false;
		Blob.S.canMove = true;

        // Deactivate PauseMessage
        PauseMessage.S.gameObject.SetActive(false);

        // If SubMenu enabled when Paused, re-select this GO when Unpaused
        // TBR: Account for SubMenu having 2 to 4 options
        //for (int i = 0; i < GameManager.S.gameSubMenu.buttonGO.Count; i++) {
        //	if (previousSelectedSubMenuGameObject == GameManager.S.gameSubMenu.buttonGO[i]) {
        //		Utilities.S.SetSelectedGO(previousSelectedSubMenuGameObject);
        //	}
        //	GameManager.S.gameSubMenu.buttonCS[i].interactable = true;
        //}

        if (playSound) {
			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}

		// Update Delegate
		UpdateManager.updateDelegate -= Loop;

		gameObject.SetActive(false);
	}

	// Returns the time in '0:00' format
	public string GetTime() {
        string zeroString = "";

        if (seconds < 10) {
            zeroString = "0";
        }

        return minutes.ToString() + ":" + zeroString + seconds.ToString();
    }

	// Display Time, Steps, & Gold
	void Time_Steps_Gold_TXT() {
        if (Blob.S) {
            // Time
            fileStatsNumText.text = GetTime() + "\n" +
            // Steps Count
            Blob.S.stepCount + "\n" +
            // Gold
            Party.S.gold;
		}
	}

	// Display Party Stats (Level, HP, MP, EXP)
	public void UpdateGUI() {
        for (int i = 0; i < Party.S.stats.Count; i++) {
            playerNameText[i].text = Party.S.stats[i].name;

            statsNumText[i].text = Party.S.stats[i].LVL + "\n" +
                Party.S.stats[i].HP + "/" + Party.S.stats[i].maxHP + "\n" +
                Party.S.stats[i].MP + "/" + Party.S.stats[i].maxMP + "\n" +
                Party.S.stats[i].EXP + "\n" +
                Party.S.GetExpToNextLevel(i);
        }
    }

	// Sets the menu's mini party member sprites' animations and text color
	// - Target party member is selected AUTOMATICALLY
	public void SetSelectedMemberAnim(string animName) {
		for (int i = 0; i < playerNameButtons.Count; i++) {
			if (playerNameButtons[i].gameObject.activeInHierarchy) {
				if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == playerNameButtons[i].gameObject) {
					playerAnims[i].CrossFade(animName, 0);
					playerNameText[i].color = new Color32(205, 208, 0, 255);
				} else {
					playerAnims[i].CrossFade("Idle", 0);
					playerNameText[i].color = new Color32(255, 255, 255, 255);
				}
			}
		}
	}

	// Sets the menu's mini party member sprites' animations and text color
	// - Target party member is selected MANUALLY
	public void SetSelectedMemberAnim(string animName, int ndx) {
		// Set all party members' anims and text color
		for (int i = 0; i < playerNameButtons.Count; i++) {
			if (playerNameButtons[i].gameObject.activeInHierarchy) {
				playerAnims[i].CrossFade("Idle", 0);
				playerNameText[i].color = new Color32(255, 255, 255, 255);
			}
		}

		// Set target anim and text color
		playerAnims[ndx].CrossFade(animName, 0);
		playerNameText[ndx].color = new Color32(205, 208, 0, 255);
	}

	// Activate the animation and text color of the previously selected party member
	public void SetPreviousSelectedPlayerAnimAndColor(GameObject previousGO) {
		if (previousGO == playerNameButtons[0].gameObject) {
			SetSelectedMemberAnim("Walk", 0);
		} else if (previousGO == playerNameButtons[1].gameObject) {
			SetSelectedMemberAnim("Walk", 1);
		} else if (previousGO == playerNameButtons[2].gameObject) {
			SetSelectedMemberAnim("Walk", 2);
		}
	}
}