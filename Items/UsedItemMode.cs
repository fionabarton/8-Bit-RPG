using UnityEngine;

/// <summary>
/// ItemScreen Mode/Step 3: UsedItemMode
/// - Consumed an item
/// </summary>
public class UsedItemMode : MonoBehaviour {
	public void Loop(ItemMenu itemScreen) {
        if (PauseMessage.S.dialogueFinished) {
            if (Input.GetButtonDown("SNES B Button")) {
				// Set party animations to idle
				PauseMenu.S.SetSelectedMemberAnim("Idle");

				// Go back to PickItem mode
				itemScreen.pickItemMode.Setup(Items.S.menu);
			}
        }
    }
}