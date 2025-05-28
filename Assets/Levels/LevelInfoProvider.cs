using UnityEngine;

class LevelInfoProvider : MonoBehaviour {
    public Color backgroundFill;
    public BoxCollider2D leftBound;
    public BoxCollider2D rightBound;

    void Start() {
        GameManager.instance.camera.LevelInfo(backgroundFill, leftBound, rightBound);
    }
}
