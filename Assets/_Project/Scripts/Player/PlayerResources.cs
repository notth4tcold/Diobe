using UnityEngine;

[System.Serializable]
public class PlayerResources {
    private PlayerStats stats;

    public void SetStats(PlayerStats playerStats) {
        stats = playerStats;
    }

    public int health;
    public int mana;

    public int baseHealth = 50;
    public int baseMana = 30;

    public float vitalityMultiplier = 10f;
    public float intelligenceMultiplier = 5f;

    public float baseHealthRegenPercent = 0.01f; // 1% por segundo
    public float baseManaRegenPercent = 0.1f; // 10% por segundo

    public float vitalityRegenMultiplier = 0.0001f; // +0.01% crit por ponto (1% a cada 100 VIT)
    public float intelligenceRegenMultiplier = 0.0001f; // +0.01% crit por ponto (1% a cada 100 INT)

    public int MaxHealth =>
        baseHealth +
        Mathf.RoundToInt(stats.Get(StatType.Vitality) * vitalityMultiplier) +
        Mathf.RoundToInt(stats.Get(StatType.MaxHealth));

    public int MaxMana =>
        baseMana +
        Mathf.RoundToInt(stats.Get(StatType.Intelligence) * intelligenceMultiplier) +
        Mathf.RoundToInt(stats.Get(StatType.MaxMana));

    public float HealthRegenPercent =>
        baseHealthRegenPercent +
        stats.Get(StatType.Vitality) * vitalityRegenMultiplier +
        stats.Get(StatType.HealthRegen);

    public float ManaRegenPercent =>
        baseManaRegenPercent +
        stats.Get(StatType.Intelligence) * intelligenceRegenMultiplier +
        stats.Get(StatType.ManaRegen);

    public float HealthRegen => MaxHealth * HealthRegenPercent;

    public float ManaRegen => MaxMana * ManaRegenPercent;
}