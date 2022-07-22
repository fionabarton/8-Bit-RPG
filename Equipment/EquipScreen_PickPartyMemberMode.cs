using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// EquipScreen Mode/Step 1: PickPartyMember
/// - Select which party member to equip
/// </summary>
public class EquipScreen_PickPartyMemberMode : MonoBehaviour {
	public void SetUp(EquipMenu equipScreen) {
		try {
			// Switch mode
			equipScreen.SwitchMode(eEquipScreenMode.pickPartyMember, PauseMenu.S.playerNameButtons[equipScreen.playerNdx].gameObject, false);

			equipScreen.DisplayCurrentStats(equipScreen.playerNdx);

			// Buttons Interactable
			Utilities.S.ButtonsInteractable(equipScreen.equippedButtons, false);
			Utilities.S.ButtonsInteractable(equipScreen.inventoryButtons, false);
			Utilities.S.ButtonsInteractable(PauseMenu.S.playerNameButtons, true);
			Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, false);

			// Remove & Add Listeners
			Utilities.S.RemoveListeners(PauseMenu.S.playerNameButtons);
			PauseMenu.S.playerNameButtons[0].onClick.AddListener(delegate { equipScreen.pickTypeToEquipMode.SetUp(0, equipScreen, 6); });
			PauseMenu.S.playerNameButtons[1].onClick.AddListener(delegate { equipScreen.pickTypeToEquipMode.SetUp(1, equipScreen, 6); });
			PauseMenu.S.playerNameButtons[2].onClick.AddListener(delegate { equipScreen.pickTypeToEquipMode.SetUp(2, equipScreen, 6); });

			// Position Cursor
			Utilities.S.PositionCursor(PauseMenu.S.playerNameButtons[equipScreen.playerNdx].gameObject, 0, 60, 3);

			// Display Text
			PauseMessage.S.DisplayText("Assign whose equipment?!");
		}
		catch (NullReferenceException) { }
	}

	public void Loop(EquipMenu equipScreen) {
		if (equipScreen.canUpdate) {
			if (EquipMenu.S.previousSelectedGameObject != UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject) {
				// Position Cursor
				Utilities.S.PositionCursor(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject, 0, 60, 3);

				// Display currently selected Member's Stats/Equipment 
				for (int i = 0; i < PauseMenu.S.playerNameButtons.Count; i++) {
					if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == PauseMenu.S.playerNameButtons[i].gameObject) {
						// Audio: Selection (when a new gameObject is selected)
						Utilities.S.PlayButtonSelectedSFX(ref EquipMenu.S.previousSelectedGameObject);

						equipScreen.DisplayCurrentStats(i);
						equipScreen.DisplayCurrentEquipmentNames(i);
					}
				}
			}

			// Set selected member animation to walk
			PauseMenu.S.SetSelectedMemberAnim("Walk");
		}

		// Deactivate EquipScreen
		if (Input.GetButtonDown("SNES Y Button")) {
			equipScreen.Deactivate(true);
		}
	}
}