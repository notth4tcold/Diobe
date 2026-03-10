using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private PlayerInputHandler input;

    Rigidbody2D rb;
    Vector2 moveInput;
    private GroundCheck groundCheck;
    private Animator anim;
    private Player player;

    private float baseMoveForce = 100.0f;
    private float baseMaxVelocity = 10.0f;
    float MoveForce => baseMoveForce * (1f + player.stats.Get(StatType.MoveSpeed));
    float MaxVelocity => baseMaxVelocity * (1f + player.stats.Get(StatType.MoveSpeed));

    private float fakeFrictionValue = 0.95f;
    private float groundDeceleration = 80f;

    private float startTimeBtwRunSound = 0.3f;
    private float timeBtwRunSound;

    private bool facingRight = true;
    public event System.Action<bool> OnFacingChanged;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();
        anim = GetComponent<Animator>();

        GameManager.Instance.SubscribeToPlayerReady(HandlePlayerReady);
    }

    void FixedUpdate() {
        if (UIManager.Instance.IsUIBlocking) return;
        moveInput = input.MoveInput;

        HandleMove();
    }

    void HandleMove() {
        rb.AddForce(Vector2.right * moveInput.x * MoveForce, ForceMode2D.Force);
        anim.SetBool("Run", moveInput.x != 0 && groundCheck.IsGrounded());

        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -MaxVelocity, MaxVelocity);
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
            OnFacingChanged?.Invoke(facingRight);
        }
    }

    void OnDestroy() {
        GameManager.Instance.UnsubscribeFromPlayerReady(HandlePlayerReady);
    }

    private void HandlePlayerReady(Player p) {
        player = p;
    }
}
