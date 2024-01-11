using System.Collections.Generic;
using UnityEngine;

// AI, Status Effects (Poison, Blind, Confuse, etc.)

/// <summary>
/// Stores the party's stats
/// </summary>
public class Party : MonoBehaviour {
	[Header("Set Dynamically")]
	public List<PartyStats> stats = new List<PartyStats>();

	// Amount of members in the party
	public int partyNdx;

	// Amount of gold
	public int gold;

	private static Party _S;
	public static Party S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;

		//// Player 1
		//stats.Add(new PartyStats("Blob", 40, 40, 40, 6, 6, 6,
		//    2, 2, 2, 2, 1, 1, 1, 1,
		//    0, 1, 12,
		//    new List<Spell> { Spells.S.spells[1], Spells.S.spells[0], Spells.S.spells[2], Spells.S.spells[4], Spells.S.spells[5], Spells.S.spells[3], Spells.S.spells[6], Spells.S.spells[7], Spells.S.spells[8], Spells.S.spells[9], Spells.S.spells[10], Spells.S.spells[11] },
		//    new List<bool>(new bool[30]),
		//    new List<int> { 0, 0, 7, 23, 47, 110, 220, 450, 800, 1300, 2000 },
		//    false, 0)
		//);
		//// Player 2
		//stats.Add(new PartyStats("Bill", 32, 32, 32, 15, 15, 15,
		//    1, 1, 1, 1, 2, 2, 2, 2,
		//    0, 1, 12,
		//    new List<Spell> { Spells.S.spells[3], Spells.S.spells[1], Spells.S.spells[0], Spells.S.spells[4], Spells.S.spells[5], Spells.S.spells[2], Spells.S.spells[6], Spells.S.spells[7], Spells.S.spells[8], Spells.S.spells[9], Spells.S.spells[10], Spells.S.spells[11] },
		//    new List<bool>(new bool[30]),
		//    new List<int> { 0, 0, 9, 23, 55, 110, 250, 450, 850, 1300, 2100 },
		//    false, 0)
		//);
		//// Player 3
		//stats.Add(new PartyStats("Fake Bill", 25, 25, 25, 10, 10, 10,
		//    1, 1, 1, 1, 2, 2, 2, 2,
		//    0, 1, 12,
		//    new List<Spell> { Spells.S.spells[3], Spells.S.spells[4], Spells.S.spells[0], Spells.S.spells[2], Spells.S.spells[1], Spells.S.spells[5], Spells.S.spells[6], Spells.S.spells[7], Spells.S.spells[8], Spells.S.spells[9] },
		//    new List<bool>(new bool[30]),
		//    new List<int> { 0, 0, 9, 23, 55, 110, 250, 450, 850, 1300, 2100 },
		//    false, 0)
		//);
	}

	public void Start() {
		// Player 1
		stats.Add(new PartyStats("Blob", 40, 40, 40, 6, 6, 6,
			2, 2, 2, 2, 1, 1, 1, 1,
			0, 1, 13,
			new List<Spell> { Spells.S.spells[1], Spells.S.spells[0], Spells.S.spells[2], Spells.S.spells[4], Spells.S.spells[5], Spells.S.spells[3], Spells.S.spells[6], Spells.S.spells[7], Spells.S.spells[8], Spells.S.spells[9], Spells.S.spells[10], Spells.S.spells[11], Spells.S.spells[12] },
			new List<bool>(new bool[30]),
			new List<int> { 0, 0, 7, 23, 47, 110, 220, 450, 800, 1300, 2000 },
			false, 0, 0)
		);
		// Player 2
		stats.Add(new PartyStats("Girl", 32, 32, 32, 15, 15, 15,
			1, 1, 1, 1, 2, 2, 2, 2,
			0, 1, 13,
			new List<Spell> { Spells.S.spells[3], Spells.S.spells[1], Spells.S.spells[0], Spells.S.spells[4], Spells.S.spells[5], Spells.S.spells[2], Spells.S.spells[6], Spells.S.spells[7], Spells.S.spells[8], Spells.S.spells[9], Spells.S.spells[10], Spells.S.spells[11], Spells.S.spells[12] },
			new List<bool>(new bool[30]),
			new List<int> { 0, 0, 9, 23, 55, 110, 250, 450, 850, 1300, 2100 },
			false, 0, 1)
		);
		// Player 3
		stats.Add(new PartyStats("Boy", 25, 25, 25, 10, 10, 10,
			1, 1, 1, 1, 2, 2, 2, 2,
			0, 1, 10,
			new List<Spell> { Spells.S.spells[3], Spells.S.spells[4], Spells.S.spells[0], Spells.S.spells[2], Spells.S.spells[1], Spells.S.spells[5], Spells.S.spells[6], Spells.S.spells[7], Spells.S.spells[8], Spells.S.spells[9] },
			new List<bool>(new bool[30]),
			new List<int> { 0, 0, 9, 23, 55, 110, 250, 450, 850, 1300, 2100 },
			false, 0, 2)
		);

		EquipMenu.S.SetInitialEquipment();
	}

	// HP
	public int GetHP(int playerNdx, int LVL) {
		if (playerNdx == 0) {
			return ((10) * (3 + LVL)); // Blob: Lvl 1 = 40
		} else {
			return ((8) * (3 + LVL)); // Chani: Lvl 1 = 32
		}
	}
	public void SetHP(int playerNdx) {
		stats[playerNdx].HP = GetHP(playerNdx, stats[playerNdx].LVL);
		stats[playerNdx].maxHP = stats[playerNdx].HP;
		stats[playerNdx].baseMaxHP = stats[playerNdx].HP;
	}
	public int GetHPUpgrade(int playerNdx) {
		return GetHP(playerNdx, stats[playerNdx].LVL) - GetHP(playerNdx, stats[playerNdx].previousLVL);
	}
	// MP
	public int GetMP(int playerNdx, int LVL) {
		if (playerNdx == 0) {
			return (6 * LVL); // Blob: Lvl 1 = 6
		} else {
			return ((9 * LVL) + 6); // Chani: Lvl 1 = 15
		}
	}
	public void SetMP(int playerNdx) {
		stats[playerNdx].MP = GetMP(playerNdx, stats[playerNdx].LVL);
		stats[playerNdx].maxMP = stats[playerNdx].MP;
		stats[playerNdx].baseMaxMP = stats[playerNdx].MP;
	}
	public int GetMPUpgrade(int playerNdx) {
		return GetMP(playerNdx, stats[playerNdx].LVL) - GetMP(playerNdx, stats[playerNdx].previousLVL);
	}
	// STR
	public int GetSTR(int playerNdx, int LVL) {
		if (playerNdx == 0) {
			return (int)(2 * LVL); // Blob: Lvl 1 = 2
		} else {
			return (int)(1.5f * LVL); // Chani: Lvl 1 = 1
		}
	}
	public void SetSTR(int playerNdx) {
		stats[playerNdx].STR = GetSTR(playerNdx, stats[playerNdx].LVL);
		stats[playerNdx].baseSTR = stats[playerNdx].STR;
	}
	public int GetSTRUpgrade(int playerNdx) {
		return GetSTR(playerNdx, stats[playerNdx].LVL) - GetSTR(playerNdx, stats[playerNdx].previousLVL);
	}

	// DEF
	public int GetDEF(int playerNdx, int LVL) {
		if (playerNdx == 0) {
			return (int)(2 * LVL); // Blob: Lvl 1 = 2
		} else {
			return (int)(1.5f * LVL); // Chani: Lvl 1 = 1
		}
	}
	public void SetDEF(int playerNdx) {
		stats[playerNdx].DEF = GetDEF(playerNdx, stats[playerNdx].LVL);
		stats[playerNdx].baseDEF = stats[playerNdx].DEF;
	}
	public int GetDEFUpgrade(int playerNdx) {
		return GetDEF(playerNdx, stats[playerNdx].LVL) - GetDEF(playerNdx, stats[playerNdx].previousLVL);
	}

	// WIS
	public int GetWIS(int playerNdx, int LVL) {
		if (playerNdx == 0) {
			return (int)(1.5f * LVL); // Blob: Lvl 1 = 1
		} else {
			return (int)(2 * LVL); // Chani: Lvl 1 = 2
		}
	}
	public void SetWIS(int playerNdx) {
		stats[playerNdx].WIS = GetWIS(playerNdx, stats[playerNdx].LVL);
		stats[playerNdx].baseWIS = stats[playerNdx].WIS;
	}
	public int GetWISUpgrade(int playerNdx) {
		return GetWIS(playerNdx, stats[playerNdx].LVL) - GetWIS(playerNdx, stats[playerNdx].previousLVL);
	}

	// AGI
	public int GetAGI(int playerNdx, int LVL) {
		if (playerNdx == 0) {
			return (int)(1.5f * LVL); // Blob: Lvl 1 = 1
		} else {
			return (int)(2 * LVL); // Chani: Lvl 1 = 2
		}
	}
	public void SetAGI(int playerNdx) {
		stats[playerNdx].AGI = GetAGI(playerNdx, stats[playerNdx].LVL);
		stats[playerNdx].baseAGI = stats[playerNdx].AGI;
	}
	public int GetAGIUpgrade(int playerNdx) {
		return GetAGI(playerNdx, stats[playerNdx].LVL) - GetAGI(playerNdx, stats[playerNdx].previousLVL);
	}

	// SpellNdx (Mathf.Min used to prevent spellNdx from exceeding
	// the amount of spells each party member is capable of learning)
	public int GetSpellNdx(int playerNdx, int LVL) {
		return 13;
		//if (playerNdx == 0) {
		//	return Mathf.Min((int)(0.5f * LVL), stats[playerNdx].spells.Count); // Blob: Lvl 1 = 0
		//} else {
		//	return Mathf.Min((int)(1.0f * LVL + 1), stats[playerNdx].spells.Count); // Chani: Lvl 1 = 2
		//}
	}

	public void SetSpellNdx(int playerNdx) {
		stats[playerNdx].spellNdx = GetSpellNdx(playerNdx, stats[playerNdx].LVL);
	}
	public int GetSpellNdxUpgrade(int playerNdx) {
		return GetSpellNdx(playerNdx, stats[playerNdx].LVL) - GetSpellNdx(playerNdx, stats[playerNdx].previousLVL);
	}

	public int GetExpToNextLevel(int playerNdx) {
		return stats[playerNdx].expToNextLevel[stats[playerNdx].LVL + 1] - stats[playerNdx].EXP;
	}

	public void CheckForLevelUp() {
		// Loop through all party members
		for (int i = 0; i < stats.Count; i++) {
			// Loop through levels 10 through 2
			for (int j = 10; j >= 2; j--) {
				if (stats[i].EXP >= stats[i].expToNextLevel[j] && !stats[i].hasReachedThisLevel[j]) {
					LevelUp(j, i);
				}
			}
		}
	}

	void LevelUp(int newLVL, int playerNdx) {
		stats[playerNdx].hasLeveledUp = true;

		stats[playerNdx].previousLVL = stats[playerNdx].LVL;
		stats[playerNdx].LVL = newLVL;
		SetSpellNdx(playerNdx);

		// Assign Stats
		SetHP(playerNdx);
		SetMP(playerNdx);
		SetSTR(playerNdx);
		SetAGI(playerNdx);
		SetDEF(playerNdx);
		SetWIS(playerNdx);

		// Add current equipment's stat effect(s) to party member's stats
		for (int i = 0; i < EquipMenu.S.playerEquipment[0].Count; i++) {
			if (EquipMenu.S.playerEquipment[playerNdx][i] != null) {
				EquipMenu.S.equipStatsEffect.AddItemEffect(playerNdx, EquipMenu.S.playerEquipment[playerNdx][i]);
			}
		}

		// Mark that this Level has been reached (and all previous levels)
		for (int i = 0; i < newLVL + 1; i++) {
			stats[playerNdx].hasReachedThisLevel[i] = true;
		}
	}
}

public class PartyStats {
	public string name;
	public int HP;
	public int maxHP;
	public int baseMaxHP;
	public int MP;
	public int maxMP;
	public int baseMaxMP;

	public int STR;
	public int baseSTR;
	public int DEF;
	public int baseDEF;
	public int WIS;
	public int baseWIS;
	public int AGI;
	public int baseAGI;

	public int EXP;
	public int LVL;
	public int spellNdx;
	public List<Spell> spells;
	public List<bool> hasReachedThisLevel;
	public List<int> expToNextLevel;
	public bool hasLeveledUp;
	public int previousLVL;

	public int battleID;

	public PartyStats(string name, int HP, int maxHP, int baseMaxHP, int MP, int maxMP, int baseMaxMP,
		int STR, int baseSTR, int DEF, int baseDEF, int WIS, int baseWIS, int AGI, int baseAGI,
		int EXP, int LVL, int spellNdx,
		List<Spell> spells, List<bool> hasReachedThisLevel, List<int> expToNextLevel, bool hasLeveledUp, int previousLVL, int battleID) {
		this.name = name;
		this.HP = HP;
		this.maxHP = maxHP;
		this.baseMaxHP = baseMaxHP;
		this.MP = MP;
		this.maxMP = maxMP;
		this.baseMaxMP = baseMaxMP;

		this.STR = STR;
		this.baseSTR = baseSTR;
		this.DEF = DEF;
		this.baseDEF = baseDEF;
		this.WIS = WIS;
		this.baseWIS = baseWIS;
		this.AGI = AGI;
		this.baseAGI = baseAGI;

		this.EXP = EXP;
		this.LVL = LVL;
		this.spellNdx = spellNdx;
		this.spells = spells;
		this.hasReachedThisLevel = hasReachedThisLevel;
		this.expToNextLevel = expToNextLevel;
		this.previousLVL = previousLVL;

		this.battleID = battleID;
	}
}