using UnityEngine;

public class ItemPickup : MonoBehaviour {
    [SerializeField] private ItemData itemData;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PolygonCollider2D polygonCollider;

    private float outlineThickness = 2f;
    private Material instanceMaterial;
    private bool playerNearby;
    private Player player;

    void Awake() {
        instanceMaterial = spriteRenderer.material;
        instanceMaterial.SetColor("_Color", Color.white);
        SetOutline(0f);

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
    }

    void SetOutline(float value) {
        instanceMaterial.SetFloat("_Thickness", value);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerNearby = true;
            SetOutline(outlineThickness);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerNearby = false;
            SetOutline(0f);
        }
    }

    public void Interact() {
        if (player != null && player.PickupItem(itemData)) {
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

    public ItemData GetItemData() => itemData;
    public bool IsNearby() => playerNearby;
}
