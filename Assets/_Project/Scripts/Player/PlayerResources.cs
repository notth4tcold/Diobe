using UnityEngine;

[System.Serializable]
public class PlayerResources {
    private PlayerStats stats;

    public void Initialize(PlayerStats playerStats) {
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
    public float vitalityRegenMultiplier = 0.0001f; // 1% por segundo cada 100 pontos
    public float intelligenceRegenMultiplier = 0.0001f; // 1% por segundo cada 100 pontos
    public float itemHealthRegenBonus;
    public float itemManaRegenBonus;

    public int MaxHealth => baseHealth + Mathf.RoundToInt(stats.vitality * vitalityMultiplier);
    public int MaxMana => baseMana + Mathf.RoundToInt(stats.intelligence * intelligenceMultiplier);
    public float HealthRegenPercent => baseHealthRegenPercent + stats.vitality * vitalityRegenMultiplier + itemHealthRegenBonus;
    public float HealthRegen => MaxHealth * HealthRegenPercent;
    public float ManaRegenPercent => baseManaRegenPercent + stats.intelligence * intelligenceRegenMultiplier + itemManaRegenBonus;
    public float ManaRegen => MaxMana * ManaRegenPercent;
}