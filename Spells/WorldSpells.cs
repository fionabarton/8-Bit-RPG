using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Outside of battle, handles what happens when a spell button is clicked
/// </summary>
public class WorldSpells : MonoBehaviour {
	public void AddFunctionToButton(Action<int> functionToPass, string messageToDisplay, Spell spell) {
		if (Party.S.stats[Spells.S.menu.playerNdx].MP >= spell.cost) {
			// Audio: Confirm
			AudioManager.S.PlaySFX(eSoundName.confirm);

			// Buttons Interactable
			Utilities.S.ButtonsInteractable(PauseMenu.S.playerNameButtons, true);
			Utilities.S.ButtonsInteractable(Spells.S.menu.spellsButtons, false);

			// Set Selected GameObject
			Utilities.S.SetSelectedGO(Spells.S.menu.previousSelectedPlayerGO);

			// Set previously selected GameObject
			Spells.S.menu.pickWhichMemberToHeal.previousSelectedPlayerGO = Spells.S.menu.previousSelectedPlayerGO;

			// Display Text
			PauseMessage.S.DisplayText(messageToDisplay);

			// Add Listeners
			PauseMenu.S.playerNameButtons[0].onClick.AddListener(delegate { functionToPass(0); });
			PauseMenu.S.playerNameButtons[1].onClick.AddListener(delegate { functionToPass(1); });
			PauseMenu.S.playerNameButtons[2].onClick.AddListener(delegate { functionToPass(2); });

			Spells.S.menu.canUpdate = true;
		} else {
			Spells.S.CantUseSpell("Not enough MP to cast this spell!");
			return;
		}

		// If multiple targets
		if (!spell.multipleTargets) {
			Spells.S.menu.mode = eSpellScreenMode.pickWhichMemberToHeal;
		} else {
			for (int i = 0; i <= Party.S.partyNdx; i++) {
				// Set cursor positions
				Utilities.S.PositionCursor(PauseMenu.S.playerNameButtons[i].gameObject, 0, 60, 3, i);

				// Set party animations to walk
				PauseMenu.S.playerAnims[i].CrossFade("Walk", 0);

				// Activate cursors
				ScreenCursor.S.cursorGO[i].SetActive(true);
			}

			// Set button colors
			Utilities.S.SetTextColor(PauseMenu.S.playerNameButtons, new Color32(205, 208, 0, 255));

			Spells.S.menu.mode = eSpellScreenMode.pickAllMembersToHeal;
		}
	}

	//////////////////////////////////////////////////////////
	/// Heal - Heal the selected party member 
	//////////////////////////////////////////////////////////
	public void HealSelectedPartyMember(int ndx) {
		if (Party.S.stats[ndx].HP < Party.S.stats[ndx].maxHP) {
			// Set animation to success
			PauseMenu.S.playerAnims[ndx].CrossFade("Success", 0);

			// Subtract Spell cost from CASTING Player's MP 
			GameManager.S.SubtractPlayerMP(Spells.S.menu.playerNdx, 3);

			// Add 30-45 HP to TARGET Player's HP
			int randomValue = UnityEngine.Random.Range(30, 45);
			GameManager.S.AddPlayerHP(ndx, randomValue);

			// Display Text
			if (Party.S.stats[ndx].HP >= Party.S.stats[ndx].maxHP) {
				PauseMessage.S.DisplayText("Used Heal Spell!\nHealed " + Party.S.stats[ndx].name + " back to Max HP!");
			} else {
				PauseMessage.S.DisplayText("Used Heal Spell!\nHealed " + Party.S.stats[ndx].name + " for " + randomValue + " HP!");
			}

			// Audio: Buff 1
			AudioManager.S.PlaySFX(eSoundName.buff1);
		} else {
			// Display Text
			PauseMessage.S.DisplayText(Party.S.stats[ndx].name + " already at full health...\n...no need to cast this spell!");

            // Set party animations to idle
            PauseMenu.S.SetSelectedMemberAnim("Idle");

            // Audio: Deny
            AudioManager.S.PlaySFX(eSoundName.deny);
		}
		Spells.S.SpellHelper();
	}

	//////////////////////////////////////////////////////////
	/// Detoxify - Detoxify a single party member 
	//////////////////////////////////////////////////////////
	public void DetoxifySelectedPartyMember(int ndx) {
		if (StatusEffects.S.CheckIfPoisoned(true, ndx)) {
			// Remove poison
			StatusEffects.S.RemovePoisoned(true, ndx);

			// Set animation to success
			PauseMenu.S.playerAnims[ndx].CrossFade("Success", 0);

			// If poisoned, activate overworld poisoned icons
			StatusEffects.S.SetOverworldPoisonIcons();

			// Subtract Spell cost from CASTING Player's MP 
			GameManager.S.SubtractPlayerMP(Spells.S.menu.playerNdx, 2);

			// Display Text
			PauseMessage.S.DisplayText("Used Detoxify Spell!\n" + Party.S.stats[ndx].name + " is no longer poisoned!");

			// Audio: Buff 1
			AudioManager.S.PlaySFX(eSoundName.buff1);
		} else {
			// Display Text
			PauseMessage.S.DisplayText(Party.S.stats[ndx].name + " is not suffering from the effects of poison...\n...no need to cast this spell!");

			// Set party animations to idle
			PauseMenu.S.SetSelectedMemberAnim("Idle");

			// Audio: Deny
			AudioManager.S.PlaySFX(eSoundName.deny);
		}
		Spells.S.SpellHelper();
	}

	//////////////////////////////////////////////////////////
	/// Warp
	//////////////////////////////////////////////////////////
	public void WarpSpell() {
		if (Party.S.stats[0].MP >= 1) {
			// Subtract Spell cost from CASTING Player's MP 
			GameManager.S.SubtractPlayerMP(Spells.S.menu.playerNdx, 1);

			Spells.S.menu.mode = eSpellScreenMode.pickWhereToWarp;

			// Set Selected GameObject
			Utilities.S.SetSelectedGO(Spells.S.menu.spellsButtons[0].gameObject);

			// Set previously selected GameObject
			WarpManager.S.previousSelectedLocationGO = Spells.S.menu.spellsButtons[0].gameObject;

			// Use SpellScreen's buttons to select/display warp locations
			WarpManager.S.DeactivateUnusedButtonSlots(Spells.S.menu.spellsButtons);
			WarpManager.S.AssignButtonEffect(Spells.S.menu.spellsButtons);
			WarpManager.S.AssignButtonNames(Spells.S.menu.spellsButtonNameText);
			WarpManager.S.SetButtonNavigation(Spells.S.menu.spellsButtons);

			// Audio: Confirm
			AudioManager.S.PlaySFX(eSoundName.confirm);
		} else {
			Spells.S.CantUseSpell("Not enough MP to cast this spell!");
		}
	}

	//////////////////////////////////////////////////////////
	/// Heal All - Heal all party members 
	//////////////////////////////////////////////////////////
	public void HealAllPartyMembers(int unusedIntBecauseOfAddFunctionToButtonParameter = 0) {
		int totalAmountToHeal = 0;

		if (Party.S.stats[0].HP < Party.S.stats[0].maxHP ||
			Party.S.stats[1].HP < Party.S.stats[1].maxHP ||
			Party.S.stats[2].HP < Party.S.stats[2].maxHP) {
			// Subtract Spell cost from Player's MP
			GameManager.S.SubtractPlayerMP(Spells.S.menu.playerNdx, 6);

			for (int i = 0; i < Party.S.stats.Count; i++) {
				// Get amount and max amount to heal
				int amountToHeal = UnityEngine.Random.Range(12, 20);
				int maxAmountToHeal = Party.S.stats[i].maxHP - Party.S.stats[i].HP;
				// Add Player's WIS to Heal Amount
				amountToHeal += Party.S.stats[i].WIS;

				// Add 12-20 HP to TARGET Player's HP
				GameManager.S.AddPlayerHP(i, amountToHeal);

				// Cap amountToHeal to maxAmountToHeal
				if (amountToHeal >= maxAmountToHeal) {
					amountToHeal = maxAmountToHeal;
				}

				totalAmountToHeal += amountToHeal;
			}

			// Display Text
			PauseMessage.S.DisplayText("Used Heal All Spell!\nHealed ALL party members for an average of "
				+ Utilities.S.CalculateAverage(totalAmountToHeal, Party.S.stats.Count) + " HP!");

            // Set animations to success
            for (int i = 0; i <= Party.S.partyNdx; i++) {
				PauseMenu.S.playerAnims[i].CrossFade("Success", 0);
			}

            // Audio: Buff 1
            AudioManager.S.PlaySFX(eSoundName.buff1);
		} else {
			// Display Text
			PauseMessage.S.DisplayText("The party is already at full health...\n...no need to cast this spell!");

            // Set party animations to idle
            for (int i = 0; i <= Party.S.partyNdx; i++) {
                PauseMenu.S.playerAnims[i].CrossFade("Idle", 0);
			}

            // Audio: Deny
            AudioManager.S.PlaySFX(eSoundName.deny);
		}

		// Deactivate screen cursors
		Utilities.S.SetActiveList(ScreenCursor.S.cursorGO, false);

		Spells.S.SpellHelper();
	}
}