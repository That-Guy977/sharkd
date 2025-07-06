using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

class LocaleSetting : MonoBehaviour {
    public Locale locale;
    public Color activeColor;
    public Color inactiveColor;

    Image image;

    void Awake() {
        image = GetComponent<Image>();
        LocalizationSettings.SelectedLocaleChanged += UpdateState;
        StartCoroutine(InitState());
    }

    void UpdateState(Locale currentLocale) {
        image.color = locale.Identifier == currentLocale.Identifier ? activeColor : inactiveColor;
    }

    public void SetLocale() {
        LocalizationSettings.SelectedLocale = locale;
    }

    private IEnumerator InitState() {
        yield return LocalizationSettings.InitializationOperation;
        UpdateState(LocalizationSettings.SelectedLocale);
    }
}
