using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    Rigidbody2D rb;
    Vector2 moveInput;
    private GroundCheck groundCheck;
    private Animator anim;

    [Header("Move")]
    private float moveForce = 100.0f;
    private float maxVelocity = 10.0f;
    private float fakeFrictionValue = 0.95f;
    private float startTimeBtwRunSound = 0.3f;
    private float timeBtwRunSound;
    private bool facingRight = true;

    [Header("Jump")]
    private float jumpForce = 5f;
    private float maxJumpTime = 0.2f;
    [SerializeField]
    private bool isJumping;
    public bool jumpHeld;
    private float jumpTimeCounter;
    private bool wasGrounded;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate() {
        HandleMove();
        HandleJump();
        CheckLand();
    }

    void HandleMove() {
        rb.AddForce(Vector2.right * moveInput.x * moveForce * rb.mass, ForceMode2D.Force);
        anim.SetBool("Run", moveInput.x != 0 && groundCheck.IsGrounded());

        if (Mathf.Abs(rb.linearVelocity.x) > maxVelocity) rb.linearVelocity = new Vector2(maxVelocity * moveInput.x, rb.linearVelocity.y);

        FakeFriction();
        WalkSound();
        FlipSprite(moveInput.x);
    }

    public void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FakeFriction() {
        if (groundCheck.IsGrounded()) rb.linearVelocity = new Vector2(rb.linearVelocity.x * fakeFrictionValue, rb.linearVelocity.y);
    }

    private void WalkSound() {
        if (moveInput.x == 0 || !groundCheck.IsGrounded()) return;

        if (timeBtwRunSound <= 0) {
            timeBtwRunSound = startTimeBtwRunSound;
            AudioManager.Instance.PlaySFX(SFX.PlayerRun);
        }
        timeBtwRunSound -= Time.deltaTime;
    }

    private void FlipSprite(float side) {
        if (side < 0 && facingRight || side > 0 && !facingRight) {
            facingRight = !facingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    public void HandleJump() {
        if (jumpHeld) TryStartJump(); //To make player jump automatic when hold jump button

        if (!isJumping || !jumpHeld) return;

        if (jumpTimeCounter > 0) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * rb.mass);
            jumpTimeCounter -= Time.fixedDeltaTime;
        } else {
            isJumping = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.started) TryStartJump();
        if (context.canceled) ReleaseJump();

        jumpHeld = context.ReadValueAsButton();
    }

    void TryStartJump() {
        if (!groundCheck.IsGrounded() || isJumping) return;

        anim.SetTrigger("TakeOf");
        AudioManager.Instance.PlaySFX(SFX.PlayerJump);
        isJumping = true;
        jumpTimeCounter = maxJumpTime;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * rb.mass);
    }

    public void ReleaseJump() {
        isJumping = false;
    }

    public void OnAttack(InputAction.CallbackContext context) {
        anim.SetFloat("AttackSpeed", 2);
        if (context.started) StartAttack();
        if (context.canceled) ReleaseAttack();
    }

    void StartAttack() {
        anim.SetBool("Attack", true);
    }

    void ReleaseAttack() {
        anim.SetBool("Attack", false);
    }

    void CheckLand() {
        bool grounded = groundCheck.IsGrounded();
        anim.SetBool("Jump", !grounded);

        if (!wasGrounded && grounded) {
            OnLand();
        }

        wasGrounded = grounded;
    }

    void OnLand() {
        if (rb.linearVelocity.y < -1f) {
            AudioManager.Instance.PlaySFX(SFX.PlayerLand);
            CameraManager.Instance.Shake();
        }
    }
}
