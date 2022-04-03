using System.Collections;
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

	// PlayerButtons Poisoned Icons (Overworld)
	public List<GameObject> playerButtonsPoisonedIcons;

	// PauseScreen Poisoned Icons (Overworld)
	public List<GameObject> pauseScreenPoisonedIcons;

	// Player GameObject Poisoned Icon (Overworld)
	public GameObject playerPoisonedIcon;

	[Header("Set Dynamically")]
	// Defending party members & enemies
	public List<bool> playerIsDefending = new List<bool>();
	public List<bool> enemyIsDefending = new List<bool>();

	// Party members & enemies afflicted by status effects: Paralysis, Sleep, Poison
	// If 0, then the combatant is NOT paralyzed or sleeping
	public List<int> playerIsParalyzed = new List<int>();
	public List<int> enemyIsParalyzed = new List<int>();
	public List<int> playerIsSleeping = new List<int>();
	public List<int> enemyIsSleeping = new List<int>();
	public List<bool> playerIsPoisoned = new List<bool>();
	public List<bool> enemyIsPoisoned = new List<bool>();

	private static StatusEffects _S;
	public static StatusEffects S { get { return _S; } set { _S = value; } }

	private Battle _;

	void Awake() {
		S = this;
	}

	void Start() {
		_ = Battle.S;

		playerIsDefending = new List<bool>() { false, false, false };
		enemyIsDefending = new List<bool>() { false, false, false, false, false };

		playerIsParalyzed = new List<int>() { 0, 0, 0 };
		enemyIsParalyzed = new List<int>() { 0, 0, 0, 0, 0 };

		playerIsSleeping = new List<int>() { 0, 0, 0 };
		enemyIsSleeping = new List<int>() { 0, 0, 0, 0, 0 };

		playerIsPoisoned = new List<bool>() { false, false, false };
		enemyIsPoisoned = new List<bool>() { false, false, false, false, false };
	}

	// Called at start of a battle
	public void Initialize() {
		//// Deactivate defense shields
		//Utilities.S.SetActiveList(playerShields, false);
		//Utilities.S.SetActiveList(enemyShields, false);

		//// Deactivate Status Ailment Icons (Paralyzed, Poisoned, Sleeping)
		//Utilities.S.SetActiveList(playerParalyzedIcons, false);
		//Utilities.S.SetActiveList(playerPoisonedIcons, false);
		//Utilities.S.SetActiveList(playerSleepingIcons, false);
		//Utilities.S.SetActiveList(enemyParalyzedIcons, false);
		//Utilities.S.SetActiveList(enemyPoisonedIcons, false);
		//Utilities.S.SetActiveList(enemySleepingIcons, false);

		// Reset status ailments
		for (int i = 0; i < playerIsDefending.Count; i++) {
			playerIsDefending[i] = false;
			playerIsParalyzed[i] = 0;
			playerIsSleeping[i] = 0;

			// If already poisoned...
			if (CheckIfPoisoned(true, i)) {
				// ...activate poison icon
				//playerPoisonedIcons[i].SetActive(true);
			}
		}

		for (int i = 0; i < enemyIsPoisoned.Count; i++) {
			enemyIsDefending[i] = false;
			enemyIsPoisoned[i] = false;
			enemyIsParalyzed[i] = 0;
			enemyIsSleeping[i] = 0;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////
	// Defend
	////////////////////////////////////////////////////////////////////////////////////////
	public void AddDefender(bool isPlayer, int ndx) {
		// Activate defense shield
		if (isPlayer) {
			playerIsDefending[ndx] = true;
			//playerShields[ndx].SetActive(true);
		} else {
			enemyIsDefending[ndx] = true;
			//enemyShields[ndx].SetActive(true);
		}

		// Audio: Buff 2
		AudioManager.S.PlaySFX(eSoundName.buff2);
	}
	public void RemoveDefender(bool isPlayer, int ndx) {
        // Deactivate defense shield
        if (isPlayer) {
            playerIsDefending[ndx] = false;
            //playerShields[ndx].SetActive(false);
        } else {
            enemyIsDefending[ndx] = false;
            //enemyShields[ndx].SetActive(false);
        }
    }
	// If defending, cut attackDamage in half
	public void CheckIfDefending(bool isPlayer, int ndx) {
		if (isPlayer) {
			if (playerIsDefending[ndx] == true) {
				_.attackDamage /= 2;
			}
		} else {
			if (enemyIsDefending[ndx] == true) {
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
			enemyIsPoisoned[ndx] = true;

			// Activate poisoned icon
			//enemyPoisonedIcons[ndx].SetActive(true);

			_.dialogue.DisplayText(Party.S.stats[_.PlayerNdx()].name + " has poisoned " + _.enemyStats[ndx].name + " indefinitely" + "...\n...not nice!");
		} else {
			playerIsPoisoned[ndx] = true;

			// Anim
			//_.playerAnimator[ndx].CrossFade("Poisoned", 0);

			// Activate poisoned icon
			//playerPoisonedIcons[ndx].SetActive(true);

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
            //playerPoisonedIcons[ndx].SetActive(false);

            // Display text
            if (displayText) {
				_.dialogue.DisplayText(Party.S.stats[ndx].name + " is no longer poisoned!");
			}
		} else {
			enemyIsPoisoned[ndx] = false;

			// Deactivate status ailment icon
			//enemyPoisonedIcons[ndx].SetActive(false);
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
			if (enemyIsPoisoned[ndx] == true) {
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
			//_.enemyActions.PlaySingleAttackAnimsAndSFX(ndx, false);

			// Decrement HP
			GameManager.S.SubtractPlayerHP(ndx, _.attackDamage);

			// Display text
			_.dialogue.DisplayText(poisoned + " suffers the consequences of being poisoned...\n...damaged for " + _.attackDamage + " HP!");

			// Check if dead
			if (Party.S.stats[ndx].HP < 1) {
				_.end.PlayerDeath(ndx);
				return;
			}
		} else {
			// Get 6-10% of max HP
			float lowEnd = _.enemyStats[ndx].maxHP * 0.06f;
			float highEnd = _.enemyStats[ndx].maxHP * 0.10f;
			_.attackDamage = Mathf.Max(1, (int)Random.Range(lowEnd, highEnd));

			//Spells.S.battle.DamageEnemyAnimation(ndx, true, false);

			// Decrement HP
			GameManager.S.SubtractEnemyHP(ndx, _.attackDamage);

			// Display text
			_.dialogue.DisplayText(poisoned + " suffers the consequences of being poisoned...\n...damaged for " + _.attackDamage + " HP!");

			// Check if dead
			if (_.enemyStats[ndx].HP < 1) {
				_.end.EnemyDeath(ndx);
				return;
			}
		}
		_.mode = eBattleMode.statusAilment;
	}

	////////////////////////////////////////////////////////////////////////////////////////
	// Paralyze
	////////////////////////////////////////////////////////////////////////////////////////
	public void AddParalyzed(int ndx) {
		// If this turn is a player's turn...
		if (_.PlayerNdx() != -1) {
			enemyIsParalyzed[ndx] = Random.Range(2, 4);

			// Activate paralyzed icon
			//enemyParalyzedIcons[ndx].SetActive(true);

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
			//playerParalyzedIcons[ndx].SetActive(true);

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
			//playerParalyzedIcons[ndx].SetActive(false);

			// Display text
			if (displayText) {
				_.dialogue.DisplayText(Party.S.stats[ndx].name + " is no longer paralyzed!");
			}
		} else {
			enemyIsParalyzed[ndx] = 0;

			// Deactivate status ailment icon
			//enemyParalyzedIcons[ndx].SetActive(false);
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
			if (enemyIsParalyzed[ndx] > 0) {
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
			enemyIsParalyzed[ndx] -= 1;

			if (enemyIsParalyzed[ndx] <= 0) {
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
		_.mode = eBattleMode.statusAilment;
	}

	////////////////////////////////////////////////////////////////////////////////////////
	// Sleep
	////////////////////////////////////////////////////////////////////////////////////////
	public void AddSleeping(int ndx) {
		// If this turn is a player's turn...
		if (_.PlayerNdx() != -1) {
			enemyIsSleeping[ndx] = Random.Range(2, 4);

			// Activate sleeping icon
			//enemySleepingIcons[ndx].SetActive(true);

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
			//playerSleepingIcons[ndx].SetActive(true);

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
			//playerSleepingIcons[ndx].SetActive(false);

			// Display text
			if (displayText) {
				_.dialogue.DisplayText(Party.S.stats[ndx].name + " is no longer asleep!");
			}
		} else {
			enemyIsSleeping[ndx] = 0;

			// Deactivate status ailment icon
			//enemySleepingIcons[ndx].SetActive(false);
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
			if (enemyIsSleeping[ndx] > 0) {
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
			enemyIsSleeping[ndx] -= 1;

			if (enemyIsSleeping[ndx] <= 0) {
				counterIsDepleted = true;
			}
		}

		// If counter depleted...
		if (counterIsDepleted) {
			// ...no longer sleeping
			RemoveSleeping(isPlayer, ndx);

			if (isPlayer) {
				// Anim
				//_.playerAnimator[ndx].CrossFade("Win_Battle", 0);
			}
		} else {
			// Display text
			_.dialogue.DisplayText(sleeping + " is asleep and won't wake up!");

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}
		_.mode = eBattleMode.statusAilment;
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
		//playerPoisonedIcon.SetActive(false);
		for (int i = 0; i <= Party.S.partyNdx; i++) {
			// If poisoned...
			if (CheckIfPoisoned(true, i)) {
				// ...activate poisoned icons
				//playerButtonsPoisonedIcons[i].SetActive(true);
				//pauseScreenPoisonedIcons[i].SetActive(true);
				//playerPoisonedIcon.SetActive(true);
			} else {
				// ...deactivate poisoned icons
				//playerButtonsPoisonedIcons[i].SetActive(false);
				//pauseScreenPoisonedIcons[i].SetActive(false);
			}
		}
	}
}