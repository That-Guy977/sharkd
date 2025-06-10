using UnityEngine;

class PlayerFollow : MonoBehaviour {
    PlayerController player;

    Vector3 offset;

    void Start() {
        player = GameManager.instance.player;
        offset = transform.localPosition;
        transform.SetParent(null);
    }

    void LateUpdate() {
        transform.position = player.transform.position + offset;
    }
}
