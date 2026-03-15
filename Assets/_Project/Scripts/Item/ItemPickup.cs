using UnityEngine;

public class ItemPickup : MonoBehaviour {
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PolygonCollider2D polygonCollider;

    private Item item;
    private float outlineThickness = 2f;
    private Material instanceMaterial;
    private Player player;

    void Awake() {
        instanceMaterial = spriteRenderer.material;
        instanceMaterial.SetColor("_Color", Color.white);
        SetOutline(0f);
    }

    public void Initialize(Item inventoryItem) {
        item = inventoryItem;

        if (item?.data == null) return;

        spriteRenderer.sprite = item.data.icon;

        AdjustCollider();
    }

    void AdjustCollider() {
        Destroy(polygonCollider);
        polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
    }

    void SetOutline(float value) {
        instanceMaterial.SetFloat("_Thickness", value);
    }

    public void SetHighlight(bool value) {
        SetOutline(value ? outlineThickness : 0f);
    }

    public void Interact() {
        if (player != null && player.PickupItem(item)) {
            Destroy(gameObject);
        }
    }

    void OnEnable() {
        GameManager.Instance.SubscribeToPlayerReady(HandlePlayerReady);
    }

    void OnDisable() {
        GameManager.Instance.UnsubscribeFromPlayerReady(HandlePlayerReady);
    }

    private void HandlePlayerReady(Player p) {
        player = p;
    }

    public Item GetItemData() => item;
}
