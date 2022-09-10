using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOnStart : MonoBehaviour {
    [Header("Set in Inspector")]
    public bool activateOnStart = true;

    void Start() {
        gameObject.SetActive(activateOnStart);
    }
}