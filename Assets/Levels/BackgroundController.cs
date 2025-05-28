using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
class BackgroundController : MonoBehaviour {
    public new RuntimeAnimatorController animation;

    new CameraController camera;

    readonly LinkedList<Transform> tiles = new();
    float depth;
    Sprite sprite;
    string layer;
    float spriteWidth;

    private Vector2 previousCameraPosition = Vector2.zero;

    Vector2 cameraPosition => camera.gameObject.transform.position;
    float center => (tiles.First.Value.transform.position.x + tiles.Last.Value.transform.position.x + spriteWidth) / 2f;
    int sortingOrder => transform.parent ? transform.GetSiblingIndex() : 0;

    void Awake() {
        if (!Application.isPlaying) return;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        depth = transform.position.z;
        sprite = spriteRenderer.sprite;
        layer = spriteRenderer.sortingLayerName;
        spriteWidth = sprite.bounds.size.x;
        spriteRenderer.enabled = false;
    }

    void Start() {
        if (!Application.isPlaying) return;
        camera = GameManager.instance.camera;
        int tileCount = Mathf.FloorToInt(camera.width * 1.1f / spriteWidth) + 2;
        for (int i = 0; i < tileCount; i++) {
            GameObject tile = new(sprite.name);
            tile.transform.SetParent(transform);
            tile.transform.position = new Vector3(spriteWidth * i, 0, 0);
            tiles.AddLast(tile.transform);

            SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingLayerName = layer;
            spriteRenderer.sortingOrder = sortingOrder;

            if (animation) {
                Animator animator = tile.AddComponent<Animator>();
                animator.runtimeAnimatorController = animation;
            }
        }
        camera.cameraUpdate += CameraUpdate;
    }

    void OnDestroy() {
        if (!Application.isPlaying) return;
        camera.cameraUpdate -= CameraUpdate;
    }

#if UNITY_EDITOR
    void Update() {
        GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
    }
#endif

    void CameraUpdate() {
        if (previousCameraPosition != Vector2.zero) {
            Vector3 deltaPosition = cameraPosition - previousCameraPosition;
            deltaPosition.y /= 2;
            transform.position += deltaPosition * depth;
        }
        previousCameraPosition = cameraPosition;
        if (cameraPosition.x > center + spriteWidth / 2f) {
            Transform tile = tiles.First.Value;
            tile.position += Vector3.right * (tiles.Count * spriteWidth);
            tiles.RemoveFirst();
            tiles.AddLast(tile);
        } else if (cameraPosition.x < center - spriteWidth / 2f) {
            Transform tile = tiles.Last.Value;
            tile.position += Vector3.left * (tiles.Count * spriteWidth);
            tiles.RemoveLast();
            tiles.AddFirst(tile);
        }
    }
}
