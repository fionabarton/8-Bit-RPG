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
	public int GetEnemyWithLowestHP() {
		int ndx = -1;
		int lowestHP = 9999;

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
		return ndx;
	}

	// Returns true if one of the enemy's HP is less than 25%
	public bool EnemiesNeedsHeal() {
		for (int i = 0; i < _.enemyStats.Count; i++) {
			if (!_.enemyStats[i].isDead) {
				if (Utilities.S.GetPercentage(_.enemyStats[i].HP, _.enemyStats[i].maxHP) < 0.25f) {
					return true;
				}
			}
		}
		return false;
	}

	// The enemy attempts to run away if their attack won't damage the player
	public void RunIfAttackUseless() {
		if (Random.value < _.enemyStats[_.EnemyNdx()].chanceToCallMove) {
			// Calculate attack damage to player ((Lvl * 4) + Str - Def)
			int attackDamage = ((_.enemyStats[_.EnemyNdx()].LVL * 4) + _.enemyStats[_.EnemyNdx()].STR) - Party.S.stats[0].DEF;

			// If attack doesn't do any damage...
			if (attackDamage <= 0) {
				// ...the enemy focuses on running away
				_.enemyStats[_.EnemyNdx()].AI = eEnemyAI.RunAway;
				return;
			}
		}
	}

	// Returns a random party member index
	public int GetRandomPlayerNdx() {
		int randomNdx = 0;
		float randomValue = Random.value;

		if (_.partyQty == 0) {
			for (int i = 0; i < _.playerDead.Count; i++) {
				if (!_.playerDead[i]) {
					randomNdx = i;
					break;
				}
			}
		} else if (_.partyQty == 1) {
			if (randomValue > 0.5f) {
				for (int i = 0; i < _.playerDead.Count; i++) {
					if (!_.playerDead[i]) {
						randomNdx = i;
						break;
					}
				}
			} else {
				for (int i = _.playerDead.Count - 1; i >= 0; i--) {
					if (!_.playerDead[i]) {
						randomNdx = i;
						break;
					}
				}
			}
		} else if (_.partyQty == 2) {
			if (randomValue >= 0 && randomValue <= 0.33f) {
				randomNdx = 0;
			} else if (randomValue > 0.33f && randomValue <= 0.66f) {
				randomNdx = 1;
			} else if (randomValue > 0.66f && randomValue <= 1.0f) {
				randomNdx = 2;
			}
		}
		return randomNdx;
	}

	// Get basic physical attack damage
	public void GetPhysicalAttackDamage(int attackerLVL, int attackerSTR, int attackerAGI, int defenderDEF, int defenderAGI, string attackerName, string defenderName, int defenderHP, bool targetIsPlayer, int targetNdx) {
		// Reset Attack Damage
		_.attackDamage = 0;

		// Activate display message
		_.UI.ActivateDisplayMessage();

		// 5% chance to Miss/Dodge...
		// ...AND 10% chance to Miss/Dodge if Defender AGI is more than Attacker's 
		if (Random.value <= 0.05f || (defenderAGI > attackerAGI && Random.value < 0.10f)) {
			// Set mini party member animations
			if (targetIsPlayer) {
				_.UI.SetPartyMemberAnim("Idle", "Success", targetNdx);
			} else {
				_.UI.SetPartyMemberAnim("Idle", "Fail", _.PlayerNdx());
			}

			// If there's any QTE bonus damage...
			if (_.qteBonusDamage > 0) {
				// Add QTE Bonus Damage
				_.attackDamage = _.qteBonusDamage;

				// If the bonus damage doesn't kill the defender...
				if (defenderHP > _.qteBonusDamage) {
					if (Random.value <= 0.5f) {
						_.dialogue.DisplayText(attackerName + "'s attack attempt nearly failed, but scraped " + defenderName + " for " + _.attackDamage + " points!");
					} else {
						_.dialogue.DisplayText(attackerName + " nearly missed the mark, but knicked " + defenderName + " for " + _.attackDamage + " points!");
					}
				}
			} else {
				if (Random.value <= 0.5f) {
					_.dialogue.DisplayText(attackerName + " attempted to attack " + defenderName + "... but missed!");
				} else {
					_.dialogue.DisplayText(attackerName + " missed the mark! " + defenderName + " dodged out of the way!");
				}
			}
		} else {
			// 5% chance for Critical Hit
			// Doubles the amount of damage dice to be rolled
			bool isCriticalHit = false;
			if (Random.value < 0.05f) {
				attackerLVL *= 2;
				isCriticalHit = true;
			}

			// For each level, roll one die & add its value to attackDamage
			for (int i = 0; i < attackerLVL; i++) {
				_.attackDamage += Random.Range(1, 4);
			}

			// Apply modifiers (attacker's STR & defenders DEF)
			_.attackDamage += attackerSTR;
			_.attackDamage -= defenderDEF;

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
			if (targetIsPlayer) {
				_.UI.SetPartyMemberAnim("Idle", "Damage", targetNdx);
			} else {
				_.UI.SetPartyMemberAnim("Idle", "Success", _.PlayerNdx());
			}

			// Display Text
			if (defenderHP > _.attackDamage) {
				if (isCriticalHit) {
					_.dialogue.DisplayText("Critical hit!\n" + attackerName + " struck " + defenderName + " for " + _.attackDamage + " points!");
				} else {
					_.dialogue.DisplayText(attackerName + " struck " + defenderName + " for " + _.attackDamage + " points!");
				}
			}
		}
	}
}