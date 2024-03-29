﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour {
	[Header("Set in Inspector")]
	// Player/Enemy Defense Shields
	public List<GameObject> playerShields;
	public List<GameObject> enemyShields;

	// Player/Enemy Paralyzed Icons
	public List<GameObject> playerParalyzedIcons;
	public List<GameObject> enemyParalyzedIcons;

	// Player/Enemy Poisoned Icons (Battle)
	public List<GameObject> playerPoisonedIcons;
	public List<GameObject> enemyPoisonedIcons;

	// Player/Enemy Sleeping Icons
	public List<GameObject> playerSleepingIcons;
	public List<GameObject> enemySleepingIcons;

	// Overworld Poisoned Icons 
	public List<GameObject> overworldPoisonedIcons;

	// Pause Screen Poisoned Icons (Overworld)
	public List<GameObject> pauseScreenPoisonedIcons;

	// Player Flicker (in overworld, each party member's sprite flickers when they take damage from being poisoned)
	public List<Flicker> playerFlickers;

	[Header("Set Dynamically")]
	// Defending party members 
	public List<bool> playerIsDefending = new List<bool>();

	// Party members & enemies afflicted by status effects: Paralysis, Sleep, Poison
	// If 0, then the combatant is NOT paralyzed or sleeping
	public List<int> playerIsParalyzed = new List<int>();
	public List<int> playerIsSleeping = new List<int>();
	public List<bool> playerIsPoisoned = new List<bool>();

	private static StatusEffects _S;
	public static StatusEffects S { get { return _S; } set { _S = value; } }

	private Battle _;

	void Awake() {
		S = this;
	}

	void Start() {
		_ = Battle.S;

		playerIsDefending = new List<bool>() { false, false, false };
		playerIsParalyzed = new List<int>() { 0, 0, 0 };
		playerIsSleeping = new List<int>() { 0, 0, 0 };
		playerIsPoisoned = new List<bool>() { false, false, false };
	}

	// Called at start of a battle
	public void Initialize() {
        // Deactivate defense shields
        Utilities.S.SetActiveList(playerShields, false);
        Utilities.S.SetActiveList(enemyShields, false);

        // Deactivate Status Ailment Icons (Paralyzed, Poisoned, Sleeping)
        Utilities.S.SetActiveList(playerParalyzedIcons, false);
        Utilities.S.SetActiveList(playerPoisonedIcons, false);
        Utilities.S.SetActiveList(playerSleepingIcons, false);
        Utilities.S.SetActiveList(enemyParalyzedIcons, false);
        Utilities.S.SetActiveList(enemyPoisonedIcons, false);
        Utilities.S.SetActiveList(enemySleepingIcons, false);

        // Reset status ailments
        for (int i = 0; i < playerIsDefending.Count; i++) {
			playerIsDefending[i] = false;
			playerIsParalyzed[i] = 0;
			playerIsSleeping[i] = 0;

			// If already poisoned...
			if (CheckIfPoisoned(true, i)) {
                // ...activate poison icon
                playerPoisonedIcons[i].SetActive(true);
            }
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////
	// Defend
	////////////////////////////////////////////////////////////////////////////////////////
	public void AddDefender(bool isPlayer, int ndx) {
		// Activate defense shield
		if (isPlayer) {
			playerIsDefending[ndx] = true;
			playerShields[ndx].SetActive(true);
		} else {
			_.enemyStats[ndx].isDefending = true;
			enemyShields[ndx].SetActive(true);
		}

		// Audio: Buff 2
		AudioManager.S.PlaySFX(eSoundName.buff2);
	}
	public void RemoveDefender(bool isPlayer, int ndx) {
        // Deactivate defense shield
        if (isPlayer) {
            playerIsDefending[ndx] = false;
            playerShields[ndx].SetActive(false);
        } else {
			_.enemyStats[ndx].isDefending = false;
			enemyShields[ndx].SetActive(false);
        }
    }
	// If defending, cut attackDamage in half
	public void CheckIfDefending(bool isPlayer, int ndx) {
		if (isPlayer) {
			if (playerIsDefending[ndx]) {
				_.attackDamage /= 2;
			}
		} else {
			if (_.enemyStats[ndx].isDefending) {
				_.attackDamage /= 2;
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////
	// Poison
	////////////////////////////////////////////////////////////////////////////////////////
	public void AddPoisoned(int ndx) {
		// If this turn is a player's turn...
		if (_.PlayerNdx() != -1) {
			_.enemyStats[ndx].isPoisoned = true;

			// Activate poisoned icon
			enemyPoisonedIcons[ndx].SetActive(true);

            _.dialogue.DisplayText(Party.S.stats[_.PlayerNdx()].name + " has poisoned " + _.enemyStats[ndx].name + " indefinitely" + "...\n...not nice!");
		} else {
			playerIsPoisoned[ndx] = true;

            // Anim
            //_.playerAnimator[ndx].CrossFade("Poisoned", 0);

            // Activate poisoned icon
            playerPoisonedIcons[ndx].SetActive(true);

            _.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " has poisoned " + Party.S.stats[ndx].name + " indefinitely" + " ...\n...not nice!");
		}

		// Audio: Buff 2
		AudioManager.S.PlaySFX(eSoundName.buff2);

		_.NextTurn();
	}
	public void RemovePoisoned(bool isPlayer, int ndx, bool displayText = true) {
		// If this turn is a player's turn...
		if (isPlayer) {
			playerIsPoisoned[ndx] = false;

            // Deactivate status ailment icon
            playerPoisonedIcons[ndx].SetActive(false);

            // Display text
            if (displayText) {
				_.dialogue.DisplayText(Party.S.stats[ndx].name + " is no longer poisoned!");
			}
		} else {
			_.enemyStats[ndx].isPoisoned = false;

			// Deactivate status ailment icon
			enemyPoisonedIcons[ndx].SetActive(false);

			// Display text
			if (displayText) {
				_.dialogue.DisplayText(_.enemyStats[ndx].name + " is no longer poisoned!");
			}
		}

		// Audio: Buff 2
		AudioManager.S.PlaySFX(eSoundName.buff2);
	}
	public bool CheckIfPoisoned(bool isPlayer, int ndx) {
		if (isPlayer) {
			if (playerIsPoisoned[ndx] == true) {
				return true;
			}
		} else {
			if (_.enemyStats[ndx].isPoisoned == true) {
				return true;
			}
		}
		return false;
	}

	public void Poisoned(string poisoned, bool isPlayer, int ndx) {
		// If this turn is a player's turn...
		if (isPlayer) {
			// Get 6-10% of max HP
			float lowEnd = Party.S.stats[ndx].maxHP * 0.06f;
			float highEnd = Party.S.stats[ndx].maxHP * 0.10f;
			_.attackDamage = (int)Random.Range(lowEnd, highEnd);

            // Play attack animations, SFX, and spawn objects
            _.enemyActions.PlaySingleAttackAnimsAndSFX(ndx, false);

            // Decrement HP
            GameManager.S.SubtractPlayerHP(ndx, _.attackDamage);

			// Display text
			_.dialogue.DisplayText(poisoned + " suffers the consequences of being poisoned...\n...damaged for " + _.attackDamage + " HP!");

			// Check if dead
			if (Party.S.stats[ndx].HP < 1) {
				// Add player index to list of dead combatants
				_.deadCombatantNdxs.Add(ndx);

				// Player dead mode
				_.mode = eBattleMode.playerDead;
				return;
			}
		} else {
			// Get 6-10% of max HP
			float lowEnd = _.enemyStats[ndx].maxHP * 0.06f;
			float highEnd = _.enemyStats[ndx].maxHP * 0.10f;
			_.attackDamage = Mathf.Max(1, (int)Random.Range(lowEnd, highEnd));

			// Set mini party member animations
			_.UI.SetPartyMemberAnim("Success");

			Spells.S.battle.DamageEnemyAnimation(ndx, true, false);

            // Decrement HP
            GameManager.S.SubtractEnemyHP(ndx, _.attackDamage);

			// Display text
			_.dialogue.DisplayText(poisoned + " suffers the consequences of being poisoned...\n...damaged for " + _.attackDamage + " HP!");

			// Check if dead
			if (_.enemyStats[ndx].HP < 1) {
				// Add enemy index to list of dead combatants
				_.deadCombatantNdxs.Add(ndx);

				// Enemy dead mode
				_.mode = eBattleMode.enemyDead;
				return;
			}
		}

		// Check if paralyzed
		_.mode = eBattleMode.checkIfParalyzed;
	}

	////////////////////////////////////////////////////////////////////////////////////////
	// Paralyze
	////////////////////////////////////////////////////////////////////////////////////////
	public void AddParalyzed(int ndx) {
		// If this turn is a player's turn...
		if (_.PlayerNdx() != -1) {
            _.enemyStats[ndx].isParalyzed = Random.Range(2, 4);

			// Activate paralyzed icon
			enemyParalyzedIcons[ndx].SetActive(true);

            // Reset next turn move index & deactivate help bubble
            _.StopCallingForHelp(ndx);

			// If defending...stop defending
			RemoveDefender(false, ndx);

			_.dialogue.DisplayText(Party.S.stats[_.PlayerNdx()].name + " has temporarily paralyzed " + _.enemyStats[ndx].name + "...\n...not nice!");
		} else {
			playerIsParalyzed[ndx] = Random.Range(2, 4);

            // Anim
            //_.playerAnimator[ndx].CrossFade("Paralyzed", 0);

            // Activate paralyzed icon
            playerParalyzedIcons[ndx].SetActive(true);

            // If defending...stop defending
            RemoveDefender(true, ndx);

			_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " has temporarily paralyzed " + Party.S.stats[ndx].name + "...\n...not nice!");
		}

		// Audio: Buff 2
		AudioManager.S.PlaySFX(eSoundName.buff2);

		_.NextTurn();
	}
	public void RemoveParalyzed(bool isPlayer, int ndx, bool displayText = true) {
		// If this turn is a player's turn...
		if (isPlayer) {
			playerIsParalyzed[ndx] = 0;

            // Deactivate status ailment icon
            playerParalyzedIcons[ndx].SetActive(false);

            // Display text
            if (displayText) {
				_.dialogue.DisplayText(Party.S.stats[ndx].name + " is no longer paralyzed!");
			}
		} else {
			_.enemyStats[ndx].isParalyzed = 0;

			// Deactivate status ailment icon
			enemyParalyzedIcons[ndx].SetActive(false);

			// Display text
			if (displayText) {
				_.dialogue.DisplayText(_.enemyStats[ndx].name + " is no longer paralyzed!");
			}
		}

		// Audio: Buff 2
		AudioManager.S.PlaySFX(eSoundName.buff2);
	}
	public bool CheckIfParalyzed(bool isPlayer, int ndx) {
		if (isPlayer) {
			if (playerIsParalyzed[ndx] > 0) {
				return true;
			}
		} else {
			if (_.enemyStats[ndx].isParalyzed > 0) {
				return true;
			}
		}

		return false;
	}

	public void Paralyzed(string paralyzed, bool isPlayer, int ndx) {
		bool counterIsDepleted = false;
		if (isPlayer) {
			playerIsParalyzed[ndx] -= 1;

			if (playerIsParalyzed[ndx] <= 0) {
				counterIsDepleted = true;
			}
		} else {
			_.enemyStats[ndx].isParalyzed -= 1;

			if (_.enemyStats[ndx].isParalyzed <= 0) {
				counterIsDepleted = true;
			}
		}

		// If counter depleted...
		if (counterIsDepleted) {
			// ...no longer paralyzed
			RemoveParalyzed(isPlayer, ndx);

			if (isPlayer) {
				// Anim
				//_.playerAnimator[ndx].CrossFade("Win_Battle", 0);
			}
		} else {
			// Display text
			_.dialogue.DisplayText(paralyzed + " is paralyzed and cannot move!");

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}

		// Check if sleeping
		_.mode = eBattleMode.checkIfSleeping;
	}

	////////////////////////////////////////////////////////////////////////////////////////
	// Sleep
	////////////////////////////////////////////////////////////////////////////////////////
	public void AddSleeping(int ndx) {
		// If this turn is a player's turn...
		if (_.PlayerNdx() != -1) {
			_.enemyStats[ndx].isSleeping = Random.Range(2, 4);

			// Activate sleeping icon
			enemySleepingIcons[ndx].SetActive(true);

            // Reset next turn move index & deactivate help bubble
            _.StopCallingForHelp(ndx);

			// If defending...stop defending
			RemoveDefender(false, ndx);

			_.dialogue.DisplayText(Party.S.stats[_.PlayerNdx()].name + " has temporarily put " + _.enemyStats[ndx].name + " to sleep...\n...not nice!");
		} else {
			playerIsSleeping[ndx] = Random.Range(2, 4);

            // Anim
            //_.playerAnimator[ndx].CrossFade("Sleeping", 0);

            // Activate sleeping icon
            playerSleepingIcons[ndx].SetActive(true);

            // If defending...stop defending
            RemoveDefender(true, ndx);

			_.dialogue.DisplayText(_.enemyStats[_.EnemyNdx()].name + " has temporarily put " + Party.S.stats[ndx].name + " to sleep...\n...not nice!");
		}

		// Audio: Buff 2
		AudioManager.S.PlaySFX(eSoundName.buff2);

		_.NextTurn();
	}
	public void RemoveSleeping(bool isPlayer, int ndx, bool displayText = true) {
		// If this turn is a player's turn...
		if (isPlayer) {
			playerIsSleeping[ndx] = 0;

            // Deactivate status ailment icon
            playerSleepingIcons[ndx].SetActive(false);

            // Display text
            if (displayText) {
				_.dialogue.DisplayText(Party.S.stats[ndx].name + " is no longer asleep!");
			}
		} else {
			_.enemyStats[ndx].isSleeping = 0;

			// Deactivate status ailment icon
			enemySleepingIcons[ndx].SetActive(false);

			// Display text
			if (displayText) {
				_.dialogue.DisplayText(_.enemyStats[ndx].name + " is no longer asleep!");
			}
		}

		// Audio: Buff 2
		AudioManager.S.PlaySFX(eSoundName.buff2);
	}
	public bool CheckIfSleeping(bool isPlayer, int ndx) {
		if (isPlayer) {
			if (playerIsSleeping[ndx] > 0) {
				return true;
			}
		} else {
			if (_.enemyStats[ndx].isSleeping > 0) {
				return true;
			}
		}
		return false;
	}

	public void Sleeping(string sleeping, bool isPlayer, int ndx) {
		bool counterIsDepleted = false;
		if (isPlayer) {
			playerIsSleeping[ndx] -= 1;

			if (playerIsSleeping[ndx] <= 0) {
				counterIsDepleted = true;
			}
		} else {
			_.enemyStats[ndx].isSleeping -= 1;

			if (_.enemyStats[ndx].isSleeping <= 0) {
				counterIsDepleted = true;
			}
		}

		// If counter depleted...
		if (counterIsDepleted) {
			// ...no longer sleeping
			RemoveSleeping(isPlayer, ndx);

			// Announce they're done sleeping
			_.mode = eBattleMode.doneSleeping;
		} else {
			// Display text
			_.dialogue.DisplayText(sleeping + " is asleep and won't wake up!");

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);

			// Announce they're sleeping
			_.mode = eBattleMode.isSleeping;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////
	// Helper functions
	////////////////////////////////////////////////////////////////////////////////////////

	// Returns true if the combatant has a status ailment ////////////////////
	public bool HasStatusAilment(bool isPlayer, int ndx) {
		if (CheckIfParalyzed(isPlayer, ndx) || CheckIfPoisoned(isPlayer, ndx) || CheckIfSleeping(isPlayer, ndx)) {
			return true;
		}
		return false;
	}

	// Remove all status ailments from a combatant
	public void RemoveAllStatusAilments(bool isPlayer, int ndx) {
		RemoveDefender(isPlayer, ndx);
		RemoveParalyzed(isPlayer, ndx, false);
		RemovePoisoned(isPlayer, ndx, false);
		RemoveSleeping(isPlayer, ndx, false);
	}

	// If a party member is poisoned, enable their overworld poisoned icon
	public void SetOverworldPoisonIcons() {
		// Deactivate ALL poisoned icons
		Utilities.S.SetActiveList(overworldPoisonedIcons, false);
		Utilities.S.SetActiveList(pauseScreenPoisonedIcons, false);

		for (int i = 0; i <= Party.S.partyNdx; i++) {
			// If poisoned...
			if (CheckIfPoisoned(true, i)) {
				// ...activate poisoned icons
				overworldPoisonedIcons[i].SetActive(true);
				pauseScreenPoisonedIcons[i].SetActive(true);
			} else {
				// ...deactivate poisoned icons
				overworldPoisonedIcons[i].SetActive(false);
				pauseScreenPoisonedIcons[i].SetActive(false);
			}
		}
	}

	// In battle, activates status effect icons for all affected enemy combatants 
	public void UpdateActivatedEnemyStatusIcons() {
		// Deactivate all enemy status icons
		Utilities.S.SetActiveList(enemyShields, false);
		Utilities.S.SetActiveList(enemyPoisonedIcons, false);
		Utilities.S.SetActiveList(enemyParalyzedIcons, false);
		Utilities.S.SetActiveList(enemySleepingIcons, false);

		// Activate icon if affected by status effect 
		for (int i = 0; i < _.enemyStats.Count; i++) {
			// Defending
			if (_.enemyStats[i].isDefending) {
				enemyShields[i].SetActive(true);
			}
			// Poisoned
			if (_.enemyStats[i].isPoisoned) {
				enemyPoisonedIcons[i].SetActive(true);
			}
			// Paralyzed
			if (_.enemyStats[i].isParalyzed > 0) {
				enemyParalyzedIcons[i].SetActive(true);
			}
			// Sleeping
			if (_.enemyStats[i].isSleeping > 0) {
				enemySleepingIcons[i].SetActive(true);
			}
		}
	}
}