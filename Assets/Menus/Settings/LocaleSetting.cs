using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

class LocaleSetting : MonoBehaviour {
    public Locale locale;
    public Color activeColor;
    public Color inactiveColor;

    Image image;

    void Awake() {
        image = GetComponent<Image>();
    }

    void Update() {
        image.color = locale == Settings.instance.GetLocale() ? activeColor : inactiveColor;
    }

    public void SetLocale() => Settings.instance.SetLocale(locale);
}
