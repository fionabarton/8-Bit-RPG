using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour {
	[Header ("Set in Inspector")]
	public bool 	destroyRatherThanSetActive;
	public float 	timeDuration = 1f;

	[Header ("Set Dynamically")]
	private float 	timeDone;

	void OnEnable () {
		timeDone = timeDuration + Time.time;

		StartCoroutine ("FixedUpdateCoroutine");
	}

	public IEnumerator FixedUpdateCoroutine () {	
		if (timeDone <= Time.time) {
			if (destroyRatherThanSetActive) {
				Destroy (gameObject);
			} else {
				gameObject.SetActive (false);
			}
		}

		yield return new WaitForFixedUpdate ();
		StartCoroutine ("FixedUpdateCoroutine");
	}
}
