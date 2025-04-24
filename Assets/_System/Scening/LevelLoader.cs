using UnityEngine;
using Eflatun.SceneReference;

class LevelLoader : MonoBehaviour {
    public SceneReference level;

    public void LoadLevel() {
        SceneLoader.instance.LoadLevel(level);
    }
}
