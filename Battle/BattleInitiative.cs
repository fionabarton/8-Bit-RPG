using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BattleInitiative : MonoBehaviour {
	[Header("Set dynamically")]
	private Battle _;

	// Initiative
	private int d20;
	// Key: Character Name, Value: Turn Order
	private Dictionary<int, int> turnOrder = new Dictionary<int, int>();

	void Start() {
		_ = Battle.S;
	}

	public void SetInitiative() {
		// Clear TurnOrder List
		_.turnOrder.Clear();

		// Reset turnNdx
		_.turnNdx = 0;

		// Set Party Amount to partyNdx
		_.partyQty = Party.S.partyNdx;

		//////////////////////////////////////////// PARTY MEMBERS ////////////////////////////////////////////

		// Deactivate all party stats
		Utilities.S.SetActiveList(Battle.S.UI.partyStats, false);

		// Reset PlayerDead bools
		for (int i = 0; i < _.playerDead.Count; i++) {
			_.playerDead[i] = true;
		}

		// Activate and update partystats UI
		for (int i = 0; i <= Party.S.partyNdx; i++) {
			Battle.S.UI.partyStats[i].SetActive(true);

			// Set party name and stats text
			Battle.S.UI.partyNameText[i].text = Party.S.stats[i].name;
			Battle.S.UI.UpdatePartyStats(i);

			// Reset PlayerDead bools
			_.playerDead[i] = false;
		}

		// Set party stats UI positions
		switch (Party.S.partyNdx) {
			case 0:
				Utilities.S.SetRectPosition(Battle.S.UI.partyStats[0], 0, 0);
				break;
			case 1:
				Utilities.S.SetRectPosition(Battle.S.UI.partyStats[0], -208, 0);
				Utilities.S.SetRectPosition(Battle.S.UI.partyStats[1], 208, 0);
				break;
			case 2:
				Utilities.S.SetRectPosition(Battle.S.UI.partyStats[0], -420, 0);
				Utilities.S.SetRectPosition(Battle.S.UI.partyStats[1], 0, 0);
				Utilities.S.SetRectPosition(Battle.S.UI.partyStats[2], 420, 0);
				break;
		}

		//////////////////////////////////////////// ENEMIES ////////////////////////////////////////////

		// Randomly Set Enemy Amount
		if (_.enemyAmount == 999) {
			_.randomFactor = Random.Range(0, 100);
			if (_.randomFactor < 20) {
				_.enemyAmount = 1;
			} else if (_.randomFactor >= 20 && _.randomFactor <= 40) {
				_.enemyAmount = 2;
			} else if (_.randomFactor >= 40 && _.randomFactor <= 60) {
				_.enemyAmount = 3;
			} else if (_.randomFactor >= 60 && _.randomFactor <= 80) {
				_.enemyAmount = 4;
			} else if (_.randomFactor > 66) {
				_.enemyAmount = 5;
			}
		} else if (_.enemyAmount == 0) {
			_.enemyAmount = 1;
		}

		// Set Enemy Amount (for testing)
		//_.enemyAmount = 5;
		_.totalEnemyAmount = _.enemyAmount;

        // Remove extra enemy stats
        switch (_.enemyAmount) {
			case 1:
				_.enemyStats.RemoveAt(4);
				_.enemyStats.RemoveAt(3);
				_.enemyStats.RemoveAt(2);
				_.enemyStats.RemoveAt(1);
				break;
			case 2:
				_.enemyStats.RemoveAt(4);
				_.enemyStats.RemoveAt(3);
				_.enemyStats.RemoveAt(2);
				break;
			case 3:
				_.enemyStats.RemoveAt(4);
				_.enemyStats.RemoveAt(3);
				break;
			case 4:
				_.enemyStats.RemoveAt(4);
				break;
		}

		// Deactivate all enemy sprites
		Utilities.S.SetActiveList(_.enemySprites, false);

		// Activate/set enemy sprites
		for (int i = 0; i < _.enemyAmount; i++) {
			Battle.S.enemySprites[i].SetActive(true);
			Battle.S.enemySRends[i].sprite = Battle.S.enemyStats[i].sprite;

			// Gold/EXP payout
			_.expToAdd += _.enemyStats[i].EXP;
			_.goldToAdd += _.enemyStats[i].Gold;

			_.StopCallingForHelp(i);
		}

		// Set enemy sprites positions
		_.UI.PositionEnemySprites();

		// Set Turn Order
		_.randomFactor = Random.Range(0, 100);
		// No Surprise  
		if (_.randomFactor >= 50) {
			_.dialogue.DisplayText("Beware! A " + _.enemyStats[0].name + " has appeared!");

			// Calculate Initiative
			CalculateInitiative();

			// Surprise! Initiative Randomized!
		} else if (_.randomFactor < 50) {
			// Party goes first!
			if (_.randomFactor < 25) {
				_.dialogue.DisplayText(Party.S.stats[0].name + " surprises the Enemy!");

				// Calculate Initiative
				CalculateInitiative("party");

				// Enemies go first!
			} else {
				_.dialogue.DisplayText(_.enemyStats[0].name + " surprises the Player!");

				// Calculate Initiative
				CalculateInitiative("enemies");
			}
		}
	}

	void CalculateInitiative(string whoGoesFirst = "no one") {
		// Reset Dictionary
		turnOrder.Clear();

		// For all characters to engage in battle, calculate their turn order

		// Player 1
		RollInitiative(Party.S.stats[0].battleID, Party.S.stats[0].AGI, Party.S.stats[0].LVL, true, whoGoesFirst);
		// Player 2
		if (_.partyQty >= 1) {
			RollInitiative(Party.S.stats[1].battleID, Party.S.stats[1].AGI, Party.S.stats[1].LVL, true, whoGoesFirst);
			// Player 3
			if (_.partyQty >= 2) {
				RollInitiative(Party.S.stats[2].battleID, Party.S.stats[2].AGI, Party.S.stats[2].LVL, true, whoGoesFirst);
			}
		}

		// Enemy 1
		RollInitiative(_.enemyStats[0].battleID, _.enemyStats[0].AGI, _.enemyStats[0].LVL, false, whoGoesFirst);
		// Enemy 2
		if (_.enemyAmount >= 2) {
			RollInitiative(_.enemyStats[1].battleID, _.enemyStats[1].AGI, _.enemyStats[1].LVL, false, whoGoesFirst);
			// Enemy 3
			if (_.enemyAmount >= 3) {
				RollInitiative(_.enemyStats[2].battleID, _.enemyStats[2].AGI, _.enemyStats[2].LVL, false, whoGoesFirst);
				// Enemy 4
				if (_.enemyAmount >= 4) {
					RollInitiative(_.enemyStats[3].battleID, _.enemyStats[3].AGI, _.enemyStats[3].LVL, false, whoGoesFirst);
					// Enemy 5
					if (_.enemyAmount >= 5) {
						RollInitiative(_.enemyStats[4].battleID, _.enemyStats[4].AGI, _.enemyStats[4].LVL, false, whoGoesFirst);
					}
				}
			}
		}

		// Sort Dictionary by turnOrder
		var items = from pair in turnOrder
					orderby pair.Value descending
					select pair;

		// Add each Dictionary Key (party member or enemy name as a string) to Battle.TurnOrder
		foreach (KeyValuePair<int, int> pair in items) {
			_.turnOrder.Add(pair.Key);
		}
	}

	public void RollInitiative(int battleID, int AGI, int LVL, bool playerOrEnemy, string whoGoesFirst) {
		// Roll Player/Enemy's Initiative
		d20 = Random.Range(1, 20);
		d20 += AGI + LVL;

		// If one group catches the other off guard, ensure their turn order is higher
		switch (whoGoesFirst) {
			case "party":
				if (playerOrEnemy) {
					d20 += 1000;
				}
				break;
			case "enemies":
				if (!playerOrEnemy) {
					d20 += 1000;
				}
				break;
		}

		// Add character to TurnOrder
		turnOrder.Add(battleID, d20);
	}
}