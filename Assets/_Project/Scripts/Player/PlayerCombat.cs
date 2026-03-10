using UnityEngine;

[System.Serializable]
public class PlayerCombat {
    private PlayerStats stats;

    public void SetStats(PlayerStats stats) {
        this.stats = stats;
    }

    // ----------------
    // DEFENSE
    // ----------------

    public int Armor => Mathf.RoundToInt(stats.Get(StatType.Armor));

    public float baseDodgeChance = 0.01f; //1% de chance de desviar
    public float dexterityDodgeMultiplier = 0.0005f; // +0.05% dodge por ponto (5% a cada 100 DEX)

    public float DodgeChance =>
        baseDodgeChance +
        stats.Get(StatType.Dexterity) * dexterityDodgeMultiplier +
        stats.Get(StatType.DodgeChance);

    public bool RollDodge() => Random.value <= Mathf.Clamp01(DodgeChance);

    public float BlockChance => stats.Get(StatType.BlockChance);

    public bool RollBlock() => Random.value <= Mathf.Clamp01(BlockChance);

    public int FireRes => Mathf.RoundToInt(stats.Get(StatType.FireRes) + stats.Get(StatType.AllRes));

    public int IceRes => Mathf.RoundToInt(stats.Get(StatType.IceRes) + stats.Get(StatType.AllRes));

    public int LightningRes => Mathf.RoundToInt(stats.Get(StatType.LightningRes) + stats.Get(StatType.AllRes));

    public int PoisonRes => Mathf.RoundToInt(stats.Get(StatType.PoisonRes) + stats.Get(StatType.AllRes));

    public int MagicRes => Mathf.RoundToInt(stats.Get(StatType.MagicRes) + stats.Get(StatType.AllRes));


    // ----------------
    // ATTACK
    // ----------------

    public int WeaponDamage => Mathf.RoundToInt(stats.Get(StatType.WeaponDamage));

    public int FireDamage => Mathf.RoundToInt(stats.Get(StatType.FireDamage));

    public int IceDamage => Mathf.RoundToInt(stats.Get(StatType.IceDamage));

    public int LightningDamage => Mathf.RoundToInt(stats.Get(StatType.LightningDamage));

    public int PoisonDamage => Mathf.RoundToInt(stats.Get(StatType.PoisonDamage));

    public float baseAttackSpeed = 1f;
    public float AttackSpeed => baseAttackSpeed * (1f + stats.Get(StatType.AttackSpeed));

    public float baseHitChance = 0.7f; //70% de chance de acertar o hit
    public float dexterityHitMultiplier = 0.002f; // +0.2% hit por ponto (20% a cada 100 DEX)

    public float HitChance =>
        baseHitChance +
        stats.Get(StatType.Dexterity) * dexterityHitMultiplier +
        stats.Get(StatType.HitChance);

    bool RollHit() => Random.value <= Mathf.Clamp01(HitChance);

    public float baseCritChance = 0.05f; //5% de chance de critico
    public float dexterityCritMultiplier = 0.0005f; // +0.05% crit por ponto (5% a cada 100 DEX)

    public float CritChance =>
        baseCritChance +
        stats.Get(StatType.Dexterity) * dexterityCritMultiplier +
        stats.Get(StatType.CritChance);

    bool RollCrit() => Random.value <= Mathf.Clamp01(CritChance);

    public float baseCritMultiplier = 2f;

    public float CritMultiplier => baseCritMultiplier + stats.Get(StatType.CritMultiplier);


    // ----------------
    // FINAL DAMAGE
    // ----------------

    public float strengthMultiplier = 1.5f;

    public int Damage => WeaponDamage + Mathf.RoundToInt(stats.Get(StatType.Strength) * strengthMultiplier);

    // ----------------
    // DAMAGE CREATION
    // ----------------

    public DamageInfo CreateDamage() {
        DamageInfo dmg = new();

        dmg.isHit = RollHit();

        if (!dmg.isHit) return dmg;

        dmg.isCrit = RollCrit();

        int physicalDamage = Damage;

        if (dmg.isCrit) physicalDamage = Mathf.RoundToInt(physicalDamage * CritMultiplier);

        dmg.physical = physicalDamage;

        // Elemental damage
        dmg.fire = FireDamage;
        dmg.ice = IceDamage;
        dmg.lightning = LightningDamage;
        dmg.poison = PoisonDamage;

        // Flags de defesa do alvo
        dmg.canDodge = true;
        dmg.canBlock = true;

        return dmg;
    }
}

public struct DamageInfo {
    public int physical;
    public int fire;
    public int ice;
    public int lightning;
    public int poison;
    public int magic;

    public bool isHit;
    public bool isCrit;

    public bool canDodge;
    public bool canBlock;
}