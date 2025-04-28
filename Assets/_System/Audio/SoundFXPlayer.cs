using UnityEngine;

class SoundFXPlayer : MonoBehaviour {
    public static SoundFXPlayer instance;

    AudioSource source;

    void Awake() {
        if (!instance) {
            instance = this;
        } else {
            Destroy(this);
        }
        source = GetComponent<AudioSource>();
    }

    public void Play(AudioPlayable playable, float volume) {
        source.PlayOneShot(playable.clip, playable.volume * volume);
    }
}
