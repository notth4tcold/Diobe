[System.Serializable]
public class ItemModifier {
    public StatDefinition stat;
    public float value;

    public ItemModifier(StatDefinition stat, float value) {
        this.stat = stat;
        this.value = value;
    }

    public bool IsPercent() => stat.isPercent;
}