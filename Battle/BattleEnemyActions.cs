using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemyActions : MonoBehaviour {
	private Battle _;

	void Start() {
		_ = Battle.S;
	}

	// Index = 0
	// Attack ONE Party Member
	public void Attack() {
		// Calculate Attack Damage
		_.stats.GetPhysicalAttackDamage(_.enemyStats[_.EnemyNdx()].LVL,
									   _.enemyStats[_.EnemyNdx()].STR, _.enemyStats[_.EnemyNdx()].AGI,
									   Party.S.stats[_.targetNdx].DEF, Party.S.stats[_.targetNdx].AGI,
									   _.enemyStats[_.EnemyNdx()].name, Party.S.stats[_.targetNdx].name,
										Party.S.stats[_.targetNdx].HP, true, _.targetNdx);

		// Subtract Player Health
		GameManager.S.SubtractPlayerHP(_.targetNdx, _.attackDamage, true);

		// Play attack animations, SFX, and spawn objects
		PlaySingleAttackAnimsAndSFX(_.targetNdx);
		//StartCoroutine("MultiAttack");

		// Player Death or Next Turn
		if (Party.S.stats[_.targetNdx].HP < 1) {
			// Add player index to list of dead combatants
			_.deadCombatantNdxs.Add(_.targetNdx);

			// Player dead mode
			_.mode = eBattleMode.playerDead;
		} else {
            if (_.qteEnabled) {
				// If not sleeping or paralyzed...attempt to block!
				if (!StatusEffects.S.CheckIfParalyzed(true, _.targetNdx) &&
					!StatusEffects.S.CheckIfSleeping(true, _.targetNdx)) {
					// Index of the party member that is blocking
					_.qte.blockerNdx = _.targetNdx;

					// Set qteType to Block
					_.qte.qteType = 4;

					// Enable progress bar/timer 
					_.qte.Initialize();

					// Set battleMode to QTE
					_.mode = eBattleMode.qte;

					// Deactivate enemy sprites
					for (int i = 0; i < _.enemyAmount; i++) {
						_.enemySprites[i].SetActive(false);
					}
				} else {
					// Deactivate Battle Text
					_.dialogue.displayMessageTextTop.gameObject.transform.parent.gameObject.SetActive(false);

					_.NextTurn();
				}
			} else {
				_.NextTurn();
			}
		}
	}

	public IEnumerator MultiAttack() {
		PlaySingleAttackAnimsAndSFX(0);
		yield return new WaitForSeconds(0.1f);
		PlaySingleAttackAnimsAndSFX(1);
		yield return new WaitForSeconds(0.1f);
		PlaySingleAttackAnimsAndSFX(0);
		yield return new WaitForSeconds(0.1f);
		PlaySingleAttackAnimsAndSFX(1);
		yield return new WaitForSeconds(0.1f);
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Index = 1
	// Defend
	public void Defend() {
		StatusEffects.S.AddDefender(false, _.EnemyNdx());

		_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " defends themself until their next turn!");

		_.NextTurn();
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Index = 2
	// Run
	public void Run() {
		_.end.EnemyRun(_.EnemyNdx());
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Index = 3
	// Stunned
	public void Stunned() {
		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.deny);

		_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " is stunned and doesn't move!\nWhat a rube!");
		_.NextTurn();
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Index = 4
	// Heal Spell
	public void AttemptHealSpell(int mpCost = 3) {
		// Enough MP
		if (_.enemyStats[_.EnemyNdx()].MP >= mpCost) {
			ColorScreen.S.PlayClip("Swell", 1);
		} else {
			// Not enough MP
			_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " attempts to cast a Heal Spell...\n...But doesn't have enough MP to do so!");

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);

			_.NextTurn();
		}
	}

	// Called after BattleBlackScreen "Swell" animation
	public void HealSpell(int mpCost = 3, int minVal = 30, int maxVal = 45, eSoundName sfx = eSoundName.buff1) {
		// Subtract Spell cost from Enemy's MP
		_.enemyStats[_.EnemyNdx()].MP -= mpCost;

		// Get amount and max amount to heal
		int amountToHeal = Random.Range(minVal, maxVal);
		int maxAmountToHeal = _.enemyStats[_.targetNdx].maxHP - _.enemyStats[_.targetNdx].HP;
		// Add Enemy's WIS to Heal Amount
		amountToHeal += _.enemyStats[_.targetNdx].WIS;

		// Add 30-45 HP to TARGET Player's HP
		GameManager.S.AddEnemyHP(_.targetNdx, amountToHeal);

		// Display Text
		if (amountToHeal >= maxAmountToHeal) {
			if (_.targetNdx == _.EnemyNdx()) {
				_.dialogue.DisplayText(_.enemyStats[_.targetNdx].name + " casts a Heal Spell!\nHealed itself back to Max HP!");
			} else {
				_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " casts a Heal Spell!\nHealed " + _.enemyStats[_.targetNdx].name + " back to Max HP!");
			}

			// Prevents Floating Score being higher than the acutal amount healed
			amountToHeal = maxAmountToHeal;
		} else {
			if (_.targetNdx == _.EnemyNdx()) {
				_.dialogue.DisplayText(_.enemyStats[_.targetNdx].name + " casts a Heal Spell!\nHealed itself for " + amountToHeal + " HP!");
			} else {
				_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " casts a Heal Spell!\nHealed " + _.enemyStats[_.targetNdx].name + " for " + amountToHeal + " HP!");
			}
		}

		// Display Floating Score
		GameManager.S.InstantiateFloatingScore(_.enemySprites[_.targetNdx], amountToHeal.ToString(), Color.green, -2f);

		// Flicker Enemy Anim 
		_.enemyAnims[_.targetNdx].CrossFade("Damage", 0);

		// Audio: Buff
		AudioManager.S.PlaySFX(sfx);

		_.NextTurn();
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Index = 8
	// Single Attack
	public void AttemptAttackSingle(int mpCost = 3) {
		// Enough MP
		if (_.enemyStats[_.EnemyNdx()].MP >= mpCost) {
			ColorScreen.S.PlayClip("Flicker", 3);
		} else {
			// Not enough MP
			_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " attempts to cast Fireball...\n...But doesn't have enough MP to do so!");

			// Set mini party member animations
			_.UI.SetPartyMemberAnim("Success");

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);

			_.NextTurn();
		}
	}

	public void AttackSingle(int mpCost = 3, int minVal = 8, int maxVal = 12, eSoundName sfx = eSoundName.fireball) {
		// Subtract Enemy MP
		_.enemyStats[_.EnemyNdx()].MP -= mpCost;

		// 5% chance to Miss/Dodge...
		// ...but 10% chance if Defender WIS is more than Attacker's 
		if (Random.value <= 0.05f || (_.enemyStats[_.EnemyNdx()].WIS > Party.S.stats[0].WIS && Random.value < 0.10f)) {
			if (Random.value <= 0.5f) {
				_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " attempted to cast Fireball... but missed the party completely!");
			} else {
				_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " cast Fireball, but the party deftly dodged out of the way!");
			}

			// Set mini party member animations
			_.UI.SetPartyMemberAnim("Success");

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);

			_.NextTurn();
		} else {
			// Subtract HP
			_.attackDamage = Random.Range(minVal, maxVal);

			// Subtract Player Health
			GameManager.S.SubtractPlayerHP(_.targetNdx, _.attackDamage, true);

			// Play attack animations, SFX, and spawn objects
			PlaySingleAttackAnimsAndSFX(_.targetNdx);

			_.dialogue.DisplayText("Used Fireball Spell!\nHit " + Party.S.stats[_.targetNdx].name + " for " + _.attackDamage + " HP!");

			// Audio: Fireblast
			AudioManager.S.PlaySFX(sfx);

			// Player Death or Next Turn
			if (Party.S.stats[_.targetNdx].HP < 1) {
				// Add player index to list of dead combatants
				_.deadCombatantNdxs.Add(_.targetNdx);

				// Player dead mode
				_.mode = eBattleMode.playerDead;
			} else {
				_.NextTurn();
			}
		}
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Index = 5
	// Attack All
	public void AttemptAttackAll(int mpCost = 3) {
		// Enough MP
		if (_.enemyStats[_.EnemyNdx()].MP >= mpCost) {
			ColorScreen.S.PlayClip("Flicker", 2);
		} else {
			// Not enough MP
			_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " attempts to cast Fireblast...\n...But doesn't have enough MP to do so!");

			// Set mini party member animations
			_.UI.SetPartyMemberAnim("Success");

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);

			_.NextTurn();
		}
	}

	public void AttackAll(int mpCost = 3, int minVal = 10, int maxVal = 15, eSoundName sfx = eSoundName.fireblast) {
		// Subtract Enemy MP
		_.enemyStats[_.EnemyNdx()].MP -= mpCost;

		// 5% chance to Miss/Dodge...
		// ...but 10% chance if Defender WIS is more than Attacker's 
		if (Random.value <= 0.05f || (_.enemyStats[_.EnemyNdx()].WIS > Party.S.stats[0].WIS && Random.value < 0.10f)) {
			if (Random.value <= 0.5f) {
				_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " attempted to cast Fireblast... but missed the party completely!");
			} else {
				_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " cast Fireblast, but the party deftly dodged out of the way!");
			}

			// Set mini party member animations
			_.UI.SetPartyMemberAnim("Success");

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);

			_.NextTurn();
		} else {
			// Shake Screen Anim
			Battle.S.battleUIAnim.CrossFade("BattleUI_Shake", 0);

			// Set mini party member animations
			_.UI.SetPartyMemberAnim("Damage");

			List<int> deadPlayers = new List<int>();

			// Subtract HP
			_.attackDamage = Random.Range(minVal, maxVal);

			// Add Enemy's WIS to Damage
			_.attackDamage += _.enemyStats[_.EnemyNdx()].WIS;

			// Cache AttackDamage. When more than one Defender, prevents splitting it in 1/2 more than once.
			int tAttackDamage = _.attackDamage;

			// Used to Calculate AVERAGE Damage
			int totalAttackDamage = 0;

			// Loop through Players
			for (int i = 0; i < (Party.S.partyNdx + 1); i++) {
				// Subtract Player's DEF from Damage
				_.attackDamage -= Party.S.stats[i].DEF;

				// If DEFENDING, cut AttackDamage in HALF
				StatusEffects.S.CheckIfDefending(true, i);

				if (_.attackDamage < 0) {
					_.attackDamage = 0;
				}

				// Subtract Player Health
				GameManager.S.SubtractPlayerHP(i, _.attackDamage, true);

				// Add to to TotalAttackDamage (Used to Calculate AVERAGE Damage)
				totalAttackDamage += _.attackDamage;

				// Shake Enemy 1, 2, & 3's Anim
				if (!_.playerDead[i]) {
					// If player doesn't have a status ailment...
					if (!StatusEffects.S.HasStatusAilment(true, i)) {
						// Animation: Player Damage
						//_.playerAnimator[i].CrossFade("Damage", 0);
					}

					// Display Floating Score
					GameManager.S.InstantiateFloatingScore(_.UI.partyStartsTextBoxSprite[i].gameObject, _.attackDamage.ToString(), Color.red);
				}

				// If DEFENDING, Reset AttackDamage for next Enemy
				_.attackDamage = tAttackDamage;

				// If Player HP < 0, DEAD!
				if (Party.S.stats[i].HP < 1 && !_.playerDead[i]) {
					deadPlayers.Add(i);
				}
			}

			_.dialogue.DisplayText("Used Fireblast Spell!\nHit ENTIRE party for an average of " + Utilities.S.CalculateAverage(totalAttackDamage, (Party.S.partyNdx + 1)) + " HP!");

			// Audio: Fireblast
			AudioManager.S.PlaySFX(sfx);

			// If no one is killed...
			if (deadPlayers.Count <= 0) {
				_.NextTurn();
			} else {
				PlayersDeath(deadPlayers, totalAttackDamage);
			}
		}
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Index = 7
	// Call for backup next turn
	public void CallForBackupNextTurn() {
		_.enemyStats[_.EnemyNdx()].nextTurnActionNdx = 6;

		// Activate Enemy "Help" Word Bubble
		_.UI.enemyHelpBubblesGO[_.EnemyNdx()].SetActive(true);

		_.enemyStats[_.EnemyNdx()].isCallingForHelp = true;

		_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " is getting ready to call for help!");
		_.NextTurn();
	}

	// Index = 6
	// Call for backup 
	public void CallForBackup() {
		// Deactivate Enemy "Help" Word Bubble, etc.
		_.StopCallingForHelp(_.EnemyNdx());

		if (_.enemyAmount < 5) {
			CallForBackupHelper(_.enemyAmount);
		} else {
			_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " called for backup...\n...but no one came!");

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}

		_.NextTurn();
	}

	public void CallForBackupHelper(int enemyNdx, eSoundName sfx = eSoundName.run) {
		// Display Text
		_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " called for backup...\n...and someone came!");

		// Add to EnemyAmount 
		_.enemyAmount += 1;
		_.totalEnemyAmount += 1;

		// Get all enemy battleIDs
		List<int> usedBattleIDs = new List<int>();
		for (int i = 0; i < _.enemyStats.Count; i++) {
			usedBattleIDs.Add(_.enemyStats[i].battleID);
		}

		// Get unused battleID
		int unusedBattleID = -1;
		if (!usedBattleIDs.Contains(3)) {
			unusedBattleID = 3;
		} else if (!usedBattleIDs.Contains(4)) {
			unusedBattleID = 4;
		} else if (!usedBattleIDs.Contains(5)) {
			unusedBattleID = 5;
		} else if (!usedBattleIDs.Contains(6)) {
			unusedBattleID = 6;
		} else if (!usedBattleIDs.Contains(7)) {
			unusedBattleID = 7;
		}

		// Clone and add EnemyStats
		var clone = Instantiate(Player.S.enemyStats[enemyNdx]);
		_.enemyStats.Add(clone);

		int ndx = _.enemyStats.Count - 1;

		// Give enemy a unique battleID
		clone.battleID = unusedBattleID; //_.totalEnemyAmount + 2;

		// Add to Turn Order
		_.turnOrder.Add(clone.battleID);

		// Gold/EXP payout
		_.expToAdd += clone.EXP;
		_.goldToAdd += clone.Gold;

		// Activate/Deactivate Enemy Buttons, Stats, Sprites
		_.enemySprites[ndx].SetActive(true);
		_.enemySRends[ndx].sprite = clone.sprite;

		// Enable/Update Health Bars
		//ProgressBars.S.enemyHealthBarsCS[enemyNdx].transform.parent.gameObject.SetActive(true);
		//ProgressBars.S.enemyHealthBarsCS[enemyNdx].UpdateBar(_.enemyStats[enemyNdx].HP, _.enemyStats[enemyNdx].maxHP);

		// Set enemy sprites positions
		_.UI.PositionEnemySprites();

		// Audio: Run
		AudioManager.S.PlaySFX(sfx);
	}

	public void PlayersDeath(List<int> deadPlayers, int totalAttackDamage) {
		for (int i = 0; i < deadPlayers.Count; i++) {
			// Add player indexes to list of dead combatants
			_.deadCombatantNdxs.Add(deadPlayers[i]);
		}

		// Player dead mode
		_.mode = eBattleMode.playerDead;
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Index = 9
	// Charge
	public void Charge(eSoundName sfx = eSoundName.buff2) {
		// Audio: Buff
		AudioManager.S.PlaySFX(sfx);

		_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " is getting ready to do something cool...\n...what could it be?!");
		_.NextTurn();
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Index = 10
	// Poison
	public void Poison() {
		// Poison party member
		StatusEffects.S.AddPoisoned(_.targetNdx);

		// Play attack animations, SFX, and spawn objects
		PlaySingleAttackAnimsAndSFX(_.targetNdx, true, false);
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Index = 11
	// Paralyze
	public void Paralyze() {
		// Paralyze party member
		StatusEffects.S.AddParalyzed(_.targetNdx);

		// Play attack animations, SFX, and spawn objects
		PlaySingleAttackAnimsAndSFX(_.targetNdx, true, false);
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Index = 12
	// Sleep
	public void Sleep() {
		// Put party member to sleep
		StatusEffects.S.AddSleeping(_.targetNdx);

		// Play attack animations, SFX, and spawn objects
		PlaySingleAttackAnimsAndSFX(_.targetNdx, true, false);
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Index = 13
	// Steal
	public void AttemptSteal() {
		_.targetNdx = _.stats.GetRandomPlayerNdx();
		ColorScreen.S.PlayClip("Flicker", 8);
	}

	public void Steal() {
		if (Inventory.S.GetItemList().Count != 0) {
			// 50% chance to miss...
			if (Random.value <= 0.5f) {
				// Get random party item index and item
				int itemNdx = Random.Range(0, Inventory.S.GetItemList().Count);
				Item tItem = Inventory.S.GetItemList()[itemNdx];

				if (tItem.type != eItemType.Important) {
					// Remove item from party inventory
					Inventory.S.RemoveItemFromInventory(tItem);

					// Add item to stolen items inventory
					_.enemyStats[_.EnemyNdx()].stolenItems.Add(tItem);
					_.enemyStats[_.EnemyNdx()].amountToSteal += 1;

					// Play attack animations, SFX, and spawn objects
					PlaySingleAttackAnimsAndSFX(_.targetNdx, true, false);

					// Display text: item stolen
					_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " swiped a " + tItem.name + " from " + Party.S.stats[_.targetNdx].name + ".\n" + WordManager.S.GetRandomInterjection() + "!");
				} else {
					// Set mini party member animations
					_.UI.SetPartyMemberAnim("Success");

					// Audio: Deny
					AudioManager.S.PlaySFX(eSoundName.deny);

					// Display text: can't steal an important item
					_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " attempted to steal a " + tItem.name + " from " + Party.S.stats[_.targetNdx].name + "...\n...but it can't be stolen!\n" + WordManager.S.GetRandomExclamation() + "!");
				}
			} else {
				// Set mini party member animations
				_.UI.SetPartyMemberAnim("Success");

				// Audio: Deny
				AudioManager.S.PlaySFX(eSoundName.deny);

				// Display text: miss
				_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " attempted to loot an item from " + Party.S.stats[_.targetNdx].name + "...\n...but missed the mark!\n" + WordManager.S.GetRandomExclamation() + "!");
			}
		} else {
			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);

			// Display text: no items to steal
			_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " attempted to steal an item from " + Party.S.stats[_.targetNdx].name + "...\n...but they've got nothing!\n" + WordManager.S.GetRandomExclamation() + "!");
		}

		_.NextTurn();
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Play attack animations, SFX, and spawn objects
	public void PlaySingleAttackAnimsAndSFX(int playerToAttack, bool playEnemyAnim = true, bool displayFloatingScore = true) {
		// If player doesn't have a status ailment...
		//if (!StatusEffects.S.HasStatusAilment(true, playerToAttack)) {
		//	// Animation: Player Damage
		//	_.playerAnimator[playerToAttack].CrossFade("Damage", 0);
		//}

		// Audio: Damage
		AudioManager.S.PlayRandomDamageSFX();

		// Animation: Shake Screen
		Battle.S.battleUIAnim.CrossFade("BattleUI_Shake", 0);

		// Set mini party member animations
		_.UI.SetPartyMemberAnim("Idle", "Damage", playerToAttack);

		// Get and position Explosion game object
		//GameObject explosion = ObjectPool.S.GetPooledObject("Explosion");
		//ObjectPool.S.PosAndEnableObj(explosion, _.playerSprite[playerToAttack]);

		// Display Floating Score
		if (displayFloatingScore) {
            GameManager.S.InstantiateFloatingScore(_.UI.partyStartsTextBoxSprite[playerToAttack].gameObject, _.attackDamage.ToString(), Color.red);
        }
    }
}