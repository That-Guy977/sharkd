using UnityEngine;

[ExecuteAlways]
class EntitySpawnPoint : MonoBehaviour {
    public Entity entity;
    public Direction facing = Direction.Right;

    void Awake() {
        if (!Application.isPlaying) return;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        gameObject.SetActive(false);
    }

    void Start() {
        if (!Application.isPlaying) return;
        entity.transform.position = transform.position;
        entity.GetComponent<Entity>().facing = facing;
        entity.gameObject.SetActive(true);
    }

#if UNITY_EDITOR
    void Update() {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facing.Value();
        transform.localScale = scale;
    }
#endif
}
