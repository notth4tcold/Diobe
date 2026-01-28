using UnityEngine;

public class GroundCheck : MonoBehaviour {
    [SerializeField]
    Transform groundCheck;

    [SerializeField]
    private bool grounded;

    [SerializeField]
    LayerMask groundLayer;

    float groundRadius = 0.15f;

    void FixedUpdate() {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    public bool IsGrounded() {
        return grounded;
    }
}
