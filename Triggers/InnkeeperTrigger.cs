using System.Collections;
using UnityEngine;

public class InnkeeperTrigger : ActivateOnButtonPress {
	[Header("Set in Inspector")]
	public string	offerMessage = "<color=yellow><Inn Keeper></color> Rooms are 10 gold a night. Restores your HP & MP. Would you like to spend the night?";
	public string	notEnoughMoneyMessage = "Begone with you, penniless fool! Waste not my worthless time!";
	public string	partyWasHealedMessage = "Health and magic restored. Bless your heart, babe!";
	public string	noMessage = "That's cool. Later, bro.";

	public int		cost = 10;

	// Sets which direction the NPC faces on start
	// 0 = right, 1 = up, 2 = left, 3 = down
	public int walkDirection;

	[Header("Set Dynamically")]
	private Animator anim;
	
	// Flip
	private bool facingRight;

    private void Start() {
		anim = GetComponent<Animator>();

		if (anim != null) {
			// Set animation based on walk direction
			SetWalkDirectionAnimation();
		}
    }

    protected override void Action() {
		// Set Camera to Innkeeper gameObject
		CamManager.S.ChangeTarget(gameObject, true);

		// Set SubMenu Text
		GameManager.S.gameSubMenu.SetText("Yes", "No");

		DialogueManager.S.DisplayText(offerMessage);

		// Face towards player
		if (anim != null) {
			FacePlayer();
		}

		// Activate Sub Menu after Dialogue 
		DialogueManager.S.activateSubMenu = true;
		// Don't activate Text Box Cursor 
		DialogueManager.S.dontActivateCursor = true;
		// Gray Out Text Box after Dialogue 
		//DialogueManager.S.grayOutTextBox = true;

		// Set OnClick Methods
		Utilities.S.RemoveListeners(GameManager.S.gameSubMenu.buttonCS);
		GameManager.S.gameSubMenu.buttonCS[0].onClick.AddListener(Yes);
		GameManager.S.gameSubMenu.buttonCS[1].onClick.AddListener(No);
		//RPG.S.gameSubMenu.subMenuButtonCS[2].onClick.AddListener(Option2);
		//RPG.S.gameSubMenu.subMenuButtonCS[3].onClick.AddListener(Option3);

		// Set button navigation
		Utilities.S.SetButtonNavigation(GameManager.S.gameSubMenu.buttonCS[0], GameManager.S.gameSubMenu.buttonCS[1], GameManager.S.gameSubMenu.buttonCS[1]);
		Utilities.S.SetButtonNavigation(GameManager.S.gameSubMenu.buttonCS[1], GameManager.S.gameSubMenu.buttonCS[0], GameManager.S.gameSubMenu.buttonCS[0]);
	}

	public void Yes() {
		if (!GameManager.S.paused) {
			AudioManager.S.PlaySFX(eSoundName.confirm);

			if (Party.S.gold >= cost) {
				// Subtract item cost from Player's Gold
				Party.S.gold -= cost;

				// Set HP/MP to max
				Party.S.stats[0].HP = Party.S.stats[0].maxHP;
				Party.S.stats[0].MP = Party.S.stats[0].maxMP;
				Party.S.stats[1].HP = Party.S.stats[1].maxHP;
				Party.S.stats[1].MP = Party.S.stats[1].maxMP;
				Party.S.stats[2].HP = Party.S.stats[2].maxHP;
				Party.S.stats[2].MP = Party.S.stats[2].maxMP;

				// Cure poison
				StatusEffects.S.playerIsPoisoned[0] = false;
				StatusEffects.S.playerIsPoisoned[1] = false;
				StatusEffects.S.playerIsPoisoned[2] = false;

				StartCoroutine("CloseCurtains");
			} else {
				// Display Text: Not enough Gold
				DialogueManager.S.DisplayText(notEnoughMoneyMessage);
			}
		}
	}

	// 1) Freeze input, close curtains
	IEnumerator CloseCurtains() {
		// Deactivate player input
		GameManager.S.paused = true;

		Curtain.S.Close();

		// Audio: Win
		StartCoroutine(AudioManager.S.PlaySongThenResumePreviousSong(6));

		// Wait for 1.5 seconds
		yield return new WaitForSeconds(1.5f);

		// Deactivate poisoned icons
		StatusEffects.S.SetOverworldPoisonIcons();

		StartCoroutine("OpenCurtains");
	}
	// 2) Open curtains
	IEnumerator OpenCurtains() {
		DialogueManager.S.DeactivateTextBox();

		// Refreeze Player
		Player.S.canMove = false;

		Curtain.S.Open();

		yield return new WaitForSeconds(0.75f);
		DisplayDialogue();
	}
	// 3) Dialogue, unfreeze input
	void DisplayDialogue() {
		// Audio: Buff 1
		AudioManager.S.PlaySFX(eSoundName.buff1);

		// Reactivate player input
		GameManager.S.paused = false;

		DialogueManager.S.ResetSettings();
		DialogueManager.S.DisplayText(partyWasHealedMessage);
	}

	public void No() {
		AudioManager.S.PlaySFX(eSoundName.deny);

		DialogueManager.S.ResetSettings();
		DialogueManager.S.DisplayText(noMessage);
	}

	private void SetWalkDirectionAnimation() {
		// Set animation
		switch (walkDirection) {
			case 1: anim.CrossFade("Walk_Up", 0); break;
			case 3: anim.CrossFade("Walk_Down", 0); break;
			case 0:
				anim.CrossFade("Walk_Side", 0);
				// Flip
				if (facingRight) { Utilities.S.Flip(gameObject, ref facingRight); }
				break;
			case 2:
				anim.CrossFade("Walk_Side", 0);
				// Flip
				if (!facingRight) { Utilities.S.Flip(gameObject, ref facingRight); }
				break;
		}
	}

	// Face direction of Player
	public void FacePlayer() {
		if (Player.S.gameObject.transform.position.x < transform.position.x &&
			!Utilities.S.isCloserHorizontally(gameObject, Player.S.gameObject)) { // Left
																				  // If facing right, flip
			if (transform.localScale.x > 0) { Utilities.S.Flip(gameObject, ref facingRight); }
			anim.Play("Walk_Side", 0, 1);
		} else if (Player.S.gameObject.transform.position.x > transform.position.x &&
			!Utilities.S.isCloserHorizontally(gameObject, Player.S.gameObject)) { // Right
																				  // If facing left, flip
			if (transform.localScale.x < 0) { Utilities.S.Flip(gameObject, ref facingRight); }
			anim.Play("Walk_Side", 0, 1);
		} else if (Player.S.gameObject.transform.position.y < transform.position.y &&
			Utilities.S.isCloserHorizontally(gameObject, Player.S.gameObject)) { // Down
			anim.Play("Walk_Down", 0, 1);
		} else if (Player.S.gameObject.transform.position.y > transform.position.y &&
			Utilities.S.isCloserHorizontally(gameObject, Player.S.gameObject)) { // Up
			anim.Play("Walk_Up", 0, 1);
		}
	}
}