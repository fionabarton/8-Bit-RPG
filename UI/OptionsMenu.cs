using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	public List<Slider> sliders;
	public List<GameObject> sliderTextGO;
	public List<string> sliderDescriptions = new List<string> {
		"Set the master volume!",
		"Set the background music volume!",
		"Set the sound effects volume!",
		"Set the rate at which text is displayed!",
		"Enable Quick Time Events (QTE) in battle!"};

	// Rect transform (for positioning game object)
	public RectTransform rectTrans;

	public GameObject blackScreenGO;

	[Header("Set Dynamically")]
	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	public bool canUpdate;

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
		// Reset to default settings
		//PlayerPrefs.SetFloat("Master Volume", 0.25f);
		//PlayerPrefs.SetFloat("BGM Volume", 0.5f);
		//PlayerPrefs.SetFloat("SFX Volume", 0.5f);
		//PlayerPrefs.SetFloat("Text Speed", 0.05f);

		// Load settings
		if (PlayerPrefs.HasKey("Master Volume")) {
			sliders[0].value = PlayerPrefs.GetFloat("Master Volume");
			AudioManager.S.SetMasterVolume(sliders[0].value);
		} else {
			AudioManager.S.SetMasterVolume(0.25f);
		}
		if (PlayerPrefs.HasKey("BGM Volume")) {
			sliders[1].value = PlayerPrefs.GetFloat("BGM Volume");
			AudioManager.S.SetBGMVolume(sliders[1].value);
		} else {
			AudioManager.S.SetBGMVolume(0.5f);
		}
		if (PlayerPrefs.HasKey("SFX Volume")) {
			sliders[2].value = PlayerPrefs.GetFloat("SFX Volume");
			AudioManager.S.SetSFXVolume(sliders[2].value);
		} else {
			AudioManager.S.SetSFXVolume(0.5f);
		}
		if (PlayerPrefs.HasKey("Text Speed")) {
			sliders[3].value = PlayerPrefs.GetFloat("Text Speed");
			textSpeed = sliders[3].value;
		} else {
			textSpeed = 0.05f;
		}
		if (PlayerPrefs.HasKey("QTE Enabled")) {
			sliders[4].value = PlayerPrefs.GetInt("QTE Enabled");
			EnableQTE((int)sliders[4].value, false);
		} else {
			EnableQTE(1, false);
		}

		// Adds a listener to each slider and invokes a method when the value changes
		sliders[0].onValueChanged.AddListener(delegate { SetMasterVolume(); });
		sliders[1].onValueChanged.AddListener(delegate { SetBGMVolume(); });
		sliders[2].onValueChanged.AddListener(delegate { SetSFXVolume(); });
		sliders[3].onValueChanged.AddListener(delegate { SetTextSpeed(); });
		sliders[4].onValueChanged.AddListener(delegate { EnableQTE((int)sliders[4].value); });

		Deactivate();
	}

	// Set in Inspector on OptionsScreen
	public void Activate(float anchoredYPosition = 70, bool activateBlackScreen = false) {
		// Buttons Interactable
		Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, false);

		gameObject.SetActive(true);

		// Position game object
		rectTrans.anchoredPosition = new Vector2(0, anchoredYPosition);

		// Activate black screen
		blackScreenGO.SetActive(activateBlackScreen);

		// Set Selected Gameobject 
		Utilities.S.SetSelectedGO(sliders[0].gameObject);

		// Set Cursor Position set to Selected Button
		Utilities.S.PositionCursor(sliderTextGO[0], -125, 0, 0);

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
		AudioManager.S.SetMasterVolume(sliders[0].value);

		// Save settings
		PlayerPrefs.SetFloat("Master Volume", sliders[0].value);

		// Audio: Selection
		AudioManager.S.masterVolSelection.Play();
	}

	public void SetBGMVolume() {
		// Set the volume of all BGMs to the value of its slider
		AudioManager.S.SetBGMVolume(sliders[1].value);

		// Save settings
		PlayerPrefs.SetFloat("BGM Volume", sliders[1].value);

		// Audio: Selection
		AudioManager.S.bgmCS[8].Play();
	}

	public void SetSFXVolume() {
		// Set the volume of all SFXs to the value of its slider
		AudioManager.S.SetSFXVolume(sliders[2].value);

		// Save settings
		PlayerPrefs.SetFloat("SFX Volume", sliders[2].value);

		// Audio: Selection
		AudioManager.S.PlaySFX(eSoundName.selection);
	}

	public void SetTextSpeed() {
		// Set the text speed to the value of its slider
		textSpeed = sliders[3].value;

		// Save settings
		PlayerPrefs.SetFloat("Text Speed", sliders[3].value);

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
		PlayerPrefs.SetInt("QTE Enabled", (int)sliders[4].value);

        // Audio: Selection
        if (playSFX) {
			AudioManager.S.masterVolSelection.Play();
		}
	}

	public void SetCursorPosition() {
		for (int i = 0; i < sliderTextGO.Count; i++) {
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == sliders[i].gameObject) {
				PauseMessage.S.SetText(sliderDescriptions[i]);

				// Set Cursor Position set to Selected Button
				Utilities.S.PositionCursor(sliderTextGO[i], -105, 0, 0);

				// Set selected button text color	
				sliderTextGO[i].GetComponent<Text>().color = new Color32(205, 208, 0, 255);

				// Audio: Selection (when a new gameObject is selected)
				Utilities.S.PlayButtonSelectedSFX(ref Items.S.menu.previousSelectedGameObject);
			} else {
				// Set non-selected button text color
				sliderTextGO[i].GetComponent<Text>().color = new Color32(255, 255, 255, 255);
			}
		}
	}
}