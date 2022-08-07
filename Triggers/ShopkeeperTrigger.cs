using System.Collections;
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

    public eShopkeeperMode mode = eShopkeeperMode.pickBuyOrSell; // 0: Nothing, 1: Buy, 2: Sell

    [Header("Set Dynamically")]
    public List<Item> inventory = new List<Item>();

    private void Start() {
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
        DialogueManager.S.DisplayText("<color=yellow><Shop Keeper></color> Wanna buy some hot junk? Or maybe you'd rather sell some hot junk instead? Hmmm?");
        GameManager.S.gameSubMenu.SetText("Buy junk!", "Sell junk!", "No thanks.", "", 3);

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
        DialogueManager.S.DisplayText("Fantastic! What would you like to buy? Hmmm?");
        mode = eShopkeeperMode.pickedBuy;
    }

    void Sell() {
        // Audio: Confirm
        AudioManager.S.PlaySFX(eSoundName.confirm);

        DialogueManager.S.ResetSettings();
        DialogueManager.S.DisplayText("How grand! What would you like to sell? Hmmm?");
        mode = eShopkeeperMode.pickedSell;
    }

    void No() {
        // Audio: Deny
        AudioManager.S.PlaySFX(eSoundName.deny);

        DialogueManager.S.ResetSettings();
        DialogueManager.S.DisplayText("Hmmm? Oh, okay, that's cool. Please come again soon!");
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
}