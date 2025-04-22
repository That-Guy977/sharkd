using UnityEngine;
using UnityEngine.InputSystem;

class PlayerController : MonoBehaviour {
    public bool control = true;
    public bool combat = true;
    public float speed;

    public new CameraFollow camera;

    new Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;
    Animator animator;

    private float move = 0;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start() {
        camera.enabled = true;
    }

    void Update() {
        if (move > 0) {
            spriteRenderer.flipX = false;
        } else if (move < 0) {
            spriteRenderer.flipX = true;
        }
        animator.SetFloat("velx", Mathf.Abs(rigidbody.velocity.x));
    }

    void FixedUpdate() {
        Vector2 vel = rigidbody.velocity;
        vel.x = move * speed;
        rigidbody.velocity = vel;
    }

    protected void OnMove(InputValue input) {
        move = input.Get<float>();
    }
}
