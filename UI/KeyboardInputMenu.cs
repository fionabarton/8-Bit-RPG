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

	public GameObject startingSelectedGO;

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
		canUpdate = true;

		// Freeze player
		GameManager.S.paused = true;
		Blob.S.canMove = false;

		// Activate Cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);

		// Set Selected Gameobject (Keyboard Input Menu: A Button)
		Utilities.S.SetSelectedGO(startingSelectedGO);

		// Initialize previously selected GameObject
		previousSelectedGameObject = startingSelectedGO;

		// Update Delgate
		UpdateManager.updateDelegate += Loop;

		gameObject.SetActive(true);
	}

	public void Deactivate() {
		// Unfreeze player
		GameManager.S.paused = false;
		Blob.S.canMove = true;

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
			ScreenCursor.S.cursorGO[0].SetActive(true);
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
					// Set selected button text color	
					buttonsGO[i].gameObject.GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
				}
			}
			canUpdate = false;
		}

		// Backspace input
		if (Input.GetButtonDown("SNES Y Button") || Input.GetKeyDown(KeyCode.Backspace)) {
			Backspace();
		}

		// Space input
		if (Input.GetKeyDown(KeyCode.Space)) {
			PressedKey(85);
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

					// Audio: Confirm
					AudioManager.S.PlaySFX(eSoundName.confirm);
				}
			}
        } else {
			// Input box shake animation
			inputBoxAnim.CrossFade("Shake", 0);

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}
	}

	// Sets the displayed name to a predetermined default name
	public void DontCare() {
		// Get a default name
		inputString = dontCareNames[dontCareNdx];
		DisplayText(inputString + GetRemainingWhitespace());

		// Increment index
		if (dontCareNdx < dontCareNames.Count - 1) {
			dontCareNdx += 1;
		} else {
			dontCareNdx = 0;
		}

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);
	}

	// Remove the last char from the displayed name
	public void Backspace() {
		if(inputString.Length > 0) {
			// Remove last char from the string
			inputString = inputString.Remove(inputString.Length - 1);
			DisplayText(inputString + GetRemainingWhitespace());
        } else {
			// Input box shake animation
			inputBoxAnim.CrossFade("Shake", 0);
		}

		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.deny);
	}

	public void OK() {

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