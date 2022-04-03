using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trigger that opens or closes a chest OnButtonPress
/// </summary>
public class ChestTrigger : ActivateOnButtonPress {
	[Header ("Set in Inspector")]
	// Item within this chest
	public eItem			item;

	// Used by ChestManager.cs to track whether or not this chest has been opened
	public int				ndx;

	// Sprites
	public Sprite			openChest, closedChest;
	public SpriteRenderer	sRend;

	[Header("Set Dynamically")]
	public bool				chestIsOpen;

	protected override void Action() {
		if (!chestIsOpen) {
			OpenChest();

			// Audio: Win
			StartCoroutine(AudioManager.S.PlaySongThenResumePreviousSong(6));
		} else {
			// Display Dialogue
			DialogueManager.S.DisplayText("You've already looted this chest. It's empty, you greedy pig.");

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}
	}

	void OpenChest(){
		// Change Sprite
		sRend.sprite = openChest;

		// Add Item to Inventory
		Item tItem = Items.S.GetItem(item);
		Inventory.S.AddItemToInventory(tItem);

		// Display Dialogue 
		DialogueManager.S.DisplayText ("Wow, a " + tItem.name + "! The party adds it to their inventory!");

		chestIsOpen = true;

		// Inform ChestManager.cs that this chest has been opened 
		ChestManager.S.isOpen[ndx] = true;
	}
}