using UnityEngine;
using UnityEngine.InputSystem;

class PlayerController : MonoBehaviour {
    public bool control = true;
    public bool combat = true;
    [Header("Movement")]
    public float speed;
    public float jumpAscentDuration;
    public float jumpDescentDuration;
    public float jumpHeight;

    [Header("Config")]
    public LayerMask groundLayer;
    public new CameraFollow camera;

    new Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;
    Animator animator;

    float jumpVelocity;
    float jumpGravity;
    float fallGravity;

    private float move = 0;

    bool grounded => Physics2D.Raycast(
        transform.position,
        Vector2.down,
        0.1f,
        groundLayer
    );
    float gravity => rigidbody.velocity.y > 0 ? jumpGravity : fallGravity;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        jumpVelocity = 2.0f * jumpHeight / jumpAscentDuration;
        jumpGravity = 2.0f * jumpHeight / Mathf.Pow(jumpAscentDuration, 2);
        fallGravity = 2.0f * jumpHeight / Mathf.Pow(jumpDescentDuration, 2);
    }

    void Update() {
        if (move > 0) {
            spriteRenderer.flipX = false;
        } else if (move < 0) {
            spriteRenderer.flipX = true;
        }
        animator.SetBool("grounded", grounded);
        animator.SetBool("running", move != 0);
        animator.SetFloat("vely", rigidbody.velocity.y);
    }

    void FixedUpdate() {
        Vector2 vel = rigidbody.velocity;
        vel.x = move * speed;
        rigidbody.velocity = vel;
        rigidbody.AddForce(Vector2.down * gravity);
    }

    void OnEnable() {
        camera.enabled = true;
    }

    void OnDisable() {
        camera.enabled = false;
    }

    protected void OnMove(InputValue input) {
        move = input.Get<float>();
    }

    protected void OnJump() {
        if (!grounded) return;
        rigidbody.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        animator.SetTrigger("jump");
    }
}
