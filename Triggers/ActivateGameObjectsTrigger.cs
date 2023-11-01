using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// On collision activates and deactivates two unique lists of game objects.
public class ActivateGameObjectsTrigger : ActivateOnCollision {
    [Header("Set in Inspector")]
    public List<GameObject> objectsToActivate = new List<GameObject>();
    public List<GameObject> objectsToDeactivate = new List<GameObject>();

    public bool             setPlayerIsOnLowerLevel = false;
    public bool             activateOnStart = false;

    // Specifically useful for activating bounds game objects depending on the player's ground level
    private void Start() {
        if (activateOnStart) {
            if (Player.S.isOnLowerLevel) {
                // Deactivate list of game objects
                Utilities.S.SetActiveList(objectsToActivate, false);

                // Activate list of game objects
                Utilities.S.SetActiveList(objectsToDeactivate, true);
            } else {
                // Activate list of game objects
                Utilities.S.SetActiveList(objectsToActivate, true);

                // Deactivate list of game objects
                Utilities.S.SetActiveList(objectsToDeactivate, false);
            }
        }
    }

    protected override void Action() {
        // Activate list of game objects
        Utilities.S.SetActiveList(objectsToActivate, true);

        // Deactivate list of game objects
        Utilities.S.SetActiveList(objectsToDeactivate, false);

        // Cache whether the player is on a lower or higher ground level
        Player.S.isOnLowerLevel = setPlayerIsOnLowerLevel;
    }
}