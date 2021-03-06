using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores and manages the party's inventory
/// </summary>
public class Inventory : MonoBehaviour {
    [Header("Set Dynamically")]
    private static Inventory _S;
    public static Inventory S { get { return _S; } set { _S = value; } }

    public Dictionary<Item, int> items = new Dictionary<Item, int>();

    void Awake() {
        S = this;
    }

    void Start() {
        // Add items to inventory
        //AddItemToInventory(Items.S.items[25]);
        AddItemToInventory(Items.S.items[25]);
        AddItemToInventory(Items.S.items[25]);
        //AddItemToInventory(Items.S.items[26]);
        AddItemToInventory(Items.S.items[26]);
        AddItemToInventory(Items.S.items[26]);
        AddItemToInventory(Items.S.items[27]);
        AddItemToInventory(Items.S.items[27]);
        //AddItemToInventory(Items.S.items[27]);
        AddItemToInventory(Items.S.items[0]);
        AddItemToInventory(Items.S.items[0]);
        AddItemToInventory(Items.S.items[1]);

        AddItemToInventory(Items.S.items[23]);
        AddItemToInventory(Items.S.items[23]);
        AddItemToInventory(Items.S.items[2]);
        AddItemToInventory(Items.S.items[3]);
        AddItemToInventory(Items.S.items[4]);
        AddItemToInventory(Items.S.items[5]);
        AddItemToInventory(Items.S.items[6]);
        AddItemToInventory(Items.S.items[7]);
        AddItemToInventory(Items.S.items[8]);
        AddItemToInventory(Items.S.items[9]);

        AddItemToInventory(Items.S.items[10]);
        AddItemToInventory(Items.S.items[0]);
        AddItemToInventory(Items.S.items[11]);
        AddItemToInventory(Items.S.items[12]);
        AddItemToInventory(Items.S.items[13]);
        AddItemToInventory(Items.S.items[14]);
        AddItemToInventory(Items.S.items[15]);
        AddItemToInventory(Items.S.items[16]);
        AddItemToInventory(Items.S.items[17]);
        AddItemToInventory(Items.S.items[18]);
        AddItemToInventory(Items.S.items[19]);
        AddItemToInventory(Items.S.items[20]);
        AddItemToInventory(Items.S.items[21]);

        AddItemToInventory(Items.S.items[22]);
        AddItemToInventory(Items.S.items[22]);
        AddItemToInventory(Items.S.items[24]);
        AddItemToInventory(Items.S.items[24]);
        AddItemToInventory(Items.S.items[24]);
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
        //Items.S.menu.AssignItemNames();
        //inventPauseMenu.S.UpdateGUI();

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