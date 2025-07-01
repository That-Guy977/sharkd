using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Audio;

class Settings : Singleton<Settings> {
    public void SetLocale(Locale locale) {
        LocalizationSettings.SelectedLocale = locale;
    }

    public void SetVolume(AudioMixerGroup group, float volume) {
        group.audioMixer.SetFloat($"{group.name}Volume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
    }
}
