using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// Handles creating character names via user keyboard input
public class KeyboardInputMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	// List of buttons for each letter, number, symbol, etc. input
	public List<GameObject> buttonsGO;

	// Each slots represents one char 
	public List<Text> charSlotsText;

	[Header("Set Dynamically")]
	private string inputString = "";

	// Uppercase: 0-25, Lowercase: 26-51, Numbers: 52-61, Symbols: 62-65, 66-69, 70-89
	private string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789()[]!?.,~@#$%^&*+-=_\"'` :;/\\";

	// Variables related to predetermined default names
	private int dontCareNdx;
	private List<string> dontCareNames = new List<string>() { "Butthead", "Mildew", "Gunt", "Love Gum", "Moon Unit" };

	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

	// Ensures audio is only played once when button is selected
	public GameObject previousSelectedGameObject;

	private Animator inputBoxAnim;

	private bool subMenuIsActive;

	private static KeyboardInputMenu _S;
	public static KeyboardInputMenu S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;

		inputBoxAnim = GetComponentInChildren<Animator>();
	}

	void Start() {
		gameObject.SetActive(false);
	}

	public void Activate() {
		// Reset text
		inputString = "";
		for(int i = 0; i < charSlotsText.Count; i++) {
			charSlotsText[i].text = "";
		}

		canUpdate = true;

		// Freeze player
		GameManager.S.paused = true;
		Player.S.canMove = false;

		// Activate Cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);
		ScreenCursor.S.cursorGO[1].SetActive(true);
		ScreenCursor.S.ResetAnimClip();

		// Set Selected Gameobject (Keyboard Input Menu: A Button)
		Utilities.S.SetSelectedGO(buttonsGO[0]);
		
		// Initialize previously selected GameObject
		previousSelectedGameObject = buttonsGO[0];

		// Set active char cursor position
		Utilities.S.PositionCursor(charSlotsText[inputString.Length].gameObject, 0, 10, 3, 1);

		// Activate PauseMessage
		PauseMessage.S.DisplayText("Yo.\nSo what's the name, ya little goon?");

		// Update Delgate
		UpdateManager.updateDelegate += Loop;

		gameObject.SetActive(true);
	}

	public void Deactivate() {
		// Unfreeze player
		GameManager.S.paused = false;
		Player.S.canMove = true;

		// Deactivate PauseMessage
		PauseMessage.S.gameObject.SetActive(false);

		// Deactivate screen cursors
		Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

		// Update Delegate
		UpdateManager.updateDelegate -= Loop;

		gameObject.SetActive(false);
	}

	public void Loop() {
		// Reset canUpdate
		if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f) {
			canUpdate = true;
		}

		// Set cursor position and highlight selected button
		if (canUpdate) {
			for (int i = 0; i < buttonsGO.Count; i++) {
				if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == buttonsGO[i]) {
					// Set Cursor Position to Selected Button
					Utilities.S.PositionCursor(buttonsGO[i], -30, -10, 0);

					// Set selected button text color	
					buttonsGO[i].gameObject.GetComponentInChildren<Text>().color = new Color32(205, 208, 0, 255);

					// Audio: Selection (when a new gameObject is selected)
					Utilities.S.PlayButtonSelectedSFX(ref previousSelectedGameObject);
				} else {
					// Set unselected button text color	
					buttonsGO[i].gameObject.GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
				}
			}
			canUpdate = false;
		}

        if (!subMenuIsActive) {
			// Backspace input
			if (Input.GetButtonDown("SNES Y Button") || Input.GetKeyDown(KeyCode.Backspace)) {
				Backspace();
			}

			// Space input
			if (Input.GetKeyDown(KeyCode.Space)) {
				PressedKey(85);
			}
		}
	}

	// Add a char to the displayed name
	public void PressedKey(int ndx) {
		if (inputString.Length < 15) {
			for (int i = 0; i < characters.Length; i++) {
				if (i == ndx) {
					// Add user keyboard input to string
					inputString += characters[ndx];
					DisplayText(inputString + GetRemainingWhitespace());

					if (inputString.Length < 15) {
						// Set active char cursor position
						Utilities.S.PositionCursor(charSlotsText[inputString.Length].gameObject, 0, 10, 3, 1);
					} else {
						// Deactivate cursor
						ScreenCursor.S.cursorGO[1].SetActive(false);

						// Set Selected Gameobject (Keyboard Input Menu: OK Button)
						canUpdate = true;
						Utilities.S.SetSelectedGO(buttonsGO[92]);
					}

					// Display text
					PauseMessage.S.DisplayText(WordManager.S.GetRandomExclamation() + "!\nYeah, you add that character!");

					// Audio: Confirm
					AudioManager.S.PlaySFX(eSoundName.confirm);
				}
			}
        } else {
			// Input box shake animation
			inputBoxAnim.CrossFade("Shake", 0);

			// Audio: Damage
			AudioManager.S.PlayRandomDamageSFX();

			// Display text
			PauseMessage.S.DisplayText(WordManager.S.GetRandomInterjection() + "!\nYa can't add anymore characters;\nthere's no more room!");
		}
	}

	// Remove the last char from the displayed name
	public void Backspace() {
		if (inputString.Length > 0) {
			// Remove last char from the string
			inputString = inputString.Remove(inputString.Length - 1);
			DisplayText(inputString + GetRemainingWhitespace());

			// Set active char cursor position
			ScreenCursor.S.cursorGO[1].SetActive(true);
			ScreenCursor.S.ResetAnimClip();
			Utilities.S.PositionCursor(charSlotsText[inputString.Length].gameObject, 0, 10, 3, 1);

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);

			// Display text
			PauseMessage.S.DisplayText(WordManager.S.GetRandomExclamation() + "!\nYeah, you delete that character!");
		} else {
			// Input box shake animation
			inputBoxAnim.CrossFade("Shake", 0);

			// Audio: Damage
			AudioManager.S.PlayRandomDamageSFX();

			// Display text
			PauseMessage.S.DisplayText(WordManager.S.GetRandomInterjection() + "!\nYa can't delete anymore characters;\nthere's nothing left to delete!");
		}
	}

	// Sets the displayed name to a predetermined default name
	public void DontCare() {
		// Get a default name
		inputString = dontCareNames[dontCareNdx];
		DisplayText(inputString + GetRemainingWhitespace());

		// Set active char cursor position
		ScreenCursor.S.cursorGO[1].SetActive(true);
		ScreenCursor.S.ResetAnimClip();
		Utilities.S.PositionCursor(charSlotsText[inputString.Length].gameObject, 0, 10, 3, 1);

		// Increment index
		if (dontCareNdx < dontCareNames.Count - 1) {
			dontCareNdx += 1;
		} else {
			dontCareNdx = 0;
		}

		// Display text
		PauseMessage.S.DisplayText(WordManager.S.GetRandomExclamation() + "!\nNice \"choice\", lazy bones!");

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);
	}

	public void OK() {
		subMenuIsActive = true;

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);

		// Deactivate cursors
		Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

		// Reset OK button text color	
		buttonsGO[92].gameObject.GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);

		PauseMessage.S.DisplayText("Are you sure about this name?\nWell, are ya?", false, true);
		GameManager.S.gameSubMenu.SetText();

		// Set OnClick Methods
		Utilities.S.RemoveListeners(GameManager.S.gameSubMenu.buttonCS);
		GameManager.S.gameSubMenu.buttonCS[0].onClick.AddListener(delegate { Yes(1); });
		GameManager.S.gameSubMenu.buttonCS[1].onClick.AddListener(No);

		// Set button navigation
		Utilities.S.SetButtonNavigation(GameManager.S.gameSubMenu.buttonCS[0], GameManager.S.gameSubMenu.buttonCS[1], GameManager.S.gameSubMenu.buttonCS[1]);
		Utilities.S.SetButtonNavigation(GameManager.S.gameSubMenu.buttonCS[1], GameManager.S.gameSubMenu.buttonCS[0], GameManager.S.gameSubMenu.buttonCS[0]);
	}

	public void Yes(int ndx) {
		// Set selected party member's name
		Party.S.stats[ndx].name = inputString;

		// Audio: Buff 1
		AudioManager.S.PlaySFX(eSoundName.buff1);

		subMenuIsActive = false;

		// Display text
		DialogueManager.S.ResetSettings();
		PauseMessage.S.DisplayText(WordManager.S.GetRandomExclamation() + "!\nThe name has been set!");

		// Set Selected Gameobject
		Utilities.S.SetSelectedGO(previousSelectedGameObject);

		// Activate Cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);
		ScreenCursor.S.cursorGO[1].SetActive(true);
		ScreenCursor.S.ResetAnimClip();

		canUpdate = true;
	}

	public void No() {
		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.deny);

		subMenuIsActive = false;

		DialogueManager.S.ResetSettings();
		PauseMessage.S.DisplayText("Oh, okay. That's cool.\nSo what's the name?");

		// Set Selected Gameobject
		Utilities.S.SetSelectedGO(previousSelectedGameObject);

		// Activate Cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);
		ScreenCursor.S.cursorGO[1].SetActive(true);
		ScreenCursor.S.ResetAnimClip();

		canUpdate = true;
	}

	// Returns a string of whitespace as long as the amount of remaining empty chars 
	public string GetRemainingWhitespace() {
		string remainingChars = "";
		if (inputString.Length < 15) {
			// Get amount of remaining space
			int amountOfRemainingChars = 15 - inputString.Length;

			// Populate string with whitespace 
			for (int j = 0; j < amountOfRemainingChars; j++) {
				remainingChars += " ";
			}
		}

		return remainingChars;
	}

	// Display each char of the 'text' argument in an individual Text object
	public void DisplayText(string text) {
		for(int i = 0; i < charSlotsText.Count; i++) {
			charSlotsText[i].text = text[i].ToString();
		}
    }

	//  private void Update() {
	//foreach (char cIt in Input.inputString) {
	//	if (characters.Contains(cIt)) {
	//		Debug.Log(cIt);
	//	}

	//	if(cIt == '\b') {
	//		Debug.Log(cIt);
	//          }
	//}
	//  }
}