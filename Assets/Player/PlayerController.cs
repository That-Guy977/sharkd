using UnityEngine;
using UnityEngine.InputSystem;

class PlayerController : MonoBehaviour
{
    public bool control = true;
    public bool combat = true;
    [Header("Movement")]
    public float jumpAscentDuration;
    public float jumpDescentDuration;
    public float jumpHeight;
    public float speed;

    [Header("Config")]
    public LayerMask groundLayer;
    public new CameraFollow camera;

    new Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;
    Animator animator;

    private float move = 0;

    private float jumpVelocity;
    private float jumpGravity;
    private float fallGravity;

    bool grounded => Physics2D.Raycast(
        transform.position,
        Vector2.down,
        0.1f,
        groundLayer
    );

    float customGravity => rigidbody.velocity.y > 0 ? jumpGravity : fallGravity;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        jumpVelocity = (2.0f * jumpHeight / jumpAscentDuration);
        jumpGravity = (2.0f * jumpHeight) / (Mathf.Pow(jumpAscentDuration, 2));
        fallGravity = (2.0f * jumpHeight) / (Mathf.Pow(jumpDescentDuration, 2));

        // Use our custom gravity instead
        rigidbody.gravityScale = 0;
    }

    void OnEnable()
    {
        camera.enabled = true;
    }

    void Update()
    {
        if (move > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (move < 0)
        {
            spriteRenderer.flipX = true;
        }
        animator.SetBool("grounded", grounded);
        animator.SetBool("running", move != 0);
        animator.SetFloat("vely", rigidbody.velocity.y);
    }

    void FixedUpdate()
    {
        Vector2 vel = rigidbody.velocity;
        vel.x = move * speed;
        rigidbody.velocity = vel;
        rigidbody.AddForce(customGravity * Time.fixedDeltaTime * Vector2.down, ForceMode2D.Impulse);
    }

    protected void OnMove(InputValue input)
    {
        move = input.Get<float>();
    }

    protected void OnJump()
    {
        if (!grounded)
        {
            return;
        }
        rigidbody.AddForce(jumpVelocity * Vector2.up, ForceMode2D.Impulse);
        animator.SetTrigger("jump");
    }
}
