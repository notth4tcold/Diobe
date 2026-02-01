
using System;
using System.Collections.Generic;

[Serializable]
public class CharacterSaveData {
    public string id;
    public string playerName;
    public int level;
    public int exp;
    public DateTime lastSave;
    public CharacterClass characterClass;
    public List<InventoryItemSaveData> items = new();
}

[Serializable]
public enum CharacterClass {
    Warrior,
    Archer,
    Mage
}

[Serializable]
public class InventoryItemSaveData {
    public string itemId;
    public int x;
    public int y;
}