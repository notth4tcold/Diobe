using UnityEngine;

public class PlayerJump : MonoBehaviour {
    [SerializeField] private PlayerInputHandler input;

    Rigidbody2D rb;
    private GroundCheck groundCheck;
    private Animator anim;

    [Header("Jump")]
    private float jumpForce = 5f;
    private float maxJumpTime = 0.2f;
    private bool isJumping;
    private bool jumpHeld;
    private float jumpTimeCounter;
    private bool wasGrounded;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();
        anim = GetComponent<Animator>();

        input.OnJumpPressed += TryStartJump;
        input.OnJumpReleased += ReleaseJump;
    }

    void FixedUpdate() {
        if (UIManager.Instance.IsUIBlocking) return;
        jumpHeld = input.JumpHeld;

        HandleJump();
        CheckLand();
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
    }
}
