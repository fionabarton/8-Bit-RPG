using UnityEngine;

public class QuestReaction : MonoBehaviour {
	[Header ("Set in Inspector")]
	// Indexes of which quests to react to
	public int questIsCompletedNdx = -1;

	// Actions to perform
	public eQuestAction questIsCompletedAction;

	// List of actions to perform
	
	// Deactivate GameObject if associated Quest has been Completed
	void OnEnable() {
		// If associated quest has been completed...
		if (questIsCompletedNdx >= 0) {
			if (QuestManager.S.quests[questIsCompletedNdx].isCompleted) {
				switch (questIsCompletedAction) {
					case eQuestAction.deactivateGo:
						gameObject.SetActive(false);
						break;
					case eQuestAction.activateGo:
						gameObject.SetActive(true);
						break;
				}
            } else {
				switch (questIsCompletedAction) {
					case eQuestAction.deactivateGo:
						gameObject.SetActive(true);
						break;
					case eQuestAction.activateGo:
						gameObject.SetActive(false);
						break;
				}
			}
		}
	}
}