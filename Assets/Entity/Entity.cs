using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

class Entity : MonoBehaviour {
    public int maxHealth;
    public Slider healthBar;
    public Animator highlight;
    public RectTransform fixedReflection;

    [Header("Messages")]
    public UnityEvent<Vector2, bool> onHit;

    private int health;
    private Direction currentDir;

    public Direction facing {
        get => currentDir;
        set {
            currentDir = value;
            UpdateFacing(transform);
            UpdateFacing(fixedReflection);
        }
    }

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

    void UpdateFacing(Transform transform) {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facing.AsScale();
        transform.localScale = scale;
    }
}
