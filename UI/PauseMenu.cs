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

	// Stat frame animators
	public List<Animator> statFrameAnims;

	// Parent gameObject of BlackScreen & Player 1-3
	// Used to change its order in hierarchy for items, skills, & gear menus
	public GameObject partyMembersGO;

	[Header("Set Dynamically")]
	// Amount of time spent playing (excluding time spent on title screen)
	public int seconds;
	public int minutes;

	// Resets timer
	public float timeDone;

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
		SetUp();
		gameObject.SetActive(false);
	}

	void SetUp() {
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

			// Set party stats UI positions
			switch (Party.S.partyNdx) {
				case 0:
					Utilities.S.SetRectPosition(playerGO[0], 0, 328);
					break;
				case 1:
					Utilities.S.SetRectPosition(playerGO[0], -200, 328);
					Utilities.S.SetRectPosition(playerGO[1], 200, 328);
					break;
				case 2:
					Utilities.S.SetRectPosition(playerGO[0], -400, 328);
					Utilities.S.SetRectPosition(playerGO[1], 0, 328);
					Utilities.S.SetRectPosition(playerGO[2], 400, 328);
					break;
			}

			// Activate Cursor
			ScreenCursor.S.cursorGO[0].SetActive(true);
		}
		catch (Exception e) {
			Debug.Log(e);
		}
	}

    void OnDisable() {
		// Deactivate screen cursors
		Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);
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

	// ************ PAUSE ************ \\
	public void Pause() {
        // If SubMenu enabled when Paused, re-select this GO when Unpaused
        previousSelectedSubMenuGameObject = null;
        for (int i = 0; i < GameManager.S.gameSubMenu.buttonGO.Count; i++) {
            if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == GameManager.S.gameSubMenu.buttonGO[i]) {
                previousSelectedSubMenuGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            }
            GameManager.S.gameSubMenu.buttonCS[i].interactable = false;
        }

        gameObject.SetActive(true);

		SetUp();

        // Buttons Interactable
        Utilities.S.ButtonsInteractable(buttonCS, true);

        // Set Selected Gameobject (Pause Screen: Items Button)
        Utilities.S.SetSelectedGO(buttonGO[0]);

        // Initialize previously selected GameObject
        previousSelectedGameObject = buttonGO[0];

        // Freeze player
        GameManager.S.paused = true;
		Player.S.canMove = false;

        // Activate PauseMessage
        PauseMessage.S.DisplayText("Welcome to the Pause Screen!");

        // Audio: Confirm
        AudioManager.S.PlaySFX(eSoundName.confirm);

		// Update Delgate
		UpdateManager.updateDelegate += Loop;
	}

    public void UnPause(bool playSound = false) {
		// Unpause game
		GameManager.S.paused = false;

		// If sub menu is inactive, unfreeze player
		if (!GameManager.S.gameSubMenu.gameObject.activeInHierarchy) {
			Player.S.canMove = true;
		}

        // Deactivate PauseMessage
        PauseMessage.S.gameObject.SetActive(false);

        // If SubMenu enabled when Paused, re-select this GO when Unpaused
        for (int i = 0; i < GameManager.S.gameSubMenu.buttonGO.Count; i++) {
            if (previousSelectedSubMenuGameObject == GameManager.S.gameSubMenu.buttonGO[i]) {
                Utilities.S.SetSelectedGO(previousSelectedSubMenuGameObject);
            }
            GameManager.S.gameSubMenu.buttonCS[i].interactable = true;
        }

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
	public void Time_Steps_Gold_TXT() {
        if (isActiveAndEnabled) {
            // Time
            fileStatsNumText.text = GetTime() + "\n" +
            // Steps Count
            Player.S.stepCount + "\n" +
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
	public void SetSelectedMemberAnim(string animName, bool setStatFrameAnim = false) {
		for (int i = 0; i < playerNameButtons.Count; i++) {
			if (playerNameButtons[i].gameObject.activeInHierarchy) {
				if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == playerNameButtons[i].gameObject) {
					playerAnims[i].CrossFade(animName, 0);

                    if (setStatFrameAnim) {
						if (animName == "Idle") {
							statFrameAnims[i].CrossFade(animName, 0);
						} else {
							statFrameAnims[i].CrossFade("Flash", 0);
						}
					}
				} else {
					playerAnims[i].CrossFade("Idle", 0);

					if (setStatFrameAnim) {
						statFrameAnims[i].CrossFade("Idle", 0);
					}
				}
			}
		}
	}

	// Sets the menu's mini party member sprites' animations and text color
	// - Target party member is selected MANUALLY
	public void SetSelectedMemberAnim(string animName, int playerNdx) {
		// Set all party members' anims
		for (int i = 0; i < playerNameButtons.Count; i++) {
			if (playerNameButtons[i].gameObject.activeInHierarchy) {
				playerAnims[i].CrossFade("Idle", 0);
			}
		}

		// Set target anim 
		playerAnims[playerNdx].CrossFade(animName, 0);
	}

	// Play the previously played animation clip of the selected party member
	public void SetPreviousSelectedPlayerAnimAndColor(string animName, int playerNdx) {
		for (int i = 0; i < playerNameButtons.Count; i++) {
			if (playerNameButtons[i].gameObject.activeInHierarchy) {
				if (i == playerNdx) {
					playerAnims[i].CrossFade(animName, 0);
				} else {
					playerAnims[i].CrossFade("Idle", 0);
				}
			}
		}
	}

	// Used to set 'partyMembersGO' above/below items, skills, & gear menus
	public void SwapPartyMemberGOParentAndOrderInHierarchy(bool isAboveMenus = true) {
		if(isAboveMenus) {
			partyMembersGO.transform.SetParent(Battle.S.gameObject.transform);
			partyMembersGO.transform.SetSiblingIndex(11);
        } else {
			partyMembersGO.transform.SetParent(gameObject.transform);
			partyMembersGO.transform.SetAsFirstSibling();
		}
	}
}