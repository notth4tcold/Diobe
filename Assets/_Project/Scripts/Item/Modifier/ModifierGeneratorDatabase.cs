using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifierGeneratorDatabase", menuName = "Scriptable Objects/ModifierGeneratorDatabase")]
public class ModifierGeneratorDatabase : ScriptableObject {
    public ModifierTierDatabase tierDatabase;

    public List<ModifierGeneratorData> generators = new();

    Dictionary<EquipmentType, ModifierGeneratorData> lookup;

    void Init() {
        if (lookup != null) return;
        lookup = new();

        foreach (var generator in generators) {
            if (generator == null) continue;

            if (lookup.ContainsKey(generator.equipmentType)) {
                Debug.LogError($"Generator duplicated for {generator.equipmentType}", this);
                continue;
            }

            lookup.Add(generator.equipmentType, generator);
        }
    }

    public List<ItemModifier> Generate(EquipmentType type, int itemLevel, LootQuality quality) {
        Init();

        if (!lookup.TryGetValue(type, out var generator)) {
            Debug.LogWarning($"No modifier generator for {type}", this);
            return new List<ItemModifier>();
        }

        // TODO ver Prefix / Suffix affixes E permitir cores
        return generator.Generate(itemLevel, quality, tierDatabase);
    }

    public int GenerateItemLevel(int playerLevel, LootQuality quality) {
        if (tierDatabase == null) {
            Debug.LogError("ModifierTierDatabase is missing", this);
            return playerLevel;
        }

        var tier = tierDatabase.GetTierData(playerLevel);

        if (tier == null) {
            Debug.LogWarning($"No tier found for player level {playerLevel}", this);
            return playerLevel;
        }

        return tier.RollItemLevel(playerLevel, quality);
    }
}