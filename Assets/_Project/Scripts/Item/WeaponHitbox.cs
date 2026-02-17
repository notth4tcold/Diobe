using UnityEngine;

public class WeaponHitbox : MonoBehaviour {
    [SerializeField] private ItemData itemData;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PolygonCollider2D polygonCollider;

    private bool canHit = false;

    void Awake() {
        Initialize(itemData);
    }

    public void Initialize(ItemData data) {
        if (data == null) return;

        itemData = data;
        spriteRenderer.sprite = data.icon;

        AdjustCollider();
    }

    void AdjustCollider() {
        Destroy(polygonCollider);
        polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = true;
        //polygonCollider.enabled = false;
    }

    // public void EnableHitbox() {
    //     canHit = true;
    //     polygonCollider.enabled = true;
    // }

    // public void DisableHitbox() {
    //     canHit = false;
    //     polygonCollider.enabled = false;
    // }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!canHit) return;

        // TODO Implementar depois que criar inimigo other.GetComponent<Enemy>(); enemy.TakeDamage(damage);
    }

    public ItemData GetItemData() => itemData;
}
