using System;
using System.Collections.Generic;

[Serializable]
public class PlayerStats {
    Dictionary<StatType, float> baseStats = new();
    Dictionary<StatType, float> equipmentStats = new();
    Dictionary<StatType, float> buffStats = new();

    Dictionary<StatType, float> finalStats = new();

    public event Action OnStatsChanged;

    public PlayerStats() {
        Initialize();
    }

    public void Initialize() {
        foreach (StatType stat in Enum.GetValues(typeof(StatType))) {
            baseStats[stat] = 0;
            equipmentStats[stat] = 0;
            buffStats[stat] = 0;
            finalStats[stat] = 0;
        }

        Recalculate();
    }

    public float Get(StatType stat) {
        return finalStats[stat];
    }

    public void SetBase(StatType stat, float value) {
        baseStats[stat] = value;
        Recalculate();
    }

    public void AddEquipment(StatType stat, float value) {
        equipmentStats[stat] += value;
        Recalculate();
    }

    public void RemoveEquipment(StatType stat, float value) {
        equipmentStats[stat] -= value;
        Recalculate();
    }

    public void AddBuff(StatType stat, float value) {
        buffStats[stat] += value;
        Recalculate();
    }

    public void RemoveBuff(StatType stat, float value) {
        buffStats[stat] -= value;
        Recalculate();
    }

    void Recalculate() {
        foreach (StatType stat in Enum.GetValues(typeof(StatType))) {
            finalStats[stat] =
                baseStats[stat] +
                equipmentStats[stat] +
                buffStats[stat];
        }

        OnStatsChanged?.Invoke();
    }

    public List<StatValue> BuildBaseStatsSaveData() {
        List<StatValue> list = new();

        foreach (var kv in baseStats) {
            list.Add(new StatValue {
                stat = kv.Key,
                value = kv.Value
            });
        }

        return list;
    }
}