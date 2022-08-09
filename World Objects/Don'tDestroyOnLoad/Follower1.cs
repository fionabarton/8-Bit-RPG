using UnityEngine;

public class Follower1 : MonoBehaviour {
    [Header("Set Dynamically")]
    private static bool exists;

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