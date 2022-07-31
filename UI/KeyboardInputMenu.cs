using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// Handles creating character names via user keyboard input
public class KeyboardInputMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	public List<GameObject> buttonsGO;

	[Header("Set Dynamically")]
	public Text inputBoxText;

	public string inputString;

	public GameObject startingSelectedGO;

	// 0-25, 26-51, 52-61, 62-65, 66-69, 70-89
	private string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789()[]!?.,~@#$%^&*+-=_\"'` :;/\\";

	// 
	private int dontCareNdx;
	private List<string> dontCareNames = new List<string>() { "Butthead", "Mildew", "Gunt", "Love Gum", "Moon Unit" };

	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

	// Ensures audio is only played once when button is selected
	public GameObject previousSelectedGameObject;

	private static KeyboardInputMenu _S;
	public static KeyboardInputMenu S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;
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

		// Backspace
		if (Input.GetButtonDown("SNES Y Button")) {
			Backspace();
		}

		// Space
		if (Input.GetKeyDown(KeyCode.Space)) {
			PressedKey(85);
		}
	}

	public void PressedKey(int ndx) {
		if (inputString.Length < 15) {
			for (int i = 0; i < characters.Length; i++) {
				if (i == ndx) {
					// Add user keyboard input to string
					inputString += characters[ndx];
					inputBoxText.text = inputString + GetRemainingChars();

					// Audio: Confirm
					AudioManager.S.PlaySFX(eSoundName.confirm);
				}
			}
        } else {
			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}
	}

	public void DontCare() {
		inputString = dontCareNames[dontCareNdx];
		inputBoxText.text = inputString + GetRemainingChars();

		// Increment index
		if (dontCareNdx < dontCareNames.Count - 1) {
			dontCareNdx += 1;
		} else {
			dontCareNdx = 0;
		}

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);
	}

	public void Backspace() {
		if(inputString.Length > 0) {
			// Remove last char from the string
			inputString = inputString.Remove(inputString.Length - 1);
			inputBoxText.text = inputString + GetRemainingChars();
		}

		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.deny);
	}

	public void OK() {

    }

	// Returns a string of remaining chars
	public string GetRemainingChars() {
		string remainingChars = "";
		if (inputString.Length < 15) {
			// Get amount of remaing space
			int amountOfRemainingChars = 15 - inputString.Length;

			for (int j = 0; j < amountOfRemainingChars; j++) {
				remainingChars += "_";
			}
		}

		return remainingChars;
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