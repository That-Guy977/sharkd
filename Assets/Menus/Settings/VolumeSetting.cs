using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

class VolumeSetting : MonoBehaviour {
    public AudioMixerGroup audioGroup;
    public Slider control;
    public TMP_Text preview;

    void OnEnable() {
        control.value = Settings.instance.GetVolume(audioGroup);
    }

    void Update() {
        Settings.instance.SetVolume(audioGroup, control.value);
        preview.text = $"{Mathf.RoundToInt(control.value * 100)}%";
    }
}
