using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

class Entity : MonoBehaviour {
    public int maxHealth;
    public Slider healthBar;
    public Animator highlight;

    [Header("Messages")]
    public UnityEvent<Vector2, bool> onHit;

    private int health;

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
        if (health == 0) return;
        health -= damage;
        health = Mathf.Max(health, 0);
        highlight.speed = 1;
        highlight.SetTrigger("flash");
        onHit.Invoke(knockback, health == 0);
    }
}
