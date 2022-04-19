using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Blob : MonoBehaviour {
	[Header("Set in Inspector")]
	public GameObject	playerTriggerGO;
	public Transform	movePoint;
	public LayerMask	bounds;

	// Follower variables
	public List<GameObject> followers;
	public List<Transform>	followerMovePoints;
	public List<Animator>	followerAnims;

	// Variables for getting/setting the order in layer for all party members
	public List<Transform>		partyTransforms;
	public List<SpriteRenderer> partySRends;

	[Header("Set Dynamically")]
	public Animator			anim;
	public SpriteRenderer	sRend;
	public Flicker			flicker;

	// Follower variables
	public List<Vector3>	movePoints;
	public List<string>		animations;
	public List<bool>		facingRights;

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
	}

    void Start() {
		movePoint.parent = null;
		followerMovePoints[0].parent = null;
		followerMovePoints[1].parent = null;
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
			followers[0].transform.position = Vector3.MoveTowards(followers[0].transform.position, followerMovePoints[0].position, speed * Time.deltaTime);
			followers[1].transform.position = Vector3.MoveTowards(followers[1].transform.position, followerMovePoints[1].position, speed * Time.deltaTime);

			// If gameObject is on movePoint
			if (Vector3.Distance(transform.position, movePoint.position) == 0f) {
				// Horizontal input detected
				if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f) {
					// If potential new movePoint position doesn't overlap with any bounds...
					if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal") / 2f, 0f, 0f), 0.2f, bounds)) {
						// Cache and set followers' move points and facingRights
						SetFollowerMovePoints();

						// Reposition Blob's movePoint horizontally
						movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal") / 2f, 0f, 0f);

						stepCount += 1;

						// Check for random encounter 
						CheckForRandomEncounter();

						CheckForPoisonDamage();
					}

					// Set trigger position
					Utilities.S.SetLocalPosition(playerTriggerGO, 0.375f, 0);

					// Cache and set all party member's animations
					SetFollowerAnimations("Walk_Side");

					// Set the order in layer for all party members based on their y-pos
					SetOrderInLayer();

				// Vertical input detected
				} else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f) {
					// If potential new movePoint position doesn't overlap with any bounds...
					if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical") / 2f, 0f), 0.2f, bounds)) {
						// Cache and set followers' move points and facingRights
						SetFollowerMovePoints();

						// Reposition Blob's movePoint vertically
						movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical") / 2f, 0f);

						stepCount += 1;

						// Check for random encounter 
						CheckForRandomEncounter();

						CheckForPoisonDamage();
					}

					if (Input.GetAxisRaw("Vertical") > 0) {
						// Set trigger position
						Utilities.S.SetLocalPosition(playerTriggerGO, 0, 0.375f);

						// Cache and set all party member's animations
						SetFollowerAnimations("Walk_Up");
					} else {
						// Set trigger position
						Utilities.S.SetLocalPosition(playerTriggerGO, 0, -0.375f);

						// Cache and set all party member's animations
						SetFollowerAnimations("Walk_Down");
					}

					// Set the order in layer for all party members based on their y-pos
					SetOrderInLayer();
				}
			}
		}
	}

	// Cache and set followers' move points 
	void SetFollowerMovePoints() {
		// Cache move point and facingRight
		movePoints.Insert(0, movePoint.position);
		facingRights.Insert(0, facingRight);

		if (movePoints.Count > 3) {
			// Set follower's movePoint pos
			followerMovePoints[1].position = movePoints[3];

			// Set followers' facing 
			SetFollowerFacing(1);

			// Remove from lists
			movePoints.RemoveAt(movePoints.Count - 1);
			facingRights.RemoveAt(facingRights.Count - 1);
		}
		if (movePoints.Count > 1) { 
			// Set follower's movePoint pos 
			followerMovePoints[0].position = movePoints[1];

			// Set followers' facing
			SetFollowerFacing(0);
		}
	}

	// Set followers' facing
	void SetFollowerFacing(int ndx) {
		if (facingRights[ndx]) {
			Utilities.S.SetScale(followers[ndx], 1, followers[ndx].transform.localScale.y);
		} else {
			Utilities.S.SetScale(followers[ndx], -1, followers[ndx].transform.localScale.y);
		}
	}

	// Cache and set animations for all party members
	void SetFollowerAnimations(string animationToAdd) {
		// Set Blob's animation
		anim.CrossFade(animationToAdd, 0);

		// Cache animation to add
		animations.Insert(0, animationToAdd);

		// Set followers' animations
		if (animations.Count > 3) { 
			followerAnims[1].CrossFade(animations[3], 0);
			animations.RemoveAt(animations.Count - 1);
		}
		if (animations.Count > 1) { 
			followerAnims[0].CrossFade(animations[1], 0);
		}
	}

	// Set the order in layer for all party members based on their y-pos
	void SetOrderInLayer() {
		// Get each party member's y-pos
		List<float> yPositions = new List<float>();
		for (int i = 0; i < partyTransforms.Count; i++) {
			yPositions.Add(partyTransforms[i].position.y);
		}

		// Set highest party member order
		float minValue = yPositions.Min();
		int minIndex = yPositions.IndexOf(minValue);
		partySRends[minIndex].sortingOrder = 2;

		// Set lowest party member order
		float maxValue = yPositions.Max();
		int maxIndex = yPositions.IndexOf(maxValue);
		partySRends[maxIndex].sortingOrder = 0;

		// Set middle party member order
		for (int i = 0; i < yPositions.Count; i++) {
			if(i != minIndex && i != maxIndex) {
				partySRends[i].sortingOrder = 1;
			}
        }
	}

	// Check for random encounter 
	void CheckForRandomEncounter() {
		if (canEncounter) {
			if(Random.Range(0, encounterRate) == 0) {
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

		// Close curtain
		Curtain.S.Close();

		// Audio: Start Battle
		AudioManager.S.PlaySong(eSongName.startBattle);

		// Yield
		yield return new WaitForSeconds(1.5f);

		isBattling = true;

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

	void FixedLoop() {
		// Flip scale
		if (Input.GetAxisRaw("Horizontal") > 0 && !facingRight ||
			Input.GetAxisRaw("Horizontal") < 0 && facingRight) {
			Utilities.S.Flip(gameObject, ref facingRight);
		}
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