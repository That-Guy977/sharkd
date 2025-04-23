using UnityEngine;
using Eflatun.SceneReference;

class MainMenu : MonoBehaviour {
    public SceneReference intro;
    public SceneReference map;

    SceneLoader loader;

    void Awake() {
        loader = GetComponent<SceneLoader>();
    }

    public void StartGame() {
        if (!GameManager.instance.introPlayed) {
            loader.LoadScene(intro);
        } else {
            loader.LoadScene(map);
        }
    }
}
