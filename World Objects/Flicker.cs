using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// If a party member is poisoned, have their sprite "flicker" with each step.
public class Flicker : MonoBehaviour {
	[Header("Set in Inspector")]
	public SpriteRenderer sRend;

	[Header("Set Dynamically")]
	public bool isInvincible; 
	public bool isFlashing;
	private float timeToFlash;
	private float flashRate;
	private float timeToEndInvincibility;

	public void FixedLoop() {
		if (isFlashing) {
			if (Time.time >= timeToEndInvincibility) {
				EndInvincibility();
			} else {
				if (Time.time >= timeToFlash) {
					// "Flash" the sprite by enabling its SpriteRenderer
					sRend.enabled = !sRend.enabled;

					// Increase the rate at which the sprite will flash
					flashRate -= 0.01f;

					// Reset the timer
					timeToFlash = Time.time + flashRate;
				}
			}
		}
	}

	public void StartInvincibility(float duration = 3f, float _flashRate = 0.25f, bool _isInvincible = true) {
		isInvincible = _isInvincible;
		isFlashing = true;

		flashRate = 0;
		timeToEndInvincibility = 0;

		// Set timers
		flashRate = _flashRate;
		timeToFlash = Time.time + _flashRate;
		timeToEndInvincibility = Time.time + duration;

		// Add FixedLoop() to UpdateManager
		UpdateManager.fixedUpdateDelegate += FixedLoop;
	}

	public void EndInvincibility() {
		isInvincible = false;
		isFlashing = false;

		// Enable SpriteRenderer
		sRend.enabled = true;

		// Remove FixedLoop() from UpdateManager
		UpdateManager.fixedUpdateDelegate -= FixedLoop;
	}
}