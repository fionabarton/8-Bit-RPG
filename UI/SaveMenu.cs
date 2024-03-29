﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SaveMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	// Load, save, delete buttons
	public List<Button> actionButtons;

	// File slot buttons
	public List<Button> slotButtons;

	// File slot text descriptions (name, lvl, time, etc.)
	public List<Text> slotDataText;

	// Party member sprite game objects
	public List<GameObject> slot1PartyMemberSprites;
	public List<GameObject> slot2PartyMemberSprites;
	public List<GameObject> slot3PartyMemberSprites;

	public List<Animator> slot1PartyMemberAnims;
	public List<Animator> slot2PartyMemberAnims;
	public List<Animator> slot3PartyMemberAnims;

	// Reposition load and delete buttons depending on scene
	public RectTransform frameImageRectTrans;
	public RectTransform loadButtonRectTrans;
	public RectTransform deleteButtonRectTrans;

	[Header("Set Dynamically")]
	// For Input & Display Message
	public eSaveScreenMode saveScreenMode;

	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

	// Ensures audio is only played once when button is selected
	public GameObject previousSelectedActionButton;
	public GameObject previousSelectedSlotButton;

	// Index of the file the user is currently playing
	public int currentFileNdx;

	// Load, Save, Delete
	public int currentActionNdx;

	private static SaveMenu _S;
	public static SaveMenu S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;
	}

	void Start() {
		gameObject.SetActive(false);
	}

	void SetUp() {
		try {
			// Freeze Player
			if (GameManager.S.currentScene != "Title_Screen") {
				GameManager.S.paused = true;
				
				// Activate save button
				actionButtons[1].gameObject.SetActive(true);

				// Set action button navigation
				Utilities.S.SetButtonNavigation(actionButtons[0], actionButtons[2], actionButtons[1]);
				Utilities.S.SetButtonNavigation(actionButtons[2], actionButtons[1], actionButtons[0]);

				// Set action button positions
				loadButtonRectTrans.anchoredPosition = new Vector2(0, 70);
				deleteButtonRectTrans.anchoredPosition = new Vector2(0, -70);

				// Set action button frame size
				frameImageRectTrans.sizeDelta = new Vector2(205, 260);

				PauseMessage.S.DisplayText("Would you like to\nLoad, Save, or Delete a file?");
			} else {
				// Deactivate save button
				actionButtons[1].gameObject.SetActive(false);

				// Set action button navigation
				Utilities.S.SetButtonNavigation(actionButtons[0], actionButtons[2], actionButtons[2]);
				Utilities.S.SetButtonNavigation(actionButtons[2], actionButtons[0], actionButtons[0]);

				// Set action button positions
				loadButtonRectTrans.anchoredPosition = new Vector2(0, 35);
				deleteButtonRectTrans.anchoredPosition = new Vector2(0, -35);

				// Set action button frame size
				frameImageRectTrans.sizeDelta = new Vector2(205, 190);

				PauseMessage.S.DisplayText("Would you like to\nLoad or Delete a file?");
			}

			// Switch ScreenMode 
			saveScreenMode = eSaveScreenMode.pickAction;

			// Remove Listeners and Update GUI 
			Utilities.S.RemoveListeners(actionButtons);
			Utilities.S.RemoveListeners(slotButtons);

			UpdateGUI();

			// Buttons Interactable
			Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, false);
			Utilities.S.ButtonsInteractable(slotButtons, false);
			Utilities.S.ButtonsInteractable(actionButtons, true);

			// Set Selected GameObject
			Utilities.S.SetSelectedGO(previousSelectedActionButton);

            // Add Listeners
            actionButtons[0].onClick.AddListener(delegate { ClickedLoadSaveOrDelete(0); });
            actionButtons[1].onClick.AddListener(delegate { ClickedLoadSaveOrDelete(1); });
            actionButtons[2].onClick.AddListener(delegate { ClickedLoadSaveOrDelete(2); });

            // Reset slot buttons color
            Utilities.S.SetTextColor(slotButtons, new Color32(255, 255, 255, 255));

            canUpdate = true;
		}
		catch (Exception e) {
			Debug.Log(e);
		}
	}

	public void Activate() {
		// Ensures first slot is selected when screen enabled
		previousSelectedActionButton = actionButtons[0].gameObject;
		previousSelectedSlotButton = slotButtons[0].gameObject;

		SetUp();
		
		// Add Loop() to Update Delgate
		UpdateManager.updateDelegate += Loop;

        // Freeze player
        Player.S.canMove = false; 

        gameObject.SetActive(true);

        // Activate screen cursor
        ScreenCursor.S.cursorGO[0].SetActive(true);

        // Audio: Confirm
        AudioManager.S.PlaySFX(eSoundName.confirm);
    }

	public void Deactivate(bool playSound = false) {
		if (GameManager.S.currentScene != "Title_Screen") {
			// Unfreeze player
			GameManager.S.paused = false;
			Player.S.canMove = true;

			// Deactivate screen cursor
			ScreenCursor.S.cursorGO[0].SetActive(false);

			// Set Camera to Player gameObject
			CamManager.S.ChangeTarget(Player.S.gameObject, true);
		} else {
			TitleMenu.S.Activate();

			// Set Selected GameObject (New Game Button)
			Utilities.S.SetSelectedGO(TitleMenu.S.previousSelectedButton);

			// Buttons Interactable
			Utilities.S.ButtonsInteractable(TitleMenu.S.buttons, true);
		}

        if (playSound) {
            // Audio: Deny
            AudioManager.S.PlaySFX(eSoundName.deny);
        }

        // Deactivate PauseMessage
        PauseMessage.S.gameObject.SetActive(false);

        // Update Delegate
        UpdateManager.updateDelegate -= Loop;

		// Deactivate this gameObject
		gameObject.SetActive(false);
	}

	public void Loop() {
		// Reset canUpdate
		if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f) {
			canUpdate = true;
		}

		switch (saveScreenMode) {
			case eSaveScreenMode.pickAction:
				if (canUpdate) {
					for (int i = 0; i < actionButtons.Count; i++) {
						if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == actionButtons[i].gameObject) {
							// Set Cursor Position set to Selected Button
							Utilities.S.PositionCursor(actionButtons[i].gameObject, 100, 5);

							// Set selected button text color	
							actionButtons[i].GetComponentInChildren<Text>().color = new Color32(205, 208, 0, 255);

							// Audio: Selection (when a new gameObject is selected)
							Utilities.S.PlayButtonSelectedSFX(ref previousSelectedActionButton);
						} else {
							// Set non-selected button text color
							actionButtons[i].GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
						}
					}
					canUpdate = false;
				}

				if (Input.GetButtonDown("SNES Y Button")) {
					Deactivate(true);

					// Display text
					if(GameManager.S.currentScene != "Title_Screen") {
						DialogueManager.S.ResetSettings();
						DialogueManager.S.DisplayText("Thanks for the call.\nCatch ya later, babe!");
					}
				}
				break;
			case eSaveScreenMode.pickFile:
				if (canUpdate) {
					for (int i = 0; i < slotButtons.Count; i++) {
						if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == slotButtons[i].gameObject) {
							// Set Cursor Position set to Selected Button
							Utilities.S.PositionCursor(slotButtons[i].gameObject, 100, 5);

							// Set selected button text color	
							slotButtons[i].GetComponentInChildren<Text>().color = new Color32(205, 208, 0, 255);

							// Audio: Selection (when a new gameObject is selected)
							Utilities.S.PlayButtonSelectedSFX(ref previousSelectedSlotButton);

							// Set party sprite anim clips
							SetPartyMemberSpriteAnims(i, "Walk");
						} else {
							// Set non-selected button text color
							slotButtons[i].GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);

							// Set party sprite anim clips
							SetPartyMemberSpriteAnims(i, "Idle");
						}
					}
					canUpdate = false;
				}

				if (Input.GetButtonDown("SNES Y Button")) {
					// Audio: Deny
					AudioManager.S.PlaySFX(eSoundName.deny);

					SetUp();
				}
				break;
			case eSaveScreenMode.subMenu:
				if (Input.GetButtonDown("SNES Y Button")) {
					No(currentActionNdx);
				}
				break;
			case eSaveScreenMode.cannotPeformAction:
				if (PauseMessage.S.dialogueFinished) {
					if (Input.GetButtonDown("SNES B Button") || Input.GetButtonDown("SNES Y Button")) {
						// Audio: Deny
						AudioManager.S.PlaySFX(eSoundName.deny);

						ClickedLoadSaveOrDelete(currentActionNdx);
					}
				}
				break;
			case eSaveScreenMode.pickedFile:
				if (PauseMessage.S.dialogueFinished) {
					if (Input.GetButtonDown("SNES B Button") || Input.GetButtonDown("SNES Y Button")) {
						// Audio: Confirm
						AudioManager.S.PlaySFX(eSoundName.confirm);

						SetUp();
					}
				}
				break;
		}
	}
	///////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
	void ClickedLoadSaveOrDelete(int loadSaveOrDelete, bool playSound = true) {
		// Remove Listeners and Update GUI 
		Utilities.S.RemoveListeners(actionButtons);
		UpdateGUI();

		//Buttons Interactable
		Utilities.S.ButtonsInteractable(slotButtons, true);
		Utilities.S.ButtonsInteractable(actionButtons, false);

		// Set Selected GameObject
		Utilities.S.SetSelectedGO(slotButtons[0].gameObject);

		currentActionNdx = loadSaveOrDelete;

		switch (loadSaveOrDelete) {
			case 0:
				slotButtons[0].onClick.AddListener(delegate { ClickedLoadButton(0); });
				slotButtons[1].onClick.AddListener(delegate { ClickedLoadButton(1); });
				slotButtons[2].onClick.AddListener(delegate { ClickedLoadButton(2); });

				PauseMessage.S.DisplayText("Load which file?");
				break;
			case 1:
				slotButtons[0].onClick.AddListener(delegate { ClickedSaveButton(0); });
				slotButtons[1].onClick.AddListener(delegate { ClickedSaveButton(1); });
				slotButtons[2].onClick.AddListener(delegate { ClickedSaveButton(2); });

				PauseMessage.S.DisplayText("Save your progress to which file?");
				break;
			case 2:
				slotButtons[0].onClick.AddListener(delegate { ClickedDeleteButton(0); });
				slotButtons[1].onClick.AddListener(delegate { ClickedDeleteButton(1); });
				slotButtons[2].onClick.AddListener(delegate { ClickedDeleteButton(2); });

				PauseMessage.S.DisplayText("Delete which file?");
				break;
		}

		// Audio: Confirm
		if (playSound) {
			AudioManager.S.PlaySFX(eSoundName.confirm);
		}

		canUpdate = true;

		saveScreenMode = eSaveScreenMode.pickFile;
	}
	///////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
	void ClickedLoadButton(int fileNdx) {
		// Prevent empty files from being loaded
		if (PlayerPrefs.HasKey(fileNdx + "Time")) {
			if (PlayerPrefs.GetString(fileNdx + "Time") == "0:00") {
				// Cannot perform this action
				CannotPerformAction("This file cannot be loaded because it is empty!");
			} else {
				// Set Text
				ExitGameMenu.S.Activate("Are you sure that you would like to load this file?", false);

				// Set OnClick Methods
				Utilities.S.RemoveListeners(ExitGameMenu.S.buttonCS);
				ExitGameMenu.S.buttonCS[0].onClick.AddListener(delegate { LoadFile(fileNdx); });
				ExitGameMenu.S.buttonCS[1].onClick.AddListener(delegate { No(0); });

				ClickedActionButtonHelper();
			}
		} else {
			// Cannot perform this action
			CannotPerformAction("This file cannot be loaded because it is empty!");
		}
	}

	void ClickedSaveButton(int fileNdx) {
		// Set Text
		if (PlayerPrefs.HasKey(fileNdx + "Time")) {
			if (PlayerPrefs.GetString(fileNdx + "Time") == "0:00") {
				ExitGameMenu.S.Activate("Are you sure that you would like to save your progress to this empty file?", false);
			} else {
				ExitGameMenu.S.Activate("This file contains existing save data,\nare you sure that you would like to save your progress over it?", false);
			}
        } else {		
			ExitGameMenu.S.Activate("This file contains existing save data,\nare you sure that you would like to save over it?", false);
		}

		// Set OnClick Methods
		Utilities.S.RemoveListeners(ExitGameMenu.S.buttonCS);
		ExitGameMenu.S.buttonCS[0].onClick.AddListener(delegate { SaveFile(fileNdx); });
		ExitGameMenu.S.buttonCS[1].onClick.AddListener(delegate { No(1); });

		ClickedActionButtonHelper();
	}

	void ClickedDeleteButton(int fileNdx) {
		// Prevent empty files from being deleted
		if (PlayerPrefs.HasKey(fileNdx + "Time")) {
			if (PlayerPrefs.GetString(fileNdx + "Time") == "0:00") {
				// Cannot perform this action
				CannotPerformAction("This file cannot be deleted because it is empty!");
			} else {
				// Set Text
				ExitGameMenu.S.Activate("Are you sure that you would like to delete this file?", false);

				// Set OnClick Methods
				Utilities.S.RemoveListeners(ExitGameMenu.S.buttonCS);
				ExitGameMenu.S.buttonCS[0].onClick.AddListener(delegate { DeleteFile(fileNdx); });
				ExitGameMenu.S.buttonCS[1].onClick.AddListener(delegate { No(2); });

				ClickedActionButtonHelper();
			}
		} else {
			// Cannot perform this action
			CannotPerformAction("This file cannot be deleted because it is empty!");
		}
	}

	void ClickedActionButtonHelper() {
		// Remove Listeners
		Utilities.S.RemoveListeners(slotButtons);

		// Buttons Interactable
		Utilities.S.ButtonsInteractable(slotButtons, false);

		// Deactivate screen cursor
		ScreenCursor.S.cursorGO[0].SetActive(false);

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);

		saveScreenMode = eSaveScreenMode.subMenu;
	}

	void CannotPerformAction(string message) {
		PauseMessage.S.DisplayText(message);

		//Buttons Interactable
		Utilities.S.ButtonsInteractable(slotButtons, false);

		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.deny);

		canUpdate = true;

		saveScreenMode = eSaveScreenMode.cannotPeformAction;
	}
	///////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
	void LoadFile(int fileNdx) {
		ExitGameMenu.S.Deactivate();

		// Remove listeners
		Utilities.S.RemoveListeners(ExitGameMenu.S.buttonCS);

		// Set party sprite anim clips
		SetPartyMemberSpriteAnims(fileNdx, "Success");

		// Reset stats to starting stats
		Party.S.stats.Clear();

		// Player 1
		Party.S.stats.Add(new PartyStats("Blob", 40, 40, 40, 6, 6, 6,
            2, 2, 2, 2, 1, 1, 1, 1,
            0, 1, 13,
            new List<Spell> { Spells.S.spells[1], Spells.S.spells[0], Spells.S.spells[2], Spells.S.spells[4], Spells.S.spells[5], Spells.S.spells[3], Spells.S.spells[6], Spells.S.spells[7], Spells.S.spells[8], Spells.S.spells[9], Spells.S.spells[10], Spells.S.spells[11], Spells.S.spells[12] },
            new List<bool>(new bool[30]),
            new List<int> { 0, 0, 7, 23, 47, 110, 220, 450, 800, 1300, 2000 },
            false, 0, 0)
        );
        // Player 2
        Party.S.stats.Add(new PartyStats("Girl", 32, 32, 32, 15, 15, 15,
            1, 1, 1, 1, 2, 2, 2, 2,
            0, 1, 13,
            new List<Spell> { Spells.S.spells[3], Spells.S.spells[1], Spells.S.spells[0], Spells.S.spells[4], Spells.S.spells[5], Spells.S.spells[2], Spells.S.spells[6], Spells.S.spells[7], Spells.S.spells[8], Spells.S.spells[9], Spells.S.spells[10], Spells.S.spells[11], Spells.S.spells[12] },
            new List<bool>(new bool[30]),
            new List<int> { 0, 0, 9, 23, 55, 110, 250, 450, 850, 1300, 2100 },
            false, 0, 1)
        );
        // Player 3
        Party.S.stats.Add(new PartyStats("Boy", 25, 25, 25, 10, 10, 10,
            1, 1, 1, 1, 2, 2, 2, 2,
            0, 1, 10,
            new List<Spell> { Spells.S.spells[3], Spells.S.spells[4], Spells.S.spells[0], Spells.S.spells[2], Spells.S.spells[1], Spells.S.spells[5], Spells.S.spells[6], Spells.S.spells[7], Spells.S.spells[8], Spells.S.spells[9] },
            new List<bool>(new bool[30]),
            new List<int> { 0, 0, 9, 23, 55, 110, 250, 450, 850, 1300, 2100 },
            false, 0, 2)
        );

        if (PlayerPrefs.HasKey(fileNdx + "Player1Level")) { Party.S.stats[0].LVL = PlayerPrefs.GetInt(fileNdx + "Player1Level"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player2Level")) { Party.S.stats[1].LVL = PlayerPrefs.GetInt(fileNdx + "Player2Level"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player3Level")) { Party.S.stats[2].LVL = PlayerPrefs.GetInt(fileNdx + "Player3Level"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player1Exp")) { Party.S.stats[0].EXP = PlayerPrefs.GetInt(fileNdx + "Player1Exp"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player2Exp")) { Party.S.stats[1].EXP = PlayerPrefs.GetInt(fileNdx + "Player2Exp"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player3Exp")) { Party.S.stats[2].EXP = PlayerPrefs.GetInt(fileNdx + "Player3Exp"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player1IsPoisoned")) { StatusEffects.S.playerIsPoisoned[0] = Convert.ToBoolean(PlayerPrefs.GetInt(fileNdx + "Player1IsPoisoned")); }
		if (PlayerPrefs.HasKey(fileNdx + "Player2IsPoisoned")) { StatusEffects.S.playerIsPoisoned[1] = Convert.ToBoolean(PlayerPrefs.GetInt(fileNdx + "Player2IsPoisoned")); }
		if (PlayerPrefs.HasKey(fileNdx + "Player3IsPoisoned")) { StatusEffects.S.playerIsPoisoned[2] = Convert.ToBoolean(PlayerPrefs.GetInt(fileNdx + "Player3IsPoisoned")); }
		if (PlayerPrefs.HasKey(fileNdx + "Player1Name")) { Party.S.stats[0].name = PlayerPrefs.GetString(fileNdx + "Player1Name"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player2Name")) { Party.S.stats[1].name = PlayerPrefs.GetString(fileNdx + "Player2Name"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player3Name")) { Party.S.stats[2].name = PlayerPrefs.GetString(fileNdx + "Player3Name"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player1Gear")) { EquipMenu.S.GetEquippedGearString(PlayerPrefs.GetString(fileNdx + "Player1Gear"), 0); }
		if (PlayerPrefs.HasKey(fileNdx + "Player2Gear")) { EquipMenu.S.GetEquippedGearString(PlayerPrefs.GetString(fileNdx + "Player2Gear"), 1); }
		if (PlayerPrefs.HasKey(fileNdx + "Player3Gear")) { EquipMenu.S.GetEquippedGearString(PlayerPrefs.GetString(fileNdx + "Player3Gear"), 2); }
		if (PlayerPrefs.HasKey(fileNdx + "Gold")) { Party.S.gold = PlayerPrefs.GetInt(fileNdx + "Gold"); }
		if (PlayerPrefs.HasKey(fileNdx + "Steps")) { Player.S.stepCount = PlayerPrefs.GetInt(fileNdx + "Steps"); }
		if (PlayerPrefs.HasKey(fileNdx + "Time")) { PauseMenu.S.fileStatsNumText.text = PlayerPrefs.GetString(fileNdx + "Time"); } // Stores Time in 0:00 format
		if (PlayerPrefs.HasKey(fileNdx + "Seconds")) { PauseMenu.S.seconds = PlayerPrefs.GetInt(fileNdx + "Seconds"); }
		if (PlayerPrefs.HasKey(fileNdx + "Minutes")) { PauseMenu.S.minutes = PlayerPrefs.GetInt(fileNdx + "Minutes"); }
		if (PlayerPrefs.HasKey(fileNdx + "LocationNdx")) { WarpManager.S.locationNdx = PlayerPrefs.GetInt(fileNdx + "LocationNdx"); }
		if (PlayerPrefs.HasKey(fileNdx + "LocationName")) { WarpManager.S.locationName = PlayerPrefs.GetString(fileNdx + "LocationName"); }
		if (PlayerPrefs.HasKey(fileNdx + "VisitedLocations")) { WarpManager.S.visitedLocationNdxs = PlayerPrefs.GetString(fileNdx + "VisitedLocations"); }
		if (PlayerPrefs.HasKey(fileNdx + "Inventory")) { Inventory.S.GetInventoryFromString(PlayerPrefs.GetString(fileNdx + "Inventory")); }
		if (PlayerPrefs.HasKey(fileNdx + "UnlockedDoors")) { DoorManager.S.UnlockDoorsFromString(PlayerPrefs.GetString(fileNdx + "UnlockedDoors")); }
		if (PlayerPrefs.HasKey(fileNdx + "OpenChests")) { ChestManager.S.OpenChestsFromString(PlayerPrefs.GetString(fileNdx + "OpenChests")); }
		if (PlayerPrefs.HasKey(fileNdx + "QuestsCompleted")) { QuestManager.S.GetIsCompletedFromString(PlayerPrefs.GetString(fileNdx + "QuestsCompleted")); }
		if (PlayerPrefs.HasKey(fileNdx + "KeyItemsDeactivated")) { KeyItemManager.S.DeactivateItemsFromString(PlayerPrefs.GetString(fileNdx + "KeyItemsDeactivated")); }
		if (PlayerPrefs.HasKey(fileNdx + "PartyNdx")) { Party.S.partyNdx = PlayerPrefs.GetInt(fileNdx + "PartyNdx"); }

		// Level Up
		Party.S.CheckForLevelUp();
		Party.S.stats[0].hasLeveledUp = false;
		Party.S.stats[1].hasLeveledUp = false;
		Party.S.stats[2].hasLeveledUp = false;

		// Set current HP/MP
		if (PlayerPrefs.HasKey(fileNdx + "Player1HP")) { Party.S.stats[0].HP = PlayerPrefs.GetInt(fileNdx + "Player1HP"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player2HP")) { Party.S.stats[1].HP = PlayerPrefs.GetInt(fileNdx + "Player2HP"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player3HP")) { Party.S.stats[2].HP = PlayerPrefs.GetInt(fileNdx + "Player3HP"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player1MP")) { Party.S.stats[0].MP = PlayerPrefs.GetInt(fileNdx + "Player1MP"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player2MP")) { Party.S.stats[1].MP = PlayerPrefs.GetInt(fileNdx + "Player2MP"); }
		if (PlayerPrefs.HasKey(fileNdx + "Player3MP")) { Party.S.stats[2].MP = PlayerPrefs.GetInt(fileNdx + "Player3MP"); }

		// Deactivate all party members
		for (int i = 0; i < Player.S.followers.followersGO.Count; i++) {
			Player.S.followers.followersGO[i].SetActive(false);
		}

		// Activate party members
		for (int i = 1; i <= Party.S.partyNdx; i++) {
			Player.S.followers.followersGO[i - 1].SetActive(true);
		}

		currentFileNdx = fileNdx;

		// Separate string into individual chars
		char[] visitedLocationsNdxArray = WarpManager.S.visitedLocationNdxs.ToCharArray();

		// Add saved visited locations
		WarpManager.S.visitedLocations.Clear();
		for (int i = 0; i < visitedLocationsNdxArray.Length; i++) {
			int ndx = visitedLocationsNdxArray[i] - 48;

			WarpManager.S.visitedLocations.Add(WarpManager.S.locations[ndx]);
		}

		// Close Curtains
		Curtain.S.Close();

		// Audio: Buff 2
		AudioManager.S.PlaySFX(eSoundName.buff2);

		// Delay, then Load Scene
		Invoke("Warp", 1f);
    }


    void Warp() {
        // Warp to this file's current location
        StartCoroutine(WarpManager.S.Warp(
            WarpManager.S.visitedLocations[PlayerPrefs.GetInt(currentFileNdx + "LocationNdx")].position,
            true,
            WarpManager.S.visitedLocations[PlayerPrefs.GetInt(currentFileNdx + "LocationNdx")].sceneName));
    }

    void SaveFile(int fileNdx) {
		ExitGameMenu.S.Deactivate();

		PlayerPrefs.SetInt(fileNdx + "Player1Level", Party.S.stats[0].LVL);
		PlayerPrefs.SetInt(fileNdx + "Player2Level", Party.S.stats[1].LVL);
		PlayerPrefs.SetInt(fileNdx + "Player3Level", Party.S.stats[2].LVL);
		PlayerPrefs.SetInt(fileNdx + "Player1Exp", Party.S.stats[0].EXP);
		PlayerPrefs.SetInt(fileNdx + "Player2Exp", Party.S.stats[1].EXP);
		PlayerPrefs.SetInt(fileNdx + "Player3Exp", Party.S.stats[2].EXP);
		PlayerPrefs.SetInt(fileNdx + "Player1HP", Party.S.stats[0].HP);
		PlayerPrefs.SetInt(fileNdx + "Player2HP", Party.S.stats[1].HP);
		PlayerPrefs.SetInt(fileNdx + "Player3HP", Party.S.stats[2].HP);
		PlayerPrefs.SetInt(fileNdx + "Player1MP", Party.S.stats[0].MP);
		PlayerPrefs.SetInt(fileNdx + "Player2MP", Party.S.stats[1].MP);
		PlayerPrefs.SetInt(fileNdx + "Player3MP", Party.S.stats[2].MP);
		PlayerPrefs.SetInt(fileNdx + "Player1IsPoisoned", Convert.ToInt32(StatusEffects.S.playerIsPoisoned[0]));
		PlayerPrefs.SetInt(fileNdx + "Player2IsPoisoned", Convert.ToInt32(StatusEffects.S.playerIsPoisoned[1]));
		PlayerPrefs.SetInt(fileNdx + "Player3IsPoisoned", Convert.ToInt32(StatusEffects.S.playerIsPoisoned[2]));
		PlayerPrefs.SetString(fileNdx + "Player1Name", Party.S.stats[0].name);
		PlayerPrefs.SetString(fileNdx + "Player2Name", Party.S.stats[1].name);
		PlayerPrefs.SetString(fileNdx + "Player3Name", Party.S.stats[2].name);
		PlayerPrefs.SetString(fileNdx + "Player1Gear", EquipMenu.S.GetEquippedGearString(0));
		PlayerPrefs.SetString(fileNdx + "Player2Gear", EquipMenu.S.GetEquippedGearString(1));
		PlayerPrefs.SetString(fileNdx + "Player3Gear", EquipMenu.S.GetEquippedGearString(2));
		PlayerPrefs.SetInt(fileNdx + "Gold", Party.S.gold);
		PlayerPrefs.SetInt(fileNdx + "Steps", Player.S.stepCount);
		PlayerPrefs.SetString(fileNdx + "Time", PauseMenu.S.GetTime()); // Stores Time in 0:00 format
		PlayerPrefs.SetInt(fileNdx + "Seconds", PauseMenu.S.seconds);
		PlayerPrefs.SetInt(fileNdx + "Minutes", PauseMenu.S.minutes);		
		PlayerPrefs.SetInt(fileNdx + "LocationNdx", WarpManager.S.locationNdx);
		PlayerPrefs.SetString(fileNdx + "LocationName", WarpManager.S.locationName);
		PlayerPrefs.SetString(fileNdx + "VisitedLocations", WarpManager.S.visitedLocationNdxs);
		PlayerPrefs.SetString(fileNdx + "Inventory", Inventory.S.GetInventoryString());
		PlayerPrefs.SetString(fileNdx + "UnlockedDoors", DoorManager.S.GetIsUnlockedString());
		PlayerPrefs.SetString(fileNdx + "OpenChests", ChestManager.S.GetIsOpenString());
		PlayerPrefs.SetString(fileNdx + "QuestsCompleted", QuestManager.S.GetIsCompletedString());
		PlayerPrefs.SetString(fileNdx + "KeyItemsDeactivated", KeyItemManager.S.GetIsDeactivatedString());
		PlayerPrefs.SetInt(fileNdx + "PartyNdx", Party.S.partyNdx);

		FileHelper("Saved game!");
	}

	void DeleteFile(int fileNdx) {
		ExitGameMenu.S.Deactivate();

		PlayerPrefs.SetInt(fileNdx + "Player1Level", 0);
		PlayerPrefs.SetInt(fileNdx + "Player2Level", 0);
		PlayerPrefs.SetInt(fileNdx + "Player3Level", 0);
		PlayerPrefs.SetInt(fileNdx + "Player1Exp", 0);
		PlayerPrefs.SetInt(fileNdx + "Player2Exp", 0);
		PlayerPrefs.SetInt(fileNdx + "Player3Exp", 0);
		PlayerPrefs.SetInt(fileNdx + "Player1HP", 0);
		PlayerPrefs.SetInt(fileNdx + "Player2HP", 0);
		PlayerPrefs.SetInt(fileNdx + "Player3HP", 0);
		PlayerPrefs.SetInt(fileNdx + "Player1MP", 0);
		PlayerPrefs.SetInt(fileNdx + "Player2MP", 0);
		PlayerPrefs.SetInt(fileNdx + "Player3MP", 0);
		PlayerPrefs.SetInt(fileNdx + "Player1IsPoisoned", 0);
		PlayerPrefs.SetInt(fileNdx + "Player2IsPoisoned", 0);
		PlayerPrefs.SetInt(fileNdx + "Player3IsPoisoned", 0);
		PlayerPrefs.SetString(fileNdx + "Player1Name", "");
		PlayerPrefs.SetString(fileNdx + "Player2Name", "");
		PlayerPrefs.SetString(fileNdx + "Player3Name", "");
		PlayerPrefs.SetString(fileNdx + "Player1Gear", "");
		PlayerPrefs.SetString(fileNdx + "Player2Gear", "");
		PlayerPrefs.SetString(fileNdx + "Player3Gear", "");
		PlayerPrefs.SetInt(fileNdx + "Gold", 0);
		PlayerPrefs.SetInt(fileNdx + "Steps", 0);
		PlayerPrefs.SetString(fileNdx + "Time", "0:00"); // Stores Time in 0:00 format
		PlayerPrefs.SetInt(fileNdx + "Seconds", 0);
		PlayerPrefs.SetInt(fileNdx + "Minutes", 0);
		PlayerPrefs.SetInt(fileNdx + "LocationNdx", 0);
		PlayerPrefs.SetString(fileNdx + "LocationName", "");
		PlayerPrefs.SetString(fileNdx + "VisitedLocations", "");
		PlayerPrefs.SetString(fileNdx + "Inventory", "");
		PlayerPrefs.SetString(fileNdx + "UnlockedDoors", "");
		PlayerPrefs.SetString(fileNdx + "OpenChests", "");
		PlayerPrefs.SetString(fileNdx + "QuestsCompleted", "");
		PlayerPrefs.SetString(fileNdx + "KeyItemsDeactivated", "");
		PlayerPrefs.SetInt(fileNdx + "PartyNdx", 0);

		FileHelper("Deleted game!");
	}

	void FileHelper(string message) {
		// Audio: Buff 1
		AudioManager.S.PlaySFX(eSoundName.buff1);

		// Reset sub menu
		GameManager.S.pauseSubMenu.ResetSettings();

		// Reactivate screen cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);

		// Remove Listeners and Update GUI 
		Utilities.S.RemoveListeners(slotButtons);
		UpdateGUI();

		//Buttons Interactable
		Utilities.S.ButtonsInteractable(slotButtons, false);
		Utilities.S.ButtonsInteractable(actionButtons, false);

		saveScreenMode = eSaveScreenMode.pickedFile;

		PauseMessage.S.DisplayText(message);
	}

	void No(int actionNdx) {
		ExitGameMenu.S.Deactivate();

		AudioManager.S.PlaySFX(eSoundName.deny);
		GameManager.S.pauseSubMenu.ResetSettings();

		// Reactivate screen cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);

		ClickedLoadSaveOrDelete(actionNdx, false);
	}
	///////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
	void UpdateGUI() {
		// Deactivate all slot party member sprites
		Utilities.S.SetActiveList(slot1PartyMemberSprites, false);
		Utilities.S.SetActiveList(slot2PartyMemberSprites, false);
		Utilities.S.SetActiveList(slot3PartyMemberSprites, false);

		for (int i = 0; i < slotDataText.Count; i++) {
			// Set slot text
			if (PlayerPrefs.HasKey(i + "Time")) {
				if (PlayerPrefs.GetString(i + "Time") == "0:00") {
					slotDataText[i].text = "<color=yellow>New Game</color>";
				} else {
					slotDataText[i].text =
					"<color=yellow>Name:</color> " + PlayerPrefs.GetString(i + "Player1Name") + "    " + "<color=yellow>Level:</color> " + PlayerPrefs.GetInt(i + "Player1Level") + "    " +
					"<color=yellow>Time:</color> " + PlayerPrefs.GetString(i + "Time") + "\n" + "<color=yellow>Location:</color> " + PlayerPrefs.GetString(i + "LocationName") + "    " +
					"<color=yellow>Gold:</color> " + PlayerPrefs.GetInt(i + "Gold");
				}
			} else {
				slotDataText[i].text =
					"<color=yellow>Name:</color> " + PlayerPrefs.GetString(i + "Player1Name") + "    " + "<color=yellow>Level:</color> " + PlayerPrefs.GetInt(i + "Player1Level") + "    " +
					"<color=yellow>Time:</color> " + PlayerPrefs.GetString(i + "Time") + "\n" + "<color=yellow>Location:</color> " + PlayerPrefs.GetString(i + "LocationName") + "    " +
					"<color=yellow>Gold:</color> " + PlayerPrefs.GetInt(i + "Gold");
			}

			// Activate slot party member sprites
			if (PlayerPrefs.HasKey(i + "Time")) {
				if (PlayerPrefs.GetString(i + "Time") != "0:00") {
					for (int j = 0; j <= PlayerPrefs.GetInt(i + "PartyNdx"); j++) {
						switch (i) {
							case 0:
								slot1PartyMemberSprites[j].SetActive(true);
								break;
							case 1:
								slot2PartyMemberSprites[j].SetActive(true);
								break;
							case 2:
								slot3PartyMemberSprites[j].SetActive(true);
								break;
						}
					}
				}
			}
		}
		PauseMenu.S.UpdateGUI();
	}

	// Set party sprite anim clips
	void SetPartyMemberSpriteAnims(int fileNdx, string clipName) {
		for (int j = 0; j < slot1PartyMemberAnims.Count; j++) {
			switch (fileNdx) {
				case 0:
					if (slot1PartyMemberAnims[j].gameObject.activeInHierarchy) {
						slot1PartyMemberAnims[j].CrossFade(clipName, 0);
					}
					break;
				case 1:
					if (slot2PartyMemberAnims[j].gameObject.activeInHierarchy) {
						slot2PartyMemberAnims[j].CrossFade(clipName, 0);
					}
					break;
				case 2:
					if (slot3PartyMemberAnims[j].gameObject.activeInHierarchy) {
						slot3PartyMemberAnims[j].CrossFade(clipName, 0);
					}
					break;
			}
		}
	}
}