using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTrigger : ActivateOnButtonPress {
	protected override void Action() {
        // Set Camera to Item gameObject
        CamManager.S.ChangeTarget(gameObject, true);

        // Get and position Poof game object
        GameObject poof = ObjectPool.S.GetPooledObject("Poof");
        ObjectPool.S.PosAndEnableObj(poof, gameObject);

        // Interactable Trigger (without this, occasionally results in console warning)
        InteractableCursor.S.Deactivate();

        SaveMenu.S.Activate();
	}

    public void ThisLoop() {
        // Remove ThisLoop() from UpdateManager delegate on scene change.
        // This prevents an occasional bug when the Player is within this trigger on scene change.
        // Would prefer a better solution... 
        if (!GameManager.S.canInput) {
            UpdateManager.updateDelegate -= ThisLoop;
        }

        if (SaveMenu.S.gameObject.activeInHierarchy) {
            if (SaveMenu.S.saveScreenMode == eSaveScreenMode.pickAction) {
                if (Input.GetButtonDown("SNES Y Button")) {
                    ResetTrigger();
                }
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.CompareTag("PlayerTrigger")) {
            // Add ThisLoop() to Update Delgate
            UpdateManager.updateDelegate += ThisLoop;

            base.OnTriggerEnter2D(coll);
        }
    }

    protected override void OnTriggerExit2D(Collider2D coll) {
        if (coll.gameObject.CompareTag("PlayerTrigger")) {
            base.OnTriggerExit2D(coll);

            // Remove ThisLoop() from Update Delgate
            UpdateManager.updateDelegate -= ThisLoop;
        }
    }
}