using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpellScreen Mode/Step 4: PickWhichMemberToHeal
/// - Select which party member to use spell on
/// </summary>
public class PickWhichMemberToHeal : MonoBehaviour {
	[Header("Set Dynamically")]
	// Ensures audio is only played once when button is selected
	public GameObject previousSelectedPlayerGO;

	public void Loop(SpellMenu spellScreen) {
		if (spellScreen.canUpdate) {
			Utilities.S.PositionCursor(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject, 0, 110, 3);

			// Audio: Selection (when a new gameObject is selected)
			Utilities.S.PlayButtonSelectedSFX(ref previousSelectedPlayerGO);

			spellScreen.canUpdate = false;
		}

		if (PauseMessage.S.dialogueFinished) {
			if (Input.GetButtonDown("SNES Y Button")) {
				// Audio: Deny
				AudioManager.S.PlaySFX(eSoundName.deny);

				spellScreen.LoadSpells(spellScreen.playerNdx); // Go Back

				// Play the previously played animation clip of the selected party member
				PauseMenu.S.SetPreviousSelectedPlayerAnimAndColor("Walk", spellScreen.playerNdx);
			}
		}
	}
}