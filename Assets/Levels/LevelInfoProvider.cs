using UnityEngine;

class LevelInfoProvider : MonoBehaviour {
    public Color backgroundFill;
    public BoxCollider2D leftBound;
    public BoxCollider2D rightBound;

    public float left => leftBound.bounds.max.x;
    public float right => rightBound.bounds.min.x;

    void Start() {
        GameManager.instance.camera.LevelInfo(this);
    }
}
