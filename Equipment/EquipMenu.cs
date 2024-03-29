﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	public Text titleText;
	public GameObject equippedItemTypeNames;

	// Equipped Buttons (the currently selected party member's equipment)
	public List<Button> equippedButtons;
	public List<Text> equippedButtonsTxt;

	// Inventory Buttons (dynamic list of different types of items to be equipped (list of either weapon, armor, etc.))
	public List<Button> inventoryButtons;
	public List<Text> inventoryButtonsTxt;

	// Rect transform (for positioning game object)
	public RectTransform rectTrans;

	[Header("Set Dynamically")]
	public int playerNdx = 0;

	// Each party member's current equipment ([playerNdx][Weapon, Armor, Helmet, Other])
	public List<List<Item>> playerEquipment = new List<List<Item>>();
	
	public eEquipScreenMode equipScreenMode = eEquipScreenMode.pickPartyMember;

	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

	public GameObject previousSelectedGameObject;

	private static EquipMenu _S;
	public static EquipMenu S { get { return _S; } set { _S = value; } }

	public EquipStatsEffect equipStatsEffect;
	public EquipScreen_PickPartyMemberMode pickPartyMemberMode;
	public EquipScreen_PickTypeToEquipMode pickTypeToEquipMode;
	public EquipScreen_PickItemToEquipMode pickItemToEquipMode;

	void Awake() {
		S = this;

		// Get components
		equipStatsEffect = GetComponent<EquipStatsEffect>();
		pickPartyMemberMode = GetComponent<EquipScreen_PickPartyMemberMode>();
		pickTypeToEquipMode = GetComponent<EquipScreen_PickTypeToEquipMode>();
		pickItemToEquipMode = GetComponent<EquipScreen_PickItemToEquipMode>();
	}

	void Start() {
		gameObject.SetActive(false);
	}

	public void SetInitialEquipment() {
		// Intialize the party's equipment 
		playerEquipment.Add(new List<Item> {
			Items.S.items[18], Items.S.items[19], Items.S.items[20], Items.S.items[21]
		});

		playerEquipment.Add(new List<Item> {
			Items.S.items[18], Items.S.items[19], Items.S.items[20], Items.S.items[21]
		});

		playerEquipment.Add(new List<Item> {
			Items.S.items[18], Items.S.items[19], Items.S.items[20], Items.S.items[21]
		});

		// Add effect of each party member's equipment
		equipStatsEffect.AddItemEffect(0, Items.S.items[18]);
		equipStatsEffect.AddItemEffect(0, Items.S.items[19]);
		equipStatsEffect.AddItemEffect(0, Items.S.items[20]);
		equipStatsEffect.AddItemEffect(0, Items.S.items[21]);

		equipStatsEffect.AddItemEffect(1, Items.S.items[18]);
		equipStatsEffect.AddItemEffect(1, Items.S.items[19]);
		equipStatsEffect.AddItemEffect(1, Items.S.items[20]);
		equipStatsEffect.AddItemEffect(1, Items.S.items[21]);

		equipStatsEffect.AddItemEffect(2, Items.S.items[18]);
		equipStatsEffect.AddItemEffect(2, Items.S.items[19]);
		equipStatsEffect.AddItemEffect(2, Items.S.items[20]);
		equipStatsEffect.AddItemEffect(2, Items.S.items[21]);
	}

	public void Activate(float anchoredYPosition = -25) {
		gameObject.SetActive(true);

		playerNdx = 0;

		// Position game object
		rectTrans.anchoredPosition = new Vector2(0, anchoredYPosition);

		// Ensures first slots are selected when screen enabled
		pickTypeToEquipMode.previousSelectedGameObject = equippedButtons[0].gameObject;
		pickItemToEquipMode.previousSelectedGameObject = inventoryButtons[0].gameObject;

		// Reset equippedButtons text color
		Utilities.S.SetTextColor(equippedButtons, new Color32(255, 255, 255, 255));

		// Add Loop() to Update Delgate
		UpdateManager.updateDelegate += Loop;

		if (!GameManager.S.IsBattling()) {
			DisplayCurrentEquipmentNames(0);

			// Set up for pick party member mode
			pickPartyMemberMode.SetUp(S);

			// Ensures first slots are selected when screen enabled
			previousSelectedGameObject = PauseMenu.S.playerNameButtons[playerNdx].gameObject;

			PauseMenu.S.playerNameButtons[0].Select();
			PauseMenu.S.playerNameButtons[0].OnSelect(null);

			// Set party member button navigation
			Utilities.S.SetHorizontalButtonsNavigation(PauseMenu.S.playerNameButtons, Party.S.partyNdx + 1);
		} else {
			// Set up for pick type to equip mode
			pickTypeToEquipMode.SetUp(Battle.S.PlayerNdx(), S);

			// Ensures first slots are selected when screen enabled
			previousSelectedGameObject = Battle.S.UI.partyNameButtonsCS[playerNdx].gameObject;

			Utilities.S.SetHorizontalButtonsNavigation(Battle.S.UI.partyNameButtonsCS, Party.S.partyNdx + 1);

			// Deactivate battle game objects, enemy sprites in particular
			Battle.S.UI.battleGameObjects.SetActive(false);
		}

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);
	}

	public void Deactivate(bool playSound = false) {
		// Go back to Pause Screen
		if (GameManager.S.currentScene != "Battle") { 
			// Activate Cursor
			ScreenCursor.S.cursorGO[0].SetActive(true);

			// Pause Buttons Interactable
			Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, true);
			Utilities.S.ButtonsInteractable(PauseMenu.S.playerNameButtons, false);

			// Set Selected Gameobject (Pause Screen: Equip Button)
			Utilities.S.SetSelectedGO(PauseMenu.S.buttonGO[2]);

			// Set party animations to idle
			PauseMenu.S.SetSelectedMemberAnim("Idle", true);

			PauseMessage.S.DisplayText("Welcome to the Pause Screen!");

			// Set pause menu's party sprites below skills menu
			PauseMenu.S.SwapPartyMemberGOParentAndOrderInHierarchy(false);

			PauseMenu.S.canUpdate = true;
        } else {
			// Activate battle game objects, enemy sprites in particular
			Battle.S.UI.battleGameObjects.SetActive(true);

			// Deactivate Cursor
			ScreenCursor.S.cursorGO[0].SetActive(false);
		}

		if (playSound) {
			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}

		// Deactivate inventory buttons
		for (int i = 0; i < inventoryButtons.Count; i++) {
			inventoryButtons[i].gameObject.SetActive(false);
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

		switch (equipScreenMode) {
			case eEquipScreenMode.pickPartyMember:
				pickPartyMemberMode.Loop(S);
				break;
			case eEquipScreenMode.pickTypeToEquip:
				pickTypeToEquipMode.Loop(S);
				break;
			case eEquipScreenMode.pickItemToEquip:
				pickItemToEquipMode.Loop(S);
				break;
			case eEquipScreenMode.noInventory:
			case eEquipScreenMode.equippedItem:
				GoBackToPickTypeToEquipMode("SNES B Button", 99);
				GoBackToPickTypeToEquipMode("SNES Y Button", 99);
				break;
		}
	}

	public void GoBackToPickTypeToEquipMode(string inputName, int soundNdx) {
		if (!GameManager.S.IsBattling()) {
			if (PauseMessage.S.dialogueFinished) {
				if (Input.GetButtonDown(inputName)) {
					GoBackToPickTypeToEquipModeHelper(soundNdx);

					// Set selected member animation to walk
					PauseMenu.S.playerAnims[playerNdx].CrossFade("Walk", 0);
				}
			}
        } else {
			if (Battle.S.dialogue.dialogueFinished) {
				if (Input.GetButtonDown(inputName)) {
					GoBackToPickTypeToEquipModeHelper(soundNdx);
				}
			}
		}
	}

	public void GoBackToPickTypeToEquipModeHelper(int soundNdx) {
		// Deactivate Buttons
		for (int i = 0; i < inventoryButtons.Count; i++) {
			inventoryButtons[i].gameObject.SetActive(false);
		}

		// Reset firstSlotNdx
		pickItemToEquipMode.firstSlotNdx = 0;

		// Set Up pickTypeToEquip mode
		pickTypeToEquipMode.SetUp(playerNdx, S, soundNdx);

		// Set Selected Gameobject 
		Utilities.S.SetSelectedGO(pickTypeToEquipMode.previousSelectedGameObject);

		// Activate Cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);

		// Reset inventoryButtons text color
		Utilities.S.SetTextColor(inventoryButtons, new Color32(255, 255, 255, 255));
	}

	// Display member's name and current stats
	public void DisplayCurrentStats(int playerNdx) {
		titleText.text = "Gear: " + "<color=white>" + Party.S.stats[playerNdx].name + "</color>";
		equipStatsEffect.currentAttributeAmounts.text = Party.S.stats[playerNdx].STR + "\n" + Party.S.stats[playerNdx].DEF + "\n" + Party.S.stats[playerNdx].WIS + "\n" + Party.S.stats[playerNdx].AGI;
		equipStatsEffect.potentialStats.text = "";
	}

	// Display the names of the member's current equipment
	public void DisplayCurrentEquipmentNames(int playerNdx) {
		// Set Button Text
		for (int i = 0; i < equippedButtonsTxt.Count; i++) {
			equippedButtonsTxt[i].text = playerEquipment[playerNdx][i].name;
		}
	}

	// Assign names to inventory slot buttons' text
	public void AssignInventorySlotNames() {
		for (int j = 0; j < SortItems.S.tItems.Count; j++) {
			if (j < inventoryButtonsTxt.Count) {
				string ndx = (j + 1 + pickItemToEquipMode.firstSlotNdx).ToString();
				inventoryButtonsTxt[j].text = (ndx) + ") " + SortItems.S.tItems[j + pickItemToEquipMode.firstSlotNdx].name;
			}
		}
	}

	// Remove equipped item and equip new item
	public void EquipItem(int playerNdx, Item item) {
		// Remove Listeners
		Utilities.S.RemoveListeners(inventoryButtons);

		// Switch mode
		SwitchMode(eEquipScreenMode.equippedItem, null, false);

		// Buttons Interactable
		Utilities.S.ButtonsInteractable(inventoryButtons, false);

		// Deactivate screen cursors
		Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

		// Add old item back to inventory
		Inventory.S.AddItemToInventory(playerEquipment[playerNdx][(int)item.type]);
		// Remove new item from inventory
		Inventory.S.RemoveItemFromInventory(item);

		// Subtract old item stat effect 
		equipStatsEffect.RemoveItemEffect(playerNdx, playerEquipment[playerNdx][(int)item.type]);

		// Equip new item
		playerEquipment[playerNdx][(int)item.type] = item;

		if (!GameManager.S.IsBattling()) {
			PauseMessage.S.DisplayText(Party.S.stats[playerNdx].name + " equipped " + item.name + "!");

			PauseMenu.S.playerAnims[playerNdx].CrossFade("Success", 0);
		} else {
			Battle.S.dialogue.DisplayText(Party.S.stats[playerNdx].name + " equipped " + item.name + "!");
		}

		// Add Item StatEffect
		equipStatsEffect.AddItemEffect(playerNdx, item);

		// Update GUI
		DisplayCurrentStats(playerNdx);
		DisplayCurrentEquipmentNames(playerNdx);

		// Audio: Buff 1
		AudioManager.S.PlaySFX(eSoundName.buff1);
	}

	public void SwitchMode(eEquipScreenMode mode, GameObject selectedGO, bool potentialStats) {
		canUpdate = true;

		// Switch ScreenMode
		equipScreenMode = mode;

		// Activate Potential Stat
		equipStatsEffect.potentialStatHolder.SetActive(potentialStats);

		// Set Selected GameObject 
		Utilities.S.SetSelectedGO(selectedGO);
	}

	public int GetEquippedItemCount(Item item) {
		int count = 0;

		// Loop through each party member
		for (int i = 0; i <= Party.S.partyNdx; i++) {
			// Loop through each equipment slot (weapon, armor, helmet, accessory)
			for (int j = 0; j < 4; j++) {
				if (playerEquipment[i][j].name == item.name) {
					count += 1;
				}
			}
		}
		return count;
	}

	// Load/save a party members's equipped gear //////////////////
	///////////////////////////////////////////////////////////////

	// Save a party members's equipped gear:
	// Convert a party members's equipped gear into a string of item ids
	public string GetEquippedGearString(int ndx) {
		// Get list of items
		List<Item> list = new List<Item>(playerEquipment[ndx]);

		// Initialize string to store item ids
		string inventoryString = "";

		// Loop over each item 
		for (int i = 0; i < list.Count; i++) {
			// Add item id to inventorystring
			inventoryString += list[i].id;
		}

		// Return string of item keys
		return inventoryString;
	}

	// Load a party members's equipped gear:
	// Convert a string of item ids into a party members's equipped gear
	public void GetEquippedGearString(string inventoryString, int ndx) {
		// Clear current inventory 
		playerEquipment[ndx].Clear();

		// Initialize string to temporarily store each item id
		string itemId = "";

		// Loop over string of item ids
		for (int i = 0; i < inventoryString.Length; i++) {
			// Build 3-char item id
			itemId += inventoryString[i];

			// Every 3rd char...
			if ((i + 1) % 3 == 0) {
				// If item id is valid...
				for (int j = 0; j < Items.S.items.Length; j++) {
					if (itemId == Items.S.items[j].id) {
						// Add item to player equipment
						playerEquipment[ndx].Add(Items.S.items[j]);

						// Add item effect
						equipStatsEffect.AddItemEffect(ndx, Items.S.items[j]);
					}
				}

				// Reset string to build next 3-char item id
				itemId = "";
			}
		}
	}
}