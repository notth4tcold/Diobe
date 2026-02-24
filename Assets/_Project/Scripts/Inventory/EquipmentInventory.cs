using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : MonoBehaviour {
    private Dictionary<EquipmentType, InventoryItem> slots = new Dictionary<EquipmentType, InventoryItem>();
    private InventoryItem ring1;
    private InventoryItem ring2;

    public static event Action<InventoryItem> OnItemUnequipped;
    public static event Action<InventoryItem> OnItemEquipped;

    void Awake() {
        foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType))) {
            if (type == EquipmentType.None || type == EquipmentType.Ring) continue;
            slots[type] = null;
        }
    }

    public bool CanEquipItem(InventoryItem item) {
        if (item.data.type != ItemType.Weapon && item.data.type != ItemType.Armor) return false;
        if (item.data.equipmentType == EquipmentType.None) return false;

        return true;
    }

    public bool EquipNewItem(InventoryItem item) {
        if (EquipItem(item)) {
            OnItemEquipped?.Invoke(item);
            return true;
        }

        return false;
    }

    public bool EquipItem(InventoryItem item) {
        if (!CanEquipItem(item)) return false;

        EquipmentType type = item.data.equipmentType;
        if (type == EquipmentType.Ring) return false;

        if (slots[type] != null && slots[type] != item) {
            var oldItem = slots[type];
            slots[type] = null;

            OnItemUnequipped?.Invoke(oldItem);
            ReturnItemToInventory(oldItem);
        }

        slots[type] = item;
        if (type == EquipmentType.MainHand) LevelManager.Instance.GetPlayer().EquipWeapon(item);

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

                    OnItemUnequipped?.Invoke(oldItem);
                    ReturnItemToInventory(oldItem);
                }
                ring1 = item;
                break;

            case RingSlot.Ring2:
                if (ring2 != null && ring2 != item) {
                    var oldItem = ring2;
                    ring2 = null;

                    OnItemUnequipped?.Invoke(oldItem);
                    ReturnItemToInventory(oldItem);
                }
                ring2 = item;
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
        if (type == EquipmentType.MainHand) LevelManager.Instance.GetPlayer().UnequipWeapon();
    }

    public void UnequipRingInSlot(InventoryItem item, RingSlot slot) {
        if (item.data.equipmentType != EquipmentType.Ring) return;

        switch (slot) {
            case RingSlot.Ring1:
                if (ring1 == item) ring1 = null;
                else return;
                break;

            case RingSlot.Ring2:
                if (ring2 == item) ring2 = null;
                else return;
                break;
        }
    }

    private void ReturnItemToInventory(InventoryItem item) {
        if (GameManager.Instance.SpawnNewItem(item)) {
        } else {
            GameManager.Instance.DropItem(item);
        }
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
}
