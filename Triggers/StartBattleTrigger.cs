using System.Collections.Generic;
using UnityEngine;

public class StartBattleTrigger : ActivateOnButtonPress {
    [Header("Set in Inspector")]
    public int questNdx = -1;

    public int enemyAmount = 1;

    public List<EnemyStats> enemies;

    public string offerMessage = "Hey, wanna fight me?";
    public string noMessage = "No? How rude!";

    DialogueTrigger dialogueTrigger;

    private void Start() {
        // Disable if quest is completed
        if (!QuestManager.S.quests[questNdx].isCompleted) {
            dialogueTrigger = GetComponent<DialogueTrigger>();
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

    void Yes() {
        DialogueManager.S.DeactivateTextBox(false);

        // Start battle
        Player.S.enemyStats = enemies;
        Player.S.enemyAmount = enemyAmount;
        StartCoroutine(Player.S.StartBattle());
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