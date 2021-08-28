using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dialogue menu that offers the player to choose from multiple options (up to 4)
/// </summary>
public class SubMenu : MonoBehaviour {
	[Header ("Set in Inspector")]
	// Option Buttons and GameObjects
	public List <GameObject>	buttonGO;
	public List <Button>		buttonCS;

	// Text
	public List <Text>			text;

	// Frame Position
	public RectTransform		frameRT;
	// Cursor Position
	public RectTransform		cursorRT;

	[Header("Set Dynamically")]
	// Singleton
	private static SubMenu _S;
	public static SubMenu S { get { return _S; } set { _S = value; } }

	private bool 				canUpdate;

	void Awake() {
		S = this;
	}

	void OnEnable () {
		canUpdate = true;
	}

	public void Loop () {
		// Reset canUpdate
		if (Input.GetAxisRaw ("Horizontal") != 0f || Input.GetAxisRaw ("Vertical") != 0f) { 
			canUpdate = true;
		}

		// Set Cursor Position to Selected Button
		if (canUpdate) {
			Vector2 selectedButtonPos = Vector2.zero;

			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == buttonGO[0]) {
				selectedButtonPos.x = buttonGO[0].GetComponent<RectTransform> ().anchoredPosition.x;
				selectedButtonPos.y = buttonGO[0].GetComponent<RectTransform> ().anchoredPosition.y;
			} else if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == buttonGO[1]) {
				selectedButtonPos.x = buttonGO[1].GetComponent<RectTransform> ().anchoredPosition.x;
				selectedButtonPos.y = buttonGO[1].GetComponent<RectTransform> ().anchoredPosition.y;
			}else if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == buttonGO[2]) {
				selectedButtonPos.x = buttonGO[2].GetComponent<RectTransform> ().anchoredPosition.x;
				selectedButtonPos.y = buttonGO[2].GetComponent<RectTransform> ().anchoredPosition.y;
			}else if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == buttonGO[3]) {
				selectedButtonPos.x = buttonGO[3].GetComponent<RectTransform> ().anchoredPosition.x;
				selectedButtonPos.y = buttonGO[3].GetComponent<RectTransform> ().anchoredPosition.y;
			}
			cursorRT.anchoredPosition = new Vector2 ((selectedButtonPos.x + 150), (selectedButtonPos.y));

			// Prevent contents of this if statement from being called until next user directional input
			canUpdate = false;
		}
	}

	public void SetText(string option1 = "Yes", string option2 = "No", string option3 = "3rd", string option4 = "4th", int optionAmount = 2){ 
		// Set Selected GameObject
		if(option1 == "Yes") {
			Utilities.S.SetSelectedGO(buttonGO[1]);
		} else {
			Utilities.S.SetSelectedGO(buttonGO[0]);
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
			SetTextHelper (false, false, 150);
			frameSpritePos.y = 0;
			break;
		case 3:
			SetTextHelper (true, false, 200);
			frameSpritePos.y = -25;
			break;
		case 4:
			SetTextHelper (true, true, 250);
			frameSpritePos.y = -50;
			break;
		}

		// Set Sprite Frame Position
		frameRT.anchoredPosition = frameSpritePos;
	}

	void SetTextHelper(bool has3Options, bool has4Options, int frameSizeY){
		// Activate Text gameObjects
		text[2].gameObject.SetActive (has3Options);
		text[3].gameObject.SetActive (has4Options);

		// Buttons Interactable
		buttonCS[2].interactable = has3Options;
		buttonCS[3].interactable = has4Options;

		// Set Frame Height
		frameRT.sizeDelta = new Vector2(400, frameSizeY);
	}
}