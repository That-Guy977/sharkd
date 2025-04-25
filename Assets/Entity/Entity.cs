using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

class Entity : MonoBehaviour {
    public float maxHealth;
    public Slider healthBar;
    public UnityEvent<Vector2> onHit;
    public UnityEvent<Vector2> onDeath;

    private float health;

    void Start() {
        healthBar.maxValue = maxHealth;
    }

    void Update() {
        healthBar.value = health;
    }

    void OnEnable() {
        health = maxHealth;
    }

    public void Damage(int damage, Vector2 knockback) {
        health -= damage;
        health = Mathf.Max(health, 0);
        if (health == 0) {
            onDeath.Invoke(knockback);
        } else {
            onHit.Invoke(knockback);
        }
    }
}
