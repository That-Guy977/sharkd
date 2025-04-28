using UnityEngine;

class GuraBeamTarget : MonoBehaviour {
    public PlayerAttack attack;

    void OnTriggerEnter2D(Collider2D collider) {
        attack.BeamTarget(collider);
    }

    void OnTriggerExit2D() {
        attack.BeamTarget(null);
    }
}
