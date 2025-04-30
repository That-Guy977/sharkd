using UnityEngine;

[RequireComponent(typeof(Camera))]
class CameraController : MonoBehaviour {
    [Min(0f)]
    public float speed;
    public float offset;

    new Camera camera;
    PlayerController player;

    float leftBound;
    float rightBound;

    private float velocity = 0;
    private bool bound = false;

    float halfHeight => camera.orthographicSize;
    float halfWidth => halfHeight * camera.aspect;
    float targetPos {
        get {
            float playerPos = player.transform.position.x + offset;
            if (!bound) {
                return playerPos;
            } else {
                return Mathf.Clamp(playerPos, leftBound, rightBound);
            }
        }
    }

    public float width => halfWidth * 2;

    void Awake() {
        camera = GetComponent<Camera>();
    }

    void Start() {
        player = GameManager.instance.player;
        Vector3 pos = transform.position;
        pos.x = Mathf.SmoothDamp(
            pos.x,
            targetPos,
            ref velocity,
            1 / speed,
            Mathf.Infinity,
            Time.deltaTime
        );
        transform.position = pos;
    }

    void LateUpdate() {
        Vector3 pos = transform.position;
        pos.x = Mathf.SmoothDamp(
            pos.x,
            targetPos,
            ref velocity,
            1 / speed,
            Mathf.Infinity,
            Time.deltaTime
        );
        transform.position = pos;
    }

    public void ResetPosition() {
        camera.transform.position = new Vector3(halfWidth, halfHeight, -10);
    }

    public void SetBounds(BoxCollider2D left, BoxCollider2D right) {
        bound = true;
        leftBound = left.bounds.max.x + halfWidth;
        rightBound = Mathf.Max(right.bounds.min.x - halfWidth, halfWidth);
    }

    public void UnsetBounds() {
        bound = false;
    }
}
