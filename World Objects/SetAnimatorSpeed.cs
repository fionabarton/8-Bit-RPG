using UnityEngine;

public class SetAnimatorSpeed : MonoBehaviour {
    [Header("Set dynamically")]
    public Animator anim;

    void Start() {
        anim = GetComponent<Animator>();
    }

    public void SetSpeed(int speed = 0) {
        anim.speed = speed;
    }
}
