using UnityEngine;

class TestDamage : MonoBehaviour {
    public int damage;
    public float knockback;
    public float knockbackUpForce;

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.TryGetComponent(out Entity entity)) {
            entity.Damage(damage, KnockbackForce(collision));
        }
    }

    Vector2 KnockbackForce(Collision2D collision) {
        Vector2 direction = (collision.collider.bounds.center - collision.otherCollider.bounds.center).normalized;
        Vector2 force = direction * knockback;
        if (direction.y >= 0) {
            force += Vector2.up * knockbackUpForce;
            force = Vector2.ClampMagnitude(force, knockback);
        }
        return force;
    }
}
