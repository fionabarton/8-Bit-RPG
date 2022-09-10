using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {
    [Header("Set Dynamically")]
    private bool exists;

    void Awake() {
        // DontDestroyOnLoad
        if (!exists) {
            exists = true;
            DontDestroyOnLoad(transform.gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}