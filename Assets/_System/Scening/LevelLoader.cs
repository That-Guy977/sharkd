using UnityEngine;
using Eflatun.SceneReference;

class LevelLoader : MonoBehaviour {
    public SceneReference level;

    SceneLoader loader;

    void Awake() {
        loader = GetComponentInParent<SceneLoader>();
    }

    public void LoadLevel() {
        loader.LoadScene(level);
        GameManager.instance.state = GameManager.GameState.InLevel;
    }
}
