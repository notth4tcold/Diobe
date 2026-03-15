using System.Collections.Generic;

[System.Serializable]
public class InventoryItemSaveData {
    public string itemId;
    public int x;
    public int y;
    public int itemLevel;
    public List<ItemModifier> modifiers = new();
}