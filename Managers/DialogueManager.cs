using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
	[Header("Set in Inspector")]
	public GameObject	TextBoxSpriteGO;
	public GameObject 	cursorGO;
	public RectTransform	rtDialogueCanvas;

	[Header("Set Dynamically")]
	private Text dialogueTextCS;
	private GameObject dialogueTextGO;

	private string[] dialogueWords;
	private string dialogueSentences;

	public bool dialogueFinished = false;

	public int ndx;

	public bool activateSubMenu;
	public bool dontActivateCursor;
	public bool grayOutTextBox;

	private static DialogueManager _S;
	public static DialogueManager S { get { return _S; } set { _S = value; } }

	// Indexes of lines of dialogue that have center/middle alignment.
	// If it's empty, alignment defaults to top/left.
	public List<int>	linesWithMiddleAlignment = new List<int>();

	void Awake() {
		S = this;
	}

	void Start () {
		dialogueTextCS = GetComponentInChildren<Text> ();
		dialogueTextGO = dialogueTextCS.gameObject;

		DeactivateTextBox();
	}

	public void ThisLoop() {
		if (Input.GetButtonDown("SNES B Button")) {
			if (!GameManager.S.paused) {
				// Deactivate Text Box (On Button Press)
				if (dialogueFinished && ndx <= 0) {
					Invoke("EndDialogue", 0.1f);
				}
			}
		}
	}

	void EndDialogue() {
		// Audio: High Beep 2
		AudioManager.S.PlaySFX(eSoundName.highBeep2);

		// Deactivate text box
		if(GameManager.S.currentScene != "Title_Screen") {
			DeactivateTextBox();
        } else {
			DeactivateTextBox(false);
		}

		// Set Camera to Player gameObject
		CamManager.S.ChangeTarget(Player.S.gameObject, true);
	}

	// Display a SINGLE string
	public void DisplayText(string messageToDisplay, bool moveDown = false) {
		DeactivateTextBox();
		List<string> tMessage = new List<string> { messageToDisplay };
		DisplayText(tMessage, moveDown);
	}

	// Display a LIST of strings
	public void DisplayText(List<string> text, bool moveDown = false) {
		StartCoroutine (DisplayTextCo (text, moveDown));

		// Deactivate Overworld Player Stats
		//ScreenManager.S.playerButtonsGO.SetActive (false);
	}
	IEnumerator DisplayTextCo(List<string> text, bool moveDown = false) {
		UpdateManager.updateDelegate += ThisLoop;

		// Get amount of Dialogue Strings
		ndx = text.Count;

		dialogueFinished = false;

		// Activate Text Box
		dialogueTextGO.SetActive (true);
		TextBoxSpriteGO.SetActive(true);

		// Position Text Box
		if (moveDown) {
			rtDialogueCanvas.anchoredPosition = new Vector2 (0, -325);
		} else {
			rtDialogueCanvas.anchoredPosition = new Vector2 (0, 325);
		}

		// Set Text Alignment
		if (linesWithMiddleAlignment.Count <= 0) {
			dialogueTextCS.alignment = TextAnchor.UpperLeft;
		} else {
			for(int i = 0; i < linesWithMiddleAlignment.Count; i++) {
				if (ndx == linesWithMiddleAlignment[i]) {
					dialogueTextCS.alignment = TextAnchor.MiddleCenter;
					break;
				} else {
					dialogueTextCS.alignment = TextAnchor.UpperLeft;
				}
			}
		}

		// Freeze Player
		Player.S.canMove = false;

		// Split text argument w/ blank space
		dialogueWords = text[0].Split (' ');
        // Display text one word at a time
        for (int i = 0; i < dialogueWords.Length; i++) {
			// Audio: Dialogue
			AudioManager.S.sfxCS[0].Play();

			dialogueSentences += dialogueWords [i] + " ";
			dialogueTextCS.text = dialogueSentences;
			yield return new WaitForSeconds(OptionsMenu.S.textSpeed);
		}

		// Optionally Activate cursor
		if (!dontActivateCursor) {
			cursorGO.SetActive (true);
		} 

		// Optionally Activate Sub Menu
		if (activateSubMenu) {
			GameManager.S.gameSubMenu.gameObject.SetActive(true);

			// Update Delgate
			UpdateManager.fixedUpdateDelegate += GameManager.S.gameSubMenu.Loop;
		}

		// Gray Out Text Box
		if (grayOutTextBox) {
			GrayOutTextBox (true);
		}

		ndx -= 1;

		dialogueFinished = true;
	}

	public void ClearForNextLine() {
		StopAllCoroutines ();

		// Deactivate Cursor
		cursorGO.SetActive (false);

		// Reset Dialogue
		dialogueSentences = null;
    }

	public void DeactivateTextBox(bool canMove = true){
		StopAllCoroutines (); // if Cursor !active, prevents it from being activated at the end of DisplayTextCo()

		ndx = 0;
		linesWithMiddleAlignment.Clear();

		// Deactivate Text Box & Cursor
		dialogueTextGO.SetActive (false);
		TextBoxSpriteGO.SetActive(false);
		cursorGO.SetActive (false);
		
		// Reset Dialogue
		dialogueSentences = null;
		dialogueFinished = false;

		// Reset Text Box Color
		GrayOutTextBox (false);

		// Reset sub menu
		ResetSettings();

		// Unfreeze Player
		Player.S.canMove = canMove;

		// Overworld Player Stats
		//Blob.S.playerUITimer = Time.time + 1.5f;

		UpdateManager.updateDelegate -= ThisLoop;
	}

	public void GrayOutTextBox(bool grayOut){
		Color c;
		
		if (grayOut) {
			c = new Color (.5f, .5f, .5f, .94f);
		} else {
			c = new Color (1, 1, 1, .94f);
		}

		// Set Text Color
		dialogueTextCS.color = c;
	}

	public void ResetSettings() {
		// Gray Out Text Box after Dialogue 
		grayOutTextBox = false;

		// Activate Text Box Cursor
		dontActivateCursor = false;
		// Don't activate Sub Menu after Dialogue 
		activateSubMenu = false;

		// Reset sub menu
		GameManager.S.gameSubMenu.ResetSettings();
	}
}
