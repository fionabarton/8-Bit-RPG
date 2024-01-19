using UnityEngine;

public class AddPartyMemberTrigger : ActivateOnButtonPress {
    [Header("Set in Inspector")]
    public int      questNdx = -1;
    public int      partyMemberNdx;

    public string   offerMessage = "Hey, want me to join the party?";
    public string   yesMessage = "Good choice. You won't regret it!";
    public string   noMessage = "That's cool, man. Feel free to ask me again though.";

    // Sets which direction the NPC faces on start
    // 0 = right, 1 = up, 2 = left, 3 = down
    public int      walkDirection;

    [Header("Set Dynamically")]
    private Animator anim;

    // Flip
    private bool    facingRight;

    bool            playerHasAcceptedOffer = false;
    BoxCollider2D   boxColl;

    void Start() {
        anim = GetComponent<Animator>();
        boxColl = GetComponent<BoxCollider2D>();

        // Set animation based on walk direction
        SetWalkDirectionAnimation();
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

        // Face towards player
        if (anim != null) {
            FacePlayer();
        }

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
                        playerHasAcceptedOffer = false;
                        
                        // Deactivate dialogue box & trigger
                        DialogueManager.S.DeactivateTextBox(false);
                        boxColl.enabled = false;

                        QuestManager.S.quests[questNdx].isCompleted = true;

                        // Activate KeyboardInput
                        KeyboardInputMenu.S.Activate(partyMemberNdx, GameManager.S.currentScene);

                        // Audio: Buff 1
                        AudioManager.S.PlaySFX(eSoundName.buff1);
                    }
                }
            }
        }
    }

    void Yes() {
        // Audio: Confirm
        AudioManager.S.PlaySFX(eSoundName.confirm);

        // Display dialogue 
        DialogueManager.S.DisplayText(yesMessage);

        playerHasAcceptedOffer = true;
        GameManager.S.isNamingNewPartyMember = true;
    }

    void No() {
        // Audio: Deny
        AudioManager.S.PlaySFX(eSoundName.deny);

        DialogueManager.S.ResetSettings();
        DialogueManager.S.DisplayText(noMessage);
    }

    private void SetWalkDirectionAnimation() {
        // Set animation
        switch (walkDirection) {
            case 1: anim.CrossFade("Walk_Up", 0); break;
            case 3: anim.CrossFade("Walk_Down", 0); break;
            case 0:
                anim.CrossFade("Walk_Side", 0);
                // Flip
                if (facingRight) { Utilities.S.Flip(gameObject, ref facingRight); }
                break;
            case 2:
                anim.CrossFade("Walk_Side", 0);
                // Flip
                if (!facingRight) { Utilities.S.Flip(gameObject, ref facingRight); }
                break;
        }
    }

    // Face direction of Player
    public void FacePlayer() {
        if (Player.S.gameObject.transform.position.x < transform.position.x &&
            !Utilities.S.isCloserHorizontally(gameObject, Player.S.gameObject)) { // Left
                                                                                  // If facing right, flip
            if (transform.localScale.x > 0) { Utilities.S.Flip(gameObject, ref facingRight); }
            anim.Play("Walk_Side", 0, 1);
        } else if (Player.S.gameObject.transform.position.x > transform.position.x &&
            !Utilities.S.isCloserHorizontally(gameObject, Player.S.gameObject)) { // Right
                                                                                  // If facing left, flip
            if (transform.localScale.x < 0) { Utilities.S.Flip(gameObject, ref facingRight); }
            anim.Play("Walk_Side", 0, 1);
        } else if (Player.S.gameObject.transform.position.y < transform.position.y &&
            Utilities.S.isCloserHorizontally(gameObject, Player.S.gameObject)) { // Down
            anim.Play("Walk_Down", 0, 1);
        } else if (Player.S.gameObject.transform.position.y > transform.position.y &&
            Utilities.S.isCloserHorizontally(gameObject, Player.S.gameObject)) { // Up
            anim.Play("Walk_Up", 0, 1);
        }
    }
}