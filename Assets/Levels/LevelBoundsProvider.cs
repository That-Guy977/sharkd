using UnityEngine;

class LevelBoundsProvider : MonoBehaviour {
    public BoxCollider2D leftBound;
    public BoxCollider2D rightBound;

    void OnEnable() {
        GameManager.instance.camera.SetBounds(leftBound, rightBound);
    }

    void OnDisable() {
        GameManager.instance.camera.UnsetBounds();
    }
}
