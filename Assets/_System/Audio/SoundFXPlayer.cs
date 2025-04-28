using System.Collections;
using UnityEngine;

class SoundFXPlayer : MonoBehaviour {
    public static SoundFXPlayer instance;

    public AudioSource template;

    void Awake() {
        if (!instance) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    public AudioSource Play(AudioPlayable playable, float volume = 1) {
        AudioSource source = Instantiate(template, transform);
        source.clip = playable.clip;
        source.volume *= playable.volume * volume;
        source.Play();
        StartCoroutine(AutoStop(source));
        return source;
    }

    public AudioSource PlayLoop(AudioPlayable playable, float volume = 1) {
        AudioSource source = Instantiate(template, transform);
        source.clip = playable.clip;
        source.volume *= playable.volume * volume;
        source.loop = true;
        source.Play();
        return source;
    }

    private IEnumerator AutoStop(AudioSource source) {
        do {
            if (!source.isPlaying) {
                Destroy(source.gameObject);
                break;
            }
            yield return null;
        } while (source);
    }
}
