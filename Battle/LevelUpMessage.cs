using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpMessage : MonoBehaviour {
	[Header("Set in Inspector")]
	public GameObject levelUpMessageGO;

	public Text playerNameAndLevel;
	public Text amountIncreased;
	public Text statValues;
	public Text newSpellMessage;

	void Start() {
		levelUpMessageGO.SetActive(false);
	}

	public void Initialize(int ndx) {
		// Activate level up message gameObject
		levelUpMessageGO.SetActive(true);

		// Display player's name and new level
		playerNameAndLevel.text = Party.S.stats[ndx].name + " Lvl " + Party.S.stats[ndx].LVL + "!";

		string s = "";

		// Display amount stats were increased
		s += Party.S.GetHPUpgrade(ndx) > 0 ? "+" + Party.S.GetHPUpgrade(ndx).ToString() + "\n" : s += "0" + "\n";
		s += Party.S.GetMPUpgrade(ndx) > 0 ? "+" + Party.S.GetMPUpgrade(ndx).ToString() + "\n" : s += "0" + "\n";
		s += Party.S.GetSTRUpgrade(ndx) > 0 ? "+" + Party.S.GetSTRUpgrade(ndx).ToString() + "\n" : s += "0" + "\n";
		s += Party.S.GetDEFUpgrade(ndx) > 0 ? "+" + Party.S.GetDEFUpgrade(ndx).ToString() + "\n" : s += "0" + "\n";
		s += Party.S.GetWISUpgrade(ndx) > 0 ? "+" + Party.S.GetWISUpgrade(ndx).ToString() + "\n" : s += "0" + "\n";
		s += Party.S.GetAGIUpgrade(ndx) > 0 ? "+" + Party.S.GetAGIUpgrade(ndx).ToString() : s += "0";
		amountIncreased.text = s;

		// Display previous level stats compared to new level
		int lvlNdx = Party.S.stats[ndx].LVL;
		int prevLvlNdx = Party.S.stats[ndx].previousLVL;
		s = Party.S.GetHPUpgrade(ndx) > 0 ? Party.S.GetHP(ndx, prevLvlNdx).ToString() + " > " +
			Party.S.GetHP(ndx, lvlNdx).ToString() + "\n" : s += Party.S.GetHP(ndx, lvlNdx).ToString() + "\n";
		s += Party.S.GetMPUpgrade(ndx) > 0 ? Party.S.GetMP(ndx, prevLvlNdx).ToString() + " > " +
			Party.S.GetMP(ndx, lvlNdx).ToString() + "\n" : s += Party.S.GetMP(ndx, lvlNdx).ToString() + "\n";
		s += Party.S.GetSTRUpgrade(ndx) > 0 ? Party.S.GetSTR(ndx, prevLvlNdx).ToString() + " > " +
			Party.S.GetSTR(ndx, lvlNdx).ToString() + "\n" : s += Party.S.GetSTR(ndx, lvlNdx).ToString() + "\n";
		s += Party.S.GetDEFUpgrade(ndx) > 0 ? Party.S.GetDEF(ndx, prevLvlNdx).ToString() + " > " +
			Party.S.GetDEF(ndx, lvlNdx).ToString() + "\n" : s += Party.S.GetDEF(ndx, lvlNdx).ToString() + "\n";
		s += Party.S.GetWISUpgrade(ndx) > 0 ? Party.S.GetWIS(ndx, prevLvlNdx).ToString() + " > " +
			Party.S.GetWIS(ndx, lvlNdx).ToString() + "\n" : s += Party.S.GetWIS(ndx, lvlNdx).ToString() + "\n";
		s += Party.S.GetAGIUpgrade(ndx) > 0 ? Party.S.GetAGI(ndx, prevLvlNdx).ToString() + " > " +
			Party.S.GetAGI(ndx, lvlNdx).ToString() : s += Party.S.GetAGI(ndx, lvlNdx).ToString();
		statValues.text = s;

		// Display what new spell was learned
		if (Party.S.GetSpellNdxUpgrade(ndx) > 0) {
			s = "Learned  a new spell:\n" + Party.S.stats[ndx].spells[Party.S.stats[ndx].spellNdx].name + "!";
		} else {
			s = "Level increased\n" + "from " + Party.S.stats[ndx].previousLVL + " to " + Party.S.stats[ndx].LVL + "!";
		}
		newSpellMessage.text = s;
	}
}