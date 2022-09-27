using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handles creating character names via user keyboard input
public class KeyboardInputMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	// List of buttons for each letter, number, symbol, etc. input
	public List<GameObject> buttonsGO; 
	public List<Button> buttonsCS;

	// Each slots represents one char 
	public List<Text> charSlotsText;

	// Displayed party member image animator
	public Animator playerImageAnim;

	// Cached animator controllers for each party member; to be assigned dynamically to playerImageAnim.runtimeAnimatorController
	public List<RuntimeAnimatorController> playerAnimatorControllers;

	[Header("Set Dynamically")]
	public eKeyboardInputMenuMode mode = eKeyboardInputMenuMode.editName;
	
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

	// Scene to load after confirming name
	public string sceneToLoad = "Playground";

	private static KeyboardInputMenu _S;
	public static KeyboardInputMenu S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;

		inputBoxAnim = GetComponentInChildren<Animator>();
	}

	void Start() {
		gameObject.SetActive(false);
	}

	public void Activate(int partyMemberNdx = 0, string _sceneToLoad = "Playground") {
		if (GameManager.S.currentScene != "Title_Screen") {
			// Deactivate 'Back To Menu' button  
			buttonsGO[93].SetActive(false);

			// Set buttons navigation
			Utilities.S.SetButtonNavigation(buttonsCS[31], buttonsCS[56], buttonsCS[36], buttonsCS[4], buttonsCS[32]); // a 
			Utilities.S.SetButtonNavigation(buttonsCS[32], buttonsCS[57], buttonsCS[37], buttonsCS[31], buttonsCS[33]); // b 
			Utilities.S.SetButtonNavigation(buttonsCS[33], buttonsCS[58], buttonsCS[38], buttonsCS[32], buttonsCS[34]); // c 
			Utilities.S.SetButtonNavigation(buttonsCS[34], buttonsCS[59], buttonsCS[39], buttonsCS[33], buttonsCS[35]); // d 
			Utilities.S.SetButtonNavigation(buttonsCS[35], buttonsCS[60], buttonsCS[40], buttonsCS[34], buttonsCS[61]); // e 

			Utilities.S.SetButtonNavigation(buttonsCS[56], buttonsCS[51], buttonsCS[31], buttonsCS[29], buttonsCS[57]); // z 
			Utilities.S.SetButtonNavigation(buttonsCS[57], buttonsCS[52], buttonsCS[32], buttonsCS[56], buttonsCS[58]); // !
			Utilities.S.SetButtonNavigation(buttonsCS[58], buttonsCS[53], buttonsCS[33], buttonsCS[57], buttonsCS[59]); // ?
			Utilities.S.SetButtonNavigation(buttonsCS[59], buttonsCS[54], buttonsCS[34], buttonsCS[58], buttonsCS[60]); // .
			Utilities.S.SetButtonNavigation(buttonsCS[60], buttonsCS[55], buttonsCS[35], buttonsCS[59], buttonsCS[86]); // ,

			Utilities.S.SetButtonNavigation(buttonsCS[30], buttonsCS[25], buttonsCS[0], buttonsCS[92], buttonsCS[91]); // Don't Care
			Utilities.S.SetButtonNavigation(buttonsCS[91], buttonsCS[86], buttonsCS[61], buttonsCS[30], buttonsCS[92]); // Backspace
		} else {
			// Activate 'Back To Menu' button  
			buttonsGO[93].SetActive(true);

			// Set buttons navigation
			Utilities.S.SetButtonNavigation(buttonsCS[31], buttonsCS[93], buttonsCS[36], buttonsCS[4], buttonsCS[32]); // a 
			Utilities.S.SetButtonNavigation(buttonsCS[32], buttonsCS[93], buttonsCS[37], buttonsCS[31], buttonsCS[33]); // b 
			Utilities.S.SetButtonNavigation(buttonsCS[33], buttonsCS[93], buttonsCS[38], buttonsCS[32], buttonsCS[34]); // c 
			Utilities.S.SetButtonNavigation(buttonsCS[34], buttonsCS[93], buttonsCS[39], buttonsCS[33], buttonsCS[35]); // d 
			Utilities.S.SetButtonNavigation(buttonsCS[35], buttonsCS[93], buttonsCS[40], buttonsCS[34], buttonsCS[61]); // e 

			Utilities.S.SetButtonNavigation(buttonsCS[56], buttonsCS[51], buttonsCS[93], buttonsCS[29], buttonsCS[57]); // z 
			Utilities.S.SetButtonNavigation(buttonsCS[57], buttonsCS[52], buttonsCS[93], buttonsCS[56], buttonsCS[58]); // !
			Utilities.S.SetButtonNavigation(buttonsCS[58], buttonsCS[53], buttonsCS[93], buttonsCS[57], buttonsCS[59]); // ?
			Utilities.S.SetButtonNavigation(buttonsCS[59], buttonsCS[54], buttonsCS[93], buttonsCS[58], buttonsCS[60]); // .
			Utilities.S.SetButtonNavigation(buttonsCS[60], buttonsCS[55], buttonsCS[93], buttonsCS[59], buttonsCS[86]); // ,

			Utilities.S.SetButtonNavigation(buttonsCS[30], buttonsCS[25], buttonsCS[0], buttonsCS[92], buttonsCS[93]); // Don't Care
			Utilities.S.SetButtonNavigation(buttonsCS[91], buttonsCS[86], buttonsCS[61], buttonsCS[93], buttonsCS[92]); // Backspace
		}

		// Set mode
		mode = eKeyboardInputMenuMode.editName;

		// Set scene to load
		sceneToLoad = _sceneToLoad;

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
		Utilities.S.PositionCursor(charSlotsText[inputString.Length].gameObject, 0, 60, 3, 1);

		// Activate PauseMessage
		PauseMessage.S.DisplayText("Yo.\nSo what's the name, ya little goon?", false, false, -374);

		// Update Delgate
		UpdateManager.updateDelegate += Loop;

		gameObject.SetActive(true);

		// Set party member image animator controller
		playerImageAnim.runtimeAnimatorController = playerAnimatorControllers[partyMemberNdx] as RuntimeAnimatorController;

		// Set party member image animation clip
		playerImageAnim.CrossFade("Walk", 0);

		// Audio: Never
		AudioManager.S.PlaySong(eSongName.never);
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
					Utilities.S.PositionCursor(buttonsGO[i], -30, 40, 0);

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

		switch (mode) {
			case eKeyboardInputMenuMode.editName:
				// Backspace input
				if (Input.GetButtonDown("SNES Y Button") || Input.GetKeyDown(KeyCode.Backspace)) {
					Backspace();
				}

				// Space input
				if (Input.GetKeyDown(KeyCode.Space)) {
					PressedKey(85);
				}
				break;
			case eKeyboardInputMenuMode.nameConfirmation:
				if (Input.GetButtonDown("SNES Y Button")) {
					No();
				}
				break;
			case eKeyboardInputMenuMode.nameConfirmed:
				if (PauseMessage.S.dialogueFinished) {
					if (Input.GetButtonDown("SNES B Button")) {
						// Close Curtains
						Curtain.S.Close();

						// Audio: Buff 2
						AudioManager.S.PlaySFX(eSoundName.buff2);

						// Delay, then Load Scene
						Invoke("LoadScene", 1f);

						mode = eKeyboardInputMenuMode.loadingScene;
					}
				}
				break;
			case eKeyboardInputMenuMode.loadingScene:
				break;
        }
	}

	void LoadScene() {
		Deactivate();

		// Open Curtains
		Curtain.S.Open();

		// Load scene
		GameManager.S.LoadLevel(sceneToLoad);
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
						Utilities.S.PositionCursor(charSlotsText[inputString.Length].gameObject, 0, 60, 3, 1);
					} else {
						// Deactivate cursor
						ScreenCursor.S.cursorGO[1].SetActive(false);

						// Set Selected Gameobject (Keyboard Input Menu: OK Button)
						canUpdate = true;
						Utilities.S.SetSelectedGO(buttonsGO[92]);
					}

					// Display text
					PauseMessage.S.DisplayText(WordManager.S.GetRandomExclamation() + "!\nYeah, you add that character!", false, false, -374);

					// Audio: Confirm
					AudioManager.S.PlaySFX(eSoundName.confirm);
				}
			}

			// Set party member image animation clip
			playerImageAnim.CrossFade("Walk", 0);
		} else {
			// Input box shake animation
			inputBoxAnim.CrossFade("Shake", 0);

			// Audio: Damage
			AudioManager.S.PlayRandomDamageSFX();

			// Display text
			PauseMessage.S.DisplayText(WordManager.S.GetRandomInterjection() + "!\nYa can't add anymore characters;\nthere's no more room!", false, false, -374);

			// Set party member image animation clip
			playerImageAnim.CrossFade("Damage", 0);
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
			Utilities.S.PositionCursor(charSlotsText[inputString.Length].gameObject, 0, 60, 3, 1);

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);

			// Display text
			PauseMessage.S.DisplayText(WordManager.S.GetRandomExclamation() + "!\nYeah, you delete that character!", false, false, -374);

			// Set party member image animation clip
			playerImageAnim.CrossFade("Walk", 0);
		} else {
            // Input box shake animation
            inputBoxAnim.CrossFade("Shake", 0);

            // Audio: Damage
            AudioManager.S.PlayRandomDamageSFX();

            // Display text
            PauseMessage.S.DisplayText(WordManager.S.GetRandomInterjection() + "!\nYa can't delete anymore characters;\nthere's nothing left to delete!", false, false, -374);

            // Set party member image animation clip
            playerImageAnim.CrossFade("Damage", 0);
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
		Utilities.S.PositionCursor(charSlotsText[inputString.Length].gameObject, 0, 60, 3, 1);

		// Increment index
		if (dontCareNdx < dontCareNames.Count - 1) {
			dontCareNdx += 1;
		} else {
			dontCareNdx = 0;
		}

		// Display text
		PauseMessage.S.DisplayText(WordManager.S.GetRandomExclamation() + "!\nNice \"choice\", lazy bones!", false, false, -374);

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);
	}

	public void BackToMenu() {
		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.deny);

		// Audio: Soap
		AudioManager.S.PlaySong(eSongName.soap);

		Deactivate();
        TitleMenu.S.Activate();
		TitleMenu.S.SetSelectedButton(0);
	}

	public void OK() {
		// Set mode
		mode = eKeyboardInputMenuMode.nameConfirmation;

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);

		// Deactivate cursors
		Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

		// Reset OK button text color	
		buttonsGO[92].gameObject.GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);

		ExitGameMenu.S.Activate("Are you sure about this name?\nWell, are ya?", false);
		
		// Set OnClick Methods
		Utilities.S.RemoveListeners(ExitGameMenu.S.buttonCS);
		ExitGameMenu.S.buttonCS[0].onClick.AddListener(delegate { Yes(0); });
		ExitGameMenu.S.buttonCS[1].onClick.AddListener(No);

		// Set party member image animation clip
		playerImageAnim.CrossFade("Idle", 0);
	}

	public void Yes(int ndx) {
		ExitGameMenu.S.Deactivate();

		// Set mode
		mode = eKeyboardInputMenuMode.nameConfirmed;

		// Set selected party member's name
		Party.S.stats[ndx].name = inputString;

		// Audio: Win
		StartCoroutine(AudioManager.S.PlaySongThenResumePreviousSong(6));

		// Display text
		DialogueManager.S.ResetSettings();
		PauseMessage.S.DisplayText(WordManager.S.GetRandomExclamation() + "!\nThe name has been set!\nPress the action button to proceed!", false, false, -374);

		// Set Selected Gameobject
		Utilities.S.SetSelectedGO(null);

		// Activate Cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);
		ScreenCursor.S.cursorGO[1].SetActive(true);
		ScreenCursor.S.ResetAnimClip();

		// Set party member image animation clip
		playerImageAnim.CrossFade("Success", 0);

		canUpdate = true;
	}

	public void No() {
		ExitGameMenu.S.Deactivate();

		// Set mode
		mode = eKeyboardInputMenuMode.editName;

		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.deny);

		DialogueManager.S.ResetSettings();
		PauseMessage.S.DisplayText("Oh, okay. That's cool.\nSo what's the name?", false, false, -374);

		// Set Selected Gameobject
		Utilities.S.SetSelectedGO(previousSelectedGameObject);

		// Activate Cursor
		ScreenCursor.S.cursorGO[0].SetActive(true);
		ScreenCursor.S.cursorGO[1].SetActive(true);
		ScreenCursor.S.ResetAnimClip();

		// Set party member image animation clip
		playerImageAnim.CrossFade("Walk", 0);

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
}