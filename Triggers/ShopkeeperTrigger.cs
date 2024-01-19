using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If there is a child gameObject that also has a collider (ex. Solid Collider):
/// Make sure that the collider has a RigidBody set to Kinematic to prevent it from
/// triggering something unintentionally
/// </summary>
public class ShopkeeperTrigger : ActivateOnButtonPress {
    [Header("Set in Inspector")]
    // Shop Inventory
    public List<eItem> itemsToPopulateInventory;

    public string offerMessage = "<color=yellow><Shop Keeper></color> Wanna buy some hot junk? Or maybe you'd rather sell some hot junk instead? Hmmm?";
    public string buyMessage = "Fantastic! What would you like to buy? Hmmm?";
    public string sellMessage = "How grand! What would you like to sell? Hmmm?";
    public string noMessage = "Hmmm? Oh, okay, that's cool. Please come again soon!";

    // Sets which direction the NPC faces on start
    // 0 = right, 1 = up, 2 = left, 3 = down
    public int walkDirection;

    [Header("Set Dynamically")]
    private Animator anim;

    public List<Item> inventory = new List<Item>();

    public eShopkeeperMode mode = eShopkeeperMode.pickBuyOrSell; // 0: Nothing, 1: Buy, 2: Sell

    // Flip
    private bool facingRight;

    private void Start() {
        anim = GetComponent<Animator>();

        // Set animation based on walk direction
        SetWalkDirectionAnimation();

        // Convert eItem enumeration into Item
        inventory.Clear();
        for (int i = 0; i < itemsToPopulateInventory.Count; i++) {
            inventory.Add(Items.S.GetItem(itemsToPopulateInventory[i]));
        }
    }

    protected override void Action() {
        mode = eShopkeeperMode.pickBuyOrSell;

        // Set Camera to Shopkeeper gameObject
        CamManager.S.ChangeTarget(gameObject, true);

        // Set Text
        DialogueManager.S.DisplayText(offerMessage);
        GameManager.S.gameSubMenu.SetText("Buy junk!", "Sell junk!", "No thanks.", "", 3);

        // Face towards player
        FacePlayer();

        // Activate Sub Menu after Dialogue 
        DialogueManager.S.activateSubMenu = true;
        // Don't activate Text Box Cursor 
        DialogueManager.S.dontActivateCursor = true;
        // Gray Out Text Box after Dialogue 
        //DialogueManager.S.grayOutTextBox = true;

        // Set OnClick Methods
        Utilities.S.RemoveListeners(GameManager.S.gameSubMenu.buttonCS);
        GameManager.S.gameSubMenu.buttonCS[0].onClick.AddListener(Buy);
        GameManager.S.gameSubMenu.buttonCS[1].onClick.AddListener(Sell);
        GameManager.S.gameSubMenu.buttonCS[2].onClick.AddListener(No);

        // Set button navigation
        Utilities.S.SetButtonNavigation(GameManager.S.gameSubMenu.buttonCS[0], GameManager.S.gameSubMenu.buttonCS[2], GameManager.S.gameSubMenu.buttonCS[1]);
        Utilities.S.SetButtonNavigation(GameManager.S.gameSubMenu.buttonCS[1], GameManager.S.gameSubMenu.buttonCS[0], GameManager.S.gameSubMenu.buttonCS[2]);
        Utilities.S.SetButtonNavigation(GameManager.S.gameSubMenu.buttonCS[2], GameManager.S.gameSubMenu.buttonCS[1], GameManager.S.gameSubMenu.buttonCS[0]);
    }

    void Buy() {
        // Audio: Confirm
        AudioManager.S.PlaySFX(eSoundName.confirm);

        DialogueManager.S.ResetSettings();
        DialogueManager.S.DisplayText(buyMessage);
        mode = eShopkeeperMode.pickedBuy;
    }

    void Sell() {
        // Audio: Confirm
        AudioManager.S.PlaySFX(eSoundName.confirm);

        DialogueManager.S.ResetSettings();
        DialogueManager.S.DisplayText(sellMessage);
        mode = eShopkeeperMode.pickedSell;
    }

    void No() {
        // Audio: Deny
        AudioManager.S.PlaySFX(eSoundName.deny);

        DialogueManager.S.ResetSettings();
        DialogueManager.S.DisplayText(noMessage);
    }

    public void ThisLoop() {
        // Remove ThisLoop() from UpdateManager delegate on scene change.
        // This prevents an occasional bug when the Player is within this trigger on scene change.
        // Would prefer a better solution... 
        if (!GameManager.S.canInput) {
            UpdateManager.updateDelegate -= ThisLoop;
        }

        if (Input.GetButtonDown("SNES A Button")) {
            // Activate Shop Screen
            if (DialogueManager.S.dialogueFinished) {
                if (mode != eShopkeeperMode.pickBuyOrSell) {
                    if (mode == eShopkeeperMode.pickedBuy) {
                        // Import shopkeeper inventory
                        ShopMenu.S.ImportInventory(inventory);
                        ShopMenu.S.buyOrSellMode = true;
                    } else if (mode == eShopkeeperMode.pickedSell) {
                        // Import party inventory
                        ShopMenu.S.ImportInventory(Inventory.S.GetItemList());
                        ShopMenu.S.buyOrSellMode = false;
                    }

                    // Activate Shop Screen
                    ShopMenu.S.Activate();

                    DialogueManager.S.DeactivateTextBox(false);

                    // Subscribe ResetTrigger() to the OnShopScreenDeactivated event
                    EventManager.OnShopScreenDeactivated += ResetTrigger;

                    // Reset ability to input
                    mode = eShopkeeperMode.pickBuyOrSell;
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

            // Unsubscribe ResetTrigger() from the OnShopScreenDeactivated event
            EventManager.OnShopScreenDeactivated -= ResetTrigger;
        }
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