using UnityEngine;

public class Follower2 : MonoBehaviour {
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