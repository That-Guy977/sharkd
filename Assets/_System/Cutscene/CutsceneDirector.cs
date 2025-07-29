using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

class CutsceneDirector : MonoBehaviour {
    public float skipHoldTime;
    public Image skipProgress;

    PlayableDirector director;
    Animator animator;

    private Coroutine skipCheck;

    void Awake() {
        director = GetComponent<PlayableDirector>();
        animator = GetComponent<Animator>();
    }

    void Clean() {
        if (skipCheck != null) {
            StopCoroutine(skipCheck);
            skipProgress.fillAmount = 0;
        }
    }

    protected void OnSkip(InputValue input) {
        if (director.state != PlayState.Playing) return;
        if (input.isPressed) {
            skipCheck = StartCoroutine(SkipCheck());
        } else {
            if (skipCheck != null) {
                StopCoroutine(skipCheck);
                skipProgress.fillAmount = 0;
            }
        }
    }

    public void Play(PlayableAsset playable, Action onDone) {
        StartCoroutine(DoPlay(playable, onDone));
    }

    private IEnumerator DoPlay(PlayableAsset playable, Action onDone) {
        animator.SetTrigger("show");
        yield return new AnimatorPlaying(animator);
        director.Play(playable);
        yield return new WaitWhile(() => director.state == PlayState.Playing);
        animator.SetTrigger("hide");
        yield return new AnimatorPlaying(animator);
        onDone();
        skipProgress.fillAmount = 0;
        StopAllCoroutines();
    }

    private IEnumerator SkipCheck() {
        float elapsedTime = 0;
        do {
            elapsedTime += Time.unscaledDeltaTime;
            skipProgress.fillAmount = elapsedTime / skipHoldTime;
            yield return null;
        } while (elapsedTime < skipHoldTime);
        director.Stop();
        skipCheck = null;
    }
}
