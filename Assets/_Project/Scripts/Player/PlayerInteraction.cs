using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {
    [SerializeField] private PlayerInputHandler input;

    private List<ItemPickup> itemsInRange = new();
    private ItemPickup currentClosest;

    void Awake() {
        input.OnInteractPressed += HandleInteract;
    }

    void Update() {
        if (UIManager.Instance.IsUIBlocking) {
            HideCurrent();
            return;
        }

        UpdateClosestItem();
    }

    private void HandleInteract() {
        if (UIManager.Instance.IsUIBlocking) return;
        if (currentClosest == null) return;

        currentClosest.Interact();
    }

    private void UpdateClosestItem() {
        if (itemsInRange.Count == 0) {
            HideCurrent();
            return;
        }

        ItemPickup newClosest = GetClosestItem();

        if (newClosest == currentClosest) return;

        HideCurrent();
        currentClosest = newClosest;

        currentClosest.SetHighlight(true);
        ItemTooltipUI.Instance.ShowWorld(currentClosest.GetItemData().GetTooltip(), currentClosest.transform);
    }

    private ItemPickup GetClosestItem() {
        ItemPickup best = null;
        float bestDist = float.MaxValue;

        foreach (var item in itemsInRange) {
            if (item == null) continue;

            float dist = (item.transform.position - transform.position).sqrMagnitude;

            if (dist < bestDist) {
                best = item;
                bestDist = dist;
            }
        }

        return best;
    }

    private void HideCurrent() {
        if (currentClosest == null) return;

        currentClosest.SetHighlight(false);
        currentClosest = null;
        ItemTooltipUI.Instance.Hide();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        ItemPickup item = other.GetComponentInParent<ItemPickup>();

        if (item == null || itemsInRange.Contains(item)) return;
        itemsInRange.Add(item);
    }

    private void OnTriggerExit2D(Collider2D other) {
        ItemPickup item = other.GetComponentInParent<ItemPickup>();

        if (item == null) return;
        itemsInRange.Remove(item);
    }

    void OnDestroy() {
        input.OnInteractPressed -= HandleInteract;
    }
}
