using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Stats")]
public class EnemyStats : ScriptableObject {
	public new string name;
	public Sprite sprite;
	public int HP; // set to maxHP in BattleInitiative.cs
	public int MP; // set to maxMP in BattleInitiative.cs
	public int maxHP;
	public int maxMP;
	public int STR;
	public int DEF;
	public int WIS;
	public int AGI;
	public int EXP;
	public int Gold;
	public int LVL;

	// Drop Item
	public List<eItem> itemsToDrop = new List<eItem>();
	public float chanceToDrop;
	public int maxAmountToSteal; // amount of items that can be stolen from this enemy
	public int amountToSteal; // set to maxAmountToSteal in BattleInitiative.cs

	// Items stolen from party
	public List<Item> stolenItems = new List<Item>();

	// AI
	public eEnemyAI AI;
	public int AI_id = -1; // Default setting simply calls random move

	public int questNdx = -1;

	// Chance to call action (range from 0.0 to 1.0)
	public float chanceToCallAction = 0.5f;
	public int defaultAction;

	// Actions/Moves Enemy can perform
	public List<int> actionList;

	public bool isDead; // set to false in BattleInitiative.cs
	
	// Calling for help
	public bool isCallingForHelp;
	public int	nextTurnActionNdx;

	// Amount of time the player has to block an attack
	public float timeToQTEBlock = 1.5f;

	// Set dynamically when entering battle
	public int battleID;

	// Text messages describing how each action was performed
	// Attack messages
	public string attackHitMessage(string attackerName) {
		string a = "";
		return a;
    }
	// public string attackHitMessage =
	// public string attackCriticalHitMessage =
	// public string attackMissedMessage1 =
	// public string attackMissedMessage2 =
	// public string attackMissedNoQTEDamageMessage1 =
	// public string attackMissedNoQTEDamageMessage2 =

	// Defend message
	public string defendMessage = " defends themself until their next turn!";
	public string GetDefendMessage() {
		string message = name + defendMessage;
		return message;
	}

	// Run messages
	public string runSuccessMessage = " ran away!\nCOWARD! HO HO HO!";
	public string GetRunSuccessMessage() {
		string message = name + runSuccessMessage;
		return message;
	}
	public string runFailureMessage = " attempts to run...\n...but the party has blocked the path!";
	public string GetRunFailureMessage() {
		string message = name + runFailureMessage;
		return message;
	}

	// Stunned message
	public string stunnedMessage = " is stunned and doesn't move!\nWhat a rube!";
	public string GetStunnedMessage() {
		string message = name + stunnedMessage;
		return message;
	}

	// Heal messages
	// public string healButNoMPMessage = " attempts to cast a Heal Spell...\n...But doesn't have enough MP to do so!";
	// public string healSelfToMaxMessage = " casts a Heal Spell!\nHealed itself back to Max HP!";
	// public string healOtherToMaxMessageA = ""; 
	// public string healOtherToMaxMessageB = ""; 
	// public string healSelfMessageA = ""; 
	// public string healSelfMessageB = ""; 
	// public string healOtherMessageA = ""; 
	// public string healOtherMessageB = ""; 

	// Attack single messages
	// public string attackSingleButNoMPMessage = " attempts to cast Fireball...\n...But doesn't have enough MP to do so!";
	// public string attackSingleMissedMessage1 = " attempted to cast Fireball... but missed the party completely!";
	// public string attackSingleMissedMessage2 = " cast Fireball, but the party deftly dodged out of the way!";
	// public string attackSingleHitMessageA = "";
	// public string attackSingleHitMessageB = "";

	// Attack all messages
	public string attackAllButNoMPMessage = " attempts to cast Fireblast...\n...But doesn't have enough MP to do so!";
	public string GetAttackAllButNoMPMessage() {
		string message = name + attackAllButNoMPMessage;
		return message;
	}
	public string attackAllMissedMessage1 = " attempted to cast Fireblast... but missed the party completely!";
	public string GetAttackAllMissedMessage1() {
		string message = name + attackAllMissedMessage1;
		return message;
	}
	public string attackAllMissedMessage2 = " cast Fireblast, but the party deftly dodged out of the way!";
	public string GetAttackAllMissedMessage2() {
		string message = name + attackAllMissedMessage2;
		return message;
	}
	public string attackAllHitMessageA = "Used Fireblast Spell!\nHit ENTIRE party for an average of ";
	public string attackAllHitMessageB = " HP!";
	public string GetAttackAllHitMessage(int a, int b) {
		string message = attackAllHitMessageA + Utilities.S.CalculateAverage(a, b) + attackAllHitMessageB;
		return message;
	}

	// Call for backup messages
	public string callForBackupNextTurnMessage = " is getting ready to call for help!";
	public string GetCallForBackupNextTurnMessage() {
		string message = name + callForBackupNextTurnMessage;
		return message;
	}
	public string callForBackupFailureMessage = " called for backup...\n...but no one came!";
	public string GetCallForBackupFailureMessage() {
		string message = name + callForBackupFailureMessage;
		return message;
	}
	public string callForBackupSuccessMessage = " called for backup...\n...and someone came!";
	public string GetCallForBackupSuccessMessage() {
		string message = name + callForBackupSuccessMessage;
		return message;
	}

	// Charge message
	public string chargeMessage = " is getting ready to do something cool...\n...what could it be?!";
	public string GetChargeMessage() {
		string message = name + chargeMessage;
		return message;
	}

	// Steal messages
	public string stealSuccessMessage = " swiped a ";
	public string GetStealSuccessMessage(string itemName, string targetName) {
		string message = name + stealSuccessMessage + itemName + " from " + targetName + ".\n" + WordManager.S.GetRandomInterjection() + "!";
		return message;
	}
	public string stealButImportantItemMessageA = " attempted to steal a ";
	public string stealButImportantItemMessageB = "...\n...but it can't be stolen!\n";
	public string GetStealButImportantItemMessage(string itemName, string targetName) {
		string message = name + stealButImportantItemMessageA + itemName + " from " + targetName + stealButImportantItemMessageB + WordManager.S.GetRandomExclamation() + "!";
		return message;
	}
	public string stealFailureMessageA = " attempted to loot an item from ";
	public string stealFailureMessageB = "...\n...but missed the mark!\n";
	public string GetStealFailureMessage(string targetName) {
		string message = name + stealFailureMessageA + targetName + stealFailureMessageB + WordManager.S.GetRandomExclamation() + "!";
		return message;
	}
	public string stealButTargetHasNothingMessageA = " attempted to steal an item from ";
	public string stealButTargetHasNothingMessageB = "...\n...but they've got nothing!\n";
	public string GetStealButTargetHasNothingMessage(string targetName) {
		string message = name + stealButTargetHasNothingMessageA + targetName + stealButTargetHasNothingMessageB + WordManager.S.GetRandomExclamation() + "!";
		return message;
	}
}