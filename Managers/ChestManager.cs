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
		return Utilities.S.SaveListOfBoolValues(ref isOpen);
	}

	// Load which chests are open:
	// Read string of 0's and 1's to set list of bools
	public void OpenChestsFromString(string isUnlockedString) {
		Utilities.S.LoadListOfBoolValues(isUnlockedString, ref isOpen);
	}
}