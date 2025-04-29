using System.Collections;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Eflatun.SceneReference;

class MusicPlayer : MonoBehaviour {
    public static MusicPlayer instance;

    public float fadeInDuration;
    public float fadeOutDuration;
    public float pauseVolume;
    public float pauseFadeInDuration;
    public float pauseFadeOutDuration;
    [SerializedDictionary("Scene", "Music")]
    public SceneDictionary<AudioSingleProvider> music;

    AudioSource source;

    private float clipVolume;

    void Awake() {
        if (!instance) {
            instance = this;
        } else {
            Destroy(this);
        }
        source = GetComponent<AudioSource>();
    }

    void Start() {
        source.ignoreListenerPause = true;
        source.volume = 0;
    }

    public void Play(SceneReference scene) {
        if (music.TryGetValue(scene, out var playable)) {
            source.clip = playable.clip;
            clipVolume = playable.volume;
            source.Play();
            StopAllCoroutines();
            StartCoroutine(Fade(clipVolume, fadeInDuration));
        }
    }

    public void Stop() {
        StopAllCoroutines();
        StartCoroutine(Fade(0, fadeOutDuration));
    }

    public void Pause() {
        if (GameManager.instance.state == GameState.Transitioning) return;
        StopAllCoroutines();
        StartCoroutine(Fade(clipVolume * pauseVolume, pauseFadeOutDuration));
    }

    public void Unpause() {
        if (GameManager.instance.state == GameState.Transitioning) return;
        StopAllCoroutines();
        StartCoroutine(Fade(clipVolume, pauseFadeInDuration));
    }

    private IEnumerator Fade(float target, float time) {
#if UNITY_EDITOR
        yield return null;
        yield return null;
#endif
        float elapsedTime = 0;
        float start = source.volume;
        do {
            elapsedTime += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(start, target, elapsedTime / time);
            yield return null;
        } while (elapsedTime < time);
    }
}
