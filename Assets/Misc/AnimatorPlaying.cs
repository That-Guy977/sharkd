using UnityEngine;

class AnimatorPlaying : CustomYieldInstruction {
    Animator animator;

    public override bool keepWaiting {
        get {
            if (bufferFrames != 0) {
                bufferFrames--;
                return true;
            } else {
                return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
            }
        }
    }

    private int bufferFrames;

    public AnimatorPlaying(Animator animator) {
        this.animator = animator;
        bufferFrames = 2;
    }
}
