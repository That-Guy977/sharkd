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
    public float dashCooldownDuration;

    [Header("Combat")]
    public float attackCooldownDuration;
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
    GawrAttack attack;
    new Rigidbody2D rigidbody;
    new BoxCollider2D collider;
    Animator animator;
    SpriteRenderer spriteRenderer;

    float jumpVelocity;
    float jumpGravity;
    float fallGravity;
    float dashDuration;

    private PlayerState state;
    private Coroutine activeState;
    private Vector2 move;
    private bool dashCooldown;
    private bool attackCooldown;

    RaycastHit2D ground => Physics2D.BoxCast(
        transform.position,
        new Vector2(collider.size.x, 0.1f),
        0,
        Vector2.down,
        0,
        groundLayer
    );
    float gravity => rigidbody.velocity.y > 0 ? jumpGravity : fallGravity;

    public PlayerState currentState => state;
    public Coroutine entrance { get; private set; }
    public bool canJump => state == PlayerState.None && ground;
    public bool canDash => state == PlayerState.None && !dashCooldown;
    public bool canAttack => state == PlayerState.None && !attackCooldown;

    void Awake() {
        entity = GetComponent<Entity>();
        attack = GetComponent<GawrAttack>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CalculateKinematics();
    }

    void Start() {
        state = PlayerState.None;
        StopAllCoroutines();
        activeState = null;
        move = Vector2.zero;
        SoundFXChecks();
        entrance = StartCoroutine(Entrance());
    }

    void Update() {
        if (state == PlayerState.None || state == PlayerState.Dash) {
            SetFacing(move);
        }
        animator.SetInteger("state", (int)state);
        animator.SetFloat("movex", Mathf.Abs(move.x));
        animator.SetFloat("movey", move.y);
        animator.SetFloat("air", ground ? -1 : 1);
        animator.SetFloat("vely", rigidbody.velocity.y);
#if UNITY_EDITOR
        CalculateKinematics();
#endif
    }

    void FixedUpdate() {
        switch (state) {
            case PlayerState.None:
                rigidbody.velocity = rigidbody.velocity.WithX(move.x * speed);
                rigidbody.AddForce(Vector2.down * gravity);
                break;
            case PlayerState.Dash:
                rigidbody.velocity = move * dashSpeed;
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
        if (activeState != null) {
            StopCoroutine(activeState);
        }
        dashCooldown = false;
        attackCooldown = false;
        attack.Clean();
        activeState = StartCoroutine(Stun(defeat));
        if (defeat) {
            StartCoroutine(Defeat());
        }
    }

    public void Move(float dir) {
        move = Vector2.right * dir;
    }

    public void Jump() {
        rigidbody.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        if (ground.collider.TryGetComponent(out TerrainTypeProvider terrain)) {
            WalkSoundProvider.instance.Emit(terrain.type, WalkSoundType.Jump);
        }
    }

    public void Dash(Vector2 dir) {
        move = dir;
        activeState = StartCoroutine(DoDash());
        SoundFXPlayer.instance.Play(dashSounds);
    }

    public void Attack() {
        activeState = StartCoroutine(DoAttack());
    }

    private IEnumerator DoDash() {
        state = PlayerState.Dash;
        yield return new WaitForSeconds(dashDuration);
        state = PlayerState.None;
        activeState = null;
        dashCooldown = true;
        yield return new WaitForSeconds(dashCooldownDuration);
        dashCooldown = false;
    }

    private IEnumerator DoAttack() {
        state = PlayerState.Attack;
        yield return new AnimatorPlaying(animator);
        state = PlayerState.None;
        activeState = null;
        attack.Clean();
        attackCooldown = true;
        yield return new WaitForSeconds(attackCooldownDuration);
        attackCooldown = false;
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
        Time.timeScale = 1;
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
        entrance = null;
    }

    private IEnumerator StepSoundLoop() {
        while (true) {
            TerrainTypeProvider terrain;
            while (
                state != PlayerState.None && state != PlayerState.Attack && state != PlayerState.Turn
                || Mathf.Abs(rigidbody.velocity.x) < 0.1f
                || !ground
                || !ground.collider.TryGetComponent(out terrain)
            ) {
                yield return null;
            }
            WalkSoundProvider.instance.Emit(terrain.type, WalkSoundType.Step);
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
