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
	public List<Animator> partyAnims;
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

	// Dropped Items
	public List<Item> droppedItems = new List<Item>();

	// Stores index of enemy that's currently being targeted
	public int targetNdx;

	// Stores index of player or enemy that's taking their turn
	public int animNdx;

	public float chanceToRun = 0.5f;

	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

	public GameObject previousSelectedGameObject;

	// Ensures audio is only played once when button is selected
	public GameObject previousSelectedForAudio;

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
					case eBattleMode.itemMenu:
						// On vertical input, scroll the item list when the first or last slot is selected
						UI.ScrollList(Inventory.S.GetItemList().Count, true);

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
							if (enemyStats[EnemyNdx()].nextTurnMoveNdx != 999) {
								// Cache move index
								int moveNdx = enemyStats[EnemyNdx()].nextTurnMoveNdx;

								// Reset this enemy's nextTurnMoveNdx
								enemyStats[EnemyNdx()].nextTurnMoveNdx = 999;

								// ...call previously announced move this turn
								enemyAI.CallEnemyMove(moveNdx);
								// If the enemy didn't announce what move it would perform during its previous turn...
							} else {
								// ...let its AI dictate what move to perform
								enemyAI.EnemyAI(enemyStats[EnemyNdx()].id);
							}
						}
						break;
					case eBattleMode.statusAilment:
						if (Input.GetButtonDown("SNES B Button")) {
							// If this turn is a player's turn...
							if (PlayerNdx() != -1) {
								// If paralyzed or sleeping...
								if (StatusEffects.S.CheckIfParalyzed(true, PlayerNdx()) ||
									StatusEffects.S.CheckIfSleeping(true, PlayerNdx())) {
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
								// If paralyzed or sleeping...
								if (StatusEffects.S.CheckIfParalyzed(false, EnemyNdx()) ||
									StatusEffects.S.CheckIfSleeping(false, EnemyNdx())) {
									// Skip turn
									NextTurn();

									// If next turn is a player's turn...
									if (PlayerNdx() != -1) {
										PlayerTurn();
									} else {
										EnemyTurn();
									}
								} else {
									EnemyTurn(false);
								}
							}
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
		}
	}

	public void FixedLoop() {
		switch (mode) {
			case eBattleMode.actionButtons:
				// Set Target Cursor Position: player action buttons (fight, defend, item, run, etc.)
				UI.ActionOptionButtonsCursorPosition(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);

				if (canUpdate) {
					// Audio: Selection (when a new gameObject is selected)
					Utilities.S.PlayButtonSelectedSFX(ref previousSelectedForAudio);

					canUpdate = false;
				}
				break;
			case eBattleMode.canGoBackToFightButton:
			case eBattleMode.selectPartyMember:
				if (canUpdate) {
					// Audio: Selection (when a new gameObject is selected)
					Utilities.S.PlayButtonSelectedSFX(ref previousSelectedForAudio);

					canUpdate = false;
				}
				break;
			case eBattleMode.itemMenu:
			case eBattleMode.spellMenu:
				if (canUpdate) {
					// Audio: Selection (when a new gameObject is selected)
					Utilities.S.PlayButtonSelectedSFX(ref previousSelectedForAudio);

					canUpdate = false;
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

	// Called in RPGLevelManager.LoadSettings when Scene changes to Battle
	public void InitializeBattle() {
		// Ensures Fight button is selected for first player turn
		previousSelectedGameObject = playerActions.actionButtonsGO[0];

		playerActions.ButtonsDisableAll();

		dialogue.Initialize();
		StatusEffects.S.Initialize();

		// Clear Dropped Items
		droppedItems.Clear();

		// Reset playerActions.buttonsCS text color
		Utilities.S.SetTextColor(playerActions.actionButtonsCS, new Color32(39, 201, 255, 255));

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
		} else {
			mode = eBattleMode.enemyTurn;
		}

        string battleIDsString = "turnNdx: " + turnNdx + " || battleIDs: ";
        for (int i = 0; i < enemyStats.Count; i++) {
			//battleIDsString += turnOrder.IndexOf(enemyStats[i].battleID) + ", ";
			battleIDsString += enemyStats[i].battleID + ", ";
		}
        Debug.Log(battleIDsString);
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
		TurnHelper(PlayerNdx(), true);

		// If player has status ailment...
		if (checkForAilment) {
			if (HasAilment(Party.S.stats[PlayerNdx()].name, true, PlayerNdx())) {
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
		TurnHelper(EnemyNdx(), false);

        // If enemy has status ailment...
        if (checkForAilment) {
            if (HasAilment(enemyStats[EnemyNdx()].name, false, EnemyNdx())) {
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
	}

	bool HasAilment(string name, bool isPlayer, int ndx) {
		// If poisoned...
		if (StatusEffects.S.CheckIfPoisoned(isPlayer, ndx)) {
			StatusEffects.S.Poisoned(name, isPlayer, ndx);
			return true;
		}

		// If paralyzed...
		if (StatusEffects.S.CheckIfParalyzed(isPlayer, ndx)) {
			StatusEffects.S.Paralyzed(name, isPlayer, ndx);
			return true;
		}

		// If sleeping...
		if (StatusEffects.S.CheckIfSleeping(isPlayer, ndx)) {
			StatusEffects.S.Sleeping(name, isPlayer, ndx);
			return true;
		}
		return false;
	}

	// Reset next turn move index & deactivate help bubble
	public void StopCallingForHelp(int ndx) {
		// Reset next turn move index
		enemyStats[ndx].nextTurnMoveNdx = 999;

		enemyStats[ndx].isCallingForHelp = false;

		// Deactivate Enemy "Help" Word Bubble
		UI.enemyHelpBubblesGO[ndx].SetActive(false);
	}
}