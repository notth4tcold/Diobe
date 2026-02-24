using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private PlayerInputHandler input;

    Rigidbody2D rb;
    Vector2 moveInput;
    private GroundCheck groundCheck;
    private Animator anim;

    [Header("Move")]
    private float moveForce = 100.0f;
    private float maxVelocity = 10.0f;
    private float fakeFrictionValue = 0.95f;
    private float groundDeceleration = 80f;
    private float startTimeBtwRunSound = 0.3f;
    private float timeBtwRunSound;
    private bool facingRight = true;

    [Header("Jump")]
    private float jumpForce = 5f;
    private float maxJumpTime = 0.2f;
    private bool isJumping;
    private bool jumpHeld;
    private float jumpTimeCounter;
    private bool wasGrounded;

    [SerializeField] private PlayerWeapon weapon;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();
        anim = GetComponent<Animator>();

        input.OnJumpPressed += TryStartJump;
        input.OnJumpReleased += ReleaseJump;
        input.OnAttackPressed += StartAttack;
        input.OnAttackReleased += ReleaseAttack;
    }

    void FixedUpdate() {
        if (UIManager.Instance.IsUIBlocking) return;

        moveInput = input.MoveInput;
        jumpHeld = input.JumpHeld;

        HandleMove();
        HandleJump();
        CheckLand();
    }

    void HandleMove() {
        rb.AddForce(Vector2.right * moveInput.x * moveForce, ForceMode2D.Force);
        anim.SetBool("Run", moveInput.x != 0 && groundCheck.IsGrounded());

        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxVelocity, maxVelocity);
        rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);

        FakeFriction();
        WalkSound();
        FlipSprite(moveInput.x);
    }

    private void FakeFriction() {
        if (!groundCheck.IsGrounded()) return;

        if (moveInput.x == 0) {
            float newX = Mathf.MoveTowards(rb.linearVelocity.x, 0, groundDeceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        } else {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * fakeFrictionValue, rb.linearVelocity.y);
        }
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
            weapon.SetFacing(facingRight);
        }
    }

    public void HandleJump() {
        if (jumpHeld) TryStartJump(); //To make player jump automatic when hold jump button

        if (!isJumping || !jumpHeld) return;

        if (jumpTimeCounter > 0) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpTimeCounter -= Time.fixedDeltaTime;
        } else {
            isJumping = false;
        }
    }

    void TryStartJump() {
        if (!groundCheck.IsGrounded() || isJumping) return;

        anim.SetTrigger("TakeOf");
        AudioManager.Instance.PlaySFX(SFX.PlayerJump);
        isJumping = true;
        jumpTimeCounter = maxJumpTime;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    public void ReleaseJump() {
        isJumping = false;
    }

    void StartAttack() {
        if (UIManager.Instance.IsUIBlocking) return;

        anim.SetFloat("AttackSpeed", 2f);
        anim.SetBool("Attack", true);
    }

    void ReleaseAttack() {
        anim.SetBool("Attack", false);
        weapon.ReleaseAttack();
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

    void OnDestroy() {
        input.OnJumpPressed -= TryStartJump;
        input.OnJumpReleased -= ReleaseJump;
        input.OnAttackPressed -= StartAttack;
        input.OnAttackReleased -= ReleaseAttack;
    }
}
