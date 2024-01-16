using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : ActivateOnButtonPress {
	[Header("Set in Inspector")]
	// To add proper Item to Inventory
	public eItem 	item;

	// If != 0, this is a key item. Gives the index of which item is deactivated to KeyItemManager.cs
	public int		keyItemNdx = -1;

	// If != 0, this is a quest item. Gives the index of which quest is completed to QuestManager.cs
	public int		questItemNdx = -1;

	// Deactivated when item is picked up by player 
	public GameObject solidColliderGO;

	[Header("Set Dynamically")]
	private SpriteRenderer	sRend;
	private BoxCollider2D	boxColl;

    private void Start() {
		sRend = GetComponent<SpriteRenderer>();
		boxColl = GetComponent<BoxCollider2D>();
	}

    protected override void Action() {
		// Set Camera to Item gameObject
		CamManager.S.ChangeTarget(gameObject, true);

		// Audio: Win
		StartCoroutine(AudioManager.S.PlaySongThenResumePreviousSong(6));

		// Get and position Poof game object
		GameObject poof = ObjectPool.S.GetPooledObject("Poof");
		ObjectPool.S.PosAndEnableObj(poof, gameObject);

		// Interactable Trigger (without this, occasionally results in console warning)
		InteractableCursor.S.Deactivate();

		// Add Item to Inventory
		Inventory.S.AddItemToInventory(Items.S.items[(int)item]);

		// Display Dialogue 
		DialogueManager.S.DisplayText("Neat, a " + Items.S.items[(int)item].name + "! The party adds it to their inventory!");

		// Deactivate sprite renderer, trigger, & collider
		// (Would prefer deactivating gameobject, but that would kill the audio coroutine called above)
		sRend.enabled = false;
		boxColl.enabled = false;
		solidColliderGO.SetActive(false);

		// Deactivate...PERMANENTLY! (KeyItemManager.cs)
		if (keyItemNdx != -1) {
			KeyItemManager.S.isDeactivated[keyItemNdx] = true;
		}

		// Quest completed (QuestManager.cs)
		if (questItemNdx != -1) {
			QuestManager.S.quests[questItemNdx].isCompleted = true;
		}
	}
}