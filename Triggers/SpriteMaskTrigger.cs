using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// On trigger, (de)activates a sprite mask which hides/reveals this party member's bottom half
public class SpriteMaskTrigger : MonoBehaviour {
    [Header("Set in Inspector")]
    [SerializeField] GameObject         spriteMaskGO;

    [SerializeField] SpriteRenderer     maskBorderSRend;
    [SerializeField] List<Sprite>       maskBorderSprites; // (blue, brown, green)

    void ActivateSprites(int ndx) {
        // Set specific border sprite (blue, brown, green)
        maskBorderSRend.sprite = maskBorderSprites[ndx];

        // Activate mask border gameObject
        maskBorderSRend.gameObject.SetActive(true);

        // Activate sprite mask gameObject
        spriteMaskGO.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (!GameManager.S.paused) {
            if (coll.gameObject.CompareTag("SubmergedBlue")) {
                ActivateSprites(0);
            } else if (coll.gameObject.CompareTag("SubmergedBrown")) {
                ActivateSprites(1);
            } else if (coll.gameObject.CompareTag("SubmergedGreen")) {
                ActivateSprites(2);
            }
        }
    }

    void OnTriggerExit2D(Collider2D coll) {
        if (!GameManager.S.paused) {
            if (coll.gameObject.CompareTag("SubmergedBlue") || coll.gameObject.CompareTag("SubmergedBrown") || coll.gameObject.CompareTag("SubmergedGreen")) {
                // Deactivate mask border gameObject
                maskBorderSRend.gameObject.SetActive(false);

                // Deactivate sprite mask gameObject
                spriteMaskGO.SetActive(false);
            }
        }
    }
}