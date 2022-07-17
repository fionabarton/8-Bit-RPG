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
				// Activate the animation and text color of the previously selected party member
				PauseMenu.S.SetPreviousSelectedPlayerAnimAndColor(spellScreen.previousSelectedPlayerGO);

				// Load spells of selected party member
				spellScreen.LoadSpells(spellScreen.playerNdx);
			}
		}
	}
}