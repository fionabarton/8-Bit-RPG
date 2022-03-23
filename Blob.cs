using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : MonoBehaviour {
	[Header("Set in Inspector")]
	public GameObject	playerTriggerGO;

	public Transform	movePoint;
	public LayerMask	bounds;

	[Header("Set Dynamically")]
	public Animator		anim;

	private float		speed = 3f;
	private bool		facingRight = true;
	private static bool exists;
	public int			lastDirection;
	public bool			canMove = true;
	public float		destination;
	public bool			alreadyTriggered; // Prevents triggering multiple triggers

	// Singleton
	private static Blob _S;
	public static Blob S { get { return _S; } set { _S = value; } }

	void Awake() {
		// Singleton
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
	}

    void Start() {
		movePoint.parent = null;
	}

    void Loop() {
        if (canMove) {
			// Move gameObject towards movePoint
			transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);

			// If gameObject is on movePoint
			if (Vector3.Distance(transform.position, movePoint.position) == 0f) {
				if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f) {
					if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal") / 2f, 0f, 0f), 0.2f, bounds)) {
						movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal") / 2f, 0f, 0f);
					}

					// Set anim and trigger position
					anim.CrossFade("Walk_Side", 0);
					Utilities.S.SetLocalPosition(playerTriggerGO, 0.375f, 0);
				} else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f) {
					if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical") / 2f, 0f), 0.2f, bounds)) {
						movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical") / 2f, 0f);
					}

					// Set anim and trigger position
					if (Input.GetAxisRaw("Vertical") > 0) {
						anim.CrossFade("Walk_Up", 0);
						Utilities.S.SetLocalPosition(playerTriggerGO, 0, 0.375f);
					} else {
						anim.CrossFade("Walk_Down", 0);
						Utilities.S.SetLocalPosition(playerTriggerGO, 0, -0.375f);
					}
				}
			}
		}
	}

	void FixedLoop() {
		// Flip scale
		if (Input.GetAxisRaw("Horizontal") > 0 && !facingRight ||
			Input.GetAxisRaw("Horizontal") < 0 && facingRight) {
			Utilities.S.Flip(gameObject, ref facingRight);
		}
	}
}
