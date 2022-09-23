using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// SpellScreen Mode/Step 1: PickWhichSpellsToDisplay
/// - Select which party member's spells to use
/// </summary>
public class PickWhichSpellsToDisplay : MonoBehaviour {
	[Header("Set in Inspector")]
	public Text titleText;

	public void Setup(SpellMenu spellScreen) {
		spellScreen.firstSlotNdx = 0;

		try {
			if (GameManager.S.currentScene != "Battle") {
				// Buttons Interactable
				Utilities.S.ButtonsInteractable(PauseMenu.S.playerNameButtons, true);
				Utilities.S.ButtonsInteractable(spellScreen.spellsButtons, false);
				Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, false);

				spellScreen.canUpdate = true;

				// Remove Listeners
				Utilities.S.RemoveListeners(PauseMenu.S.playerNameButtons);
				// Add Listeners
				PauseMenu.S.playerNameButtons[0].onClick.AddListener(delegate { spellScreen.LoadSpells(0, true); });
				PauseMenu.S.playerNameButtons[1].onClick.AddListener(delegate { spellScreen.LoadSpells(1, true); });
				PauseMenu.S.playerNameButtons[2].onClick.AddListener(delegate { spellScreen.LoadSpells(2, true); });

				// Set Slot Headers Text 
				spellScreen.nameHeaderText.text = "Name:";

				// Set Selected gameObject
				Utilities.S.SetSelectedGO(spellScreen.previousSelectedPlayerGO);

				// Reset which spell to select
				spellScreen.previousSelectedSpellGO = spellScreen.spellsButtons[0].gameObject;

				// Display Text
				PauseMessage.S.DisplayText("Use whose skills?!");

				// Switch ScreenMode 
				spellScreen.mode = eSpellScreenMode.pickWhichSpellsToDisplay;

				// Activate Cursor
				ScreenCursor.S.cursorGO[0].SetActive(true);

				// Set pause menu's party sprites above skills menu
				PauseMenu.S.SwapPartyMemberGOParentAndOrderInHierarchy();
			} else {
				// Set Turn Cursor sorting layer BELOW UI
				//Battle.S.UI.turnCursorSRend.sortingLayerName = "0";
			}
		}
		catch (Exception e) {
			Debug.Log(e);
		}
	}

	public void Loop(SpellMenu spellScreen) {
		if (spellScreen.canUpdate) {
			// Display each Member's Spells
			for (int i = 0; i < PauseMenu.S.playerNameButtons.Count; i++) {
				if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == PauseMenu.S.playerNameButtons[i].gameObject) {
					spellScreen.DeactivateUnusedSpellsSlots(i);
					spellScreen.DisplaySpellsDescriptions(i);
					spellScreen.AssignSpellsNames(i);
					Utilities.S.PositionCursor(PauseMenu.S.playerNameButtons[i].gameObject, 0, 110, 3);
					titleText.text = "Skills: " + "<color=white>" + Party.S.stats[i].name + "</color>";

					// Audio: Selection (when a new gameObject is selected)
					Utilities.S.PlayButtonSelectedSFX(ref spellScreen.previousSelectedPlayerGO);
				}
			}

			// Set selected member animation to walk
			PauseMenu.S.SetSelectedMemberAnim("Walk", true);

			spellScreen.canUpdate = false;
		}
	}
}