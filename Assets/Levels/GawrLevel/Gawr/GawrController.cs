using System.Collections;
using UnityEngine;

class GawrController : MonoBehaviour {
    [Header("Movement")]
    public float speed;
    public float jumpAscentDuration;
    public float jumpDescentDuration;
    public float jumpHeight;
    public float dashSpeed;
    public float dashDistance;

    [Header("Combat")]
    public float stunDuration;
    public int defeatSlowdownSteps;
    public float defeatInitialSlowdown;
    public float defeatDelay;

    [Header("Misc")]
    public float entranceFadeInDuration;
    public float entranceFadeOutDuration;

    [Header("Config")]
    public GawrLevelLogic levelController;
    public LayerMask groundLayer;
    public AudioSingleProvider entranceSound;
    public AudioBankProvider dashSounds;

    Entity entity;
    new Rigidbody2D rigidbody;
    new BoxCollider2D collider;
    Animator animator;
    SpriteRenderer spriteRenderer;

    float jumpVelocity;
    float jumpGravity;
    float fallGravity;
    float dashDuration;

    private PlayerState state;
    private Vector2 dashDirection;

    RaycastHit2D ground => Physics2D.BoxCast(
        transform.position,
        new Vector2(collider.size.x, 0.1f),
        0,
        Vector2.down,
        0,
        groundLayer
    );
    float gravity => rigidbody.velocity.y > 0 ? jumpGravity : fallGravity;

    void Awake() {
        entity = GetComponent<Entity>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
        CalculateKinematics();
    }

    void Start() {
        state = PlayerState.None;
        SoundFXChecks();
        StartCoroutine(Entrance());
    }

    void Update() {
        animator.SetInteger("state", (int)state);
        animator.SetFloat("air", ground ? -1 : 1);
        animator.SetFloat("vely", rigidbody.velocity.y);
#if UNITY_EDITOR
        CalculateKinematics();
#endif
    }

    void FixedUpdate() {
        switch (state) {
            case PlayerState.None:
                Move();
                break;
            case PlayerState.Dash:
                rigidbody.velocity = dashDirection * dashSpeed;
                break;
            case PlayerState.Attack:
                goto case PlayerState.None;
            case PlayerState.Stun:
                break;
            case PlayerState.Defeat:
                rigidbody.velocity = Vector2.zero;
                break;
        }
    }

    void SoundFXChecks() {
        StartCoroutine(StepSoundLoop());
        StartCoroutine(LandCheck());
    }

    public void OnHit(Vector2 knockback, bool defeat) {
        SetFacing(-knockback);
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(knockback, ForceMode2D.Impulse);
        StartCoroutine(Stun(defeat));
        if (defeat) {
            StartCoroutine(Defeat());
        }
    }

    void Move() {
        rigidbody.AddForce(Vector2.down * gravity);
    }

    private IEnumerator Stun(bool defeat) {
        state = PlayerState.Stun;
        yield return new WaitForSeconds(stunDuration);
        if (defeat) {
            state = PlayerState.Defeat;
        } else {
            state = PlayerState.None;
        }
    }

    private IEnumerator Defeat() {
        Time.timeScale = defeatInitialSlowdown;
        for (int i = 1; i <= defeatSlowdownSteps; i++) {
            yield return new WaitForSecondsRealtime(stunDuration / defeatSlowdownSteps);
            Time.timeScale = Mathf.Lerp(defeatInitialSlowdown, 0, (float)i / defeatSlowdownSteps);
        }
        yield return new WaitForSecondsRealtime(defeatDelay);
        levelController.Win();
    }

    void SetFacing(Vector2 direction) {
        if (direction.x > 0) {
            entity.facing = Direction.Right;
        } else if (direction.x < 0) {
            entity.facing = Direction.Left;
        }
    }

    void CalculateKinematics() {
        jumpVelocity = 2.0f * jumpHeight / jumpAscentDuration;
        jumpGravity = 2.0f * jumpHeight / Mathf.Pow(jumpAscentDuration, 2);
        fallGravity = 2.0f * jumpHeight / Mathf.Pow(jumpDescentDuration, 2);
        dashDuration = dashDistance / dashSpeed;
    }

    private IEnumerator Entrance() {
        spriteRenderer.enabled = false;
        entity.hud.enabled = false;
        yield return null;
        yield return new WaitUntil(() => GameManager.instance.state != GameState.Transitioning);
        entity.highlight.speed = 1 / entranceFadeInDuration;
        entity.highlight.SetTrigger("highlight");
        yield return new AnimatorPlaying(entity.highlight);
        spriteRenderer.enabled = true;
        SoundFXPlayer.instance.Play(entranceSound);
        entity.highlight.speed = 1 / entranceFadeOutDuration;
        entity.highlight.SetTrigger("dehighlight");
        yield return new AnimatorPlaying(entity.highlight);
        entity.highlight.speed = 1;
        entity.highlight.SetTrigger("reset");
        entity.hud.enabled = true;
    }

    private IEnumerator StepSoundLoop() {
        while (true) {
            while (
                state != PlayerState.None && state != PlayerState.Attack && state != PlayerState.Turn
                || Mathf.Abs(rigidbody.velocity.x) < 0.1
                || !ground
                || !ground.collider.TryGetComponent(out TerrainTypeProvider _)
            ) {
                yield return null;
            }
            if (ground.collider.TryGetComponent(out TerrainTypeProvider terrain)) {
                WalkSoundProvider.instance.Emit(terrain.type, WalkSoundType.Step);
            }
            yield return new WaitForSeconds(1 / WalkSoundProvider.instance.stepRate / Mathf.Abs(rigidbody.velocity.x));
        }
    }

    private IEnumerator LandCheck() {
        while (true) {
            yield return new WaitUntil(() => !ground);
            yield return new WaitUntil(() => ground);
            if (ground.collider.TryGetComponent(out TerrainTypeProvider terrain)) {
                WalkSoundProvider.instance.Emit(terrain.type, WalkSoundType.Land);
            }
        }
    }
}
