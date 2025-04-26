using UnityEngine;

[ExecuteAlways]
class SpawnPoint : MonoBehaviour {
    public Direction facing;

    void Awake() {
        if (!Application.isPlaying) return;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    void Start() {
        if (!Application.isPlaying) return;
        PlayerController player = GameManager.instance.player;
        player.transform.position = transform.position;
        player.GetComponent<Entity>().SetFacing(facing);
        player.gameObject.SetActive(true);
    }

    #if UNITY_EDITOR
    void Update() {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facing switch {
            Direction.Right => 1,
            Direction.Left => -1,
        };
        transform.localScale = scale;
    }
    #endif
}
