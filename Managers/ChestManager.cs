using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// "Closes" any chests in a scene that have already been opened
/// </summary>
public class ChestManager : MonoBehaviour {
	[Header("Set in Inspector")]
	public List<bool>    			isOpen = new List<bool> ();

	[Header("Set Dynamically")]
	private static ChestManager _S;
	public static ChestManager S { get { return _S; } set { _S = value; } }

	void Awake(){
		S = this;
	}

	// Called in RPG.cs
	public void SetObjects () {
		// In the scene that was just loaded, find the parent gameObject holding all chests within the scene
		GameObject chestsGO = GameObject.Find ("Chests");

		if (chestsGO != null) {
			foreach (Transform child in chestsGO.transform) {
				ChestTrigger tChest = child.gameObject.GetComponent<ChestTrigger> ();

				if (tChest != null) {
					for (int i = 0; i < isOpen.Count; i++) {
						if (tChest.ndx == i) {
							if (isOpen [i]) {			
								tChest.sRend.sprite = tChest.openChest;

								tChest.chestIsOpen = true;
							} 
						}
					}
				}
			}
		}
	}

	// Load/save which chests are opened //////////////////////////
	///////////////////////////////////////////////////////////////

	// Save which chests are open:
	// Convert list of bools into a string of 0's and 1's
	public string GetIsOpenString() {
		// Initialize string to store 0's and 1's
		string isOpenString = "";

		// Loop over all chests
		for (int i = 0; i < isOpen.Count; i++) {
			// If chest is closed/open, add 0/1
			if (isOpen[i]) {
				isOpenString += "1";
			} else {
				isOpenString += "0";
			}
		}

		// Return string of 0's and 1's
		return isOpenString;
	}

	// Load which chests are open:
	// Read string of 0's and 1's to set list of bools
	public void OpenChestsFromString(string isOpenString) {
		// Loop over string of 0's and 1's
		for (int i = 0; i < isOpenString.Length; i++) {
			// If char is 0/1, close/open chest
			if (isOpenString[i].ToString() == "0") {
				isOpen[i] = false;
			} else {
				isOpen[i] = true;
			}
		}
	}
}