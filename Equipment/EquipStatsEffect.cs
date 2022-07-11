using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipStatsEffect : MonoBehaviour {
	[Header("Set in Inspector")]
	// Potential Stats
	public GameObject potentialStatHolder;
	public Text currentAttributeAmounts; // STR, DEF, WIS, AGI
	public Text potentialStats;
	public List<GameObject> arrowGO;
	public List<Animator> arrowAnim;

	void OnDisable() {
		// Deactivate Arrow Sprites
		for (int i = 0; i <= arrowGO.Count - 1; i++) {
			arrowGO[i].SetActive(false);
		}
	}

	// Add item's stat effect to party member's stats
	public void AddItemEffect(int playerNdx, Item item) {
		item.isEquipped = true;

		switch (item.statEffect) {
			case eItemStatEffect.AGI: Party.S.stats[playerNdx].AGI += item.statEffectMaxValue; break;
			case eItemStatEffect.DEF: Party.S.stats[playerNdx].DEF += item.statEffectMaxValue; break;
			case eItemStatEffect.HP: Party.S.stats[playerNdx].HP += item.statEffectMaxValue; break;
			case eItemStatEffect.MP: Party.S.stats[playerNdx].MP += item.statEffectMaxValue; break;
			case eItemStatEffect.STR: Party.S.stats[playerNdx].STR += item.statEffectMaxValue; break;
			case eItemStatEffect.WIS: Party.S.stats[playerNdx].WIS += item.statEffectMaxValue; break;
		}
	}

	// Remove item's stat effect from party member's stats
	public void RemoveItemEffect(int playerNdx, Item item) {
		item.isEquipped = false;

		// Subtract Item Effect
		switch (item.statEffect) {
			case eItemStatEffect.AGI: Party.S.stats[playerNdx].AGI -= item.statEffectMaxValue; break;
			case eItemStatEffect.DEF: Party.S.stats[playerNdx].DEF -= item.statEffectMaxValue; break;
			case eItemStatEffect.HP: Party.S.stats[playerNdx].HP -= item.statEffectMaxValue; break;
			case eItemStatEffect.MP: Party.S.stats[playerNdx].MP -= item.statEffectMaxValue; break;
			case eItemStatEffect.STR: Party.S.stats[playerNdx].STR -= item.statEffectMaxValue; break;
			case eItemStatEffect.WIS: Party.S.stats[playerNdx].WIS -= item.statEffectMaxValue; break;
		}
	}

	// Display party member's stats if they equipped this item
	public void DisplayPotentialStats(int playerNdx, Item tItem, List<List<Item>> playerEquipment) {
		// Deactivate Arrow GameObjects
		for (int i = 0; i <= arrowGO.Count - 1; i++) {
			arrowGO[i].SetActive(false);
		}

		// Get Current Stats
		List<int> potential = new List<int>() { Party.S.stats[playerNdx].STR, Party.S.stats[playerNdx].DEF, Party.S.stats[playerNdx].WIS, Party.S.stats[playerNdx].AGI };

		// Subtract stats of currently equipped item 
		switch (playerEquipment[playerNdx][(int)tItem.type].statEffect) {
			case eItemStatEffect.STR: potential[0] -= playerEquipment[playerNdx][(int)tItem.type].statEffectMaxValue; break;
			case eItemStatEffect.DEF: potential[1] -= playerEquipment[playerNdx][(int)tItem.type].statEffectMaxValue; break;
			case eItemStatEffect.WIS: potential[2] -= playerEquipment[playerNdx][(int)tItem.type].statEffectMaxValue; break;
			case eItemStatEffect.AGI: potential[3] -= playerEquipment[playerNdx][(int)tItem.type].statEffectMaxValue; break;
		}

		// Add stats of item to be potentially equipped
		switch (tItem.statEffect) {
			case eItemStatEffect.STR: potential[0] += tItem.statEffectMaxValue; break;
			case eItemStatEffect.DEF: potential[1] += tItem.statEffectMaxValue; break;
			case eItemStatEffect.WIS: potential[2] += tItem.statEffectMaxValue; break;
			case eItemStatEffect.AGI: potential[3] += tItem.statEffectMaxValue; break;
		}

		// Find difference between current & potential Stats
		List<int> statDifference = new List<int>() { potential[0] - Party.S.stats[playerNdx].STR, potential[1] - Party.S.stats[playerNdx].DEF, potential[2] - Party.S.stats[playerNdx].WIS, potential[3] - Party.S.stats[playerNdx].AGI };

		// If Current Stats != Potential Stats, activate potential stats & arrows
		if (potential[0] != Party.S.stats[playerNdx].STR) {
			ActivatePotentialStatsAndArrow(0, statDifference[0]);
		}
		if (potential[1] != Party.S.stats[playerNdx].DEF) {
			ActivatePotentialStatsAndArrow(1, statDifference[1]);
		}
		if (potential[2] != Party.S.stats[playerNdx].WIS) {
			ActivatePotentialStatsAndArrow(2, statDifference[2]);
		}
		if (potential[3] != Party.S.stats[playerNdx].AGI) {
			ActivatePotentialStatsAndArrow(3, statDifference[3]);
		}

		// Update GUI
		potentialStats.text = potential[0] + "\n" + potential[1] + "\n" + potential[2] + "\n" + potential[3];
	}

	// Activate potential stats and animate up or down arrows 
	void ActivatePotentialStatsAndArrow(int ndx, int amount) {
		// Activate Potential Stat
		potentialStatHolder.SetActive(true);

		// Activate Arrow GameObject
		arrowGO[ndx].SetActive(true);

		// Set Arrow Animation
		if (amount > 0) {
			arrowAnim[ndx].CrossFade("Arrow_Up", 0);
		} else {
			arrowAnim[ndx].CrossFade("Arrow_Down", 0);
		}
	}
}