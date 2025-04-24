using UnityEngine;

class SpawnPoint : MonoBehaviour {
    void Awake() {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void Start() {
        GameObject player =  GameManager.instance.player;
        player.transform.position = transform.position;
        player.SetActive(true);
    }
}
