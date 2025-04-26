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
        Vector3 scale = target.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (source.flipX ? -1 : 1);
        target.transform.localScale = scale;
    }
}
