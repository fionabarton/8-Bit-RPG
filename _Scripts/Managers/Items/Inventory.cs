using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores and manages the party's inventory
/// </summary>
public class Inventory : MonoBehaviour
{
	[Header("Set Dynamically")]
	// Singleton
	private static Inventory		_S;
	public static Inventory			S { get { return _S; } set { _S = value; } }

	public Dictionary<Item, int>	items = new Dictionary<Item, int>();

	void Awake() {
		// Singleton
		S = this;
	}

	// Add HP & MP potions to inventory
	void Start() {
        AddItemToInventory(ItemManager.S.items[0]);
        AddItemToInventory(ItemManager.S.items[0]);
        AddItemToInventory(ItemManager.S.items[1]);
        AddItemToInventory(ItemManager.S.items[1]);
		AddItemToInventory(ItemManager.S.items[2]);
		AddItemToInventory(ItemManager.S.items[5]);
		AddItemToInventory(ItemManager.S.items[6]);
		AddItemToInventory(ItemManager.S.items[8]);
		AddItemToInventory(ItemManager.S.items[9]);
		AddItemToInventory(ItemManager.S.items[15]);
        AddItemToInventory(ItemManager.S.items[15]);
    } 

	public void AddItemToInventory(Item name) {
		if (items.ContainsKey(name)) {
			items[name] += 1;
		} else {
			items[name] = 1;
		}
	}

    public void RemoveItemFromInventory(Item name) {
        items[name]--;

        // Update Pause & Overworld GUI
        ItemScreen.S.AssignItemNames();
        PauseScreen.S.UpdateGUI();

        // Remove the entry if the count goes to 0.
        if (items[name] == 0) {
            items.Remove(name);
        }
    }

    // Return a List of all the Dictionary keys
    public List<Item> GetItemList() {
		List<Item> list = new List<Item>(items.Keys);
		return list;
	}

	// Return how many of that item are in inventory
	public int GetItemCount(Item name) {
		if (items.ContainsKey(name)) {
			return items[name];
		}
		return 0;
	}
}