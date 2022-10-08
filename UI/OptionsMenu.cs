using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	// Sliders
	public List<Slider> slidersGO;

	// Options
	public List<GameObject> optionsGO;
	public List<GameObject> optionsTextGO;
	public List<string> optionsDescriptions = new List<string> {
		"Set the master volume!",
		"Set the background music volume!",
		"Set the sound effects volume!",
		"Set the rate at which text is displayed!",
		"Enable Quick Time Events (QTE) in battle!",
		"Enable whether audio is audible!",
		"Return all settings back to their default values!"};

	// Rect transform (for positioning game object)
	public RectTransform rectTrans;

	[Header("Set Dynamically")]
	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

	public GameObject previousSelectedGameObject;

	public float textSpeed = 0.1f;

	private static OptionsMenu _S;
	public static OptionsMenu S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;
	}

	public void OnEnable() {
		// Add Loop() to Update Delgate
		UpdateManager.updateDelegate += Loop;
	}

	public void Start() {
		// Load settings
		if (PlayerPrefs.HasKey("Master Volume")) {
			slidersGO[0].value = PlayerPrefs.GetFloat("Master Volume");
			AudioManager.S.SetMasterVolume(slidersGO[0].value);
		} else {
			AudioManager.S.SetMasterVolume(0.25f);
		}
		if (PlayerPrefs.HasKey("BGM Volume")) {
			slidersGO[1].value = PlayerPrefs.GetFloat("BGM Volume");
			AudioManager.S.SetBGMVolume(slidersGO[1].value);
		} else {
			AudioManager.S.SetBGMVolume(0.5f);
		}
		if (PlayerPrefs.HasKey("SFX Volume")) {
			slidersGO[2].value = PlayerPrefs.GetFloat("SFX Volume");
			AudioManager.S.SetSFXVolume(slidersGO[2].value);
		} else {
			AudioManager.S.SetSFXVolume(0.5f);
		}
		if (PlayerPrefs.HasKey("Text Speed")) {
			slidersGO[3].value = PlayerPrefs.GetFloat("Text Speed");
			textSpeed = slidersGO[3].value;
		} else {
			textSpeed = 0.05f;
		}
		if (PlayerPrefs.HasKey("QTE Enabled")) {
			slidersGO[4].value = PlayerPrefs.GetInt("QTE Enabled");
			EnableQTE((int)slidersGO[4].value, false);
		} else {
			EnableQTE(1, false);
		}
		if (PlayerPrefs.HasKey("Mute Audio")) {
			slidersGO[5].value = PlayerPrefs.GetInt("Mute Audio");

			if(slidersGO[5].value == 0) {
				AudioManager.S.PauseAndMuteAudio();
			} 
		} 

		// Adds a listener to each slider and invokes a method when the value changes
		slidersGO[0].onValueChanged.AddListener(delegate { SetMasterVolume(); });
		slidersGO[1].onValueChanged.AddListener(delegate { SetBGMVolume(); });
		slidersGO[2].onValueChanged.AddListener(delegate { SetSFXVolume(); });
		slidersGO[3].onValueChanged.AddListener(delegate { SetTextSpeed(); });
		slidersGO[4].onValueChanged.AddListener(delegate { EnableQTE((int)slidersGO[4].value); });
		slidersGO[5].onValueChanged.AddListener(delegate { MuteAudio(); });
		optionsGO[6].GetComponent<Button>().onClick.AddListener(delegate { ResetSettings(); });

		Deactivate();
	}

	// Called in script
	public void Activate(float anchoredYPosition = 70) {
		// Buttons Interactable
		Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, false);

		gameObject.SetActive(true);

		// Position game object
		rectTrans.anchoredPosition = new Vector2(0, anchoredYPosition);

		// Set Selected Gameobject 
		Utilities.S.SetSelectedGO(slidersGO[0].gameObject);
		previousSelectedGameObject = slidersGO[0].gameObject;

		// Set Cursor Position set to Selected Button
		Utilities.S.PositionCursor(optionsTextGO[0], -125, 0, 0);

		PauseMessage.S.DisplayText("Set the master volume!");

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);

		canUpdate = true;
	}

	public void Deactivate(bool playSound = false) {
		if (GameManager.S.currentScene != "Title_Screen") {
			// Buttons Interactable
			Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, true);

			// Set previously selected GameObject
			PauseMenu.S.previousSelectedGameObject = PauseMenu.S.buttonGO[3];

			// Set Selected Gameobject (Pause Screen: Options Button)
			Utilities.S.SetSelectedGO(PauseMenu.S.buttonGO[3]);

			PauseMessage.S.DisplayText("Welcome to the Pause Screen!");

			PauseMenu.S.canUpdate = true;
		} else {
			// Set Selected GameObject (New Game Button)
			Utilities.S.SetSelectedGO(TitleMenu.S.previousSelectedButton);

			PauseMessage.S.gameObject.SetActive(false);
		}

		if (playSound) {
			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}

		// Update Delegate
		UpdateManager.updateDelegate -= Loop;

		// Deactivate this gameObject
		gameObject.SetActive(false);
	}

	public void Loop() {
		// Reset canUpdate
		if (Input.GetAxisRaw("Vertical") != 0f) {
			canUpdate = true;
		}

		if (canUpdate) {
			SetCursorPosition();
			canUpdate = false;
		}

		// Deactivate menu
		if (Input.GetButtonDown("SNES Y Button")) {
			Deactivate(true);
		}
	}

	public void SetMasterVolume() {
		// Set the volume of the AudioListener to the value of its slider
		AudioManager.S.SetMasterVolume(slidersGO[0].value);

		// Save settings
		PlayerPrefs.SetFloat("Master Volume", slidersGO[0].value);

		// Audio: Selection
		AudioManager.S.masterVolSelection.Play();
	}

	public void SetBGMVolume() {
		// Set the volume of all BGMs to the value of its slider
		AudioManager.S.SetBGMVolume(slidersGO[1].value);

		// Save settings
		PlayerPrefs.SetFloat("BGM Volume", slidersGO[1].value);

		// Audio: Selection
		AudioManager.S.bgmCS[8].Play();
	}

	public void SetSFXVolume() {
		// Set the volume of all SFXs to the value of its slider
		AudioManager.S.SetSFXVolume(slidersGO[2].value);

		// Save settings
		PlayerPrefs.SetFloat("SFX Volume", slidersGO[2].value);

		// Audio: Selection
		AudioManager.S.PlaySFX(eSoundName.selection);
	}

	public void SetTextSpeed() {
		// Set the text speed to the value of its slider
		textSpeed = slidersGO[3].value;

		// Save settings
		PlayerPrefs.SetFloat("Text Speed", slidersGO[3].value);

		// Display text
		PauseMessage.S.DisplayText("With the 'Text Speed' set at this value, text will be displayed on screen this quickly!");

		// Audio: Selection
		AudioManager.S.masterVolSelection.Play();
	}

	public void EnableQTE(int value, bool playSFX = true) {
		// Set the volume of all SFXs to the value of its slider
		if (value == 1) {
			Battle.S.qteEnabled = true;
		} else {
			Battle.S.qteEnabled = false;
		}

		// Save settings
		PlayerPrefs.SetInt("QTE Enabled", (int)slidersGO[4].value);

        // Audio: Selection
        if (playSFX) {
			AudioManager.S.masterVolSelection.Play();
		}
	}
	
	public void MuteAudio() {
		// (Un)mute audio
		AudioManager.S.PauseAndMuteAudio();

		// Save settings
		PlayerPrefs.SetInt("Mute Audio", (int)slidersGO[5].value);

		// Audio: Selection
		AudioManager.S.masterVolSelection.Play();
	}

	public void ResetSettings() {
		// Set sliders
		slidersGO[0].value = 0.25f;
		slidersGO[1].value = 0.5f;
		slidersGO[2].value = 0.5f;
		slidersGO[3].value = 0.05f;
		slidersGO[4].value = 1f;
		slidersGO[5].value = 1f;

		// Audio: Buff 1
		AudioManager.S.PlaySFX(eSoundName.buff1);
	}

	public void SetCursorPosition() {
		List<int> cursorXPos = new List<int>() { -180, -335, -290, -145, -135, -205, -280 };

		for (int i = 0; i < optionsTextGO.Count; i++) {
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == optionsGO[i].gameObject) {
				PauseMessage.S.SetText(optionsDescriptions[i]);

				// Set Cursor Position set to Selected Button
				Utilities.S.PositionCursor(optionsTextGO[i], cursorXPos[i], 48, 0);
						
				// Set selected button text color	
				optionsTextGO[i].GetComponent<Text>().color = new Color32(205, 208, 0, 255);

				// Audio: Selection (when a new gameObject is selected)
				Utilities.S.PlayButtonSelectedSFX(ref previousSelectedGameObject);
			} else {
				// Set non-selected button text color
				optionsTextGO[i].GetComponent<Text>().color = new Color32(255, 255, 255, 255);
			}
		}
	}
}