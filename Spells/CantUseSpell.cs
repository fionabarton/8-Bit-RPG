using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpellScreen Mode/Step 6: CantUseSpell
/// - Can't use this spell
/// </summary
public class CantUseSpell : MonoBehaviour {
	public void Loop(SpellMenu spellScreen) {
		if (PauseMessage.S.dialogueFinished) {
			if (Input.GetButtonDown("SNES B Button")) {
				spellScreen.ScreenOffPlayerTurn();
			}
		}
	}
}