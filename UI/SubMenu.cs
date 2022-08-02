using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dialogue menu that offers the player to choose from multiple options (up to 4)
/// </summary>
public class SubMenu : MonoBehaviour {
	[Header("Set in Inspector")]
	// Option Buttons and GameObjects
	public List<GameObject> buttonGO;
	public List<Button> buttonCS;

	// Text
	public List<Text> text;

	// Frame Position
	public RectTransform frameRT;
	// Cursor Position
	public RectTransform cursorRT;

	public bool isPauseSubMenu;

	[Header("Set Dynamically")]
	// Allows parts of Loop() to be called once rather than repeatedly every frame.
	private bool canUpdate;

	// Ensures audio is only played once when button is selected
	GameObject previousSelectedGameObject;

	void OnEnable() {
		canUpdate = true;
	}

	public void Loop() {
		//if ((!GameManager.S.paused && !isPauseSubMenu) || (GameManager.S.paused && isPauseSubMenu) || GameManager.S.currentScene == "Title_Screen") {
			// Reset canUpdate
			if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f) {
				canUpdate = true;
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
		//}
	}

	public void SetText(string option1 = "Yes", string option2 = "No", string option3 = "3rd", string option4 = "4th", int optionAmount = 2) {
		// Set Selected GameObject
		if (option1 == "Yes") {
			Utilities.S.SetSelectedGO(buttonGO[1]);
			previousSelectedGameObject = buttonGO[1];
		} else {
			Utilities.S.SetSelectedGO(buttonGO[0]);
			previousSelectedGameObject = buttonGO[0];
		}

		// Get Frame Sprite Position
		Vector2 frameSpritePos = frameRT.anchoredPosition;

		// Set Text
		text[0].text = option1;
		text[1].text = option2;
		text[2].text = option3;
		text[3].text = option4;

		switch (optionAmount) {
			case 2:
				SetTextHelper(false, false, 150);
				frameSpritePos.y = 0;
				break;
			case 3:
				SetTextHelper(true, false, 200);
				frameSpritePos.y = -25;
				break;
			case 4:
				SetTextHelper(true, true, 250);
				frameSpritePos.y = -50;
				break;
		}

		// Set Sprite Frame Position
		frameRT.anchoredPosition = frameSpritePos;
	}

	void SetTextHelper(bool has3Options, bool has4Options, int frameSizeY) {
		// Activate Text gameObjects
		text[2].gameObject.SetActive(has3Options);
		text[3].gameObject.SetActive(has4Options);

		// Buttons Interactable
		buttonCS[2].interactable = has3Options;
		buttonCS[3].interactable = has4Options;

		// Set Frame Height
		frameRT.sizeDelta = new Vector2(400, frameSizeY);
	}

	public void ResetSettings() {
		// Set order in hierarchy
		//gameObject.transform.SetAsFirstSibling();

		// Set position
		Utilities.S.SetRectPosition(gameObject, -415, 80);

		// Deactivate Sub Menu
		gameObject.SetActive(false);

		// Update Delgate
		UpdateManager.fixedUpdateDelegate -= Loop;
	}
}