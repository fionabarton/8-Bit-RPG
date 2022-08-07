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
								tDoor.doorMode = eDoorMode.open;
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
}