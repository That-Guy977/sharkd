using UnityEngine;

class GawrSlashTarget : MonoBehaviour {
    public PlayerAttack attack;

    void OnTriggerEnter2D(Collider2D collider) {
        attack.SlashTarget(collider);
    }
}
