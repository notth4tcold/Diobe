
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

    public PlayerStats stats = new();
    public PlayerResources resources = new();
    public PlayerCombat combat = new();

    public DateTime lastSave;

    public List<InventoryItemSaveData> items = new();
}

[Serializable]
public class InventoryItemSaveData {
    public string itemId;
    public int x;
    public int y;
}