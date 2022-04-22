using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    [Header("Set in Inspector")]
    public List<EnemyStats> enemies0;
    public List<EnemyStats> enemies1;

	public List<EnemyStats> GetEnemies(int locationNdx) {
        // Clear enemy list
        Blob.S.enemyStats.Clear();

        // Declare local variables
        List<EnemyStats> tEnemies = new List<EnemyStats>();
        int randomNdx;

        // Populate list of randomly selected enemies from this location
        switch (locationNdx) {
            case 0:
                for(int i = 0; i < 5; i++) {
                    randomNdx = Random.Range(0, enemies0.Count);
                    tEnemies.Add(enemies0[randomNdx]);
                }
                break;
            case 1:
                for (int i = 0; i < 5; i++) {
                    randomNdx = Random.Range(0, enemies1.Count);
                    tEnemies.Add(enemies1[randomNdx]);
                }
                break;
        }

        // Return list of randomly selected enemies from this location
        return tEnemies;
    } 
}