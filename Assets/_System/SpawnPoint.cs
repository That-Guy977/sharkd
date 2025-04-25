using UnityEngine;

class SpawnPoint : MonoBehaviour {
    public Direction facing;

    public enum Direction {
        Left, Right
    }

    void Awake() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    void Start() {
        GameObject player = GameManager.instance.player;
        player.transform.position = transform.position;
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        playerSprite.flipX = facing == Direction.Right;
        player.SetActive(true);
    }
}
