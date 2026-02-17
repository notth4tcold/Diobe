using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    [SerializeField] private Transform handPosition;
    [SerializeField] private Transform handRotation;
    [SerializeField] GameObject itemEquipPrefab;

    private GameObject currentWeapon;
    private bool facingRight = true;
    private float addAngle = 45f;

    void Start() {
        ResetWeaponPosition();
    }

    void Update() {
        if (currentWeapon == null) return;
        WeaponFollowHand();
    }

    void WeaponFollowHand() {
        float offset = facingRight ? -addAngle : addAngle;

        currentWeapon.transform.position = handPosition.position;
        currentWeapon.transform.eulerAngles = new Vector3(
            handRotation.eulerAngles.x,
            handRotation.eulerAngles.y,
            handRotation.eulerAngles.z + offset
        );
    }

    public void ReleaseAttack() {
        ResetWeaponPosition();
    }

    void ResetWeaponPosition() {
        if (currentWeapon == null) return;
        if (facingRight) currentWeapon.transform.rotation = Quaternion.Euler(0, 0, -70);
        else currentWeapon.transform.rotation = Quaternion.Euler(0, 0, 70);
    }

    public void SetFacing(bool isFacingRight) {
        facingRight = isFacingRight;
    }

    public void Equip(ItemData weapon) {
        currentWeapon = Instantiate(itemEquipPrefab, handPosition.position, Quaternion.identity, gameObject.transform);

        WeaponHitbox hitbox = currentWeapon.GetComponent<WeaponHitbox>();
        hitbox.Initialize(weapon);
    }

    public bool HasWeapon => currentWeapon != null;
}
