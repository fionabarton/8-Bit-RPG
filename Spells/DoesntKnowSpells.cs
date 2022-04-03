using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpellScreen Mode/Step 3: DoesntKnowSpells
/// - This party member doesn't know any spells
/// </summary>
public class DoesntKnowSpells : MonoBehaviour {
	public void Loop(SpellMenu spellScreen) {
		if (PauseMessage.S.dialogueFinished) {
			if (Input.GetButtonDown("SNES B Button")) {
				spellScreen.pickWhichSpellsToDisplay.Setup(spellScreen);
			}
		}
	}
}