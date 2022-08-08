using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for swaping open/closed door sprites on collision
/// </summary>
public class SpriteSwapTrigger : MonoBehaviour {
	[Header("Set in Inspector")]
	public List<Sprite> sprites = new List<Sprite>();

	public bool swapOnlyOnFirstCollision = true;

	public eSoundName soundName;

	public SpriteRenderer sRend;

	void OnTriggerEnter2D(Collider2D coll) {
		if (swapOnlyOnFirstCollision) {
			if (coll.gameObject.CompareTag("PlayerTrigger")) {
				sRend.sprite = sprites[0];

				AudioManager.S.PlaySFX(soundName);

				swapOnlyOnFirstCollision = false;
			}
		}
	}
}