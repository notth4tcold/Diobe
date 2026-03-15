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