using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// "Unlocks" any doors in a scene that have already been opened
/// </summary>
public class DoorManager : MonoBehaviour {
	[Header("Set in Inspector")]
	public List<bool>    			isUnlocked = new List<bool> ();

	[Header("Set Dynamically")]
	private Transform tTransform;

	private static DoorManager _S;
	public static DoorManager S { get { return _S; } set { _S = value; } }

	void Awake(){
		S = this;
	}

	public void SetObjects () {
		// Find this scene's "Doors" game object holder
		GameObject tGO = GameObject.Find ("Doors");

		if (tGO != null) {
			tTransform = tGO.transform;

			// For each door, check and set whether its locked 
			foreach (Transform child in tTransform) {
				DoorTrigger tDoor = child.gameObject.GetComponent<DoorTrigger> ();

				if (tDoor != null) {
					for (int i = 0; i < isUnlocked.Count; i++) {
						if (tDoor.ndx == i) {
							if (isUnlocked [i]) {
								// Switch eDoorMode
								tDoor.doorMode = eDoorMode.closed;
								// Change Sprite
								tDoor.sRend.sprite = tDoor.closedDoorSprite;
								// Disable Collider
								tDoor.solidColl.enabled = false;
							} 
						}
					}
				}
			}
		}
	}

	// Load/save which doors are unlocked/locked //////////////////
	///////////////////////////////////////////////////////////////

	// Save which doors are unlocked/locked:
	// Convert list of bools into a string of 0's and 1's
	public string GetIsUnlockedString() {
		// Initialize string to store 0's and 1's
		string isUnlockedString = "";

		// Loop over all doors
		for (int i = 0; i < isUnlocked.Count; i++) {
			// If door is unlocked/locked, add 0/1
			if (isUnlocked[i]) {
				isUnlockedString += "1";
			} else {
				isUnlockedString += "0";
			}
		}

		// Return string of 0's and 1's
		return isUnlockedString;
	}

	// Load which doors are unlocked/locked:
	// Read string of 0's and 1's to set list of bools
	public void UnlockDoorsFromString(string isUnlockedString) {
		// Loop over string of 0's and 1's
		for (int i = 0; i < isUnlockedString.Length; i++) {
			// If char is 0/1, lock/unlock door
			if (isUnlockedString[i].ToString() == "0") {
				isUnlocked[i] = false;
			} else {
				isUnlocked[i] = true;
			}
		}
	}
}