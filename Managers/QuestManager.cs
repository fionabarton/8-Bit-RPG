using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {
	[Header("Set Dynamically")]
	public List<bool> isCompleted = new List<bool>();

	public Quest[] quests;

	private static QuestManager _S;
	public static QuestManager S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;
	}

    void Start() {
		InitializeQuests();
	}

    public void InitializeQuests() {
		// Initialize array of quests
		quests = new Quest[3];

		quests[0] = new Quest(000, "Go defeat some dude.", Items.S.items[23], 50, 000);
		quests[1] = new Quest(001, "Go retrieve/deliver something.", Items.S.items[23], 50, 000);
		quests[2] = new Quest(002, "Go do something within the time limit.", Items.S.items[23], 50, 000);
	}

	// Load/save which quests are completed ///////////////////////
	///////////////////////////////////////////////////////////////

	// Save which quests are completed:
	// Convert list of bools into a string of 0's and 1's
	public string GetIsCompletedString() {
		// Set values of intermediary list
		for (int i = 0; i < quests.Length; i++) {
			isCompleted[i] = quests[i].isCompleted;
		}

		// Return string of 0's and 1's
		return Utilities.S.SaveListOfBoolValues(ref isCompleted);
	}

	// Load which quests are completed:
	// Read string of 0's and 1's to set list of bools
	public void GetIsCompletedFromString(string isCompletedString) {
		// Set values of intermediary list
		Utilities.S.LoadListOfBoolValues(isCompletedString, ref isCompleted);

		// Set values of quests.isCompleted
		for (int i = 0; i < quests.Length; i++) {
			quests[i].isCompleted = isCompleted[i];
		}
	}
}

public class Quest {
	public int id;
	public string description;
	public Item reward;
	public int enemyId;
	public int timeLimit;
	public bool isCompleted;

	public Quest(int questId, string questDescription, Item questReward, int questEnemyId, int questTimeLimit = -1,
		bool questIsCompleted = false) {
		id = questId;
		description = questDescription;
		reward = questReward;
		enemyId = questEnemyId;
		timeLimit = questTimeLimit;
		isCompleted = questIsCompleted;
	}
}