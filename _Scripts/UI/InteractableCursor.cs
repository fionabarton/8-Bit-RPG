using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to RPGMainCamera; used to get InteractableCursorHolder gameObject
/// </summary>
public class InteractableCursor : MonoBehaviour{
	[Header("Set Dynamically")]
	// Singleton
	private static InteractableCursor _S;
	public static InteractableCursor S { get { return _S; } set { _S = value; } }
	
	[Header("Set in Inspector")]
	// Cursor GameObject
	public GameObject cursorGO;

	void Awake() {
		S = this;
	}

	// Activate cursor gameObject
	public void Activate (GameObject newParentGO) {
		// Set position
		Utilities.S.SetPosition(cursorGO, newParentGO.transform.position.x, newParentGO.transform.position.y + 0.5f);
		// Set parent
		cursorGO.transform.SetParent(newParentGO.transform);
		// Activate gameObject
		cursorGO.SetActive(true);
	}

	// Dectivate cursor gameObject 
	public void Deactivate() {
		// Set parent
		cursorGO.transform.SetParent(CamManager.S.transform);
		// Deactivate gameObject
		cursorGO.SetActive(false);
	}
}