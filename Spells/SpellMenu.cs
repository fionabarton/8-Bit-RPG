using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpellMenu : MonoBehaviour {
	// Overworld & Battle Spells: Antidote, Revive (50 & 100% success rate), Full Heal, Full Party Heal
	// Overworld Spells: Evac, Warp
	// Battle ONLY Spells: Buff (Strength, Agility), AOE (Attack, Heal), Status (Sleep, Poison, Confuse, Blind)

	[Header("Set in Inspector")]
	// Spells "Buttons"
	public List<Button> spellsButtons;
	public List<Text> spellsButtonNameText;
	public List<Text> spellsButtonMPCostText;
	public List<Text> spellsButtonTypeText;

	public Text nameHeaderText;
	public GameObject slotHeadersHolder;

	public GameObject previousSelectedSpellGO;

	[Header("Set Dynamically")]
	public int playerNdx = 0; // Used to set Player 1 or 2 for DisplaySpellsDescriptions(), set in LoadSpells()

	// For Input & Display MessageRemoveAllListeners
	public eSpellScreenMode mode;

	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

	public GameObject previousSelectedPlayerGO;

	// Caches what index of the inventory is currently stored in the first item slot
	public int firstSlotNdx;

	// Prevents instantly registering input when the first or last slot is selected
	private bool verticalAxisIsInUse;
	private bool firstOrLastSlotSelected;
	
	public int previousSelectedNdx;

	public PickWhichSpellsToDisplay pickWhichSpellsToDisplay;
	public PickSpell pickSpell;
	public DoesntKnowSpells doesntKnowSpells;
	public PickWhichMemberToHeal pickWhichMemberToHeal;
	public UsedSpell usedSpell;
	public CantUseSpell cantUseSpell;

	void Awake() {
		// Get components	
		pickWhichSpellsToDisplay = GetComponent<PickWhichSpellsToDisplay>();
		pickSpell = GetComponent<PickSpell>();
		doesntKnowSpells = GetComponent<DoesntKnowSpells>();
		pickWhichMemberToHeal = GetComponent<PickWhichMemberToHeal>();
		usedSpell = GetComponent<UsedSpell>();
		cantUseSpell = GetComponent<CantUseSpell>();
	}

	void Start() {
		gameObject.SetActive(false);
	}

	public void Activate() {
		// Ensures first slots are selected when screen enabled
		previousSelectedPlayerGO = PauseMenu.S.playerNameButtons[0].gameObject;
		previousSelectedSpellGO = spellsButtons[0].gameObject;

		firstSlotNdx = 0;
		firstOrLastSlotSelected = true;

		pickWhichSpellsToDisplay.Setup(Spells.S.menu);

		// Add Loop() to Update Delgate
		UpdateManager.updateDelegate += Loop;

		// Activate MP Cost header
		slotHeadersHolder.SetActive(true);

		// Set party member button navigation
		Utilities.S.SetHorizontalButtonsNavigation(PauseMenu.S.playerNameButtons, Party.S.partyNdx + 1);

		gameObject.SetActive(true);

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);
	}

	public void Deactivate(bool playSound = false) {
		// Set Battle Turn Cursor sorting layer ABOVE UI
		//Battle.S.UI.turnCursorSRend.sortingLayerName = "Above UI";

		// Remove Listeners
		Utilities.S.RemoveListeners(spellsButtons);

		if (GameManager.S.currentScene != "Battle") {
			// Buttons Interactable
			Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, true);
			Utilities.S.ButtonsInteractable(PauseMenu.S.playerNameButtons, false);

			// Set Selected Gameobject (Pause Screen: Spells Button)
			Utilities.S.SetSelectedGO(PauseMenu.S.buttonGO[1]);

			PauseMessage.S.DisplayText("Welcome to the Pause Screen!");

			// Set pause menu's party sprites below skills menu
			PauseMenu.S.SwapPartyMemberGOParentAndOrderInHierarchy(false);

			PauseMenu.S.canUpdate = true;

			// Activate cursor 
			if (!ScreenCursor.S.cursorGO[0].activeInHierarchy) {
				ScreenCursor.S.cursorGO[0].SetActive(true);
			}
		} else {
			//// If Player didn't use a Spell, go back to Player Turn
			//if (mode != eSpellScreenMode.pickSpell) {
			//	if (Battle.S.mode == eBattleMode.spellMenu) {
			//		Battle.S.PlayerTurn(false, false);
			//	}
			//}

			//// Deactivate screen cursors
			//Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);
		}

		if (playSound) {
			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}

		// Set party animations to idle
		PauseMenu.S.SetSelectedMemberAnim("Idle", true);

		// Remove Loop() from Update Delgate
		UpdateManager.updateDelegate -= Loop;

		// Deactivate this gameObject
		gameObject.SetActive(false);
	}

	public void Loop() {
		// Reset canUpdate
		if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f) {
			canUpdate = true;
		}

		// Deactivate SpellScreen
		if (GameManager.S.currentScene != "Battle") {
			if (mode == eSpellScreenMode.pickWhichSpellsToDisplay) {
				if (Input.GetButtonDown("SNES Y Button")) {
					Deactivate(true);
				}
			}
		} else {
			if (Input.GetButtonDown("SNES Y Button")) {
				// Audio: Deny
				AudioManager.S.PlaySFX(eSoundName.deny);
				ScreenOffPlayerTurn();
			}
		}

		switch (mode) {
			case eSpellScreenMode.pickWhichSpellsToDisplay:
				pickWhichSpellsToDisplay.Loop(Spells.S.menu);
				break;
			case eSpellScreenMode.pickSpell:
				// On vertical input, scroll the item list when the first or last slot is selected
				if (Party.S.stats[playerNdx].spellNdx > spellsButtons.Count) {
					ScrollSpellList();
				}

				pickSpell.Loop(Spells.S.menu);
				break;
			case eSpellScreenMode.doesntKnowSpells:
				doesntKnowSpells.Loop(Spells.S.menu);
				break;
			case eSpellScreenMode.pickWhichMemberToHeal:
				pickWhichMemberToHeal.Loop(Spells.S.menu);
				break;
			case eSpellScreenMode.pickAllMembersToHeal:
				if (Input.GetButtonDown("SNES Y Button")) {
					GoBackToPickSpellMode();
				}
				break;
			case eSpellScreenMode.pickWhereToWarp:
				if (canUpdate) {
					WarpManager.S.DisplayButtonDescriptions(spellsButtons, -160);
				}

				if (Input.GetButtonDown("SNES Y Button")) {
					GoBackToPickSpellMode();

					// Activate MP Cost header
					slotHeadersHolder.SetActive(true);
					// Reset Slot Headers Text 
					nameHeaderText.text = "Name:";
				}
				break;
			case eSpellScreenMode.usedSpell:
				usedSpell.Loop(Spells.S.menu);
				break;
			// During Battle... "Not Enough MP Message", then back to Player Turn w/ Button Press
			case eSpellScreenMode.cantUseSpell:
				cantUseSpell.Loop(Spells.S.menu);
				break;
		}
	}

	// On vertical input, scroll the spell list when the first or last slot is selected
	void ScrollSpellList() {
		if (Party.S.stats[playerNdx].spellNdx > 0) {
			// If first or last slot selected...
			if (firstOrLastSlotSelected) {
				if (Input.GetAxisRaw("Vertical") == 0) {
					verticalAxisIsInUse = false;
				} else {
					if (!verticalAxisIsInUse) {
						if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == spellsButtons[0].gameObject) {
							if (Input.GetAxisRaw("Vertical") > 0) {
								if (firstSlotNdx == 0) {
									firstSlotNdx = Party.S.stats[playerNdx].spellNdx - spellsButtons.Count;

									// Set selected GameObject
									Utilities.S.SetSelectedGO(spellsButtons[spellsButtons.Count - 1].gameObject);
								} else {
									firstSlotNdx -= 1;
								}
							}
						} else if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == spellsButtons[spellsButtons.Count - 1].gameObject) {
							if (Input.GetAxisRaw("Vertical") < 0) {
								if (firstSlotNdx + spellsButtons.Count == Party.S.stats[playerNdx].spellNdx) {
									firstSlotNdx = 0;

									// Set selected GameObject
									Utilities.S.SetSelectedGO(spellsButtons[0].gameObject);
								} else {
									firstSlotNdx += 1;
								}
							}
						}

						AssignSpellsEffect(playerNdx);
						AssignSpellsNames(playerNdx);

						// Audio: Selection
						AudioManager.S.PlaySFX(eSoundName.selection);

						verticalAxisIsInUse = true;

						// Allows scrolling when the vertical axis is held down in 0.2 seconds
						Invoke("VerticalAxisScrollDelay", 0.2f);
					}
				}
			}

			// Check if current selected gameObject is not the previous selected gameObject
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != previousSelectedSpellGO) {
				// Check if first or last slot is selected
				if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == spellsButtons[0].gameObject
				 || UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == spellsButtons[spellsButtons.Count - 1].gameObject) {
					firstOrLastSlotSelected = true;
					verticalAxisIsInUse = true;

					// Allows scrolling when the vertical axis is held down in 0.2 seconds
					Invoke("VerticalAxisScrollDelay", 0.2f);
				} else {
					firstOrLastSlotSelected = false;
				}

			}
		}
	}

	// Allows scrolling when the vertical axis is held down 
	void VerticalAxisScrollDelay() {
		verticalAxisIsInUse = false;
	}

	void GoBackToPickSpellMode() {
		if (PauseMessage.S.dialogueFinished) {
			// Deactivate screen cursors
			Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);

			LoadSpells(playerNdx); // Go Back

			// Play the previously played animation clip of the selected party member
			PauseMenu.S.SetPreviousSelectedPlayerAnimAndColor("Walk", playerNdx);
		}
	}

	public void LoadSpells(int playerNdx, bool playSound = false) {
		// Buttons Interactable
		Utilities.S.ButtonsInteractable(PauseMenu.S.playerNameButtons, false);
		Utilities.S.ButtonsInteractable(spellsButtons, true);

		this.playerNdx = playerNdx; // Now used to DisplaySpellsDescriptions

		//if (GameManager.S.currentScene == "Battle") {
		//	// Activate Spell Screen
		//	Activate();
		//}

		// Empty Inventory
		if (Party.S.stats[playerNdx].spellNdx == 0) {
			PauseMessage.S.DisplayText(Party.S.stats[playerNdx].name + " knows no skills, fool!");

			canUpdate = true;
			// Switch ScreenMode
			mode = eSpellScreenMode.doesntKnowSpells;

			// Deactivate screen cursors
			Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		} else {
			canUpdate = true;

			// Set pause menu's party sprites below skills menu
			PauseMenu.S.SwapPartyMemberGOParentAndOrderInHierarchy(false);

			// Switch ScreenMode 
			mode = eSpellScreenMode.pickSpell;
			
			// Set Selected GameObject 
			// If previousSelectedGameObject is enabled...
			if (previousSelectedSpellGO.activeInHierarchy && !GameManager.S.IsBattling()) {
				// Select previousSelectedGameObject
				Utilities.S.SetSelectedGO(previousSelectedSpellGO);

				// Set previously selected GameObject
				Battle.S.previousSelectedForAudio = previousSelectedSpellGO;
			} else {
				//// Set previously selected GameObject
				//previousSelectedSpellGO = spellsButtons[0].gameObject;

				//// Select first spell in the list
				//Utilities.S.SetSelectedGO(spellsButtons[0].gameObject);

				// Select previous itemButton in the list
				Utilities.S.SetSelectedGO(spellsButtons[previousSelectedNdx].gameObject);
			}

			// Activate Cursor
			ScreenCursor.S.cursorGO[0].SetActive(true);

			if (playSound) {
				// Audio: Confirm
				AudioManager.S.PlaySFX(eSoundName.confirm);
			}
		}

		DeactivateUnusedSpellsSlots(playerNdx);
		AssignSpellsEffect(playerNdx);
		DisplaySpellsDescriptions(playerNdx);

		// Set the first and last button’s navigation 
		SetButtonNavigation();
	}

	public void DisplaySpellsDescriptions(int playerNdx) {
		for (int i = 0; i < spellsButtons.Count; i++) {
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == spellsButtons[i].gameObject) {
				PauseMessage.S.SetText(Party.S.stats[playerNdx].spells[i + firstSlotNdx].description);

				// Cursor Position set to Selected Button
				Utilities.S.PositionCursor(spellsButtons[i].gameObject, -175, 45, 0);

				// Set selected button text color	
				spellsButtonNameText[i].color = new Color32(205, 208, 0, 255);
				spellsButtonTypeText[i].color = new Color32(205, 208, 0, 255);
				spellsButtonMPCostText[i].color = new Color32(205, 208, 0, 255);

				// Audio: Selection (when a new gameObject is selected)
				Utilities.S.PlayButtonSelectedSFX(ref previousSelectedSpellGO);
				// Cache Selected Gameobject's index 
				previousSelectedNdx = i;
			} else {
				// Set non-selected button text color
				spellsButtonNameText[i].color = new Color32(255, 255, 255, 255);
				spellsButtonTypeText[i].color = new Color32(255, 255, 255, 255);
				spellsButtonMPCostText[i].color = new Color32(255, 255, 255, 255);
			}
		}
	}

	public void DeactivateUnusedSpellsSlots(int playerNdx) {
		for (int i = 0; i < spellsButtons.Count; i++) {
			if (i < Party.S.stats[playerNdx].spellNdx) {
				spellsButtons[i].gameObject.SetActive(true);
				spellsButtonTypeText[i].gameObject.SetActive(true);
				spellsButtonMPCostText[i].gameObject.SetActive(true);
			} else {
				spellsButtons[i].gameObject.SetActive(false);
				spellsButtonTypeText[i].gameObject.SetActive(false);
				spellsButtonMPCostText[i].gameObject.SetActive(false);
			}
		}
	}

	public void AssignSpellsEffect(int playerNdx) {
		Utilities.S.RemoveListeners(PauseMenu.S.playerNameButtons);
		Utilities.S.RemoveListeners(spellsButtons);

		for (int i = 0; i < spellsButtons.Count; i++) {
			// Add listener to Spell Button
			int copy = i;
			spellsButtons[copy].onClick.AddListener(delegate { UseSpell(Party.S.stats[playerNdx].spells[firstSlotNdx + copy]); });

			// Assign Button Name Text
			AssignSpellsNames(playerNdx);
		}
	}

	public void AssignSpellsNames(int playerNdx) {
		for (int i = 0; i < spellsButtons.Count; i++) {
			if (firstSlotNdx + i < Party.S.stats[playerNdx].spellNdx) {
				// Assign Button Name Text
				string ndx = (firstSlotNdx + i + 1).ToString();
				spellsButtonNameText[i].text = ndx + ") " + Party.S.stats[playerNdx].spells[firstSlotNdx + i].name;
				spellsButtonTypeText[i].text = Party.S.stats[playerNdx].spells[firstSlotNdx + i].type.ToString();
				spellsButtonMPCostText[i].text = Party.S.stats[playerNdx].spells[firstSlotNdx + i].cost.ToString();
			}
		}
	}

	// Set the first and last button’s navigation 
	public void SetButtonNavigation() {
		// Reset all button's navigation to automatic
		Utilities.S.ResetButtonNavigation(spellsButtons);

		// Set button navigation if inventory is less than 10
		if (Party.S.stats[playerNdx].spellNdx <= spellsButtons.Count) {
			if (Party.S.stats[playerNdx].spellNdx > 1) {
				// Set first button navigation
				Utilities.S.SetButtonNavigation(
					spellsButtons[0],
					spellsButtons[Party.S.stats[playerNdx].spellNdx - 1],
					spellsButtons[1]);

				// Set last button navigation
				Utilities.S.SetButtonNavigation(
					spellsButtons[Party.S.stats[playerNdx].spellNdx - 1],
					spellsButtons[Party.S.stats[playerNdx].spellNdx - 2],
					spellsButtons[0]);
			}
		}
	}

	public void ScreenOffPlayerTurn() {
		PauseMessage.S.gameObject.SetActive(false);

		Deactivate(true);

		if (Battle.S.mode == eBattleMode.spellMenu) {
			Battle.S.PlayerTurn(false, false);
		}
	}

	// Inspired by ConsumeItem() in ItemScreen.cs
	public void UseSpell(Spell spell) {
		canUpdate = true;

		if (GameManager.S.IsBattling()) { // if Battle
			if (spell.name == "Heal") {
				Spells.S.battle.AddFunctionToButton(Spells.S.battle.AttemptHealSinglePartyMember, "Heal which party member?", spell);
			} else if (spell.name == "Fireball") {
				Spells.S.battle.AddFunctionToButton(Spells.S.battle.AttemptAttackSelectedEnemy, "Attack which enemy?", spell);
			} else if (spell.name == "Fireblast") {
				Spells.S.battle.AddFunctionToButton(Spells.S.battle.AttemptAttackAllEnemies, "Attack all enemies?", spell);
			} else if (spell.name == "Heal All") {
				Spells.S.battle.AddFunctionToButton(Spells.S.battle.AttemptHealAll, "Heal all party members?", spell);
			} else if (spell.name == "Revive") {
				Spells.S.battle.AddFunctionToButton(Spells.S.battle.AttemptReviveSelectedPartyMember, "Revive which party member?", spell);
			} else if (spell.name == "Detoxify") {
				Spells.S.battle.AddFunctionToButton(Spells.S.battle.AttemptDetoxifySinglePartyMember, "Detoxify which poisoned party member?", spell);
			} else if (spell.name == "Mobilize") {
				Spells.S.battle.AddFunctionToButton(Spells.S.battle.AttemptMobilizeSinglePartyMember, "Restore the mobility of which paralyzed party member?", spell);
			} else if (spell.name == "Wake") {
				Spells.S.battle.AddFunctionToButton(Spells.S.battle.AttemptWakeSinglePartyMember, "Wake up which sleeping party member?", spell);
			} else if (spell.name == "Poison") {
				Spells.S.battle.AddFunctionToButton(Spells.S.battle.AttemptPoisonSinglePartyMember, "Indefinitely poison which enemy?", spell);
			} else if (spell.name == "Paralyze") {
				Spells.S.battle.AddFunctionToButton(Spells.S.battle.AttemptParalyzeSinglePartyMember, "Temporarily paralyze which enemy?", spell);
			} else if (spell.name == "Sleep") {
				Spells.S.battle.AddFunctionToButton(Spells.S.battle.AttemptSleepSinglePartyMember, "Temporarily put which enemy to sleep?", spell);
			} else if (spell.name == "Steal") {
				Spells.S.battle.AddFunctionToButton(Spells.S.battle.AttemptStealSinglePartyMember, "Attempt to steal an item from which enemy?", spell);
			} else {
				Spells.S.CantUseSpell("Can't use this skill during battle!");
			}

			// Deactivate skill description message
			Battle.S.UI.descriptionMessageGO.SetActive(false);
		}
		else { // if Overworld
			if (spell.name == "Heal") {
				Spells.S.world.AddFunctionToButton(Spells.S.world.HealSelectedPartyMember, "Heal which party member?", spell);
			} else if (spell.name == "Warp") {
				Spells.S.world.WarpSpell();
			} else if (spell.name == "Heal All") {
				Spells.S.world.AddFunctionToButton(Spells.S.world.HealAllPartyMembers, "Heal all party members?", spell);
			} else if (spell.name == "Detoxify") {
				Spells.S.world.AddFunctionToButton(Spells.S.world.DetoxifySelectedPartyMember, "Detoxify which poisoned party member?", spell);
			} else {
				Spells.S.CantUseSpell("You ain't battlin' no one, so ya can't use this skill!");
			}
		}
	}
}