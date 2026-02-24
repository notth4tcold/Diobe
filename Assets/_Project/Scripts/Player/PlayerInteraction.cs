using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {
    [SerializeField] private PlayerInputHandler input;

    private List<ItemPickup> itemsInRange = new();

    void Awake() {
        input.OnInteractPressed += HandleInteract;
    }

    private void HandleInteract() {
        if (UIManager.Instance.IsUIBlocking) return;

        if (itemsInRange.Count == 0) return;

        ItemPickup closest = GetClosestItem();
        closest.Interact();

        itemsInRange.Remove(closest);
    }

    private ItemPickup GetClosestItem() {
        return itemsInRange.OrderBy(item => Vector2.Distance(transform.position, item.transform.position)).FirstOrDefault();
    }

    void OnDestroy() {
        input.OnInteractPressed -= HandleInteract;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        ItemPickup item = other.GetComponentInParent<ItemPickup>();
        if (item != null && !itemsInRange.Contains(item)) {
            itemsInRange.Add(item);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        ItemPickup item = other.GetComponentInParent<ItemPickup>();
        if (item != null) {
            itemsInRange.Remove(item);
        }
    }
}
