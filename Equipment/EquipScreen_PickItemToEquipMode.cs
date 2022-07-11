using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// EquipScreen Mode/Step 3: PickItemToEquip
/// - Select which item of a certain type to equip 
/// - Ex. Weapons: Wooden Club, Steel Sword, etc.
/// </summary>
public class EquipScreen_PickItemToEquipMode : MonoBehaviour {
	[Header("Set Dynamically")]
	// Ensures audio is only played once when button is selected
	public GameObject previousSelectedGameObject;

	public void SetUp(eItemType itemType, EquipMenu equipScreen) {
		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);

		// Buttons Interactable
		Utilities.S.ButtonsInteractable(equipScreen.inventoryButtons, true);
		Utilities.S.ButtonsInteractable(equipScreen.equippedButtons, false);

		// No items of this type
		if (SortItems.S.tItems.Count <= 0) {
			// Switch Mode 
			equipScreen.equipScreenMode = eEquipScreenMode.noInventory;

			// Deactivate screen cursors
			Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

			PauseMessage.S.DisplayText("You don't have any items of this type to equip!");
		}

		// Add Listeners
		AddListenersToInventoryButtons(equipScreen.playerNdx, equipScreen);

		// Switch mode
		equipScreen.SwitchMode(eEquipScreenMode.pickItemToEquip, equipScreen.inventoryButtons[0].gameObject, false);

		// Set the first and last button’s navigation
		SetButtonNavigation(equipScreen);

		// Initialize previously selected GameObject
		previousSelectedGameObject = equipScreen.inventoryButtons[0].gameObject;
	}

	// Set the first and last button’s navigation 
	public void SetButtonNavigation(EquipMenu equipScreen) {
		// Reset all button's navigation to automatic
		for (int i = 0; i < equipScreen.inventoryButtons.Count; i++) {
			// Get the Navigation data
			Navigation navigation = equipScreen.inventoryButtons[i].navigation;

			// Switch mode to Automatic
			navigation.mode = Navigation.Mode.Automatic;

			// Reassign the struct data to the button
			equipScreen.inventoryButtons[i].navigation = navigation;
		}

		// Set button navigation if inventory is less than 10
		//if (SortItems.S.tItems.Count < equipScreen.inventoryButtons.Count) {
		if (SortItems.S.tItems.Count > 1) {
			// Set first button navigation
			Utilities.S.SetButtonNavigation(
				equipScreen.inventoryButtons[0],
				equipScreen.inventoryButtons[SortItems.S.tItems.Count - 1],
				equipScreen.inventoryButtons[1]);

			// Set last button navigation
			Utilities.S.SetButtonNavigation(
				equipScreen.inventoryButtons[SortItems.S.tItems.Count - 1],
				equipScreen.inventoryButtons[SortItems.S.tItems.Count - 2],
				equipScreen.inventoryButtons[0]);
			//}
		}
	}

	public void Loop(EquipMenu equipScreen) {
		if (equipScreen.canUpdate) {
			DisplayInventoryDescriptions(equipScreen.playerNdx, equipScreen);
			equipScreen.canUpdate = false;
		}

		// Go back to pickTypeToEquip
		equipScreen.GoBackToPickTypeToEquipMode("SNES Y Button", 7);

	}

	// Add listeners to inventory buttons
	public void AddListenersToInventoryButtons(int playerNdx, EquipMenu equipScreen) {
		// Remove and add listeners
		for (int i = 0; i < equipScreen.inventoryButtons.Count; i++) {
			int tInt = i;
			equipScreen.inventoryButtons[tInt].onClick.RemoveAllListeners();
			equipScreen.inventoryButtons[tInt].onClick.AddListener(delegate { equipScreen.EquipItem(playerNdx, SortItems.S.tItems[tInt]); });
		}
	}

	// Display description of item to be potentially equipped
	public void DisplayInventoryDescriptions(int playerNdx, EquipMenu equipScreen) {
		if (SortItems.S.tItems != null) {
			for (int i = 0; i < SortItems.S.tItems.Count; i++) {
				if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == equipScreen.inventoryButtons[i].gameObject) {
					// Display item's description
					PauseMessage.S.SetText(SortItems.S.tItems[i].description);

					// Set cursor position to currently selected button
					Utilities.S.PositionCursor(equipScreen.inventoryButtons[i].gameObject, -160, 0, 0);

					// Set selected button text color	
					equipScreen.inventoryButtonsTxt[i].color = new Color32(205, 208, 0, 255);

					// Calculate and display potential stats
					EquipMenu.S.equipStatsEffect.DisplayPotentialStats(playerNdx, SortItems.S.tItems[i], equipScreen.playerEquipment);

					// Audio: Selection (when a new gameObject is selected)
					Utilities.S.PlayButtonSelectedSFX(ref previousSelectedGameObject);
				} else {
					// Set non-selected button text color
					equipScreen.inventoryButtonsTxt[i].color = new Color32(39, 201, 255, 255);
				}
			}
		}
	}
}