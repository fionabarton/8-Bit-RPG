using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour {
	[Header("Set Dynamically")]
	public SpellMenu menu;
	public BattleSpells battle;
	public WorldSpells world;

	public Spell[] spells;

	private static Spells _S;
	public static Spells S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;

		InitializeSpells();

		// Get components
		menu = GetComponent<SpellMenu>();
		battle = GetComponent<BattleSpells>();
		world = GetComponent<WorldSpells>();
	}

	public void InitializeSpells() {
		// Initialize array of spells
		spells = new Spell[20];

		// Heal
		spells[0] = new Spell("Heal",
			eSpellType.Healing, eSpellStatEffect.HP, eSpellUseableMode.any, 30, 45, 3,
			"Heals a single party member for at least 30 HP." + "\n Cost: 3 MP");

		// Fireball
		spells[1] = new Spell("Fireball",
			eSpellType.Offensive, eSpellStatEffect.HP, eSpellUseableMode.battle, 8, 12, 2,
			"Blasts a single enemy for at least 8 HP." + "\n Cost: 2 MP");

		// Warp
		spells[2] = new Spell("Warp",
			eSpellType.World, eSpellStatEffect.none, eSpellUseableMode.world, 0, 0, 1,
			"Instantaneously transports the party\nto a previously visited location." + "\n Cost: 1 MP");

		// Fireblast
		spells[3] = new Spell("Fireblast",
			eSpellType.Offensive, eSpellStatEffect.HP, eSpellUseableMode.battle, 12, 20, 3,
			"Blasts ALL enemies for at least 12 HP." + "\n Cost: 3 MP", true);

		// Heal All
		spells[4] = new Spell("Heal All",
			eSpellType.Healing, eSpellStatEffect.HP, eSpellUseableMode.battle, 12, 20, 6,
			"Heals ALL party members for at least 12 HP." + "\n Cost: 6 MP", true);

		// Revive
		spells[5] = new Spell("Revive",
			eSpellType.Healing, eSpellStatEffect.HP, eSpellUseableMode.battle, 12, 20, 6,
			"Revives a fallen party member and\nrestores a small amount of their HP." + "\n Cost: 6 MP");

		// Detoxify 
		spells[6] = new Spell("Detoxify",
			eSpellType.Healing, eSpellStatEffect.none, eSpellUseableMode.battle, 0, 0, 2,
			"Eradicates any toxins that have\ninfected a poisoned party member." + "\n Cost: 2 MP");

		// Mobilize 
		spells[7] = new Spell("Mobilize",
			eSpellType.Healing, eSpellStatEffect.none, eSpellUseableMode.battle, 0, 0, 2,
			"Restores the mobility of a paralyzed party member." + "\n Cost: 2 MP");

		// Wake 
		spells[8] = new Spell("Wake",
			eSpellType.Healing, eSpellStatEffect.none, eSpellUseableMode.battle, 0, 0, 2,
			"Wakes up a sleeping party member." + "\n Cost: 2 MP");

		// Poison 
		spells[9] = new Spell("Poison",
			eSpellType.Offensive, eSpellStatEffect.none, eSpellUseableMode.battle, 0, 0, 1,
			"Poisons a single enemy. At the start of each turn,\nit damages the enemy for a small amount of HP." + "\n Cost: 1 MP");

		// Paralyze 
		spells[10] = new Spell("Paralyze",
			eSpellType.Offensive, eSpellStatEffect.none, eSpellUseableMode.battle, 0, 0, 1,
			"Temporarily paralyzes a single enemy for a few turns." + "\n Cost: 1 MP");

		// Sleep 
		spells[11] = new Spell("Sleep",
			eSpellType.Offensive, eSpellStatEffect.none, eSpellUseableMode.battle, 0, 0, 1,
			"Temporarily puts a single enemy to sleep for a few turns." + "\n Cost: 1 MP");

		// Steal 
		spells[12] = new Spell("Steal",
			eSpellType.Thievery, eSpellStatEffect.none, eSpellUseableMode.battle, 0, 0, 1,
			"Attempts to steal an item from an enemy." + "\n Cost: 1 MP");
	}

	// Spell Utilities ////////////////////////////////////////////

	public void SpellHelper() {
		// Buttons Interactable
		Utilities.S.ButtonsInteractable(PauseMenu.S.playerNameButtons, false);
		Utilities.S.ButtonsInteractable(menu.spellsButtons, false);

		// Update GUI
		PauseMenu.S.UpdateGUI();

		// Deactivate screen cursors
		Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

		menu.canUpdate = true;

		// Switch ScreenMode 
		menu.mode = eSpellScreenMode.usedSpell;
	}

	public void CantUseSpell(string message) {
		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.deny);

		// if Battle
		if (GameManager.S.IsBattling()) {
			Battle.S.playerActions.ButtonsDisableAll();

			// Activate display message
			Battle.S.UI.ActivateDisplayMessage();

			Battle.S.dialogue.DisplayText(message);

			// Switch Mode
			Battle.S.mode = eBattleMode.playerTurn;

            Utilities.S.RemoveListeners(menu.spellsButtons);

            menu.canUpdate = true;

            // Switch ScreenMode 
            menu.mode = eSpellScreenMode.cantUseSpell;
        } else {
			PauseMessage.S.DisplayText(message);

			SpellHelper();
        }
	}
}

public class Spell {
	public string name;
	public eSpellType type;
	public eSpellStatEffect statEffect;
	public eSpellUseableMode useableMode;
	public int statEffectMinValue;
	public int statEffectMaxValue;
	public int cost;
	public string description;
	public bool multipleTargets;

	public Spell(string spellName,
				eSpellType spellType, eSpellStatEffect spellStatEffect, eSpellUseableMode spellUseableMode,
				int spellStatEffectMinValue, int spellStatEffectMaxValue, int spellCost,
				string spellDescription, bool spellMultipleTargets = false) {
		name = spellName;
		type = spellType;
		statEffect = spellStatEffect;
		useableMode = spellUseableMode;
		statEffectMinValue = spellStatEffectMinValue;
		statEffectMaxValue = spellStatEffectMaxValue;
		cost = spellCost;
		description = spellDescription;
		multipleTargets = spellMultipleTargets;
	}
}