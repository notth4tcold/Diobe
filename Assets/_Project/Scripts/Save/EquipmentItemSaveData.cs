using System.Collections.Generic;

[System.Serializable]
public class EquipmentItemSaveData {
    public string itemId;
    public RingSlot slot;
    public int itemLevel;
    public List<ItemModifier> modifiers = new();
}
