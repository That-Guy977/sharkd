using UnityEngine;
using UnityEngine.Audio;
using TMPro;

class VolumeSetting : MonoBehaviour {
    public AudioMixerGroup audioGroup;
    public TMP_Text preview;

    public void SetVolume(float value) {
        Settings.instance.SetVolume(audioGroup, value);
        preview.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
}
