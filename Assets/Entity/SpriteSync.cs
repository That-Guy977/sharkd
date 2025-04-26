using UnityEngine;

[ExecuteAlways]
class SpriteMaskSync : MonoBehaviour {
    SpriteRenderer source;
    SpriteMask target;

    void Awake() {
        source = transform.parent.GetComponent<SpriteRenderer>();
        target = GetComponent<SpriteMask>();
    }

    void LateUpdate() {
        target.sprite = source.sprite;
    }
}
