using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestTrigger : ActivateOnButtonPress {
	[Header ("Set in Inspector")]
	public eItem			item;

	// Used by ChestManager.cs to track whether or not this chest has been opened
	public int				ndx;

	public Sprite			openChest, closedChest;
	public SpriteRenderer	sRend;

	[Header("Set Dynamically")]
	public bool				chestIsOpen;

	protected override void Action() {
		if (!chestIsOpen) {
			OpenChest();
		} else {
			// Display Dialogue
			DialogueManager.S.DisplayText("You've already looted this chest. It's empty, you greedy pig.");
		}
	}

	void OpenChest(){
		// Change Sprite
		sRend.sprite = openChest;

		// Add Item to Inventory
		Item tItem = ItemManager.S.GetItem (item);
		Inventory.S.AddItemToInventory(tItem);

		// Display Dialogue 
		DialogueManager.S.DisplayText ("Wow, a " + tItem.name + "! The party adds it to their inventory!");

		chestIsOpen = true;

		// Inform ChestManager.cs that this chest has been opened 
		ChestManager.S.isOpen[ndx] = true;
	}
}
