using UnityEngine;

[System.Serializable]
public class PlayerResources {
    public int health;
    public int mana;

    public int baseHealth = 50;
    public int baseMana = 30;
    public float vitalityMultiplier = 10f;
    public float intelligenceMultiplier = 5f;

    private PlayerStats stats;

    public void Initialize(PlayerStats playerStats) {
        stats = playerStats;
    }

    public int MaxHealth => baseHealth + Mathf.RoundToInt(stats.vitality * vitalityMultiplier);
    public int MaxMana => baseMana + Mathf.RoundToInt(stats.intelligence * intelligenceMultiplier);
    public float HealthRegen => MaxHealth * 0.1f;
    public float ManaRegen => MaxMana * 0.1f;
}