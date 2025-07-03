using UnityEngine;

class CameraSync : MonoBehaviour {
    Canvas canvas;
    new Camera camera;

    void Awake() {
        canvas = GetComponent<Canvas>();
    }

    void Start() {
        camera = GameManager.instance.camera.camera;
        canvas.worldCamera = camera;
    }
}
