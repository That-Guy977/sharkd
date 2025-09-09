using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

class Entity : MonoBehaviour {
    public int maxHealth;
    public Slider healthBar;
    public Animator highlight;
    public Canvas hud;
    public RectTransform fixedReflection;
    public AudioBankProvider damageSounds;

    [Header("Messages")]
    public UnityEvent<Vector2, bool> onHit;

    new Collider2D collider;

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
    public bool invincible { get; set; }
    public Vector2 position => collider.bounds.center;

    void Awake() {
        collider = GetComponent<Collider2D>();
    }

    void Start() {
        healthBar.maxValue = maxHealth;
    }

    void Update() {
        healthBar.value = health;
    }

    void OnEnable() {
        hud.enabled = true;
        health = maxHealth;
        invincible = false;
    }

    public void Damage(int damage, Vector2 knockback) {
        if (invincible || health == 0) return;
        health -= damage;
        health = Mathf.Max(health, 0);
        highlight.speed = 1;
        highlight.SetTrigger("flash");
        SoundFXPlayer.instance.Play(damageSounds);
        onHit.Invoke(knockback, health == 0);
    }

    public bool FacingTowards(Component targetPos) {
        return facing switch {
            Direction.Right => targetPos.transform.position.x > transform.position.x,
            Direction.Left => targetPos.transform.position.x < transform.position.x,
        };
    }

    public Direction Towards(Component target) {
        return Towards(target.transform.position.x);
    }

    public Direction Towards(float target) {
        return (Direction)Mathf.Sign(target - transform.position.x);
    }

    public T FarthestTarget<T>(IEnumerable<T> targets) where T : Component {
        T farthest = null;
        float maxDist = 0;
        foreach (var target in targets) {
            float dist = Vector2.Distance(transform.position, target.transform.position);
            if (dist > maxDist) {
                farthest = target;
                maxDist = dist;
            }
        }
        return farthest;
    }

    void UpdateFacing(Transform transform) {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facing.Value();
        transform.localScale = scale;
    }

    public AnimatorPlaying Highlight(float duration) {
        highlight.speed = 1 / duration;
        highlight.SetTrigger("highlight");
        return new AnimatorPlaying(highlight);
    }

    public AnimatorPlaying Dehighlight(float duration) {
        highlight.speed = 1 / duration;
        highlight.SetTrigger("dehighlight");
        return new AnimatorPlaying(highlight);
    }

    public void ResetHighlight() {
        highlight.speed = 1;
        highlight.SetTrigger("reset");
    }
}
