using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour {
    public Sprite sprite;
    public string layer = "Background";
    [Range(0f, 1f)]
    public float depth;

    new Camera camera;

    GameObject background;
    readonly LinkedList<GameObject> tiles = new();
    float spriteWidth;

    private Vector2 previousCameraPosition = Vector2.zero;

    Vector2 cameraPosition { get => camera.gameObject.transform.position; }
    float center { get => (tiles.First.Value.transform.position.x + tiles.Last.Value.transform.position.x + spriteWidth) / 2f; }

    public void Awake() {
        background = new($"Background_{sprite.name}");
        background.transform.SetParent(transform);
        spriteWidth = sprite.bounds.size.x;
    }

    public void Start() {
        camera = GameManager.instance.camera;
        int tileCount = Mathf.FloorToInt(camera.orthographicSize * camera.aspect * 2.1f / spriteWidth) + 2;
        for (int i = 0; i < tileCount; i++) {
            GameObject tile = new(sprite.name, typeof(SpriteRenderer));
            tile.transform.SetParent(background.transform);
            SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingLayerName = layer;
            tile.transform.position = new Vector3(spriteWidth * i, 0, depth);
            tiles.AddLast(tile);
        }
    }

    public void LateUpdate() {
        if (previousCameraPosition != Vector2.zero) {
            background.transform.position += (Vector3)(cameraPosition - previousCameraPosition) * depth;
        }
        previousCameraPosition = cameraPosition;
        if (cameraPosition.x > center + spriteWidth / 2f) {
            GameObject tile = tiles.First.Value;
            tile.transform.position += Vector3.right * (tiles.Count * spriteWidth);
            tiles.RemoveFirst();
            tiles.AddLast(tile);
        } else if (cameraPosition.x < center - spriteWidth / 2f) {
            GameObject tile = tiles.Last.Value;
            tile.transform.position += Vector3.left * (tiles.Count * spriteWidth);
            tiles.RemoveLast();
            tiles.AddFirst(tile);
        }
    }
}
