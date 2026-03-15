using UnityEngine;

[CreateAssetMenu(fileName = "ModifierTierData", menuName = "Scriptable Objects/ModifierTierData")]
public class ModifierTierData : ScriptableObject {
    public ModifierTier tier;
    public int minLevel;
    public int maxLevel;

    public bool InRange(int level) {
        return level >= minLevel && level <= maxLevel;
    }

    public int RollItemLevel(int playerLevel, LootQuality quality) {
        float exponent = GetExponent(quality);

        float baseRoll = Mathf.Pow(Random.value, exponent);

        float playerBias = Mathf.InverseLerp(minLevel, maxLevel, playerLevel);

        float roll = Mathf.Lerp(baseRoll, playerBias, 0.35f); //65% RNG & 35% player level

        int range = maxLevel - minLevel;

        int level = minLevel + Mathf.RoundToInt(roll * range);

        return Mathf.Clamp(level, minLevel, maxLevel);
    }

    static float GetExponent(LootQuality quality) {
        return quality switch {
            LootQuality.Normal => 2f,
            LootQuality.Elite => 1.3f,
            LootQuality.Boss => 0.8f,
            _ => 2f
        };
    }
}