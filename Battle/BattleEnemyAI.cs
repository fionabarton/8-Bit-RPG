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
    public void EnemyAI(int enemyId) {
        _.playerActions.ButtonsDisableAll();

        switch (enemyId) {
            case 0:

                break;
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
            case 4:

                break;
            case 5:

                break;
            case 6:

                break;
            case 7:

                break;
            case 8:

                break;
        }
        //ChanceToCallMove(13,9);

        // Call for backup
        ChanceToCallMove(7);

        // Select Random Move
        //CallRandomMove();

        // Attack OR Defend
        //ChanceToCallMove(0, 1);


        // Steal OR Attack
        //ChanceToCallMove(13,0);

        // _.enemyActions.AttemptSteal();
        // _.enemyActions.Defend();

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

        //ChanceToCallMove(7);

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

        //switch (_.enemyStats[_.EnemyNdx()].AI) {
        //    case eEnemyAI.FightWisely:
        //        // If HP is less than 10%...
        //        if (Utilities.S.GetPercentage(_.enemyStats[_.EnemyNdx()].HP, _.enemyStats[_.EnemyNdx()].maxHP) < 0.1f) {
        //            // If MP is less than 10%...
        //            if (Utilities.S.GetPercentage(_.enemyStats[_.EnemyNdx()].MP, _.enemyStats[_.EnemyNdx()].maxMP) < 0.1f) {
        //                // Run OR Defend
        //                ChanceToCallMove(2, 1);
        //            } else {
        //                // Heal Spell OR Defend
        //                ChanceToCallMove(4, 1);
        //            }
        //        } else {
        //            // If MP is less than 10%...
        //            if (Utilities.S.GetPercentage(_.enemyStats[_.EnemyNdx()].MP, _.enemyStats[_.EnemyNdx()].maxMP) < 0.1f) {
        //                // Attack OR Defend
        //                ChanceToCallMove(0, 1);
        //            } else {
        //                // Attack All Spell OR Attack
        //                ChanceToCallMove(5, 0);
        //            }
        //        }
        //        break;
        //    case eEnemyAI.FocusOnAttack:
        //        // If MP is less than 10%...
        //        if (Utilities.S.GetPercentage(_.enemyStats[_.EnemyNdx()].MP, _.enemyStats[_.EnemyNdx()].maxMP) < 0.1f) {
        //            // Attack
        //            ChanceToCallMove(0);
        //        } else {
        //            if (_.partyQty == 0) {
        //                // Attack Single or Attack
        //                ChanceToCallMove(8, 0);
        //            } else {
        //                // Attack All OR Attack Single 
        //                ChanceToCallMove(5, 8);
        //            }
        //        }
        //        break;
        //    case eEnemyAI.FocusOnDefend:
        //        // Defend or Run
        //        ChanceToCallMove(1, 2);
        //        break;
        //    case eEnemyAI.FocusOnHeal:
        //        // If any enemy's HP is less than 25%...
        //        if (_.stats.EnemiesNeedsHeal()) {
        //            // If MP is less than 10%...
        //            if (Utilities.S.GetPercentage(_.enemyStats[_.EnemyNdx()].MP, _.enemyStats[_.EnemyNdx()].maxMP) < 0.1f) {
        //                // Defend OR Run 
        //                ChanceToCallMove(1, 2);
        //            } else {
        //                // Heal Spell OR Defend
        //                ChanceToCallMove(4, 1);
        //            }
        //        } else {
        //            // Attack OR Defend
        //            ChanceToCallMove(0, 1);
        //        }
        //        break;
        //    case eEnemyAI.Random:
        //        // Select Random Move
        //        CallRandomMove();
        //        break;
        //    case eEnemyAI.RunAway:
        //        // Run or Defend
        //        ChanceToCallMove(2, 1);
        //        break;
        //    case eEnemyAI.CallForBackup:
        //        // Call for backup next turn
        //        if (Random.value < _.enemyStats[_.EnemyNdx()].chanceToCallMove / 3) {
        //            ChanceToCallMove(7);
        //        } else {
        //            // Attack or Defend 
        //            ChanceToCallMove(0, 1);
        //        }
        //        break;
        //    case eEnemyAI.Charge:
        //        if (_.roundNdx % 3 == 0) { // Every third round...
        //            if (_.partyQty == 0) {
        //                // Attack Single or Attack
        //                ChanceToCallMove(8, 0);
        //            } else {
        //                // Attack All OR Attack Single 
        //                ChanceToCallMove(5, 8);
        //            }
        //        } else {
        //            _.enemyActions.Charge();
        //        }
        //        break;
        //    case eEnemyAI.DontUseMP:
        //    default:
        //        break;
        //}
    }

    // If Enemy is lucky, check if it has desired move
    void ChanceToCallMove(int firstChoiceNdx, int secondChoiceNdx = 999) {
        // If lucky, call first choice move...
        if (Random.value < _.enemyStats[_.EnemyNdx()].chanceToCallMove) {
            // Loop through the Enemy's Moves, check if Enemy knows move
            for (int i = 0; i < _.enemyStats[_.EnemyNdx()].moveList.Count; i++) {
                if (firstChoiceNdx == _.enemyStats[_.EnemyNdx()].moveList[i]) {
                    CallEnemyMove(firstChoiceNdx);
                    return;
                }
            }
        } else {
            // ...otherwise test your on luck on second choice move...
            if (secondChoiceNdx != 999) {
                ChanceToCallMove(secondChoiceNdx);
                return;
            }
        }

        // If all else fails, call a random move
        CallRandomMove();
    }

    // Randomly select move that the Enemy knows
    void CallRandomMove() {
        // Get random index
        int randomNdx = _.enemyStats[_.EnemyNdx()].moveList[Random.Range(0, _.enemyStats[_.EnemyNdx()].moveList.Count)];

        // Call random move 
        CallEnemyMove(randomNdx);
    }

    public void CallEnemyMove(int moveNdx) {
        switch (moveNdx) {
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
            //case 10: _.enemyActions.Poison(); break;
            //case 11: _.enemyActions.Paralyze(); break;
            //case 12: _.enemyActions.Sleep(); break;
            case 13: _.enemyActions.AttemptSteal(); break;
            default: _.enemyActions.Attack(); break;
        }
    }
}