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
				// Load spells of selected party member
				spellScreen.LoadSpells(spellScreen.playerNdx);

				// Play the previously played animation clip of the selected party member
				PauseMenu.S.SetPreviousSelectedPlayerAnimAndColor("Walk", spellScreen.playerNdx);
			}
		}
	}
}