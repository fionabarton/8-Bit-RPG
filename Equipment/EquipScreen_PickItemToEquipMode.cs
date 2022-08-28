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

	// Caches what index of the inventory is currently stored in the first item slot
	public int firstSlotNdx;

	// Prevents instantly registering input when the first or last slot is selected
	private bool verticalAxisIsInUse;
	private bool firstOrLastSlotSelected;

	public void SetUp(eItemType itemType, EquipMenu equipScreen) {
		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);

		// Buttons Interactable
		Utilities.S.ButtonsInteractable(equipScreen.inventoryButtons, true);
		Utilities.S.ButtonsInteractable(equipScreen.equippedButtons, false);

		firstOrLastSlotSelected = true;

		// No items of this type
		if (SortItems.S.tItems.Count <= 0) {
			// Switch Mode 
			equipScreen.equipScreenMode = eEquipScreenMode.noInventory;

			// Deactivate screen cursors
			Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

			if (!Player.S.isBattling) {
				PauseMessage.S.DisplayText("You don't have any items of this type to equip!");
            } else {
				Battle.S.dialogue.DisplayText("You don't have any items of this type to equip!");
			}
        } else {
			// Add Listeners
			AddListenersToInventoryButtons(equipScreen.playerNdx, equipScreen);

			// Switch mode
			equipScreen.SwitchMode(eEquipScreenMode.pickItemToEquip, equipScreen.inventoryButtons[0].gameObject, false);

			// Set the first and last button’s navigation
			SetButtonNavigation(equipScreen);

			// Initialize previously selected GameObject
			previousSelectedGameObject = equipScreen.inventoryButtons[0].gameObject;
		}
	}

	// Set the first and last button’s navigation 
	public void SetButtonNavigation(EquipMenu equipScreen) {
		// Reset all button's navigation to automatic
		Utilities.S.ResetButtonNavigation(equipScreen.inventoryButtons);

		// Set button navigation if inventory is less than 10
		if (SortItems.S.tItems.Count < equipScreen.inventoryButtons.Count) {
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
			}
		}
	}

	public void Loop(EquipMenu equipScreen) {
		// On vertical input, scroll the item list when the first or last slot is selected
		if (SortItems.S.tItems.Count > equipScreen.inventoryButtons.Count) {
			ScrollItemList(equipScreen);
		}

		if (equipScreen.canUpdate) {
			DisplayInventoryDescriptions(equipScreen.playerNdx, equipScreen);
			equipScreen.canUpdate = false;
		}

		// Go back to pickTypeToEquip
		equipScreen.GoBackToPickTypeToEquipMode("SNES Y Button", 7);
	}

	// Add listeners to inventory buttons
	public void AddListenersToInventoryButtons(int playerNdx, EquipMenu equipScreen) {
		Utilities.S.RemoveListeners(equipScreen.inventoryButtons);

		// Remove and add listeners
		for (int i = 0; i < equipScreen.inventoryButtons.Count; i++) {
			int copy = i;
			equipScreen.inventoryButtons[copy].onClick.AddListener(delegate { equipScreen.EquipItem(playerNdx, SortItems.S.tItems[firstSlotNdx + copy]); });
		}
	}

	// On vertical input, scroll the item list when the first or last slot is selected
	public void ScrollItemList(EquipMenu equipScreen) {
		if (SortItems.S.tItems.Count > 1) {
			// If first or last slot selected...
			if (firstOrLastSlotSelected) {
				if (Input.GetAxisRaw("Vertical") == 0) {
					verticalAxisIsInUse = false;
				} else {
					if (!verticalAxisIsInUse) {
						if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == equipScreen.inventoryButtons[0].gameObject) {
							if (Input.GetAxisRaw("Vertical") > 0) {
								if (firstSlotNdx == 0) {
									firstSlotNdx = SortItems.S.tItems.Count - equipScreen.inventoryButtons.Count;

									// Set  selected GameObject
									Utilities.S.SetSelectedGO(equipScreen.inventoryButtons[equipScreen.inventoryButtons.Count - 1].gameObject);
								} else {
									firstSlotNdx -= 1;
								}
							}
						} else if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == equipScreen.inventoryButtons[equipScreen.inventoryButtons.Count - 1].gameObject) {
							if (Input.GetAxisRaw("Vertical") < 0) {
								if (firstSlotNdx + equipScreen.inventoryButtons.Count == SortItems.S.tItems.Count) {
									firstSlotNdx = 0;

									// Set  selected GameObject
									Utilities.S.SetSelectedGO(equipScreen.inventoryButtons[0].gameObject);
								} else {
									firstSlotNdx += 1;
								}
							}
						}

						// Assign names to inventory slot buttons' text
						equipScreen.AssignInventorySlotNames();

						// Add listeners to inventory buttons
						AddListenersToInventoryButtons(equipScreen.playerNdx, equipScreen);

						// Audio: Selection
						AudioManager.S.PlaySFX(eSoundName.selection);

						verticalAxisIsInUse = true;

						// Allows scrolling when the vertical axis is held down in 0.2 seconds
						Invoke("VerticalAxisScrollDelay", 0.2f);
					}
				}
			}

			// Check if current selected gameObject is not the previous selected gameObject
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != previousSelectedGameObject) {
				// Check if first or last slot is selected
				if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == equipScreen.inventoryButtons[0].gameObject
				 || UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == equipScreen.inventoryButtons[equipScreen.inventoryButtons.Count - 1].gameObject) {
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

	// Display description of item to be potentially equipped
	public void DisplayInventoryDescriptions(int playerNdx, EquipMenu equipScreen) {
		if (SortItems.S.tItems != null) {
			for (int i = 0; i < equipScreen.inventoryButtons.Count; i++) {
				if (i < SortItems.S.tItems.Count) {
					if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == equipScreen.inventoryButtons[i].gameObject) {
						// Display item's description
						if (!Player.S.isBattling) {
							PauseMessage.S.SetText(SortItems.S.tItems[firstSlotNdx + i].description);
						} else {
							Battle.S.dialogue.SetText(SortItems.S.tItems[firstSlotNdx + i].description);
						}

						// Set cursor position to currently selected button
						if (!Player.S.isBattling) {
							Utilities.S.PositionCursor(equipScreen.inventoryButtons[i].gameObject, -160, -35, 0);
						} else {
							Utilities.S.PositionCursor(equipScreen.inventoryButtons[i].gameObject, -160, 40, 0);
						}

						// Set selected button text color	
						equipScreen.inventoryButtonsTxt[i].color = new Color32(205, 208, 0, 255);

						// Calculate and display potential stats
						EquipMenu.S.equipStatsEffect.DisplayPotentialStats(playerNdx, SortItems.S.tItems[firstSlotNdx + i], equipScreen.playerEquipment);

						// Audio: Selection (when a new gameObject is selected)
						Utilities.S.PlayButtonSelectedSFX(ref previousSelectedGameObject);
					} else {
						// Set non-selected button text color
						equipScreen.inventoryButtonsTxt[i].color = new Color32(255, 255, 255, 255);
					}
				}
			}
		}
	}
}