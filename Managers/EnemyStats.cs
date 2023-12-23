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
	// Attack messages ///////////////////////////////////////////////////////////////////////
	// Attack: missed, but QTE Damage
	public string attackMissedButQTEDamageMessage1 = "'s attack attempt nearly failed, but scraped ";
	public string GetAttackMissedButQTEDamageMessage1(string targetName, int attackDamage) {
		string message = name + attackMissedButQTEDamageMessage1 + targetName + " for " + attackDamage + " points!";
		return message;
	}
	public string attackMissedButQTEDamageMessage2 = " nearly missed the mark, but knicked ";
	public string GetAttackMissedButQTEDamageMessage2(string targetName, int attackDamage) {
		string message = name + attackMissedButQTEDamageMessage2 + targetName + " for " + attackDamage + " points!";
		return message;
	}
	// Attack: missed, no QTE Damage
	public string attackMissedNoQTEDamageMessage1A = " attempted to attack ";
	public string attackMissedNoQTEDamageMessage1B = "... but missed!";
	public string GetAttackMissedNoQTEDamageMessage1(string targetName) {
		string message = name + attackMissedNoQTEDamageMessage1A + targetName + attackMissedNoQTEDamageMessage1B;
		return message;
	}
	public string attackMissedNoQTEDamageMessage2A = " missed the mark! ";
	public string attackMissedNoQTEDamageMessage2B = " dodged out of the way!";
	public string GetAttackMissedNoQTEDamageMessage2(string targetName) {
		string message = name + attackMissedNoQTEDamageMessage2A + targetName + attackMissedNoQTEDamageMessage2B;
		return message;
	}
	// Attack: critical hit
	public string attackCriticalHitMessageA = "Critical hit!\n";
	public string attackCriticalHitMessageB = " struck ";
	public string GetAttackCriticalHitMessage(string targetName, int attackDamage) {
		string message = attackCriticalHitMessageA + name + attackCriticalHitMessageB + targetName + " for " + attackDamage + " points!";
		return message;
	}
	// Attack: normal hit
	public string attackHitMessage = " struck ";
	public string GetAttackHitMessage(string targetName, int attackDamage) {
		string message = name + attackHitMessage + targetName + " for " + attackDamage + " points!";
		return message;
	}

	// Defend message ///////////////////////////////////////////////////////////////////////
	public string defendMessage = " defends themself until their next turn!";
	public string GetDefendMessage() {
		string message = name + defendMessage;
		return message;
	}

	// Run messages ///////////////////////////////////////////////////////////////////////
	// Run: success
	public string runSuccessMessage = " ran away!\nCOWARD! HO HO HO!";
	public string GetRunSuccessMessage() {
		string message = name + runSuccessMessage;
		return message;
	}
	// Run: failure
	public string runFailureMessage = " attempts to run...\n...but the party has blocked the path!";
	public string GetRunFailureMessage() {
		string message = name + runFailureMessage;
		return message;
	}

	// Stunned message ///////////////////////////////////////////////////////////////////////
	public string stunnedMessage = " is stunned and doesn't move!\nWhat a rube!";
	public string GetStunnedMessage() {
		string message = name + stunnedMessage;
		return message;
	}

	// Heal messages ///////////////////////////////////////////////////////////////////////
	// Heal: no MP
	public string healButNoMPMessage = " attempts to cast a Heal Spell...\n...But doesn't have enough MP to do so!";
	public string GetHealButNoMPMessage() {
		string message = name + healButNoMPMessage;
		return message;
	}
	// Heal: self to max
	public string healSelfToMaxMessage = " casts a Heal Spell!\nHealed itself back to Max HP!";
	public string GetHealSelfToMaxMessage() {
		string message = name + healSelfToMaxMessage;
		return message;
	}
	// Heal: other to max
	public string healOtherToMaxMessageA = " casts a Heal Spell!\nHealed "; 
	public string healOtherToMaxMessageB = " back to Max HP!"; 
	public string GetHealOtherToMaxMessage(string targetName) {
		string message = name + healOtherToMaxMessageA + targetName + healOtherToMaxMessageB;
		return message;
	}
	// Heal: self 
	public string healSelfMessageA = " casts a Heal Spell!\nHealed itself for "; 
	public string healSelfMessageB = " HP!"; 
	public string GetHealSelfMessage(int amountToHeal) {
		string message = name + healSelfMessageA + amountToHeal + healSelfMessageB;
		return message;
	}
	// Heal: other
	public string healOtherMessageA = " casts a Heal Spell!\nHealed "; 
    public string healOtherMessageB = " HP!"; 
	public string GetHealOtherMessage(string targetName, int amountToHeal) {
		string message = name + healOtherMessageA + targetName + " for " + amountToHeal + healOtherMessageB;
		return message;
	}

	// Attack single messages ///////////////////////////////////////////////////////////////////////
	// Attack single: no MP
	public string attackSingleButNoMPMessage = " attempts to cast Fireball...\n...But doesn't have enough MP to do so!";
	public string GetAttackSingleButNoMPMessage() {
		string message = name + attackSingleButNoMPMessage;
		return message;
	}
	// Attack single: missed
	public string attackSingleMissedMessage1 = " attempted to cast Fireball... but missed the party completely!";
	public string GetAttackSingleMissedMessage1() {
		string message = name + attackSingleMissedMessage1;
		return message;
	}
	public string attackSingleMissedMessage2 = " cast Fireball, but the party deftly dodged out of the way!";
	public string GetAttackSingleMissedMessage2() {
		string message = name + attackSingleMissedMessage2;
		return message;
	}
	// Attack single: hit
	public string attackSingleHitMessageA = "Used Fireball Spell!\nHit ";
	public string attackSingleHitMessageB = " HP!";
	public string GetAttackSingleHitMessage(string targetName, int attackDamage) {
		string message = attackSingleHitMessageA + targetName + " for " + attackDamage + attackSingleHitMessageB;
		return message;
	}

	// Attack all messages ///////////////////////////////////////////////////////////////////////
	// Attack all: no MP
	public string attackAllButNoMPMessage = " attempts to cast Fireblast...\n...But doesn't have enough MP to do so!";
	public string GetAttackAllButNoMPMessage() {
		string message = name + attackAllButNoMPMessage;
		return message;
	}
	// Attack single: missed
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
	// Attack single: hit
	public string attackAllHitMessageA = "Used Fireblast Spell!\nHit ENTIRE party for an average of ";
	public string attackAllHitMessageB = " HP!";
	public string GetAttackAllHitMessage(int a, int b) {
		string message = attackAllHitMessageA + Utilities.S.CalculateAverage(a, b) + attackAllHitMessageB;
		return message;
	}

	// Call for backup messages ///////////////////////////////////////////////////////////////////////
	// Call for backup: getting ready
	public string callForBackupNextTurnMessage = " is getting ready to call for help!";
	public string GetCallForBackupNextTurnMessage() {
		string message = name + callForBackupNextTurnMessage;
		return message;
	}
	// Call for backup: failure
	public string callForBackupFailureMessage = " called for backup...\n...but no one came!";
	public string GetCallForBackupFailureMessage() {
		string message = name + callForBackupFailureMessage;
		return message;
	}
	// Call for backup: success
	public string callForBackupSuccessMessage = " called for backup...\n...and someone came!";
	public string GetCallForBackupSuccessMessage() {
		string message = name + callForBackupSuccessMessage;
		return message;
	}

	// Charge message ///////////////////////////////////////////////////////////////////////
	public string chargeMessage = " is getting ready to do something cool...\n...what could it be?!";
	public string GetChargeMessage() {
		string message = name + chargeMessage;
		return message;
	}

	// Steal messages ///////////////////////////////////////////////////////////////////////
	// Steal: success
	public string stealSuccessMessage = " swiped a ";
	public string GetStealSuccessMessage(string itemName, string targetName) {
		string message = name + stealSuccessMessage + itemName + " from " + targetName + ".\n" + WordManager.S.GetRandomInterjection() + "!";
		return message;
	}
	// Steal: but important item
	public string stealButImportantItemMessageA = " attempted to steal a ";
	public string stealButImportantItemMessageB = "...\n...but it can't be stolen!\n";
	public string GetStealButImportantItemMessage(string itemName, string targetName) {
		string message = name + stealButImportantItemMessageA + itemName + " from " + targetName + stealButImportantItemMessageB + WordManager.S.GetRandomExclamation() + "!";
		return message;
	}
	// Steal: failure
	public string stealFailureMessageA = " attempted to loot an item from ";
	public string stealFailureMessageB = "...\n...but missed the mark!\n";
	public string GetStealFailureMessage(string targetName) {
		string message = name + stealFailureMessageA + targetName + stealFailureMessageB + WordManager.S.GetRandomExclamation() + "!";
		return message;
	}
	// Steal: but target has nothing
	public string stealButTargetHasNothingMessageA = " attempted to steal an item from ";
	public string stealButTargetHasNothingMessageB = "...\n...but they've got nothing!\n";
	public string GetStealButTargetHasNothingMessage(string targetName) {
		string message = name + stealButTargetHasNothingMessageA + targetName + stealButTargetHasNothingMessageB + WordManager.S.GetRandomExclamation() + "!";
		return message;
	}
}