using System.Collections.Generic;

public static class StatDefinitions {
    private static readonly HashSet<StatType> PercentStats = new()
    {
        // Resources
        StatType.HealthRegen,
        StatType.ManaRegen,

        // Defense
        StatType.FireRes,
        StatType.IceRes,
        StatType.LightningRes,
        StatType.PoisonRes,
        StatType.MagicRes,
        StatType.AllRes,

        // Attack
        StatType.AttackSpeed,
        StatType.HitChance,
        StatType.CritChance,

        // Utility
        StatType.MoveSpeed,
        StatType.DodgeChance,
        StatType.BlockChance,
    };

    public static bool IsPercent(StatType stat) {
        return PercentStats.Contains(stat);
    }
}

public enum StatType {
    // Primary Attributes
    Strength,       // Aumenta dano físico, aumentar armadura?, Requisitos de armas pesadas,
    Dexterity,      // Aumenta chance de acerto, Chance de esquiva, Chance de crítico, Requisitos de armas leves
    Intelligence,   // Aumenta dano mágico, Mana maxima, Regeneração de Mana, Poder de feitiços
    Vitality,       // Vida máxima, Regeneração de vida

    // Resources
    MaxHealth,
    MaxMana,
    HealthRegen,
    ManaRegen,

    // Defense
    Armor,
    FireRes,
    IceRes,
    LightningRes,
    PoisonRes,
    MagicRes,       //resistência a dano mágico puro, não elemental
    AllRes,

    // Attack
    WeaponDamage,
    FireDamage,
    IceDamage,
    LightningDamage,
    PoisonDamage,

    AttackSpeed,
    HitChance,
    CritChance,
    CritMultiplier,

    // Utility
    MoveSpeed,
    DodgeChance,
    BlockChance,
}