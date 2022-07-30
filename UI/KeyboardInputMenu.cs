using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class KeyboardInputMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	public List<GameObject> buttonsGO;

	[Header("Set Dynamically")]
	public Text inputBoxText;

	public string inputString;

	public GameObject startingSelectedGO;

	private string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvxyz0123456789()[]!?.,~@#$%^&*+-=_ \" \' / \\ :;";

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
	}

	public void PressedKey(int ndx) {
		for (int i = 0; i < characters.Length; i++) {
			if(i == ndx) {
				// Add user keyboard input to string
				inputString += characters[ndx];
				inputBoxText.text = inputString + GetRemainingChars();
			}
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
	}

	public void Backspace() {
		if(inputString.Length > 0) {
			// Remove last char from the string
			inputString = inputString.Remove(inputString.Length - 1);
			inputBoxText.text = inputString + GetRemainingChars();
		}
	}

	public void OK() {

    }

	public string GetRemainingChars() {
		// Get a string of remaining chars
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