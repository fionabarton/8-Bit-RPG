using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// During battle, handles what happens when an item button is clicked
/// </summary>
public class BattleItems : MonoBehaviour {
	[Header("Set Dynamically")]
	int amountToHeal;
	int maxAmountToHeal;

	private Battle _;

	void Start() {
		_ = Battle.S;
	}

	public void AddFunctionToButton(Action<int, Item> functionToPass, string messageToDisplay, Item item) {
		_.UI.RemoveAllListeners();

		Utilities.S.ButtonsInteractable(_.UI.optionButtonsCS, false);
		Utilities.S.ButtonsInteractable(_.UI.enemySpriteButtonsCS, false);
		Utilities.S.ButtonsInteractable(_.UI.partyNameButtonsCS, true);

		// Set a Player Button as Selected GameObject
		Utilities.S.SetSelectedGO(_.UI.partyNameButtonsCS[0].gameObject);

		// Set previously selected GameObject
		//_.previousSelectedForAudio = _.playerActions.playerButtonGO[_.PlayerNdx()].gameObject;

		//_.dialogue.DisplayText(messageToDisplay);

		// Audio: Confirm
		AudioManager.S.PlaySFX(eSoundName.confirm);

		// Add Item Listeners to Player Buttons
		_.UI.partyNameButtonsCS[0].onClick.AddListener(delegate { functionToPass(0, item); });
		_.UI.partyNameButtonsCS[1].onClick.AddListener(delegate { functionToPass(1, item); });
		_.UI.partyNameButtonsCS[2].onClick.AddListener(delegate { functionToPass(2, item); });

		// If multiple targets
		if (!item.multipleTargets) {
			_.mode = eBattleMode.selectPartyMember;
			_.UI.SetHorizontalButtonsNavigation(_.UI.partyNameButtonsCS, Party.S.partyNdx + 1);
		} else {
			_.mode = eBattleMode.selectAll;
			_.UI.TargetAllPartyMembers();
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////
	// Heal party members
	////////////////////////////////////////////////////////////////////////////////////////

	public void Heal(int ndx, Item item, int min, int max) {
		// Get amount and max amount to heal
		amountToHeal = UnityEngine.Random.Range(min, max);
		maxAmountToHeal = Party.S.stats[ndx].maxHP - Party.S.stats[ndx].HP;

		// Cap amountToHeal to maxAmountToHeal
		if (amountToHeal >= maxAmountToHeal) {
			amountToHeal = maxAmountToHeal;
		}

		// Add to TARGET Player's HP
		GameManager.S.AddPlayerHP(ndx, amountToHeal);

		CurePlayerAnimation(ndx, true, amountToHeal);
	}

	public void HPPotion(int ndx, Item item) {
		// Activate display message
		_.UI.ActivateDisplayMessage();

		if (_.playerDead[ndx]) {
			ItemIsNotUseful(Party.S.stats[ndx].name + " is dead...\n...and dead folk can't be healed, dummy!");
			return;
		}

		// If HP is less than maxHP
		if (Party.S.stats[ndx].HP < Party.S.stats[ndx].maxHP) {
			Heal(ndx, item, item.statEffectMinValue, item.statEffectMaxValue);

			// Display Text
			if (amountToHeal >= maxAmountToHeal) {
				_.dialogue.DisplayText("Used " + item.name + "!\nHealed " + Party.S.stats[ndx].name + " back to Max HP!");
			} else {
				_.dialogue.DisplayText("Used " + item.name + "!\nHealed " + Party.S.stats[ndx].name + " for " + amountToHeal + " HP!");
			}

			ItemIsUseful(item);
		} else {
			ItemIsNotUseful(Party.S.stats[ndx].name + " already at full health...\n...no need to use this potion!");
		}
	}

	public void MPPotion(int ndx, Item item) {
		// Activate display message
		_.UI.ActivateDisplayMessage();

		if (_.playerDead[ndx]) {
			ItemIsNotUseful(Party.S.stats[ndx].name + " is dead...\n...and dead folk can't be healed, dummy!");
			return;
		}

		// If MP is less than maxMP
		if (Party.S.stats[ndx].MP < Party.S.stats[ndx].maxMP) {
			// Get amount and max amount to heal
			amountToHeal = UnityEngine.Random.Range(item.statEffectMinValue, item.statEffectMaxValue);
			maxAmountToHeal = Party.S.stats[ndx].maxMP - Party.S.stats[ndx].MP;

			// Add 12-20 MP to TARGET Player's MP
			GameManager.S.AddPlayerMP(ndx, amountToHeal);

			// Display Text
			if (amountToHeal >= maxAmountToHeal) {
				_.dialogue.DisplayText("Used " + item.name + "!\n" + Party.S.stats[ndx].name + " back to Max MP!");

				// Prevents Floating Score being higher than the acutal amount healed
				amountToHeal = maxAmountToHeal;
			} else {
				_.dialogue.DisplayText("Used " + item.name + "!\n" + Party.S.stats[ndx].name + " gained " + amountToHeal + " MP!");
			}

			CurePlayerAnimation(ndx, true, amountToHeal, false);

			ItemIsUseful(item);
		} else {
			ItemIsNotUseful(Party.S.stats[ndx].name + " already at full magic...\n...no need to use this potion!");
		}
	}

	public void HealAllPotion(int unusedIntBecauseOfAddFunctionToButtonParameter, Item item) {
		// Activate display message
		_.UI.ActivateDisplayMessage();

		int totalAmountToHeal = 0;

		if (Party.S.stats[0].HP < Party.S.stats[0].maxHP ||
			Party.S.stats[1].HP < Party.S.stats[1].maxHP ||
			Party.S.stats[2].HP < Party.S.stats[2].maxHP) {
			for (int i = 0; i < _.playerDead.Count; i++) {
				if (!_.playerDead[i]) {
					Heal(i, item, item.statEffectMinValue, item.statEffectMaxValue);

					totalAmountToHeal += amountToHeal;
				}
			}

			// Display Text
			_.dialogue.DisplayText("Used " + item.name + "!\nHealed ALL party members for an average of "
				+ Utilities.S.CalculateAverage(totalAmountToHeal, _.partyQty + 1) + " HP!");

			ItemIsUseful(item);
		} else {
			ItemIsNotUseful("The party is already at full health...\n...no need to use this potion!");
		}
	}

	public void RevivePotion(int ndx, Item item) {
		// Activate display message
		_.UI.ActivateDisplayMessage();

		if (_.playerDead[ndx]) {
			_.playerDead[ndx] = false;

			// Add to PartyQty 
			_.partyQty += 1;

			// Add Player to Turn Order
			_.turnOrder.Add(Party.S.stats[ndx].battleID);

			// Get 6-10% of max HP
			float lowEnd = Mathf.Max(1, Party.S.stats[ndx].maxHP * 0.06f);
			float highEnd = Mathf.Max(1, Party.S.stats[ndx].maxHP * 0.10f);
			Heal(ndx, item, (int)lowEnd, (int)highEnd);

			// Display Text
			if (amountToHeal >= maxAmountToHeal) {
				_.dialogue.DisplayText("Used " + item.name + "!\nHealed " + Party.S.stats[ndx].name + " back to Max HP!");
			} else {
				_.dialogue.DisplayText("Used " + item.name + "!\nHealed " + Party.S.stats[ndx].name + " for " + amountToHeal + " HP!");
			}

			ItemIsUseful(item);
		} else {
			ItemIsNotUseful(Party.S.stats[ndx].name + " ain't dead...\n...and dead folk don't need to be revived, dummy!");
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////
	// Cure status ailments
	////////////////////////////////////////////////////////////////////////////////////////

	public void DetoxifyPotion(int ndx, Item item) {
		// Activate display message
		_.UI.ActivateDisplayMessage();

		if (_.playerDead[ndx]) {
			ItemIsNotUseful(Party.S.stats[ndx].name + " is dead...\n...and dead folk don't need to be detoxified, dummy!");
			return;
		}

		if (StatusEffects.S.CheckIfPoisoned(true, ndx)) {
			// Remove poison
			StatusEffects.S.RemovePoisoned(true, ndx);

			// Display Text
			_.dialogue.DisplayText("Used " + item.name + "!\n" + Party.S.stats[ndx].name + " is no longer poisoned!");

			CurePlayerAnimation(ndx);

			ItemIsUseful(item);
		} else {
			ItemIsNotUseful(Party.S.stats[ndx].name + " is not suffering from the effects of poison...\n...no need to use this potion!");
		}
	}

	public void MobilizePotion(int ndx, Item item) {
		// Activate display message
		_.UI.ActivateDisplayMessage();

		if (_.playerDead[ndx]) {
			ItemIsNotUseful(Party.S.stats[ndx].name + " is dead...\n...and dead folk don't need their mobility restored, dummy!");
			return;
		}

		if (StatusEffects.S.CheckIfParalyzed(true, ndx)) {
			// Remove paralyzed
			StatusEffects.S.RemoveParalyzed(true, ndx);

			// Display Text
			_.dialogue.DisplayText("Used " + item.name + "!\n" + Party.S.stats[ndx].name + " is no longer paralyzed!");

			CurePlayerAnimation(ndx);

			ItemIsUseful(item);
		} else {
			ItemIsNotUseful(Party.S.stats[ndx].name + " is not suffering from the effects of paralysis...\n...no need to use this potion!");
		}
	}

	public void WakePotion(int ndx, Item item) {
		// Activate display message
		_.UI.ActivateDisplayMessage();

		if (_.playerDead[ndx]) {
			ItemIsNotUseful(Party.S.stats[ndx].name + " is dead...\n...and dead folk don't need to wake up, dummy!");
			return;
		}

		if (StatusEffects.S.CheckIfSleeping(true, ndx)) {
			// Remove sleeping
			StatusEffects.S.RemoveSleeping(true, ndx);

			// Display Text
			_.dialogue.DisplayText("Used " + item.name + "!\n" + Party.S.stats[ndx].name + " is no longer sleeping!");

			CurePlayerAnimation(ndx);

			ItemIsUseful(item);
		} else {
			ItemIsNotUseful(Party.S.stats[ndx].name + " is not sleeping...\n...no need to use this potion!");
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////
	// Helper functions
	////////////////////////////////////////////////////////////////////////////////////////

	public void DisableButtonsAndRemoveListeners() {
		//_.playerActions.ButtonsDisableAll();
		Utilities.S.ButtonsInteractable(_.UI.partyNameButtonsCS, false);
		//Utilities.S.RemoveListeners(_.UI.partyNameButtonsCS);
		_.UI.RemoveAllListeners();
	}

	public void ItemIsUseful(Item item) {
		// Remove from Inventory
		Inventory.S.RemoveItemFromInventory(item);

		// Audio: Buff 1
		AudioManager.S.PlaySFX(eSoundName.buff1);

		_.NextTurn();

		DisableButtonsAndRemoveListeners();
	}

	public void ItemIsNotUseful(string message) {
		// Display Text
		_.dialogue.DisplayText(message);

		// Deactivate Cursors
		Utilities.S.SetActiveList(_.UI.cursors, false);

		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.deny);

		// Switch Mode
		if (StatusEffects.S.HasStatusAilment(true, _.PlayerNdx())) {
			_.mode = eBattleMode.statusAilment;
		} else {
			_.mode = eBattleMode.playerTurn;
		}

		DisableButtonsAndRemoveListeners();
	}

	public void CantUseItemInBattle() {
		//Items.S.menu.Deactivate();

		// Deactivate PauseMessage
		//PauseMessage.S.gameObject.SetActive(false);

		_.playerActions.ButtonsInteractable(false, false, false, false, false, false, false, false, false, false);

		// Activate display message
		_.UI.ActivateDisplayMessage();

		_.dialogue.DisplayText("This item ain't usable in battle... sorry!");

		// Audio: Deny
		AudioManager.S.PlaySFX(eSoundName.deny);

		// Switch Mode
		_.mode = eBattleMode.playerTurn;
	}

    public void CurePlayerAnimation(int ndx, bool displayFloatingScore = false, int scoreAmount = 0, bool greenOrBlue = true) {
		//// Get and position Poof game object
		// GameObject poof = ObjectPool.S.GetPooledObject("Poof");
		// ObjectPool.S.PosAndEnableObj(poof, _.playerSprite[ndx]);

		// Display Floating Score
		if (displayFloatingScore) {
            if (greenOrBlue) {
                GameManager.S.InstantiateFloatingScore(_.UI.partyStartsTextBoxSprite[ndx].gameObject, scoreAmount.ToString(), Color.green);
            } else {
                GameManager.S.InstantiateFloatingScore(_.UI.partyStartsTextBoxSprite[ndx].gameObject, scoreAmount.ToString(), new Color32(39, 201, 255, 255));
            }
        }

		//// Set anim
		//_.playerAnimator[ndx].CrossFade("Win_Battle", 0);

		// Animation: Flicker party member
		Battle.S.partyAnims[ndx].CrossFade("Flicker", 0);
	}
}