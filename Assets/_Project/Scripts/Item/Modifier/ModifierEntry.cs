[System.Serializable]
public class ModifierEntry {
    public StatDefinition stat;

    public int GetWeight() => stat.GetWeight();
    public float RollValue(int itemLevel, LootQuality quality, ModifierTier tier) => stat.RollValue(itemLevel, quality, tier);
}