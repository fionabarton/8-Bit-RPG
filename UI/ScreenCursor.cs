using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to RPGMainCamera; used to get ScreenCursor gameObject
/// </summary>
public class ScreenCursor : MonoBehaviour {
    [Header("Set Dynamically")]
    private static ScreenCursor _S;
    public static ScreenCursor S { get { return _S; } set { _S = value; } }

    [Header("Set in Inspector")]
    public List<GameObject> cursorGO = new List<GameObject>();
    public List<RectTransform> rectTrans = new List<RectTransform>();
    public List<Animator> anims = new List<Animator>();

    void Awake() {
        S = this;
    }

    private void Start() {
        // Deactivate screen cursors
        Utilities.S.SetActiveList(cursorGO, false);
    }

    // Play all animators from the first animation frame
    public void ResetAnimClip() {
        for(int i = 0; i < anims.Count; i++) {
            if (anims[i].gameObject.activeInHierarchy) {
                anims[i].Play("Cursor", -1, 0);
            }
        }
    }
}
