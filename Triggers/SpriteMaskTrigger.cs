using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// On trigger, (de)activates a sprite mask which hides/reveals this party member's bottom half
public class SpriteMaskTrigger : MonoBehaviour {
    [Header("Set in Inspector")]
    [SerializeField] GameObject         spriteMaskGameObject;

    [SerializeField] List<GameObject>   borderSprites; // (blue, brown, green)

    void DeactivateAllSprites() {
        // Deactivate all border sprites (blue, brown, green)
        for (int i = 0; i < borderSprites.Count; i++) {
            borderSprites[i].SetActive(false);
        }

        // Deactivate sprite mask gameObject
        spriteMaskGameObject.SetActive(false);
    }

    void ActivateSprites(int ndx) {
        // Deactivate sprites
        DeactivateAllSprites();

        // Activate specific border sprite (blue, brown, green)
        borderSprites[ndx].SetActive(true);

        // Activate sprite mask gameObject
        spriteMaskGameObject.SetActive(true);
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
            if (coll.gameObject.CompareTag("SubmergedBlue")) {
                DeactivateAllSprites();
            } else if (coll.gameObject.CompareTag("SubmergedBrown")) {
                DeactivateAllSprites();
            } else if (coll.gameObject.CompareTag("SubmergedGreen")) {
                DeactivateAllSprites();
            }
        }
    }
}