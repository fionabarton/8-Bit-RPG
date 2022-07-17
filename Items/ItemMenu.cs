using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	// Item "Buttons"
	public List<Button> itemButtons;
	public List<Text> itemButtonsNameText;
	public List<Text> itemButtonsTypeText;
	public List<Text> itemButtonsValueText;
	public List<Text> itemButtonsQTYOwnedText;
	public List<Text> itemButtonsQTYEquippedText;

	public Text nameHeaderText;
	public GameObject slotHeadersHolder;

	public Button sortButton;

	[Header("Set Dynamically")]
	// For Input & Display Message
	public eItemMenuMode mode;

	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

	public GameObject previousSelectedGameObject;

	// Caches what index of the inventory is currently stored in the first item slot
	public int firstSlotNdx;

	// Prevents instantly registering input when the first or last slot is selected
	private bool verticalAxisIsInUse;
	private bool firstOrLastSlotSelected;

	public PickItemMode pickItemMode;
	public PickPartyMemberMode pickPartyMemberMode;
	public UsedItemMode usedItemMode;

	void Awake() {
		// Get components
		pickItemMode = GetComponent<PickItemMode>();
		pickPartyMemberMode = GetComponent<PickPartyMemberMode>();
		usedItemMode = GetComponent<UsedItemMode>();
	}

	void Start() {
		gameObject.SetActive(false);
	}

	public void Activate() {
		// Ensures first slot is selected when screen enabled
		previousSelectedGameObject = itemButtons[0].gameObject;

		firstSlotNdx = 0;
		firstOrLastSlotSelected = true;

		gameObject.SetActive(true);

		// Add Loop() to Update Delgate
		UpdateManager.updateDelegate += Loop;

		pickItemMode.Setup(Items.S.menu);

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);
	}

	public void Deactivate(bool playSound = false) {
        // Deactivate Cursors if in Battle Mode
        if (!GameManager.S.paused) {
            Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);
        }

        // Set Battle Turn Cursor sorting layer ABOVE UI
        //Battle.S.UI.turnCursorSRend.sortingLayerName = "Above UI";

        // Remove Listeners
        Utilities.S.RemoveListeners(itemButtons);

		// Buttons Interactable
		Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, true);

		// Set Selected Gameobject (Pause Screen: Items Button)
		Utilities.S.SetSelectedGO(PauseMenu.S.buttonGO[0]);

        PauseMessage.S.DisplayText("Welcome to the Pause Screen!");

        PauseMenu.S.canUpdate = true;

		if (playSound) {
			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}

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

		switch (mode) {
			case eItemMenuMode.pickItem:
				// On vertical input, scroll the item list when the first or last slot is selected
				if (Inventory.S.GetItemList().Count > itemButtons.Count) {
					ScrollItemList();
				}

				pickItemMode.Loop(Items.S.menu);
				break;
			case eItemMenuMode.pickPartyMember:
				pickPartyMemberMode.Loop(Items.S.menu);
				break;
			case eItemMenuMode.pickAllPartyMembers:
				if (Input.GetButtonDown("SNES Y Button")) {
					GoBackToPickItemMode();
				}
				break;
			case eItemMenuMode.pickWhereToWarp:
				if (canUpdate) {
					WarpManager.S.DisplayButtonDescriptions(itemButtons, -170);
				}

				if (Input.GetButtonDown("SNES Y Button")) {
					GoBackToPickItemMode();
				}
				break;
			case eItemMenuMode.usedItem:
				usedItemMode.Loop(Items.S.menu);
				break;
		}

		// TEST: Sort Inventory
		if (Input.GetKeyDown(KeyCode.X)) {
			//_items = SortItems.S.SortByABC(_items);
			Inventory.S.items = SortItems.S.SortByValue(Inventory.S.items);
		}
	}

	// On vertical input, scroll the item list when the first or last slot is selected
	void ScrollItemList() {
		if (Inventory.S.GetItemList().Count > 1) {
			// If first or last slot selected...
			if (firstOrLastSlotSelected) {
				if (Input.GetAxisRaw("Vertical") == 0) {
					verticalAxisIsInUse = false;
				} else {
					if (!verticalAxisIsInUse) {
						if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == itemButtons[0].gameObject) {
							if (Input.GetAxisRaw("Vertical") > 0) {
								if (firstSlotNdx == 0) {
									firstSlotNdx = Inventory.S.GetItemList().Count - itemButtons.Count;

									// Set  selected GameObject
									Utilities.S.SetSelectedGO(itemButtons[itemButtons.Count - 1].gameObject);
								} else {
									firstSlotNdx -= 1;
								}
							}
						} else if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == itemButtons[itemButtons.Count - 1].gameObject) {
							if (Input.GetAxisRaw("Vertical") < 0) {
								if (firstSlotNdx + itemButtons.Count == Inventory.S.GetItemList().Count) {
									firstSlotNdx = 0;

									// Set  selected GameObject
									Utilities.S.SetSelectedGO(itemButtons[0].gameObject);
								} else {
									firstSlotNdx += 1;
								}
							}
						}

						AssignItemEffect();
						AssignItemNames();

						// Audio: Selection
						AudioManager.S.PlaySFX(eSoundName.selection);

						verticalAxisIsInUse = true;

						// Allows scrolling when the vertical axis is held down in 0.2 seconds
						Invoke("VerticalAxisScrollDelay", 0.2f);
					}
				}
			}

			// Check if first or last slot is selected
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == itemButtons[0].gameObject
			 || UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == itemButtons[itemButtons.Count-1].gameObject) {
				firstOrLastSlotSelected = true;
			} else {
				firstOrLastSlotSelected = false;
			}
		}
	}

	// Allows scrolling when the vertical axis is held down 
	void VerticalAxisScrollDelay() {
		verticalAxisIsInUse = false;
	}

	void GoBackToPickItemMode() {
		if (PauseMessage.S.dialogueFinished) {
			// Set party animations to idle
			PauseMenu.S.SetSelectedMemberAnim("Idle");

			// Reset button colors
			Utilities.S.SetTextColor(PauseMenu.S.playerNameButtons, new Color32(255, 255, 255, 200));

			// Deactivate screen cursors
			Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);

			// Go back to PickItem mode
			pickItemMode.Setup(Items.S.menu);
		}
	}

	public void AssignItemEffect() {
		Utilities.S.RemoveListeners(itemButtons);

		for (int i = 0; i < itemButtons.Count; i++) {
			int copy = i;
			itemButtons[copy].onClick.AddListener(delegate { ConsumeItem(Inventory.S.GetItemList()[firstSlotNdx + copy]); });
		}
	}

	public void AssignItemNames() {
		for (int i = 0; i < itemButtons.Count; i++) {
			if (firstSlotNdx + i < Inventory.S.GetItemList().Count) {
				string inventoryNdx = (firstSlotNdx + i + 1).ToString();

				itemButtonsNameText[i].text = inventoryNdx + ") " + Inventory.S.GetItemList()[firstSlotNdx + i].name;
				itemButtonsTypeText[i].text = Inventory.S.GetItemList()[firstSlotNdx + i].type.ToString();
				itemButtonsValueText[i].text = Inventory.S.GetItemList()[firstSlotNdx + i].value.ToString();
				itemButtonsQTYOwnedText[i].text = Inventory.S.GetItemCount(Inventory.S.GetItemList()[firstSlotNdx + i]).ToString();
				itemButtonsQTYEquippedText[i].text = EquipMenu.S.GetEquippedItemCount(Inventory.S.GetItemList()[firstSlotNdx + i]).ToString();
			}
		}
	}

	public void ConsumeItem(Item item) {
		// Check if the item is in inventory
		if (Inventory.S.items.ContainsKey(item)) {
			canUpdate = true;

			if (Blob.S.isBattling) { // if Battle
				if (item.name == "Health Potion") {
					Items.S.battle.AddFunctionToButton(Items.S.battle.HPPotion, "Use potion on which party member?", item);
				} else if (item.name == "Magic Potion") {
					Items.S.battle.AddFunctionToButton(Items.S.battle.MPPotion, "Use potion on which party member?", item);
				} else if (item.name == "Heal All Potion") {
					Items.S.battle.AddFunctionToButton(Items.S.battle.HealAllPotion, "Use potion to heal all party members?", item);
				} else if (item.name == "Revive Potion") {
					Items.S.battle.AddFunctionToButton(Items.S.battle.RevivePotion, "Use potion to revive which party member?", item);
				} else if (item.name == "Detoxify Potion") {
					Items.S.battle.AddFunctionToButton(Items.S.battle.DetoxifyPotion, "Use potion to detoxify which poisoned party member?", item);
				} else if (item.name == "Mobilize Potion") {
					Items.S.battle.AddFunctionToButton(Items.S.battle.MobilizePotion, "Use potion to restore the mobility of which paralyzed party member?", item);
				} else if (item.name == "Wake Potion") {
					Items.S.battle.AddFunctionToButton(Items.S.battle.WakePotion, "Use potion to wake up which sleeping party member?", item);
				} else {
					Items.S.battle.CantUseItemInBattle();
				}
			} else { // if Overworld
				if (item.name == "Health Potion") {
					Items.S.world.AddFunctionToButton(Items.S.world.HPPotion, "Heal which party member?", item);
				} else if (item.name == "Magic Potion") {
					Items.S.world.AddFunctionToButton(Items.S.world.MPPotion, "Use MP potion on which party member?", item);
				} else if (item.name == "Detoxify Potion") {
					Items.S.world.AddFunctionToButton(Items.S.world.DetoxifyPotion, "Use potion to detoxify which poisoned party member?", item);
				} else if (item.name == "Heal All Potion") {
					Items.S.world.AddFunctionToButton(Items.S.world.HealAllPotion, "Use potion to heal all party members?", item);
				} else if (item.name == "Warp Potion") {
					Items.S.world.WarpPotion();
				} else {
					Items.S.world.CantUseItem();
				}
			}

			//Battle.S.PlayerTurn();
		}
	}
}