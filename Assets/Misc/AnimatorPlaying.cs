using UnityEngine;

class AnimatorPlaying : CustomYieldInstruction {
    Animator animator;

    public override bool keepWaiting {
        get {
            if (firstFrame) {
                firstFrame = false;
                return true;
            } else {
                return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
            }
        }
    }

    private bool firstFrame = true;

    public AnimatorPlaying(Animator animator) {
        this.animator = animator;
    }
}
