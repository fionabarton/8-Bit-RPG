using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTrigger : ActivateOnButtonPress {
	protected override void Action() {
        // Set Camera to SaveTrigger gameObject
        CamManager.S.ChangeTarget(gameObject, true);

        // Set SubMenu Text
        GameManager.S.gameSubMenu.SetText("Yes", "No");

        DialogueManager.S.DisplayText("What up, babe?\nDo you feel like making a call to\nsave, load, or delete your game file?");

        // Activate Sub Menu after Dialogue 
        DialogueManager.S.activateSubMenu = true;
        // Don't activate Text Box Cursor 
        DialogueManager.S.dontActivateCursor = true;

        // Set OnClick Methods
        Utilities.S.RemoveListeners(GameManager.S.gameSubMenu.buttonCS);
        GameManager.S.gameSubMenu.buttonCS[0].onClick.AddListener(Yes);
        GameManager.S.gameSubMenu.buttonCS[1].onClick.AddListener(No);

        // Set button navigation
        Utilities.S.SetButtonNavigation(GameManager.S.gameSubMenu.buttonCS[0], GameManager.S.gameSubMenu.buttonCS[1], GameManager.S.gameSubMenu.buttonCS[1]);
        Utilities.S.SetButtonNavigation(GameManager.S.gameSubMenu.buttonCS[1], GameManager.S.gameSubMenu.buttonCS[0], GameManager.S.gameSubMenu.buttonCS[0]);

        // Interactable Trigger (without this, occasionally results in console warning)
        InteractableCursor.S.Deactivate();
    }

    public void Yes() {
        SaveMenu.S.Activate();
    }

    public void No() {
        AudioManager.S.PlaySFX(eSoundName.deny);

        DialogueManager.S.ResetSettings();
        DialogueManager.S.DisplayText("That's cool. Later, babe.");
    }

    public void ThisLoop() {
        // Remove ThisLoop() from UpdateManager delegate on scene change.
        // This prevents an occasional bug when the Player is within this trigger on scene change.
        // Would prefer a better solution... 
        if (!GameManager.S.canInput) {
            UpdateManager.updateDelegate -= ThisLoop;
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