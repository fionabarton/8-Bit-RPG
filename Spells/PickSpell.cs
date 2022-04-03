using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpellScreen Mode/Step 2: PickSpell
/// - Select which spell to use
/// </summary>
public class PickSpell : MonoBehaviour {
	public void Loop(SpellMenu spellScreen) {
		if (spellScreen.canUpdate) {
			spellScreen.DisplaySpellsDescriptions(spellScreen.playerNdx);
			spellScreen.canUpdate = false;
		}

		if (PauseMessage.S.dialogueFinished) {
			if (Input.GetButtonDown("SNES Y Button")) {
				// Go Back
				spellScreen.pickWhichSpellsToDisplay.Setup(spellScreen);

				// Audio: Deny
				AudioManager.S.PlaySFX(eSoundName.deny);
			}
		}
	}
}