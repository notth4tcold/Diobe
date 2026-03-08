using System.Collections.Generic;

[System.Serializable]
public class InventoryItem {
    public ItemData data;
    public int x;
    public int y;
    public int itemLevel;
    public List<ItemModifier> modifiers = new();

    public InventoryItem(ItemData data, int x, int y) {
        this.data = data;
        this.x = x;
        this.y = y;
    }

    public void GenerateModifiers(int level) {
        itemLevel = level;

        int count = UnityEngine.Random.Range(1, 4);

        for (int i = 0; i < count; i++) {
            modifiers.Add(RandomModifier(level));
        }
    }

    ItemModifier RandomModifier(int level) { // TODO adicionar random modifier baseado no tipo do item
        StatType stat = (StatType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(StatType)).Length);

        float value = UnityEngine.Random.Range(1, level + 5);

        return new ItemModifier(stat, value);
    }

    public string GetTooltip() {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"<b>{data.itemName}</b>");
        sb.AppendLine($"Item Level: {itemLevel}");
        sb.AppendLine("");

        foreach (var mod in modifiers) {
            string value = mod.value.ToString("0.#");
            if (mod.IsPercent()) sb.AppendLine($"+{value}% {mod.stat}");
            else sb.AppendLine($"+{value} {mod.stat}");
        }

        return sb.ToString();
    }

    public int Width => data.width;
    public int Height => data.height;
}
