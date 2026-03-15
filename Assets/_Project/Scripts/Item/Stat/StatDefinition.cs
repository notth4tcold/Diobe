using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatDefinition", menuName = "Scriptable Objects/StatDefinition")]
public class StatDefinition : ScriptableObject {
    public StatType stat;

    public bool isPercent;

    public ModifierRarity rarity = ModifierRarity.Common;

    public List<TierValues> tiers = new();

    public float RollValue(int itemLevel, LootQuality quality, ModifierTier tier) {
        var tierData = tiers.Find(t => t.tier == tier);

        if (tierData == null || tierData.values.Count == 0) {
            Debug.LogWarning($"{stat} has no values for tier {tier}");
            return 0;
        }

        float exponent = GetExponent(quality);

        float baseRoll = Mathf.Pow(Random.value, exponent);

        float itemBias = Mathf.InverseLerp(1, 100, itemLevel);

        float roll = Mathf.Lerp(baseRoll, itemBias, 0.35f); //65% RNG & 35% player level

        int index = Mathf.RoundToInt(roll * (tierData.values.Count - 1));

        return tierData.values[index];
    }

    static float GetExponent(LootQuality quality) {
        return quality switch {
            LootQuality.Normal => 2f,
            LootQuality.Elite => 1.3f,
            LootQuality.Boss => 0.8f,
            _ => 2f
        };
    }

    public int GetWeight() {
        return rarity switch {
            ModifierRarity.Common => 100,
            ModifierRarity.Uncommon => 60,
            ModifierRarity.Rare => 30,
            ModifierRarity.VeryRare => 10,
            ModifierRarity.Legendary => 3,
            _ => 100
        };
    }
}