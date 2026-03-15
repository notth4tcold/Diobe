using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentUI : MonoBehaviour, IItemContainerUI {
    [Header("References")]
    [SerializeField] private Transform itemsParent;
    [SerializeField] private ItemUI itemPrefab;
    [SerializeField] private EquipmentSlotUI[] slots;

    private Dictionary<Item, ItemUI> itemToUI = new();
    private bool isReceivingItem;
    private Vector2 defaultInventoryCelltSize;
    private Player player;

    void Initialize() {
        defaultInventoryCelltSize = UIManager.Instance.DefaultInventoryCelltSize;
        BuildFromEquipment();
    }

    public void BuildFromEquipment() {
        ClearUI();

        foreach (var slot in slots) {
            Item item = GetItemFromSlot(slot);
            if (item != null) CreateInventoryItemUI(item, slot);
        }
    }

    private void CreateInventoryItemUI(Item item, EquipmentSlotUI slot) {
        ItemUI itemUI = Instantiate(itemPrefab, itemsParent);
        itemUI.Init(item, this);
        itemToUI[item] = itemUI;
        SnapToSlot(itemUI, slot);
    }

    public void SnapToSlot(ItemUI itemUI, EquipmentSlotUI slot) {
        RectTransform slotRect = slot.GetComponent<RectTransform>();
        if (slotRect == null) return;

        itemUI.Rect.SetParent(itemsParent);
        itemUI.Rect.sizeDelta = new Vector2(
            slotRect.sizeDelta.x,
            slotRect.sizeDelta.y
        );
        itemUI.Rect.anchoredPosition = slotRect.anchoredPosition + new Vector2(-slotRect.sizeDelta.x * 0.5f, slotRect.sizeDelta.y * 0.5f);
    }

    public void PreviewPlacement(Item item, PointerEventData eventData) {
        ClearPreview();

        EquipmentSlotUI slot = GetSlotUnderMouse(eventData);
        if (slot == null) return;

        bool valid = player.EquipmentInventory.CanEquipItem(item, player.level);
        slot.SetHighlight(valid ? Color.green : Color.red);
    }

    private Item GetItemFromSlot(EquipmentSlotUI slot) {
        if (slot.SlotType == EquipmentType.Ring) {
            switch (slot.RingSlot) {
                case RingSlot.Ring1: return player.EquipmentInventory.GetRing1();
                case RingSlot.Ring2: return player.EquipmentInventory.GetRing2();
            }
        }

        return player.EquipmentInventory.GetItem(slot.SlotType);
    }

    private EquipmentSlotUI GetSlotFromItem(Item item) {
        foreach (var slot in slots) {
            if (slot.SlotType == EquipmentType.Ring) {
                if (slot.RingSlot == RingSlot.Ring1 && player.EquipmentInventory.GetRing1() == item) return slot;
                if (slot.RingSlot == RingSlot.Ring2 && player.EquipmentInventory.GetRing2() == item) return slot;
            } else {
                if (player.EquipmentInventory.GetItem(slot.SlotType) == item) return slot;
            }
        }

        return null;
    }

    private EquipmentSlotUI GetSlotUnderMouse(PointerEventData eventData) {
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results) {
            var slot = result.gameObject.GetComponentInParent<EquipmentSlotUI>();
            if (slot != null) return slot;
        }

        return null;
    }

    private void ClearUI() {
        foreach (Transform child in itemsParent) Destroy(child.gameObject);
        itemToUI.Clear();
    }

    public void ClearPreview() {
        foreach (var slot in slots) slot.ClearHighlight();
    }

    public void OnBeginDrag(ItemUI itemUI, PointerEventData eventData) {
        AudioManager.Instance.PlaySFX(SFX.UIUnequipItem);

        EquipmentSlotUI slot = GetSlotFromItem(itemUI.Item);
        if (slot == null) return;
        RectTransform slotRect = slot.GetComponent<RectTransform>();
        if (slotRect == null) return;

        itemUI.Rect.sizeDelta = new Vector2(
            itemUI.Item.Width * defaultInventoryCelltSize.x,
            itemUI.Item.Height * defaultInventoryCelltSize.y
        );
    }

    public void OnDrag(ItemUI itemUI, PointerEventData eventData) {
        itemUI.Rect.position = eventData.position + new Vector2(-itemUI.Rect.sizeDelta.x * 0.5f, itemUI.Rect.sizeDelta.y * 0.5f);
    }

    public bool CanReceiveItem(ItemUI itemUI, PointerEventData eventData) {
        var slot = GetSlotUnderMouse(eventData);
        if (slot == null) return false;

        return player.EquipmentInventory.CanEquipItem(itemUI.Item, player.level);
    }

    public void ReceiveItem(ItemUI itemUI, PointerEventData eventData) {
        var slot = GetSlotUnderMouse(eventData);
        if (slot == null) return;

        isReceivingItem = true;
        if (slot.SlotType == EquipmentType.Ring) player.EquipmentInventory.EquipRingInSlot(itemUI.Item, slot.RingSlot, player.level);
        else player.EquipmentInventory.EquipItem(itemUI.Item, player.level);
        isReceivingItem = false;

        itemToUI[itemUI.Item] = itemUI;
        SnapToSlot(itemUI, slot);

        AudioManager.Instance.PlaySFX(SFX.UIEquipItem);
    }

    public void CancelDrag(ItemUI itemUI, PointerEventData eventData) {
        EquipmentSlotUI slot = GetSlotFromItem(itemUI.Item);
        if (slot == null) return;

        SnapToSlot(itemUI, slot);
    }

    public void DetachItem(ItemUI itemUI) {
        EquipmentSlotUI slot = GetSlotFromItem(itemUI.Item);
        if (slot == null) return;

        if (slot.SlotType == EquipmentType.Ring) player.EquipmentInventory.UnequipRingInSlot(itemUI.Item, slot.RingSlot);
        else player.EquipmentInventory.UnequipItem(itemUI.Item);
        itemToUI.Remove(itemUI.Item);
    }

    public void RemoveItem(ItemUI itemUI) {
        if (itemToUI.TryGetValue(itemUI.Item, out var ui)) {
            DetachItem(itemUI);
            Destroy(ui.gameObject);
        }
    }

    public void ShowVisualState(ItemUI itemUI, PointerEventData eventData) {
        PreviewPlacement(itemUI.Item, eventData);
    }


    public void ClearVisualState(ItemUI itemUI) {
        ClearPreview();
    }

    void OnEnable() {
        EquipmentSlots.OnItemEquipped += HandleItemEquipped;
        EquipmentSlots.OnItemRemoved += HandleItemRemoved;
        GameManager.Instance.SubscribeToPlayerReady(HandlePlayerReady);
    }

    void OnDisable() {
        EquipmentSlots.OnItemEquipped -= HandleItemEquipped;
        EquipmentSlots.OnItemRemoved -= HandleItemRemoved;
        GameManager.Instance.UnsubscribeFromPlayerReady(HandlePlayerReady);
    }

    void HandleItemEquipped(Item item) {
        if (isReceivingItem) return;

        EquipmentSlotUI slot = GetSlotFromItem(item);
        CreateInventoryItemUI(item, slot);
    }

    void HandleItemRemoved(Item item) {
        RemoveItem(itemToUI[item]);
    }

    private void HandlePlayerReady(Player p) {
        if (player != null) return;
        player = p;
        Initialize();
    }
}
