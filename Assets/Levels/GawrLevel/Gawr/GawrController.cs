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

    [Header("Misc")]
    public float entranceFadeInDuration;
    public float entranceFadeOutDuration;

    [Header("Config")]
    public GawrLevelLogic levelController;
    public LayerMask groundLayer;
    public AudioSingleProvider entranceSound;
    public AudioBankProvider dashSounds;

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

    RaycastHit2D ground => Physics2D.BoxCast(
        transform.position,
        new Vector2(collider.size.x, 0.1f),
        0,
        Vector2.down,
        0,
        groundLayer
    );
    float gravity => rigidbody.velocity.y > 0 ? jumpGravity : fallGravity;

    public Entity entity { get; private set; }
    public PlayerState currentState => state;
    public bool active => state != PlayerState.Stun && entrance == null;
    public bool grounded => ground;
    public Coroutine entrance { get; private set; }

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
                rigidbody.velocity = move.normalized * dashSpeed;
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
        attack.Clean();
        activeState = StartCoroutine(Stun(defeat));
        if (defeat) {
            levelController.Win();
        }
    }

    public void Move(Direction dir, float factor = 1) {
        move = dir.AsVector() * factor;
    }

    public void Stop(Direction dir) {
        move = Vector2.zero;
        entity.facing = dir;
    }

    public void Jump() {
        rigidbody.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        if (ground.collider.TryGetComponent(out TerrainTypeProvider terrain)) {
            WalkSoundProvider.instance.Emit(terrain.type, WalkSoundType.Jump);
        }
    }

    public Coroutine Dash(Vector2 dir) {
        move = dir;
        SoundFXPlayer.instance.Play(dashSounds);
        return activeState = StartCoroutine(DoDash());
    }

    public Coroutine Attack(Direction dir) {
        entity.facing = dir;
        return activeState = StartCoroutine(DoAttack());
    }

    private IEnumerator DoDash() {
        state = PlayerState.Dash;
        yield return new WaitForSeconds(dashDuration);
        state = PlayerState.None;
        activeState = null;
    }

    private IEnumerator DoAttack() {
        state = PlayerState.Attack;
        yield return new AnimatorPlaying(animator);
        state = PlayerState.None;
        activeState = null;
        attack.Clean();
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

    public void SetFrozen(bool frozen) {
        rigidbody.isKinematic = frozen;
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
        yield return entity.Highlight(entranceFadeInDuration);
        spriteRenderer.enabled = true;
        SoundFXPlayer.instance.Play(entranceSound);
        yield return entity.Dehighlight(entranceFadeOutDuration);
        entity.ResetHighlight();
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
