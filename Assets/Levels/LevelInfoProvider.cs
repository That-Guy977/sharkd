using UnityEngine;

class LevelInfoProvider : MonoBehaviour {
    public Transform target;
    public Color backgroundFill;
    public BoxCollider2D leftBound;
    public BoxCollider2D rightBound;

    public float left => leftBound.bounds.max.x;
    public float right => rightBound.bounds.min.x;

    void Start() {
        GameManager.instance.camera.LevelInfo(this);
    }

    public bool InBounds(float pos, float size = 0) {
        return pos - size / 2 >= left && pos + size / 2 <= right;
    }
}
