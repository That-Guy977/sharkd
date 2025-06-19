using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
class CameraController : MonoBehaviour {
    [Min(0f)]
    public float speed;
    public Vector2 secondaryTrackRange;
    public Vector2 secondaryLoseRange;

    new Camera camera;
    PlayerController player;

    Color backgroundColor;
    float leftBound;
    float rightBound;
    Transform secondaryTarget;

    private Vector2 velocity = Vector2.zero;
    private bool bound = false;
    private bool secondaryTracking = false;

    float halfHeight => camera.orthographicSize;
    float halfWidth => halfHeight * camera.aspect;
    Vector2 halfSize => new Vector2(halfWidth, halfHeight);
    Vector2 targetPos {
        get {
            Vector2 target = player.transform.position;
            if (secondaryTarget && secondaryTarget.gameObject.activeInHierarchy) {
                Bounds secondaryTrackingBounds = new Bounds(
                    player.transform.position,
                    (!secondaryTracking ? secondaryTrackRange : secondaryLoseRange) * halfSize * 4
                );
                secondaryTracking = secondaryTrackingBounds.Contains(secondaryTarget.position);
                if (secondaryTracking) {
                    target = (target + (Vector2)secondaryTarget.position) / 2;
                }
            }
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
        camera.transform.position = (Vector3)halfSize + Vector3.forward * -10;
        camera.backgroundColor = backgroundColor;
        bound = false;
        secondaryTarget = null;
        secondaryTracking = false;
    }

    public void LevelInfo(LevelInfoProvider level) {
        camera.backgroundColor = level.backgroundFill;
        bound = true;
        leftBound = level.left + halfWidth;
        rightBound = Mathf.Max(level.right - halfWidth, halfWidth);
        secondaryTarget = level.target;
    }
}
