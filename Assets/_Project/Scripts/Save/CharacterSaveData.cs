
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