using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseTrigger : ActivateOnButtonPress {
    [Header("Set in Inspector")]
    public eItem item;

    protected override void Action() {
        // Set Camera to Item gameObject
        CamManager.S.ChangeTarget(gameObject, true);

        DialogueManager.S.DisplayText("I'm a " + Items.S.items[(int)item].name +
                                         "! Wanna purchase me for " + Items.S.items[(int)item].value +
                                         " gold?");

        // Set SubMenu Text
        GameManager.S.gameSubMenu.SetText("Yes", "No");

        // Activate Sub Menu after Dialogue 
        DialogueManager.S.activateSubMenu = true;
        // Don't activate Text Box Cursor 
        DialogueManager.S.dontActivateCursor = true;
        // Gray Out Text Box after Dialogue 
        //DialogueManager.S.grayOutTextBox = true;

        // Set OnClick Methods
        Utilities.S.RemoveListeners(GameManager.S.gameSubMenu.buttonCS);
        GameManager.S.gameSubMenu.buttonCS[0].onClick.AddListener(Yes);
        GameManager.S.gameSubMenu.buttonCS[1].onClick.AddListener(No);

        // Set button navigation
        Utilities.S.SetButtonNavigation(GameManager.S.gameSubMenu.buttonCS[0], GameManager.S.gameSubMenu.buttonCS[1], GameManager.S.gameSubMenu.buttonCS[1]);
        Utilities.S.SetButtonNavigation(GameManager.S.gameSubMenu.buttonCS[1], GameManager.S.gameSubMenu.buttonCS[0], GameManager.S.gameSubMenu.buttonCS[0]);
    }

    void Yes() {
        DialogueManager.S.ResetSettings();

        Item tItem = Items.S.items[(int)item];

        if (Party.S.gold >= tItem.value) {
            // Added to Player Inventory
            Inventory.S.AddItemToInventory(tItem);

            DialogueManager.S.DisplayText("Yahoo! Thank you for purchasing me!");

            // Subtract item price from Player's Gold
            Party.S.gold -= tItem.value;

            // Audio: Buff 1
            AudioManager.S.PlaySFX(eSoundName.buff1);
        } else {
            DialogueManager.S.DisplayText("You ain't got enough money, jerk!");

            // Audio: Deny
            AudioManager.S.PlaySFX(eSoundName.deny);
        }
    }

    void No() {
        // Audio: Deny
        AudioManager.S.PlaySFX(eSoundName.deny);

        DialogueManager.S.ResetSettings();
        DialogueManager.S.DisplayText("That's cool. Later, bro.");
    }
}
