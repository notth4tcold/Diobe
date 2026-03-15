using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifierGeneratorData", menuName = "Scriptable Objects/ModifierGeneratorData")]
public class ModifierGeneratorData : ScriptableObject {
    public EquipmentType equipmentType;

    public int maxModifiers = 4;

    public List<StatDefinition> possibleStats = new();

    public List<ItemModifier> Generate(int itemLevel, LootQuality quality, ModifierTierDatabase tierDatabase) {
        List<ItemModifier> result = new();

        var tierData = tierDatabase.GetTierData(itemLevel);
        if (tierData == null) return result;

        int count = Random.Range(1, maxModifiers + 1);

        HashSet<StatDefinition> usedStats = new();

        for (int i = 0; i < count; i++) {
            var stat = RollWeighted(possibleStats);
            if (stat == null || usedStats.Contains(stat)) continue;

            usedStats.Add(stat);

            float value = stat.RollValue(itemLevel, quality, tierData.tier);

            result.Add(new ItemModifier(stat, value));
        }

        return result;
    }

    StatDefinition RollWeighted(List<StatDefinition> stats) {
        int totalWeight = 0;

        foreach (var s in stats) totalWeight += s.GetWeight();
        if (totalWeight <= 0) return null;

        int roll = Random.Range(0, totalWeight);

        foreach (var s in stats) {
            roll -= s.GetWeight();
            if (roll < 0) return s;
        }

        return null;
    }
}
