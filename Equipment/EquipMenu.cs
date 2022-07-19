using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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

	public void Activate() {
		gameObject.SetActive(true);

		playerNdx = 0;

		// Ensures first slots are selected when screen enabled
		previousSelectedGameObject = PauseMenu.S.playerNameButtons[playerNdx].gameObject;
		pickTypeToEquipMode.previousSelectedGameObject = equippedButtons[0].gameObject;
		pickItemToEquipMode.previousSelectedGameObject = inventoryButtons[0].gameObject;

		DisplayCurrentEquipmentNames(0);

		pickPartyMemberMode.SetUp(S);

		// Add Loop() to Update Delgate
		UpdateManager.updateDelegate += Loop;

		PauseMenu.S.playerNameButtons[0].Select();
		PauseMenu.S.playerNameButtons[0].OnSelect(null);

		// Set anim controller to party member 1
		//playerAnim.runtimeAnimatorController = PlayerButtons.S.anim[0].runtimeAnimatorController;

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);
	}

	public void Deactivate(bool playSound = false) {
		// Activate Cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);

		// Go back to Pause Screen
		if (GameManager.S.currentScene != "Battle") {
			// Pause Buttons Interactable
			Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, true);

			// Set Selected Gameobject (Pause Screen: Equip Button)
			Utilities.S.SetSelectedGO(PauseMenu.S.buttonGO[2]);

			PauseMessage.S.DisplayText("Welcome to the Pause Screen!");

			PauseMenu.S.canUpdate = true;
		}

		if (playSound) {
			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}

		// Deactivate inventory buttons
		for (int i = 0; i < inventoryButtons.Count; i++) {
			inventoryButtons[i].gameObject.SetActive(false);
		}

		// Set party animations to idle
		PauseMenu.S.SetSelectedMemberAnim("Idle");

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
				// Go back to pickTypeToEquip mode 
				GoBackToPickTypeToEquipMode("SNES B Button", 99);
				GoBackToPickTypeToEquipMode("SNES Y Button", 99);
				break;
		}
	}

	public void GoBackToPickTypeToEquipMode(string inputName, int soundNdx) {
		if (PauseMessage.S.dialogueFinished) {
			if (Input.GetButtonDown(inputName)) {
				// Deactivate Buttons
				for (int i = 0; i < inventoryButtons.Count; i++) {
					inventoryButtons[i].gameObject.SetActive(false);
				}

				// Set Up pickTypeToEquip mode
				pickTypeToEquipMode.SetUp(playerNdx, S, soundNdx);

				// Set Selected Gameobject 
				Utilities.S.SetSelectedGO(pickTypeToEquipMode.previousSelectedGameObject);

				// Activate Cursor
				ScreenCursor.S.cursorGO[0].SetActive(true);

				// Reset inventoryButtons text color
				Utilities.S.SetTextColor(inventoryButtons, new Color32(255, 255, 255, 255));

				// Set selected member animation to walk
				PauseMenu.S.playerAnims[playerNdx].CrossFade("Walk", 0);
			}
		}
	}

	// Display member's name and current stats
	public void DisplayCurrentStats(int playerNdx) {
		titleText.text = "Spells: " + "<color=white>" + Party.S.stats[playerNdx].name + "</color>";
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

	// eEquipScreenMode.equippedItem /////////////////////////////////////////////////////////////

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

		PauseMessage.S.DisplayText(Party.S.stats[playerNdx].name + " equipped " + item.name + "!");

		// Add Item StatEffect
		equipStatsEffect.AddItemEffect(playerNdx, item);

		// Update GUI
		DisplayCurrentStats(playerNdx);
		DisplayCurrentEquipmentNames(playerNdx);

		// Audio: Buff 1
		AudioManager.S.PlaySFX(eSoundName.buff1);

		//playerAnim.CrossFade("Success", 0);
		PauseMenu.S.playerAnims[playerNdx].CrossFade("Success", 0);
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
}