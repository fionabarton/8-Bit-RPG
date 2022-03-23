using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : ActivateOnButtonPress {
	[Header("Set in Inspector")]
	// Lists of dialogue to be displayed OnButtonPress:
	// - Which list is displayed depends on which quests have been completed
	public List<string> 	messages0;
	public List<string> 	messages1;
	public List<string>     messages2;
	public List<string>     messages3;

    // Store the indexes of quests that when completed affect which dialogue to display 
    public List<int>		questNdx;

	[Header ("Set Dynamically")]
	// Store unaltered versions of the lists of dialogue
	public List<string>		messagesClone0;
	public List<string>		messagesClone1;
	public List<string>		messagesClone2;
	public List<string>		messagesClone3;

    void OnEnable() {
		// Store unaltered versions of the lists of dialogue
		messagesClone0 = new List<string>(messages0);
		messagesClone1 = new List<string>(messages1);
		messagesClone2 = new List<string>(messages2);
		messagesClone3 = new List<string>(messages3);
	}

	// Reset the sets of dialogue to their original values
	public void ResetMessages() {
		messages0 = new List<string>(messagesClone0);
		messages1 = new List<string>(messagesClone1);
		messages2 = new List<string>(messagesClone2);
		messages3 = new List<string>(messagesClone3);
	}

	// Return the highest index in questNdx
	int highestQuestCompleted() {
		int tNdx = 0;

		for (int i = 0; i < questNdx.Count; i++) {
			if (QuestManager.S.completed[questNdx[i]]) {
				if (questNdx[i] > tNdx) {
					tNdx = i;
				}
			}
		}

		return tNdx;
	}

	// If Player has entered trigger, called OnButtonPress
	protected override void Action() {
		// Display dialogue associated with the highest quest completed
		switch (highestQuestCompleted()) {
			case 0:
				DialogueManager.S.DisplayText(messages0);
				break;
			case 1:
				DialogueManager.S.DisplayText(messages1);
				break;
			case 2:
				DialogueManager.S.DisplayText(messages2);
				break;
			case 3:
				DialogueManager.S.DisplayText(messages3);
				break;
		}

		// Deactivate Interactable Trigger
		InteractableCursor.S.Deactivate();

		// If this gameObject is an NPC, face direction of Player
		NPCMovement npc = GetComponent<NPCMovement>();
        if (npc) {
			npc.StopAndFacePlayer();
        }
    }

	public void ThisLoop () {
		// Remove ThisLoop() from UpdateManager delegate on scene change.
		// This prevents an occasional bug when the Player is within this trigger on scene change.
		// Would prefer a better solution... 
		if (!RPG.S.canInput) {
            UpdateManager.updateDelegate -= ThisLoop;
        }

        if (Input.GetButtonDown("SNES A Button")) {
			if (firstButtonPressed) {
				if (!RPG.S.paused) {
					// If the list of dialogue has multiple elements/lines...
					if (DialogueManager.S.dialogueFinished && DialogueManager.S.ndx > 0) {
						// Reset DialogueManager's text and cursor
						DialogueManager.S.ClearForNextLine();

						// Remove the list of dialogue's first element/line
						List<string> tMessage = new List<string>();
						switch (highestQuestCompleted()) {
							case 0:
								tMessage = messages0;
								break;
							case 1:
								tMessage = messages1;
								break;
							case 2:
								tMessage = messages2;
								break;
							case 3:
								tMessage = messages3;
								break;
						}
						tMessage.RemoveAt(0);

						// Display the list of dialogue with one less element/line
						DialogueManager.S.DisplayText(tMessage);
					}
                }
			}
		}
	}

	protected override void OnTriggerEnter2D(Collider2D coll) {
        if (enabled) {
			if (coll.gameObject.CompareTag("PlayerTrigger")) {
				if (!Blob.S.alreadyTriggered) {
					base.OnTriggerEnter2D(coll);

					// Add ThisLoop() to Update Delgate
					UpdateManager.updateDelegate += ThisLoop;
				}
			}
		}
	}

	protected override void OnTriggerExit2D(Collider2D coll) {
		if (enabled) {
			if (coll.gameObject.CompareTag("PlayerTrigger")) {
				base.OnTriggerExit2D(coll);

				// Reset the sets of dialogue to their original values
				ResetMessages();

				// Remove ThisLoop() from Update Delgate
				UpdateManager.updateDelegate -= ThisLoop;
			}
		}
	}
}
