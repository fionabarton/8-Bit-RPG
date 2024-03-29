﻿using System.Collections;
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
								tDoor.doorMode = eDoorMode.open;
								// Change Sprite
								tDoor.sRend.sprite = tDoor.openDoorSprite;
								// Disable colliders
								tDoor.solidColl.enabled = false;
								tDoor.triggerColl.enabled = false;
								// Deactivate trigger
								tDoor.triggerHasBeenDeactivated = true;
							} 
						}
					}
				}
			}
		}
	}

	// Load/save which doors are unlocked /////////////////////////
	///////////////////////////////////////////////////////////////

	// Save which doors are unlocked:
	// Convert list of bools into a string of 0's and 1's
	public string GetIsUnlockedString() {
		return Utilities.S.SaveListOfBoolValues(ref isUnlocked);
	}

	// Load which doors are unlocked:
	// Read string of 0's and 1's to set list of bools
	public void UnlockDoorsFromString(string isUnlockedString) {
		Utilities.S.LoadListOfBoolValues(isUnlockedString, ref isUnlocked);
	}
}