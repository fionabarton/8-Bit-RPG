using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Manages input, animation, etc. for the main character amongst other related functions
public class Blob : MonoBehaviour {
	[Header("Set in Inspector")]
	public GameObject	playerTriggerGO;
	public Transform	movePoint;
	public LayerMask	bounds;

	[Header("Set Dynamically")]
	public Animator			anim;
	public SpriteRenderer	sRend;
	public Flicker			flicker;
	public EnemyManager		enemyManager;
	public Followers		followers;

	const float			walkSpeed = 3f;
	const float			runSpeed = 6f;
	private float		speed = walkSpeed;
	private bool		facingRight = true;
	private static bool exists;
	public int			lastDirection;
	public bool			canMove = true;
	public float		destination;
	public bool			alreadyTriggered; // Prevents triggering multiple triggers

	// Battle variables
	public bool			isBattling = false;
	public bool			canEncounter = true;
	public int			encounterRate = 24;
	public List<EnemyStats> enemyStats;
	public int			enemyAmount = 999; // Amount of enemies to battle. If 999, set to a random amount
	public int			locationNdx = 0;
	public int			stepsUntilEncounter = 10;

	public int			stepCount = 0;

	public bool			hasRunningShoes = true;

	private static Blob _S;
	public static Blob S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;

		// DontDestroyOnLoad
		if (!exists) {
			exists = true;
			DontDestroyOnLoad(transform.gameObject);
		} else {
			Destroy(gameObject);
		}

		// Add Loop() and FixedLoop() to UpdateManager
		UpdateManager.updateDelegate += Loop;
		UpdateManager.fixedUpdateDelegate += FixedLoop;

		anim = GetComponent<Animator>();
		sRend = GetComponent<SpriteRenderer>();
		flicker = GetComponent<Flicker>();
		enemyManager = GetComponent<EnemyManager>();
		followers = GetComponent<Followers>();
	}

    void Start() {
		movePoint.parent = null;
		followers.followerMovePoints[0].parent = null;
		followers.followerMovePoints[1].parent = null;
	}

    void Loop() {
        if (canMove) {
			// If B button down, double speed
            if (hasRunningShoes) {
                if (Input.GetButton("SNES Y Button")) {
					speed = runSpeed;
				} else {
					speed = walkSpeed;
				}
            }

			// Move Blob and its followers towards their movePoints
			transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
			followers.followersGO[0].transform.position = Vector3.MoveTowards(followers.followersGO[0].transform.position, followers.followerMovePoints[0].position, speed * Time.deltaTime);
			followers.followersGO[1].transform.position = Vector3.MoveTowards(followers.followersGO[1].transform.position, followers.followerMovePoints[1].position, speed * Time.deltaTime);

			// If gameObject is on movePoint
			if (Vector3.Distance(transform.position, movePoint.position) == 0f) {
				// Horizontal input detected
				if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f) {
					// If potential new movePoint position doesn't overlap with any bounds...
					if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal") / 2f, 0f, 0f), 0.2f, bounds)) {
						// Cache and set followers' move points and facingRights
						followers.SetFollowerMovePoints(movePoint, facingRight);

						// Reposition Blob's movePoint horizontally
						movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal") / 2f, 0f, 0f);

						stepCount += 1;

						// Check for random encounter 
						CheckForRandomEncounter();

						CheckForPoisonDamage();

						// Cache and set all follower's animations
						followers.SetFollowerAnimations("Walk_Side");

						// Set the order in layer for all party members based on their y-pos
						followers.SetOrderInLayer();
					}

					// Set trigger position
					Utilities.S.SetLocalPosition(playerTriggerGO, 0.375f, 0);

					// Set Blob's animation
					anim.CrossFade("Walk_Side", 0);

				// Vertical input detected
				} else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f) {
					// If potential new movePoint position doesn't overlap with any bounds...
					if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical") / 2f, 0f), 0.2f, bounds)) {
						// Cache and set followers' move points and facingRights
						followers.SetFollowerMovePoints(movePoint, facingRight);

						// Reposition Blob's movePoint vertically
						movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical") / 2f, 0f);

						stepCount += 1;

						// Check for random encounter 
						CheckForRandomEncounter();

						CheckForPoisonDamage();

						if (Input.GetAxisRaw("Vertical") > 0) {
							// Cache and set all follower's animations
							followers.SetFollowerAnimations("Walk_Up");
						} else {
							// Cache and set all follower's animations
							followers.SetFollowerAnimations("Walk_Down");
						}

						// Set the order in layer for all party members based on their y-pos
						followers.SetOrderInLayer();
					}

					if (Input.GetAxisRaw("Vertical") > 0) {
						// Set trigger position
						Utilities.S.SetLocalPosition(playerTriggerGO, 0, 0.375f);

						// Set Blob's animation
						anim.CrossFade("Walk_Up", 0);
					} else {
						// Set trigger position
						Utilities.S.SetLocalPosition(playerTriggerGO, 0, -0.375f);

						// Set Blob's animation
						anim.CrossFade("Walk_Down", 0);
					}
				}
			}
		}
	}

	void FixedLoop() {
		if (canMove) {
			// Flip scale
			if (Input.GetAxisRaw("Horizontal") > 0 && !facingRight ||
				Input.GetAxisRaw("Horizontal") < 0 && facingRight) {
				Utilities.S.Flip(gameObject, ref facingRight);
			}
		}
	}

	// Check for random encounter 
	void CheckForRandomEncounter() {
		//if (canEncounter) {
		//	if(Random.Range(0, encounterRate) == 0) {
		//		// Start battle
		//		StartCoroutine("StartBattle");
		//	}
		//}

		if (canEncounter) {
			if (stepsUntilEncounter > 0) {
				stepsUntilEncounter -= 1;
			} else {
				stepsUntilEncounter = Random.Range(10, 100);

				// Start battle
				StartCoroutine("StartBattle");
			}
		}
	}

	public IEnumerator StartBattle() {
		// Set enemy stats
		Battle.S.ImportEnemyStats(enemyStats, enemyAmount);

		// Freeze player
		canMove = false;
		alreadyTriggered = true;

		// Set followers' movePoints to their current position
		followers.followerMovePoints[0].position = followers.followersGO[0].transform.position;
		followers.followerMovePoints[1].position = followers.followersGO[1].transform.position;

		// Close curtain
		Curtain.S.Close();

		// Audio: Start Battle
		AudioManager.S.PlaySong(eSongName.startBattle);

		isBattling = true;

		// Yield
		yield return new WaitForSeconds(1.5f);

		// Activate battle UI and gameobjects
		Battle.S.UI.battleMenu.SetActive(true);
		Battle.S.UI.battleGameObjects.SetActive(true);

		Battle.S.InitializeBattle();

		// Open curtain
		Curtain.S.Open();

		// Add Update & Fixed Update Delegate
		UpdateManager.updateDelegate += Battle.S.Loop;
		UpdateManager.fixedUpdateDelegate += Battle.S.FixedLoop;

		// Audio: Ninja
		AudioManager.S.PlaySong(eSongName.ninja);
	}

	void CheckForPoisonDamage() {
		// Every 4 steps...
		if (!flicker.isInvincible) {
			if (stepCount % 4 == 0) {
				// For each party member...
				for (int i = 0; i <= Party.S.partyNdx; i++) {
					// If poisoned...
					if (StatusEffects.S.CheckIfPoisoned(true, i)) {
						// ...decrement HP by 1
						if (Party.S.stats[i].HP > 1) {
							Party.S.stats[i].HP -= 1;
						}

						// Audio: Damage
						AudioManager.S.PlayRandomDamageSFX();

						// Start flickering
						flicker.StartInvincibility(0.5f, 0.1f, false);

						// Display Floating Score
						GameManager.S.InstantiateFloatingScore(gameObject, "-1", Color.red);
					}
				}
			}
		}
	}
}