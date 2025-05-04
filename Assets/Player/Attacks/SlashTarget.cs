using UnityEngine;

class SlashTarget : MonoBehaviour {
    public PlayerAttack attack;

    void OnTriggerEnter2D(Collider2D collider) {
        attack.SlashTarget(collider);
    }
}
