using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStats : MonoBehaviour {
	[Header("Set Dynamically")]
	private Battle _;

	void Start() {
		_ = Battle.S;
	}

	// Returns the index of the party member with the lowest HP
	public int GetPlayerWithLowestHP() {
		int ndx = 0;
		int lowestHP = 9999;

		for (int i = 0; i <= Party.S.partyNdx; i++) {
			if (!_.playerDead[i]) {
				if (Party.S.stats[i].HP < lowestHP) {
					lowestHP = Party.S.stats[i].HP;
					ndx = i;
				}
			}
		}
		return ndx;
	}

	// Returns the index of the enemy with the lowest HP
	// If all enemies at full HP, returns a random enemy index
	public int GetEnemyWithLowestHP() {
		int ndx = -1;
		int lowestHP = 9999;

		// Get the enemy with lowest HP
		for (int i = 0; i < _.enemyStats.Count; i++) {
			if (!_.enemyStats[i].isDead) {
				if (_.enemyStats[i].HP < _.enemyStats[i].maxHP) {
					if (_.enemyStats[i].HP < lowestHP) {
						lowestHP = _.enemyStats[i].HP;
						ndx = i;
					}
				}
			}
		}

		// If all enemies at full HP, select one at random instead
		if(ndx == -1) {
			ndx = Random.Range(0, _.enemyStats.Count);
        }

		return ndx;
	}

	// Returns true if one of the enemy's HP is less than 25%
	public bool EnemiesNeedHeal(float percentage = 0.25f) {
		for (int i = 0; i < _.enemyStats.Count; i++) {
			if (!_.enemyStats[i].isDead) {
				if (Utilities.S.GetPercentage(_.enemyStats[i].HP, _.enemyStats[i].maxHP) < percentage) {
					return true;
				}
			}
		}
		return false;
	}

	// Returns true if one of the enemy's HP is less than 30 HP
	public bool EnemiesNeedHeal(int amount = 30) {
		for (int i = 0; i < _.enemyStats.Count; i++) {
			if (!_.enemyStats[i].isDead) {
				if (_.enemyStats[i].HP < amount) {
					return true;
				}
			}
		}
		return false;
	}

	// The enemy attempts to run away if their attack won't damage the player
	//public void RunIfAttackUseless() {
	//	if (Random.value < _.enemyStats[_.EnemyNdx()].chanceToCallMove) {
	//		// Calculate attack damage to player ((Lvl * 4) + Str - Def)
	//		int attackDamage = ((_.enemyStats[_.EnemyNdx()].LVL * 4) + _.enemyStats[_.EnemyNdx()].STR) - Party.S.stats[0].DEF;

	//		// If attack doesn't do any damage...
	//		if (attackDamage <= 0) {
	//			// ...the enemy focuses on running away
	//			_.enemyStats[_.EnemyNdx()].AI = eEnemyAI.RunAway;
	//			return;
	//		}
	//	}
	//}

	// Returns a random party member index
	public int GetRandomPlayerNdx() {
		int randomNdx = 0;
		float randomValue = Random.value;

        if (randomValue < 0.33f) {
			if (!_.playerDead[0]) {
				randomNdx = 0;
			} else {
				if (Random.value > 0.5f) {
					if (!_.playerDead[1]) {
						randomNdx = 1;
					} else if (!_.playerDead[2]) {
						randomNdx = 2;
					}
				} else {
					if (!_.playerDead[2]) {
						randomNdx = 2;
					} else if (!_.playerDead[1]) {
						randomNdx = 1;
					}
				}
			}
		} else if (randomValue >= 0.33f && randomValue < 0.66f) {
			if (!_.playerDead[1]) {
				randomNdx = 1;
			} else {
				if (Random.value > 0.5f) {
					if (!_.playerDead[0]) {
						randomNdx = 0;
					} else if (!_.playerDead[2]) {
						randomNdx = 2;
					}
				} else {
					if (!_.playerDead[2]) {
						randomNdx = 2;
					} else if (!_.playerDead[0]) {
						randomNdx = 0;
					}
				}
			}
		} else {
			if (!_.playerDead[2]) {
				randomNdx = 2;
			} else {
				if (Random.value > 0.5f) {
					if (!_.playerDead[0]) {
						randomNdx = 0;
					} else if (!_.playerDead[1]) {
						randomNdx = 1;
					}
				} else {
					if (!_.playerDead[1]) {
						randomNdx = 1;
					} else if (!_.playerDead[0]) {
						randomNdx = 0;
					}
				}
			}
		}

		return randomNdx;
	}

	// Get basic physical attack damage
	public void GetAttackEnemyDamage(PartyStats partyMember, EnemyStats enemy, bool targetIsPlayer, int targetNdx) {
		// Reset Attack Damage
		_.attackDamage = 0;

		// Activate display message
		_.UI.ActivateDisplayMessage();

		// 5% chance to Miss/Dodge...
		// ...AND 10% chance to Miss/Dodge if Defender AGI is more than Attacker's 
		if (Random.value <= 0.05f || (enemy.AGI > partyMember.AGI && Random.value < 0.10f)) {
			// Set mini party member animations
			_.UI.SetPartyMemberAnim("Idle", "Fail", _.PlayerNdx());

			// If there's any QTE bonus damage...
			if (_.qteBonusDamage > 0) {
				// Add QTE Bonus Damage
				_.attackDamage = _.qteBonusDamage;

				// If the bonus damage doesn't kill the defender...
				if (enemy.HP > _.qteBonusDamage) {
					if (Random.value <= 0.5f) {
						_.dialogue.DisplayText(partyMember.name + "'s attack attempt nearly failed, but scraped " + enemy.name + " for " + _.attackDamage + " points!");
					} else {
						_.dialogue.DisplayText(partyMember.name + " nearly missed the mark, but knicked " + enemy.name + " for " + _.attackDamage + " points!");
					}
				}
			} else {
				if (Random.value <= 0.5f) {
					_.dialogue.DisplayText(partyMember.name + " attempted to attack " + enemy.name + "... but missed!");
				} else {
					_.dialogue.DisplayText(partyMember.name + " missed the mark! " + enemy.name + " dodged out of the way!");
				}
			}
		} else {
			// 5% chance for Critical Hit
			// Doubles the amount of damage dice to be rolled
			bool isCriticalHit = false;
			int critBonusDamageRolls = 0;
			if (Random.value < 0.05f) {
				isCriticalHit = true;
				critBonusDamageRolls = partyMember.LVL;
			}

			// For each level, roll one die & add its value to attackDamage
			for (int i = 0; i < (partyMember.LVL + critBonusDamageRolls); i++) {
				_.attackDamage += Random.Range(1, 4);
			}

			// Apply modifiers (attacker's STR & defenders DEF)
			_.attackDamage += partyMember.STR;
			_.attackDamage -= enemy.DEF;

			// If no damage is done...
			if (_.attackDamage <= 0) {
				_.attackDamage = 0;

				// 50% chance of increasing damage to 1 HP
				if (Random.value > 0.5f) {
					_.attackDamage = 1;
				}
			}

			// Add QTE Bonus Damage
			_.attackDamage += _.qteBonusDamage;

			// If DEFENDING, cut AttackDamage in HALF
			StatusEffects.S.CheckIfDefending(targetIsPlayer, targetNdx);

			// Set mini party member animations
			_.UI.SetPartyMemberAnim("Idle", "Success", _.PlayerNdx());

			// Display Text
			if (isCriticalHit) {
				_.dialogue.DisplayText("Critical hit!\n" + partyMember.name + " struck " + enemy.name + " for " + _.attackDamage + " points!");
			} else {
				_.dialogue.DisplayText(partyMember.name + " struck " + enemy.name + " for " + _.attackDamage + " points!");
			}
		}
	}

	// Get basic physical attack damage
	public void GetAttackPartyMemberDamage(PartyStats partyMember, EnemyStats enemy, bool targetIsPlayer, int targetNdx) {
		// Reset Attack Damage
		_.attackDamage = 0;

		// Activate display message
		_.UI.ActivateDisplayMessage();

		// 5% chance to Miss/Dodge...
		// ...AND 10% chance to Miss/Dodge if Defender AGI is more than Attacker's 
		if (Random.value <= 0.05f || (partyMember.AGI > enemy.AGI && Random.value < 0.10f)) {
			// Set mini party member animations
			_.UI.SetPartyMemberAnim("Idle", "Success", targetNdx);

			// If there's any QTE bonus damage...
			if (_.qteBonusDamage > 0) {
				// Add QTE Bonus Damage
				_.attackDamage = _.qteBonusDamage;

				// If the bonus damage doesn't kill the defender...
				if (partyMember.HP > _.qteBonusDamage) {
					if (Random.value <= 0.5f) {
						_.dialogue.DisplayText(enemy.GetAttackMissedButQTEDamageMessage1(partyMember.name, _.attackDamage));
					} else {
						_.dialogue.DisplayText(enemy.GetAttackMissedButQTEDamageMessage2(partyMember.name, _.attackDamage));
					}
				}
			} else {
				if (Random.value <= 0.5f) {
					_.dialogue.DisplayText(enemy.GetAttackMissedNoQTEDamageMessage1(partyMember.name));
				} else {
					_.dialogue.DisplayText(enemy.GetAttackMissedNoQTEDamageMessage2(partyMember.name));
				}
			}
		} else {
			// 5% chance for Critical Hit
			// Doubles the amount of damage dice to be rolled
			bool isCriticalHit = false;
			int critBonusDamageRolls = 0;
			if (Random.value < 0.05f) {
				isCriticalHit = true;
				critBonusDamageRolls = enemy.LVL;
			}

			// For each level, roll one die & add its value to attackDamage
			for (int i = 0; i < (enemy.LVL + critBonusDamageRolls); i++) {
				_.attackDamage += Random.Range(1, 4);
			}

			// Apply modifiers (attacker's STR & defenders DEF)
			_.attackDamage += enemy.STR;
			_.attackDamage -= partyMember.DEF;

			// If no damage is done...
			if (_.attackDamage <= 0) {
				_.attackDamage = 0;

				// 50% chance of increasing damage to 1 HP
				if (Random.value > 0.5f) {
					_.attackDamage = 1;
				}
			}

			// Add QTE Bonus Damage
			_.attackDamage += _.qteBonusDamage;

			// If DEFENDING, cut AttackDamage in HALF
			StatusEffects.S.CheckIfDefending(targetIsPlayer, targetNdx);

			// Set mini party member animations
			_.UI.SetPartyMemberAnim("Idle", "Damage", targetNdx);

			// Display Text
			if (isCriticalHit) {
				_.dialogue.DisplayText(enemy.GetAttackCriticalHitMessage(partyMember.name, _.attackDamage));
			} else {
				_.dialogue.DisplayText(enemy.GetAttackHitMessage(partyMember.name, _.attackDamage));
			}
		}
	}
}