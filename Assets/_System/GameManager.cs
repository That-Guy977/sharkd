using UnityEngine;

class GameManager : MonoBehaviour {
    public static GameManager instance;

    [field: SerializeField] public GameObject player { get; private set; }
    [field: SerializeField] public new Camera camera { get; private set; }

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
}
