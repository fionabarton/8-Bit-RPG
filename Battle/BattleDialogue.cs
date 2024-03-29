﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogue : MonoBehaviour {
	[Header("Set in Inspector")]
	// Text
	public Text displayMessageTextBottom;
	public Text displayMessageTextTop;

	// Cursors
	public GameObject dialogueCursor;

	// Dialogue
	public bool dialogueFinished = true;
	public int dialogueNdx = 99;
	public List<string> message;

	// Text box position
	public RectTransform rtDialogueCanvas;

	public void Initialize() {
		// Reset Dialogue
		dialogueFinished = true;
		dialogueNdx = 0;
		message.Clear();

		// Deactivate Battle Text
		//displayMessageTextTop.gameObject.transform.parent.gameObject.SetActive(false);
	}

	public void Loop() {
		if (dialogueNdx <= 0) {
			if (Input.GetButtonDown("SNES B Button") || Input.GetButtonDown("SNES Y Button")) {
				dialogueFinished = true;
				dialogueNdx = 0;
			}
		} else if (dialogueNdx > 0) { // For Multiple Lines
			if (Input.GetButtonDown("SNES B Button")) {
				if (message.Count > 0) {
					List<string> tMessage;

					tMessage = message;

					tMessage.RemoveAt(0);

					// Call DisplayText() with one less line of "messages"
					DisplayText(tMessage);
				}
			}
		}
	}

	// Display a SINGLE string
	public void DisplayText(string messageToDisplay, float anchoredYPosition = -400) {
		// Reset Dialogue
		dialogueFinished = true;
		dialogueNdx = 0;

		// Convert message string into a list of strings
		List<string> tMessage = new List<string> { messageToDisplay };
		DisplayText(tMessage, anchoredYPosition);
	}

	// Display a LIST of strings
	public void DisplayText(List<string> text, float anchoredYPosition = -400) {
		StopAllCoroutines();
		StartCoroutine(DisplayTextCo(text, anchoredYPosition));
	}
	IEnumerator DisplayTextCo(List<string> text, float anchoredYPosition) {
		// Deactivate Cursor
		dialogueCursor.SetActive(false);

		// Get amount of Dialogue Strings
		dialogueNdx = text.Count;

		// Position Text Box
		rtDialogueCanvas.anchoredPosition = new Vector2(0, anchoredYPosition);

		if (text.Count > 0) {
			dialogueFinished = false;

			string dialogueSentences = null;

			// Split text argument w/ blank space
			string[] dialogueWords = text[0].Split(' ');
			// Display text one word at a time
			for (int i = 0; i < dialogueWords.Length; i++) {
				// Audio: Dialogue
				AudioManager.S.PlaySFX(eSoundName.dialogue);

				dialogueSentences += dialogueWords[i] + " ";
				displayMessageTextBottom.text = dialogueSentences;
				yield return new WaitForSeconds(OptionsMenu.S.textSpeed);
			}

			// Activate cursor
			dialogueCursor.SetActive(true);

			dialogueNdx -= 1;

			dialogueFinished = true;
		}
	}

	// Set Text Instantly 
	// - No delay/stagger between displaying each word)
	public void SetText(string text, bool upperLeftAlignment = false, bool activateSubMenu = false) {
		StopCoroutine("DisplayTextCo");

		// Set Text Alignment
		//if (upperLeftAlignment) {
		//	message.alignment = TextAnchor.UpperLeft;
		//} else {
		//	message.alignment = TextAnchor.MiddleCenter;
		//}

		displayMessageTextBottom.text = text;

		// Optionally Activate Sub Menu
		//if (activateSubMenu) {
		//	GameManager.S.pauseSubMenu.gameObject.SetActive(true);

		//	// Update Delgate
		//	UpdateManager.fixedUpdateDelegate += GameManager.S.pauseSubMenu.Loop;
		//}
	}
}