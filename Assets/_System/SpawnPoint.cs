using UnityEngine;

class SpawnPoint : MonoBehaviour {
    void Start() {
        GameObject player =  GameManager.instance.player;
        player.transform.position = transform.position;
        player.SetActive(true);
    }
}
