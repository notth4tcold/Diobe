using System.Collections.Generic;

[System.Serializable]
public class Item {
    public ItemData data;
    public int x;
    public int y;
    public int itemLevel;
    // TODO adicionar valor de venda

    public List<ItemModifier> modifiers = new();

    public Item(ItemData data, int x, int y) {
        this.data = data;
        this.x = x;
        this.y = y;
    }

    public void GenerateModifiers(int playerLevel, LootQuality quality) {
        if (data == null) return;

        itemLevel = GameManager.Instance.GetModifierGeneratorDatabase().GenerateItemLevel(playerLevel, quality);
        modifiers = GameManager.Instance.GetModifierGeneratorDatabase().Generate(data.equipmentType, itemLevel, quality);
    }

    public string GetTooltip() {
        if (data == null) return "Invalid Item";

        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"<b>{data.itemName}</b>");
        sb.AppendLine($"Item Level: {itemLevel}");

        if (modifiers.Count > 0) sb.AppendLine("");

        foreach (var mod in modifiers) {
            string value = mod.value.ToString("0.#");
            if (mod.IsPercent()) sb.AppendLine($"+{value}% {mod.stat.stat}");
            else sb.AppendLine($"+{value} {mod.stat.stat}");
        }

        return sb.ToString();
    }

    public int Width => data.width;
    public int Height => data.height;
}
