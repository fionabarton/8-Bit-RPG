using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour {
    [Header("Set in Inspector")]
    public List<Button> buttons;

    [Header("Set Dynamically")]
    // Allows parts of Loop() to be called once rather than repeatedly every frame.
    public bool canUpdate;

    // Ensures audio is only played once when button is selected
    public GameObject previousSelectedButton;

    private static TitleMenu _S;
    public static TitleMenu S { get { return _S; } set { _S = value; } }

    void Awake() {
        S = this;
    }

    void Start() {
        buttons[0].transform.parent.gameObject.SetActive(false);
    }

    public void Activate() {
        // Activate TitleScreenButtons gameObject
        buttons[0].transform.parent.gameObject.SetActive(true);

        // Add listeners
        buttons[0].onClick.AddListener(NewGame);
        buttons[1].onClick.AddListener(SaveMenu.S.Activate);
        buttons[2].onClick.AddListener(delegate { OptionsMenu.S.Activate(70, true); });

        // Set Selected GameObject and Position Cursor 
        if (PlayerPrefs.HasKey("0Time") || PlayerPrefs.HasKey("1Time") || PlayerPrefs.HasKey("2Time")) {
            if (PlayerPrefs.GetString("0Time") == "0:00" && PlayerPrefs.GetString("1Time") == "0:00" && PlayerPrefs.GetString("2Time") == "0:00") {
                // Set Selected GameObject: New Game Button
                SetSelectedButton(0);
            } else {
                // Set Selected GameObject: Load Game Button
                SetSelectedButton(0);
            }
        } else {
            // Set Selected GameObject: New Game Button
            SetSelectedButton(0);
        }

        // Activate Cursor
        ScreenCursor.S.cursorGO[0].SetActive(true);

        // Prevent player input
        Player.S.canMove = false;

        // Add Loop() to Update Delgate
        UpdateManager.updateDelegate += Loop;
    }

    void SetSelectedButton(int ndx) {
        Utilities.S.SetSelectedGO(buttons[ndx].gameObject);
        Utilities.S.PositionCursor(buttons[ndx].gameObject, 150, -10);

        // Set selected button text color	
        buttons[ndx].gameObject.GetComponentInChildren<Text>().color = new Color32(205, 208, 0, 255);
    }

    public void Deactivate() {
        // Deactivate TitleScreenButtons gameObject
        buttons[0].transform.parent.gameObject.SetActive(false);

        // Deactivate screen cursors
        Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

        // Update Delegate
        UpdateManager.updateDelegate -= Loop;
    }

    public void Loop() {
        // Reset canUpdate
        if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f) {
            canUpdate = true;
        }

        if (canUpdate) {
            //if (!OptionsMenu.S.gameObject.activeInHierarchy && !SaveMenu.S.gameObject.activeInHierarchy) {
            if (!OptionsMenu.S.gameObject.activeInHierarchy) {
                for (int i = 0; i < buttons.Count; i++) {
                    if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == buttons[i].gameObject) {
                        // Set Cursor Position set to Selected Button
                        Utilities.S.PositionCursor(buttons[i].gameObject, 150, -10);

                        // Set selected button text color	
                        buttons[i].gameObject.GetComponentInChildren<Text>().color = new Color32(205, 208, 0, 255);

                        // Audio: Selection (when a new gameObject is selected)
                        Utilities.S.PlayButtonSelectedSFX(ref previousSelectedButton);
                    } else {
                        // Set non-selected button text color
                        buttons[i].gameObject.GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
                    }
                }

            }
        }
    }

    public void NewGame() {
        // Remove listeners
        Utilities.S.RemoveListeners(buttons);

        // Close Curtains
        Curtain.S.Close();
        //// Activate Black Screen
        //ColorScreen.S.ActivateBlackScreen();

        // Audio: Buff 2
        AudioManager.S.PlaySFX(eSoundName.buff2);

        // Delay, then Load Scene
        Invoke("LoadFirstScene", 1f);
    }

    void LoadFirstScene() {
        Deactivate();

        KeyboardInputMenu.S.Activate(2);

        // Open Curtains
        Curtain.S.Open();
        //// Deactivate Black Screen
        //ColorScreen.S.anim.Play("Clear Screen", 0, 0);

        //GameManager.S.LoadLevel("Playground");
    }
}