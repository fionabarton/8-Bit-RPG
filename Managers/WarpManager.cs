using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarpManager : MonoBehaviour {
	[Header("Set Dynamically")]
	// All warp locations 
	public List<WarpLocation> locations = new List<WarpLocation>();

	// Locations the party has already visited and can warp to
	public List<WarpLocation> visitedLocations = new List<WarpLocation>();

	// Ensures audio is only played once when button is selected
	public GameObject previousSelectedLocationGO;

	// Index and name of the player's current location
	public int locationNdx;
	public string locationName;
	public string visitedLocationNdxs;

	private static WarpManager _S;
	public static WarpManager S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;
	}

	void Start() {
		locations.Add(new WarpLocation("Starting Point", "Playground", new Vector3(0, 0, 0), "The location at which you started this wreck of a \"game\".", 1));
		locations.Add(new WarpLocation("Brown Valley", "New Scene", new Vector3(0, 0, 0), "Enemies are afoot in this region; beware, fool!", 3));
		locations.Add(new WarpLocation("Mountain Top", "Town_1", new Vector3(0, -11, 0), "A vaguely interesting area populated by a few vaguely interesting businesses.", 1));
		locations.Add(new WarpLocation("Purple Cave", "Area_5", new Vector3(0, 2, 0), "Get to the end of this regal cave and face the ultimate foe!\nBeware: the walls don't have colliders yet!", 1));
		locations.Add(new WarpLocation("Your Shack", "Shack_interior", new Vector3(0, 0, 0), "A dumpy shack that you happen to live in. Nice!", 1));
	}

	// Record that the player has visited this location
	public void HasVisited(string sceneName) {
		// Return if the party has already visited this location
		for (int i = 0; i < visitedLocations.Count; i++) {
			if (sceneName == visitedLocations[i].sceneName) {
				locationNdx = i;
				locationName = visitedLocations[i].name;
				return;
			}
		}

		// Add this location to the list of locations the party can warp to
		for (int i = 0; i < locations.Count; i++) {
			if (sceneName == locations[i].sceneName) {
				visitedLocations.Add(locations[i]);
				locationNdx = visitedLocations.Count - 1;
				locationName = locations[i].name;
				visitedLocationNdxs += i;
				return;
			}
		}
	}

	public void DisplayButtonDescriptions(List<Button> buttons, int cursorDistanceFromCenter) {
		for (int i = 0; i < visitedLocations.Count; i++) {
			if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == buttons[i].gameObject) {
				PauseMessage.S.SetText("<color=#FFFFFF>Name:</color> " +
				visitedLocations[i].name + "<color=#FFFFFF>\nDescription:</color> " + visitedLocations[i].description, true);

				// Cursor Position set to Selected Button
				Utilities.S.PositionCursor(buttons[i].gameObject, cursorDistanceFromCenter - 15, 45, 0);

				// Set selected button text color	
				buttons[i].gameObject.GetComponentInChildren<Text>().color = new Color32(205, 208, 0, 255);

				// Audio: Selection (when a new gameObject is selected)
				Utilities.S.PlayButtonSelectedSFX(ref previousSelectedLocationGO);
			} else {
				// Set non-selected button text color
				buttons[i].gameObject.GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
			}
		}
	}

	public void DeactivateUnusedButtonSlots(List<Button> buttons) {
		for (int i = 0; i < buttons.Count; i++) {
			buttons[i].gameObject.SetActive(false);

			// Deactivate unique SpellScreen or ItemScreen buttons
			if (Spells.S.menu.gameObject.activeInHierarchy) {
				Spells.S.menu.spellsButtonTypeText[i].gameObject.SetActive(false);
				Spells.S.menu.spellsButtonMPCostText[i].gameObject.SetActive(false);

				// Set slot header text
				Spells.S.menu.nameHeaderText.text = "Warp Destination:";
				Spells.S.menu.slotHeadersHolder.SetActive(false);
			} else if (Items.S.menu.gameObject.activeInHierarchy) {
				Items.S.menu.itemButtonsTypeText[i].gameObject.SetActive(false);
				Items.S.menu.itemButtonsValueText[i].gameObject.SetActive(false);
				Items.S.menu.itemButtonsQTYOwnedText[i].gameObject.SetActive(false);
				Items.S.menu.itemButtonsQTYEquippedText[i].gameObject.SetActive(false);

				// Set slot header text
				Items.S.menu.nameHeaderText.text = "Warp Destination:";
				Items.S.menu.slotHeadersHolder.SetActive(false);
			}
		}

		for (int i = 0; i < visitedLocations.Count; i++) {
			buttons[i].gameObject.SetActive(true);
		}
	}

	public void AssignButtonEffect(List<Button> buttons) {
		Utilities.S.RemoveListeners(buttons);

		for (int i = 0; i < visitedLocations.Count; i++) {
			// Add listener to Button
			int copy = i;
			buttons[copy].onClick.AddListener(delegate {
				StartCoroutine(Warp(
					visitedLocations[copy].position,
					true,
					visitedLocations[copy].sceneName));
			});
		}
	}

	public void AssignButtonNames(List<Text> buttonsText) {
		for (int i = 0; i < visitedLocations.Count; i++) {
			// Assign Button Name Text
			string ndx = (i + 1).ToString();
			buttonsText[i].text = ndx + ") " + visitedLocations[i].name;
		}
	}

	// Set the first and last button’s navigation 
	public void SetButtonNavigation(List<Button> buttons) {
		// Reset all button's navigation to automatic
		Utilities.S.ResetButtonNavigation(buttons);

		// Set button navigation if inventory is less than 10
		if (visitedLocations.Count <= buttons.Count) {
			if (visitedLocations.Count > 1) {
				// Set first button navigation
				Utilities.S.SetButtonNavigation(
					buttons[0],
					buttons[visitedLocations.Count - 1],
					buttons[1]);

				// Set last button navigation
				Utilities.S.SetButtonNavigation(
					buttons[visitedLocations.Count - 1],
					buttons[visitedLocations.Count - 2],
					buttons[0]);
			}
		}
	}

	// "Warps" the player to a new scene and/or position
	public IEnumerator Warp(Vector3 destinationPos,
							bool warpToNewScene,
							string sceneName = "zTown",
							bool camFollows = true,
							Vector3 camWarpPos = default(Vector3)) {
		// If used a warp potion, remove it from the inventory 
		if (Items.S.menu.gameObject.activeInHierarchy) {
			Inventory.S.RemoveItemFromInventory(Items.S.items[23]);
		}
		
		// Remove listeners
		Utilities.S.RemoveListeners(Items.S.menu.itemButtons);
		Utilities.S.RemoveListeners(Spells.S.menu.spellsButtons);
		Utilities.S.RemoveListeners(GameManager.S.gameSubMenu.buttonCS);

		// Activate Black Screen
		ColorScreen.S.ActivateBlackScreen();

		if (!warpToNewScene) {
			// Set PlayerPos to destinationPos
			Player.S.followers.followersGO[0].transform.position = destinationPos;
			Player.S.followers.followersGO[1].transform.position = destinationPos;
		} else {
			// Set PlayerRespawnPos to destinationPos
			Player.S.gameObject.transform.position = destinationPos;
			Player.S.respawnPos = destinationPos;
			Player.S.followers.followersGO[0].transform.position = destinationPos;
			Player.S.followers.followersGO[1].transform.position = destinationPos;
		}

		// Set player move point position
		Player.S.movePoint.position = destinationPos;
		if (Player.S.followers.followersGO[0].activeInHierarchy) {
			Player.S.followers.followerMovePoints[0].transform.position = destinationPos;
		}
		if (Player.S.followers.followersGO[1].activeInHierarchy) {
			Player.S.followers.followerMovePoints[1].transform.position = destinationPos;
		}

		// Clear cached follower properties
		Player.S.followers.movePoints.Clear();
		Player.S.followers.animations.Clear();
		Player.S.followers.facingRights.Clear();

        // Reset sorting orders
        Player.S.followers.partySRends[0].sortingOrder = 2;
        Player.S.followers.partySRends[1].sortingOrder = 1;
        Player.S.followers.partySRends[2].sortingOrder = 0;

		// Camera Settings
		if (camFollows) {
			// Camera Follows Player
			CamManager.S.camMode = eCamMode.followAll;
		} else {
			// Set Freeze Camera Position
			CamManager.S.camMode = eCamMode.freezeCam;
			CamManager.S.transform.position = camWarpPos;
		}

		// Wait for 0.5f seconds
		yield return new WaitForSeconds(0.5f);

		if (!warpToNewScene) {
			// Deactivate Black Screen
			ColorScreen.S.anim.Play("Clear Screen", 0, 0);
			// Unfreeze Player
			Player.S.canMove = true;
		} else {
            // Load Scene
            GameManager.S.LoadLevel(sceneName);
		}
	}
}

public class WarpLocation {
	public string name;
	public string sceneName;
	public Vector3 position;
	public string description;
	public int playerFacingDirection;

	public WarpLocation(string name, string sceneName, Vector3 position, string description, int playerFacingDirection) {
		this.name = name;
		this.sceneName = sceneName;
		this.position = position;
		this.description = description;
		this.playerFacingDirection = playerFacingDirection;
	}
}