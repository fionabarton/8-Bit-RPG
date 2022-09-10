using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Manages party members that follow the lead character in the overworld
public class Followers : MonoBehaviour {
    [Header("Set in Inspector")]
	// Follower variables
	public List<GameObject> followersGO;
	public List<Transform> followerMovePoints;
	public List<Animator> followerAnims;

	// Variables for getting/setting the order in layer for all party members
	public List<Transform> partyTransforms;
	public List<SpriteRenderer> partySRends;

	[Header("Set Dynamically")]
	// Follower variables
	public List<Vector3> movePoints;
	public List<string> animations;
	public List<bool> facingRights;

	// Cache and set followers' move points 
	public void AddFollowerMovePoints(Transform movePoint, bool facingRight) {
		// Cache move point and facingRight
		movePoints.Insert(0, movePoint.position);
		facingRights.Insert(0, facingRight);

		if (movePoints.Count > 3) {
            // Set follower's movePoint pos
            if (followersGO[1].gameObject.activeInHierarchy) {
                followerMovePoints[1].position = movePoints[3];
			}

			// Set followers' facing 
			SetFollowerFacing(1);

			// Remove from lists
			movePoints.RemoveAt(movePoints.Count - 1);
			facingRights.RemoveAt(facingRights.Count - 1);
		}
		if (movePoints.Count > 1) {
			// Set follower's movePoint pos 
			if (followersGO[0].gameObject.activeInHierarchy) {
				followerMovePoints[0].position = movePoints[1];
			}

			// Set followers' facing
			SetFollowerFacing(0);
		}
	}

	// Set followers' facing
	public void SetFollowerFacing(int ndx) {
		if (facingRights[ndx]) {
			Utilities.S.SetScale(followersGO[ndx], 1, followersGO[ndx].transform.localScale.y);
		} else {
			Utilities.S.SetScale(followersGO[ndx], -1, followersGO[ndx].transform.localScale.y);
		}
	}

	// Cache and set animations for all followers
	public void AddFollowerAnimations(string animationToAdd) {
		// Cache animation to add
		animations.Insert(0, animationToAdd);

		// Set followers' animations
		if (animations.Count > 3) {
			SetOrderInLayer();

			if (followersGO[1].activeInHierarchy) {
				followerAnims[1].CrossFade(animations[3], 0);
			}

			animations.RemoveAt(animations.Count - 1);
		}
		if (animations.Count > 1) {
			if (followersGO[0].activeInHierarchy) {
				followerAnims[0].CrossFade(animations[1], 0);
			}
		}
	}

	// Set the order in layer for all party members based on their y-pos
	public void SetOrderInLayer() {
		if (movePoints.Count > 2) {
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
				if (i != minIndex && i != maxIndex) {
					partySRends[i].sortingOrder = 1;
				}
			}
		}
	}
}