using UnityEngine;

class SpawnPoint : MonoBehaviour {
    public Direction facing;

    void Awake() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    void Start() {
        PlayerController player = GameManager.instance.player;
        player.transform.position = transform.position;
        player.facing = facing;
        player.gameObject.SetActive(true);
    }
}
