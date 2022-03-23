using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A set of general functions that are HOPEFULLY useful in a multitude of projects
/// </summary>
public class Utilities : MonoBehaviour
{
    [Header("Set Dynamically")]
    // Singleton
    private static Utilities _S;
    public static Utilities S { get { return _S; } set { _S = value; } }

    void Awake() {
        S = this;
    }

	////////////////////////////////////////////////////////////////////////////////
	// Set GameObject Position
	public void SetPosition(GameObject tGO, float x, float y) {
		Vector3 tPos = tGO.transform.position;
		tPos.x = x;
		tPos.y = y;
		tGO.transform.position = tPos;
	}
	// Set GameObject LOCAL Position
	public void SetLocalPosition(GameObject tGO, float x, float y) {
		Vector3 tPos = tGO.transform.localPosition;
		tPos.x = x;
		tPos.y = y;
		tGO.transform.localPosition = tPos;
	}
	////////////////////////////////////////////////////////////////////////////////
	// Set GameObject Scale
	public void SetScale(GameObject tGO, float x, float y) {
		Vector3 tScale = tGO.transform.localScale;
		tScale.x = x;
		tScale.y = y;
		tGO.transform.localScale = tScale;
	}

	// Flip GameObject Scale/Sprite
	public void Flip(GameObject go, ref bool facingRight) {
		facingRight = !facingRight;
		SetScale(go, go.transform.localScale.x * -1, go.transform.localScale.y);
	}
	////////////////////////////////////////////////////////////////////////////////
	// Set Selected GameObject
	public void SetSelectedGO(GameObject tGO) {
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(tGO);
	}
	////////////////////////////////////////////////////////////////////////////////
	// Gradually Move GameObject's X Position  
	public void MoveXPosition(GameObject tGO, float speed) {
		Vector3 tPos = tGO.transform.position;
		tPos.x += speed * Time.fixedDeltaTime;
		tGO.transform.position = tPos;
	}
	// Gradually Move GameObject's Y Position  
	public void MoveYPosition(GameObject tGO, float speed) {
		Vector3 tPos = tGO.transform.position;
		tPos.y += speed * Time.fixedDeltaTime;
		tGO.transform.position = tPos;
	}
	////////////////////////////////////////////////////////////////////////////////
	// Get Percentage. Returns a float from 0.00...1 to 1.0
	public float GetPercentage(float value, float maxValue) {
		return value / maxValue;
	}
	////////////////////////////////////////////////////////////////////////////////
	// Map a value within a set of numbers to a different set of numbers
	public float Map(float OldMin, float OldMax, float NewMin, float NewMax, float valueToMap) {
		float OldRange = (OldMax - OldMin);
		float NewRange = (NewMax - NewMin);
		float NewValue = (((valueToMap - OldMin) * NewRange) / OldRange) + NewMin;

		return (NewValue);
	}
	////////////////////////////////////////////////////////////////////////////////
	// Calculate Average/Mean of two integers
	public int CalculateAverage(int a, int b) {
		// Account for DivideByZeroException
		if (b == 0) {
			return a;
		} else {
			int c = a / b;
			return c;
		}
	}
	////////////////////////////////////////////////////////////////////////////////
	// Make a list of buttons interactable
	public void ButtonsInteractable(List<Button> buttons, bool isInteractable) {
		for (int i = 0; i <= buttons.Count - 1; i++) {
			buttons[i].interactable = isInteractable;
		}
	}

	// Remove listeners from a list of buttons 
	public void RemoveListeners(List<Button> buttons) {
		for (int i = 0; i <= buttons.Count - 1; i++) {
			buttons[i].onClick.RemoveAllListeners();
		}
	}

	////////////////////////////////////////////////////////////////////////////////
	// Returns true if this gameObject is closer to another gameObject
	// horizontally than it is vertically
	public bool isCloserHorizontally(GameObject gameObject1, GameObject gameObject2) {
		float tX = Mathf.Abs(gameObject1.transform.position.x - gameObject2.transform.position.x);
		float tY = Mathf.Abs(gameObject1.transform.position.y - gameObject2.transform.position.y);

		if (tX < tY) { return true; } else { return false; }
	}
}
