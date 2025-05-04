using UnityEngine;

class GawrAttack : MonoBehaviour {
    public int damage;
    public float knockback;
    public float knockbackUp;
    public GameObject container;
    public BoxCollider2D hurtbox;
    public AudioBankProvider sounds;
    public AudioBankProvider hitSounds;

    Entity entity;

    void Awake() {
        entity = GetComponent<Entity>();
    }

    public void Clean() {
        StopAllCoroutines();
        container.SetActive(false);
        hurtbox.gameObject.SetActive(false);
    }

    public void SlashStart() {
        container.SetActive(true);
    }

    public void Slash() {
        hurtbox.gameObject.SetActive(true);
        SoundFXPlayer.instance.Play(sounds);
    }

    public void SlashEnd() {
        Clean();
    }

    public void Target(Collider2D collider) {
        if (collider.TryGetComponent(out Entity target)) {
            target.Damage(damage, (entity.facing.AsVector() * knockback).SlightUp(knockbackUp));
            SoundFXPlayer.instance.Play(hitSounds);
        }
    }
}
