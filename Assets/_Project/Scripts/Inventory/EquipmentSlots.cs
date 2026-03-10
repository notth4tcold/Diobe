using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlots : MonoBehaviour {
    private Dictionary<EquipmentType, InventoryItem> slots = new();
    private InventoryItem ring1;
    private InventoryItem ring2;

    public static event Action<InventoryItem> OnItemEquipped;
    public static event Action<InventoryItem> OnItemUnequipped;
    public static event Action<InventoryItem> OnItemRemoved;

    private Player player;

    public void Initialize() {
        foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType))) {
            if (type == EquipmentType.None || type == EquipmentType.Ring) continue;
            if (!slots.ContainsKey(type)) slots[type] = null;
        }
    }

    public bool CanEquipItem(InventoryItem item) {
        if (item.data.type != ItemType.Weapon && item.data.type != ItemType.Armor) return false;
        if (item.data.equipmentType == EquipmentType.None) return false;

        return true;
    }

    public bool EquipItem(InventoryItem item) {
        if (!CanEquipItem(item)) return false;

        EquipmentType type = item.data.equipmentType;
        if (type == EquipmentType.Ring) return false;

        if (slots[type] != null && slots[type] != item) {
            var oldItem = slots[type];
            slots[type] = null;

            OnItemRemoved?.Invoke(oldItem);
            OnItemUnequipped?.Invoke(oldItem);
            ReturnItemToInventory(oldItem);
        }

        slots[type] = item;
        OnItemEquipped?.Invoke(item);

        return true;
    }

    public bool EquipRingInSlot(InventoryItem item, RingSlot slot) {
        if (!CanEquipItem(item)) return false;
        if (item.data.equipmentType != EquipmentType.Ring) return false;

        switch (slot) {
            case RingSlot.Ring1:
                if (ring1 != null && ring1 != item) {
                    var oldItem = ring1;
                    ring1 = null;

                    OnItemRemoved?.Invoke(oldItem);
                    OnItemUnequipped?.Invoke(oldItem);
                    ReturnItemToInventory(oldItem);
                }
                ring1 = item;
                OnItemEquipped?.Invoke(item);
                break;

            case RingSlot.Ring2:
                if (ring2 != null && ring2 != item) {
                    var oldItem = ring2;
                    ring2 = null;

                    OnItemRemoved?.Invoke(oldItem);
                    OnItemUnequipped?.Invoke(oldItem);
                    ReturnItemToInventory(oldItem);
                }
                ring2 = item;
                OnItemEquipped?.Invoke(item);
                break;
        }

        return true;
    }

    public void UnequipItem(InventoryItem item) {
        EquipmentType type = item.data.equipmentType;
        if (type == EquipmentType.Ring) return;

        if (!slots.ContainsKey(type)) return;
        if (slots[type] != item) return;

        slots[type] = null;
        OnItemUnequipped?.Invoke(item);
    }

    public void UnequipRingInSlot(InventoryItem item, RingSlot slot) {
        if (item.data.equipmentType != EquipmentType.Ring) return;

        switch (slot) {
            case RingSlot.Ring1:
                if (ring1 == item) {
                    ring1 = null;
                    OnItemUnequipped?.Invoke(item);
                } else return;
                break;

            case RingSlot.Ring2:
                if (ring2 == item) {
                    ring2 = null;
                    OnItemUnequipped?.Invoke(item);
                } else return;
                break;
        }
    }

    private void ReturnItemToInventory(InventoryItem item) {
        if (player.SpawnItem(item)) {
        } else player.DropItem(item);
    }

    public void ResetSlots() {
        foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType))) {
            if (type == EquipmentType.None || type == EquipmentType.Ring) continue;
            slots[type] = null;
        }
        ring1 = null;
        ring2 = null;
    }

    public List<EquipmentItemSaveData> BuildSaveData() {
        List<EquipmentItemSaveData> itemsData = new();

        foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType))) {
            if (type == EquipmentType.None || type == EquipmentType.Ring) continue;
            if (slots[type] == null) continue;

            itemsData.Add(new EquipmentItemSaveData {
                itemId = slots[type].data.id,
                slot = RingSlot.None,
                itemLevel = slots[type].itemLevel,
                modifiers = slots[type].modifiers
            });
        }

        if (ring1 != null) {
            itemsData.Add(new EquipmentItemSaveData {
                itemId = ring1.data.id,
                slot = RingSlot.Ring1,
                itemLevel = ring1.itemLevel,
                modifiers = ring1.modifiers
            });
        }

        if (ring2 != null) {
            itemsData.Add(new EquipmentItemSaveData {
                itemId = ring2.data.id,
                slot = RingSlot.Ring2,
                itemLevel = ring2.itemLevel,
                modifiers = ring2.modifiers
            });
        }

        return itemsData;
    }

    public InventoryItem GetItem(EquipmentType type) {
        if (type == EquipmentType.Ring) return null;

        if (slots.ContainsKey(type)) return slots[type];

        return null;
    }

    public InventoryItem GetRing1() => ring1;
    public InventoryItem GetRing2() => ring2;
    public InventoryItem[] GetAllRings() => new InventoryItem[] { ring1, ring2 };

    public bool HasWeapon => slots[EquipmentType.MainHand] != null;

    void OnEnable() {
        GameManager.Instance.SubscribeToPlayerReady(HandlePlayerReady);
    }

    void OnDisable() {
        GameManager.Instance.UnsubscribeFromPlayerReady(HandlePlayerReady);
    }

    private void HandlePlayerReady(Player p) {
        player = p;
    }
}
