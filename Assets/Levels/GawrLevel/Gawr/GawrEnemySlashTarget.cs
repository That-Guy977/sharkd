using UnityEngine;

class GawrEnemySlashTarget : MonoBehaviour {
    public GawrAttack attack;

    void OnTriggerEnter2D(Collider2D collider) {
        attack.Target(collider);
    }
}
