using UnityEngine;

class MainMenu : MonoBehaviour {
    public Canvas levelSelect;

    public void Play() {
        GameManager.instance.OpenOverlay(levelSelect);
    }

    public void ShowSettings() {
        GameManager.instance.Settings();
    }
}
