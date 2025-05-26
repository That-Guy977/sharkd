using UnityEngine;

class LevelBoundsProvider : MonoBehaviour {
    public BoxCollider2D leftBound;
    public BoxCollider2D rightBound;

    void Start() {
        GameManager.instance.camera.SetBounds(leftBound, rightBound);
    }

    void OnDestroy() {
        GameManager.instance.camera.UnsetBounds();
    }
}
