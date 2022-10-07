using UnityEngine;

public class AddPartyMemberTrigger : ActivateOnButtonPress {
    [Header("Set in Inspector")]
    public int questNdx = -1;
    public int partyMemberNdx;

    public string offerMessage = "Hey, want me to join the party?";
    public string noMessage = "That's cool, man. Feel free to ask me again though.";

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
        DialogueManager.S.DeactivateTextBox();
        DialogueManager.S.ResetSettings();

        QuestManager.S.quests[questNdx].isCompleted = true;

        // Activate KeyboardInput
        KeyboardInputMenu.S.Activate(partyMemberNdx, GameManager.S.currentScene);

        // Audio: Buff 1
        AudioManager.S.PlaySFX(eSoundName.buff1);
    }

    void No() {
        // Audio: Deny
        AudioManager.S.PlaySFX(eSoundName.deny);

        DialogueManager.S.ResetSettings();
        DialogueManager.S.DisplayText(noMessage);
    }
}