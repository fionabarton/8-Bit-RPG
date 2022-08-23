using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMessage : MonoBehaviour {
	[Header("Set in Inspector")]
	public Text message; // named "Text" in the Hierarchy/Inspector
	public GameObject cursorGO;

	// Text box position
	public RectTransform rtDialogueCanvas;

	[Header("Set Dynamically")]
	public bool dialogueFinished;

	private static PauseMessage _S;
	public static PauseMessage S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;
	}

	public void DisplayText(string text, bool upperLeftAlignment = false, bool activateSubMenu = false, float anchoredYPosition = -424) {
		gameObject.SetActive(true);

		StopAllCoroutines();

		StopCoroutine("DisplayTextCo");
		StartCoroutine(DisplayTextCo(text, upperLeftAlignment, activateSubMenu, anchoredYPosition));
	}
	IEnumerator DisplayTextCo(string text, bool upperLeftAlignment, bool activateSubMenu = false, float anchoredYPosition = -424) {
		// Deactivate Cursor
		cursorGO.SetActive(false);

		// Reset Text Strings
		string dialogueSentences = null;

		// Set Text Alignment
		if (upperLeftAlignment) {
			message.alignment = TextAnchor.UpperLeft;
		} else {
			message.alignment = TextAnchor.MiddleCenter;
		}

		// Position Text Box
		rtDialogueCanvas.anchoredPosition = new Vector2(0, anchoredYPosition);

		dialogueFinished = false;

		// Split text argument w/ blank space
		string[] dialogueWords = text.Split(' ');
		// Display text one word at a time
		for (int i = 0; i < dialogueWords.Length; i++) {
			dialogueSentences += dialogueWords[i] + " ";
			message.text = dialogueSentences;
            yield return new WaitForSeconds(OptionsMenu.S.textSpeed);
		}
		// Activate cursor
		cursorGO.SetActive(true);

		// Optionally Activate Sub Menu
		if (activateSubMenu) {
			GameManager.S.gameSubMenu.gameObject.SetActive(true);

			// Update Delgate
			UpdateManager.fixedUpdateDelegate += GameManager.S.gameSubMenu.Loop;
		}

		// Dialogue Finished
		dialogueFinished = true;
	}

	// Set Text Instantly 
	// - No delay/stagger between displaying each word)
	public void SetText(string text, bool upperLeftAlignment = false, bool activateSubMenu = false, float anchoredYPosition = -424) {
		StopCoroutine("DisplayTextCo");

		// Set Text Alignment
		if (upperLeftAlignment) {
			message.alignment = TextAnchor.UpperLeft;
		} else {
			message.alignment = TextAnchor.MiddleCenter;
		}

		// Position Text Box
		rtDialogueCanvas.anchoredPosition = new Vector2(0, anchoredYPosition);

		message.text = text;

		// Optionally Activate Sub Menu
		if (activateSubMenu) {
			GameManager.S.gameSubMenu.gameObject.SetActive(true);

			// Update Delgate
			UpdateManager.fixedUpdateDelegate += GameManager.S.gameSubMenu.Loop;
		}
	}
}