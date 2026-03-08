[System.Serializable]
public class ItemModifier {
    public StatType stat;
    public float value;

    public ItemModifier(StatType stat, float value) {
        this.stat = stat;
        this.value = value;
    }

    public bool IsPercent() => IsPercent(stat);

    private bool IsPercent(StatType stat) {
        return stat == StatType.DodgeChance ||
                stat == StatType.BlockChance ||
                stat == StatType.FireRes ||
                stat == StatType.IceRes ||
                stat == StatType.LightningRes ||
                stat == StatType.PoisonRes ||
                stat == StatType.MagicRes ||
                stat == StatType.AllRes ||
                stat == StatType.CritChance ||
                stat == StatType.AttackSpeed ||
                stat == StatType.MoveSpeed ||
                stat == StatType.HealthRegen ||
                stat == StatType.ManaRegen;
    }
}

public enum StatType {
    Armor,
    DodgeChance,
    BlockChance,

    FireRes,
    IceRes,
    LightningRes,
    PoisonRes,
    MagicRes,
    AllRes,

    WeaponDamage,
    FireDamage,
    IceDamage,
    LightningDamage,
    PoisonDamage,

    CritChance,
    CritDamage,

    AttackSpeed,
    MoveSpeed,

    Health,
    Mana,
    HealthRegen,
    ManaRegen,

    Strength,
    Dexterity,
    Intelligence,
    Vitality
}