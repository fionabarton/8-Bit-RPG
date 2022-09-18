using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// ItemScreen Mode/Step 1: PickItem
/// - Select an item to use
/// </summary>
public class PickItemMode : MonoBehaviour {
	[Header("Set Dynamically")]
	public int previousSelectedNdx;

	public void Setup(ItemMenu itemScreen) {
		try {
			itemScreen.mode = eItemMenuMode.pickItem;

			DeactivateUnusedItemSlots(itemScreen);
			itemScreen.AssignItemNames();
			itemScreen.AssignItemEffect();

			// Buttons Interactable
			Utilities.S.ButtonsInteractable(itemScreen.itemButtons, true);
			Utilities.S.ButtonsInteractable(PauseMenu.S.playerNameButtons, false);
			Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, false);

			itemScreen.canUpdate = true;

			// Activate Slot Headers 
			itemScreen.nameHeaderText.text = "Name:";
			itemScreen.slotHeadersHolder.SetActive(true);

			// Reset party name text colors
			Utilities.S.SetTextColor(PauseMenu.S.playerNameButtons, new Color32(255, 255, 255, 255));

			// If Inventory Empty 
			if (Inventory.S.GetItemList().Count == 0) {
				PauseMessage.S.DisplayText("You have no items, fool!");

				// Deactivate screen cursors
				Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);
			} else {
				// If previousSelectedGameObject is enabled...
				if (Items.S.menu.previousSelectedGameObject.activeInHierarchy) {
					// Select previousSelectedGameObject
					Utilities.S.SetSelectedGO(Items.S.menu.previousSelectedGameObject);

					// Set previously selected GameObject
					Battle.S.previousSelectedForAudio = Items.S.menu.previousSelectedGameObject;
				} else {
					// Select previous itemButton in the list
					Utilities.S.SetSelectedGO(itemScreen.itemButtons[previousSelectedNdx - 1].gameObject);
				}

				// Set button navigation if inventory is less than 10
				SetButtonNavigation(itemScreen);

                // Activate Cursor
                ScreenCursor.S.cursorGO[0].SetActive(true);
            }

			// Set Battle Turn Cursor sorting layer BELOW UI
			//Battle.S.UI.turnCursorSRend.sortingLayerName = "0";
		}
		catch (NullReferenceException) { }

		// Remove Listeners
		itemScreen.sortButton.onClick.RemoveAllListeners();
		// Assign Listener (Sort Button)
		itemScreen.sortButton.onClick.AddListener(delegate { Inventory.S.items = SortItems.S.SortByABC(Inventory.S.items); });
	}

	// Set the first and last button’s navigation if the player’s inventory is less than 10
	public void SetButtonNavigation(ItemMenu itemScreen) {
		// Reset all button's navigation to automatic
		Utilities.S.ResetButtonNavigation(itemScreen.itemButtons);

		// Set button navigation if inventory is less than 10
		if (Inventory.S.GetItemList().Count <= itemScreen.itemButtons.Count) {
			if (Inventory.S.GetItemList().Count > 1) {
				// Set first button navigation
				Utilities.S.SetButtonNavigation(
					itemScreen.itemButtons[0],
					itemScreen.itemButtons[Inventory.S.GetItemList().Count - 1],
					itemScreen.itemButtons[1]);

				// Set last button navigation
				Utilities.S.SetButtonNavigation(
					itemScreen.itemButtons[Inventory.S.GetItemList().Count - 1],
					itemScreen.itemButtons[Inventory.S.GetItemList().Count - 2],
					itemScreen.itemButtons[0]);
			}
		}
	}

	public void Loop(ItemMenu itemScreen) {
		if (Inventory.S.GetItemList().Count > 0) {
			if (itemScreen.canUpdate) {
				DisplayItemDescriptions(itemScreen);
				itemScreen.canUpdate = false;
			}
		}

		// Deactivate menu
		if (Input.GetButtonDown("SNES Y Button")) {
			itemScreen.Deactivate(true);
        }
	}

	public void DisplayItemDescriptions(ItemMenu itemScreen) {
		for (int i = 0; i < itemScreen.itemButtons.Count; i++) {
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == itemScreen.itemButtons[i].gameObject) {
                PauseMessage.S.SetText(Inventory.S.GetItemList()[i + itemScreen.firstSlotNdx].description);

                // Set Cursor Position set to Selected Button
                Utilities.S.PositionCursor(itemScreen.itemButtons[i].gameObject, -175, 45, 0);

                // Set selected button text color	
                itemScreen.itemButtonsNameText[i].color = new Color32(205, 208, 0, 255);
				itemScreen.itemButtonsTypeText[i].color = new Color32(205, 208, 0, 255);
				itemScreen.itemButtonsValueText[i].color = new Color32(205, 208, 0, 255);
				itemScreen.itemButtonsQTYOwnedText[i].color = new Color32(205, 208, 0, 255);
				itemScreen.itemButtonsQTYEquippedText[i].color = new Color32(205, 208, 0, 255);

				// Audio: Selection (when a new gameObject is selected)
				Utilities.S.PlayButtonSelectedSFX(ref Items.S.menu.previousSelectedGameObject);
				// Cache Selected Gameobject's index 
				previousSelectedNdx = i;
			} else {
				// Set non-selected button text color
				itemScreen.itemButtonsNameText[i].color = new Color32(255, 255, 255, 255);
				itemScreen.itemButtonsTypeText[i].color = new Color32(255, 255, 255, 255);
				itemScreen.itemButtonsValueText[i].color = new Color32(255, 255, 255, 255);
				itemScreen.itemButtonsQTYOwnedText[i].color = new Color32(255, 255, 255, 255);
				itemScreen.itemButtonsQTYEquippedText[i].color = new Color32(255, 255, 255, 255);
			}
		}
	}

	void DeactivateUnusedItemSlots(ItemMenu itemScreen) {
		for (int i = 0; i < itemScreen.itemButtons.Count; i++) {
			if (i < Inventory.S.GetItemList().Count) {
				itemScreen.itemButtons[i].gameObject.SetActive(true);
				itemScreen.itemButtonsTypeText[i].gameObject.SetActive(true);
				itemScreen.itemButtonsValueText[i].gameObject.SetActive(true);
				itemScreen.itemButtonsQTYOwnedText[i].gameObject.SetActive(true);
				itemScreen.itemButtonsQTYEquippedText[i].gameObject.SetActive(true);
			} else {
				itemScreen.itemButtons[i].gameObject.SetActive(false);
				itemScreen.itemButtonsTypeText[i].gameObject.SetActive(false);
				itemScreen.itemButtonsValueText[i].gameObject.SetActive(false);
				itemScreen.itemButtonsQTYOwnedText[i].gameObject.SetActive(false);
				itemScreen.itemButtonsQTYEquippedText[i].gameObject.SetActive(false);
			}
		}
	}
}