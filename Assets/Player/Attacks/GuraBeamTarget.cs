using UnityEngine;

class GuraBeamTarget : MonoBehaviour {
    public PlayerAttack attack;

    void OnTriggerEnter2D(Collider2D collider) {
        attack.GuraTarget(collider);
    }

    void OnTriggerExit2D() {
        attack.GuraTarget(null);
    }
}
