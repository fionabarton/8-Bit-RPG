using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {
	[Header("Set in Inspector")]
	// Text display
	public GameObject displayMessageGO;

	// Player actions
	public GameObject playerActionButtonsGO;

	public GameObject battleMenu;
	public GameObject battleGameObjects;

	// Party stats
	public List<GameObject> partyStats;
	public List<Text> partyNameText;
	public List<Text> partyStatsText;
	public List<Image> partyStartsTextBoxSprite;
	public List<Text> partyStatsHeader;
	public List<Button> partyNameButtonsCS;

	// Mini party member images
	public List<Animator> miniPartyAnims;

	// Party member text box sprite frames 
	public List<Animator> partyTextBoxSpriteAnims;

	// Option buttons
	public List<GameObject> optionButtonsGO;
	public List<Button> optionButtonsCS;
	public List<Text> optionButtonsText;
	public List<GameObject> amountButtonsGO;
	public List<Text> amountButtonsText;

	// Option buttons headers (ex. "Item Name", "MP Cost")
	public GameObject optionsButtonsHeaderTextGO;
	public Text optionsButtonsNameHeaderText;
	public Text optionsButtonsAmountHeaderText;

	// Enemy sprites/buttons
	public List<GameObject> enemyButtonsGO;
	public List<Button> enemyButtonsCS;
	public List<GameObject> enemyHelpBubblesGO;

	// Player whose turn it is
	public Text playerNameText;

	// Cursors
	public List<GameObject> cursors = new List<GameObject>();
	public GameObject actionOptionsButtonsCursor;
	public List<GameObject> enemyButtonsCursors = new List<GameObject>();
	public List<GameObject> partyNameButtonsCursors = new List<GameObject>();

	[Header("Set Dynamically")]
	// Caches what index of the inventory is currently stored in the first item slot
	public int firstSlotNdx;

	// Prevents instantly registering input when the first or last slot is selected
	public bool verticalAxisIsInUse;
	public bool firstOrLastSlotSelected;

	public GameObject previousSelectedOptionButtonGO;

	private Battle _;

	void Start() {
		_ = Battle.S;
	}

	public void ActivatePlayerActionsMenu() {
		playerActionButtonsGO.SetActive(true);
		displayMessageGO.SetActive(false);
		actionOptionsButtonsCursor.SetActive(true);
		Utilities.S.SetActiveList(enemyButtonsCursors, false);
		Utilities.S.SetActiveList(partyNameButtonsCursors, false);
	}

	public void ActivateDisplayMessage() {
		displayMessageGO.SetActive(true);
		playerActionButtonsGO.SetActive(false);
		actionOptionsButtonsCursor.SetActive(false);
		Utilities.S.SetActiveList(enemyButtonsCursors, false);
		Utilities.S.SetActiveList(partyNameButtonsCursors, false);
	}

	public void ActionOptionButtonsCursorPosition(GameObject go) {
		// Activate Cursor
		if (!actionOptionsButtonsCursor.activeInHierarchy) {
			actionOptionsButtonsCursor.SetActive(true);
		}

		// Position Cursor & Set Anim
		if (actionOptionsButtonsCursor.activeInHierarchy) {
			// Action buttons
			if (go == _.playerActions.actionButtonsGO[0]) {
				actionOptionsButtonsCursor.transform.localPosition = new Vector2(-6.1f, -3.725f);
			} else if (go == _.playerActions.actionButtonsGO[1]) {
				actionOptionsButtonsCursor.transform.localPosition = new Vector2(-4.5f, -3.725f);
			} else if (go == _.playerActions.actionButtonsGO[2]) {
				actionOptionsButtonsCursor.transform.localPosition = new Vector2(-6.1f, -4.3f);
			} else if (go == _.playerActions.actionButtonsGO[3]) {
				actionOptionsButtonsCursor.transform.localPosition = new Vector2(-4.5f, -4.3f);
			} else if (go == _.playerActions.actionButtonsGO[4]) {
				actionOptionsButtonsCursor.transform.localPosition = new Vector2(-6.1f, -4.9f);
			} else if (go == _.playerActions.actionButtonsGO[5]) {
				actionOptionsButtonsCursor.transform.localPosition = new Vector2(-4.5f, -4.9f);

			// Option buttons
			} else if (go == optionButtonsGO[0]) {
				actionOptionsButtonsCursor.transform.localPosition = new Vector2(-2, -2.7f);
			} else if (go == optionButtonsGO[1]) {
				actionOptionsButtonsCursor.transform.localPosition = new Vector2(-2, -3.25f);
			} else if (go == optionButtonsGO[2]) {
				actionOptionsButtonsCursor.transform.localPosition = new Vector2(-2, -3.83f);
			} else if (go == optionButtonsGO[3]) {
				actionOptionsButtonsCursor.transform.localPosition = new Vector2(-2, -4.4f);
			} else if (go == optionButtonsGO[4]) {
				actionOptionsButtonsCursor.transform.localPosition = new Vector2(-2, -5f);
			}
		}
	}

	public void EnemySpriteButtonCursorPosition(GameObject go) {
		// Activate Cursor
		if (!enemyButtonsCursors[0].activeInHierarchy) {
			enemyButtonsCursors[0].SetActive(true);
		}

		// Position Cursor & Set Anim
		if (enemyButtonsCursors[0].activeInHierarchy) {
			// Set positions
			switch (_.enemyAmount) {
				case 1:
					if (go == enemyButtonsGO[0]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(0, 2.5f);
					}
					break;
				case 2:
					if (go == enemyButtonsGO[0]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(-1.625f, 2.5f);
					} else if (go == enemyButtonsGO[1]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(1.625f, 2.5f);
					}
					break;
				case 3:
					if (go == enemyButtonsGO[0]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(-3.25f, 2.5f);
					} else if (go == enemyButtonsGO[1]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(0, 2.5f);
					} else if (go == enemyButtonsGO[2]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(3.25f, 2.5f);
					}
					break;
				case 4:
					if (go == enemyButtonsGO[0]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(-4.875f, 2.5f);
					} else if (go == enemyButtonsGO[1]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(-1.625f, 2.5f);
					} else if (go == enemyButtonsGO[2]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(1.625f, 2.5f);
					} else if (go == enemyButtonsGO[3]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(4.875f, 2.5f);
					}
					break;
				case 5:
					if (go == enemyButtonsGO[0]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(-6.5f, 2.5f);
					} else if (go == enemyButtonsGO[1]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(-3.25f, 2.5f);
					} else if (go == enemyButtonsGO[2]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(0, 2.5f);
					} else if (go == enemyButtonsGO[3]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(3.25f, 2.5f);
					} else if (go == enemyButtonsGO[4]) {
						enemyButtonsCursors[0].transform.localPosition = new Vector2(6.5f, 2.5f);
					}
					break;
			}
		}
	}

	public void PartyNameButtonCursorPosition(GameObject go) {
		// Activate Cursor
		if (!partyNameButtonsCursors[0].activeInHierarchy) {
			partyNameButtonsCursors[0].SetActive(true);
		}

		// Position Cursor 
		if (partyNameButtonsCursors[0].activeInHierarchy) {
			// Set positions
			switch (Party.S.partyNdx) {
				case 0:
					if (go == partyNameButtonsCS[0].gameObject) {
						partyNameButtonsCursors[0].transform.localPosition = new Vector2(0, 5.8f);
					}
					break;
				case 1:
					if (go == partyNameButtonsCS[0].gameObject) {
						partyNameButtonsCursors[0].transform.localPosition = new Vector2(-2f, 5.8f);
					} else if (go == partyNameButtonsCS[1].gameObject) {
						partyNameButtonsCursors[0].transform.localPosition = new Vector2(2f, 5.8f);
					}
					break;
				case 2:
					if (go == partyNameButtonsCS[0].gameObject) {
						partyNameButtonsCursors[0].transform.localPosition = new Vector2(-4f, 5.8f);
					} else if (go == partyNameButtonsCS[1].gameObject) {
						partyNameButtonsCursors[0].transform.localPosition = new Vector2(0, 5.8f);
					} else if (go == partyNameButtonsCS[2].gameObject) {
						partyNameButtonsCursors[0].transform.localPosition = new Vector2(4f, 5.8f);
					}
					break;
			}
		}
	}
	//Utilities.S.SetLocalPosition(Battle.S.enemySprites[0], -4.875f, 0);
	//Utilities.S.SetLocalPosition(Battle.S.enemySprites[1], -1.625f, 0);
	//Utilities.S.SetLocalPosition(Battle.S.enemySprites[2], 1.625f, 0);
	//Utilities.S.SetLocalPosition(Battle.S.enemySprites[3], 4.875f, 0);

	//Utilities.S.SetLocalPosition(Battle.S.enemySprites[0], -6.5f, 0);
	//Utilities.S.SetLocalPosition(Battle.S.enemySprites[1], -3.25f, 0);
	//Utilities.S.SetLocalPosition(Battle.S.enemySprites[2], 0, 0);
	//Utilities.S.SetLocalPosition(Battle.S.enemySprites[3], 3.25f, 0);
	//Utilities.S.SetLocalPosition(Battle.S.enemySprites[4], 6.5f, 0);
	public void TargetAllEnemies() {
		// Deactivate all cursors
		Utilities.S.SetActiveList(enemyButtonsCursors, false);

		for(int i = 0; i < _.enemyAmount; i++) {
			enemyButtonsCursors[i].SetActive(true);
		}

		// Activate active cursors
		switch (_.enemyAmount) {
			case 1:
				enemyButtonsCursors[0].transform.localPosition = new Vector2(0, 2.5f);
				break;
			case 2:
				enemyButtonsCursors[0].transform.localPosition = new Vector2(-1.625f, 2.5f);
				enemyButtonsCursors[1].transform.localPosition = new Vector2(1.625f, 2.5f);
				break;
			case 3:
				enemyButtonsCursors[0].transform.localPosition = new Vector2(-3.25f, 2.5f);
				enemyButtonsCursors[1].transform.localPosition = new Vector2(0, 2.5f);
				enemyButtonsCursors[2].transform.localPosition = new Vector2(3.25f, 2.5f);
				break;
			case 4:
				enemyButtonsCursors[0].transform.localPosition = new Vector2(-4.875f, 2.5f);
				enemyButtonsCursors[1].transform.localPosition = new Vector2(-1.625f, 2.5f);
				enemyButtonsCursors[2].transform.localPosition = new Vector2(1.625f, 2.5f);
				enemyButtonsCursors[3].transform.localPosition = new Vector2(4.875f, 2.5f);
				break;
			case 5:
				enemyButtonsCursors[0].transform.localPosition = new Vector2(-6.5f, 2.5f);
				enemyButtonsCursors[1].transform.localPosition = new Vector2(-3.25f, 2.5f);
				enemyButtonsCursors[2].transform.localPosition = new Vector2(0, 2.5f);
				enemyButtonsCursors[3].transform.localPosition = new Vector2(3.25f, 2.5f);
				enemyButtonsCursors[4].transform.localPosition = new Vector2(6.5f, 2.5f);
				break;
		}
	}
	
	public void TargetAllPartyMembers() {
		// Activate all cursors
		for (int i = 0; i <= Party.S.partyNdx; i++) {
			partyNameButtonsCursors[i].SetActive(true);
		}

		// Position and activate active cursors
		switch (Party.S.partyNdx) {
			case 0:
				partyNameButtonsCursors[0].transform.localPosition = new Vector2(0, 5.8f);
				break;
			case 1:
                if (!_.playerDead[0]) { 
					partyNameButtonsCursors[0].transform.localPosition = new Vector2(-2f, 5.8f);
                } else {
					partyNameButtonsCursors[0].SetActive(false);
				}
				if (!_.playerDead[1]) {
					partyNameButtonsCursors[1].transform.localPosition = new Vector2(2f, 5.8f);
				} else {
					partyNameButtonsCursors[1].SetActive(false);
				}
				break;
			case 2:
				if (!_.playerDead[0]) {
					partyNameButtonsCursors[0].transform.localPosition = new Vector2(-4f, 5.8f);
				} else {
					partyNameButtonsCursors[0].SetActive(false);
				}
				if (!_.playerDead[1]) {
					partyNameButtonsCursors[1].transform.localPosition = new Vector2(0, 5.8f);
				} else {
					partyNameButtonsCursors[1].SetActive(false);
				}
				if (!_.playerDead[2]) {
					partyNameButtonsCursors[2].transform.localPosition = new Vector2(4f, 5.8f);
				} else {
					partyNameButtonsCursors[2].SetActive(false);
				}
				break;
		}
	}

	public void UpdatePartyStats(int ndx, bool playerFlickerClip = false) {
		partyStatsText[ndx].text =
			Party.S.stats[ndx].HP.ToString() + "/" + Party.S.stats[ndx].maxHP.ToString() +
			"\n" + Party.S.stats[ndx].MP.ToString() + "/" + Party.S.stats[ndx].maxMP.ToString() +
			"\n" + Party.S.stats[ndx].LVL.ToString();

        if (!playerFlickerClip) {
			// Set party member text box sprite frames animation speed
			if (_.PlayerNdx() == ndx) {
				_.partyStatAnims[ndx].speed = 1;
			} else {
				_.partyStatAnims[ndx].speed = 0;
			}

			// Set text and textBoxSprite color
			if (Utilities.S.GetPercentage(Party.S.stats[ndx].HP, Party.S.stats[ndx].maxHP) >= 0.25f) {
				if (_.partyStatAnims[ndx].gameObject.activeInHierarchy) {
					_.partyStatAnims[ndx].Play("White Idle", -1, 0);
				}
			} else if (Party.S.stats[ndx].HP > 0 && Utilities.S.GetPercentage(Party.S.stats[ndx].HP, Party.S.stats[ndx].maxHP) < 0.25f) {
				if (_.partyStatAnims[ndx].gameObject.activeInHierarchy) {
					_.partyStatAnims[ndx].Play("Orange Idle", -1, 0);
				}
			} else {
				if (_.partyStatAnims[ndx].gameObject.activeInHierarchy) {
					_.partyStatAnims[ndx].Play("Red Idle", -1, 0);
				}
			}
		} else {
			SetPartyFlickerAnim(ndx);
		}
	}

	public void SetPartyFlickerAnim(int ndx) {
		_.partyStatAnims[ndx].speed = 1;

		// Set text and textBoxSprite color
		if (Utilities.S.GetPercentage(Party.S.stats[ndx].HP, Party.S.stats[ndx].maxHP) >= 0.25f) {
			if (_.partyStatAnims[ndx].gameObject.activeInHierarchy) {
				_.partyStatAnims[ndx].Play("White Flicker", -1, 0);
			}
		} else if (Party.S.stats[ndx].HP > 0 && Utilities.S.GetPercentage(Party.S.stats[ndx].HP, Party.S.stats[ndx].maxHP) < 0.25f) {
			if (_.partyStatAnims[ndx].gameObject.activeInHierarchy) {
				_.partyStatAnims[ndx].Play("Orange Flicker", -1, 0);
			}
		} else {
			if (_.partyStatAnims[ndx].gameObject.activeInHierarchy) {
				_.partyStatAnims[ndx].Play("Red Flicker", -1, 0);
			}
		}
	}

	//// Unused because of animator conflict ("Death" clip sets the sprite renderer's color)
	//public void SetEnemyColor(int ndx) {
	//	if (Utilities.S.GetPercentage(Battle.S.enemyStats[ndx].HP, Battle.S.enemyStats[ndx].maxHP) >= 0.25f) {
	//		_.enemySRends[ndx].color = new Color32(255, 255, 255, 255);
	//	} else if (Battle.S.enemyStats[ndx].HP > 0 && Utilities.S.GetPercentage(Battle.S.enemyStats[ndx].HP, Battle.S.enemyStats[ndx].maxHP) < 0.25f) {
	//		_.enemySRends[ndx].color = new Color32(229, 92, 15, 255);
	//	} else {
	//		_.enemySRends[ndx].color = new Color32(168, 7, 32, 255);
	//	}
	//}

	public void PositionEnemySprites() {
		// Set enemy sprites positions
		switch (_.enemyAmount) {
			case 1:
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[0], 0, 0);
				break;
			case 2:
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[0], -1.625f, 0);
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[1], 1.625f, 0);
				break;
			case 3:
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[0], -3.25f, 0);
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[1], 0, 0);
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[2], 3.25f, 0);
				break;
			case 4:
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[0], -4.875f, 0);
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[1], -1.625f, 0);
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[2], 1.625f, 0);
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[3], 4.875f, 0);
				break;
			case 5:
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[0], -6.5f, 0);
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[1], -3.25f, 0);
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[2], 0, 0);
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[3], 3.25f, 0);
				Utilities.S.SetLocalPosition(Battle.S.enemySprites[4], 6.5f, 0);
				break;
				//case 1:
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[0], 0, 0);
				//	break;
				//case 2:
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[0], -1.5f, 0);
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[1], 1.5f, 0);
				//	break;
				//case 3:
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[0], -3f, 0);
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[1], 0, 0);
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[2], 3f, 0);
				//	break;
				//case 4:
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[0], -4.5f, 0);
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[1], -1.5f, 0);
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[2], 1.5f, 0);
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[3], 4.5f, 0);
				//	break;
				//case 5:
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[0], -6f, 0);
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[1], -3f, 0);
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[2], 0, 0);
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[3], 3f, 0);
				//	Utilities.S.SetLocalPosition(Battle.S.enemySprites[4], 6f, 0);
				//	break;
		}

		// Deactivate all enemy help bubbles sprites
		Utilities.S.SetActiveList(_.UI.enemyHelpBubblesGO, false);

		// Activate enemy help bubbles sprites
		for (int i = 0; i < _.enemyAmount; i++) {
            if (_.enemyStats[i].isCallingForHelp) {
				_.UI.enemyHelpBubblesGO[i].SetActive(true);
			}

			// Update Health Bars
			ProgressBars.S.enemyHealthBarsCS[i].UpdateBar(_.enemyStats[i].HP, _.enemyStats[i].maxHP);
		}

		// Update activated status ailment icons
		StatusEffects.S.UpdateActivatedEnemyStatusIcons();
	}

	// On vertical input, scroll the spell list when the first or last slot is selected
	public void ScrollList(int amount, bool itemOrSpellNames) {
		// If first or last slot selected...
		if (firstOrLastSlotSelected) {
			if (Input.GetAxisRaw("Vertical") == 0) {
				verticalAxisIsInUse = false;
			} else {
				if (!verticalAxisIsInUse) {
					if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == optionButtonsGO[0]) {
						if (Input.GetAxisRaw("Vertical") > 0) {
							if (firstSlotNdx == 0) {
								firstSlotNdx = amount - optionButtonsGO.Count;

								// Set  selected GameObject
								Utilities.S.SetSelectedGO(optionButtonsGO[4]);

							} else {
								firstSlotNdx -= 1;
							}
						}
					} else if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == optionButtonsGO[4]) {
						if (Input.GetAxisRaw("Vertical") < 0) {
							if (firstSlotNdx + optionButtonsGO.Count == amount) {
								firstSlotNdx = 0;

								// Set  selected GameObject
								Utilities.S.SetSelectedGO(optionButtonsGO[0]);
							} else {
								firstSlotNdx += 1;
							}
						}
					}

					if (itemOrSpellNames) {
						AssignItemNames();
                    } else {
						AssignSpellNames();
					}

					// Audio: Selection
					AudioManager.S.PlaySFX(eSoundName.selection);

					verticalAxisIsInUse = true;

					// Allows scrolling when the vertical axis is held down in 0.2 seconds
					Invoke("VerticalAxisScrollDelay", 0.2f);
				}
			}
		}

		// Check if current selected gameObject is not the previous selected gameObject
		if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != previousSelectedOptionButtonGO) {
			// Check if first or last slot is selected
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == optionButtonsGO[0]
			 || UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == optionButtonsGO[4]) {
				firstOrLastSlotSelected = true;
				verticalAxisIsInUse = true;

				// Allows scrolling when the vertical axis is held down in 0.2 seconds
				Invoke("VerticalAxisScrollDelay", 0.2f);
			} else {
				firstOrLastSlotSelected = false;
			}

			// Audio: Selection (when a new gameObject is selected)
			Utilities.S.PlayButtonSelectedSFX(ref previousSelectedOptionButtonGO);
		}
	}

	// Allows scrolling when the vertical axis is held down 
	void VerticalAxisScrollDelay() {
		verticalAxisIsInUse = false;
	}

	public void DeactivateUnusedSlots(int amount) {
		for (int i = 0; i < optionButtonsGO.Count; i++) {
			if (i < amount) {
				optionButtonsGO[i].gameObject.SetActive(true);
				amountButtonsGO[i].gameObject.SetActive(true);
			} else {
				optionButtonsGO[i].gameObject.SetActive(false);
				amountButtonsGO[i].gameObject.SetActive(false);
			}
		}
	}

	// Assign on button click effects
	public void AssignAttackEnemyEffect() {
		Utilities.S.RemoveListeners(enemyButtonsCS);
		for (int i = 0; i < _.enemyAmount; i++) {
			// Add listener to option button
			int copy = i;
			enemyButtonsCS[copy].onClick.AddListener(delegate { _.playerActions.ClickedAttackEnemy(copy); });
		}
	}
	public void AssignAttackAllEnemiesEffect() {
		Utilities.S.RemoveListeners(enemyButtonsCS);
		for (int i = 0; i < _.enemyAmount; i++) {
			// Add listener to option button
			int copy = i;
			enemyButtonsCS[copy].onClick.AddListener(delegate { _.playerActions.AttackAllEnemies(); });
		}
	}
	public void AssignItemEffect() {
		Utilities.S.RemoveListeners(optionButtonsCS);
		for (int i = 0; i < optionButtonsCS.Count; i++) {
			// Add listener to option button
			int copy = i;
			optionButtonsCS[copy].onClick.AddListener(delegate { Items.S.menu.ConsumeItem(Inventory.S.GetBattleItemList()[firstSlotNdx + copy]); });
		}
	}
	public void AssignSpellEffect() {
		Utilities.S.RemoveListeners(optionButtonsCS);
		for (int i = 0; i < optionButtonsCS.Count; i++) {
			// Add listener to option button
			int copy = i;
			optionButtonsCS[copy].onClick.AddListener(delegate { Spells.S.menu.UseSpell(Party.S.stats[_.PlayerNdx()].spells[firstSlotNdx + copy]); });
		}
	}

	// Assign slot name text
	public void AssignEnemyNames() {
		// Assign option slots names
		for (int i = 0; i < optionButtonsGO.Count; i++) {
			if (firstSlotNdx + i < _.enemyAmount) {
				string ndx = (firstSlotNdx + i + 1).ToString();
				optionButtonsText[i].text = ndx + ") " + _.enemyStats[firstSlotNdx + i].name;
				amountButtonsText[i].text = "";
			}
		}
	}
	public void AssignItemNames() {
		// Assign option slots names
		for (int i = 0; i < optionButtonsGO.Count; i++) {
			if (firstSlotNdx + i < Inventory.S.GetBattleItemList().Count) {
				string ndx = (firstSlotNdx + i + 1).ToString();
				optionButtonsText[i].text = ndx + ") " + Inventory.S.GetBattleItemList()[firstSlotNdx + i].name;
				amountButtonsText[i].text = "x" + Inventory.S.GetItemCount(Inventory.S.GetBattleItemList()[firstSlotNdx + i]).ToString();
				//Inventory.S.GetItemList()[firstSlotNdx + i].type = 
			}
		}
	}
	public void AssignSpellNames() {
		// Assign option slots names
		for (int i = 0; i < optionButtonsGO.Count; i++) {
			if (firstSlotNdx + i < Party.S.stats[_.PlayerNdx()].spellNdx) {
				string ndx = (firstSlotNdx + i + 1).ToString();
				optionButtonsText[i].text = ndx + ") " + Party.S.stats[_.PlayerNdx()].spells[firstSlotNdx + i].name;
				amountButtonsText[i].text =  Party.S.stats[_.PlayerNdx()].spells[firstSlotNdx + i].cost.ToString();
			}
		}
	}

	// Set the first and last button’s navigation 
	public void SetOptionButtonsNavigation(int amount) {
		// Reset all button's navigation to automatic
		Utilities.S.ResetButtonNavigation(optionButtonsCS);

		// Set button navigation if inventory is less than optionButtonsGO.Count
		if (amount <= optionButtonsCS.Count) {
			if (amount > 1) {
				// Set first button navigation
				Utilities.S.SetButtonNavigation(
					optionButtonsCS[0],
					optionButtonsCS[amount - 1],
					optionButtonsCS[1]);

				// Set last button navigation
				Utilities.S.SetButtonNavigation(
					optionButtonsCS[amount - 1],
					optionButtonsCS[amount - 2],
					optionButtonsCS[0]);
			}
		}
	}

	// Sets the mini party members' animations
	public void SetPartyMemberAnim(string animName = "Idle", string selectedMemberAnimName = "Walk", int selectedMemberNdx = -1) {
		// Set all party member animations
		for (int i = 0; i < miniPartyAnims.Count; i++) {
			if (miniPartyAnims[i].gameObject.activeInHierarchy) {
				if(i != selectedMemberNdx) {
					if (Party.S.stats[i].HP > 0) {
						miniPartyAnims[i].CrossFade(animName, 0);
					} else {
						miniPartyAnims[i].CrossFade("Fail", 0);
					}
				}
			}
		}

		// Set target member animation
		if(selectedMemberNdx != -1) {
			miniPartyAnims[selectedMemberNdx].CrossFade(selectedMemberAnimName, 0);
		}
	}

	public void RemoveAllListeners() {
		Utilities.S.RemoveListeners(optionButtonsCS);
		Utilities.S.RemoveListeners(enemyButtonsCS);
		Utilities.S.RemoveListeners(partyNameButtonsCS);
	}

	// Set the sorting layer of each battle cursor's sprite renderer
	public void SetCursorSpriteSortingLayer(string layerName) {
		for (int i = 0; i < cursors.Count; i++) {
			SpriteRenderer sRend = cursors[i].GetComponent<SpriteRenderer>();
			if (sRend) {
				sRend.sortingLayerName = layerName;
			}
		}
	}

	// Activates text header above options buttons (ex. "Item Name", "MP Cost")
	public void ActivateOptionsButtonTextHeader(string nameText, string amountText) {
		// Activate header parent gameobject
		optionsButtonsHeaderTextGO.SetActive(true);

		// Set 'Name' and 'Amount' text 
		optionsButtonsNameHeaderText.text = nameText;
		optionsButtonsAmountHeaderText.text = amountText;
	}
}