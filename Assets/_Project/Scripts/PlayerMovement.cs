using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    Rigidbody2D rb;
    Vector2 moveInput;
    private GroundCheck groundCheck;

    [Header("Move")]
    private float moveForce = 100.0f;
    private float maxVelocity = 10.0f;
    private float fakeFrictionValue = 0.95f;

    [Header("Jump")]
    private float jumpForce = 5f;
    private float maxJumpTime = 0.2f;
    [SerializeField]
    private bool isJumping;
    private bool jumpHeld;
    private float jumpTimeCounter;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();
    }

    void FixedUpdate() {
        HandleMove();
        HandleJump();
    }

    void HandleMove() {
        rb.AddForce(Vector2.right * moveInput.x * moveForce * rb.mass, ForceMode2D.Force);
        if (Mathf.Abs(rb.linearVelocity.x) > maxVelocity) rb.linearVelocity = new Vector2(maxVelocity * moveInput.x, rb.linearVelocity.y);
        FakeFriction();
    }

    public void HandleJump() {
        if (!isJumping || !jumpHeld) return;

        if (jumpTimeCounter > 0) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * rb.mass);
            jumpTimeCounter -= Time.fixedDeltaTime;
        } else {
            isJumping = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FakeFriction() {
        if (moveInput.x != 0 && groundCheck.IsGrounded()) rb.linearVelocity = new Vector2(rb.linearVelocity.x * fakeFrictionValue, rb.linearVelocity.y);
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.started) TryStartJump();
        if (context.canceled) ReleaseJump();

        jumpHeld = context.ReadValueAsButton();
    }

    void TryStartJump() {
        if (!groundCheck.IsGrounded() || isJumping) return;

        isJumping = true;
        jumpTimeCounter = maxJumpTime;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * rb.mass);
    }

    public void ReleaseJump() {
        isJumping = false;
    }
}
