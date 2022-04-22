using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : ActivateOnCollision {
    [Header("Set in Inspector")]
    public bool canEncounter = false;
    public int  encounterRate = 24;
    public int  locationNdx = 0;

    protected override void Action() {
        // Enable random encounters
        Blob.S.canEncounter = canEncounter;

        // Set encounter rate
        Blob.S.encounterRate = encounterRate;

        // Set location
        Blob.S.locationNdx = locationNdx;
        
        // Get/set enemies based on location
        Blob.S.enemyStats = Blob.S.enemyManager.GetEnemies(locationNdx);
    }
}