using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Items : MonoBehaviour {
    [Header("Set in Inspector")]
    // TBR: Has yet to be implemented; will be used to display image of item on ItemScreen
    public Sprite[] itemSprite = new Sprite[30];

    [Header("Set Dynamically")]
    public ItemMenu menu;
    public BattleItems battle;
    public WorldItems world;

    public Item[] items;

    private static Items _S;
    public static Items S { get { return _S; } set { _S = value; } }

    void Awake() {
        S = this;

        InitializeItems();

        // Get components
        menu = GetComponent<ItemMenu>();
        battle = GetComponent<BattleItems>();
        world = GetComponent<WorldItems>();
    }

    public void InitializeItems() {
        // Initialize array of items
        items = new Item[30];

        // Health Potion
        items[0] = new Item("000", "Health Potion", eItemType.Consumable, eItemStatEffect.HP, 30, 45, 8,
        "Heals a single party member for at least 30 HP." + "\n Value: 8 Gold", itemSprite[0]);

        // Magic Potion
        items[1] = new Item("001", "Magic Potion", eItemType.Consumable, eItemStatEffect.MP, 12, 20, 24,
        "Replenishes at least 12 MP for a single party member." + "\n Value: 24 Gold", itemSprite[1]);

        // Paper Sword
        items[2] = new Item("002", "Paper Sword", eItemType.Weapon, eItemStatEffect.STR, 10, 10, 5,
        "Capable of inflicting light physical damage upon an enemy.\nAdds +10 to Strength." + "\n Value: 5 Gold", itemSprite[2]);

        // Crap 
        items[3] = new Item("003", "Crap", eItemType.Useless, eItemStatEffect.nothing, 0, 0, 0,
        "It's crap. Literal crap... yuck." + "\n Value: 0 Gold", itemSprite[3]);

        // Nothing 
        items[4] = new Item("004", "Nothing", eItemType.Useless, eItemStatEffect.nothing, 0, 0, 0,
        "It's nothing. Absolutely nothing.\nReally? What a disappointing game!" + "\n Value: 0 Gold", itemSprite[4]);

        // Paper Armor
        items[5] = new Item("005", "Paper Armor", eItemType.Armor, eItemStatEffect.DEF, 6, 6, 7,
        "Reduces the amount of damage taken from an enemy.\nAdds +6 to Defense." + "\n Value: 7 Gold", itemSprite[5]);

        // Paper Helmet
        items[6] = new Item("006", "Paper Helmet", eItemType.Helmet, eItemStatEffect.DEF, 9, 9, 9,
        "Reduces the amount of damage taken from an enemy.\nAdds +9 to Defense." + "\n Value: 9 Gold", itemSprite[6]);

        // Paper Accessory
        items[7] = new Item("007", "Paper Accessory", eItemType.Accessory, eItemStatEffect.AGI, 9, 9, 9,
        "Increases the speed of a party member.\nAdds +9 to Agility." + "\n Value: 9 Gold", itemSprite[7]);

        // Wooden Sword
        items[8] = new Item("008", "Wooden Sword", eItemType.Weapon, eItemStatEffect.STR, 12, 12, 6,
        "Capable of inflicting moderate physical damage\nupon an enemy. Adds +12 to Strength." + "\n Value: 6 Gold", itemSprite[8]);

        // Paper Wand
        items[9] = new Item("009", "Paper Wand", eItemType.Weapon, eItemStatEffect.WIS, 10, 10, 1,
        "Capable of inflicting light magic damage upon an enemy.\nAdds +10 to Wisdom." + "\n Value: 1 Gold", itemSprite[9]);

        // Berry
        items[10] = new Item("010", "Berry", eItemType.Ingredient, eItemStatEffect.nothing, 0, 0, 5,
        "Aside from its resale value,\nthe party has no current use for this tasty berry." + "\n Value: 5 Gold", itemSprite[10]);

        // Small Key
        items[11] = new Item("011", "Small Key", eItemType.Important, eItemStatEffect.nothing, 0, 0, 0,
        "A small key that can unlock any small door.\nIt can only be used on a single door." + "\n Value: 0 Gold", itemSprite[11]);

        // Bug_1
        items[12] = new Item("012", "Blue Bat", eItemType.Ingredient, eItemStatEffect.nothing, 0, 0, 15,
        "Aside from its resale value,\nthe party has no current use for this creature." + "\n Value: 15 Gold", itemSprite[12]);

        // Bug_2
        items[13] = new Item("013", "Violet Butterfly", eItemType.Ingredient, eItemStatEffect.nothing, 0, 0, 15,
        "Aside from its resale value,\nthe party has no current use for this creature." + "\n Value: 15 Gold", itemSprite[13]);

        // Bug_3
        items[14] = new Item("014", "Vampire Bat", eItemType.Ingredient, eItemStatEffect.nothing, 0, 0, 15,
        "Aside from its resale value,\nthe party has no current use for this creature." + "\n Value: 15 Gold", itemSprite[14]);

        // Bug_4
        items[15] = new Item("015", "Orange Butterfly", eItemType.Ingredient, eItemStatEffect.nothing, 0, 0, 15,
        "Aside from its resale value,\nthe party has no current use for this creature." + "\n Value: 15 Gold", itemSprite[15]);

        // Bug_5
        items[16] = new Item("016", "Blue Butterfly", eItemType.Ingredient, eItemStatEffect.nothing, 0, 0, 15,
        "Aside from its resale value,\nthe party has no current use for this creature." + "\n Value: 15 Gold", itemSprite[16]);

        // Bug_6
        items[17] = new Item("017", "Bumble Bee", eItemType.Ingredient, eItemStatEffect.nothing, 0, 0, 15,
        "Aside from its resale value,\nthe party has no current use for this creature." + "\n Value: 15 Gold", itemSprite[17]);

        // Default Weapon
        items[18] = new Item("018", "Default Weapon", eItemType.Weapon, eItemStatEffect.STR, 1, 1, 5,
        "The weapon that each party member starts out with.\nAdds +1 to Strength." + "\n Value: 5 Gold", itemSprite[18], true);

        // Default Armor
        items[19] = new Item("019", "Default Armor", eItemType.Armor, eItemStatEffect.DEF, 1, 1, 5,
        "The armor that each party member starts out with.\nAdds +1 to Defense." + "\n Value: 5 Gold", itemSprite[19], true);

        // Default Helmet
        items[20] = new Item("020", "Default Helmet", eItemType.Helmet, eItemStatEffect.DEF, 1, 1, 5,
        "The helmet that each party member starts out with.\nAdds +1 to Defense." + "\n Value: 5 Gold", itemSprite[20], true);

        // Default Accessory
        items[21] = new Item("021", "Default Accessory", eItemType.Accessory, eItemStatEffect.AGI, 1, 1, 5,
        "The accessory that each party member starts out with.\nAdds +1 to Agility." + "\n Value: 5 Gold", itemSprite[21], true);

        // Heal All Potion
        items[22] = new Item("022", "Heal All Potion", eItemType.Consumable, eItemStatEffect.HP, 12, 20, 20,
        "Heals ALL party members for at least 12 HP!" + "\n Value: 20 Gold", itemSprite[22], false, true);

        // Warp Potion
        items[23] = new Item("023", "Warp Potion", eItemType.Consumable, eItemStatEffect.nothing, 0, 0, 15,
        "Instantaneously transports the party\nto a previously visited location." + "\n Value: 15 Gold", itemSprite[23]);

        // Revive Potion
        items[24] = new Item("024", "Revive Potion", eItemType.Consumable, eItemStatEffect.HP, 12, 20, 25,
        "Revives a fallen party member and\nrestores a small amount of their HP." + "\n Value: 25 Gold", itemSprite[24]);

        // Detoxify Potion
        items[25] = new Item("025", "Detoxify Potion", eItemType.Consumable, eItemStatEffect.nothing, 0, 0, 5,
        "Eradicates any toxins that have infected\na poisoned party member." + "\n Value: 5 Gold", itemSprite[25]);

        // Mobilize Potion
        items[26] = new Item("026", "Mobilize Potion", eItemType.Consumable, eItemStatEffect.nothing, 0, 0, 5,
        "Restores the mobility of a paralyzed party member." + "\n Value: 5 Gold", itemSprite[26]);

        // Wake Potion
        items[27] = new Item("027", "Wake Potion", eItemType.Consumable, eItemStatEffect.nothing, 0, 0, 5,
        "Wakes up a sleeping party member." + "\n Value: 5 Gold", itemSprite[27]);

        // Simon's Whip
        items[28] = new Item("028", "Simon's Whip", eItemType.Weapon, eItemStatEffect.STR, 1, 1, 10,
        "A whip that strikes all enemies.\nAdds +1 to Strength." + "\n Value: 10 Gold", itemSprite[28], true, true);

        // Mega Whip
        items[29] = new Item("029", "Mega Whip", eItemType.Weapon, eItemStatEffect.STR, 5, 5, 50,
        "A mega whip that blasts all enemies.\nAdds +5 to Strength." + "\n Value: 50 Gold", itemSprite[29], true, true);
    }

    public Item GetItem(eItem itemNdx) {
        Item tItem = items[(int)itemNdx];
        return tItem;
    }
}

public class Item {
    public string id;
    public string name;
    public eItemType type;
    public eItemStatEffect statEffect;
    public int statEffectMinValue;
    public int statEffectMaxValue;
    public int value;
    public string description;
    public Sprite sprite;
    public bool isEquipped;
    public bool multipleTargets;

    public Item(string itemId, string itemName,
                eItemType itemType, eItemStatEffect itemStatEffect,
                int itemStatEffectMinValue, int itemStatEffectMaxValue, int itemValue, string itemDescription,
                Sprite itemSprite, bool itemIsEquipped = false, bool itemMultipleTargets = false) {
        id = itemId;
        name = itemName;
        type = itemType;
        statEffect = itemStatEffect;
        statEffectMinValue = itemStatEffectMinValue;
        statEffectMaxValue = itemStatEffectMaxValue;
        value = itemValue;
        description = itemDescription;
        sprite = itemSprite;
        isEquipped = itemIsEquipped;
        multipleTargets = itemMultipleTargets;
    }
}