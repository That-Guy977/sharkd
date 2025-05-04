using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

class Entity : MonoBehaviour {
    public int maxHealth;
    public Slider healthBar;
    public Animator highlight;
    public Canvas hud;
    public RectTransform fixedReflection;
    public AudioBankProvider damageSounds;

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
        SoundFXPlayer.instance.Play(damageSounds);
        onHit.Invoke(knockback, health == 0);
    }

    public static float Distance(Component a, Component b) {
        return Mathf.Abs(a.transform.position.x - b.transform.position.x);
    }

    public bool FacingTowards(Component targetPos) {
        return facing switch {
            Direction.Right => targetPos.transform.position.x > transform.position.x,
            Direction.Left => targetPos.transform.position.x < transform.position.x,
        };
    }

    public Direction Towards(Component target) {
        return (Direction)Mathf.Sign(target.transform.position.x - transform.position.x);
    }

    void UpdateFacing(Transform transform) {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facing.Value();
        transform.localScale = scale;
    }
}
