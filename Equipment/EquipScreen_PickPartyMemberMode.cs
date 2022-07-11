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
			// Set anim
			//equipScreen.playerAnim.CrossFade("Idle", 0);

			// Switch mode
			equipScreen.SwitchMode(eEquipScreenMode.pickPartyMember, PauseMenu.S.playerNameButtons[equipScreen.playerNdx].gameObject, false);

			// Position Cursor
			Utilities.S.PositionCursor(PauseMenu.S.playerNameButtons[equipScreen.playerNdx].gameObject, 0, 60, 3);

			equipScreen.DisplayCurrentStats(equipScreen.playerNdx);

			// Buttons Interactable
			Utilities.S.ButtonsInteractable(equipScreen.equippedButtons, false);
			Utilities.S.ButtonsInteractable(PauseMenu.S.playerNameButtons, true);
			Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, false);

			// Remove & Add Listeners
			Utilities.S.RemoveListeners(PauseMenu.S.playerNameButtons);
			PauseMenu.S.playerNameButtons[0].onClick.AddListener(delegate { equipScreen.pickTypeToEquipMode.SetUp(0, equipScreen, 6); });
			PauseMenu.S.playerNameButtons[1].onClick.AddListener(delegate { equipScreen.pickTypeToEquipMode.SetUp(1, equipScreen, 6); });
			PauseMenu.S.playerNameButtons[2].onClick.AddListener(delegate { equipScreen.pickTypeToEquipMode.SetUp(2, equipScreen, 6); });

			// Set anim
			//PlayerButtons.S.anim[equipScreen.playerNdx].CrossFade("Walk", 0);

			// Display Text
			PauseMessage.S.DisplayText("Assign whose equipment?!");

			// Activate Cursor
			ScreenCursor.S.cursorGO[0].SetActive(true);
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
					// Set anims
					//if (PlayerButtons.S.buttonsCS[i].gameObject.activeInHierarchy) {
					//	PlayerButtons.S.anim[i].CrossFade("Idle", 0);
					//}

					if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == PauseMenu.S.playerNameButtons[i].gameObject) {
						// Audio: Selection (when a new gameObject is selected)
						Utilities.S.PlayButtonSelectedSFX(ref EquipMenu.S.previousSelectedGameObject);

						equipScreen.DisplayCurrentStats(i);
						equipScreen.DisplayCurrentEquipmentNames(i);

						// Set anim
						//equipScreen.playerAnim.runtimeAnimatorController = PlayerButtons.S.anim[i].runtimeAnimatorController;
						//PlayerButtons.S.anim[i].CrossFade("Walk", 0);
					}
				}
			}
		}

		// Deactivate EquipScreen
		if (Input.GetButtonDown("SNES Y Button")) {
			equipScreen.Deactivate(true);
		}
	}
}