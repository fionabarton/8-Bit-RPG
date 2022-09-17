using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtain : MonoBehaviour {
	[Header("Set in Inspector")]
	public Animator anim;

	[Header("Set Dynamically")]
	public bool isOpen;
	
	private static Curtain _S;
	public static Curtain S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;
	}

	public void Open() {
		isOpen = true;

		if (Random.value > 0.5f) {
			anim.Play("Horizontal_Open");
		} else {
			anim.Play("Vertical_Open");
		}
	}

	public void Close() {
		isOpen = false;

		if (Random.value > 0.5f) {
			anim.Play("Horizontal_Close");
		} else {
			anim.Play("Vertical_Close");
		}
	}
}