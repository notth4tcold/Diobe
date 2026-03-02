using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentUI : MonoBehaviour, IItemContainerUI {
    [Header("References")]
    [SerializeField] private Transform itemsParent;
    [SerializeField] private InventoryItemUI itemPrefab;
    [SerializeField] private EquipmentSlotUI[] slots;

    private Dictionary<InventoryItem, InventoryItemUI> itemToUI = new();
    private EquipmentInventory equipmentInventory;
    private bool isReceivingItem;
    private Vector2 defaultInventoryCelltSize;

    void Initialize() {
        defaultInventoryCelltSize = UIManager.Instance.DefaultInventoryCelltSize;
        BuildFromEquipment();
    }

    public void BuildFromEquipment() {
        ClearUI();

        foreach (var slot in slots) {
            InventoryItem item = GetItemFromSlot(slot);
            if (item != null) CreateInventoryItemUI(item, slot);
        }
    }

    private void CreateInventoryItemUI(InventoryItem item, EquipmentSlotUI slot) {
        InventoryItemUI itemUI = Instantiate(itemPrefab, itemsParent);
        itemUI.Init(item, this);
        itemToUI[item] = itemUI;
        SnapToSlot(itemUI, slot);
    }

    public void SnapToSlot(InventoryItemUI itemUI, EquipmentSlotUI slot) {
        RectTransform slotRect = slot.GetComponent<RectTransform>();
        if (slotRect == null) return;

        itemUI.Rect.SetParent(itemsParent);
        itemUI.Rect.sizeDelta = new Vector2(
            slotRect.sizeDelta.x,
            slotRect.sizeDelta.y
        );
        itemUI.Rect.anchoredPosition = slotRect.anchoredPosition + new Vector2(-slotRect.sizeDelta.x * 0.5f, slotRect.sizeDelta.y * 0.5f);
    }

    public void PreviewPlacement(InventoryItem item, PointerEventData eventData) {
        ClearPreview();

        EquipmentSlotUI slot = GetSlotUnderMouse(eventData);
        if (slot == null) return;

        bool valid = item.data.equipmentType == slot.SlotType;
        slot.SetHighlight(valid ? Color.green : Color.red);
    }

    private InventoryItem GetItemFromSlot(EquipmentSlotUI slot) {
        if (slot.SlotType == EquipmentType.Ring) {
            switch (slot.RingSlot) {
                case RingSlot.Ring1: return equipmentInventory.GetRing1();
                case RingSlot.Ring2: return equipmentInventory.GetRing2();
            }
        }

        return equipmentInventory.GetItem(slot.SlotType);
    }

    private EquipmentSlotUI GetSlotFromItem(InventoryItem item) {
        foreach (var slot in slots) {
            if (slot.SlotType == EquipmentType.Ring) {
                if (slot.RingSlot == RingSlot.Ring1 && equipmentInventory.GetRing1() == item) return slot;
                if (slot.RingSlot == RingSlot.Ring2 && equipmentInventory.GetRing2() == item) return slot;
            } else {
                if (equipmentInventory.GetItem(slot.SlotType) == item) return slot;
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

    public void OnBeginDrag(InventoryItemUI itemUI, PointerEventData eventData) {
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

    public void OnDrag(InventoryItemUI itemUI, PointerEventData eventData) {
        itemUI.Rect.position = eventData.position + new Vector2(-itemUI.Rect.sizeDelta.x * 0.5f, itemUI.Rect.sizeDelta.y * 0.5f);
    }

    public bool CanReceiveItem(InventoryItemUI itemUI, PointerEventData eventData) {
        var slot = GetSlotUnderMouse(eventData);
        if (slot == null) return false;

        return itemUI.Item.data.equipmentType == slot.SlotType;
    }

    public void ReceiveItem(InventoryItemUI itemUI, PointerEventData eventData) {
        var slot = GetSlotUnderMouse(eventData);
        if (slot == null) return;

        isReceivingItem = true;
        if (slot.SlotType == EquipmentType.Ring) equipmentInventory.EquipRingInSlot(itemUI.Item, slot.RingSlot);
        else equipmentInventory.EquipItem(itemUI.Item);
        isReceivingItem = false;

        itemToUI[itemUI.Item] = itemUI;
        SnapToSlot(itemUI, slot);

        AudioManager.Instance.PlaySFX(SFX.UIEquipItem);
    }

    public void CancelDrag(InventoryItemUI itemUI, PointerEventData eventData) {
        EquipmentSlotUI slot = GetSlotFromItem(itemUI.Item);
        if (slot == null) return;

        SnapToSlot(itemUI, slot);
    }

    public void DetachItem(InventoryItemUI itemUI) {
        EquipmentSlotUI slot = GetSlotFromItem(itemUI.Item);
        if (slot == null) return;

        if (slot.SlotType == EquipmentType.Ring) equipmentInventory.UnequipRingInSlot(itemUI.Item, slot.RingSlot);
        else equipmentInventory.UnequipItem(itemUI.Item);
        itemToUI.Remove(itemUI.Item);
    }

    public void RemoveItem(InventoryItemUI itemUI) {
        if (itemToUI.TryGetValue(itemUI.Item, out var ui)) {
            DetachItem(itemUI);
            Destroy(ui.gameObject);
        }
    }

    public void ShowVisualState(InventoryItemUI itemUI, PointerEventData eventData) {
        PreviewPlacement(itemUI.Item, eventData);
    }


    public void ClearVisualState(InventoryItemUI itemUI) {
        ClearPreview();
    }

    void OnEnable() {
        EquipmentInventory.OnItemEquipped += HandleItemEquipped;
        EquipmentInventory.OnItemRemoved += HandleItemRemoved;
        GameManager.Instance.SubscribeToPlayerReady(HandlePlayerReady);
    }

    void OnDisable() {
        EquipmentInventory.OnItemEquipped -= HandleItemEquipped;
        EquipmentInventory.OnItemRemoved -= HandleItemRemoved;
        GameManager.Instance.UnsubscribeFromPlayerReady(HandlePlayerReady);
    }

    void HandleItemEquipped(InventoryItem item) {
        if (isReceivingItem) return;

        EquipmentSlotUI slot = GetSlotFromItem(item);
        CreateInventoryItemUI(item, slot);
    }

    void HandleItemRemoved(InventoryItem item) {
        RemoveItem(itemToUI[item]);
    }

    private void HandlePlayerReady(Player p) {
        if (equipmentInventory != null) return;
        equipmentInventory = p.EquipmentInventory;
        Initialize();
    }
}
