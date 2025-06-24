using UnityEngine;

class MainMenu : MonoBehaviour {
    public Canvas levelSelect;
    public Canvas credits;

    public void Play() {
        GameManager.instance.OpenOverlay(levelSelect);
    }

    public void ShowSettings() {
        GameManager.instance.Settings();
    }

    public void ShowCredits() {
        GameManager.instance.OpenOverlay(credits);
    }
}
