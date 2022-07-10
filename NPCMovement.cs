using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement AI for NPCs 
/// </summary>
public class NPCMovement : MonoBehaviour {
	[Header("Set in Inspector")]
	public Transform movePoint;

	public LayerMask bounds;
	public LayerMask playerBounds;

	public Vector2 waitDuration = new Vector2(0.75f, 1.25f);

	[Header("Set Dynamically")]
	private Animator anim;

	private float speed = 2f;

	private bool isWalking;
	private int walkDirection;
	// 0 = right, 1 = up, 2 = left, 3 = down

	private float timer = 0;

	// Flip
	private bool facingRight;

	void Start() {
		anim = GetComponent<Animator>();

		movePoint.parent = null;

		StartCoroutine("FixedUpdateCoroutine");
	}

	public IEnumerator FixedUpdateCoroutine() {
		// If not paused, and there isn't any dialogue being displayed...
		if (!GameManager.S.paused && !DialogueManager.S.TextBoxSpriteGO.activeInHierarchy) {
            if (isWalking) {
				// Move gameObject towards movePoint
				transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);

				// If gameObject has reached movePoint, wait
				if (Vector3.Distance(transform.position, movePoint.position) == 0f) {
					Wait();
				}
            } else {
				// Decrement timer
				timer -= Time.deltaTime;

				// If timer < 0, get a new direction and start moving
				if (timer < 0) {
					// Get new random direction
					walkDirection = Random.Range(0, 4);

					// Move movePoint and start moving towards it 
					switch (walkDirection) {
						case 0:
							CheckIfWalkDirectionIsValid(new Vector3(1 / 2f, 0f, 0f));
							break;
						case 1:
							CheckIfWalkDirectionIsValid(new Vector3(0f, 1 / 2f, 0f));
							break;
						case 2:
							CheckIfWalkDirectionIsValid(new Vector3(-1 / 2f, 0f, 0f));
							break;
						case 3:
							CheckIfWalkDirectionIsValid(new Vector3(0f, -1 / 2f, 0f));
							break;
					}
				}
            }
        }
		yield return new WaitForFixedUpdate();
		StartCoroutine("FixedUpdateCoroutine");
	}

	// If no bounds in nextMovePointPos, move movePoint and start moving towards it.
	// Otherwise, get a new direction and try again 
	void CheckIfWalkDirectionIsValid(Vector3 nextMovePointPos) {
		// If no bounds in this direction
		if (!Physics2D.OverlapCircle(movePoint.position + nextMovePointPos, 0.2f, bounds) &&
			!Physics2D.OverlapCircle(movePoint.position + nextMovePointPos, 0.5f, playerBounds)) {
			// Move movePoint in that direction
			movePoint.position += nextMovePointPos;
			// Start moving the NPC towards the movePoint
			Walk();
		} else {
			// Reset walkDirection and try again
			walkDirection = Random.Range(0, 4);
		}
	}

	public void Walk() {
		isWalking = true;

		// Set animation
        switch (walkDirection) {
			case 1: anim.CrossFade("Walk_Up", 0); break;
			case 3: anim.CrossFade("Walk_Down", 0); break;
			case 0: anim.CrossFade("Walk_Side", 0);
				// Flip
				if (facingRight) { Utilities.S.Flip(gameObject, ref facingRight); } break;
			case 2: anim.CrossFade("Walk_Side", 0);
				// Flip
				if (!facingRight) { Utilities.S.Flip(gameObject, ref facingRight); } break;
		}
	}

	public void Wait() {
		isWalking = false;

		// Reset timer
		timer = Random.Range(waitDuration.x, waitDuration.y);
	}

	public void StopAndFacePlayer() {
		// Ensure the NPC waits when dialogue is over
		Wait();

		// Face direction of Player
		if (Blob.S.gameObject.transform.position.x < transform.position.x &&
			!Utilities.S.isCloserHorizontally(gameObject, Blob.S.gameObject)) { // Left
			// If facing right, flip
			if (transform.localScale.x > 0) { Utilities.S.Flip(gameObject, ref facingRight); }

			anim.Play("Walk_Side", 0, 1);
		} else if (Blob.S.gameObject.transform.position.x > transform.position.x &&
			!Utilities.S.isCloserHorizontally(gameObject, Blob.S.gameObject)) { // Right
			// If facing left, flip
			if (transform.localScale.x < 0) { Utilities.S.Flip(gameObject, ref facingRight); }

			anim.Play("Walk_Side", 0, 1);
		} else if (Blob.S.gameObject.transform.position.y < transform.position.y &&
			Utilities.S.isCloserHorizontally(gameObject, Blob.S.gameObject)) { // Down
			anim.Play("Walk_Down", 0, 1);
		} else if (Blob.S.gameObject.transform.position.y > transform.position.y &&
			Utilities.S.isCloserHorizontally(gameObject, Blob.S.gameObject)) { // Up
			anim.Play("Walk_Up", 0, 1);
		}
	}
}