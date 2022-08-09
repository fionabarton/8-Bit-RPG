using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarpTrigger : MonoBehaviour {
	[Header("Set in Inspector")]
	public bool warpOnContact;

	// New Scene
	public bool warpToNewScene;
	public string sceneName;

	// Player Pos
	public Vector3 playerWarpPos;

	// Camera
	public bool camFollows;
	public Vector3 camWarpPos;

	// Warp on contact
	void OnTriggerEnter2D(Collider2D coll) {
		if (warpOnContact) {
			if (coll.gameObject.CompareTag("PlayerTrigger")) {
				StartCoroutine(WarpManager.S.Warp(playerWarpPos, warpToNewScene, sceneName, camFollows, camWarpPos));
			}
		}
	}

	// Warp on Button Press
	void OnTriggerStay2D(Collider2D coll) {
		if (coll.gameObject.CompareTag("Player")) {
			//Blob.S.rigid.sleepMode = RigidbodySleepMode2D.NeverSleep;

			if (Input.GetButtonDown("SNES B Button")) {
				StartCoroutine(WarpManager.S.Warp(playerWarpPos, warpToNewScene, sceneName, camFollows, camWarpPos));
			}
		}
	}

	void OnTriggerExit2D(Collider2D coll) {
		if (coll.gameObject.CompareTag("Player")) {
			//Player.S.rigid.sleepMode = RigidbodySleepMode2D.StartAwake;
		}
	}
}