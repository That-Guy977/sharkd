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

    new Rigidbody2D rigidbody;
    new BoxCollider2D collider;
    SpriteRenderer spriteRenderer;
    Animator animator;
    Animator highlight;

    float jumpVelocity;
    float jumpGravity;
    float fallGravity;
    float dashDuration;

    private PlayerState state;
    private Coroutine activeCoroutine;
    private Character character;
    private Direction facing;
    private Vector2 move;
    private Vector2 dashDirection;
    private bool dashCooldown;
    private bool turnCooldown;

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
        Stun,
        Defeat,
        Turn,
    }

    public enum Character {
        Gura,
        Gawr,
    }

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        highlight = GetComponent<Entity>().highlight;
    }

    void Update() {
        if (state == PlayerState.None) {
            SetFacing(move);
        }
        spriteRenderer.flipX = facing switch {
            Direction.Left => true,
            Direction.Right => false,
        };
        animator.SetInteger("state", (int)state);
        animator.SetFloat("char", (int)character);
        animator.SetFloat("movex", Mathf.Abs(move.x));
        animator.SetFloat("movey", move.y);
        animator.SetBool("grounded", grounded);
        animator.SetFloat("vely", rigidbody.velocity.y);
        #if UNITY_EDITOR
            CalculateKinematics();
        #endif
    }

    void FixedUpdate() {
        switch (state) {
            case PlayerState.None:
            case PlayerState.Turn:
                Vector2 vel = rigidbody.velocity;
                vel.x = move.x * speed;
                rigidbody.velocity = vel;
                rigidbody.AddForce(Vector2.down * gravity);
                break;
            case PlayerState.Dash:
                rigidbody.velocity = dashDirection * dashSpeed;
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
        activeCoroutine = null;
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
        activeCoroutine = StartCoroutine(Dash());
    }

    protected void OnTurn() {
        if (state != PlayerState.None || turnCooldown) return;
        activeCoroutine = StartCoroutine(Turn());
    }

    public void OnHit(Vector2 knockback, bool defeat) {
        SetFacing(-knockback);
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(knockback, ForceMode2D.Impulse);
        if (activeCoroutine != null) {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartCoroutine(Stun(defeat));
        if (defeat) {
            StartCoroutine(Defeat());
        }
    }

    private IEnumerator Dash() {
        state = PlayerState.Dash;
        yield return new WaitForSeconds(dashDuration);
        state = PlayerState.None;
        activeCoroutine = null;
        dashCooldown = true;
        yield return new WaitForSeconds(dashCooldownDuration);
        dashCooldown = false;
    }

    private IEnumerator Turn() {
        state = PlayerState.Turn;
        highlight.speed = 1 / turnInDuration;
        highlight.SetTrigger("highlight");
        yield return new AnimatorPlaying(highlight);
        character = character switch {
            Character.Gura => Character.Gawr,
            Character.Gawr => Character.Gura,
        };
        highlight.speed = 1 / turnOutDuration;
        highlight.SetTrigger("dehighlight");
        yield return new AnimatorPlaying(highlight);
        state = PlayerState.None;
        highlight.speed = 1;
        highlight.SetTrigger("reset");
        activeCoroutine = null;
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
            activeCoroutine = null;
        }
    }

    private IEnumerator Defeat() {
        GameManager.instance.state = GameManager.GameState.Defeat;
        Time.timeScale = defeatInitialSlowdown;
        for (int i = 0; i < defeatSlowdownSteps; i++) {
            yield return new WaitForSecondsRealtime(stunDuration / defeatSlowdownSteps);
            Time.timeScale = Mathf.Lerp(defeatInitialSlowdown, 0, (float) i / defeatSlowdownSteps);
        }
        yield return new WaitForSecondsRealtime(defeatDelay);
        GameManager.instance.Defeat();
    }

    public void SetFacing(Direction direction) {
        facing = direction;
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
