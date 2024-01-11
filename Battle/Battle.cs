using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Battle : MonoBehaviour {
	[Header("Set in Inspector")]
	public List<GameObject> enemySprites;
	public List<SpriteRenderer> enemySRends;

	///////////////////////////////// ANIMATORS /////////////////////////////////
	public List<Animator> partyStatAnims;
	public List<Animator> enemyAnims;

	// Shakes the canvas back and forth
	public Animator battleUIAnim;

	/////////////////////////////////// QTE /////////////////////////////////
	//// QTE enabled
	public bool qteEnabled = false;

	[Header("Set Dynamically")]
	public eBattleMode mode;

	// Attack Damage, Random Factor
	public int attackDamage, randomFactor, qteBonusDamage;

	public int enemyAmount, partyQty;

	public int totalEnemyAmount;

	// The names of currently engaged Party Members & Enemies
	public List<int> turnOrder;

	// The index of which character's turn it currently is
	public int turnNdx;

	// Incremented each time all combatants have taken their turn
	public int roundNdx;

	public List<EnemyStats> enemyStats = new List<EnemyStats>();
	public List<GameObject> enemyGameObjectHolders = new List<GameObject>();

	public List<bool> playerDead;

	public int expToAdd, goldToAdd;

	// Dynamic list that stores which quests have been completed during this battle
	public List<int> completedQuestNdxs;

	// Dropped Items
	public List<Item> droppedItems = new List<Item>();

	// Stores index of enemy that's currently being targeted
	public int targetNdx;

	// Stores index of player or enemy that's taking their turn
	public int activeCombatantAnimNdx;

	public float chanceToRun = 0.5f;

	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

	public GameObject previousSelectedGameObject;

	// Ensures audio is only played once when button is selected
	public GameObject previousSelectedForAudio;

	// Dynamic list that stores the indexes of which members/enemies died this turn
	public List<int> deadCombatantNdxs;

	// List that tracks what status ailments are afflicting the active combatant
	public List<bool> activeCombatantHasAilments = new List<bool> { false, false, false };

	public BattleInitiative initiative;
	public BattleDialogue dialogue;
	public BattleEnd end;
	public BattleEnemyActions enemyActions;
	public BattleEnemyAI enemyAI;
	public BattlePlayerActions playerActions;
	public BattleUI UI;
	public BattleQTE qte;
	public BattleStats stats;

	private static Battle _S;
	public static Battle S { get { return _S; } set { _S = value; } }

	// DontDestroyOnLoad
	private static bool exists;

	void Awake() {
		S = this;

		// Get components
		initiative = GetComponent<BattleInitiative>();
		dialogue = GetComponent<BattleDialogue>();
		end = GetComponent<BattleEnd>();
		enemyActions = GetComponent<BattleEnemyActions>();
		enemyAI = GetComponent<BattleEnemyAI>();
		playerActions = GetComponent<BattlePlayerActions>();
		UI = GetComponent<BattleUI>();
		qte = GetComponent<BattleQTE>();
		stats = GetComponent<BattleStats>();

		// DontDestroyOnLoad
		if (!exists) {
			exists = true;
			DontDestroyOnLoad(transform.gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	#region Update Loop
	public void Loop() {
		if (dialogue.dialogueFinished) {
			// Dialogue Loop
			dialogue.Loop();

			// Reset canUpdate
			if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f) {
				canUpdate = true;
			}

			if (dialogue.dialogueNdx <= 0) {
				switch (mode) {
					case eBattleMode.actionButtons:
						break;
					case eBattleMode.canGoBackToFightButton:
						UI.EnemySpriteButtonCursorPosition(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);

						if (Input.GetButtonDown("SNES Y Button")) {
							playerActions.GoBackToActionButtons();
						}
						break;
					case eBattleMode.canGoBackToFightButtonMultipleTargets:
						if (Input.GetButtonDown("SNES Y Button")) {
							playerActions.GoBackToActionButtons();
						}
						break;
					case eBattleMode.selectPartyMember:
						UI.PartyNameButtonCursorPosition(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);

						if (Input.GetButtonDown("SNES Y Button")) {
							playerActions.GoBackToActionButtons();
						}
						break;
					case eBattleMode.selectAll:
						if (Input.GetButtonDown("SNES Y Button")) {
							playerActions.GoBackToActionButtons();
						}
						break;
					case eBattleMode.playerTurn:
						if (Input.GetButtonDown("SNES B Button")) {
							PlayerTurn();
						}
                        break;
					case eBattleMode.qteInitialize:
						qte.Loop();

						if (Input.GetButtonDown("SNES Y Button")) {
							// Audio: Deny
							AudioManager.S.PlaySFX(eSoundName.deny);

							playerActions.FightButton();
						}
						break;
					case eBattleMode.qte:
						qte.Loop();
						break;
					case eBattleMode.itemMenu:
						// On vertical input, scroll the item list when the first or last slot is selected
						UI.ScrollList(Inventory.S.GetBattleItemList().Count, true);

						if (Input.GetButtonDown("SNES Y Button")) {
							playerActions.GoBackToActionButtons();
						}
						break;
					case eBattleMode.spellMenu:
						// On vertical input, scroll the item list when the first or last slot is selected
						UI.ScrollList(Party.S.stats[PlayerNdx()].spellNdx, false);

						if (Input.GetButtonDown("SNES Y Button")) {
							playerActions.GoBackToActionButtons();
						}
						break;
					case eBattleMode.gearMenu:
						if(EquipMenu.S.equipScreenMode == eEquipScreenMode.pickTypeToEquip) {
                            if (Input.GetButtonDown("SNES Y Button")) {
                                playerActions.GoBackToActionButtons();
                            }
                        }
						break;
					case eBattleMode.triedToRunFromBoss:
						if (Input.GetButtonDown("SNES B Button")) {
							PlayerTurn();
						}
						break;
					case eBattleMode.enemyTurn:
						if (Input.GetButtonDown("SNES B Button")) {
							EnemyTurn();
						}
						break;
					case eBattleMode.enemyAction:
						if (Input.GetButtonDown("SNES B Button")) {
							// If the enemy announced what move it would perform during its previous turn...
							if (enemyStats[EnemyNdx()].nextTurnActionNdx != 999) {
								// Cache move index
								int moveNdx = enemyStats[EnemyNdx()].nextTurnActionNdx;

								// Reset this enemy's nextTurnMoveNdx
								enemyStats[EnemyNdx()].nextTurnActionNdx = 999;

								// ...call previously announced move this turn
								enemyAI.CallEnemyAction(moveNdx);
							// If the enemy didn't announce what move it would perform during its previous turn...
							} else {
								// ...let its AI dictate what move to perform
								enemyAI.EnemyAI();
							}
						}
						break;
					case eBattleMode.checkIfParalyzed:
						// If this turn is a player's turn...
						if (PlayerNdx() != -1) {
							if (StatusEffects.S.CheckIfParalyzed(true, PlayerNdx())) {
                                if (Input.GetButtonDown("SNES B Button")) {
                                    StatusEffects.S.Paralyzed(Party.S.stats[PlayerNdx()].name, true, PlayerNdx());
								}
							} else {
								mode = eBattleMode.checkIfSleeping;
							}
						} else {
							if (StatusEffects.S.CheckIfParalyzed(false, EnemyNdx())) {
                                if (Input.GetButtonDown("SNES B Button")) {
                                    StatusEffects.S.Paralyzed(enemyStats[EnemyNdx()].name, false, EnemyNdx());
								}
							} else {
								mode = eBattleMode.checkIfSleeping;
							}
						}
						break;
					case eBattleMode.checkIfSleeping:
						// If this turn is a player's turn...
						if (PlayerNdx() != -1) {
							if (StatusEffects.S.CheckIfSleeping(true, PlayerNdx())) {
								if (Input.GetButtonDown("SNES B Button")) {
									StatusEffects.S.Sleeping(Party.S.stats[PlayerNdx()].name, true, PlayerNdx());
								}
							} else {
								// If paralyzed, skip turn
								if (StatusEffects.S.CheckIfParalyzed(true, PlayerNdx())) {
									if (Input.GetButtonDown("SNES B Button")) {
										// Skip turn
										NextTurn();

										// If next turn is a player's turn...
										if (PlayerNdx() != -1) {
											PlayerTurn();
										} else {
											EnemyTurn();
										}
									}
								} else {
									if (Input.GetButtonDown("SNES B Button")) {
										PlayerTurn(true, false);
									}
								}
							}
						} else {
							if (StatusEffects.S.CheckIfSleeping(false, EnemyNdx())) {
								if (Input.GetButtonDown("SNES B Button")) {
									StatusEffects.S.Sleeping(enemyStats[EnemyNdx()].name, false, EnemyNdx());
								}
							} else {
								// If paralyzed, skip turn
								if (StatusEffects.S.CheckIfParalyzed(false, EnemyNdx())) {
									if (Input.GetButtonDown("SNES B Button")) {
										// Skip turn
										NextTurn();

										// If next turn is a player's turn...
										if (PlayerNdx() != -1) {
											PlayerTurn();
										} else {
											EnemyTurn();
										}
									}
								} else {
									if (Input.GetButtonDown("SNES B Button")) {
										EnemyTurn(false);
									}
								}
							}
						}
						break;
					case eBattleMode.doneSleeping:
						if (Input.GetButtonDown("SNES B Button")) {
							// If this turn is a player's turn...
							if (PlayerNdx() != -1) {
								// If paralyzed, skip turn
								if (StatusEffects.S.CheckIfParalyzed(true, PlayerNdx())) {
									// Skip turn
									NextTurn();

									// If next turn is a player's turn...
									if (PlayerNdx() != -1) {
										PlayerTurn();
									} else {
										EnemyTurn();
									}
								} else {
									PlayerTurn(true, false);
								}
							} else {
								if (StatusEffects.S.CheckIfParalyzed(false, EnemyNdx())) {
									if (Input.GetButtonDown("SNES B Button")) {
										// Skip turn
										NextTurn();

										// If next turn is a player's turn...
										if (PlayerNdx() != -1) {
											PlayerTurn();
										} else {
											EnemyTurn();
										}
									}
								} else {
									if (Input.GetButtonDown("SNES B Button")) {
										EnemyTurn(false);
									}
								}
							}
						}
						break;
					case eBattleMode.isSleeping:
						if (Input.GetButtonDown("SNES B Button")) {
							// This combatant is sleeping, so skip their turn
							NextTurn();

							// If next turn is a player's turn...
							if (PlayerNdx() != -1) {
								PlayerTurn();
							} else {
								EnemyTurn();
							}
						}
						break;
					case eBattleMode.playerDead:
						if (Input.GetButtonDown("SNES B Button")) {
							end.PlayerDeath(deadCombatantNdxs[0]);
						}
						break;
					case eBattleMode.enemyDead:
						if (Input.GetButtonDown("SNES B Button")) {
							end.EnemyDeath(deadCombatantNdxs[0]);
						}
                        break;
					case eBattleMode.partyDeath:
						if (Input.GetButtonDown("SNES B Button")) {
							end.PartyDeath();
						}
						break;
					case eBattleMode.dropItem:
						if (Input.GetButtonDown("SNES B Button")) {
							end.DropItem(droppedItems[0]);
						}
						break;
					case eBattleMode.addExpAndGold:
						if (Input.GetButtonDown("SNES B Button")) {
							end.AddExpAndGold(false);
						}
						break;
					case eBattleMode.addExpAndGoldNoDrops:
						if (Input.GetButtonDown("SNES B Button")) {
							end.AddExpAndGold(true);
						}
						break;
					case eBattleMode.levelUp:
						if (Input.GetButtonDown("SNES B Button")) {
							end.LevelUp(end.membersToLevelUp[0]);
						}
						break;
					case eBattleMode.returnToWorld:
						if (Input.GetButtonDown("SNES B Button")) {
							end.ReturnToWorld();
						}
						break;
				}
			}
		}

		// Separate loop that ignores if dialogue.dialogueFinished
		switch (mode) {
			case eBattleMode.actionButtons:
				// Set buttons' text color
				for (int i = 0; i < playerActions.actionButtonsCS.Count; i++) {
					if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == playerActions.actionButtonsGO[i]) {
						// Set selected button text color	
						playerActions.actionButtonsGO[i].GetComponentInChildren<Text>().color = new Color32(205, 208, 0, 255);
					} else {
						// Set non-selected button text color
						playerActions.actionButtonsGO[i].GetComponentInChildren<Text>().color = new Color32(39, 201, 255, 255);
					}
				}
				break;
			case eBattleMode.qte:
				qte.BlockLoop();
				break;
		}
	}

	public void FixedLoop() {
		switch (mode) {
			case eBattleMode.actionButtons:
			case eBattleMode.canGoBackToFightButton:
			case eBattleMode.selectPartyMember:
			case eBattleMode.itemMenu:
			case eBattleMode.spellMenu:
				if (canUpdate) {
					// Audio: Selection (when a new gameObject is selected)
					Utilities.S.PlayButtonSelectedSFX(ref previousSelectedForAudio);

					canUpdate = false;
				}
				break;
		}

		switch (mode) {
			case eBattleMode.actionButtons:
				// Set Target Cursor Position: player action buttons (fight, defend, item, run, etc.)
				UI.ActionOptionButtonsCursorPosition(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);
				break;
			case eBattleMode.qte:
				qte.FixedLoop();
				break;
			case eBattleMode.itemMenu:
			case eBattleMode.spellMenu:
				// Set buttons' text color
				for (int i = 0; i < UI.optionButtonsCS.Count; i++) {
					if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == UI.optionButtonsGO[i]) {
						// Set selected button text color	
						UI.optionButtonsGO[i].GetComponentInChildren<Text>().color = new Color32(205, 208, 0, 255);
					} else {
						// Set non-selected button text color
						UI.optionButtonsGO[i].GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
					}
				}

				// Set Target Cursor Position: Enemies or Party
				UI.ActionOptionButtonsCursorPosition(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);
				break;
		}
	}
	#endregion

	///////////////////////////////// IMPORT STATS /////////////////////////////////
	public void ImportEnemyStats(List<EnemyStats> eStats, int enemyAmount) {
		// Set enemy amount
		this.enemyAmount = enemyAmount;

		// Clear/Reset Enemy Stats
		enemyStats.Clear();

		for (int i = 0; i < eStats.Count; i++) {
			// Add Enemy Stats
			var clone = Instantiate(eStats[i]);
			enemyStats.Add(clone);
			enemyStats[i].battleID = i + 3;
		}
	}

	public void InitializeBattle() {
		// Ensures Fight button is selected for first player turn
		previousSelectedGameObject = playerActions.actionButtonsGO[0];

		playerActions.ButtonsDisableAll();

		dialogue.Initialize();
		StatusEffects.S.Initialize();

		// Clear dropped items, completed quests, dead combatants
		droppedItems.Clear();
		completedQuestNdxs.Clear();
		deadCombatantNdxs.Clear();

		// Reset playerActions.buttonsCS text color
		Utilities.S.SetTextColor(playerActions.actionButtonsCS, new Color32(39, 201, 255, 255));

		// Reset party member stat frames' animator speed
		for (int i = 0; i < partyStatAnims.Count; i++) {
			partyStatAnims[i].speed = 0;
		}

		// Set Mode
		mode = eBattleMode.actionButtons;

		// Activate display message
		UI.ActivateDisplayMessage();

        // Deactivate Cursors
        Utilities.S.SetActiveList(UI.cursors, false);

		// Deactivate Enemy "Help" Word Bubbles
		Utilities.S.SetActiveList(UI.enemyHelpBubblesGO, false);

		// Reset Exp/Gold to add
		expToAdd = 0;
		goldToAdd = 0;

		// Reset Chance to Run
		chanceToRun = 0.5f;

		// Reset roundNdx
		roundNdx = 1;

		// Get enemy amount, turn order, position & activate enemy sprites
		initiative.SetInitiative();

		// Switch mode (playerTurn or enemyTurn) based off of turnNdx
		if (turnNdx == turnOrder.IndexOf(Party.S.stats[0].battleID) || turnNdx == turnOrder.IndexOf(Party.S.stats[1].battleID) || turnNdx == turnOrder.IndexOf(Party.S.stats[2].battleID)) {
			mode = eBattleMode.playerTurn;
		} else if (turnNdx == turnOrder.IndexOf(enemyStats[0].battleID) || turnNdx == turnOrder.IndexOf(enemyStats[1].battleID) || turnNdx == turnOrder.IndexOf(enemyStats[2].battleID) || turnNdx == turnOrder.IndexOf(enemyStats[3].battleID) || turnNdx == turnOrder.IndexOf(enemyStats[4].battleID)) {
			mode = eBattleMode.enemyTurn;
		}
	}

	public void NextTurn() {
		// Change turnNdx
		if (turnNdx <= ((enemyAmount - 1) + (partyQty))) {
			turnNdx += 1;
		} else {
			turnNdx = 0;
			roundNdx += 1;
		}

        // Deactivate cursors
        Utilities.S.SetActiveList(UI.cursors, false);

		// Reset button to be selected (Fight Button)
		previousSelectedGameObject = playerActions.actionButtonsGO[0];

		// Switch mode (playerTurn or enemyTurn) based off of turnNdx
		if (PlayerNdx() != -1) {
			mode = eBattleMode.playerTurn;

			// Rest option buttons' text color
			for (int i = 0; i < UI.optionButtonsCS.Count; i++) {
				UI.optionButtonsGO[i].GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
			}
		} else {
			mode = eBattleMode.enemyTurn;
		}

  //      string battleIDsString = "turnNdx: " + turnNdx + " || battleIDs: ";
  //      for (int i = 0; i < enemyStats.Count; i++) {
		//	battleIDsString += enemyStats[i].battleID + ", ";
		//}
  //      Debug.Log(battleIDsString);
    }

	// Return current player turn index, otherwise return -1	
	public int PlayerNdx() {
		for (int i = 0; i < Party.S.stats.Count; i++) {
			if (turnNdx == turnOrder.IndexOf(Party.S.stats[i].battleID)) {
				return i;
			}
		}
		return -1;
	}

	// Return current enemy turn index, otherwise return -1																																						
	public int EnemyNdx() {
		for (int i = 0; i < enemyStats.Count; i++) {
			if (turnNdx == turnOrder.IndexOf(enemyStats[i].battleID)) {
				return i;
			}
		}
        return -1;
	}

	public void PlayerTurn(bool setPreviousSelected = true, bool checkForAilment = true) { // if (Input.GetButtonDown ("Submit"))
		// Set mini party member animations
		UI.SetPartyMemberAnim("Idle", "Walk", PlayerNdx());

		TurnHelper(PlayerNdx(), true);

		// If player has status ailment...
		if (checkForAilment) {
			// If poisoned...
			if (StatusEffects.S.CheckIfPoisoned(true, PlayerNdx())) {
				StatusEffects.S.Poisoned(Party.S.stats[PlayerNdx()].name, true, PlayerNdx());
				return;
            } else if (StatusEffects.S.CheckIfParalyzed(true, PlayerNdx())) {
				StatusEffects.S.Paralyzed(Party.S.stats[PlayerNdx()].name, true, PlayerNdx());
				return;
			} else if (StatusEffects.S.CheckIfSleeping(true, PlayerNdx())) {
				StatusEffects.S.Sleeping(Party.S.stats[PlayerNdx()].name, true, PlayerNdx());
				return;
			}
		}

		playerActions.ButtonsInitialInteractable();

		// Set name of player whose turn it is
		UI.playerNameText.text = Party.S.stats[PlayerNdx()].name;

		// Set Selected Button 
		Utilities.S.SetSelectedGO(previousSelectedGameObject);

		// Set previously selected GameObject
		if (setPreviousSelected) {
			previousSelectedForAudio = previousSelectedGameObject;
		}

		UI.firstSlotNdx = 0;

		// Deactivate unused option slots
		UI.DeactivateUnusedSlots(enemyAmount);

		// Assign option slots names
		UI.AssignEnemyNames();

		// Activate action
		UI.ActivatePlayerActionsMenu();

		// Switch Mode
		mode = eBattleMode.actionButtons;
	}

	// Enemy is about to act!
	public void EnemyTurn(bool checkForAilment = true) { // if (Input.GetButtonDown ("Submit"))
		// Set mini party member animations
		UI.SetPartyMemberAnim("Idle", "Idle");

		TurnHelper(EnemyNdx(), false);

        // If enemy has status ailment...
        if (checkForAilment) {
			// If poisoned...
			if (StatusEffects.S.CheckIfPoisoned(false, EnemyNdx())) {
				StatusEffects.S.Poisoned(enemyStats[EnemyNdx()].name, false, EnemyNdx());
				return;
			} else if (StatusEffects.S.CheckIfParalyzed(false, EnemyNdx())) {
				StatusEffects.S.Paralyzed(enemyStats[EnemyNdx()].name, false, EnemyNdx());
				return;
			} else if (StatusEffects.S.CheckIfSleeping(false, EnemyNdx())) {
				StatusEffects.S.Sleeping(enemyStats[EnemyNdx()].name, false, EnemyNdx());
				return;
			}
		}

        // Activate display message
        UI.ActivateDisplayMessage();

		//DisplayText: THE ENEMY IS ABOUT TO ACT!
		dialogue.DisplayText(enemyStats[EnemyNdx()].name + " is about to act!");

		// Anim: Flicker
		enemyAnims[EnemyNdx()].CrossFade("Damage", 0);

		// Switch Mode
		mode = eBattleMode.enemyAction;
	}

	public void TurnHelper(int ndx, bool isPlayerTurn) {
		// If Defended previous turn, remove from Defenders list
		StatusEffects.S.RemoveDefender(isPlayerTurn, ndx);

		// Reset playerActions.buttonsCS text color
		Utilities.S.SetTextColor(playerActions.actionButtonsCS, new Color32(39, 201, 255, 255));

		// Deactivate all enemy sprites
		Utilities.S.SetActiveList(enemySprites, false);

		// Activate/set enemy sprites
		for (int i = 0; i < enemyAmount; i++) {
			enemySprites[i].SetActive(true);
			enemySRends[i].sprite = enemyStats[i].sprite;
		}

		// Set enemy sprites positions
		UI.PositionEnemySprites();

		// Set party member text box sprite frames 
		for (int i = 0; i < partyStatAnims.Count; i++) {
			UI.UpdatePartyStats(i);
		}
	}

	// Reset next turn move index & deactivate help bubble
	public void StopCallingForHelp(int ndx) {
		// Reset next turn move index
		enemyStats[ndx].nextTurnActionNdx = 999;

		enemyStats[ndx].isCallingForHelp = false;

		// Deactivate Enemy "Help" Word Bubble
		UI.enemyHelpBubblesGO[ndx].SetActive(false);
	}
}