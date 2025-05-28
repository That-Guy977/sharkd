using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
class CameraController : MonoBehaviour {
    [Min(0f)]
    public float speed;

    new Camera camera;
    PlayerController player;

    Color backgroundColor;
    float leftBound;
    float rightBound;

    private Vector2 velocity = Vector2.zero;
    private bool bound = false;

    float halfHeight => camera.orthographicSize;
    float halfWidth => halfHeight * camera.aspect;
    Vector2 targetPos {
        get {
            Vector2 target = player.transform.position;
            if (bound) {
                target.x = Mathf.Clamp(target.x, leftBound, rightBound);
            }
            target.y = Mathf.Max(target.y, halfHeight);
            return target;
        }
    }

    public float width => halfWidth * 2;

    public event Action cameraUpdate;

    void Awake() {
        camera = GetComponent<Camera>();
        backgroundColor = camera.backgroundColor;
    }

    void Start() {
        player = GameManager.instance.player;
    }

    void LateUpdate() {
        Vector3 pos = Vector2.SmoothDamp(transform.position, targetPos, ref velocity, 1 / speed);
        pos.z = transform.position.z;
        transform.position = pos;
        cameraUpdate?.Invoke();
    }

    public void Clean() {
        camera.transform.position = new Vector3(halfWidth, halfHeight, -10);
        camera.backgroundColor = backgroundColor;
        bound = false;
    }

    public void LevelInfo(Color backgroundFill, BoxCollider2D left, BoxCollider2D right) {
        camera.backgroundColor = backgroundFill;
        bound = true;
        leftBound = left.bounds.max.x + halfWidth;
        rightBound = Mathf.Max(right.bounds.min.x - halfWidth, halfWidth);
    }
}
