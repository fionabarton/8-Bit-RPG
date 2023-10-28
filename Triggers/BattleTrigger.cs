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
        Player.S.canEncounter = canEncounter;

        // Set encounter rate
        Player.S.encounterRate = encounterRate;

        // Set location
        Player.S.locationNdx = locationNdx;
        
        // Get/set enemies based on location
        Player.S.enemyStats = Player.S.enemyManager.GetEnemies(locationNdx);
    }

    protected override void OnTriggerExit2D(Collider2D coll) {
        base.OnTriggerExit2D(coll);

        Player.S.canEncounter = false;
    }
}