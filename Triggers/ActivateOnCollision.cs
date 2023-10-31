using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnCollision : MonoBehaviour {
	[Header("Set in Inspector")]
	public string tagToCompare = "PlayerTrigger";

	protected virtual void OnTriggerEnter2D(Collider2D coll) {
		if (!GameManager.S.paused) {
			if (coll.gameObject.CompareTag(tagToCompare)) {
				// Update Delgate
				UpdateManager.updateDelegate += Loop;

				Action();
			}
		}
	}

	protected virtual void OnTriggerExit2D(Collider2D coll) {
		if (coll.gameObject.CompareTag(tagToCompare)) {
			// Update Delgate
			UpdateManager.updateDelegate -= Loop;
		}
	}

	public void Loop() {

	}

	protected virtual void Action() {

	}
}