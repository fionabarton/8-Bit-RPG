using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGameMenu : MonoBehaviour{
    [Header("Set in Inspector")]
    // Yes/No Buttons and GameObjects
    public List<GameObject> buttonGO;
    public List<Button> buttonCS;

	// Text displayed in dialogue box
	public Text textMessage;

	// Cursor Position
	public RectTransform cursorRT;

	[Header("Set Dynamically")]
    // Allows parts of Loop() to be called once rather than repeatedly every frame.
    private bool canUpdate;

	// Dicatates whether listeners are set here or elsewhere
	private bool isExitGameSubMenu;

	// Ensures audio is only played once when button is selected
	GameObject previousSelectedGameObject;

	private static ExitGameMenu _S;
	public static ExitGameMenu S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;
	}

	void OnEnable() {
        canUpdate = true;
    }

	void Start() {
		gameObject.SetActive(false);
	}

	// Add listener in Inspector: PauseMenu > Buttons > ExitGameButton
	public void ActivateInPauseMenuInspector() {
		Activate();
	}

	public void Activate(string message = "Are you sure that you\nwould like to exit the game?", bool addExitGameListeners = true) {
		// Set text
		textMessage.text = message;

		// Set selected gameObject
		Utilities.S.SetSelectedGO(buttonGO[1]);
		previousSelectedGameObject = buttonGO[1];

		// Add Loop() to Update Delgate
		UpdateManager.updateDelegate += Loop;

        // Set OnClick Methods
        if (addExitGameListeners) {
			Utilities.S.RemoveListeners(buttonCS);
			buttonCS[0].onClick.AddListener(Yes);
			buttonCS[1].onClick.AddListener(No);

			isExitGameSubMenu = true;
        } else {
			isExitGameSubMenu = false;
        }

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);

		gameObject.SetActive(true);

		canUpdate = true;
	}

	public void Deactivate(bool playSound = false) {
		if (GameManager.S.currentScene != "Title_Screen") {
			// Buttons Interactable
			Utilities.S.ButtonsInteractable(PauseMenu.S.buttonCS, true);

			PauseMenu.S.canUpdate = true;
		} else {
			// Set Selected GameObject (New Game Button)
			Utilities.S.SetSelectedGO(TitleMenu.S.previousSelectedButton);

			// Buttons Interactable
			Utilities.S.ButtonsInteractable(TitleMenu.S.buttons, true);
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
		if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f) {
			canUpdate = true;
		}

        if (isExitGameSubMenu) {
			if (Input.GetButtonDown("SNES Y Button")) {
				No();
			}
		}

		// Set Cursor Position to Selected Button
		if (canUpdate) {
			Vector2 selectedButtonPos = Vector2.zero;

			for (int i = 0; i < buttonGO.Count; i++) {
				if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == buttonGO[i]) {
					selectedButtonPos.x = buttonGO[i].GetComponent<RectTransform>().anchoredPosition.x;
					selectedButtonPos.y = buttonGO[i].GetComponent<RectTransform>().anchoredPosition.y;

					// Set selected button text color	
					buttonGO[i].GetComponentInChildren<Text>().color = new Color32(205, 208, 0, 255);
				} else {
					// Set non-selected button text color
					if (buttonGO[i].transform.GetChild(0).gameObject.activeInHierarchy) {
						buttonGO[i].GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
					}
				}
			}

			cursorRT.anchoredPosition = new Vector2((selectedButtonPos.x + 150), (selectedButtonPos.y));

			// Audio: Selection (when a new gameObject is selected)
			Utilities.S.PlayButtonSelectedSFX(ref previousSelectedGameObject);

			// Prevent contents of this if statement from being called until next user directional input
			canUpdate = false;
		}
	}

	public void Yes() {
		// Audio: Buff 1
		AudioManager.S.PlaySFX(eSoundName.buff1);

		YesNoHelper();

		// Quit application
		Application.Quit();
	}

	public void No() {
		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.deny);

		YesNoHelper();
	}

	void YesNoHelper() {
		// Set Selected GameObject: Exit Game Button
		if (!GameManager.S.paused) {
			Utilities.S.SetSelectedGO(TitleMenu.S.buttons[3].gameObject);
		} else {
			Utilities.S.SetSelectedGO(PauseMenu.S.buttonGO[4]);
		}

		Deactivate();
	}
}