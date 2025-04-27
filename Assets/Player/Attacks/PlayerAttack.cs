using System.Collections;
using UnityEngine;
using UnityEngine.UI;

class PlayerAttack : MonoBehaviour {
    public int maxMana;
    public float manaRegenRate;
    public float manaRegenDelay;
    public Slider manaBar;

    [Header("Gura Attacks")]
    public int beamDamage;
    public float beamDamageInterval;
    public float beamKnockback;
    public int beamManaThreshold;
    public float beamManaDrain;
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
    private Coroutine manaDelay;
    private float mana;
    private bool manaRegen;
    private Collider2D targetCollider;

    public bool beamCanStart => mana >= beamManaThreshold;
    public bool beamCanAttack => mana > 0;

    void Awake() {
        entity = GetComponent<Entity>();
    }

    void Start() {
        manaBar.maxValue = maxMana;
    }

    void Update() {
        if (manaRegen) {
            mana += manaRegenRate * Time.deltaTime;
            mana = Mathf.Min(mana, maxMana);
        }
        manaBar.value = mana;
    }

    void OnEnable() {
        mana = maxMana;
    }

    public void Reset() {
        StopAllCoroutines();
        repeatDamage = null;
        manaDelay = StartCoroutine(ManaRegenDelay());
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
        manaRegen = false;
        if (manaDelay != null) {
            StopCoroutine(manaDelay);
        }
        beamOrigin.gameObject.SetActive(true);
        beam.gameObject.SetActive(true);
        while (true) {
            if (!beamCanAttack) {
                GuraCancel();
                break;
            }
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
            mana -= beamManaDrain * Time.deltaTime;
            mana = Mathf.Max(mana, 0);
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
                    entity.facing.AsVector() * beamKnockback
                );
                time -= beamDamageInterval;
            }
            yield return null;
        }
    }

    private IEnumerator ManaRegenDelay() {
        manaRegen = false;
        yield return new WaitForSeconds(manaRegenDelay);
        manaRegen = true;
    }
}
