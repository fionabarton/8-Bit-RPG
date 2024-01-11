using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    [Header("Set in Inspector")]
    public List<EnemyStats> enemies0;
    public List<EnemyStats> enemies1;
    public List<EnemyStats> enemies2;

    public List<EnemyStats> GetEnemies(int locationNdx) {
        // Clear enemy list
        Player.S.enemyStats.Clear();

        // Declare local variables
        List<EnemyStats> tEnemies = new List<EnemyStats>();
        int randomNdx;

        // Populate list of randomly selected enemies from this location
        switch (locationNdx) {
            case 0:
                for(int i = 0; i < 5; i++) {
                    // Randomly select enemies from this group
                    randomNdx = Random.Range(0, enemies0.Count);
                    tEnemies.Add(enemies0[randomNdx]);
                }

                // Randomize enemy amount
                Player.S.enemyAmount = 999;
                break;
            case 1:
                for (int i = 0; i < 5; i++) {
                    // Randomly select enemies from this group
                    randomNdx = Random.Range(0, enemies1.Count);
                    tEnemies.Add(enemies1[randomNdx]);
                }

                // Randomize enemy amount
                Player.S.enemyAmount = 999;
                break;
            case 2:
                // Get random float
                float randomVal = Random.value;

                // Randomly select a group of enemies
                //if(randomVal >= 0.5f) {
                //    for (int i = 0; i < 5; i++) {
                //        // Explicitly select enemies from this group
                //        tEnemies.Add(enemies0[i]);
                //    }

                //    // Explictly set enemy amount
                //    Player.S.enemyAmount = 1;
                //} else {
                    for (int i = 0; i < 5; i++) {
                        // Randomly select enemies from this group
                        randomNdx = Random.Range(0, enemies2.Count);
                        tEnemies.Add(enemies2[randomNdx]);
                    }

                    // Randomize enemy amount
                    Player.S.enemyAmount = 999;
                //}              
                break;
        }

        // Return list of randomly selected enemies from this location
        return tEnemies;
    } 
}