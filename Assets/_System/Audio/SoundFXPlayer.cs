using System.Collections;
using UnityEngine;

class SoundFXPlayer : Singleton<SoundFXPlayer> {
    public AudioSource template;

    public AudioSource Play(AudioProvider provider, float volume = 1) {
        AudioSource source = Instantiate(template, transform);
        source.clip = provider.clip;
        source.volume *= provider.volume * volume;
        source.Play();
        StartCoroutine(AutoStop(source));
        return source;
    }

    public AudioSource PlayLoop(AudioProvider provider, float volume = 1) {
        AudioSource source = Instantiate(template, transform);
        source.clip = provider.clip;
        source.volume *= provider.volume * volume;
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
