using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

class PlayerController : MonoBehaviour {
    [Header("Movement")]
    public float speed;
    public float jumpAscentDuration;
    public float jumpDescentDuration;
    public float jumpHeight;
    public float dashSpeed;
    public float dashDistance;
    public float dashCooldownDuration;

    [Header("Combat")]
    public float guraAttackDamage;
    public float guraAttackRate;
    public float guraAttackExitTime;
    public float guraAttackCooldownDuration;
    public float gawrAttackDamage;
    public float gawrAttackCooldownDuration;
    public float turnInDuration;
    public float turnOutDuration;
    public float turnCooldownDuration;
    public float stunDuration;
    public int defeatSlowdownSteps;
    public float defeatInitialSlowdown;
    public float defeatDelay;

    [Header("Config")]
    public LayerMask groundLayer;
    public new CameraFollow camera;

    Entity entity;
    new Rigidbody2D rigidbody;
    new BoxCollider2D collider;
    Animator animator;

    float jumpVelocity;
    float jumpGravity;
    float fallGravity;
    float dashDuration;

    private PlayerState state;
    private Coroutine activeState;
    private Character character;
    private Direction facing;
    private Vector2 move;
    private Vector2 dashDirection;
    private bool dashCooldown;
    private bool turnCooldown;
    private bool attackCooldown;

    bool grounded => Physics2D.BoxCast(
        transform.position,
        new Vector2(collider.size.x, 0.1f),
        0,
        Vector2.down,
        0,
        groundLayer
    );
    float gravity => rigidbody.velocity.y > 0 ? jumpGravity : fallGravity;

    enum PlayerState {
        None,
        Dash,
        Attack,
        Turn,
        Stun,
        Defeat,
    }

    public enum Character {
        Gura,
        Gawr,
    }

    void Awake() {
        entity = GetComponent<Entity>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        if (state == PlayerState.None) {
            SetFacing(move);
        }
        entity.SetFacing(facing);
        animator.SetInteger("state", (int)state);
        animator.SetFloat("char", (int)character);
        animator.SetFloat("movex", Mathf.Abs(move.x));
        animator.SetFloat("movey", move.y);
        animator.SetFloat("air", grounded ? -1 : 1);
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
                if (character == Character.Gawr) {
                    goto case PlayerState.None;
                } else {
                    rigidbody.velocity = Vector2.zero;
                }
                break;
            case PlayerState.Turn:
                goto case PlayerState.None;
            case PlayerState.Stun:
                break;
            case PlayerState.Defeat:
                rigidbody.velocity = Vector2.zero;
                break;
        }
    }

    void OnEnable() {
        camera.enabled = true;
        Reset();
    }

    void OnDisable() {
        if (camera) {
            camera.enabled = false;
        }
    }

    void Reset() {
        state = PlayerState.None;
        StopAllCoroutines();
        activeState = null;
        character = Character.Gura;
        move = Vector2.zero;
        dashCooldown = false;
        turnCooldown = false;
    }

    protected void OnMove(InputValue input) {
        move = input.Get<Vector2>();
    }

    protected void OnJump() {
        if (state != PlayerState.None || !grounded) return;
        rigidbody.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
    }

    protected void OnDash() {
        if (state != PlayerState.None || dashCooldown) return;
        if (move.magnitude > 0.1f) {
            dashDirection = move.normalized;
        } else {
            dashDirection = facing switch {
                Direction.Right => Vector2.right,
                Direction.Left => Vector2.left,
            };
        }
        SetFacing(dashDirection);
        activeState = StartCoroutine(Dash());
    }

    protected void OnAttack(InputValue input) {
        switch (character) {
            case Character.Gura:
                if (input.isPressed) {
                    if (state != PlayerState.None || attackCooldown) return;
                    activeState = StartCoroutine(GuraAttack());
                } else {
                    if (state != PlayerState.Attack) return;
                    StartCoroutine(OnGuraAttackCancel());
                }
                break;
            case Character.Gawr:
                if (input.isPressed) {
                    if (state != PlayerState.None || attackCooldown) return;
                    activeState = StartCoroutine(GawrAttack());
                }
                break;
        }
    }

    protected void OnTurn() {
        if (state != PlayerState.None || turnCooldown) return;
        activeState = StartCoroutine(Turn());
    }

    public void OnHit(Vector2 knockback, bool defeat) {
        SetFacing(-knockback);
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(knockback, ForceMode2D.Impulse);
        if (activeState != null) {
            StopCoroutine(activeState);
        }
        activeState = StartCoroutine(Stun(defeat));
        if (defeat) {
            StartCoroutine(Defeat());
        }
    }

    void Move() {
        Vector2 vel = rigidbody.velocity;
        vel.x = move.x * speed;
        rigidbody.velocity = vel;
        rigidbody.AddForce(Vector2.down * gravity);
    }

    private IEnumerator Dash() {
        state = PlayerState.Dash;
        yield return new WaitForSeconds(dashDuration);
        state = PlayerState.None;
        activeState = null;
        dashCooldown = true;
        yield return new WaitForSeconds(dashCooldownDuration);
        dashCooldown = false;
    }

    private IEnumerator GuraAttack() {
        state = PlayerState.Attack;
        yield return new WaitWhile(() => state == PlayerState.Attack);
        state = PlayerState.None;
        activeState = null;
        attackCooldown = true;
        yield return new WaitForSeconds(guraAttackCooldownDuration);
        attackCooldown = false;
    }

    private IEnumerator OnGuraAttackCancel() {
        yield return new WaitForSeconds(guraAttackExitTime);
        state = PlayerState.None;
    }

    private IEnumerator GawrAttack() {
        state = PlayerState.Attack;
        yield return new AnimatorPlaying(animator);
        state = PlayerState.None;
        activeState = null;
        attackCooldown = true;
        yield return new WaitForSeconds(gawrAttackCooldownDuration);
        attackCooldown = false;
    }

    private IEnumerator Turn() {
        state = PlayerState.Turn;
        entity.highlight.speed = 1 / turnInDuration;
        entity.highlight.SetTrigger("highlight");
        yield return new AnimatorPlaying(entity.highlight);
        character = character switch {
            Character.Gura => Character.Gawr,
            Character.Gawr => Character.Gura,
        };
        attackCooldown = false;
        entity.highlight.speed = 1 / turnOutDuration;
        entity.highlight.SetTrigger("dehighlight");
        yield return new AnimatorPlaying(entity.highlight);
        state = PlayerState.None;
        entity.highlight.speed = 1;
        entity.highlight.SetTrigger("reset");
        activeState = null;
        turnCooldown = true;
        yield return new WaitForSeconds(turnCooldownDuration);
        turnCooldown = false;
    }

    private IEnumerator Stun(bool defeat) {
        state = PlayerState.Stun;
        yield return new WaitForSeconds(stunDuration);
        if (defeat) {
            state = PlayerState.Defeat;
        } else {
            state = PlayerState.None;
            activeState = null;
        }
    }

    private IEnumerator Defeat() {
        GameManager.instance.state = GameManager.GameState.Defeat;
        Time.timeScale = defeatInitialSlowdown;
        for (int i = 0; i < defeatSlowdownSteps; i++) {
            yield return new WaitForSecondsRealtime(stunDuration / defeatSlowdownSteps);
            Time.timeScale = Mathf.Lerp(defeatInitialSlowdown, 0, (float)i / defeatSlowdownSteps);
        }
        yield return new WaitForSecondsRealtime(defeatDelay);
        GameManager.instance.Defeat();
    }

    void SetFacing(Vector2 direction) {
        if (direction.x > 0) {
            facing = Direction.Right;
        } else if (direction.x < 0) {
            facing = Direction.Left;
        }
    }

    void CalculateKinematics() {
        jumpVelocity = 2.0f * jumpHeight / jumpAscentDuration;
        jumpGravity = 2.0f * jumpHeight / Mathf.Pow(jumpAscentDuration, 2);
        fallGravity = 2.0f * jumpHeight / Mathf.Pow(jumpDescentDuration, 2);
        dashDuration = dashDistance / dashSpeed;
    }
}
