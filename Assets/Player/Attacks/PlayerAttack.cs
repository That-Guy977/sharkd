using System.Collections;
using UnityEngine;

class PlayerAttack : MonoBehaviour {
    [Header("Gura Attacks")]
    public int beamDamage;
    public float beamDamageInterval;
    public float beamKnockback;
    public float beamSpeed;
    public GameObject beamContainer;
    public Transform beamAnchorGround;
    public Transform beamAnchorAir;
    public BoxCollider2D beamHurtbox;
    public Animator beamSummon;
    public SpriteRenderer beamOrigin;
    public SpriteRenderer beam;
    public SpriteRenderer beamHit;

    Entity entity;

    private Coroutine repeatDamage;
    private Collider2D targetCollider;

    void Awake() {
        entity = GetComponent<Entity>();
    }

    public void Reset() {
        StopAllCoroutines();
        repeatDamage = null;
        beamContainer.SetActive(false);
        beamOrigin.gameObject.SetActive(false);
        beam.gameObject.SetActive(false);
        beamHit.gameObject.SetActive(false);
        beam.size = beam.size.WithX(0);
    }

    public void GuraSummonGround() {
        beamContainer.transform.position = beamAnchorGround.position;
        StartCoroutine(GuraSummon());
    }

    public void GuraSummonAir() {
        beamContainer.transform.position = beamAnchorAir.position;
        StartCoroutine(GuraSummon());
    }

    private IEnumerator GuraSummon() {
        beamContainer.SetActive(true);
        yield return new AnimatorPlaying(beamSummon);
        beamOrigin.gameObject.SetActive(true);
        beam.gameObject.SetActive(true);
        while (true) {
            if (targetCollider) {
                float closeEdge = entity.facing switch {
                    Direction.Right => targetCollider.bounds.min.x,
                    Direction.Left => targetCollider.bounds.max.x,
                };
                beam.size = beam.size.WithX(Mathf.Abs(beam.transform.position.x - closeEdge));
                beamHit.transform.position = new Vector3(closeEdge, beam.transform.position.y, 0);
                beamHit.gameObject.SetActive(true);
            } else {
                beam.size += Vector2.right * beamSpeed * Time.deltaTime;
                beamHit.gameObject.SetActive(false);
            }
            yield return null;
        }
    }

    public void GuraTarget(Collider2D collider) {
        targetCollider = collider;
        if (repeatDamage != null) {
            StopCoroutine(repeatDamage);
        }
        if (collider && collider.TryGetComponent(out Entity target)) {
            repeatDamage = StartCoroutine(RepeatDamage(target));
        }
    }

    public void GuraCancel() {
        Reset();
    }

    private IEnumerator RepeatDamage(Entity target) {
        float time = beamDamageInterval;
        while (true) {
            time += Time.deltaTime;
            while (time >= beamDamageInterval) {
                target.Damage(
                    beamDamage,
                    entity.facing switch {
                        Direction.Right => Vector2.right,
                        Direction.Left => Vector2.left,
                    } * beamKnockback
                );
                time -= beamDamageInterval;
            }
            yield return null;
        }
    }
}
