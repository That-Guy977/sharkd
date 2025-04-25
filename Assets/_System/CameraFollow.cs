using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour {
    [Min(0f)]
    public float speed;
    public float offset;

    new Camera camera;
    GameObject player;

    float leftBound, rightBound;

    private float velocity = 0;

    float targetPos {
        get {
            return player.transform.position.x + offset;
            // return Mathf.Clamp(player.transform.position.x + offset, leftBound, rightBound);
        }
    }

    void Awake() {
        camera = GetComponent<Camera>();
    }

    void Start() {
        player = GameManager.instance.player;
        // float cameraHalfWidth = camera.orthographicSize * camera.aspect;
        // leftBound = cameraHalfWidth;
        // rightBound = Mathf.Max(levelController.size.x - cameraHalfWidth, cameraHalfWidth);
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
}
