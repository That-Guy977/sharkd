using UnityEngine;

class GawrTarget : MonoBehaviour {
    public GawrAttack attack;

    void OnTriggerEnter2D(Collider2D collider) {
        attack.Target(collider);
    }
}
