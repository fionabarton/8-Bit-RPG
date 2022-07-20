using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	// Inventory Buttons
	public List<Button> inventoryButtons;
	public List<Text>	inventoryButtonsNameText;
	public List<Text>	inventoryButtonsTypeText;
	public List<Text>	inventoryButtonsPriceText;
	public List<Text>	inventoryButtonsQTYOwnedText;
	public List<Text>	inventoryButtonsQTYEquippedText;
	public Text			goldAmountText;

	[Header("Set Dynamically")]
	// For Input & Display Message
	public eShopScreenMode shopScreenMode = eShopScreenMode.pickItem;

	public bool buyOrSellMode;

	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

	public GameObject previousSelectedGameObject;
	public int previousSelectedNdx;

	// Caches what index of the inventory is currently stored in the first item slot
	public int firstSlotNdx;

	// Prevents instantly registering input when the first or last slot is selected
	private bool verticalAxisIsInUse;
	private bool firstOrLastSlotSelected;

	public List<Item> inventory = new List<Item>();

	private static ShopMenu _S;
	public static ShopMenu S { get { return _S; } set { _S = value; } }

	public ShopScreen_PickItemMode pickItemMode;
	public ShopScreen_ItemPurchasedOrSoldMode itemPurchasedOrSoldMode;
	public ShopScreen_DisplayPotentialStats displayPotentialStats;

	void Awake() {
		S = this;

		// Get components
		pickItemMode = GetComponent<ShopScreen_PickItemMode>();
		itemPurchasedOrSoldMode = GetComponent<ShopScreen_ItemPurchasedOrSoldMode>();
		displayPotentialStats = GetComponent<ShopScreen_DisplayPotentialStats>();
	}

	void Start() {
		Deactivate();
    }

	public void Activate() {
		// Ensures first slot is selected when screen enabled
		previousSelectedGameObject = inventoryButtons[0].gameObject;

		firstSlotNdx = 0;

		// Display gold amount
		goldAmountText.text = Party.S.gold.ToString();

		// Add Loop() to Update Delgate
		UpdateManager.updateDelegate += Loop;

		gameObject.SetActive(true);

		pickItemMode.Setup(S);
	}

	public void Deactivate() {
		// Deactivate screen cursors
		Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

		// Unpause
		GameManager.S.paused = false;
		Blob.S.canMove = true;

		// Deactivate PauseMessage and PlayerButtons
		PauseMessage.S.gameObject.SetActive(false);

		// Remove Loop() from Update Delgate
		UpdateManager.updateDelegate -= Loop;

		// Set Camera to Player gameObject
		//CamManager.S.ChangeTarget(Player.S.gameObject, true);

		// Broadcast event
		EventManager.ShopScreenDeactivated();

		// Deactivate this gameObject
		gameObject.SetActive(false);
	}

	public void Loop() {
		// Reset canUpdate
		if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f) {
			canUpdate = true;
		}

		// Deactivate ShopScreen
		if (Input.GetButtonDown("SNES Y Button")) {
			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
			Deactivate();
		}

		switch (shopScreenMode) {
			case eShopScreenMode.pickItem:
				// On vertical input, scroll the item list when the first or last slot is selected
				if (inventory.Count > inventoryButtons.Count) {
					ScrollItemList();
				}

				pickItemMode.Loop(S);
				break;
			case eShopScreenMode.itemPurchasedOrSold:
				itemPurchasedOrSoldMode.Loop(S);
				break;
		}
	}

	// On vertical input, scroll the item list when the first or last slot is selected
	void ScrollItemList() {
		if (inventory.Count > 1) {
			// If first or last slot selected...
			if (firstOrLastSlotSelected) {
				if (Input.GetAxisRaw("Vertical") == 0) {
					verticalAxisIsInUse = false;
				} else {
					if (!verticalAxisIsInUse) {
						if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == inventoryButtons[0].gameObject) {
							if (Input.GetAxisRaw("Vertical") > 0) {
								if (firstSlotNdx == 0) {
									firstSlotNdx = inventory.Count - inventoryButtons.Count;

									// Set  selected GameObject
									Utilities.S.SetSelectedGO(inventoryButtons[inventoryButtons.Count - 1].gameObject);
								} else {
									firstSlotNdx -= 1;
								}
							}
						} else if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == inventoryButtons[inventoryButtons.Count - 1].gameObject) {
							if (Input.GetAxisRaw("Vertical") < 0) {
								if (firstSlotNdx + inventoryButtons.Count == inventory.Count) {
									firstSlotNdx = 0;

									// Set  selected GameObject
									Utilities.S.SetSelectedGO(inventoryButtons[0].gameObject);
								} else {
									firstSlotNdx += 1;
								}
							}
						}

						pickItemMode.AssignItemEffect(this);
						pickItemMode.AssignItemNames(this);

						// Audio: Selection
						AudioManager.S.PlaySFX(eSoundName.selection);

						verticalAxisIsInUse = true;

						// Allows scrolling when the vertical axis is held down in 0.2 seconds
						Invoke("VerticalAxisScrollDelay", 0.2f);
					}
				}
			}

			// Check if first or last slot is selected
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == inventoryButtons[0].gameObject
			 || UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == inventoryButtons[inventoryButtons.Count-1].gameObject) {
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

	// Import inventory from shopkeeper or party 
	public void ImportInventory(List<Item> inventoryToImport) {
		// Clear Inventory
		inventory.Clear();

		// Import Inventory
		for (int i = 0; i < inventoryToImport.Count; i++) {
			inventory.Add(inventoryToImport[i]);
		}
	}
}