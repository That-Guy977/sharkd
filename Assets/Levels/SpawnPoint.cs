using UnityEngine;

[ExecuteAlways]
class SpawnPoint : MonoBehaviour {
    public Direction facing = Direction.Right;

    void Awake() {
        if (!Application.isPlaying) return;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    void Start() {
        if (!Application.isPlaying) return;
        GameManager.instance.player.Spawn(transform.position, facing);
    }

#if UNITY_EDITOR
    void Update() {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facing.Value();
        transform.localScale = scale;
    }
#endif
}
