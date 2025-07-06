using UnityEngine;
using UnityEngine.Audio;
using TMPro;

class VolumeSetting : MonoBehaviour {
    public AudioMixerGroup audioGroup;
    public TMP_Text preview;

    public void SetVolume(float value) {
        audioGroup.audioMixer.SetFloat($"{audioGroup.name}Volume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        preview.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
}
