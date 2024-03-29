﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// EquipScreen Mode/Step 2: PickTypeToEquip
/// - Select which type of item to equip (weapon, armor, etc.)
/// </summary>
public class EquipScreen_PickTypeToEquipMode : MonoBehaviour {
	[Header("Set Dynamically")]
	// Ensures audio is only played once when button is selected
	public GameObject previousSelectedGameObject;

	public void SetUp(int ndx, EquipMenu equipScreen, int soundNdx = 99) {
		// Audio
		if (soundNdx != 99) {
			AudioManager.S.PlaySFX(soundNdx);
		}

		// Buttons Interactable
		Utilities.S.ButtonsInteractable(PauseMenu.S.playerNameButtons, false);
		Utilities.S.ButtonsInteractable(Battle.S.UI.partyNameButtonsCS, false);
		Utilities.S.ButtonsInteractable(equipScreen.equippedButtons, true);

		equipScreen.playerNdx = ndx;

		// Add Listeners
		AddListenersToEquippedButtons(equipScreen);

		// Switch mode
		equipScreen.SwitchMode(eEquipScreenMode.pickTypeToEquip, equipScreen.equippedButtons[0].gameObject, false);

		// Buttons Interactable
		Utilities.S.ButtonsInteractable(equipScreen.inventoryButtons, false);

		// Activate Cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);

		// Set pause menu's party sprites below skills menu
		PauseMenu.S.SwapPartyMemberGOParentAndOrderInHierarchy(false);

		if (GameManager.S.IsBattling()) {
			equipScreen.DisplayCurrentEquipmentNames(ndx);
			equipScreen.DisplayCurrentStats(ndx);
		}
	}

	public void Loop(EquipMenu equipScreen) {
		if (equipScreen.canUpdate) {
			DisplayCurrentEquipmentDescriptions(equipScreen.playerNdx, equipScreen);

			// Display items in the inventory of the currently selected equipment type
			for (int i = 0; i < equipScreen.equippedButtons.Count; i++) {
				if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == equipScreen.equippedButtons[i].gameObject) {
					// Sort Items by ItemType
					SortItems.S.SortByItemType(Inventory.S.items, equipScreen.playerEquipment[equipScreen.playerNdx][i].type);

					// Set selected button text color	
					equipScreen.equippedButtonsTxt[i].color = new Color32(205, 208, 0, 255);

					// Deactivate Unused Inventory Buttons Slots
					for (int j = 0; j < equipScreen.inventoryButtons.Count; j++) {
						if (j < SortItems.S.tItems.Count) {
							equipScreen.inventoryButtons[j].gameObject.SetActive(true);
						} else {
							equipScreen.inventoryButtons[j].gameObject.SetActive(false);
						}
					}

					// Assign names to inventory slot buttons' text
					equipScreen.AssignInventorySlotNames();

					// Audio: Selection (when a new gameObject is selected)
					Utilities.S.PlayButtonSelectedSFX(ref previousSelectedGameObject);
				} else {
					// Set non-selected button text color
					equipScreen.equippedButtonsTxt[i].color = new Color32(255, 255, 255, 255);
				}
			}

			equipScreen.canUpdate = false;
		}

		// Go back to pickPartyMember mode
		if (PauseMessage.S.dialogueFinished) { 
			if (Input.GetButtonDown("SNES Y Button")) {
				if (!GameManager.S.IsBattling()) {
					EquipMenu.S.pickPartyMemberMode.SetUp(equipScreen);

					// Reset equippedButtons text color
					Utilities.S.SetTextColor(equipScreen.equippedButtons, new Color32(255, 255, 255, 255));

					// Audio: Deny
					AudioManager.S.PlaySFX(eSoundName.deny);
				} 
			}
		}
	}

	// Add listeners to equipped buttons
	public void AddListenersToEquippedButtons(EquipMenu equipScreen) {
		// Remove and add listeners
		for (int i = 0; i < equipScreen.equippedButtons.Count; i++) {
			int tInt = i;
			equipScreen.equippedButtons[tInt].onClick.RemoveAllListeners();
			equipScreen.equippedButtons[tInt].onClick.AddListener(delegate { equipScreen.pickItemToEquipMode.SetUp((eItemType)tInt, equipScreen); });
		}
	}

	// Display descriptions of party member's current equipment
	public void DisplayCurrentEquipmentDescriptions(int playerNdx, EquipMenu equipScreen) {
		for (int i = 0; i <= equipScreen.equippedButtons.Count - 1; i++) {
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == equipScreen.equippedButtons[i].gameObject) {
				// Display item's description
				if (!GameManager.S.IsBattling()) {
					PauseMessage.S.SetText(equipScreen.playerEquipment[playerNdx][i].description);
				} else {
					Battle.S.dialogue.SetText(equipScreen.playerEquipment[playerNdx][i].description);
				}

				// Set cursor position to currently selected button
				if (!GameManager.S.IsBattling()) {
					Utilities.S.PositionCursor(equipScreen.equippedButtons[i].gameObject, -160, -35, 0);
				} else {
					Utilities.S.PositionCursor(equipScreen.equippedButtons[i].gameObject, -160, 40, 0);
				}
			}
		}
	}
}