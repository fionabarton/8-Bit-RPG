﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deactivates any key items in a scene that have already been obtained by Player
/// </summary>
public class KeyItemManager : MonoBehaviour {
	[Header("Set Dynamically")]
	public List<bool>    			isDeactivated = new List<bool> ();

	private Transform				tTransform;

	private static KeyItemManager _S;
	public static KeyItemManager S { get { return _S; } set { _S = value; } }

	void Awake(){
		S = this;
	}

	// Called in RPG.cs
	public void SetObjects () {
		// In the scene that was just loaded, find the parent gameObject holding all key items within the scene
		GameObject tGO = GameObject.Find ("Items");

		if (tGO != null) {
			tTransform = tGO.transform;

			foreach (Transform child in tTransform) {
				ItemTrigger tItem = child.gameObject.GetComponent<ItemTrigger> ();

				if (tItem != null) {
					for (int i = 0; i < isDeactivated.Count; i++) {
						if (tItem.keyItemNdx == i) {
							if (isDeactivated [i]) {
								tItem.gameObject.SetActive (false);
							} 
						}
					}
				}
			}
		}
	}

	// Load/save which items are deactivated //////////////////////////
	///////////////////////////////////////////////////////////////

	// Save which items are deactivated:
	// Convert list of bools into a string of 0's and 1's
	public string GetIsDeactivatedString() {
		return Utilities.S.SaveListOfBoolValues(ref isDeactivated);
	}

	// Load which items are deactivated:
	// Read string of 0's and 1's to set list of bools
	public void DeactivateItemsFromString(string isDeactivatedString) {
		Utilities.S.LoadListOfBoolValues(isDeactivatedString, ref isDeactivated);
	}
}