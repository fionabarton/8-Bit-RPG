using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemyAI : MonoBehaviour {
    [Header("Set Dynamically")]
    private Battle _;

    void Start() {
        _ = Battle.S;
    }

    // More Spells, Use Items, Call for Backup
    public void EnemyAI() {
        _.playerActions.ButtonsDisableAll();

        // - Heal when needed OR valid
        // - Attack strongest/weakest
        // - Change AI based on status (ex. on the verge of death, an enemy gets more aggressive)

        // Cache data
        int enemyHP = _.enemyStats[_.EnemyNdx()].HP;
        int enemyMP = _.enemyStats[_.EnemyNdx()].MP;
        // int enemyHPToMax (maxHP - HP)

        // TESTING: Set target, then call action
        //_.targetNdx = _.stats.GetPlayerWithLowestHP();
        //ChanceToCallAction(eAction.attackAll);

        switch (_.enemyStats[_.EnemyNdx()].AI_id) {
            case 0:
                if (Random.value > 0.5f) {

                } else {

                }
                break;
            case 1:
                if (enemyHP % 2 == 0) {

                } else {

                }
                break;
            case 2:

                break;
            // Fight wisely ///////////////////////////////////////////////////////
            case 3:
                // Defend/Heal AI

                // Use heal spell when needed:
                // If any enemy's HP < 25%...
                if (_.stats.EnemiesNeedHeal(0.25f)) {
                    // If any enemy's HP < 30...
                    if (_.stats.EnemiesNeedHeal(30)) {
                        // If enemy knows heal...
                        if (KnowsAction(eAction.heal) && enemyMP >= 3) {
                            // Set target
                            //_.targetNdx = _.stats.GetEnemyWithLowestHP();

                            // Heal or defend
                            ChanceToCallAction(eAction.heal, eAction.defend);
                            return;
                        } else {
                            // Run or defend
                            ChanceToCallAction(eAction.run, eAction.defend);
                            return;
                        }
                    }
                }

                // Attack AI:

                // Set target
                _.targetNdx = _.stats.GetRandomPlayerNdx();

                // If there is only one party member...
                if (_.partyQty == 0) {
                    // If enemy knows attack single...
                    if (KnowsAction(eAction.attackSingle) && enemyMP >= 1) {
                        // Attack single or attack
                        ChanceToCallAction(eAction.attackSingle, eAction.attack);
                    } else {
                        // Attack 
                        ChanceToCallAction(eAction.attack);
                    }
                } else {
                    // If enemy knows attack all...
                    if (KnowsAction(eAction.attackAll) && enemyMP >= 3) {
                        // Attack all or attack single 
                        ChanceToCallAction(eAction.attackAll, eAction.attackSingle);
                    } else {
                        // Attack 
                        ChanceToCallAction(eAction.attack);
                    }
                }
                break;
            // CallForBackup ///////////////////////////////////////////////////////
            case 4:
                if (Random.value < _.enemyStats[_.EnemyNdx()].chanceToCallAction / 3) {
                    ChanceToCallAction(eAction.callForBackupNextTurn);
                } else {
                    // Set target
                    _.targetNdx = _.stats.GetRandomPlayerNdx();

                    // Attack or defend 
                    ChanceToCallAction(eAction.attack, eAction.defend);
                }
                break;
            // Charge //////////////////////////////////////////////////////////////
            case 5:
                // Set target
                _.targetNdx = _.stats.GetRandomPlayerNdx();

                if (_.roundNdx % 3 == 0) { // Every third round...
                    if (_.partyQty == 0) {
                        // Attack single or attack
                        ChanceToCallAction(eAction.attackSingle, eAction.attack);
                    } else {
                        // Attack all or attack single 
                        ChanceToCallAction(eAction.attackAll, eAction.attackSingle);
                    }
                } else {
                    _.enemyActions.Charge();
                }
                break;
            default:
                // Set target
                _.targetNdx = _.stats.GetRandomPlayerNdx();

                CallRandomAction();
                break;
        }
    }

    // If Enemy is lucky, check if it has desired action
    void ChanceToCallAction(eAction firstChoice, eAction secondChoice = eAction.empty) {
        int firstChoiceNdx = (int)firstChoice;

        // If lucky, call first choice action...
        if (Random.value < _.enemyStats[_.EnemyNdx()].chanceToCallAction) {
            // Loop through the enemy's actions, check if enemy knows action
            for (int i = 0; i < _.enemyStats[_.EnemyNdx()].actionList.Count; i++) {
                if (firstChoiceNdx == _.enemyStats[_.EnemyNdx()].actionList[i]) {
                    CallEnemyAction(firstChoiceNdx);
                    return;
                }
            }
        } else {
            // ...otherwise test your on luck on second choice action...
            if (secondChoice != eAction.empty) {
                ChanceToCallAction(secondChoice);
                return;
            }
        }

        // If all else fails, call a random action
        CallRandomAction();
    }

    // Randomly select action that the enemy knows
    void CallRandomAction() {
        // Get random index
        int randomNdx = _.enemyStats[_.EnemyNdx()].actionList[Random.Range(0, _.enemyStats[_.EnemyNdx()].actionList.Count)];

        // Call random action 
        CallEnemyAction(randomNdx);
    }

    // Returns true if enemy knows move
    bool KnowsAction(eAction enemyAction) {
        int ndx = (int)enemyAction;

        //Debug.Log(ndx);

        for (int i = 0; i < _.enemyStats[_.EnemyNdx()].actionList.Count; i++) {
            if (ndx == _.enemyStats[_.EnemyNdx()].actionList[i]) {
                return true;
            }
        }
        return false;
    }

    public void CallEnemyAction(int actionNdx) {
        // Order must match order found in EnumManager.eAction
        switch (actionNdx) {
            case 0: _.enemyActions.Attack(); break;
            case 1: _.enemyActions.Defend(); break;
            case 2: _.enemyActions.Run(); break;
            case 3: _.enemyActions.Stunned(); break;
            case 4: _.enemyActions.AttemptHealSpell(); break;
            case 5: _.enemyActions.AttemptAttackAll(); break;
            case 6: _.enemyActions.CallForBackup(); break;
            case 7: _.enemyActions.CallForBackupNextTurn(); break;
            case 8: _.enemyActions.AttemptAttackSingle(); break;
            case 9: _.enemyActions.Charge(); break;
            case 10: _.enemyActions.Poison(); break;
            case 11: _.enemyActions.Paralyze(); break;
            case 12: _.enemyActions.Sleep(); break;
            case 13: _.enemyActions.AttemptSteal(); break;
            default: _.enemyActions.Attack(); break;
        }
        //Debug.Log(actionNdx);
    }
}

//////////////////////////////////////////////////////////////////
// STATUS AILMENTS 
//////////////////////////////////////////////////////////////////

//// Poison
//int playerToPoison = Battle.S.stats.GetRandomPlayerNdx();
//if (StatusEffects.S.CheckIfPoisoned(true, playerToPoison)) {
//    // Attack OR Defend
//    ChanceToCallMove(0, 1);
//} else {
//    //if (Random.value < 0.5f) {
//    //    _.enemyActions.Poison(playerToPoison);
//    //} else {
//    //    // Attack OR Defend
//    //    ChanceToCallMove(0, 1);
//    //}
//    _.enemyActions.Poison(playerToPoison);
//}
//return;

//////////////////////////////////////////////////////////////////

//// Paralyze
//int playerToParalyze = Battle.S.stats.GetRandomPlayerNdx();
//if (StatusEffects.S.CheckIfParalyzed(true, playerToParalyze)) {
//    // Attack OR Defend
//    ChanceToCallMove(0, 1);
//} else {
//    //if (Random.value < 0.5f) {
//    //    _.enemyActions.Paralyze(playerToParalyze);
//    //} else {
//    //    // Attack OR Defend
//    //    ChanceToCallMove(0, 1);
//    //}
//    _.enemyActions.Paralyze(playerToParalyze);
//}
//return;
//////////////////////////////////////////////////////////////////

//// Sleep
//int playerToSleep = Battle.S.stats.GetRandomPlayerNdx();
//if (StatusEffects.S.CheckIfSleeping(true, playerToSleep)) {
//    // Attack OR Defend
//    ChanceToCallMove(0, 1);
//} else {
//    //if (Random.value < 0.5f) {
//    //    _.enemyActions.Sleep(playerToSleep);
//    //} else {
//    //    // Attack OR Defend
//    //    ChanceToCallMove(0, 1);
//    //}
//    _.enemyActions.Sleep(playerToSleep);
//}
//return;
//////////////////////////////////////////////////////////////////