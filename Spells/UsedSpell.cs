using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpellScreen Mode/Step 5: UsedSpell
/// - Used a spell
/// </summary
public class UsedSpell : MonoBehaviour {
	public void Loop(SpellMenu spellScreen) {
		if (PauseMessage.S.dialogueFinished) {
			if (Input.GetButtonDown("SNES B Button")) {
				// Set animation to idle
				PlayerButtons.S.SetSelectedAnim("Idle");
				spellScreen.LoadSpells(spellScreen.playerNdx);
			}
		}
	}
}