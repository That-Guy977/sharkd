using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

class Entity : MonoBehaviour {
    public int maxHealth;
    public Slider healthBar;
    public int flashCounts;
    public float flashDuration;
    public SpriteRenderer flashSprite;

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
        StartCoroutine(Flash());
        onHit.Invoke(knockback, health == 0);
    }

    private IEnumerator Flash() {
        for (int i = 0; i < flashCounts; i++) {
            flashSprite.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
            flashSprite.color = default;
            yield return new WaitForSeconds(flashDuration);
        }
    }
}
