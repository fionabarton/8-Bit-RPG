using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// During battle handles what happens when buttons are pressed
/// (Fight, Spell, Item, Run, Party Members, & Enemies)
/// </summary>
public class BattlePlayerActions : MonoBehaviour {
	[Header("Set in Inspector")]
	// Fight, Spell, Item, Defend, Run
	public List<GameObject> actionButtonsGO;
	public List<Button> actionButtonsCS;

	private Battle _;

	void Start() {
		_ = Battle.S;
	}

	// choose an enemy to attack
	public void FightButton(bool playSound = false) {
		// Cache Selected Gameobject (Fight Button) 
		_.previousSelectedGameObject = actionButtonsGO[0];

		ButtonsDisableAll();

		Utilities.S.ButtonsInteractable(_.UI.enemyButtonsCS, true);
		Utilities.S.ButtonsInteractable(_.UI.partyNameButtonsCS, false);

		_.UI.firstSlotNdx = 0;

		// Deactivate all enemy sprite buttons
		Utilities.S.SetActiveList(_.UI.enemyButtonsGO, false);

		// Activate active enemy sprite buttons
		for (int i = 0; i < _.enemyAmount; i++) {
			_.UI.enemyButtonsGO[i].SetActive(true);
		}

		_.UI.actionOptionsButtonsCursor.SetActive(false);

		// Set selected gameObject
		Utilities.S.SetSelectedGO(_.UI.enemyButtonsGO[0]);

		// Audio: Confirm
		if (playSound) {
			AudioManager.S.PlaySFX(eSoundName.confirm);
		}

        if (!EquipMenu.S.playerEquipment[_.PlayerNdx()][0].multipleTargets) {
			// Assign option slots effect
			_.UI.AssignAttackEnemyEffect();

			// Switch Mode
			_.mode = eBattleMode.canGoBackToFightButton;

			// Set the first and last button’s navigation 
			Utilities.S.SetHorizontalButtonsNavigation(_.UI.enemyButtonsCS, _.enemyAmount);
		} else {
			// Assign option slots effect
			_.UI.AssignAttackAllEnemiesEffect();

			// Switch Mode
			_.mode = eBattleMode.selectAll;
			_.UI.TargetAllEnemies();
		}
    }

	public void ClickedAttackEnemy(int ndx) {
		_.targetNdx = ndx;
		_.animNdx = _.PlayerNdx();

		AttackEnemy(ndx);

		// Remove all listeners
		_.UI.RemoveAllListeners();
	}

	public void AttackEnemy(int ndx) {
		// Calculate Attack Damage
		_.stats.GetPhysicalAttackDamage(Party.S.stats[_.PlayerNdx()].LVL,
								Party.S.stats[_.PlayerNdx()].STR, Party.S.stats[_.PlayerNdx()].AGI,
								_.enemyStats[ndx].DEF, _.enemyStats[ndx].AGI,
								Party.S.stats[_.PlayerNdx()].name, _.enemyStats[ndx].name,
								_.enemyStats[ndx].HP, false, ndx);

		// Subtract Enemy Health
		GameManager.S.SubtractEnemyHP(ndx, _.attackDamage);

		ButtonsDisableAll();
		Utilities.S.ButtonsInteractable(_.UI.enemyButtonsCS, false);

		// Flicker Enemy Anim 
		_.enemyAnims[ndx].CrossFade("Damage", 0);

		GameManager.S.InstantiateFloatingScore(_.enemySprites[ndx], _.attackDamage.ToString(), Color.red, -2f);

		AudioManager.S.PlayRandomDamageSFX();

		// Enemy Death or Next Turn
		if (_.enemyStats[ndx].HP < 1) {
			_.end.EnemyDeath(ndx);
		} else {
			_.NextTurn();
		}
	}

	public void AttackAllEnemies() {
		// Remove all listeners
		_.UI.RemoveAllListeners();

		// Reset Attack Damage
		_.attackDamage = 0;

		// Activate display message
		_.UI.ActivateDisplayMessage();

		// 5% chance to Miss/Dodge...
		if (Random.value <= 0.05f) {
			// Set mini party member animations
			_.UI.SetPartyMemberAnim("Idle", "Fail", _.PlayerNdx());

			if (Random.value <= 0.5f) {
				_.dialogue.DisplayText(Party.S.stats[_.PlayerNdx()].name + " attempted to attack all the bad guys... but missed!");
			} else {
				_.dialogue.DisplayText(Party.S.stats[_.PlayerNdx()].name + " missed the mark! All the bad guys dodged out of the way!");
			}

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);

			_.NextTurn();
		} else {
			// Set mini party member animations
			_.UI.SetPartyMemberAnim("Idle", "Success", _.PlayerNdx());

			List<int> deadEnemies = new List<int>();

			// Cache AttackDamage. When more than one Defender, prevents splitting it in 1/2 more than once.
			int tAttackDamage = _.attackDamage;

			// Used to Calculate AVERAGE Damage
			int totalAttackDamage = 0;

			int attackerLVL = Party.S.stats[_.PlayerNdx()].LVL;

			// 5% chance for Critical Hit
			// Doubles the amount of damage dice to be rolled
			if (Random.value < 0.05f) {
				attackerLVL *= 2;
			}

			// Loop through enemies
			for (int i = 0; i < _.enemyAmount; i++) {
				// For each level, roll one die & add its value to attackDamage
				for (int j = 0; j < attackerLVL; j++) {
					_.attackDamage += Random.Range(1, 4);
				}

				// Apply modifiers (attacker's STR & defenders DEF)
				_.attackDamage += Party.S.stats[_.PlayerNdx()].STR;
				_.attackDamage -= _.enemyStats[i].DEF;

				// If DEFENDING, cut AttackDamage in HALF
				StatusEffects.S.CheckIfDefending(false, i);

				if (_.attackDamage < 0) {
					_.attackDamage = 0;
				}

				// Subtract Enemy Heath
				GameManager.S.SubtractEnemyHP(i, _.attackDamage);

				// Add to to TotalAttackDamage (Used to Calculate AVERAGE Damage)
				totalAttackDamage += _.attackDamage;

				// Display Floating Score
				GameManager.S.InstantiateFloatingScore(_.enemySprites[i], _.attackDamage.ToString(), Color.red, -2f);

				// Shake Enemy Anim
				if (!_.enemyStats[i].isDead) {
					_.enemyAnims[i].CrossFade("Damage", 0);
				}

				// If DEFENDING, Reset AttackDamage for next Enemy
				_.attackDamage = tAttackDamage;

				// If Enemy HP < 0... DEAD!
				if (_.enemyStats[i].HP < 1 && !_.enemyStats[i].isDead) {
					deadEnemies.Add(i);
				}
			}

			// If no one is killed...
			if (deadEnemies.Count <= 0) {
				_.dialogue.DisplayText(Party.S.stats[_.PlayerNdx()].name + " got'em!\nHit ALL Enemies for an average of " + Utilities.S.CalculateAverage(totalAttackDamage, _.enemyAmount) + " HP!");

				// Audio: Fireblast
				AudioManager.S.PlaySFX(eSoundName.fireblast);

				_.NextTurn();
			} else {
				Spells.S.battle.EnemiesDeath(deadEnemies, totalAttackDamage, Party.S.stats[_.PlayerNdx()].name + " got'em!");
			}
		}
	}

	// go back to player action buttons (fight, defend, item, run, etc.)
	public void GoBackToActionButtons() { // if (Input.GetButtonDown ("Cancel"))
		// Remove listeners
		_.UI.RemoveAllListeners();

		// Rest option buttons' text color
		for (int i = 0; i < _.UI.optionButtonsCS.Count; i++) {
			_.UI.optionButtonsGO[i].GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
		}

		_.UI.actionOptionsButtonsCursor.SetActive(true);
		Utilities.S.SetActiveList(_.UI.partyNameButtonsCursors, false);

		// Deactivate gear menu
        if (EquipMenu.S.gameObject.activeInHierarchy) {
			EquipMenu.S.Deactivate();
        }

		_.PlayerTurn(true, false);

		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.deny);
	}

	// Run Button
	// 50% chance to run from battle. For each failed attempt, chance increases by 12.5%
	public void RunButton() {
		ButtonsDisableAll();

		// Activate display message
		_.UI.ActivateDisplayMessage();

		// Cache Selected Gameobject (Run Button) 
		Battle.S.previousSelectedGameObject = actionButtonsGO[4];

		// Not a "boss battle", so the party can attempt to run
		if (_.enemyStats[0].questNdx == -1) {
			if (Random.value < _.chanceToRun) {     // || Stats.S.LVL[0] - enemyStats[0].LVL >= 5
				Utilities.S.SetActiveList(_.UI.cursors, false);

				// Display Text
				if (_.partyQty >= 1) {
					_.dialogue.DisplayText("The party has fled the battle!");
				} else {
					_.dialogue.DisplayText(Party.S.stats[_.PlayerNdx()].name + " has fled the battle!");
				};

				// Return to Overworld
				_.end.ReturnToWorldDelay();

				// Audio: Run
				AudioManager.S.PlaySFX(eSoundName.run);
			} else {
				_.dialogue.DisplayText(_.enemyStats[0].name + " has blocked the path!");
				_.NextTurn();

				// Increase chance to run
				_.chanceToRun += 0.125f;

				// Audio: Deny
				AudioManager.S.PlaySFX(eSoundName.deny);
			}
		} else { // It's a "boss battle", so the party cannot run
			_.mode = eBattleMode.triedToRunFromBoss;

			Utilities.S.SetActiveList(_.UI.cursors, false);
			_.dialogue.DisplayText(_.enemyStats[0].name + " is deadly serious...\n...there is ZERO chance of running away!");

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}
	}
	// Defend Button
	// Reduces attack damage by 50%
	public void DefendButton() {
		ButtonsDisableAll();

		// Defend until next turn
		StatusEffects.S.AddDefender(true, _.PlayerNdx());

		// Activate display message
		_.UI.ActivateDisplayMessage();

		_.dialogue.DisplayText(Party.S.stats[_.PlayerNdx()].name + " defends themself until their next turn!");

		_.NextTurn();
	}

	// Spell Button
	public void SpellButton() {
		// Cache Selected Gameobject (Spell Button) 
		Battle.S.previousSelectedGameObject = actionButtonsGO[1];

		if (Party.S.stats[_.PlayerNdx()].spellNdx > 0) {
			ButtonsInteractable(false, false, false, false, false, false, true, true, true, true, true);
			Utilities.S.ButtonsInteractable(_.UI.enemyButtonsCS, false);
			Utilities.S.ButtonsInteractable(_.UI.partyNameButtonsCS, false);

			_.UI.firstSlotNdx = 0;
			_.UI.firstOrLastSlotSelected = true;

			// Set selected gameObject
			Utilities.S.SetSelectedGO(_.UI.optionButtonsGO[0]);
			_.UI.previousSelectedOptionButtonGO = _.UI.optionButtonsGO[0];

			// Deactivate unused option slots
			_.UI.DeactivateUnusedSlots(Party.S.stats[_.PlayerNdx()].spellNdx);

			// Assign option slots names
			_.UI.AssignSpellNames();

			// Assign option slots effect
			_.UI.AssignSpellEffect();

			// Set the first and last button’s navigation 
			_.UI.SetOptionButtonsNavigation(Party.S.stats[_.PlayerNdx()].spellNdx);

			// Audio: Confirm
			AudioManager.S.PlaySFX(eSoundName.confirm);

			// Switch Mode
			_.mode = eBattleMode.spellMenu;
		} else {
			// Activate display message
			_.UI.ActivateDisplayMessage();

			// Knows no Spells, go back to Player Turn
			_.dialogue.DisplayText(Party.S.stats[_.PlayerNdx()].name + " doesn't know any spells!");

			// Switch Mode
			_.mode = eBattleMode.playerTurn;

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}
	}
	// Item Button
	public void ItemButton() {
		// Cache Selected Gameobject (Item Button) 
		Battle.S.previousSelectedGameObject = actionButtonsGO[2];

		// If Player has an Item 
		if (Inventory.S.GetItemList().Count > 0) {
			ButtonsInteractable(false, false, false, false, false, false, true, true, true, true, true);
			Utilities.S.ButtonsInteractable(_.UI.enemyButtonsCS, false);
			Utilities.S.ButtonsInteractable(_.UI.partyNameButtonsCS, false);

			_.UI.firstSlotNdx = 0;
			_.UI.firstOrLastSlotSelected = true;

			// Set selected gameObject
			Utilities.S.SetSelectedGO(_.UI.optionButtonsGO[0]);
			_.UI.previousSelectedOptionButtonGO = _.UI.optionButtonsGO[0];

			// Deactivate unused option slots
			_.UI.DeactivateUnusedSlots(Inventory.S.GetItemList().Count);

			// Assign option slots names
			_.UI.AssignItemNames();

			// Assign option slots effect
			_.UI.AssignItemEffect();

			// Set the first and last button’s navigation 
			_.UI.SetOptionButtonsNavigation(Inventory.S.GetItemList().Count);

			// Audio: Confirm
			AudioManager.S.PlaySFX(eSoundName.confirm);

			// Switch Mode
			_.mode = eBattleMode.itemMenu;
		} else {
			// Activate display message
			_.UI.ActivateDisplayMessage();

			// Has no Items, go back to Player Turn 
			_.dialogue.DisplayText(Party.S.stats[_.PlayerNdx()].name + " has no items!");

			// Switch Mode
			_.mode = eBattleMode.playerTurn;

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}
	}

	public void GearButton() {
		// Cache Selected Gameobject (Gear Button) 
		Battle.S.previousSelectedGameObject = actionButtonsGO[4];

		ButtonsDisableAll();
		Utilities.S.ButtonsInteractable(_.UI.optionButtonsCS, false);
		Utilities.S.ButtonsInteractable(_.UI.enemyButtonsCS, false);
		Utilities.S.ButtonsInteractable(_.UI.partyNameButtonsCS, true);

		_.UI.RemoveAllListeners();

		EquipMenu.S.Activate(50);

		// Activate display message
		_.UI.ActivateDisplayMessage();

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);

		// Switch Mode
		_.mode = eBattleMode.gearMenu;
	}

	public void ButtonsInteractable(bool fight, bool spell, bool item, bool defend, bool gear, bool run, bool oButton1, bool oButton2, bool oButton3, bool oButton4, bool oButton5) {
		actionButtonsCS[0].interactable = fight;
		actionButtonsCS[1].interactable = spell;
		actionButtonsCS[2].interactable = item;
		actionButtonsCS[3].interactable = defend;
		actionButtonsCS[4].interactable = gear;
		actionButtonsCS[5].interactable = run;

		_.UI.optionButtonsCS[0].interactable = oButton1;
		_.UI.optionButtonsCS[1].interactable = oButton1;
		_.UI.optionButtonsCS[2].interactable = oButton1;
		_.UI.optionButtonsCS[3].interactable = oButton1;
		_.UI.optionButtonsCS[4].interactable = oButton1;
	}

	public void ButtonsInitialInteractable() { ButtonsInteractable(true, true, true, true, true, true, false, false, false, false, false); }

	public void ButtonsDisableAll() { ButtonsInteractable(false, false, false, false, false, false, false, false, false, false, false); }
}