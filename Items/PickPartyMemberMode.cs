using UnityEngine;

/// <summary>
/// ItemScreen Mode/Step 2: PickPartyMember
/// - Select which party member to use an item on
/// </summary>
public class PickPartyMemberMode : MonoBehaviour {
	[Header("Set Dynamically")]
	// Ensures audio is only played once when button is selected
	public GameObject previousSelectedPlayerGO;

	public void Loop(ItemMenu itemScreen) {
		if (itemScreen.canUpdate) {
			Utilities.S.PositionCursor(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject, 0, 110, 3);

			// Audio: Selection (when a new gameObject is selected)
			Utilities.S.PlayButtonSelectedSFX(ref previousSelectedPlayerGO);

			itemScreen.canUpdate = false;
		}

        if (PauseMessage.S.dialogueFinished) {
            if (Input.GetButtonDown("SNES Y Button")) {
				// Audio: Deny
				AudioManager.S.PlaySFX(eSoundName.deny);

				// Go back to PickItem mode
				itemScreen.pickItemMode.Setup(Items.S.menu);
			}
        }
    }
}