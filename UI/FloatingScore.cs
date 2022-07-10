using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingScore : MonoBehaviour {
	[Header ("Set in Inspector")]
	public float 	speed = 1f;

	[Header ("Set Dynamically")]
	private bool	canScale;

	void OnEnable(){
		StartCoroutine("FixedUpdateCoroutine");
	}

	void OnDisable(){
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;
	}

	public IEnumerator FixedUpdateCoroutine () {
		Vector3 tPos = transform.localPosition;
		tPos.y += speed * Time.fixedDeltaTime;
		transform.localPosition = tPos;

		Invoke ("ScaleUp", 1.0f);
		if (canScale) {
			transform.localScale += (Vector3.one * Time.fixedDeltaTime)/5;
		}

		yield return new WaitForFixedUpdate ();
		StartCoroutine("FixedUpdateCoroutine");
	}

	void ScaleUp() {
		canScale = true;
	}
}
