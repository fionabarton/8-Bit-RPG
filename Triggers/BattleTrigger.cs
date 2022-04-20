using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : ActivateOnCollision {
    [Header("Set in Inspector")]
    public bool canEncounter = false;

    public int encounterRate = 24;

    protected override void Action() {
        // Enable random encounters
        Blob.S.canEncounter = canEncounter;

        // Set encounter rate
        Blob.S.encounterRate = encounterRate;
    }
}