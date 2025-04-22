using UnityEngine;
using UnityEngine.InputSystem;

class PlayerController : MonoBehaviour {
    public bool control = true;
    public bool combat = true;
    [Header("Movement")]
    public float speed;
    public float jumpForce;
    public float downForceFactor;
    public float downJerk;

    [Header("Config")]
    public LayerMask groundLayer;
    public new CameraFollow camera;

    new Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;
    Animator animator;

    private float move = 0;

    bool grounded => Physics2D.Raycast(
        transform.position,
        Vector2.down,
        0.1f,
        groundLayer
    );

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void OnEnable() {
        camera.enabled = true;
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
        if (!grounded && vel.y < 0) {
            rigidbody.gravityScale = Mathf.MoveTowards(rigidbody.gravityScale, downForceFactor, downJerk * Time.fixedDeltaTime);
        } else {
            rigidbody.gravityScale = 1;
        }
    }

    protected void OnMove(InputValue input) {
        move = input.Get<float>();
    }

    protected void OnJump() {
        if (grounded) {
            rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetTrigger("jump");
        }
    }
}
