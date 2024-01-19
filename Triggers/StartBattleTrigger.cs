using System.Collections.Generic;
using UnityEngine;

public class StartBattleTrigger : ActivateOnButtonPress {
    [Header("Set in Inspector")]
    public int questNdx = -1;

    public int enemyAmount = 1;

    public List<EnemyStats> enemies;

    public string offerMessage = "Hey, wanna fight me?";
    public string yesMessage = "Yes? Well, then come at me, bro!";
    public string noMessage = "No? How rude!";

    bool playerHasAcceptedOffer = false;

    DialogueTrigger dialogueTrigger;
    BoxCollider2D boxColl;

    private void Start() {
        // Disable if quest is completed
        if (!QuestManager.S.quests[questNdx].isCompleted) {
            dialogueTrigger = GetComponent<DialogueTrigger>();
            boxColl = GetComponent<BoxCollider2D>();
        } else {
            base.OnTriggerExit2D(Player.S.playerTriggerGO.GetComponent<BoxCollider2D>());

            if (dialogueTrigger) {
                dialogueTrigger.RecallOnTriggerEnter2D();
            }

            enabled = false;
        }
    }

    protected override void Action() {
        // Set camera to this gameObject
        CamManager.S.ChangeTarget(gameObject, true);

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
    }

    public void Update() {
        // If player said yes to offer, on button press start battle
        if (playerHasAcceptedOffer) {
            if (Input.GetButtonDown("SNES B Button")) {
                if (!GameManager.S.paused) {
                    if (DialogueManager.S.dialogueFinished && DialogueManager.S.ndx <= 0) {
                        // Deactivate dialogue box & trigger
                        DialogueManager.S.DeactivateTextBox(false);
                        boxColl.enabled = false;

                        // Start battle
                        Player.S.enemyStats = enemies;
                        Player.S.enemyAmount = enemyAmount;
                        StartCoroutine(Player.S.StartBattle());
                    }
                }
            }
        }
    }

    void Yes() {
        // Audio: Confirm
        AudioManager.S.PlaySFX(eSoundName.confirm);

        // Display dialogue 
        DialogueManager.S.ResetSettings();
        DialogueManager.S.DisplayText(yesMessage);

        playerHasAcceptedOffer = true;
    }

    void No() {
        // Audio: Deny
        AudioManager.S.PlaySFX(eSoundName.deny);

        // Display dialogue 
        DialogueManager.S.ResetSettings();
        DialogueManager.S.DisplayText(noMessage);
    }

    protected override void OnTriggerEnter2D(Collider2D coll) {
        if (!QuestManager.S.quests[questNdx].isCompleted) {
            if (coll.gameObject.CompareTag("PlayerTrigger")) {
                if (!Player.S.alreadyTriggered) {
                    base.OnTriggerEnter2D(coll);
                }
            }
        }
    }

    protected override void OnTriggerExit2D(Collider2D coll) {
        if (enabled) {
            if (coll.gameObject.CompareTag("PlayerTrigger")) {
                base.OnTriggerExit2D(coll);
            }
        }
    }
}