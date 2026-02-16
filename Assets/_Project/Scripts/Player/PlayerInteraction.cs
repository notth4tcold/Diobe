using UnityEngine;

public class PlayerInteraction : MonoBehaviour {
    [SerializeField] private PlayerInputHandler input;

    private ItemPickup currentItem;

    void Awake() {
        input.OnInteractPressed += HandleInteract;
    }

    private void HandleInteract() {
        if (UIManager.Instance.IsUIBlocking) return;

        if (currentItem != null) {
            currentItem.Interact();
            currentItem = null;
        }
    }

    void OnDestroy() {
        input.OnInteractPressed -= HandleInteract;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        ItemPickup item = other.GetComponentInParent<ItemPickup>();
        if (item != null) {
            currentItem = item;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        ItemPickup item = other.GetComponentInParent<ItemPickup>();
        if (item != null && item == currentItem) {
            currentItem = null;
        }
    }
}
