using UnityEngine;

/// <summary>
/// Trigger that performs an action implemented its child class OnButtonPress
/// </summary>
public class ActivateOnButtonPress : MonoBehaviour {
    [Header("Set in Inspector")]
    // Activate Interactable Cursor OnTriggerEnter2D
    public bool activateInteractableCursor = true;

    // Prevent trigger from being reset (used on "Toiletron" cutscene trigger)
    public bool canBeReset = true;

    [Header("Set Dynamically")]
    // Used in DialogueTrigger to change trigger's action after it's been pressed once
    public bool firstButtonPressed;

    // Used after a door is opened and the trigger isn't needed anymore
    public bool triggerHasBeenDeactivated;

    // 
    public DialogueTrigger t;
    public Vector2 v;

    private void Start() {
        t = GetComponent<DialogueTrigger>();
        v = gameObject.transform.position;
    }

    void OnDisable() {
        // Remove Update Delgate
        UpdateManager.updateDelegate -= Loop;
    }

    protected virtual void OnTriggerEnter2D(Collider2D coll) {
        if (!triggerHasBeenDeactivated) {
            if (!Player.S.alreadyTriggered) {
                if (!GameManager.S.paused) {
                    if (coll.gameObject.CompareTag("PlayerTrigger")) {

                        //
                        v = gameObject.transform.position;


                        // Prevents triggering multiple triggers
                        Player.S.alreadyTriggered = true;

                        // Activate Interactable Trigger
                        if (activateInteractableCursor) {
                            InteractableCursor.S.Activate(gameObject);
                        }

                        // Add Update Delgate
                        UpdateManager.updateDelegate += Loop;
                    }
                }
            }
        }
	}

	protected virtual void OnTriggerExit2D(Collider2D coll) {
        if (coll.gameObject.CompareTag("PlayerTrigger")) {
            firstButtonPressed = false;

            // Deactivate Interactable Trigger
            if (InteractableCursor.S.cursorGO) {
				InteractableCursor.S.Deactivate();
            }

            // Remove Update Delgate
            UpdateManager.updateDelegate -= Loop;

            // Unsubscribe ResetTrigger() from the OnShopScreenDeactivated event
            EventManager.OnShopScreenDeactivated -= ResetTrigger;

            // Prevents triggering multiple triggers
            Player.S.alreadyTriggered = false;
        }
    }

    //protected virtual void Loop() {
    public void Loop() {
        if (!triggerHasBeenDeactivated) {
            if (!GameManager.S.paused) {
                if (GameManager.S.canInput) {
                    if (!GameManager.S.IsBattling()) {
                        // If there hasn't been any input yet...
                        if (!firstButtonPressed) {
                            // ...Activate on button press
                            if (Input.GetButtonDown("SNES B Button")) {
                                Action();
                                firstButtonPressed = true;
                                InteractableCursor.S.Deactivate();
                            }
                        }

                        // Reset trigger
                        if (canBeReset) {
                            if (DialogueManager.S.dialogueFinished && DialogueManager.S.ndx <= 0) {
                                if (Input.GetButtonDown("SNES B Button")) {
                                    ResetTrigger();
                                }
                            }
                        }
                    }
                }
            }
        }
	}

    public void ResetTrigger() {
        firstButtonPressed = false;

        // Activate Interactable Trigger
        if (activateInteractableCursor) {
            InteractableCursor.S.Activate(v);
        }

        // Reset DialogueTrigger messages
        if (t) {
            if (t.enabled) {
                t.ResetMessages();
            }
        }
    }

    // Function implemented in child class that's called when button's pressed
    protected virtual void Action() {

    }
}