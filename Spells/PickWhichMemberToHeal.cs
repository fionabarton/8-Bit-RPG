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
			Utilities.S.PositionCursor(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject, 0, 60, 3);

			// Set animation to walk
			PlayerButtons.S.SetSelectedAnim("Walk");

			// Audio: Selection (when a new gameObject is selected)
			Utilities.S.PlayButtonSelectedSFX(ref previousSelectedPlayerGO);

			spellScreen.canUpdate = false;
		}

		if (PauseMessage.S.dialogueFinished) {
			if (Input.GetButtonDown("SNES Y Button")) {
				// Set animation to idle
				PlayerButtons.S.SetSelectedAnim("Idle");

				// Audio: Deny
				AudioManager.S.PlaySFX(eSoundName.deny);

				spellScreen.LoadSpells(spellScreen.playerNdx); // Go Back
			}
		}
	}
}