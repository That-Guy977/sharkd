using UnityEngine;

class BeamTarget : MonoBehaviour {
    public PlayerAttack attack;

    void OnTriggerEnter2D(Collider2D collider) {
        if (gameObject.activeInHierarchy) {
            attack.BeamTarget(collider);
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        attack.BeamTargetCancel(collider);
    }
}
