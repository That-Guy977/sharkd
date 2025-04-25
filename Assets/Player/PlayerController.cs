using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

class PlayerController : MonoBehaviour {
    public bool control = true;
    public bool combat = true;
    public Direction facing;
    [Header("Movement")]
    public float speed;
    public float jumpAscentDuration;
    public float jumpDescentDuration;
    public float jumpHeight;
    public float dashSpeed;
    public float dashDistance;
    public float dashCooldownDuration;

    [Header("Config")]
    public LayerMask groundLayer;
    public new CameraFollow camera;

    new Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;
    Animator animator;

    float jumpVelocity;
    float jumpGravity;
    float fallGravity;
    float dashDuration;

    private PlayerState state;
    private Vector2 move;
    private Vector2 dashDirection;
    private bool dashCooldown;

    bool grounded => Physics2D.Raycast(
        transform.position,
        Vector2.down,
        0.1f,
        groundLayer
    );
    float gravity => rigidbody.velocity.y > 0 ? jumpGravity : fallGravity;

    enum PlayerState {
        None,
        Dash,
    }

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        if (state == PlayerState.None) {
            spriteRenderer.flipX = facing == Direction.Left;
        }
        animator.SetInteger("state", (int)state);
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
                Vector2 vel = rigidbody.velocity;
                vel.x = move.x * speed;
                rigidbody.velocity = vel;
                rigidbody.AddForce(Vector2.down * gravity);
                break;
            case PlayerState.Dash:
                rigidbody.velocity = dashDirection * dashSpeed;
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
        move = Vector2.zero;
        dashCooldown = false;
    }

    protected void OnMove(InputValue input) {
        move = input.Get<Vector2>();
        if (move.x > 0) {
            facing = Direction.Right;
        } else if (move.x < 0) {
            facing = Direction.Left;
        }
    }

    protected void OnJump() {
        if (!grounded || state != PlayerState.None) return;
        rigidbody.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
    }

    protected void OnDash() {
        if (state != PlayerState.None || dashCooldown) return;
        if (move.magnitude > 0.1f) {
            dashDirection = move.normalized;
        } else {
            dashDirection = facing == Direction.Right ? Vector2.right : Vector2.left;
        }
        StartCoroutine(Dash());
    }

    private IEnumerator Dash() {
        state = PlayerState.Dash;
        yield return new WaitForSeconds(dashDuration);
        state = PlayerState.None;
        dashCooldown = true;
        yield return new WaitForSeconds(dashCooldownDuration);
        dashCooldown = false;
    }

    void CalculateKinematics() {
        jumpVelocity = 2.0f * jumpHeight / jumpAscentDuration;
        jumpGravity = 2.0f * jumpHeight / Mathf.Pow(jumpAscentDuration, 2);
        fallGravity = 2.0f * jumpHeight / Mathf.Pow(jumpDescentDuration, 2);
        dashDuration = dashDistance / dashSpeed;
    }
}
