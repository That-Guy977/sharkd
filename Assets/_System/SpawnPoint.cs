using UnityEngine;

[ExecuteAlways]
class SpawnPoint : MonoBehaviour {
    public Direction facing;

    void Awake() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    void Start() {
        if (!Application.isPlaying) return;
        PlayerController player = GameManager.instance.player;
        player.transform.position = transform.position;
        player.SetFacing(facing);
        player.gameObject.SetActive(true);
    }

    #if UNITY_EDITOR
    void Update() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = facing switch {
            Direction.Left => true,
            Direction.Right => false,
        };
    }
    #endif
}
