using UnityEngine;

public class CompleteQuestTrigger : ActivateOnButtonPress {
    [Header("Set in Inspector")]
    public int questNdx = -1;

    public string offerMessage = "Hey, would you like to accept this quest?";
    public string yesMessage = "Yahoo! Thank you accepting this quest!";
    public string noMessage = "Okay, thanks anyway. Let me know if you change your mind.";

    DialogueTrigger dialogueTrigger;

    public bool isActivated;

    private void Start() {
        if (!QuestManager.S.quests[questNdx].isCompleted) {
            dialogueTrigger = GetComponent<DialogueTrigger>();
        } else {
            enabled = false;
        }
    }

    protected override void Action() {
        // Set camera to this gameObject
        CamManager.S.ChangeTarget(gameObject, true);

        if (!QuestManager.S.quests[questNdx].isCompleted) {
            DialogueManager.S.DisplayText(offerMessage);

            // Set SubMenu Text
            GameManager.S.gameSubMenu.SetText("Yes", "No");

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
        } else {
            DialogueManager.S.DisplayText("FAILURE");
        }
    }

    public void ThisLoop() {
        if (isActivated) {
            if (Input.GetButtonDown("SNES B Button")) {
                if (DialogueManager.S.dialogueFinished && DialogueManager.S.ndx <= 0) {
                    base.OnTriggerExit2D(Player.S.playerTriggerGO.GetComponent<BoxCollider2D>());

                    dialogueTrigger.RecallOnTriggerEnter2D();

                    enabled = false;

                    // Remove ThisLoop() from Update Delgate
                    UpdateManager.updateDelegate -= ThisLoop;
                }
            }
        }
    }

    void Yes() {
        DialogueManager.S.ResetSettings();

        DialogueManager.S.DisplayText(yesMessage);

        QuestManager.S.quests[questNdx].isCompleted = true;

        // Audio: Buff 1
        AudioManager.S.PlaySFX(eSoundName.buff1);

        isActivated = true;
    }

    void No() {
        // Audio: Deny
        AudioManager.S.PlaySFX(eSoundName.deny);

        DialogueManager.S.ResetSettings();
        DialogueManager.S.DisplayText(noMessage);
    }

    protected override void OnTriggerEnter2D(Collider2D coll) {
        if (!QuestManager.S.quests[questNdx].isCompleted) {
            if (coll.gameObject.CompareTag("PlayerTrigger")) {
                if (!Player.S.alreadyTriggered) {
                    base.OnTriggerEnter2D(coll);

                    // Add ThisLoop() to Update Delgate
                    UpdateManager.updateDelegate += ThisLoop;
                }
            }
        }
    }

    protected override void OnTriggerExit2D(Collider2D coll) {
        if (enabled) {
            if (coll.gameObject.CompareTag("PlayerTrigger")) {
                base.OnTriggerExit2D(coll);

                // Remove ThisLoop() from Update Delgate
                UpdateManager.updateDelegate -= ThisLoop;
            }
       }
    }
}