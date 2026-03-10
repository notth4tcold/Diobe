
using System;
using System.Collections.Generic;

[Serializable]
public class CharacterSaveData {
    public string id;
    public string playerName;
    public CharacterClass characterClass;

    public int money;
    public int level;
    public int exp;

    public List<StatValue> baseStats = new();
    public int health;
    public int mana;

    public DateTime lastSave;

    public List<InventoryItemSaveData> items = new();
    public List<EquipmentItemSaveData> equipments = new();
}

[Serializable]
public class InventoryItemSaveData {
    public string itemId;
    public int x;
    public int y;
    public int itemLevel;
    public List<ItemModifier> modifiers = new();
}

[Serializable]
public class EquipmentItemSaveData {
    public string itemId;
    public RingSlot slot;
    public int itemLevel;
    public List<ItemModifier> modifiers = new();
}

[Serializable]
public struct StatValue {
    public StatType stat;
    public float value;
}