[System.Serializable]
public class ItemModifier {
    public StatType stat;
    public float value;

    public ItemModifier(StatType stat, float value) {
        this.stat = stat;
        this.value = value;
    }

    public bool IsPercent() => StatDefinitions.IsPercent(stat);
}