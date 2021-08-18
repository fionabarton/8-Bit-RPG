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
	// Singleton
	private static ChestManager _S;
	public static ChestManager S { get { return _S; } set { _S = value; } }

	private Transform				tTransform;

	void Awake(){
		// Singleton
		S = this;
	}

	// Called in RPG.cs
	public void SetObjects () {
		// In the scene that was just loaded, find the parent gameObject holding all chests within the scene
		GameObject chestsGO = GameObject.Find ("Chests");

		if (chestsGO != null) {
			//tTransform = chestsGO.transform;

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
}