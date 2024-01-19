using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : ActivateOnButtonPress {
	[Header("Set in Inspector")]
	public eDoorMode 		doorMode;

	public string			doorIsLockedMessage = "This door is locked. Find a key, jerk!";
	public string			doorIsUnlockedMessage = "Great! You unlocked the stupid door!";

	// Gives the index of which door has already been unlocked to DoorManager.cs
	public int				ndx; // For DoorManager

	public Sprite 			lockedDoorSprite, closedDoorSprite, openDoorSprite;

	public BoxCollider2D	solidColl;
	public BoxCollider2D	triggerColl;

	[Header("Set Dynamically")]
	public SpriteRenderer	sRend;

	void Start () {
		sRend = GetComponent<SpriteRenderer> ();
	}

	protected override void Action() {
		// Set Camera to Door gameObject
		CamManager.S.ChangeTarget(gameObject, true);

		switch (doorMode) {
		case eDoorMode.locked:
			// If Player has Key, unlock the door
			if (Inventory.S.GetItemCount(Items.S.GetItem(eItem.smallKey)) > 0) {
				UnlockDoor();

				// Audio: Win
				StartCoroutine(AudioManager.S.PlaySongThenResumePreviousSong(6));
			} else {
				// Display Text
				DialogueManager.S.DisplayText(doorIsLockedMessage);
			}
			break;
		case eDoorMode.closed:
			OpenDoor();
			break;
		}
	}

	void UnlockDoor(){
		// Switch eDoorMode
		doorMode = eDoorMode.open;

		// Change Sprite
		sRend.sprite = openDoorSprite;

		// Get and position Poof game object
		GameObject poof = ObjectPool.S.GetPooledObject("Poof");
		ObjectPool.S.PosAndEnableObj(poof, gameObject);

		// Disable colliders
		solidColl.enabled = false;
		triggerColl.enabled = false;

		// Deactivate trigger
		triggerHasBeenDeactivated = true;

		// Remove Item from Inventory
		Inventory.S.RemoveItemFromInventory (Items.S.GetItem(eItem.smallKey));

		// Display Text
		DialogueManager.S.DisplayText (doorIsUnlockedMessage);

		// Door Manager
		DoorManager.S.isUnlocked[ndx] = true;
	}

	void OpenDoor(){
		// Switch eDoorMode
		doorMode = eDoorMode.open;
		
		// Change sprite
		sRend.sprite = openDoorSprite;

		// Get and position Poof game object
		GameObject poof = ObjectPool.S.GetPooledObject("Poof");
		ObjectPool.S.PosAndEnableObj(poof, gameObject);

		// Disable colliders
		solidColl.enabled = false;
		triggerColl.enabled = false;

		// Deactivate trigger
		triggerHasBeenDeactivated = true;

		// Set Camera to Player gameObject
		CamManager.S.ChangeTarget(Player.S.gameObject, true);

		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.damage2);
	}
}