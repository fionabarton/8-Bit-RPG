using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
/*
TO BE IMPLEMENTED:
Current HP/MP, Steps, Party Members, Inventory, Equipment, Doors/Chests/KeyItems, Quests Completed/Activated
 */
public class SaveMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	public List<Button> slotButtons;

	public List<Text> slotDataText;

	// Load, Save, Delete
	public List<Button> actionButtons;

	[Header("Set Dynamically")]
	// For Input & Display Message
	public eSaveScreenMode saveScreenMode;

	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

	// Ensures audio is only played once when button is selected
	GameObject previousSelectedActionButton;
	GameObject previousSelectedSlotButton;

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
			}

			// Switch ScreenMode 
			saveScreenMode = eSaveScreenMode.pickAction;

			// Remove Listeners and Update GUI 
			Utilities.S.RemoveListeners(actionButtons);
			Utilities.S.RemoveListeners(slotButtons);

			UpdateGUI();

			PauseMessage.S.DisplayText("Would you like to\nLoad, Save, or Delete a file?");

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

			canUpdate = true;
		}
		catch (Exception e) {
			Debug.Log(e);
		}
	}

	public void Activate() {
		// Ensures first slot is selected when screen enabled
		previousSelectedActionButton = actionButtons[0].gameObject;

		SetUp();

		// Add Loop() to Update Delgate
		UpdateManager.updateDelegate += Loop;

		// Freeze player
		GameManager.S.paused = true;
		Player.S.canMove = false;

		gameObject.SetActive(true);

		// Activate screen cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);
	}

	public void Deactivate(bool playSound = false) {
		if (GameManager.S.currentScene != "Battle" || GameManager.S.currentScene != "Title_Screen") {
			// Unfreeze player
			GameManager.S.paused = false;
			Player.S.canMove = true;

			// Deactivate PauseMessage
			PauseMessage.S.gameObject.SetActive(false);

			// Buttons Interactable
			Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, true);

			// Set previously selected GameObject
			//PauseMenu.S.previousSelectedGameObject = PauseMenu.S.buttonGO[4];

			// Set Selected Gameobject (Pause Screen: Save Button)
			//Utilities.S.SetSelectedGO(PauseMenu.S.buttonGO[4]);

			// Deactivate screen cursor
			ScreenCursor.S.cursorGO[0].SetActive(false);

			PauseMenu.S.canUpdate = true;
		}

		if (GameManager.S.currentScene == "Title_Screen") {
			// Set Selected GameObject (New Game Button)
			Utilities.S.SetSelectedGO(TitleMenu.S.previousSelectedButton);

			PauseMessage.S.gameObject.SetActive(false);
		}

		if (playSound) {
			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}

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
					// Set cursor position
					if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null) {
						Utilities.S.PositionCursor(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject, 100);
					}

					// Audio: Selection (when a new gameObject is selected)
					Utilities.S.PlayButtonSelectedSFX(ref previousSelectedActionButton);
				}

				if (Input.GetButtonDown("SNES Y Button")) {
					Deactivate(true);
				}
				break;
			case eSaveScreenMode.pickFile:
				if (canUpdate) {
					// Set cursor position
					if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null) {
						Utilities.S.PositionCursor(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject, 100);
					}

					// Audio: Selection (when a new gameObject is selected)
					Utilities.S.PlayButtonSelectedSFX(ref previousSelectedSlotButton);
				}

				if (Input.GetButtonDown("SNES Y Button")) {
					// Audio: Deny
					AudioManager.S.PlaySFX(eSoundName.deny);

					SetUp();
				}
				break;
			case eSaveScreenMode.subMenu:
				if (Input.GetButtonDown("SNES Y Button")) {
					// Audio: Deny
					AudioManager.S.PlaySFX(eSoundName.deny);
					GameManager.S.pauseSubMenu.ResetSettings();

					// Reactivate screen cursor
					ScreenCursor.S.cursorGO[0].SetActive(true);

					SetUp();
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
	void ClickedLoadSaveOrDelete(int loadSaveOrDelete) {
		// Remove Listeners and Update GUI 
		Utilities.S.RemoveListeners(actionButtons);
		UpdateGUI();

		//Buttons Interactable
		Utilities.S.ButtonsInteractable(slotButtons, true);
		Utilities.S.ButtonsInteractable(actionButtons, false);

		// Set Selected GameObject
		Utilities.S.SetSelectedGO(slotButtons[0].gameObject);

		// Set previously selected GameObject
		previousSelectedSlotButton = slotButtons[0].gameObject;

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
		AudioManager.S.PlaySFX(eSoundName.confirm);

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
				PauseMessage.S.DisplayText("Are you sure that you would like to load this file?", false, true);

				// Set OnClick Methods
				Utilities.S.RemoveListeners(GameManager.S.pauseSubMenu.buttonCS);
				GameManager.S.pauseSubMenu.buttonCS[0].onClick.AddListener(delegate { LoadFile(fileNdx); });
				GameManager.S.pauseSubMenu.buttonCS[1].onClick.AddListener(delegate { No(0); });

				ClickedActionButtonHelper();
			}
		} else {
			// Cannot perform this action
			CannotPerformAction("This file cannot be loaded because it is empty!");
		}
	}

	void ClickedSaveButton(int fileNdx) {
		// Set Text
		PauseMessage.S.DisplayText("Are you sure that you would like to save your progress to this file?", false, true);

		// Set OnClick Methods
		Utilities.S.RemoveListeners(GameManager.S.pauseSubMenu.buttonCS);
		GameManager.S.pauseSubMenu.buttonCS[0].onClick.AddListener(delegate { SaveFile(fileNdx); });
		GameManager.S.pauseSubMenu.buttonCS[1].onClick.AddListener(delegate { No(1); });

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
				PauseMessage.S.DisplayText("Are you sure that you would like to delete this file?", false, true);

				// Set OnClick Methods
				Utilities.S.RemoveListeners(GameManager.S.pauseSubMenu.buttonCS);
				GameManager.S.pauseSubMenu.buttonCS[0].onClick.AddListener(delegate { DeleteFile(fileNdx); });
				GameManager.S.pauseSubMenu.buttonCS[1].onClick.AddListener(delegate { No(2); });

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

		// Set sub menu order in hierarchy
		GameManager.S.pauseSubMenu.gameObject.transform.SetAsLastSibling();

		// Set sub menu position
		Utilities.S.SetRectPosition(GameManager.S.pauseSubMenu.gameObject, -415, -180);

		// Set sub menu text
		GameManager.S.pauseSubMenu.SetText("Yes", "No");

		// Buttons Interactable
		Utilities.S.ButtonsInteractable(slotButtons, false);

		// Set button navigation
		Utilities.S.SetButtonNavigation(GameManager.S.pauseSubMenu.buttonCS[0], GameManager.S.pauseSubMenu.buttonCS[1], GameManager.S.pauseSubMenu.buttonCS[1]);
		Utilities.S.SetButtonNavigation(GameManager.S.pauseSubMenu.buttonCS[1], GameManager.S.pauseSubMenu.buttonCS[0], GameManager.S.pauseSubMenu.buttonCS[0]);

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

		saveScreenMode = eSaveScreenMode.cannotPeformAction;
	}
	///////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
	void LoadFile(int fileNdx) {
		// Reset stats to starting stats
		Party.S.stats.Clear();

        // Player 1
        Party.S.stats.Add(new PartyStats("Blob", 40, 40, 40, 6, 6, 6,
            2, 2, 2, 2, 1, 1, 1, 1,
            0, 1, 6,
            new List<Spell> { Spells.S.spells[1], Spells.S.spells[0], Spells.S.spells[2], Spells.S.spells[4], Spells.S.spells[5], Spells.S.spells[3] },
            new List<bool>(new bool[30]),
            new List<int> { 0, 0, 7, 23, 47, 110, 220, 450, 800, 1300, 2000 },
            false, 0, 0)
        );
        // Player 2
        Party.S.stats.Add(new PartyStats("Bill", 32, 32, 32, 15, 15, 15,
            1, 1, 1, 1, 2, 2, 2, 2,
            0, 1, 2,
            new List<Spell> { Spells.S.spells[0], Spells.S.spells[1], Spells.S.spells[3], Spells.S.spells[4], Spells.S.spells[5], Spells.S.spells[2] },
            new List<bool>(new bool[30]),
            new List<int> { 0, 0, 9, 23, 55, 110, 250, 450, 850, 1300, 2100 },
            false, 0, 1)
        );
        // Player 3
        Party.S.stats.Add(new PartyStats("Fake Bill", 25, 25, 25, 10, 10, 10,
            1, 1, 1, 1, 2, 2, 2, 2,
            0, 1, 6,
            new List<Spell> { Spells.S.spells[4], Spells.S.spells[3], Spells.S.spells[0], Spells.S.spells[2], Spells.S.spells[1], Spells.S.spells[5] },
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
		if (PlayerPrefs.HasKey(fileNdx + "Gold")) { Party.S.gold = PlayerPrefs.GetInt(fileNdx + "Gold"); }
		if (PlayerPrefs.HasKey(fileNdx + "Steps")) { Player.S.stepCount = PlayerPrefs.GetInt(fileNdx + "Steps"); }
		if (PlayerPrefs.HasKey(fileNdx + "Time")) { PauseMenu.S.fileStatsNumText.text = PlayerPrefs.GetString(fileNdx + "Time"); } // Stores Time in 0:00 format
		if (PlayerPrefs.HasKey(fileNdx + "Seconds")) { PauseMenu.S.seconds = PlayerPrefs.GetInt(fileNdx + "Seconds"); }
		if (PlayerPrefs.HasKey(fileNdx + "Minutes")) { PauseMenu.S.minutes = PlayerPrefs.GetInt(fileNdx + "Minutes"); }
		if (PlayerPrefs.HasKey(fileNdx + "Name")) { Party.S.stats[0].name = PlayerPrefs.GetString(fileNdx + "Name"); }
		if (PlayerPrefs.HasKey(fileNdx + "LocationNdx")) { WarpManager.S.locationNdx = PlayerPrefs.GetInt(fileNdx + "LocationNdx"); }
		if (PlayerPrefs.HasKey(fileNdx + "LocationName")) { WarpManager.S.locationName = PlayerPrefs.GetString(fileNdx + "LocationName"); }
		if (PlayerPrefs.HasKey(fileNdx + "VisitedLocations")) { WarpManager.S.visitedLocationNdxs = PlayerPrefs.GetString(fileNdx + "VisitedLocations"); }

		// Level Up
		Party.S.CheckForLevelUp();
		Party.S.stats[0].hasLeveledUp = false;
		Party.S.stats[1].hasLeveledUp = false;
		Party.S.stats[2].hasLeveledUp = false;

		currentFileNdx = fileNdx;

		// Separate string into individual chars
		char[] visitedLocationsNdxArray = WarpManager.S.visitedLocationNdxs.ToCharArray();

		// Add saved visited locations
		WarpManager.S.visitedLocations.Clear();
		for (int i = 0; i < visitedLocationsNdxArray.Length; i++) {
			int ndx = visitedLocationsNdxArray[i] - 48;

			WarpManager.S.visitedLocations.Add(WarpManager.S.locations[ndx]);
		}

        // Warp to this file's current location
        StartCoroutine(WarpManager.S.Warp(
            WarpManager.S.visitedLocations[PlayerPrefs.GetInt(fileNdx + "LocationNdx")].position,
            true,
            WarpManager.S.visitedLocations[PlayerPrefs.GetInt(fileNdx + "LocationNdx")].sceneName));
    }

	void SaveFile(int fileNdx) {
		// Slot 1
		PlayerPrefs.SetInt(fileNdx + "Player1Level", Party.S.stats[0].LVL);
		PlayerPrefs.SetInt(fileNdx + "Player2Level", Party.S.stats[1].LVL);
		PlayerPrefs.SetInt(fileNdx + "Player3Level", Party.S.stats[2].LVL);
		PlayerPrefs.SetInt(fileNdx + "Player1Exp", Party.S.stats[0].EXP);
		PlayerPrefs.SetInt(fileNdx + "Player2Exp", Party.S.stats[1].EXP);
		PlayerPrefs.SetInt(fileNdx + "Player3Exp", Party.S.stats[2].EXP);
		PlayerPrefs.SetInt(fileNdx + "Gold", Party.S.gold);
		PlayerPrefs.SetInt(fileNdx + "Steps", Player.S.stepCount);
		PlayerPrefs.SetString(fileNdx + "Time", PauseMenu.S.GetTime()); // Stores Time in 0:00 format
		PlayerPrefs.SetInt(fileNdx + "Seconds", PauseMenu.S.seconds);
		PlayerPrefs.SetInt(fileNdx + "Minutes", PauseMenu.S.minutes);
		PlayerPrefs.SetString(fileNdx + "Name", Party.S.stats[0].name);
		PlayerPrefs.SetInt(fileNdx + "LocationNdx", WarpManager.S.locationNdx);
		PlayerPrefs.SetString(fileNdx + "LocationName", WarpManager.S.locationName);
		PlayerPrefs.SetString(fileNdx + "VisitedLocations", WarpManager.S.visitedLocationNdxs);

		FileHelper("Saved game!");
	}

	void DeleteFile(int fileNdx) {
		// Slot 1		
		PlayerPrefs.SetInt(fileNdx + "Player1Level", 0);
		PlayerPrefs.SetInt(fileNdx + "Player2Level", 0);
		PlayerPrefs.SetInt(fileNdx + "Player3Level", 0);
		PlayerPrefs.SetInt(fileNdx + "Player1Exp", 0);
		PlayerPrefs.SetInt(fileNdx + "Player2Exp", 0);
		PlayerPrefs.SetInt(fileNdx + "Player3Exp", 0);
		PlayerPrefs.SetInt(fileNdx + "Gold", 0);
		PlayerPrefs.SetInt(fileNdx + "Steps", 0);
		PlayerPrefs.SetString(fileNdx + "Time", "0:00"); // Stores Time in 0:00 format
		PlayerPrefs.SetInt(fileNdx + "Seconds", 0);
		PlayerPrefs.SetInt(fileNdx + "Minutes", 0);
		PlayerPrefs.SetString(fileNdx + "Name", "");
		PlayerPrefs.SetInt(fileNdx + "LocationNdx", 0);
		PlayerPrefs.SetString(fileNdx + "LocationName", "");
		PlayerPrefs.SetString(fileNdx + "VisitedLocations", "");

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
		AudioManager.S.PlaySFX(eSoundName.deny);
		GameManager.S.pauseSubMenu.ResetSettings();

		// Reactivate screen cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);

		ClickedLoadSaveOrDelete(actionNdx);
	}
	///////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////
	void UpdateGUI() {
		for (int i = 0; i < slotDataText.Count; i++) {
			if (PlayerPrefs.HasKey(i + "Time")) {
				if (PlayerPrefs.GetString(i + "Time") == "0:00") {
					slotDataText[i].text = "<color=yellow>New Game</color>";
				} else {
					slotDataText[i].text =
					"<color=yellow>Name:</color> " + PlayerPrefs.GetString(i + "Name") + "    " + "<color=yellow>Level:</color> " + PlayerPrefs.GetInt(i + "Player1Level") + "\n" +
					"<color=yellow>Time:</color> " + PlayerPrefs.GetString(i + "Time") + "    " + "<color=yellow>Gold:</color> " + PlayerPrefs.GetInt(i + "Gold") + "    " +
					"<color=yellow>Location:</color> " + PlayerPrefs.GetString(i + "LocationName");
				}
			}
		}
		PauseMenu.S.UpdateGUI();
	}
}