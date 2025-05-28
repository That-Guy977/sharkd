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
    public AudioSingleProvider beamSummonSound;
    public AudioSingleProvider beamSound;

    [Header("Gawr Attacks")]
    public int slashDamage;
    public float slashKnockback;
    public float slashKnockbackUp;
    public GameObject slashContainer;
    public BoxCollider2D slashHurtbox;
    public AudioBankProvider slashSounds;
    public AudioBankProvider slashHitSounds;

    Entity entity;

    private Coroutine manaDelay;
    private float mana;
    private bool manaRegen;
    private Coroutine beamRepeatDamage;
    private Collider2D beamTargetCollider;
    private AudioSource beamSummonSoundSource;
    private AudioSource beamSoundSource;
    private bool slashHit;

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

    public void Clean() {
        StopAllCoroutines();
        beamRepeatDamage = null;
        beamContainer.SetActive(false);
        beamOrigin.gameObject.SetActive(false);
        beam.gameObject.SetActive(false);
        beamHit.gameObject.SetActive(false);
        beam.size = beam.size.WithX(0);
        slashContainer.SetActive(false);
        slashHurtbox.gameObject.SetActive(false);
        slashHit = false;
        if (beamSummonSoundSource) Destroy(beamSummonSoundSource.gameObject);
        if (beamSoundSource) Destroy(beamSoundSource.gameObject);
        if (!manaRegen) {
            manaDelay = StartCoroutine(ManaRegenDelay());
        }
    }

    public void BeamSummonGround() {
        beamContainer.transform.position = beamAnchorGround.position;
        StartCoroutine(BeamSummon());
    }

    public void BeamSummonAir() {
        beamContainer.transform.position = beamAnchorAir.position;
        StartCoroutine(BeamSummon());
    }

    private IEnumerator BeamSummon() {
        beamContainer.SetActive(true);
        beamSummonSoundSource = SoundFXPlayer.instance.Play(beamSummonSound);
        yield return new AnimatorPlaying(beamSummon);
        manaRegen = false;
        if (manaDelay != null) {
            StopCoroutine(manaDelay);
        }
        beamOrigin.gameObject.SetActive(true);
        beam.gameObject.SetActive(true);
        beamSoundSource = SoundFXPlayer.instance.PlayLoop(beamSound);
        while (true) {
            if (!beamCanAttack) {
                BeamCancel();
                break;
            }
            if (beamTargetCollider) {
                float closeEdge = entity.facing switch {
                    Direction.Right => beamTargetCollider.bounds.min.x,
                    Direction.Left => beamTargetCollider.bounds.max.x,
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

    public void BeamTarget(Collider2D collider) {
        beamTargetCollider = collider;
        if (beamRepeatDamage != null) {
            StopCoroutine(beamRepeatDamage);
        }
        if (collider && collider.TryGetComponent(out Entity target)) {
            beamRepeatDamage = StartCoroutine(BeamRepeatDamage(target));
        }
    }

    public void BeamCancel() {
        Clean();
        manaDelay = StartCoroutine(ManaRegenDelay());
    }

    private IEnumerator BeamRepeatDamage(Entity target) {
        float time = beamDamageInterval;
        while (true) {
            time += Time.deltaTime;
            while (time >= beamDamageInterval) {
                target.Damage(beamDamage, entity.facing.AsVector() * beamKnockback);
                time -= beamDamageInterval;
            }
            yield return null;
        }
    }

    public void SlashStart() {
        slashContainer.SetActive(true);
    }

    public void Slash() {
        slashHurtbox.gameObject.SetActive(true);
        SoundFXPlayer.instance.Play(slashSounds);
    }

    public void SlashEnd() {
        Clean();
    }

    public void SlashTarget(Collider2D collider) {
        if (collider.TryGetComponent(out Entity target)) {
            target.Damage(slashDamage, (entity.facing.AsVector() * slashKnockback).SlightUp(slashKnockbackUp));
            if (!slashHit) {
                SoundFXPlayer.instance.Play(slashHitSounds);
                slashHit = true;
            }
        }
    }

    private IEnumerator ManaRegenDelay() {
        manaRegen = false;
        yield return new WaitForSeconds(manaRegenDelay);
        manaRegen = true;
    }
}
