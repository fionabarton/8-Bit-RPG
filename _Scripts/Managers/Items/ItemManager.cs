using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum eItem { hpPotion, mpPotion, assSword, crap, nothing, assArmor, assHelmet, assOther, crapSword,
                    assWand, berry, smallKey, bug1, bug2, bug3, bug4, bug5, bug6 };
public enum eItemType { weapon, armor, helmet, other, healing, ingredient, important, nothing };
public enum eItemStatEffect { HP, MP, STR, DEF, WIS, AGI, nothing };

public class ItemManager : MonoBehaviour {
	[Header("Set in Inspector")]
    // TBR: Has yet to be implemented; will be used to display image of item on ItemScreen
	public Sprite [] 			itemSprite = new Sprite[30];

	[Header("Set Dynamically")]
	// Singleton
	private static ItemManager  _S;
	public static ItemManager   S { get { return _S; } set { _S = value; } }

	public Item[] 				items;

	void Awake() {
		S = this;

        // Initialize array of Items
		items = new Item[30];

        // Health Potion
        items[0] = new Item("Health Potion", eItemType.healing, eItemStatEffect.HP, 0, 8,
        "Replenishes at least 30 HP." + "\n Value: 8 Gold", itemSprite[0]);

        // Magic Potion
        items[1] = new Item("Magic Potion", eItemType.healing, eItemStatEffect.MP, 0, 24,
        "Replenishes at least 30 MP." + "\n Value: 24 Gold", itemSprite[1]);

        // Ass Sword
        items[2] = new Item("Ass Sword", eItemType.weapon, eItemStatEffect.STR, 10, 5,
        "Adds to Strength like a mother pucker! +10!" + "\n Value: 5 Gold", itemSprite[2]);

        // Shit 
        items[3] = new Item("Crap", eItemType.nothing, eItemStatEffect.nothing, 0, 0,
        "It's crap. Literal crap." + "\n Value: 0 Gold", itemSprite[3]);

        // Nothing 
        items[4] = new Item("Nothing", eItemType.nothing, eItemStatEffect.nothing, 0, 0,
        "It's nothing. How lame, what a poorly designed game!" + "\n Value: 0 Gold", itemSprite[4]);

        // Ass Armor
        items[5] = new Item("Ass Armor", eItemType.armor, eItemStatEffect.DEF, 6, 7,
        "Adds to Defense like some ASS ARMOR! +6!" + "\n Value: 7 Gold", itemSprite[5]);

        // Ass Helmet
        items[6] = new Item("Ass Helmet", eItemType.helmet, eItemStatEffect.DEF, 9, 9,
        "Adds to Defense like an ASS HELMET! +9!" + "\n Value: 9 Gold", itemSprite[6]);

        // Ass Other
        items[7] = new Item("Ass Other", eItemType.other, eItemStatEffect.AGI, 9, 9,
        "Adds to Agility like an ASS OTHER! +9!" + "\n Value: 9 Gold", itemSprite[7]);

        // Crap Sword
        items[8] = new Item("Crap Sword", eItemType.weapon, eItemStatEffect.STR, 12, 6,
        "Adds to Strength like a CRAP SWORD! +12!" + "\n Value: 6 Gold", itemSprite[8]);

        // Ass Wand
        items[9] = new Item("Ass Wand", eItemType.weapon, eItemStatEffect.WIS, 10, 1,
        "Adds to WISDOM! +10! ASS WAND!!!" + "\n Value: 1 Gold", itemSprite[9]);

        // Berry
        items[10] = new Item("Berry", eItemType.ingredient, eItemStatEffect.nothing, 0, 5,
        "A tasty, tasty berry. C'mon, loser! Sell me for some loser money!" + "\n Value: 5 Gold", itemSprite[10]);

        // Small Key
        items[11] = new Item("Small Key", eItemType.other, eItemStatEffect.nothing, 0, 0,
        "A small key that fits into any small lock found on any small door. BIZARRE, BIZZARE!" + "\n Value: 0 Gold", itemSprite[11]);

        // Bug_1
        items[12] = new Item("Nut Bat", eItemType.ingredient, eItemStatEffect.nothing, 0, 15,
        "A nutty little bat filled with blood of Michael Geen's birthday delight. Cheers, butthead!" + "\n Value: 15 Gold", itemSprite[12]);

        // Bug_2
        items[13] = new Item("Violet Pilot", eItemType.ingredient, eItemStatEffect.nothing, 0, 15,
        "Luxury is this banging butterfly!" + "\n Value: 15 Gold", itemSprite[13]);

        // Bug_3
        items[14] = new Item("Vampire Bat", eItemType.ingredient, eItemStatEffect.nothing, 0, 15,
        "Beware! This little butt might have rabies!" + "\n Value: 15 Gold", itemSprite[14]);

        // Bug_4
        items[15] = new Item("Orange Ollie", eItemType.ingredient, eItemStatEffect.nothing, 0, 15,
        "Flap, flap, flap! What's this guy up to?" + "\n Value: 15 Gold", itemSprite[15]);

        // Bug_5
        items[16] = new Item("Blue Betty", eItemType.ingredient, eItemStatEffect.nothing, 0, 15,
        "What a beauty! A butterfly filled with blood!" + "\n Value: 15 Gold", itemSprite[16]);

        // Bug_6
        items[17] = new Item("Bumble Bee", eItemType.ingredient, eItemStatEffect.nothing, 0, 15,
        "A tasty bumble bee. Eat me!!!" + "\n Value: 15 Gold", itemSprite[17]);

        // Nothing Weapon
        items[18] = new Item("Nothing", eItemType.nothing, eItemStatEffect.nothing, 0, 0,
        "Press the action button to select a weapon to equip.", itemSprite[18]);

        // Nothing Armor
        items[19] = new Item("Nothing", eItemType.nothing, eItemStatEffect.nothing, 0, 0,
        "Press the action button to select a set of armor to equip.", itemSprite[19]);

        // Nothing Helmet
        items[20] = new Item("Nothing", eItemType.nothing, eItemStatEffect.nothing, 0, 0,
        "Press the action button to select a helmet to equip.", itemSprite[20]);

        // Nothing Other
        items[21] = new Item("Nothing", eItemType.nothing, eItemStatEffect.nothing, 0, 0,
        "Press the action button to select an item to equip.", itemSprite[21]);
    }

    public Item GetItem(eItem itemNdx){
		Item tItem = items[(int)itemNdx];
		return tItem;
	}
}

public class Item {
	public string 			name;
	public eItemType 		type;
	public eItemStatEffect 	statEffect;
	public int 				statEffectValue;
	public int 				value;
	public string 			description;
	public Sprite 			sprite;
    public bool             isEquipped;

    public Item(string itemName, 
                eItemType itemType, eItemStatEffect itemStatEffect, 
                int itemStatEffectValue, int itemValue, 
				string itemDescription, Sprite itemSprite, bool itemIsEquipped = false) {
	    name = itemName;
	    type = itemType;
	    statEffect = itemStatEffect;
	    statEffectValue = itemStatEffectValue;
	    value = itemValue;
	    description = itemDescription;
	    sprite = itemSprite;
        isEquipped = itemIsEquipped;
	}
}