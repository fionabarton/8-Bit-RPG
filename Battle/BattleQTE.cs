using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleQTE : MonoBehaviour {
	[Header("Set in Inspector")]
	// QTE Progress bar
	public ProgressBar healthBar;

	public List<Animator> QTEInputSprites = new List<Animator>();

	[Header("Set Dynamically")]
	// QTE Mode/Type
	public int qteType = 2; // 0: Mash, 1: Hold, 2: Sequence, 3: Stop, 4: Block

	// Max value of progress bar
	const float max = 100;
	// Current value of progress bar
	public float val = 50;

	// Downward force applied to the progress bar
	public int downwardForce = 20;
	// Force player can apply to value progress bar 
	public int playerForce = 500;

	///////////////////////////////// QTE HOLD /////////////////////////////////
	public bool buttonDown = false;

	///////////////////////////////// QTE SEQUENCE /////////////////////////////////
	// Amount of commands to press
	int inputAmount = 1;

	// Timer
	public float tooLateTime = 0;
	public float tooLateTimeDone = 0;

	// Strings 
	public string inputString = "";
	public string goalString = "";

	// Helps determine if a horizontal or vertical axis was pressed down during a frame
	bool horizontalAxisIsInUse, verticalAxisIsInUse;

	///////////////////////////////// QTE STOP /////////////////////////////////
	bool barIsDecreasing;

	///////////////////////////////// QTE BLOCK /////////////////////////////////
	// Index of the party member that is blocking
	public int blockerNdx;

	private Battle _;

	void Start() {
		_ = Battle.S;
	}

	// Start QTE: switch mode & provide instructions to user
	public void StartQTE() {
		// Switch mode
		_.mode = eBattleMode.qteInitialize;

		// Reset points
		_.qteBonusDamage = 0;

		// Select QTE Mode/Type
		//qteType = Random.Range(0, 4);
		//qteType = 2;
		switch (_.PlayerNdx()) {
			case 0: // Blob: MASH
				qteType = 0;
				break;
			case 1: // Bill: SEQUENCE
				qteType = 2;
				break;
			case 2: // Fake Bill: HOLD
				qteType = 1;
				break;
		}

		// Provide instructions to player
		switch (qteType) {
			case 0:
				_.dialogue.displayMessageTextTop.text = "Get ready to MASH!";
				_.dialogue.DisplayText("MASH the action button 'til the progress bar is completely full!");
				break;
			case 1:
				_.dialogue.displayMessageTextTop.text = "Get ready to HOLD!";
				_.dialogue.DisplayText("HOLD the action button DOWN 'til the progress bar is nearly full... then LET GO!");
				break;
			case 2:
				_.dialogue.displayMessageTextTop.text = "Get ready to SEQUENCE!";
				_.dialogue.DisplayText("Before time's up, INPUT the following sequence of directions!");
				break;
			case 3:
				_.dialogue.displayMessageTextTop.text = "Get ready to STOP!";
				_.dialogue.DisplayText("WAIT until the progress bar is nearly full, then press the action button!");
				break;
		}
	}

	// Reset or initialize fields
	public void Initialize() {
		// Enable progress bar
		healthBar.gameObject.transform.parent.gameObject.SetActive(true);

		switch (qteType) {
			case 0: /////////// MASH ///////////
					// Reset 
				val = 50;
				playerForce = 500;
				downwardForce = 20;

				// Activate sprite gameObject
				QTEInputSprites[0].gameObject.SetActive(true);
				QTEInputSprites[0].CrossFade("QTEInputSprite_Button_Press", 0);

				// Audio: Confirm
				AudioManager.S.PlaySFX(eSoundName.confirm);

				// Display Text
				_.dialogue.displayMessageTextTop.text = "<color=#FF0000FF>MASH</color>\nTHAT BUTTON!";
				break;
			case 1: /////////// HOLD ///////////
					// Reset 
				val = 0;
				playerForce = 75;
				buttonDown = false;

				// Activate sprite gameObject
				QTEInputSprites[0].gameObject.SetActive(true);
				QTEInputSprites[0].CrossFade("QTEInputSprite_Button_Press", 0);

				// Audio: Confirm
				AudioManager.S.PlaySFX(eSoundName.confirm);

				// Display Text
				_.dialogue.displayMessageTextTop.text = "<color=#FF0000>HOLD</color>\nTHAT BUTTON!";
				break;
			case 2: /////////// SEQUENCE ///////////
					// Reset Strings
				_.dialogue.displayMessageTextTop.text = "";
				inputString = "";
				goalString = "";

				// Set Timer
				tooLateTime = 2;
				tooLateTimeDone = Time.time + tooLateTime;

				inputAmount = 3;

				// Audio: Confirm
				AudioManager.S.PlaySFX(eSoundName.confirm);

				// Set Goal
				StartCoroutine("SetGoals", inputAmount);
				break;
			case 3: /////////// STOP ///////////
					// Reset settings
				val = 0;
				downwardForce = 100;

				// Activate sprite gameObject
				QTEInputSprites[0].gameObject.SetActive(true);
				QTEInputSprites[0].CrossFade("QTEInputSprite_Button_Press", 0);

				// Audio: Confirm
				AudioManager.S.PlaySFX(eSoundName.confirm);

				// Display Text
				_.dialogue.displayMessageTextTop.text = "<color=#FF0000>STOP</color>\nTHAT BUTTON!";
				break;
			case 4: /////////// BLOCK ///////////
					// Activate Text
				//_.dialogue.displayMessageTextTop.gameObject.transform.parent.gameObject.SetActive(true);

				// Reset Strings
				//_.dialogue.displayMessageTextTop.text = "";
				inputString = "";
				goalString = "";

				// Set Goal (only ONE input)
				int directionToType = Random.Range(0, 4);
				goalString += directionToType.ToString();
				//_.dialogue.displayMessageTextTop.text = "Press " + ConvertDirections(goalString[0]) + " to\nBLOCK!";
				//_.dialogue.DisplayText("Press " + ConvertDirections(goalString[0]) + " to\nBLOCK!");

				// Set Timer
				tooLateTime = _.enemyStats[_.EnemyNdx()].timeToQTEBlock;
				tooLateTimeDone = Time.time + tooLateTime;

				// Display Arrow Sprite
				QTEInputSprites[0].gameObject.SetActive(true);
				switch (directionToType) {
					case 0: QTEInputSprites[0].CrossFade("Arrow_Right", 0); break;
					case 1: QTEInputSprites[0].CrossFade("Arrow_Up", 0); break;
					case 2: QTEInputSprites[0].CrossFade("Arrow_Left", 0); break;
					case 3: QTEInputSprites[0].CrossFade("Arrow_Down", 0); break;
				}
				break;
		}
	}

	public void Loop() {
		switch (_.mode) {
			case eBattleMode.qteInitialize:
				if (Input.GetButtonDown("SNES B Button")) {
					Initialize();

					// Animation: QTE CHARGE
					//_.playerAnimator[_.animNdx].CrossFade("QTE_Charge", 0);

					_.mode = eBattleMode.qte;
				}
				break;
			case eBattleMode.qte:
				switch (qteType) {
					case 0: /////////// MASH ///////////
							// Increase bar while held down
						if (Input.GetButtonDown("SNES B Button")) {
							val += (playerForce * Time.fixedDeltaTime);

							// Audio:
							float tVal2 = val * 0.01f;
							AudioManager.S.sfxCS[0].pitch = tVal2;
							AudioManager.S.PlaySFX(eSoundName.dialogue);
						}
						break;
					case 1: /////////// HOLD ///////////	
						if (!buttonDown) {
							// Start holding button down
							if (Input.GetButton("SNES B Button")) {
								buttonDown = true;
							}
						} else {
							// If user released button, check result
							if (Input.GetButtonUp("SNES B Button")) {
								if (val >= 50) {
									Result(true);
								} else {
									Result(false);
								}
							}
						}
						break;
					case 2: /////////// SEQUENCE ///////////	
						DirectionalButtonsDown();
						break;
					case 3: /////////// STOP ///////////
							// Stop cursor
						if (Input.GetButtonDown("SNES B Button")) {
							if (val >= 50) {
								Result(true);
							} else {
								Result(false);
							}
						}
						break;
				}
				break;
		}
	}

	// Separate loop for blocking: Accept input to block regardless of _.dialogue.dialogueFinished
	public void BlockLoop() {
		DirectionalButtonsDown();
	}

	// Update timer/health bar
	public void FixedLoop() {
		switch (_.mode) {
			case eBattleMode.qte:
				switch (qteType) {
					case 0: /////////// MASH ///////////
							// Update health bar UI
						healthBar.UpdateBar(val, max);

						//// Audio:
						//float tVal2 = val * 0.01f;
						//AudioManager.S.sfxCS[0].pitch = tVal2;
						//AudioManager.S.PlaySFX(0);

						// Decrease bar
						val -= (downwardForce * Time.fixedDeltaTime);

						// Win.. or LOSE!
						if (val >= max) {
							Result(true);
						} else if (val <= 0) {
							Result(false);
						}
						break;
					case 1: /////////// HOLD ///////////	
							// Update health bar UI
						healthBar.UpdateBar(val, max);

						// Audio:
						float tVal = val * 0.01f;
						AudioManager.S.sfxCS[0].pitch = tVal;
						AudioManager.S.PlaySFX(eSoundName.dialogue);

						if (buttonDown) {
							if (val <= max + 1) {
								// Increase bar while held down
								val += playerForce * Time.fixedDeltaTime;
							}
							// Held too long! BAD!
							else {
								Result(false);
							}
						}
						break;
					case 2: /////////// SEQUENCE ///////////				
					case 4: /////////// BLOCK ///////////	
							// Update health bar UI
						float timeLeft = tooLateTimeDone - Time.time;
						healthBar.UpdateBar(timeLeft, tooLateTime);

						// Time's up!
						if (Time.time >= tooLateTimeDone) {
							Result(false);
						}
						break;
					case 3: /////////// STOP ///////////
							// Increase or decrease bar
						if (barIsDecreasing) {
							val -= (downwardForce * Time.fixedDeltaTime);
						} else {
							val += (downwardForce * Time.fixedDeltaTime);
						}

						// Update health bar UI
						healthBar.UpdateBar(val, max);

						// Audio:
						float tVal1 = val * 0.01f;
						AudioManager.S.sfxCS[0].pitch = tVal1;
						AudioManager.S.PlaySFX(eSoundName.dialogue);

						// Reached limit, so change direction
						if (val >= max) {
							barIsDecreasing = true;
						} else if (val <= 0) {
							barIsDecreasing = false;
						}
						break;
				}
				break;
		}
	}

	// Handle GetButtonDown-like directional input 
	void DirectionalButtonsDown() {
		// Vertical axis input
		if (Input.GetAxisRaw("Vertical") == 0) {
			verticalAxisIsInUse = false;
		} else {
			if (Input.GetAxisRaw("Vertical") > 0) {
				if (!verticalAxisIsInUse) {
					BuildInputString("1");
					verticalAxisIsInUse = true;
				}
			} else if (Input.GetAxisRaw("Vertical") < 0) {
				if (!verticalAxisIsInUse) {
					BuildInputString("3");
					verticalAxisIsInUse = true;
				}
			}
		}

		// Horizontal axis input
		if (Input.GetAxisRaw("Horizontal") == 0) {
			horizontalAxisIsInUse = false;
		} else {
			if (Input.GetAxisRaw("Horizontal") > 0) {
				if (!horizontalAxisIsInUse) {
					BuildInputString("0");
					horizontalAxisIsInUse = true;
				}
			} else if (Input.GetAxisRaw("Horizontal") < 0) {
				if (!horizontalAxisIsInUse) {
					BuildInputString("2");
					horizontalAxisIsInUse = true;
				}
			}
		}
	}

	// End QTE: Result of the user's QTE performance
	public void Result(bool goodOrBad) {
		// Audio: Reset pitch
		AudioManager.S.sfxCS[0].pitch = 1;

		// Deactivate progress bar
		healthBar.gameObject.transform.parent.gameObject.SetActive(false);

		// Deactivate all input sprites
		for (int i = 0; i < QTEInputSprites.Count; i++) {
			QTEInputSprites[i].gameObject.SetActive(false);
		}

		// Activate enemy sprites
		for (int i = 0; i < _.enemyAmount; i++) {
			_.enemySprites[i].SetActive(true);
		}

		// Reset first input sprite position (for blocking)
		//QTEInputSprites[0].gameObject.transform.position = new Vector2(0, 2.7f);
		QTEInputSprites[0].gameObject.transform.localPosition = new Vector2(0, 0.5f);

		// Floating score to indicate bonus points

		if (goodOrBad) {
			if (qteType != 4) {
				// POSITIVE Result Message
				//_.dialogue.displayMessageTextTop.text = "<color=#00FF00>NICE!</color>";
				_.dialogue.DisplayText("<color=#00FF00>NICE!</color>");

				// Animation: QTE SUCCESS
				//_.playerAnimator[_.animNdx].CrossFade("QTE_Success", 0);
			}

			// Calculate bonus damage
			switch (qteType) {
				case 0: /////////// MASH ///////////
					_.qteBonusDamage = (int)1.5f * Party.S.stats[_.PlayerNdx()].LVL;
					break;
				case 1: /////////// HOLD ///////////
				case 3: /////////// STOP ///////////
						// Divide Bonus Damage into thirds
					int bonus = (int)1.5f * Party.S.stats[_.PlayerNdx()].LVL / 3;

					if (val >= 90 && val <= 100) {
						if (bonus < 3) {
							_.qteBonusDamage = 3;
						} else {
							_.qteBonusDamage = bonus;
						}
					} else if (val >= 75 && val <= 90) {
						if (bonus < 2) {
							_.qteBonusDamage = 2;
						} else {
							_.qteBonusDamage = bonus;
						}

					} else if (val >= 50 && val <= 75) {
						if (bonus < 1) {
							_.qteBonusDamage = 1;
						} else {
							_.qteBonusDamage = bonus;
						}
					}
					break;
				case 2: /////////// SEQUENCE ///////////	
					_.qteBonusDamage = (int)(1.5f * Party.S.stats[_.PlayerNdx()].LVL);
					break;
				case 4: /////////// BLOCK ///////////	
						// Calculate HP bonus 
					int amountToHeal = (int)(1.5f * Party.S.stats[blockerNdx].LVL);

					// Add HP to Player that is blocking
					GameManager.S.AddPlayerHP(blockerNdx, amountToHeal);

					// Get and position Poof game object
					//GameObject poof = ObjectPool.S.GetPooledObject("Poof");
					//ObjectPool.S.PosAndEnableObj(poof, _.playerSprite[blockerNdx]);

					// Set mini party member animations
					_.UI.miniPartyAnims[blockerNdx].CrossFade("Success", 0);

					// Display Floating Score
					GameManager.S.InstantiateFloatingScore(_.UI.partyStartsTextBoxSprite[blockerNdx].gameObject, amountToHeal.ToString(), Color.green);

                    // Audio: Confirm
                    AudioManager.S.PlaySFX(eSoundName.confirm);
					break;
			}
		} else {
			if (qteType != 4) {
				// NEGATIVE Result Message
				//_.dialogue.displayMessageTextTop.text = "<color=#FF0000FF>FAIL!</color>";
				_.dialogue.DisplayText("<color=#FF0000FF>FAIL!</color>");

				// Animation: QTE FAIL
				//_.playerAnimator[_.animNdx].CrossFade("QTE_Fail", 0);

				// Activate "..." Word Bubble
				//_.dotDotDotWordBubble.SetActive(true);
			}
		}

		if (qteType != 4) {
			// Attack Enemy with bonus damage
			_.playerActions.AttackEnemy(_.targetNdx);
		} else {
			// Deactivate Battle Text
			//_.dialogue.displayMessageTextTop.gameObject.transform.parent.gameObject.SetActive(false);

			_.NextTurn();
		}
	}

	///////////////////////////////// QTE SEQUENCE /////////////////////////////////
	// Convert direction char (0, 1, etc.) to string ("Right", "Up", etc.)
	string ConvertDirections(char letter) {
		string word = "";
		switch (letter) {
			case '0': word = "Right"; break;
			case '1': word = "Up"; break;
			case '2': word = "Left"; break;
			case '3': word = "Down"; break;
		}
		return word;
	}

	// Get a random string of directions and stagger the text
	IEnumerator SetGoals(int inputAmount) {
		// build random goal string of directions
		for (int i = 0; i < inputAmount; i++) {
			int directionToType = Random.Range(0, 4);
			goalString += directionToType.ToString();
		}

		// stagger display of UI text
		for (int i = 0; i < goalString.Length; i++) {
			_.dialogue.displayMessageTextTop.text += ConvertDirections(goalString[i]);

			ActivateInputSprites(i, goalString[i]);

			yield return new WaitForSeconds(0.25f);
		}
	}

	void ActivateInputSprites(int spriteNdx, int directionNdx) {
		// Activate sprite gameObject
		QTEInputSprites[spriteNdx].gameObject.SetActive(true);

		// Set animation
		switch (directionNdx - 48) {
			case 0: QTEInputSprites[spriteNdx].CrossFade("Arrow_Right", 0); break;
			case 1: QTEInputSprites[spriteNdx].CrossFade("Arrow_Up", 0); break;
			case 2: QTEInputSprites[spriteNdx].CrossFade("Arrow_Left", 0); break;
			case 3: QTEInputSprites[spriteNdx].CrossFade("Arrow_Down", 0); break;
		}

		// Set sprite positions
		switch (spriteNdx) {
			case 0:
				QTEInputSprites[0].gameObject.transform.position = new Vector2(0, 2.7f);
				break;
			case 1:
				QTEInputSprites[0].gameObject.transform.position = new Vector2(-0.375f, 2.7f);
				QTEInputSprites[1].gameObject.transform.position = new Vector2(0.375f, 2.7f);
				break;
			case 2:
				QTEInputSprites[0].gameObject.transform.position = new Vector2(-0.75f, 2.7f);
				QTEInputSprites[1].gameObject.transform.position = new Vector2(0, 2.7f);
				QTEInputSprites[2].gameObject.transform.position = new Vector2(0.75f, 2.7f);
				break;
		}
	}

	// Build a string of directions based off user input and check if it matches the goal
	void BuildInputString(string buttonName) {
		StopAllCoroutines();

		// Activate all input sprites
		for (int i = 0; i < goalString.Length; i++) {
			ActivateInputSprites(i, goalString[i]);
		}

		// Add to inputString
		inputString += buttonName;

		// Check inputString against goalString if they're same length 
		if (inputString.Length >= goalString.Length) {
			if (inputString == goalString) {
				Result(true);
			} else {
				Result(false);
			}
		}
		// Check last element of inputString against last element of goalString 
		else {
			switch (inputAmount) {
				case 2:
					if (inputString[0] != goalString[0]) { // 1st char of string
						Result(false);
					} else {
						//_.dialogue.displayMessageTextTop.text = "<color=#00FF00>" + ConvertDirections(goalString[0]) + "</color>" + ConvertDirections(goalString[1]);
						_.dialogue.DisplayText("<color=#00FF00>" + ConvertDirections(goalString[0]) + "</color>" + ConvertDirections(goalString[1]));

						// Audio: Confirm
						AudioManager.S.PlaySFX(eSoundName.confirm);
					}
					break;
				case 3:
					switch (inputString.Length) {
						case 1:
							if (inputString[0] != goalString[0]) { // 1st char of string
								Result(false);
							} else {
								//_.dialogue.displayMessageTextTop.text = "<color=#00FF00>" + ConvertDirections(goalString[0]) + "</color>" + ConvertDirections(goalString[1]) + ConvertDirections(goalString[2]);
								_.dialogue.DisplayText("<color=#00FF00>" + ConvertDirections(goalString[0]) + "</color>" + ConvertDirections(goalString[1]) + ConvertDirections(goalString[2]));

								// Audio: Confirm
								AudioManager.S.PlaySFX(eSoundName.confirm);
							}
							break;
						case 2:
							if (inputString[1] != goalString[1]) { // 2nd char of string
								Result(false);
							} else {
								//_.dialogue.displayMessageTextTop.text = "<color=#00FF00>" + ConvertDirections(goalString[0]) + ConvertDirections(goalString[1]) + "</color>" + ConvertDirections(goalString[2]);
								_.dialogue.DisplayText("<color=#00FF00>" + ConvertDirections(goalString[0]) + ConvertDirections(goalString[1]) + "</color>" + ConvertDirections(goalString[2]));

								// Audio: Confirm
								AudioManager.S.PlaySFX(eSoundName.confirm);
							}
							break;
					}
					break;
			}
		}
	}
}