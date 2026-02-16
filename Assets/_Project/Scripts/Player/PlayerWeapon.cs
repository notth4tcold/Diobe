using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    [SerializeField] private Transform handPosition;
    [SerializeField] private Transform handRotation;

    private GameObject sword;

    private float addAngle = 45f;
    private bool facingRight = true;

    void Start() {
        ResetWeaponPosition();
    }

    void Update() {
        WeaponFollowHand();
    }

    void WeaponFollowHand() {
        if (sword == null) return;

        sword.transform.position = handPosition.position;
        float offset = facingRight ? -addAngle : addAngle;

        sword.transform.eulerAngles = new Vector3(
            handRotation.eulerAngles.x,
            handRotation.eulerAngles.y,
            handRotation.eulerAngles.z + offset
        );
    }

    public void ReleaseAttack() {
        ResetWeaponPosition();
    }

    void ResetWeaponPosition() {
        if (sword == null) return;
        if (facingRight) sword.transform.rotation = Quaternion.Euler(0, 0, -70);
        else sword.transform.rotation = Quaternion.Euler(0, 0, 70);
    }

    public void SetFacing(bool isFacingRight) {
        facingRight = isFacingRight;
    }
}
