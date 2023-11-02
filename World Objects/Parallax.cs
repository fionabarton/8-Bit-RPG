using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Applies parallax scrolling (background image moves past the cam more slowly than foreground images) to this game object
public class Parallax : MonoBehaviour {
	[Header("Set in Inspector")]
	public eParallax	mode;
	public float		speedModifier = -0.25f;

	[Header ("Set Dynamically")]
	private Vector3 	pos;

	private Vector3		currentPlayerPos;
	private Vector3     previousFramePlayerPos;

	void OnBecameVisible() {
		StartCoroutine("FixedUpdateCoroutine");
	}
	void OnBecameInvisible() {
		StopCoroutine("FixedUpdateCoroutine");
	}

	public IEnumerator FixedUpdateCoroutine () {
		if (!GameManager.S.paused) { 
			// Get this game object's position
			pos = transform.position;

			switch (mode) {
                case eParallax.autoScroll:
                    pos.x += speedModifier * Time.fixedDeltaTime;
                    break;

                case eParallax.scrollWithPlayer:
					// Get current player position
					currentPlayerPos = Player.S.gameObject.transform.position;

					// If player is moving horizontally, apply parallax scrolling
					if(currentPlayerPos != previousFramePlayerPos) {
						if (Input.GetAxisRaw("Horizontal") > 0) {
                            if (!Player.S.hasRunningShoes) {
								pos.x += speedModifier * Time.fixedDeltaTime;
							} else {
								pos.x += (speedModifier * 2) * Time.fixedDeltaTime;
							}	
						} else if (Input.GetAxisRaw("Horizontal") < 0) {
							if (!Player.S.hasRunningShoes) {
								pos.x -= speedModifier * Time.fixedDeltaTime;
							} else {
								pos.x -= (speedModifier * 2) * Time.fixedDeltaTime;
							}
						}
					}
 
					// Cache player position for next frame
					previousFramePlayerPos = Player.S.gameObject.transform.position;
					break;
            }

			// Set this game object's position
			transform.position = pos;

			yield return new WaitForFixedUpdate ();
			StartCoroutine ("FixedUpdateCoroutine");
		}
	}
}