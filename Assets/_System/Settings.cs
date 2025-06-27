using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Audio;

class Settings : Singleton<Settings> {
    Locale currentLocale;
    Dictionary<AudioMixerGroup, float> mixerGroups = new();
    
    public Locale GetLocale() {
        if (!currentLocale) {
            currentLocale = LocalizationSettings.SelectedLocale;
        }
        return currentLocale;
    }

    public void SetLocale(Locale locale) {
        LocalizationSettings.SelectedLocale = currentLocale = locale;
    }

    public float GetVolume(AudioMixerGroup group) {
        if (mixerGroups.TryGetValue(group, out float volume)) {
            return volume;
        } else {
            return mixerGroups[group] = 1;
        }
    }

    public void SetVolume(AudioMixerGroup group, float volume) {
        mixerGroups[group] = volume;
        group.audioMixer.SetFloat($"{group.name}Volume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
    }
}
