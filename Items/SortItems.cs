using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Functions used to sort items
/// </summary>
public class SortItems : MonoBehaviour {
	[Header("Set Dynamically")]
	private static SortItems _S;
	public static SortItems S { get { return _S; } set { _S = value; } }

	// Temporary Items List (For Item/Equip Screens: Sort by ItemType: Weapon, Armor, Helmet, Other, Healing)
	public List<Item> tItems;

	void Awake() {
		S = this;
	}

	// Sort Items Alphabetically
	public Dictionary<Item, int> SortByABC(Dictionary<Item, int> items) {
		// Copy Item List
		tItems = new List<Item>(items.Keys);

		// Sort Ascending Alphabetically 
		tItems = tItems.OrderBy(n => n.name).ToList();
		// Sort Descending Alphabetically 
		//tItems = tItems.OrderByDescending(n => n.itemName).ToList();

		// Create tDictionary
		Dictionary<Item, int> tDict = new Dictionary<Item, int>();

		// Populate tDictionary 
		foreach (var k in tItems) {
			tDict.Add(k, items[k]);
		}

		Items.S.menu.AssignItemNames();
		Items.S.menu.AssignItemEffect();

		return tDict;
	}

	// Sort Items by Value
	public Dictionary<Item, int> SortByValue(Dictionary<Item, int> items) {
		// Copy Item List
		tItems = new List<Item>(items.Keys);

		// Sort Ascending by Value
		tItems = tItems.OrderBy(n => n.value).ToList();
		// Sort Descending by Value
		//tItems = tItems.OrderByDescending(n => n.itemValue).ToList();

		// Create tDictionary
		Dictionary<Item, int> tDict = new Dictionary<Item, int>();

		// Populate tDictionary 
		foreach (var k in tItems) {
			tDict.Add(k, items[k]);
		}

		Items.S.menu.AssignItemNames();
		Items.S.menu.AssignItemEffect();

		return tDict;
	}

	// Sort Items by ItemType; called in EquipScreen.cs
	public void SortByItemType(Dictionary<Item, int> items, eItemType tItemType) {
		// Copy Item List
		tItems = new List<Item>(items.Keys);

		// Remove Items that don't match ItemType
		for (int i = 0; i < tItems.Count; i++) {
			if (tItems[i].type != tItemType) {
				tItems.RemoveAt(i);
				i -= 1;
			}
		}
	}
}